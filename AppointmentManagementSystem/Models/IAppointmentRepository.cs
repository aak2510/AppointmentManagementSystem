namespace AppointmentManagementSystem.Models;

public interface IAppointmentRepository
{
    IEnumerable<Appointment> AllAppointment { get; }
    Appointment? GetAppointmentById(int appointmentId);
    IEnumerable<Appointment> SearchAppointments(string searchQuery);
}