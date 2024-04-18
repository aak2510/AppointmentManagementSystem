namespace AppointmentManagementSystem.Models;

public interface IAppointmentRepository
{
    IEnumerable<Appointment> AllAppointment { get; }
    Appointment? GetAppointmentById(int pieId);
    IEnumerable<Appointment> SearchAppointments(string searchQuery);
}