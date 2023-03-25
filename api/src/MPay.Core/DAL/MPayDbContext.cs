using Microsoft.EntityFrameworkCore;

namespace MPay.Core.DAL;

internal class MPayDbContext : DbContext
{
    public MPayDbContext(DbContextOptions<MPayDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}