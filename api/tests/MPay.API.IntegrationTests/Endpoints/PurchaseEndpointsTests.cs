using System.Net;
using System.Net.Http.Json;
using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.API.IntegrationTests.Endpoints;

public class PurchaseEndpointsTests : BaseEndpointsTest
{
    private const string PurchaseEndpointBasePath = "/purchases";

    public PurchaseEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePurchaseEndpoint_CreatesPurchase()
    {
        // Arrange
        var client = Factory.CreateClient();
        var addPurchaseDto = new AddPurchaseDto("Test Purchase", "Test purchase description", 10, "EUR");

        // Act
        var response = await client.PostAsJsonAsync($"{PurchaseEndpointBasePath}", addPurchaseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var purchaseId = await response.Content.ReadAsStringAsync();
        purchaseId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetPurchaseEndpoint_ReturnsPurchase()
    {
        // Arrange
        var purchase = CreatePurchase();
        await InitializeDatabaseAsync(purchase);
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{PurchaseEndpointBasePath}/{purchase.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var purchaseDto = await response.GetResultAsync<PurchaseDto>();
        purchaseDto.Should().NotBeNull();
        purchaseDto.Id.Should().Be(purchase.Id);
        purchaseDto.Name.Should().Be(purchase.Name);
        purchaseDto.Description.Should().Be(purchase.Description);
        purchaseDto.Price.Should().Be(purchase.Price);
        purchaseDto.Currency.Should().Be(purchase.Currency);
    }

    [Fact]
    public async Task CancelPurchaseEndpoint_CancelsPurchase()
    {
        // Arrange
        var purchase = CreatePurchase();
        await InitializeDatabaseAsync(purchase);
        var client = Factory.CreateClient();

        // Act
        var response = await client.PostAsync($"{PurchaseEndpointBasePath}/{purchase.Id}/cancel", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PurchasePaymentEndpoint_CompletesPurchase_WhenPaymentWasSuccessful()
    {
        // Arrange
        var purchase = CreatePurchase();
        var purchasePaymentDto = CreatePurchasePaymentDto(1000000000000000);
        await InitializeDatabaseAsync(purchase);
        var client = Factory.CreateClient();

        // Act
        var response =
            await client.PostAsJsonAsync($"{PurchaseEndpointBasePath}/{purchase.Id}/payment", purchasePaymentDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PurchasePaymentEndpoint_ReturnsBadRequest_WhenPaymentFailed()
    {
        // Arrange
        var purchase = CreatePurchase();
        var purchasePaymentDto = CreatePurchasePaymentDto(1000000000000001);
        await InitializeDatabaseAsync(purchase);
        var client = Factory.CreateClient();

        // Act
        var response =
            await client.PostAsJsonAsync($"{PurchaseEndpointBasePath}/{purchase.Id}/payment", purchasePaymentDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static PurchasePaymentDto CreatePurchasePaymentDto(long cardNumber)
    {
        return new PurchasePaymentDto("Test Name", cardNumber, 100, DateTime.MaxValue);
    }

    private static Purchase CreatePurchase()
    {
        return new Purchase
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Purchase",
            Description = "Test purchase description",
            Price = 10,
            Currency = "EUR",
            Status = PurchaseStatus.Pending,
            Payments = new List<PurchasePayment>()
        };
    }
}