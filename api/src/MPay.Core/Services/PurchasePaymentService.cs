using AutoMapper;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchasePaymentService : IPurchasePaymentService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public PurchasePaymentService(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task ProcessPaymentAsync(string id, PurchasePaymentDto purchasePaymentDto)
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

        var purchasePayment = _mapper.Map<PurchasePayment>(purchasePaymentDto);
        purchasePayment.Id = Guid.NewGuid().ToString();
        purchasePayment.PurchaseId = purchase.Id;
        purchasePayment.Status = PurchasePaymentStatus.Completed;
        purchasePayment.CreatedAt = DateTime.UtcNow;
        
        purchase.CompletedAt = purchasePayment.CreatedAt;
        purchase.Status = PurchaseStatus.Completed;
        purchase.Payments.Add(purchasePayment);

        await _purchaseRepository.UpdateAsync(purchase);
    }
}