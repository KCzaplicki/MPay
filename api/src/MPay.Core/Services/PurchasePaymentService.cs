using MPay.Core.Factories;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchasePaymentService : IPurchasePaymentService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IEnumerable<IPurchasePaymentStatusPolicy> _purchasePaymentStatusPolicies;
    private readonly IPurchasePaymentFactory _purchasePaymentFactory;

    public PurchasePaymentService(IPurchaseRepository purchaseRepository, IEnumerable<IPurchasePaymentStatusPolicy> purchasePaymentStatusPolicies, 
        IPurchasePaymentFactory purchasePaymentFactory)
    {
        _purchaseRepository = purchaseRepository;
        _purchasePaymentStatusPolicies = purchasePaymentStatusPolicies;
        _purchasePaymentFactory = purchasePaymentFactory;
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

        var purchasePayment = _purchasePaymentFactory.Create(purchase.Id, purchasePaymentDto);
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