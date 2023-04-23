using MPay.Core.Entities;
using MPay.Infrastructure.DAL;
using MPay.Infrastructure.DAL.Repositories;
using MPay.Tests.Shared.DAL;

namespace MPay.Infrastructure.Tests.DAL.Repositories;

public class PurchaseRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsPurchaseToDatabase()
    {
        // Arrange
        var purchase = CreatePurchase();
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        await purchaseRepository.AddAsync(purchase);
        
        // Assert
        var result = await context.Purchases.FindAsync(purchase.Id);
        result.Should().BeEquivalentTo(purchase);
    }
    
    [Fact]
    public async Task GetAsync_ReturnsPurchase()
    {
        // Arrange
        var purchase = CreatePurchase();
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        await context.Purchases.AddAsync(purchase);
        await context.SaveChangesAsync();
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        var result = await purchaseRepository.GetAsync(purchase.Id);
        
        // Assert
        result.Should().BeEquivalentTo(purchase);
    }

    [Fact]
    public async Task GetAsync_ReturnsPurchaseWithPayments()
    {
        // Arrange
        const int purchasePaymentsCount = 5;
        var purchase = CreatePurchase();
        purchase.Payments = CreatePurchasePayments(purchase.Id, purchasePaymentsCount);
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        await context.Purchases.AddAsync(purchase);
        await context.SaveChangesAsync();
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        var result = await purchaseRepository.GetAsync(purchase.Id);
        
        // Assert
        result.Should().BeEquivalentTo(purchase);
        result.Payments.Should().HaveCount(purchasePaymentsCount);
        result.Payments.Should().BeEquivalentTo(purchase.Payments);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPurchase()
    {
        // Arrange
        var purchase = CreatePurchase();
        await using var context = MockDbContextFactory.Create<MPayDbContext>();
        await context.Purchases.AddAsync(purchase);
        await context.SaveChangesAsync();
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        purchase.CompletedAt = new DateTime(2021, 1, 1);
        purchase.Status = PurchaseStatus.Cancelled;
        await purchaseRepository.UpdateAsync(purchase);
        context.ChangeTracker.Clear();
        
        // Assert
        var result = await context.Purchases.FindAsync(purchase.Id);
        result.Should().BeEquivalentTo(purchase);
    }
    
    [Fact]
    public async Task GetPendingAsync_ReturnsPendingPurchases()
    {
        // Arrange
        var pendingPurchase = CreatePurchase();
        var completedPurchase = CreatePurchase(PurchaseStatus.Completed);
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        await context.Purchases.AddRangeAsync(pendingPurchase, completedPurchase);
        await context.SaveChangesAsync();
        
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        var result = await purchaseRepository.GetPendingAsync();
        
        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainEquivalentOf(pendingPurchase);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsPendingPurchasesWithPayments()
    {
        // Arrange
        const int purchasePaymentsCount = 3;
        var pendingPurchase = CreatePurchase();
        pendingPurchase.Payments = CreatePurchasePayments(pendingPurchase.Id, purchasePaymentsCount);
        var completedPurchase = CreatePurchase(PurchaseStatus.Completed);
        completedPurchase.Payments = CreatePurchasePayments(completedPurchase.Id, purchasePaymentsCount);
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        await context.Purchases.AddRangeAsync(pendingPurchase, completedPurchase);
        await context.SaveChangesAsync();
        
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        var result = await purchaseRepository.GetPendingAsync();
        
        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainEquivalentOf(pendingPurchase);
        result.First().Payments.Should().HaveCount(purchasePaymentsCount);
        result.First().Payments.Should().BeEquivalentTo(pendingPurchase.Payments);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsEmptyList()
    {
        // Arrange
        var completedPurchase = CreatePurchase(PurchaseStatus.Completed);
        await using var context = MockDbContextFactory.Create<MPayDbContext>(autoDetectChangesEnabled: false);
        await context.Purchases.AddAsync(completedPurchase);
        await context.SaveChangesAsync();
        
        var purchaseRepository = new PurchaseRepository(context);
        
        // Act
        var result = await purchaseRepository.GetPendingAsync();
        
        // Assert
        result.Should().BeEmpty();
    }
    
    private static Purchase CreatePurchase(PurchaseStatus status = PurchaseStatus.Pending)
    {
        return new AutoFaker<Purchase>()
            .RuleFor(x => x.Status, status)
            .RuleFor(x => x.Payments, (List<PurchasePayment>)null)
            .Generate();
    }

    private static IList<PurchasePayment> CreatePurchasePayments(string purchaseId, int count)
    {
        return new AutoFaker<PurchasePayment>()
            .RuleFor(x => x.PurchaseId, purchaseId)
            .Generate(count);
    }
}