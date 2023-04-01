namespace MPay.Core.Policies.PurchasePaymentStatus;

internal interface IPurchasePaymentStatusPolicy
{
    int Priority { get; }

    bool CanApply(PurchasePayment purchasePayment);

    void Apply(PurchasePayment purchasePayment);
}