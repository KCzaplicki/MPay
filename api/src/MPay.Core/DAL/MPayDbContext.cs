using Microsoft.EntityFrameworkCore;
using MPay.Core.Entities;

namespace MPay.Core.DAL;

internal class MPayDbContext : DbContext
{
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchasePayment> PurchasePayments { get; set; }

    public MPayDbContext(DbContextOptions<MPayDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}