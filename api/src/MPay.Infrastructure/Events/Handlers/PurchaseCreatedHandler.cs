using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCreatedHandler : IEventHandler<PurchaseCreated>
{
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<PurchaseCreatedHandler> _logger;

    public PurchaseCreatedHandler(IWebhookClient webhookClient, ILogger<PurchaseCreatedHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCreated @event)
    {
        _logger.LogInformation($"Handling purchase created event. Purchase Id: '{@event.Id}'.");
        
        var succeeded = await _webhookClient.SendAsync(new PurchaseUpdateResult(@event.Id, PurchaseUpdateStatus.Created, @event.CreatedAt));
        
        _logger.LogInformation($"Purchase created event handled. Purchase Id: '{@event.Id}'. Webhook request succeeded: '{succeeded}'.");
    }
}