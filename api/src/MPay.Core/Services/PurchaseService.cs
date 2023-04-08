using AutoMapper;
using MPay.Abstractions.Common;
using MPay.Core.Factories;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPurchaseFactory _purchaseFactory;
    private readonly IMapper _mapper;
    private readonly IClock _clock;

    public PurchaseService(IPurchaseRepository purchaseRepository, IPurchaseFactory purchaseFactory, IMapper mapper, IClock clock)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseFactory = purchaseFactory;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<string> AddAsync(AddPurchaseDto addPurchaseDto)
    {
        var purchase = _purchaseFactory.Create(addPurchaseDto);
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
        purchase.CompletedAt = _clock.Now;

        await _purchaseRepository.UpdateAsync(purchase);
    }
}