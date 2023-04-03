using AutoMapper;
using MPay.Abstractions.Common;

namespace MPay.Core.Factories;

internal class PurchasePaymentFactory : IPurchasePaymentFactory
{
    private readonly IMapper _mapper;
    private readonly IClock _clock;

    public PurchasePaymentFactory(IMapper mapper, IClock clock)
    {
        _mapper = mapper;
        _clock = clock;
    }

    public PurchasePayment Create(string purchaseId, PurchasePaymentDto purchasePaymentDto)
    {
        var purchasePayment = _mapper.Map<PurchasePayment>(purchasePaymentDto);

        purchasePayment.Id = Guid.NewGuid().ToString();
        purchasePayment.PurchaseId = purchaseId;
        purchasePayment.CreatedAt = _clock.Now;

        return purchasePayment;
    }
}