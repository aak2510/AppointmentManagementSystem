using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        //dependcy injected container
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

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

        //seeding data
        using (var scope = app.Services.CreateScope())
        {
            //seeding initial sata into application
            // - roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //can grab an instance of this using dependency injection
            var roles = new[] { "Admin", "user" }; //CHANGE THIS TO "User"

            foreach (var role in roles)
            {
                //if no roles provided create roles
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            //seeding initial sata into application
            // - accounts
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(); //can grab an instance of this using dependency injection

            string email = "admin@admin.com";
            string password = "Secure@2"; //Env variables?

            if(await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser(email);
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user,password);

                await userManager.AddToRoleAsync(user,"Admin");
            }
        }

        //seeding the appointments
        DbInitializer.Seed(app);


        app.Run();
    }
}