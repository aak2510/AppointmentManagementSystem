using AppointmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {

    }

    // DbSet for storing upcoming appointments
    public DbSet<UpcomingAppointment> appointments { get; set; }
    // DbSet for storing archived appointments
    public DbSet<ArchivedAppointment> archivedAppointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure table name for current appointments
        modelBuilder.Entity<UpcomingAppointment>().ToTable("Appointments");
        // Configure table name for archived appointments
        modelBuilder.Entity<ArchivedAppointment>().ToTable("ArchivedAppointments");
    }
}
