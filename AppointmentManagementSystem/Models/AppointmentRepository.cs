using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models.ArchiveState;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Models;

//Repository design pattern
public class AppointmentRepository : IAppointmentRepository
{

    private readonly AppointmentDbContext _context;

    // dependency injected db context
    public AppointmentRepository(AppointmentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<UpcomingAppointment> AllUpcomingAppointments
    {
        get
        {
            return _context.appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime);
        }
    }

    public IEnumerable<ArchivedAppointment> AllArchivedAppointments
    {
        get
        {
            return _context.archivedAppointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime);
        }
    }


    public Appointment GetAppointmentById(int? id)
    {
        if (ArchiveStateSingleton.Instance.IsViewingArchivedAppointments)
        {
            return _context.archivedAppointments.Find(id);
        }
        return _context.appointments.Find(id);
    }


    public void AddAppointment(UpcomingAppointment? appointment)
    {
        _context.appointments.Add(appointment);
        _context.SaveChanges();
    }

    public void UpdateAppointment(UpcomingAppointment? appointment)
    {
        _context.appointments.Update(appointment);
        _context.SaveChanges();
    }

    public void DeleteAppointmentById(int? id)
    {
        if (ArchiveStateSingleton.Instance.IsViewingArchivedAppointments)
        {
            var archivedAppointment = _context.archivedAppointments.Find(id);
            if (archivedAppointment != null)
            {
                _context.archivedAppointments.Remove(archivedAppointment);
                _context.SaveChanges();
            }
        } 
        else
        {
            var upcomingAppointment = _context.appointments.Find(id);
            if (upcomingAppointment != null)
            {
                DeleteAndArchive(upcomingAppointment);
            }
        }
    }

    public void CheckForExpiredAppointments()
    {
        var currentDateTime = DateTime.Now; // Get current date and time
        var expiredAppointments = _context.appointments
            .Where(a => a.AppointmentDate.Date < currentDateTime.Date ||
                (a.AppointmentDate.Date == currentDateTime.Date && a.AppointmentTime.TimeOfDay < currentDateTime.TimeOfDay))
            .ToList();

        foreach (var appointment in expiredAppointments)
        {
            DeleteAndArchive(appointment);
        }
    }

    public IEnumerable<Appointment> SearchAppointments(string searchQuery)
    {
        throw new NotImplementedException();
    }



    private void DeleteAndArchive(UpcomingAppointment appointment)
    {
        // Create a new ArchivedAppointment based on the upcomingAppointment
        var archivedAppointment = new ArchivedAppointment
        {
            UserEmail = appointment.UserEmail,
            AppointmentSubject = appointment.AppointmentSubject,
            AppointmentDate = appointment.AppointmentDate,
            AppointmentTime = appointment.AppointmentTime
        };

        // Add the archivedAppointment to the archivedAppointments DbSet
        _context.archivedAppointments.Add(archivedAppointment);
        _context.SaveChanges();

        // Remove the upcomingAppointment from the appointments DbSet
        _context.appointments.Remove(appointment);
        _context.SaveChanges();
    }
}
