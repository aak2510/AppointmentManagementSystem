using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using AppointmentManagementSystem.Services;

namespace AppointmentManagementSystem.Controllers;

[Authorize]
//[Authorize(Roles = "User")]
public class AppointmentsController : Controller
{
    private readonly AppointmentDbContext _context;
    private bool _showingArchived = false;

    public AppointmentsController(AppointmentDbContext context)
    {
        _context = context;
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
        if(userEmail == null)
        {
            return NotFound();
        }
        if(userEmail == "admin@admin.com")
        {
            return View(await appointmentsContext.ToListAsync());
        }
        else
        {
            // Retrieve appointments for the current user
            var userAppointments = await appointmentsContext
            .Where(a => a.UserEmail == userEmail)
            .ToListAsync();
            return View(userAppointments);
        }
    }

    // GET: Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) { return NotFound(); }


        var appointment = await _context.appointments
            .FirstOrDefaultAsync(m => m.AppointmentId == id);

        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }

        return View(appointment);
    }

    // GET: Appointments/Create
    public IActionResult Create()
    {
        var appointment = new Appointment() { UserEmail = User.Identity.Name };
        return View(appointment);
    }

    // POST: Appointments/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] UpcomingAppointment appointment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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

        if (AppointmentValidation.IsAppointmentNull(appointment)) { return NotFound(); }
        if (AppointmentValidation.IsUserInvalid(appointment, User)) { return Unauthorized(); }

        return View(appointment);
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
