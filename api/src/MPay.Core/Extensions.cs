using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.Validators;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Core;

internal static class Extensions
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddPurchaseDto>, AddPurchaseValidator>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
    }
}