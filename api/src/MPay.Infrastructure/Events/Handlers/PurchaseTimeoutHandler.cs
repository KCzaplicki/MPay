using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseTimeoutHandler : IEventHandler<PurchaseTimeout>
{
    private readonly ILogger<PurchaseTimeoutHandler> _logger;

    public PurchaseTimeoutHandler(ILogger<PurchaseTimeoutHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseTimeout @event)
    {
        _logger.LogInformation("Purchase timeout event handled. Purchase Id: '{@event.Id}'.");
    }
}