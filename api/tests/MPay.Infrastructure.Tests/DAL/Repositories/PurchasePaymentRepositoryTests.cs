using MPay.Core.Entities;
using MPay.Infrastructure.DAL;
using MPay.Infrastructure.DAL.Repositories;
using MPay.Tests.Shared.DAL;

namespace MPay.Infrastructure.Tests.DAL.Repositories;

public class PurchasePaymentRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsPurchasePaymentToDatabase()
    {
        // Arrange
        var purchasePayment = CreatePurchasePayment();
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        var purchasePaymentRepository = new PurchasePaymentRepository(context);
        
        // Act
        await purchasePaymentRepository.AddAsync(purchasePayment);
        
        // Assert
        var result = await context.PurchasePayments.FindAsync(purchasePayment.Id);
        result.Should().BeEquivalentTo(purchasePayment);
    }

    private static PurchasePayment CreatePurchasePayment()
    {
        return new AutoFaker<PurchasePayment>()
            .Generate();
    }
}