using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPay.Core.Entities;

namespace MPay.Infrastructure.DAL.Configurations;

internal class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(512);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Price).IsRequired().HasPrecision(10, 2);
        builder.Property(x => x.Status).IsRequired();
    }
}