using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Core.Events.Handlers;

internal class PurchaseCompletedHandler : IEventHandler<PurchaseCompleted>
{
    private readonly ILogger<PurchaseCompletedHandler> _logger;
    private readonly IWebhookClient _webhookClient;

    public PurchaseCompletedHandler(IWebhookClient webhookClient, ILogger<PurchaseCompletedHandler> logger)
    {
        _webhookClient = webhookClient;
        _logger = logger;
    }

    public async Task HandleAsync(PurchaseCompleted @event)
    {
        _logger.LogInformation($"Purchase completed event handled. Purchase Id: '{@event.Id}'.");
        await _webhookClient.SendAsync(new PurchaseUpdateResultDto(@event.Id, PurchaseUpdateStatus.Completed,
            @event.CompletedAt));
        _logger.LogInformation($"Purchase completed event handled. Purchase Id: '{@event.Id}'");
    }
}