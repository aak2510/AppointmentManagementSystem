using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppointmentManagementSystem.Controllers
{
    [Authorize]
    //[Authorize(Roles = "User")]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentDbContext _context;
        //private string _UserId;

        public AppointmentsController(AppointmentDbContext context)
        {
            _context = context;
            //_UserId = context.Users.FirstOrDefault().Id;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(bool showArchived = false)
        {


            // Retrieve the current user's ID
            string userEmail = User.Identity.Name;
            if(userEmail == null)
            {
                return NotFound();
            }
            if(userEmail == "admin@admin.com")
            {
                if (showArchived)
                    return View(await _context.archivedAppointments.ToListAsync());
                else
                    return View(await _context.appointments.ToListAsync());
            }
            else
            {
                if (showArchived)
                {
                    // Retrieve appointments for the current user
                    var userAppointments = await _context.archivedAppointments
                    .Where(a => a.UserEmail == userEmail)
                    .ToListAsync();
                    return View(userAppointments);
                } 
                else
                {
                    // Retrieve appointments for the current user
                    var userAppointments = await _context.appointments
                    .Where(a => a.UserEmail == userEmail)
                    .ToListAsync();
                    return View(userAppointments);
                }
            }
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.appointments
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null || (appointment.UserEmail != User.Identity.Name && User.Identity.Name != "admin@admin.com"))
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] Appointment appointment)
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
            if (id == null)
            {
                return NotFound();
            }


            var appointment = await _context.appointments.FindAsync(id);
            if (appointment == null || (appointment.UserEmail != User.Identity.Name && User.Identity.Name != "admin@admin.com"))
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,AppointmentSubject,AppointmentDate,AppointmentTime,UserEmail")] Appointment appointment)
        {
            if (appointment == null || (appointment.UserEmail != User.Identity.Name && User.Identity.Name != "admin@admin.com"))
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.appointments
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null || (appointment.UserEmail != User.Identity.Name && User.Identity.Name != "admin@admin.com"))
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.appointments.FindAsync(id);
            if (appointment != null && (appointment.UserEmail == User.Identity.Name || User.Identity.Name == "admin@admin.com"))
            {
                _context.appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.appointments.Any(e => e.AppointmentId == id);
        }
    }
}
