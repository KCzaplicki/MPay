namespace MPay.Core.Services;

public interface IPurchasePaymentService
{
    Task ProcessPaymentAsync(string id, PurchasePaymentDto purchasePaymentDto);
}