using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels;
using AppointmentManagementSystem.Models.ArchiveState;
using NuGet.Protocol.Core.Types;
using Microsoft.CodeAnalysis.Differencing;
using Azure.Core;


namespace AppointmentManagementSystem.Controllers;

[Authorize] //Must be looged in to view
public class AppointmentsController : Controller
{
    // Instances of appointment repository and account database context
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AccountDbContext _accountContext;

    // Gets instances of IAppointmentRepository and AccountDbContext through dependency injection
    public AppointmentsController(IAppointmentRepository appointmentRepository, AccountDbContext accountContext)
    {
        _appointmentRepository = appointmentRepository;
        _accountContext = accountContext;
    }

    // GET
    // Handles requests to view appointments,
    // filtering and displaying them based on various criteria
    public async Task<IActionResult> Index(bool? showArchived, string? searchQuery)
    {
        // Check for and handle expired appointments
        // Archives experied appointments
        _appointmentRepository.CheckForExpiredAppointments();

        // Set the viewing archive state in the repository if provided
        if (showArchived.HasValue)
        {
            ArchiveStateSingleton.Instance.IsViewingArchivedAppointments = showArchived.Value;
        }

        // Retrieve the current user's email
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
            // Used within the razor index view to display the previous search
            // As well as showing an option for clearing the search
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

        // Return the view with the filtered and ordered appointments
        return View(appointments);
    }

    // GET
    // Retrieves details of a specific appointment
    public async Task<IActionResult> Details(int? id)
    {
        // Check if the appointment ID is not provided
        // If so, return a "Not Found" response
        if (id == null)
            return NotFound();

        // Retrieve the appointment from the repository
        var appointment = _appointmentRepository.GetAppointmentById(id);

        // Validate appointment
        IActionResult authorizationResult = AppointmentValidation.CheckAppointmentValidation(this, appointment);
        if (authorizationResult != null)
            return authorizationResult;

        // Retrieve the associated user information from the account context
        var user = _accountContext.Users.FirstOrDefault(u => u.Email == appointment.UserEmail);

        // Create a view model containing the appointment details and associated user information
        // If the appointment has no associated user, it won't display anything
        var viewModel = new ViewModel
        {
            UserAppointments = appointment,
            UserDetails = user
        };

        // Return a view displaying the appointment details along with associated user information
        return View(viewModel);
    }

    // GET
    // Displays the Create view
    public IActionResult Create()
    {
        return View();
    }

    // POST
    // Creates a new appointment
    [HttpPost] // Specifies that this method handles HTTP POST requests
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] UpcomingAppointment appointment)
    {
        // Clear the errors in case user trys to re-enter information
        ModelState.Clear();
        if (ModelState.IsValid)
        {
            // Check if the provided user email exists in accounts
            if (_accountContext.Users.FirstOrDefault(u => u.Email == appointment.UserEmail) != null)
            {
                // If the user exists, add the appointment to the repository
                _appointmentRepository.AddAppointment(appointment);
                // Redirect to the appointment index page
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

    // GET
    // Retrieve and display the form for editing a specific appointment.
    public async Task<IActionResult> Edit(int? id)
    {
        // If an id was not provided display not found
        if (id == null) { return NotFound(); }

        // Retrieve an appointment from the repository using the provided ID
        var appointment = _appointmentRepository.GetAppointmentById(id);

        // Validate appointment
        IActionResult authorizationResult = AppointmentValidation.CheckAppointmentValidation(this, appointment);
        if (authorizationResult != null)
            return authorizationResult;

        // Return the view containing the form for editing the appointment
        return View(appointment);
    }

    // POST
    // Handles request to update the details of a specific appointment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] UpcomingAppointment appointment)
    {
        // validate appointment
        IActionResult authorizationResult = AppointmentValidation.CheckAppointmentValidation(this, appointment);
        if (authorizationResult != null)
            return authorizationResult;

        // Check if the appointment model is valid
        if (ModelState.IsValid)
        {
            try
            {
                // Attempt to update the appointment in the repository
                _appointmentRepository.UpdateAppointment(appointment);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflicts
                if (!AppointmentExists(appointment.AppointmentId))
                {
                    // If the appointment doesn't exist, return a "Not Found" response
                    return NotFound();
                }
                else
                {
                    // If there's another concurrency issue, rethrow the exception
                    throw;
                }
            }
            // Redirect to the appointment index page after successful update
            return RedirectToAction(nameof(Index));
        }
        // If the model state is invalid, return to the edit page
        return View(appointment);
    }

    // GET
    // Request to retrieve and display the confirmation page for deleting a specific appointment.
    public async Task<IActionResult> Delete(int? id)
    {
        // Check if the appointment ID is not provided and display not found if so
        if (id == null) { return NotFound(); }

        // Retrieve the appointment from the repository using the provided ID
        var appointment = _appointmentRepository.GetAppointmentById(id);

        // Retrieve the associated user information from the account context
        var user = await _accountContext.Users.FirstOrDefaultAsync(m => m.Email == appointment.UserEmail);

        // Validate appointment
        IActionResult authorizationResult = AppointmentValidation.CheckAppointmentValidation(this,appointment);
        if (authorizationResult != null)
            return authorizationResult;

        // Create a view model containing the appointment details and associated user information
        var viewModel = new ViewModel();
        viewModel.UserAppointments = appointment;
        viewModel.UserDetails = user;

        // Return a view containing the confirmation page for deleting the appointment
        return View(viewModel);
    }

    // POST
    // Request to delete a specific appointment confirmed by the user.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Find the appointment in the repository by ID
        var appointment = _appointmentRepository.GetAppointmentById(id);

        // Check if the appointment exists and if the current user is authorized to delete it
        if (appointment != null && (appointment.UserEmail == User.Identity.Name || User.IsInRole("Admin")))
        {
            // If authorized, delete the appointment
            _appointmentRepository.DeleteAppointmentById(id);
        }

        // Redirect to the appointment index page after deletion
        return RedirectToAction(nameof(Index));
    }

    // Checks if an appointment with the specified ID exists in the repository.
    private bool AppointmentExists(int id)
    {
        // Attempt to retrieve the appointment from the repository using the provided ID
        _appointmentRepository.GetAppointmentById(id);

        // If the retrieved appointment is null, it means the appointment doesn't exist
        if (_appointmentRepository.GetAppointmentById(id) == null)
            return false;

        // Otherwise, the appointment exists
        return true;
    }
}
