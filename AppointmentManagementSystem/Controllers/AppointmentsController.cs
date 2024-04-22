using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels;
using AppointmentManagementSystem.Models.ArchiveState;


namespace AppointmentManagementSystem.Controllers;

[Authorize]
//[Authorize(Roles = "User")]
public class AppointmentsController : Controller
{
    private readonly IAppointmentRepository _appointmentRepository;

    private readonly AccountDbContext _accountContext;

    public AppointmentsController(IAppointmentRepository appointmentRepository, AccountDbContext accountContext)
    {
        _appointmentRepository = appointmentRepository;
        _accountContext = accountContext;
    }


    // GET: Appointments
    public async Task<IActionResult> Index(bool? showArchived, string? searchQuery)
    {
        //Check for expired appointments
        _appointmentRepository.CheckForExpiredAppointments();
        

        // Set the viewing archive state in the repository
        if (showArchived.HasValue)
        {
            ArchiveStateSingleton.Instance.IsViewingArchivedAppointments = showArchived.Value;
        }

        // Retrieve the current user's ID
        string userEmail = User.Identity.Name;
        if (userEmail == null)
            return NotFound();

        // Retrieve appointments based on whether to show archived or upcoming appointments
        IEnumerable<Appointment> appointments = ArchiveStateSingleton.Instance.IsViewingArchivedAppointments ?
            _appointmentRepository.AllArchivedAppointments:
            _appointmentRepository.AllUpcomingAppointments;

        // If not admin, filter appointments for the current user
        if (!User.IsInRole("Admin"))
        {
            appointments = appointments.Where(a => a.UserEmail == userEmail);
        }

        // Filter appointments based on search query
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Keeps track of searches
            ViewBag.PreviousSearchQuery = searchQuery;

            // Filters provided appointments depending on a given query.
            // This currently checks for dates and appointment subjects
            appointments = _appointmentRepository.SearchAppointments(searchQuery,appointments);
        }

        // Order appointments by date and time
        appointments = appointments
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToList();

        return View(appointments);
    }

    // GET: Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Retrieve the appointment from the repository
        var appointment = _appointmentRepository.GetAppointmentById(id);
        if (appointment == null)
        {
            return NotFound();
        }

        // Check if the user is unauthorized to view the appointment
        if (!User.IsInRole("Admin") && appointment.UserEmail != User.Identity.Name)
        {
            return Unauthorized();
        }

        // Retrieve the associated user information
        var user = _accountContext.Users.FirstOrDefault(u => u.Email == appointment.UserEmail);

        // If the appointment has no associated user, it won't display anything
        var viewModel = new ViewModel
        {
            UserAppointments = appointment,
            UserDetails = user
        };

        return View(viewModel);
    }

    // GET: Appointments/Create
    public IActionResult Create()
    {
        //var appointment = new Appointment() { UserEmail = User.Identity.Name };
        //return View(appointment);
        return View();
    }

    // POST: Appointments/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] UpcomingAppointment appointment)
    {
        // Clear the errors in case user trys to re-enter information
        ModelState.Clear();
        if (ModelState.IsValid)
        {
            if (_accountContext.Users.FirstOrDefault(u => u.Email == appointment.UserEmail) != null)
            {
                _appointmentRepository.AddAppointment(appointment);
                return RedirectToAction(nameof(Index));
            }
        }

        // If the system admin trys to create an appointment for an unregistered user,
        // we will prompt them to make the user register their details first
        string errorMessage = "No registered email found. " +
                              "Please check the email entered is correct. " +
                              "If so, please ask the user to register an account.";
        // first parameter is the model value the message will be applied to
        // second parameter is the error message to be displayed
        ModelState.AddModelError(nameof(appointment.UserEmail), errorMessage);
        // returns to the create page with the data still filled in
        return View(appointment);
    }

    // GET: Appointments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) { return NotFound(); }

        var appointment = _appointmentRepository.GetAppointmentById(id);

        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }

        return View(appointment);
    }

    // POST: Appointments/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] UpcomingAppointment appointment)
    {
        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }

        if (ModelState.IsValid)
        {
            try
            {
                _appointmentRepository.UpdateAppointment(appointment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(appointment);
    }

    // GET: Appointments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {

        if (id == null) { return NotFound(); }

        var appointment = _appointmentRepository.GetAppointmentById(id);
        var user = await _accountContext.Users.FirstOrDefaultAsync(m => m.Email == appointment.UserEmail);

        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }

        var viewModel = new ViewModel();
        viewModel.UserAppointments = appointment;
        viewModel.UserDetails = user;
        return View(viewModel);
    }

    // POST: Appointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {

        // Find the appointment by ID
        var appointment = _appointmentRepository.GetAppointmentById(id);

        if (appointment != null && (appointment.UserEmail == User.Identity.Name || User.IsInRole("Admin")))
        {
            _appointmentRepository.DeleteAppointmentById(id);
        }

        // Redirect to the Index action after deletion, passing the showArchived parameter
        return RedirectToAction(nameof(Index));
    }

    private bool AppointmentExists(int id)
    {
        _appointmentRepository.GetAppointmentById(id);
        if (_appointmentRepository.GetAppointmentById(id) == null)
            return false;
        return true;
    }
}
