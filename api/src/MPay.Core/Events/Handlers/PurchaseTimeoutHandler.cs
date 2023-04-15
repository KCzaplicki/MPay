using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Events.Handlers;

internal class PurchaseTimeoutHandler : IEventHandler<PurchaseTimeout>
{
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<PurchaseTimeoutHandler> _logger;

    public PurchaseTimeoutHandler(IWebhookClient webhookClient, ILogger<PurchaseTimeoutHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseTimeout @event)
    {
        _logger.LogInformation($"Purchase timeout event handled. Purchase Id: '{@event.Id}'.");
        await _webhookClient.SendAsync(new PurchaseUpdateResultDto(@event.Id, PurchaseUpdateStatus.Timeout, @event.TimeoutAt));
        _logger.LogInformation($"Purchase timeout event handled. Purchase Id: '{@event.Id}'.");
    }
}