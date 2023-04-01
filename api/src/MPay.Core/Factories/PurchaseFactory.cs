using AutoMapper;

namespace MPay.Core.Factories;

internal class PurchaseFactory : IPurchaseFactory
{
    private readonly IMapper _mapper;

    public PurchaseFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Purchase Create(AddPurchaseDto addPurchaseDto)
    {
        var purchase = _mapper.Map<Purchase>(addPurchaseDto);

        purchase.Id = Guid.NewGuid().ToString();
        purchase.CreatedAt = DateTime.UtcNow;
        purchase.Status = PurchaseStatus.Pending;
        purchase.Currency = purchase.Currency.ToUpperInvariant();
        purchase.Payments = new List<PurchasePayment>();

        return purchase;
    }
}