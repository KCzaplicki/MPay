using Microsoft.Extensions.Options;
using MPay.Core.Configurations;
using MPay.Core.Entities;
using MPay.Core.Policies.PurchaseTimeout;

namespace MPay.Core.Tests.Policies.PurchaseTimeout;

public class PurchaseLastPaymentTimeoutPolicyTests
{
    private readonly IOptions<PurchaseTimeoutOptions> _purchaseTimeoutOptions;

    public PurchaseLastPaymentTimeoutPolicyTests()
    {
        _purchaseTimeoutOptions = Options.Create(new PurchaseTimeoutOptions
        {
            PurchaseLastPaymentTimeoutInMinutes = 5
        });
    }

    [Fact]
    public void CanApply_ReturnsTrue_WhenLastPaymentCreationIsOlderThanTimeout()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>
            {
                new()
                {
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                }
            }
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseLastPaymentTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);

        // Act
        var result = policy.CanApply(purchase);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenLastPaymentCreationIsNewerThanTimeout()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>
            {
                new()
                {
                    CreatedAt = DateTime.UtcNow.AddMinutes(10)
                }
            }
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseLastPaymentTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);

        // Act
        var result = policy.CanApply(purchase);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchaseHasNoPayments()
    {
        // Arrange
        var purchase = new Purchase
        {
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>()
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseLastPaymentTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);

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
            Payments = new List<PurchasePayment>
            {
                new()
                {
                    CreatedAt = DateTime.UtcNow.AddMinutes(10)
                }
            }
        };
        var mockClock = MockClockFactory.Create(DateTime.UtcNow);
        var policy = new PurchaseLastPaymentTimeoutPolicy(mockClock.Object, _purchaseTimeoutOptions);

        // Act
        var result = policy.CanApply(purchase);

        // Assert
        Assert.False(result);
    }
}