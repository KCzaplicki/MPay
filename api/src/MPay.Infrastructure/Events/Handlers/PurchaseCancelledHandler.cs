using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCancelledHandler : IEventHandler<PurchaseCancelled>
{
    private readonly ILogger<PurchaseCancelledHandler> _logger;

    public PurchaseCancelledHandler(ILogger<PurchaseCancelledHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCancelled @event)
    {
        _logger.LogInformation("Purchase cancelled event handled. Purchase Id: '{@event.Id}'.");
    }
}