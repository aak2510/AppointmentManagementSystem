using AppointmentManagementSystem.Data;
using Microsoft.AspNetCore.Identity;

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
        //using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        //{
        //    //seeding initial sata into application
        //    // - roles
        //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //can grab an instance of this using dependency injection
        //    var roles = new[] { "Admin", "user" };

        //    foreach (var role in roles)
        //    {
        //        //if no roles provided create roles
        //        if (!await roleManager.RoleExistsAsync(role))
        //            await roleManager.CreateAsync(new IdentityRole(role));
        //    }
        //}

        //using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        //{
        //    //seeding initial sata into application
        //    // - accounts
        //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(); //can grab an instance of this using dependency injection

        //    string email = "admin@admin.com";
        //    string password = "Secure@2"; //Env variables?

        //    if (await userManager.FindByEmailAsync(email) == null)
        //    {
        //        var user = new IdentityUser(email);
        //        user.UserName = email;
        //        user.Email = email;
        //        user.EmailConfirmed = true;

        //        await userManager.CreateAsync(user, password);

        //        await userManager.AddToRoleAsync(user, "Admin");
        //    }
        //}
    }
}
