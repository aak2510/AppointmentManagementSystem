using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.ViewModels
{
    public class ViewModel
    {

        public AppUser UserDetails { get; set; }
        public Appointment UserAppointments { get; set; }

    }
}