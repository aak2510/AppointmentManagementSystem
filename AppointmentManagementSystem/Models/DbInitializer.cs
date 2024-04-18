using AppointmentManagementSystem.Data;

namespace AppointmentManagementSystem.Models;


public static class DbInitializer
{
    public static void Seed(IApplicationBuilder applicationBuilder)
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
    }
}
