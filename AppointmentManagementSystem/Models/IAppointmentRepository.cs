namespace AppointmentManagementSystem.Models;

public interface IAppointmentRepository
{
    /*An interface for Accessing appointments
     *Include: all apointments and filtering appointments by id and search queries */

    IEnumerable<UpcomingAppointment> AllUpcomingAppointments { get; }
    IEnumerable<ArchivedAppointment> AllArchivedAppointments { get; }

    
    
    public Appointment GetAppointmentById(int? id);

    public void AddAppointment(UpcomingAppointment? appointment);

    public void UpdateAppointment(UpcomingAppointment appointment);

    public void DeleteAppointmentById(int? id);

    public IEnumerable<Appointment> SearchAppointments(string searchQuery, IEnumerable<Appointment> appointments);

    public void CheckForExpiredAppointments();
}