using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.Repository;
using MPay.Infrastructure.DAL.Repositories;
using MPay.Infrastructure.DAL.UnitOfWork;

namespace MPay.Infrastructure.DAL;

internal static class Extensions
{
    private const string MPayConnectionString = "MPayConnectionString";

    public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MPayDbContext>(
            options => options.UseSqlite(configuration.GetConnectionString(MPayConnectionString)));
    }
    
    public static void UseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MPayDbContext>();
        dbContext.Database.Migrate();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IPurchasePaymentRepository, PurchasePaymentRepository>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
    }
}