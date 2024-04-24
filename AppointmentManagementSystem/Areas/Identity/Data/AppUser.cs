using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Areas.Identity.Data;

// This replaces the default identity user
// It also extends its functionality adding
// - First name
// - Last name
public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

}
