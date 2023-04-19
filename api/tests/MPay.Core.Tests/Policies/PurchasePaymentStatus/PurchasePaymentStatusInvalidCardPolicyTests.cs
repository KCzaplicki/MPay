using MPay.Core.Entities;
using MPay.Core.Policies.PurchasePaymentStatus;

namespace MPay.Core.Tests.Policies.PurchasePaymentStatus;

public class PurchasePaymentStatusInvalidCardPolicyTests
{
    [Fact]
    public void CanApply_ReturnsTrue_WhenCardNumberEndsWith1()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 1
        };
        var policy = new PurchasePaymentStatusInvalidCardPolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenCardNumberDoesNotEndsWith1()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 2
        };
        var policy = new PurchasePaymentStatusInvalidCardPolicy();

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
            CardNumber = 1
        };
        var policy = new PurchasePaymentStatusInvalidCardPolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Apply_SetStatusToInvalidCard()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 1
        };
        var policy = new PurchasePaymentStatusInvalidCardPolicy();

        // Act
        policy.Apply(purchasePayment);

        // Assert
        Assert.Equal(Entities.PurchasePaymentStatus.InvalidCard, purchasePayment.Status);
    }
}