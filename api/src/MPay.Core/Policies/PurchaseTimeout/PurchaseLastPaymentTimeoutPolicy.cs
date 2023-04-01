namespace MPay.Core.Policies.PurchaseTimeout;

internal class PurchaseLastPaymentTimeoutPolicy : IPurchaseTimeoutPolicy
{
    private readonly TimeSpan _timeout;

    public PurchaseLastPaymentTimeoutPolicy(int purchaseLastPaymentTimeoutInMinutes)
        => _timeout = TimeSpan.FromMinutes(purchaseLastPaymentTimeoutInMinutes);

    public bool CanApply(Purchase purchase)
        => purchase.Status == PurchaseStatus.Pending && purchase.Payments.Any() &&
           purchase.Payments.OrderByDescending(x => x.CreatedAt)
                            .First().CreatedAt.Add(_timeout) < DateTime.UtcNow;

}