using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var AppointmentDbConnection = builder.Configuration.GetConnectionString("AppointmentDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        var AccountDbConnection = builder.Configuration.GetConnectionString("AccountDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        //dependcy injected container
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        builder.Services.AddDbContext<AppointmentDbContext>(options =>
            options.UseSqlServer(AppointmentDbConnection));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(AccountDbConnection));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AccountDbContext>();

        builder.Services.AddControllersWithViews();

        builder.Services.AddRazorPages();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        } else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        //seeding data for the appointments and users
        DbInitializer.Seed(app);

        app.Run();
    }
}