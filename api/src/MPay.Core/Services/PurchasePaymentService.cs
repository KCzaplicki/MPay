using AutoMapper;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchasePaymentService : IPurchasePaymentService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IEnumerable<IPurchasePaymentStatusPolicy> _purchasePaymentStatusPolicies;
    private readonly IMapper _mapper;

    public PurchasePaymentService(IPurchaseRepository purchaseRepository, IEnumerable<IPurchasePaymentStatusPolicy> purchasePaymentStatusPolicies, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _purchasePaymentStatusPolicies = purchasePaymentStatusPolicies;
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
        purchasePayment.CreatedAt = DateTime.UtcNow;

        var purchasePaymentStatusPolicy = _purchasePaymentStatusPolicies
            .OrderByDescending(p => p.Priority)
            .FirstOrDefault(p => p.CanApply(purchasePayment));
        purchasePaymentStatusPolicy?.Apply(purchasePayment);

        if (purchasePayment.Status == default)
        {
            throw new PurchasePaymentNotProcessedException(purchasePayment.PurchaseId, purchasePayment.Id);
        }

        if (purchasePayment.Status == PurchasePaymentStatus.Completed)
        {
            purchase.CompletedAt = purchasePayment.CreatedAt;
            purchase.Status = PurchaseStatus.Completed;
        }

        purchase.Payments.Add(purchasePayment);
        await _purchaseRepository.UpdateAsync(purchase);

        if (purchasePayment.Status != PurchasePaymentStatus.Completed)
        {
            throw new PurchasePaymentFailedException(purchasePayment.PurchaseId, purchasePayment.Id, purchasePayment.Status);
        }
    }
}