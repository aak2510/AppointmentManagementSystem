using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a new builder for the web application
        var builder = WebApplication.CreateBuilder(args);

        // Retrieve connection strings for AppointmentDbConnection and AccountDbConnection from configuration
        var AppointmentDbConnection = builder.Configuration.GetConnectionString("AppointmentDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        var AccountDbConnection = builder.Configuration.GetConnectionString("AccountDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register the AppointmentRepository class to be injected whenever an IAppointmentRepository dependency is requested.
        // This enables dependency injection, allowing components to easily obtain instances of AppointmentRepository
        // without needing to create them directly.
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        // Configure AppointmentDbContext with SQL Server database connection
        builder.Services.AddDbContext<AppointmentDbContext>(options =>
            options.UseSqlServer(AppointmentDbConnection));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // Add developer page exception filter for database

        // Configure AccountDbContext with SQL Server database connection
        builder.Services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(AccountDbConnection));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // Add developer page exception filter for database

        // Configure default identity for AppUser
        builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>() // Add roles
            .AddEntityFrameworkStores<AccountDbContext>(); // Add Entity Framework stores for AccountDbContext

        // Add MVC controllers with views support to the dependency injection container.
        builder.Services.AddControllersWithViews();

        // Add Razor Pages support to the dependency injection container.
        builder.Services.AddRazorPages();

        // Add authorization services to the dependency injection container.
        builder.Services.AddAuthorization();

        // Build the application
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        } 
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Ensure secure communication.
        app.UseHttpsRedirection();
        // Enable serving static files (such as HTML, CSS, and JavaScript) from the wwwroot folder.
        app.UseStaticFiles();
        // Enable routing for incoming requests.
        app.UseRouting();
        // Enable authorization, allowing authentication and access control for protected resources.
        app.UseAuthorization();

        // Map default controller route for MVC controllers, defining the default route pattern
        // and the default controller and action if none are provided in the URL.
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Map Razor Pages, enabling the use of Razor Pages for handling requests.
        app.MapRazorPages();

        // Seed initial data for appointments and users into the database.
        // Only if the databases are empty
        DbInitializer.Seed(app);

        // Run the application
        app.Run();
    }
}