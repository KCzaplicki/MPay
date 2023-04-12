using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Events;

namespace MPay.Infrastructure.Events.Handlers;

internal class PurchaseCreatedHandler : IEventHandler<PurchaseCreated>
{
    private readonly ILogger<PurchaseCreatedHandler> _logger;

    public PurchaseCreatedHandler(ILogger<PurchaseCreatedHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleAsync(PurchaseCreated @event)
    {
        _logger.LogInformation("Purchase created event handled. Purchase Id: '{@event.Id}'.");
    }
}