using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace AppointmentManagementSystem.Models;


public static class DbInitializer
{
    public static async void Seed(IApplicationBuilder applicationBuilder)
    {
        ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.appointments.Any())
        {
            context.AddRange
            (
                new Appointment { AppointmentDate = new DateTime(2024, 4, 17), AppointmentTime = new DateTime(2024, 1, 1, 9, 0, 0), UserEmail = "test@test.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 18), AppointmentTime = new DateTime(2024, 1, 1, 10, 30, 0), UserEmail = "test@test.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 19), AppointmentTime = new DateTime(2024, 1, 1, 14, 15, 0), UserEmail = "test@test.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 20), AppointmentTime = new DateTime(2024, 1, 1, 16, 45, 0), UserEmail = "test@test.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 21), AppointmentTime = new DateTime(2024, 1, 1, 11, 0, 0), UserEmail = "test@test.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 22), AppointmentTime = new DateTime(2024, 1, 1, 13, 45, 0), UserEmail = "email@email.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 23), AppointmentTime = new DateTime(2024, 1, 1, 15, 30, 0), UserEmail = "email@email.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 24), AppointmentTime = new DateTime(2024, 1, 1, 17, 15, 0), UserEmail = "email@email.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 25), AppointmentTime = new DateTime(2024, 1, 1, 9, 45, 0), UserEmail = "email@email.com" },
                new Appointment { AppointmentDate = new DateTime(2024, 4, 26), AppointmentTime = new DateTime(2024, 1, 1, 12, 0, 0), UserEmail = "email@email.com" }
            );
        }

        context.SaveChanges();

        //Should seed the data for 3 base accounts
        //1 admin and 2 users
        //this should make use of a users class to hold stuff to store
        //like phone, email, password, Role

        ////seeding data
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            //seeding initial sata into application
            // - roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //can grab an instance of this using dependency injection
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                //if no roles provided create roles
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            //seeding initial sata into application
            // - accounts
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(); //can grab an instance of this using dependency injection

            DbUserInfo[] dbUsers = new DbUserInfo[]
            {
                new DbUserInfo
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "1234567890",
                    Role = "Admin",
                    Email = "admin@admin.com",
                    Password = "Secure@1"
                },
                new DbUserInfo
                {
                    FirstName = "Test",
                    LastName = "User",
                    PhoneNumber = "9876543210",
                    Role = "User",
                    Email = "test@test.com",
                    Password = "Secure@2"
                },
                new DbUserInfo
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "2012345678",
                    Role = "User",
                    Email = "email@email.com",
                    Password = "Secure@3"
                }
            };

            foreach (var newDbUser in dbUsers)
            {
                if (await userManager.FindByEmailAsync(newDbUser.Email) == null)
                {
                    var user = new AppUser
                    {

                        UserName = newDbUser.Email,
                        Email = newDbUser.Email,
                        EmailConfirmed = true,
                        FirstName = newDbUser.FirstName,
                        LastName = newDbUser.LastName,
                        PhoneNumber = newDbUser.PhoneNumber
                    };

                    var result = await userManager.CreateAsync(user, newDbUser.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, newDbUser.Role);
                    } else
                    {
                        // Handle error
                        throw new Exception($"Error creating seeded user data");
                    }
                }
            }
        }
    }
}

public class DbUserInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

}