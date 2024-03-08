using Faice_Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Data;

public class FaiceDbContext(DbContextOptions<FaiceDbContext> options)
        : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseInMemoryDatabase("Faice-Backend");
    }
}
