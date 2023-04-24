using Microsoft.AspNetCore.Http;
using MPay.Core.DTO;
using MPay.Infrastructure.ErrorHandling;

namespace MPay.Infrastructure.Tests.ErrorHandling;

public class ErrorDetailsExtensionsTests
{
    [Fact]
    public void MapFrom_MapPurchasePaymentResultDtoToErrorDetails()
    {
        // Arrange
        var purchasePaymentResultDto = AutoFaker.Generate<PurchasePaymentResultDto>();
        
        // Act
        var errorDetails = ErrorDetailsExtensions.MapFrom(purchasePaymentResultDto);
        
        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Title.Should().Be($"Payment with id '{purchasePaymentResultDto.PurchaseId}' failed with status '{purchasePaymentResultDto.PurchasePaymentStatus}'");
        errorDetails.ErrorCode.Should().Be("PURCHASE_PAYMENT_FAILED");
        errorDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        errorDetails.Data.Should().NotBeNull();
        errorDetails.Data.Should().ContainKey("purchase_id");
        errorDetails.Data.Should().ContainKey("purchase_payment_id");
        errorDetails.Data.Should().ContainKey("purchase_payment_status");
        errorDetails.Data["purchase_id"].Should().Be(purchasePaymentResultDto.PurchaseId);
        errorDetails.Data["purchase_payment_id"].Should().Be(purchasePaymentResultDto.PurchasePaymentId);
        errorDetails.Data["purchase_payment_status"].Should().Be(purchasePaymentResultDto.PurchasePaymentStatus);
    }
    
    [Fact]
    public void MapFrom_ThrowNotSupportedException_WhenMappingNotFound()
    {
        // Arrange
        var fromObject = new object();
        
        // Act
        Func<ErrorDetails> act = () => ErrorDetailsExtensions.MapFrom(fromObject);
        
        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}