using Microsoft.Extensions.Options;
using MPay.Core.Configurations;
using MPay.Core.Entities;
using MPay.Core.Policies.PurchaseTimeout;

namespace MPay.Core.Tests.Policies.PurchaseTimeout;

public class PurchaseCreationTimeoutPolicyTests
{
    private readonly IOptions<PurchaseTimeoutOptions> _purchaseTimeoutOptions;

    public PurchaseCreationTimeoutPolicyTests()
    {
        _purchaseTimeoutOptions = Options.Create(new PurchaseTimeoutOptions
        {
            PurchaseCreationTimeoutInMinutes = 5
        });
    }
    
    [Fact]
    public void CanApply_ReturnsTrue_WhenPurchaseCreationIsOlderThanTimeout()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>(),
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseCreationTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);
        
        // Act
        var result = policy.CanApply(purchase);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchaseCreationIsNewerThanTimeout()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>(),
            CreatedAt = DateTime.UtcNow.AddMinutes(10)
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseCreationTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);
        
        // Act
        var result = policy.CanApply(purchase);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchaseHasPayments()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>
            {
                new()
            },
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseCreationTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);
        
        // Act
        var result = policy.CanApply(purchase);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchaseStatusIsNotPending()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Completed,
            Payments = new List<PurchasePayment>(),
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseCreationTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);
        
        // Act
        var result = policy.CanApply(purchase);
        
        // Assert
        Assert.False(result);
    }
}