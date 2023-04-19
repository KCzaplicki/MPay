using Microsoft.Extensions.Logging;
using Moq;
using MPay.Core.Events;
using MPay.Core.Events.Handlers;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Tests.Events.Handlers;

public class PurchaseCreatedHandlerTests
{
    [Fact]
    public async Task HandleAsync_Calls_Webhook()
    {
        // Arrange
        var purchaseCreatedEvent = AutoFaker.Generate<PurchaseCreated>();

        var mockWebClient = new Mock<IWebhookClient>();
        var mockLogger = new Mock<ILogger<PurchaseCreatedHandler>>();
        var purchaseCreatedHandler = new PurchaseCreatedHandler(mockWebClient.Object, mockLogger.Object);

        // Act
        await purchaseCreatedHandler.HandleAsync(purchaseCreatedEvent);

        // Assert
        mockWebClient.Verify(x => x.SendAsync(It.Is<PurchaseUpdateResultDto>(
            p => p.Id == purchaseCreatedEvent.Id && p.Status == PurchaseUpdateStatus.Created &&
                 p.UpdatedAt == purchaseCreatedEvent.CreatedAt)), Times.Once);
    }
}