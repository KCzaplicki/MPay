namespace MPay.Core.Policies.PurchaseTimeout;

internal class PurchaseWithPaymentsTimeoutPolicy : IPurchaseTimeoutPolicy
{
    private readonly TimeSpan _timeout;

    public PurchaseWithPaymentsTimeoutPolicy(int purchaseWithPaymentsTimeoutInMinutes)
        => _timeout = TimeSpan.FromMinutes(purchaseWithPaymentsTimeoutInMinutes);

    public bool CanApply(Purchase purchase)
        => purchase.Status == PurchaseStatus.Pending && purchase.Payments.Any() && purchase.CreatedAt.Add(_timeout) < DateTime.UtcNow;
}