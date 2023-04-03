using Microsoft.Extensions.Options;
using MPay.Abstractions.Common;
using MPay.Core.Configurations;

namespace MPay.Core.Policies.PurchaseTimeout;

internal class PurchaseWithPaymentsTimeoutPolicy : IPurchaseTimeoutPolicy
{
    private readonly IClock _clock;
    private readonly TimeSpan _timeout;

    public PurchaseWithPaymentsTimeoutPolicy(IClock clock, IOptions<PurchaseTimeoutOptions> options)
    {
        _clock = clock;
        _timeout = TimeSpan.FromMinutes(options.Value.PurchaseWithPaymentsTimeoutInMinutes);
    }

    public bool CanApply(Purchase purchase)
        => purchase.Status == PurchaseStatus.Pending && purchase.Payments.Any() && purchase.CreatedAt.Add(_timeout) < _clock.Now;
}