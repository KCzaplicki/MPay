using Microsoft.EntityFrameworkCore;
using MPay.Core.Repository;

namespace MPay.Core.DAL.Repositories;

internal class PurchaseRepository : IPurchaseRepository
{
    private readonly MPayDbContext _context;
    private readonly DbSet<Purchase> _purchases;

    public PurchaseRepository(MPayDbContext context)
    {
        _context = context;
        _purchases = context.Purchases;
    }

    public async Task AddAsync(Purchase purchase)
    {
        await _purchases.AddAsync(purchase);
        await _context.SaveChangesAsync();
    }

    public Task<Purchase> GetAsync(string id)
        => _purchases.FirstOrDefaultAsync(p => p.Id == id);
}