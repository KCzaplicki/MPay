namespace MPay.Core.Policies;

internal class PurchaseCreationTimeoutPolicy : IPurchaseTimeoutPolicy 
{
    private readonly TimeSpan _timeout;

    public PurchaseCreationTimeoutPolicy(int purchaseCreationTimeoutInMinutes) 
        => _timeout = TimeSpan.FromMinutes(purchaseCreationTimeoutInMinutes);

    public bool CanApply(Purchase purchase)
        => !purchase.Payments.Any() && purchase.CreatedAt.Add(_timeout) < DateTime.UtcNow;
}