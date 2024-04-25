namespace AppointmentManagementSystem.Models;

public interface IAppointmentRepository
{
    // Gets all upcoming appointments.
    IEnumerable<UpcomingAppointment> AllUpcomingAppointments { get; }

    // Gets all archived appointments.
    IEnumerable<ArchivedAppointment> AllArchivedAppointments { get; }

    // Retrieves an appointment by its unique identifier.
    Appointment GetAppointmentById(int? id);

    // Adds a new upcoming appointment.
    void AddAppointment(UpcomingAppointment? appointment);

    // Updates an existing upcoming appointment.
    void UpdateAppointment(UpcomingAppointment appointment);

    // Deletes an appointment by its unique identifier.
    void DeleteAppointmentById(int? id);

    void DeleteAllAppointmentByEmail(string email);

    // Searches appointments based on the provided search query.
    IEnumerable<Appointment> SearchAppointments(string searchQuery, IEnumerable<Appointment> appointments);

    // Checks for expired appointments and archives them if necessary.
    void CheckForExpiredAppointments();
}