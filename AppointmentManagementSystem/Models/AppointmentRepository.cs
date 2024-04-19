using AppointmentManagementSystem.Data;

namespace AppointmentManagementSystem.Models;

//Repository design pattern
public class AppointmentRepository : IAppointmentRepository
{

    private readonly ApplicationDbContext _context;

    // dependency injected db context
    public AppointmentRepository(ApplicationDbContext context) 
    {
        _context = context;
    }


    public IEnumerable<Appointment> AllAppointment
    {
        get
        {
            return _context.appointments.OrderBy(p => p.AppointmentDate);
        }
    }

    public Appointment? GetAppointmentById(int userID)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Appointment> SearchAppointments(string searchQuery)
    {
        throw new NotImplementedException();
    }
}
