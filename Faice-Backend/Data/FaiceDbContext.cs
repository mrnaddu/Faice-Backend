using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Data;

public class FaiceDbContext
    : DbContext
{
    public FaiceDbContext(DbContextOptions<FaiceDbContext> options)
        : base(options)
    {
    }

}
