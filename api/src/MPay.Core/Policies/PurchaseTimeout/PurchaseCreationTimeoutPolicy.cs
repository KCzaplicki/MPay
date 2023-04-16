using Microsoft.Extensions.Options;
using MPay.Abstractions.Common;
using MPay.Core.Configurations;
using MPay.Core.Entities;

namespace MPay.Core.Policies.PurchaseTimeout;

internal class PurchaseCreationTimeoutPolicy : IPurchaseTimeoutPolicy
{
    private readonly IClock _clock;
    private readonly TimeSpan _timeout;

    public PurchaseCreationTimeoutPolicy(IClock clock, IOptions<PurchaseTimeoutOptions> options)
    {
        _clock = clock;
        _timeout = TimeSpan.FromMinutes(options.Value.PurchaseCreationTimeoutInMinutes);
    }

    public bool CanApply(Purchase purchase)
    {
        return purchase.Status == PurchaseStatus.Pending && !purchase.Payments.Any() &&
               purchase.CreatedAt.Add(_timeout) < _clock.Now;
    }
}