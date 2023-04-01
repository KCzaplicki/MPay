using AutoMapper;

namespace MPay.Core.Factories;

internal class PurchasePaymentFactory : IPurchasePaymentFactory
{
    private readonly IMapper _mapper;

    public PurchasePaymentFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public PurchasePayment Create(string purchaseId, PurchasePaymentDto purchasePaymentDto)
    {
        var purchasePayment = _mapper.Map<PurchasePayment>(purchasePaymentDto);

        purchasePayment.Id = Guid.NewGuid().ToString();
        purchasePayment.PurchaseId = purchaseId;
        purchasePayment.CreatedAt = DateTime.UtcNow;

        return purchasePayment;
    }
}