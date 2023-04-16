using Microsoft.EntityFrameworkCore;
using MPay.Core.Entities;

namespace MPay.Infrastructure.DAL;

internal class MPayDbContext : DbContext
{
    public MPayDbContext(DbContextOptions<MPayDbContext> options) : base(options)
    {
    }

    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchasePayment> PurchasePayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}