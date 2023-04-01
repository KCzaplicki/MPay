namespace MPay.Core.Policies.PurchasePaymentStatus;

internal class PurchasePaymentStatusCompletePolicy : IPurchasePaymentStatusPolicy
{
    public int Priority => 0;

    public bool CanApply(PurchasePayment purchasePayment)
        => purchasePayment.Status == default;

    public void Apply(PurchasePayment purchasePayment)
    {
        purchasePayment.Status = Entities.PurchasePaymentStatus.Completed;
    }
}