using AutoMapper;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public PurchaseService(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<string> AddAsync(AddPurchaseDto addPurchaseDto)
    {
        var purchase = _mapper.Map<Purchase>(addPurchaseDto);
        purchase.Id = Guid.NewGuid().ToString();
        purchase.CreatedAt = DateTime.UtcNow;
        purchase.Status = PurchaseStatus.Pending;
        purchase.Currency = purchase.Currency.ToUpperInvariant();
        purchase.Payments = new List<PurchasePayment>();

        await _purchaseRepository.AddAsync(purchase);

        return purchase.Id;
    }

    public async Task<PurchaseDto> GetPendingAsync(string id)
    {
        var purchase = await _purchaseRepository.GetAsync(id);

        if (purchase is null)
        {
            throw new PurchaseNotFoundException(id);
        }
        if (purchase.Status != PurchaseStatus.Pending)
        {
            throw new PurchaseStatusNotPendingException(id);
        }

        return _mapper.Map<PurchaseDto>(purchase);
    }

    public async Task CancelPurchaseAsync(string id)
    {
        var purchase = await _purchaseRepository.GetAsync(id);
        
        if (purchase is null)
        {
            throw new PurchaseNotFoundException(id);
        }
        if (purchase.Status != PurchaseStatus.Pending)
        {
            return;
        }

        purchase.Status = PurchaseStatus.Cancelled;
        purchase.CompletedAt = DateTime.UtcNow;

        await _purchaseRepository.UpdateAsync(purchase);
    }
}