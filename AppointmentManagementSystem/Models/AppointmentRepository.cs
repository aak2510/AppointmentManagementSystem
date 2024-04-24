using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models.ArchiveState;

namespace AppointmentManagementSystem.Models;

public class AppointmentRepository : IAppointmentRepository
{

    private readonly AppointmentDbContext _context;

    // Constructor to inject the database context
    public AppointmentRepository(AppointmentDbContext context)
    {
        _context = context;
    }

    // Property to get all upcoming appointments
    public IEnumerable<UpcomingAppointment> AllUpcomingAppointments
    {
        get
        {
            // Retrieve all upcoming appointments from the database and order them by date and time
            return _context.appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime);
        }
    }

    // Property to get all archived appointments
    public IEnumerable<ArchivedAppointment> AllArchivedAppointments
    {
        get
        {
            // Retrieve all archived appointments from the database and order them by date and time
            return _context.archivedAppointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime);
        }
    }

    // Retrieves an appointment by its ID
    public Appointment GetAppointmentById(int? id)
    {
        // Check if viewing archived appointments
        if (ArchiveStateSingleton.Instance.IsViewingArchivedAppointments)
        {
            // Retrieve appointment by ID from archived appointments if viewing archived
            return _context.archivedAppointments.Find(id);
        }
        else
        {
            // Retrieve appointment by ID from upcoming appointments if not viewing archived
            return _context.appointments.Find(id);
        }
    }

    // Adds a new appointment to the database
    public void AddAppointment(UpcomingAppointment? appointment)
    {
        // Add new appointment to the database
        _context.appointments.Add(appointment);
        // Save changes to the database
        _context.SaveChanges();
    }

    // Updates an existing appointment in the database
    public void UpdateAppointment(UpcomingAppointment? appointment)
    {
        // Updates appointment by
        // getting the id of the passed in, if it exisits we modify it
        // otherwise it would add it which wouldn't be the case here
        _context.appointments.Update(appointment);
        // Save changes to the database
        _context.SaveChanges();
    }

    // Deletes an appointment by its ID
    public void DeleteAppointmentById(int? id)
    {
        // Check if viewing archived appointments
        if (ArchiveStateSingleton.Instance.IsViewingArchivedAppointments)
        {
            // Retrieve archived appointment by ID
            var archivedAppointment = _context.archivedAppointments.Find(id);
            // Check if the archived appointment exists
            if (archivedAppointment != null)
            {
                // Remove the archived appointment
                _context.archivedAppointments.Remove(archivedAppointment);
                _context.SaveChanges();
            }
        } 
        else
        {
            // Retrieve upcoming appointment by ID
            var upcomingAppointment = _context.appointments.Find(id);
            // Check if the upcoming appointment exists
            if (upcomingAppointment != null)
            {
                // Deletes the appointment from the upcoming appointments
                // Adds the appointment to the archived appointmnets
                DeleteAndArchive(upcomingAppointment);
            }
        }
    }

    // Deletes the given upcomoing appointments and archives it adding it to the archived appointments
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

    // Checks for appointments that have expired and archives them
    public void CheckForExpiredAppointments()
    {
        // Gets the current local system time
        // As this is not a live application this keeps it consistent
        // With whatever system it is run on
        var currentDateTime = DateTime.Now;

        // Retrieve appointments that have an appointment date before the current date or 
        // have an appointment date equal to the current date but with a time before the current time
        var expiredAppointments = _context.appointments
            .Where(a => a.AppointmentDate.Date < currentDateTime.Date ||
                (a.AppointmentDate.Date == currentDateTime.Date && a.AppointmentTime.TimeOfDay < currentDateTime.TimeOfDay))
            .ToList();

        // Iterate through the expired appointments and archive them
        foreach (var appointment in expiredAppointments)
            DeleteAndArchive(appointment);
    }

    // Filters the provided collection of appointments based on the given search query
    // This first tries to find a date from the given string if so we filter by date
    // If not we see if the given string is a subject such as "meeting"
    public IEnumerable<Appointment> SearchAppointments(string searchQuery, IEnumerable<Appointment> appointments)
    {
        // Parse the search query as a date
        if (DateTime.TryParse(searchQuery, out DateTime searchDate))
        {
            // Search by appointment date
            appointments = appointments.Where(a => a.AppointmentDate.Date == searchDate.Date);
        } else
        {
            // Search by appointment subject partially (case-insensitive)
            appointments = appointments.Where(a => a.AppointmentSubject.ToLower().Contains(searchQuery.ToLower()));
        }

        return appointments;
    }
}
