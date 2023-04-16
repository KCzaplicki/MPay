using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Events.Handlers;

internal class PurchaseCancelledHandler : IEventHandler<PurchaseCancelled>
{
    private readonly ILogger<PurchaseCancelledHandler> _logger;
    private readonly IWebhookClient _webhookClient;

    public PurchaseCancelledHandler(IWebhookClient webhookClient, ILogger<PurchaseCancelledHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseCancelled @event)
    {
        _logger.LogInformation($"Purchase cancelled event handled. Purchase Id: '{@event.Id}'.");
        await _webhookClient.SendAsync(new PurchaseUpdateResultDto(@event.Id, PurchaseUpdateStatus.Cancelled,
            @event.CancelledAt));
        _logger.LogInformation($"Purchase cancelled event handled. Purchase Id: '{@event.Id}'.");
    }
}