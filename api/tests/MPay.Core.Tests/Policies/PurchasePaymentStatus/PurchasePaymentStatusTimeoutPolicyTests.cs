using MPay.Core.Entities;
using MPay.Core.Policies.PurchasePaymentStatus;

namespace MPay.Core.Tests.Policies.PurchasePaymentStatus;

public class PurchasePaymentStatusTimeoutPolicyTests
{
    [Fact]
    public void CanApply_ReturnsTrue_WhenCardNumberEndsWith3()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 3
        };
        var policy = new PurchasePaymentStatusTimeoutPolicy();
        
        // Act
        var result = policy.CanApply(purchasePayment);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void CanApply_ReturnsFalse_WhenCardNumberDoesNotEndsWith3()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 4
        };
        var policy = new PurchasePaymentStatusTimeoutPolicy();
        
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
            CardNumber = 3
        };
        var policy = new PurchasePaymentStatusTimeoutPolicy();
        
        // Act
        var result = policy.CanApply(purchasePayment);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Apply_SetStatusToTimeout()
    {
        // Arrange
        var purchasePayment = new PurchasePayment
        {
            Status = default,
            CardNumber = 3
        };
        var policy = new PurchasePaymentStatusTimeoutPolicy();
        
        // Act
        policy.Apply(purchasePayment);
        
        // Assert
        Assert.Equal(Entities.PurchasePaymentStatus.Timeout, purchasePayment.Status);
    }
}