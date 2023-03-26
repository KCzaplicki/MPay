using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPay.Core.Entities;

namespace MPay.Infrastructure.DAL.Configurations;

internal class PurchasePaymentConfiguration : IEntityTypeConfiguration<PurchasePayment>
{
    public void Configure(EntityTypeBuilder<PurchasePayment> builder)
    {
        builder.ToTable("PurchasePayments");
        builder.Property(x => x.CardHolderName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CardExpiry).IsRequired();
        builder.Property(x => x.CardNumber).IsRequired().HasMaxLength(16);
        builder.Property(x => x.Ccv).IsRequired().HasMaxLength(3);
        builder.Property(x => x.PurchaseId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}