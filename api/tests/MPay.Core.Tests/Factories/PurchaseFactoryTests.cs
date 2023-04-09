using MPay.Core.Entities;
using MPay.Core.Factories;

namespace MPay.Core.Tests.Factories;

public class PurchaseFactoryTests
{
    [Fact]
    public void Create_ReturnsPurchase()
    {
        // Arrange
        var addPurchaseDto = new AddPurchaseDtoFake().Generate();
        var mapper = MapperFactory.Create();
        var mockClock = MockClockFactory.Create();
        var purchaseFactory = new PurchaseFactory(mapper, mockClock.Object);

        // Act
        var purchase = purchaseFactory.Create(addPurchaseDto);

        // Assert
        purchase.Should().NotBeNull();
        purchase.Id.Should().NotBeNullOrWhiteSpace();
        purchase.Name.Should().Be(addPurchaseDto.Name);
        purchase.Price.Should().Be(addPurchaseDto.Price);
        purchase.Description.Should().Be(addPurchaseDto.Description);
        purchase.CreatedAt.Should().Be(mockClock.Object.Now);
        purchase.Status.Should().Be(PurchaseStatus.Pending);
        purchase.Currency.Should().Be(addPurchaseDto.Currency.ToUpperInvariant());
        purchase.Payments.Should().NotBeNull();
        purchase.Payments.Should().BeEmpty();
    }
}