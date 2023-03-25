using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.DAL;
using MPay.Core.DAL.Repositories;
using MPay.Core.Repository;
using MPay.Core.Validators;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Core;

internal static class Extensions
{
    public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MPayDbContext>(
            options => options.UseSqlite(configuration.GetConnectionString("MPayConnectionString")));
    }

    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void UseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MPayDbContext>();
        dbContext.Database.Migrate();
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddPurchaseDto>, AddPurchaseValidator>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
    }
}