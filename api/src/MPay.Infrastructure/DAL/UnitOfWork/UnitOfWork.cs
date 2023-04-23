namespace MPay.Infrastructure.DAL.UnitOfWork;

internal class UnitOfWork : IUnitOfWork
{
    private readonly MPayDbContext _context;

    public UnitOfWork(MPayDbContext context)
    {
        _context = context;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> executeExpression)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await executeExpression();
            await transaction.CommitAsync();

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}