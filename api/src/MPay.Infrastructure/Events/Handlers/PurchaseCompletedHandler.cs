using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCompletedHandler : IEventHandler<PurchaseCompleted>
{
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<PurchaseCompletedHandler> _logger;
    
    public PurchaseCompletedHandler(IWebhookClient webhookClient, ILogger<PurchaseCompletedHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCompleted @event)
    {
        _logger.LogInformation($"Purchase completed event handled. Purchase Id: '{@event.Id}'.");
        
        var succeeded = await _webhookClient.SendAsync(new PurchaseUpdateResult(@event.Id, PurchaseUpdateStatus.Completed, @event.CompletedAt));
        
        _logger.LogInformation($"Purchase completed event handled. Purchase Id: '{@event.Id}'. Webhook request succeeded: '{succeeded}'.");
    }
}