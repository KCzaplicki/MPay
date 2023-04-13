using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCancelledHandler : IEventHandler<PurchaseCancelled>
{
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<PurchaseCancelledHandler> _logger;

    public PurchaseCancelledHandler(IWebhookClient webhookClient, ILogger<PurchaseCancelledHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCancelled @event)
    {
        _logger.LogInformation($"Purchase cancelled event handled. Purchase Id: '{@event.Id}'.");
        
        var succeeded = await _webhookClient.SendAsync(new PurchaseUpdateResult(@event.Id, PurchaseUpdateStatus.Cancelled, @event.CancelledAt));
        
        _logger.LogInformation($"Purchase cancelled event handled. Purchase Id: '{@event.Id}'. Webhook request succeeded: '{succeeded}'.");
    }
}