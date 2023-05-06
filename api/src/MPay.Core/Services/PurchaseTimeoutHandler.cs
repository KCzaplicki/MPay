using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Policies.PurchaseTimeout;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchaseTimeoutHandler : IPurchaseTimeoutHandler
{
    private readonly IAsyncEventDispatcher _asyncEventDispatcher;
    private readonly ILogger<PurchaseTimeoutHandler> _logger;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IEnumerable<IPurchaseTimeoutPolicy> _purchaseTimeoutPolicies;

    public PurchaseTimeoutHandler(IPurchaseRepository purchaseRepository,
        IEnumerable<IPurchaseTimeoutPolicy> purchaseTimeoutPolicies, IAsyncEventDispatcher asyncEventDispatcher,
        ILogger<PurchaseTimeoutHandler> logger)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseTimeoutPolicies = purchaseTimeoutPolicies;
        _asyncEventDispatcher = asyncEventDispatcher;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var pendingPurchases = await _purchaseRepository.GetPendingAsync();
        var timedOutPurchases = 0;

        foreach (var pendingPurchase in pendingPurchases)
        {
            var purchaseTimeout = _purchaseTimeoutPolicies.Any(p => p.CanApply(pendingPurchase));
            if (purchaseTimeout)
            {
                pendingPurchase.Status = PurchaseStatus.Timeout;
                pendingPurchase.CompletedAt = DateTime.UtcNow;
                await _purchaseRepository.UpdateAsync(pendingPurchase);
                timedOutPurchases++;

                await _asyncEventDispatcher.PublishAsync(new PurchaseTimeout(pendingPurchase.Id,
                    pendingPurchase.CompletedAt.Value));
            }
        }

        if (timedOutPurchases > 0)
            _logger.LogInformation("Purchase timeout service executed. {timedOutPurchases} purchases timed out.",
                timedOutPurchases);
    }
}