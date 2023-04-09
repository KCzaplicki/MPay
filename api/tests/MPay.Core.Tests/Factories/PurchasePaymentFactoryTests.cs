using MPay.Core.Factories;

namespace MPay.Core.Tests.Factories;

public class PurchasePaymentFactoryTests
{
    [Fact]
    public void Create_ReturnsPurchasePayment()
    {
        // Arrange
        var purchaseId = Guid.NewGuid().ToString();
        var purchasePaymentDto = new PurchasePaymentDtoFake().Generate();
        var mapper = MapperFactory.Create();
        var mockClock = MockClockFactory.Create();
        var purchasePaymentFactory = new PurchasePaymentFactory(mapper, mockClock.Object);

        // Act
        var purchasePayment = purchasePaymentFactory.Create(purchaseId, purchasePaymentDto);

        // Assert
        purchasePayment.Should().NotBeNull();
        purchasePayment.Id.Should().NotBeNullOrWhiteSpace();
        purchasePayment.PurchaseId.Should().Be(purchaseId);
        purchasePayment.CardHolderName.Should().Be(purchasePaymentDto.CardHolderName);
        purchasePayment.CardNumber.Should().Be(purchasePaymentDto.CardNumber);
        purchasePayment.Ccv.Should().Be(purchasePaymentDto.Ccv);
        purchasePayment.CardExpiry.Should().Be(purchasePaymentDto.CardExpiry);
        purchasePayment.CreatedAt.Should().Be(mockClock.Object.Now);
        purchasePayment.Status.Should().Be(default);
    }
}