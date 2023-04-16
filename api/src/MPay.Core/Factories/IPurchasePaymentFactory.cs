using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.Core.Factories;

internal interface IPurchasePaymentFactory
{
    PurchasePayment Create(string purchaseId, PurchasePaymentDto purchasePaymentDto);
}