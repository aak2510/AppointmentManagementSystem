using Microsoft.AspNetCore.Mvc;
using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Services;

public static class AppointmentValidation
{
    public static IActionResult CheckAppointmentValidation(this ControllerBase controller, Appointment appointment)
    {
        // If the appointment doesn't exist
        if (appointment == null)
        {
            // Return a "Not Found" response
            return controller.NotFound();
        }

        // Check if the user is not an admin and not the owner of the appointment
        if (!controller.User.IsInRole("Admin") && appointment.UserEmail != controller.User.Identity.Name)
        {
            // If so, return an "Unauthorized" response
            return controller.Unauthorized();
        }

        // If valid, return null
        // This indicates it was successful
        return null;
    }

}
