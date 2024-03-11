using Faice_Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Data;

public class FaiceDbContext(DbContextOptions<FaiceDbContext> options) 
    : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    public DbSet<AppUser> AppUsers { get; set; }

}
