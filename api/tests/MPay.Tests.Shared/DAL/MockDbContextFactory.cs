using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MPay.Tests.Shared.DAL;

internal static class MockDbContextFactory
{
    public static TDbContext Create<TDbContext>([CallerMemberName] string databaseName = nameof(TDbContext), bool autoDetectChangesEnabled = true) where TDbContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        var constructorInfo = typeof(TDbContext).GetConstructor(new[] {typeof(DbContextOptions<TDbContext>)});
        var context = constructorInfo?.Invoke(new object[] {options}) as TDbContext;

        context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
        
        return context;
    }
}