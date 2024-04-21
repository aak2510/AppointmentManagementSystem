using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels;


namespace AppointmentManagementSystem.Controllers;

[Authorize]
//[Authorize(Roles = "User")]
public class AppointmentsController : Controller
{
    private readonly AppointmentDbContext _context;
    private readonly AccountDbContext _accountContext;
    private bool _showingArchived = false;

    public AppointmentsController(AppointmentDbContext context, AccountDbContext accountContext)
    {
        _context = context;
        _accountContext = accountContext;
    }

    // GET: Appointments
    public async Task<IActionResult> Index(bool showArchived = false)
    {
        _showingArchived = showArchived;
        ViewBag.ShowArchived = showArchived;

        IQueryable<Appointment> appointmentsContext = _showingArchived ?
            _context.archivedAppointments :
            _context.appointments;

        // Retrieve the current user's ID
        string userEmail = User.Identity.Name;
        if (userEmail == null)
        {
            return NotFound();
        }

        // Change this to admin role
        if (userEmail == "admin@admin.com")
        {
            var allAppointments = await appointmentsContext.OrderBy(a => a.AppointmentDate).ToListAsync();
            return View(allAppointments);
        }
        else
        {
            // Retrieve appointments for the current user
            var userAppointments = await appointmentsContext
            .Where(a => a.UserEmail == userEmail)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
            return View(userAppointments);
        }
    }

    // GET: Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) { return NotFound(); }


        var appointment = await _context.appointments.FirstOrDefaultAsync(m => m.AppointmentId == id);

        // returns 404 not found if no appointments exist or 
        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        // returns unauthorised 401 if the logged in user trys to access another persons appointment
        // Unless the logged in user is the admin
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }



        // Display the user information alongside the appointment information
        // First find a user with the assoicated email
        var user = _accountContext.Users.FirstOrDefault(u => u.Email == appointment.UserEmail);

        // If the appointment has no email or user registered, then it won't display anything
        var viewModel = new ViewModel();
        viewModel.UserAppointments = appointment;
        viewModel.UserDetails = user;
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
                _context.Add(appointment);
                await _context.SaveChangesAsync();
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

        var appointment = await _context.appointments.FindAsync(id);

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
                _context.Update(appointment);
                await _context.SaveChangesAsync();
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
    public async Task<IActionResult> Delete(int? id, bool isArchived)
    {
        ViewBag.ShowArchived = isArchived;

        if (id == null) { return NotFound(); }

        IQueryable<Appointment> appointmentsContext = isArchived ?
            _context.archivedAppointments :
            _context.appointments;

        var appointment = await appointmentsContext
            .FirstOrDefaultAsync(m => m.AppointmentId == id);
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
    public async Task<IActionResult> DeleteConfirmed(int id, bool isArchived)
    {
        bool showArchived = isArchived;

        // Determine the correct DbSet based on whether the appointment is archived or not
        IQueryable<Appointment> appointmentsContext = isArchived ?
            _context.archivedAppointments.Cast<Appointment>() :
            _context.appointments.Cast<Appointment>();

        // Find the appointment by ID
        var appointment = await appointmentsContext.FirstOrDefaultAsync(m => m.AppointmentId == id);

        if (appointment != null && (appointment.UserEmail == User.Identity.Name || User.Identity.Name == "admin@admin.com"))
        {
            // If the appointment is archived, remove it from archived appointments
            if (isArchived)
            {
                _context.archivedAppointments.Remove((ArchivedAppointment)appointment);
            }
            // If the appointment is not archived, remove it from upcoming appointments
            else
            {
                _context.appointments.Remove((UpcomingAppointment)appointment);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        // Redirect to the Index action after deletion, passing the showArchived parameter
        return RedirectToAction(nameof(Index), new { showArchived = showArchived });
    }

    private bool AppointmentExists(int id)
    {
        IQueryable<Appointment> appointmentsContext = _showingArchived ?
            _context.archivedAppointments :
            _context.appointments;
        return appointmentsContext.Any(e => e.AppointmentId == id);
    }
}
