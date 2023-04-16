using MPay.Core.DTO;

namespace MPay.Core.Services;

public interface IPurchasePaymentService
{
    Task<PurchasePaymentResultDto> ProcessPaymentAsync(string id, PurchasePaymentDto purchasePaymentDto);
}