namespace MPay.Core.Exceptions;

public class PurchasePaymentFailedException : MPayException
{
    public string PurchaseId { get; }
    public string PurchasePaymentId { get; }
    public PurchasePaymentStatus PurchasePaymentStatus { get; }

    public PurchasePaymentFailedException(string purchaseId, string purchasePaymentId, PurchasePaymentStatus purchasePaymentStatus) : base($"Payment with id '{purchasePaymentId}' for purchase with id '{purchaseId}' failed with status '{purchasePaymentStatus}'.")
    {
        PurchaseId = purchaseId;
        PurchasePaymentId = purchasePaymentId;
        PurchasePaymentStatus = purchasePaymentStatus;
    }
}