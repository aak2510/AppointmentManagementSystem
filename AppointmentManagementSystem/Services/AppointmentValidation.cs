using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace AppointmentManagementSystem.Services;

public static class AppointmentValidation
{
    public static bool IsAppointmentNull(Appointment? appointment)
    {
        if (appointment == null)
            return true;
        return false;
    }

    public static bool IsUserInvalid(Appointment? appointment, ClaimsPrincipal? User)
    {
        if (appointment.UserEmail != User.Identity.Name && User.Identity.Name != "admin@admin.com")
            return true;
        return false;
    }
}
