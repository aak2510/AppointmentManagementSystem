using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models.ArchiveState;

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
        _context.SaveChangesAsync();
    }

    public void UpdateAppointment(UpcomingAppointment? appointment)
    {
        _context.appointments.Update(appointment);
        _context.SaveChangesAsync();
    }

    public void DeleteAppointmentById(int? id)
    {
        if (ArchiveStateSingleton.Instance.IsViewingArchivedAppointments)
        {
            var archivedAppointment = _context.archivedAppointments.Find(id);
            if (archivedAppointment != null)
            {
                _context.archivedAppointments.Remove(archivedAppointment);
            }
        } else
        {
            var upcomingAppointment = _context.appointments.Find(id);
            if (upcomingAppointment != null)
            {
                _context.appointments.Remove(upcomingAppointment);
            }
        }
        _context.SaveChangesAsync();
    }

    public IEnumerable<Appointment> SearchAppointments(string searchQuery)
    {
        throw new NotImplementedException();
    }
}
