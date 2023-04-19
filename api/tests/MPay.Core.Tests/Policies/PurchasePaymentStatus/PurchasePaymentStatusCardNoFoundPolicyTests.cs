using MPay.Core.Entities;
using MPay.Core.Policies.PurchasePaymentStatus;

namespace MPay.Core.Tests.Policies.PurchasePaymentStatus;

public class PurchasePaymentStatusCardNoFoundPolicyTests
{
    [Fact]
    public void CanApply_ReturnsTrue_WhenCardNumberEndsWith2()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 2
        };
        var policy = new PurchasePaymentStatusCardNoFoundsPolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenCardNumberDoesNotEndsWith2()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 3
        };
        var policy = new PurchasePaymentStatusCardNoFoundsPolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchasePaymentHasBeenProcessed()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = Entities.PurchasePaymentStatus.Completed,
            CardNumber = 2
        };
        var policy = new PurchasePaymentStatusCardNoFoundsPolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Apply_SetStatusToNoFounds()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 2
        };
        var policy = new PurchasePaymentStatusCardNoFoundsPolicy();

        // Act
        policy.Apply(purchasePayment);

        // Assert
        Assert.Equal(Entities.PurchasePaymentStatus.NoFounds, purchasePayment.Status);
    }
}