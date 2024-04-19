namespace AppointmentManagementSystem.Models;

public interface IAppointmentRepository
{
    /*An interface for Accessing appointments
     *Include: all apointments and filtering appointments by id and search queries */

    IEnumerable<Appointment> AllAppointment { get; }
    Appointment? GetAppointmentById(int appointmentId);
    IEnumerable<Appointment> SearchAppointments(string searchQuery);
}