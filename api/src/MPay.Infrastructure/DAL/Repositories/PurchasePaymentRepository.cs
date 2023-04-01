using Microsoft.EntityFrameworkCore;
using MPay.Core.Entities;
using MPay.Core.Repository;

namespace MPay.Infrastructure.DAL.Repositories;

internal class PurchasePaymentRepository : IPurchasePaymentRepository
{
    private readonly MPayDbContext _context;
    private readonly DbSet<PurchasePayment> _purchasePayments;

    public PurchasePaymentRepository(MPayDbContext context)
    {
        _context = context;
        _purchasePayments = context.PurchasePayments;
    }

    public async Task AddAsync(PurchasePayment purchasePayment)
    {
        await _purchasePayments.AddAsync(purchasePayment);
        await _context.SaveChangesAsync();
    }
}