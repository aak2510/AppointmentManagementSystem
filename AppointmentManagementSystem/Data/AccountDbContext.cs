using AppointmentManagementSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data;

//Nothing added here default template 
public class AccountDbContext : IdentityDbContext<AppUser>
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
