using Faice_Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Data;

public class FaiceDbContext(DbContextOptions<FaiceDbContext> options)
        : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
