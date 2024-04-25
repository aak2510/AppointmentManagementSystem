using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.ViewModels
{
    // Used as a link to pass in the users' personal details and appointments as a single object.
    public class ViewModel
    {

        public AppUser UserDetails { get; set; }
        public Appointment UserAppointments { get; set; }

    }
}