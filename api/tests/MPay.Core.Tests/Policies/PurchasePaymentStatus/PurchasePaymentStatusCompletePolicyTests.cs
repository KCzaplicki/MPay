using MPay.Core.Entities;
using MPay.Core.Policies.PurchasePaymentStatus;

namespace MPay.Core.Tests.Policies.PurchasePaymentStatus;

public class PurchasePaymentStatusCompletePolicyTests
{
    [Fact]
    public void CanApply_ReturnsTrue_WhenPurchasePaymentHasNotBeenProcessed()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default
        };
        var policy = new PurchasePaymentStatusCompletePolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanApply_ReturnsFalse_WhenPurchasePaymentHasBeenProcessed()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = Entities.PurchasePaymentStatus.Completed
        };
        var policy = new PurchasePaymentStatusCompletePolicy();

        // Act
        var result = policy.CanApply(purchasePayment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Apply_SetStatusToCompleted()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default
        };
        var policy = new PurchasePaymentStatusCompletePolicy();

        // Act
        policy.Apply(purchasePayment);

        // Assert
        Assert.Equal(Entities.PurchasePaymentStatus.Completed, purchasePayment.Status);
    }
}