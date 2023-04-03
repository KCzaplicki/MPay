using MPay.Abstractions.Exceptions;

namespace MPay.Core.Exceptions;

public class PurchasePaymentNotProcessedException : MPayException
{
    public string PurchaseId { get; }
    public string PurchasePaymentId { get; }

    public PurchasePaymentNotProcessedException(string purchaseId, string purchasePaymentId) : base($"Payment with id '{purchasePaymentId}' for purchase with id '{purchaseId}' can't be processed.")
    {
        PurchaseId = purchaseId;
        PurchasePaymentId = purchasePaymentId;
    }
}