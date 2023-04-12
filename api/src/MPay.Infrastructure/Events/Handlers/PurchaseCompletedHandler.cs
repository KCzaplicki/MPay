using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCompletedHandler : IEventHandler<PurchaseCompleted>
{
    private readonly ILogger<PurchaseCompletedHandler> _logger;
    
    public PurchaseCompletedHandler(ILogger<PurchaseCompletedHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCompleted @event)
    {
        _logger.LogInformation("Purchase completed event handled. Purchase Id: '{@event.Id}'.");
    }
}