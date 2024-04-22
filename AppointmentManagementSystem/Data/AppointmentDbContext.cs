using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<UpcomingAppointment> appointments { get; set; }
    public DbSet<ArchivedAppointment> archivedAppointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure table name for current appointments
        modelBuilder.Entity<UpcomingAppointment>().ToTable("Appointments");
        // Configure table name for archived appointments
        modelBuilder.Entity<ArchivedAppointment>().ToTable("ArchivedAppointments");
    }
}
