using AutoMapper;
using MPay.Abstractions.Common;
using MPay.Abstractions.Events;
using MPay.Core.DTO;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Exceptions;
using MPay.Core.Factories;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchaseService : IPurchaseService
{
    private readonly IAsyncEventDispatcher _asyncEventDispatcher;
    private readonly IClock _clock;
    private readonly IMapper _mapper;
    private readonly IPurchaseFactory _purchaseFactory;
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(IPurchaseRepository purchaseRepository, IPurchaseFactory purchaseFactory,
        IAsyncEventDispatcher asyncEventDispatcher, IMapper mapper, IClock clock)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseFactory = purchaseFactory;
        _asyncEventDispatcher = asyncEventDispatcher;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<string> AddAsync(AddPurchaseDto addPurchaseDto)
    {
        var purchase = _purchaseFactory.Create(addPurchaseDto);
        await _purchaseRepository.AddAsync(purchase);

        _asyncEventDispatcher.PublishAsync(new PurchaseCreated(purchase.Id, purchase.CreatedAt));

        return purchase.Id;
    }

    public async Task<PurchaseDto> GetPendingAsync(string id)
    {
        var purchase = await _purchaseRepository.GetAsync(id);

        if (purchase is null) throw new PurchaseNotFoundException(id);
        if (purchase.Status != PurchaseStatus.Pending) throw new PurchaseStatusNotPendingException(id);

        return _mapper.Map<PurchaseDto>(purchase);
    }

    public async Task CancelPurchaseAsync(string id)
    {
        var purchase = await _purchaseRepository.GetAsync(id);

        if (purchase is null) throw new PurchaseNotFoundException(id);
        if (purchase.Status != PurchaseStatus.Pending) return;

        purchase.Status = PurchaseStatus.Cancelled;
        purchase.CompletedAt = _clock.Now;

        _asyncEventDispatcher.PublishAsync(new PurchaseCancelled(purchase.Id, purchase.CompletedAt.Value));

        await _purchaseRepository.UpdateAsync(purchase);
    }
}