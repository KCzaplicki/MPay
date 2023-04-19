using Microsoft.Extensions.Logging;
using Moq;
using MPay.Core.Events;
using MPay.Core.Events.Handlers;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Tests.Events.Handlers;

public class PurchaseTimeoutHandlerTests
{
    [Fact]
    public async Task HandleAsync_Calls_Webhook()
    {
        // Arrange
        var purchaseTimeoutEvent = AutoFaker.Generate<PurchaseTimeout>();

        var mockWebClient = new Mock<IWebhookClient>();
        var mockLogger = new Mock<ILogger<PurchaseTimeoutHandler>>();
        var purchaseTimeoutHandler = new PurchaseTimeoutHandler(mockWebClient.Object, mockLogger.Object);

        // Act
        await purchaseTimeoutHandler.HandleAsync(purchaseTimeoutEvent);

        // Assert
        mockWebClient.Verify(x => x.SendAsync(It.Is<PurchaseUpdateResultDto>(
            p => p.Id == purchaseTimeoutEvent.Id && p.Status == PurchaseUpdateStatus.Timeout &&
                 p.UpdatedAt == purchaseTimeoutEvent.TimeoutAt)), Times.Once);
    }
}