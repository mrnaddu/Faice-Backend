using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Data;

public class FaiceDbContext(DbContextOptions<FaiceDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
