using Microsoft.Extensions.Logging;
using Moq;
using MPay.Core.Events;
using MPay.Core.Events.Handlers;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Tests.Events.Handlers;

public class PurchaseCompletedHandlerTests
{
    [Fact]
    public async Task HandleAsync_Calls_Webhook()
    {
        // Arrange
        var purchaseCompletedEvent = AutoFaker.Generate<PurchaseCompleted>();

        var mockWebClient = new Mock<IWebhookClient>();
        var mockLogger = new Mock<ILogger<PurchaseCompletedHandler>>();
        var purchaseCompletedHandler = new PurchaseCompletedHandler(mockWebClient.Object, mockLogger.Object);

        // Act
        await purchaseCompletedHandler.HandleAsync(purchaseCompletedEvent);

        // Assert
        mockWebClient.Verify(x => x.SendAsync(It.Is<PurchaseUpdateResultDto>(
            p => p.Id == purchaseCompletedEvent.Id && p.Status == PurchaseUpdateStatus.Completed &&
                 p.UpdatedAt == purchaseCompletedEvent.CompletedAt)), Times.Once);
    }
}