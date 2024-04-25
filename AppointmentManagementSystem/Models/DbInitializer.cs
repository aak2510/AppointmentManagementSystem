using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace AppointmentManagementSystem.Models;


public static class DbInitializer
{
    // Method to seed the database with sample data if it is empty
    public static async void Seed(IApplicationBuilder applicationBuilder)
    {
        #region Seed Appointment data
        // Retrieve the AppointmentDbContext from the service provider
        AppointmentDbContext AppointmentContext = applicationBuilder.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppointmentDbContext>();

        // Check if there are no upcoming appointments in the database
        if (!AppointmentContext.appointments.Any())
        {
            // Add sample upcoming appointments
            AppointmentContext.appointments.AddRange
            (
                new UpcomingAppointment { AppointmentSubject = "Meeting", AppointmentDate = new DateTime(2024, 4, 29), AppointmentTime = new DateTime(2024, 1, 1, 9, 0, 0), UserEmail = "test@test.com" },
                new UpcomingAppointment { AppointmentSubject = "Interview", AppointmentDate = new DateTime(2024, 4, 27), AppointmentTime = new DateTime(2024, 1, 1, 10, 30, 0), UserEmail = "test@test.com" },
                new UpcomingAppointment { AppointmentSubject = "Meeting", AppointmentDate = new DateTime(2024, 4, 28), AppointmentTime = new DateTime(2024, 1, 1, 14, 15, 0), UserEmail = "test@test.com" },
                new UpcomingAppointment { AppointmentSubject = "Workshop", AppointmentDate = new DateTime(2024, 5, 1), AppointmentTime = new DateTime(2024, 1, 1, 16, 45, 0), UserEmail = "test@test.com" },
                new UpcomingAppointment { AppointmentSubject = "Review", AppointmentDate = new DateTime(2024, 5, 6), AppointmentTime = new DateTime(2024, 1, 1, 11, 0, 0), UserEmail = "test@test.com" },
                new UpcomingAppointment { AppointmentSubject = "Training", AppointmentDate = new DateTime(2024, 5, 3), AppointmentTime = new DateTime(2024, 1, 1, 13, 45, 0), UserEmail = "email@email.com" },
                new UpcomingAppointment { AppointmentSubject = "Meeting", AppointmentDate = new DateTime(2024, 5, 4), AppointmentTime = new DateTime(2024, 1, 1, 15, 30, 0), UserEmail = "email@email.com" },
                new UpcomingAppointment { AppointmentSubject = "Follow-up", AppointmentDate = new DateTime(2024, 5, 2), AppointmentTime = new DateTime(2024, 1, 1, 17, 15, 0), UserEmail = "email@email.com" },
                new UpcomingAppointment { AppointmentSubject = "Briefing", AppointmentDate = new DateTime(2024, 5, 1), AppointmentTime = new DateTime(2024, 1, 1, 9, 45, 0), UserEmail = "email@email.com" },
                new UpcomingAppointment { AppointmentSubject = "Client pitch", AppointmentDate = new DateTime(2024, 4, 29), AppointmentTime = new DateTime(2024, 1, 1, 12, 0, 0), UserEmail = "email@email.com" }
            );
        }

        // Check if there are no archived appointments in the database
        if (!AppointmentContext.archivedAppointments.Any())
        {
            // Add sample archived appointments
            AppointmentContext.archivedAppointments.AddRange
            (
                new ArchivedAppointment { AppointmentSubject = "Meeting", AppointmentDate = new DateTime(2023, 4, 13), AppointmentTime = new DateTime(2024, 1, 1, 9, 0, 0), UserEmail = "test@test.com" },
                new ArchivedAppointment { AppointmentSubject = "Conference", AppointmentDate = new DateTime(2022, 7, 20), AppointmentTime = new DateTime(2024, 1, 1, 14, 30, 0), UserEmail = "test@test.com" },
                new ArchivedAppointment { AppointmentSubject = "Training", AppointmentDate = new DateTime(2021, 9, 5), AppointmentTime = new DateTime(2024, 1, 1, 11, 0, 0), UserEmail = "email@email.com" },
                new ArchivedAppointment { AppointmentSubject = "Seminar", AppointmentDate = new DateTime(2020, 12, 12), AppointmentTime = new DateTime(2024, 1, 1, 9, 30, 0), UserEmail = "email@email.com" },
                new ArchivedAppointment { AppointmentSubject = "Workshop", AppointmentDate = new DateTime(2019, 6, 25), AppointmentTime = new DateTime(2024, 1, 1, 12, 0, 0), UserEmail = "email@email.com" }
            );
        }
        // Saves the data to the appointment database
        AppointmentContext.SaveChanges();
        #endregion

        #region Seed Account data
        // Seeding initial data into the application
        // Roles
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            // Get an instance of RoleManager
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create a set of new roles
            var roles = new[] { "Admin", "User" };
            // Loop through roles and add them if the dont exist already
            foreach (var role in roles)
            {
                // Create the role if it doesnt not exist
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Accounts
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            // Get the UserManager service for managing user accounts
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(); //can grab an instance of this using dependency injection

            // Define user account information to be seeded into the database
            DbUserInfo[] dbUsers = new DbUserInfo[]
            {
                // Create instances of UserInfo class to be parsed when creating accounts
                new DbUserInfo
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "1234567890",
                    Role = "Admin",
                    Email = "admin@admin.com",
                    // Passwords are hashed using SHA256
                    Password = "h493yz96x5XyxYTfAOZdey/rL0Qe2fmESwmldH9Ph9g=" 
                },
                new DbUserInfo
                {
                    FirstName = "Test",
                    LastName = "User",
                    PhoneNumber = "9876543210",
                    Role = "User",
                    Email = "test@test.com",
                    Password = "9NgIcEyeC6DRUQwjgD2NEJ4lRV6N3rkMVpndW9u0VOE="
                },
                new DbUserInfo
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "2012345678",
                    Role = "User",
                    Email = "email@email.com",
                    Password = "CYAW1j7zrwejgW47ldd36rgyOmUWHUJuwPRoOWvV5MM="
                }
            };

            // Iterate through each user in the list of user data to be seeded
            foreach (var newDbUser in dbUsers)
            {
                // Check that there is no user in the database with this email
                if (await userManager.FindByEmailAsync(newDbUser.Email) == null)
                {
                    // Create a new application user instance with the user's details
                    var user = new AppUser
                    {
                        UserName = newDbUser.Email,
                        Email = newDbUser.Email,
                        EmailConfirmed = true,
                        FirstName = newDbUser.FirstName,
                        LastName = newDbUser.LastName,
                        PhoneNumber = newDbUser.PhoneNumber
                    };

                    // Create the user account using the UserManager
                    var result = await userManager.CreateAsync(user, newDbUser.Password);

                    // If user creation is successful, assign the appropriate role to the user
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, newDbUser.Role);
                    } 
                    else
                    {
                        // If an error occurs during user creation, throw an exception
                        throw new Exception($"Error creating seeded user data");
                    }
                }
            }
        }
        #endregion
    }
}

// This class is used to setup user information.
// This information is then used to create app users
// which are then added to the database using the user manager service
public class DbUserInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}