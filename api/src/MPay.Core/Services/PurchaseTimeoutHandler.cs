using Microsoft.Extensions.Logging;
using MPay.Core.Policies;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchaseTimeoutHandler : IPurchaseTimeoutHandler
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IEnumerable<IPurchaseTimeoutPolicy> _purchaseTimeoutPolicies;
    private readonly ILogger<PurchaseTimeoutHandler> _logger;

    public PurchaseTimeoutHandler(IPurchaseRepository purchaseRepository, IEnumerable<IPurchaseTimeoutPolicy> purchaseTimeoutPolicies, ILogger<PurchaseTimeoutHandler> logger)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseTimeoutPolicies = purchaseTimeoutPolicies;
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
            }
        }

        if (timedOutPurchases > 0)
        {
            _logger.LogInformation("Purchase timeout service executed. {timedOutPurchases} purchases timed out.", timedOutPurchases);
        }
    }
}