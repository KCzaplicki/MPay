using Microsoft.Extensions.Logging;
using Moq;
using MPay.Core.Events;
using MPay.Core.Events.Handlers;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Tests.Events.Handlers;

public class PurchaseCancelledHandlerTests
{
    [Fact]
    public async Task HandleAsync_Calls_Webhook()
    {
        // Arrange
        var purchaseCancelledEvent = AutoFaker.Generate<PurchaseCancelled>();

        var mockWebClient = new Mock<IWebhookClient>();
        var mockLogger = new Mock<ILogger<PurchaseCancelledHandler>>();
        var purchaseCancelledHandler = new PurchaseCancelledHandler(mockWebClient.Object, mockLogger.Object);

        // Act
        await purchaseCancelledHandler.HandleAsync(purchaseCancelledEvent);

        // Assert
        mockWebClient.Verify(x => x.SendAsync(It.Is<PurchaseUpdateResultDto>(
            p => p.Id == purchaseCancelledEvent.Id && p.Status == PurchaseUpdateStatus.Cancelled && 
                 p.UpdatedAt == purchaseCancelledEvent.CancelledAt)), Times.Once);
    }
}