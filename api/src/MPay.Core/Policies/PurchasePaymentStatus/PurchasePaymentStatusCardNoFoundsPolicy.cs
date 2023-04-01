namespace MPay.Core.Policies.PurchasePaymentStatus;

internal class PurchasePaymentStatusCardNoFoundsPolicy : IPurchasePaymentStatusPolicy
{
    public int Priority => 10;

    public bool CanApply(PurchasePayment purchasePayment) 
        => purchasePayment.Status == default && purchasePayment.CardNumber.ToString().EndsWith("2");

    public void Apply(PurchasePayment purchasePayment)
    {
        purchasePayment.Status = Entities.PurchasePaymentStatus.NoFounds;
    }
}