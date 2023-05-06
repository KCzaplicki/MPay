using MPay.Abstractions.Events;
using MPay.Core.DTO;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Exceptions;
using MPay.Core.Factories;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Repository;

namespace MPay.Core.Services;

internal class PurchasePaymentService : IPurchasePaymentService
{
    private readonly IAsyncEventDispatcher _asyncEventDispatcher;
    private readonly IPurchasePaymentFactory _purchasePaymentFactory;
    private readonly IPurchasePaymentRepository _purchasePaymentRepository;
    private readonly IEnumerable<IPurchasePaymentStatusPolicy> _purchasePaymentStatusPolicies;
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchasePaymentService(IPurchaseRepository purchaseRepository,
        IPurchasePaymentRepository purchasePaymentRepository,
        IEnumerable<IPurchasePaymentStatusPolicy> purchasePaymentStatusPolicies,
        IPurchasePaymentFactory purchasePaymentFactory, IAsyncEventDispatcher asyncEventDispatcher)
    {
        _purchaseRepository = purchaseRepository;
        _purchasePaymentRepository = purchasePaymentRepository;
        _purchasePaymentStatusPolicies = purchasePaymentStatusPolicies;
        _purchasePaymentFactory = purchasePaymentFactory;
        _asyncEventDispatcher = asyncEventDispatcher;
    }

    public async Task<PurchasePaymentResultDto> ProcessPaymentAsync(string id, PurchasePaymentDto purchasePaymentDto)
    {
        var purchase = await _purchaseRepository.GetAsync(id);

        if (purchase is null) throw new PurchaseNotFoundException(id);
        if (purchase.Status != PurchaseStatus.Pending) throw new PurchaseStatusNotPendingException(id);

        var purchasePayment = _purchasePaymentFactory.Create(purchase.Id, purchasePaymentDto);
        var purchasePaymentStatusPolicy = _purchasePaymentStatusPolicies
            .OrderByDescending(p => p.Priority)
            .FirstOrDefault(p => p.CanApply(purchasePayment));
        purchasePaymentStatusPolicy?.Apply(purchasePayment);

        if (purchasePayment.Status == default)
            throw new PurchasePaymentNotProcessedException(purchasePayment.PurchaseId, purchasePayment.Id);

        await _purchasePaymentRepository.AddAsync(purchasePayment);

        if (purchasePayment.Status == PurchasePaymentStatus.Completed)
        {
            purchase.CompletedAt = purchasePayment.CreatedAt;
            purchase.Status = PurchaseStatus.Completed;
            await _purchaseRepository.UpdateAsync(purchase);

            await _asyncEventDispatcher.PublishAsync(new PurchaseCompleted(purchase.Id, purchase.CompletedAt.Value));
        }

        return new PurchasePaymentResultDto(purchase.Id, purchasePayment.Id, purchasePayment.Status);
    }
}