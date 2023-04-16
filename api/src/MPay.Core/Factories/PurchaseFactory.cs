using AutoMapper;
using MPay.Abstractions.Common;
using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.Core.Factories;

internal class PurchaseFactory : IPurchaseFactory
{
    private readonly IClock _clock;
    private readonly IMapper _mapper;

    public PurchaseFactory(IMapper mapper, IClock clock)
    {
        _mapper = mapper;
        _clock = clock;
    }

    public Purchase Create(AddPurchaseDto addPurchaseDto)
    {
        var purchase = _mapper.Map<Purchase>(addPurchaseDto);

        purchase.Id = Guid.NewGuid().ToString();
        purchase.CreatedAt = _clock.Now;
        purchase.Status = PurchaseStatus.Pending;
        purchase.Currency = purchase.Currency.ToUpperInvariant();
        purchase.Payments = new List<PurchasePayment>();

        return purchase;
    }
}