using MPay.Core.Entities;

namespace MPay.Core.Policies.PurchasePaymentStatus;

internal class PurchasePaymentStatusCompletePolicy : IPurchasePaymentStatusPolicy
{
    public int Priority => 0;

    public bool CanApply(PurchasePayment purchasePayment)
    {
        return purchasePayment.Status == default;
    }

    public void Apply(PurchasePayment purchasePayment)
    {
        purchasePayment.Status = Entities.PurchasePaymentStatus.Completed;
    }
}