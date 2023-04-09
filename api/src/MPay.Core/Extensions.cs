using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.Factories;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Policies.PurchaseTimeout;
using MPay.Core.Services;
using MPay.Core.Validators;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MPay.Api")]
[assembly: InternalsVisibleTo("MPay.Core.Tests")]
[assembly: InternalsVisibleTo("MPay.Tests.Shared")]
namespace MPay.Core;

internal static class Extensions
{
    internal static void AddFactories(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseFactory, PurchaseFactory>();
        services.AddScoped<IPurchasePaymentFactory, PurchasePaymentFactory>();
    }

    internal static void AddPolicies(this IServiceCollection services)
    {
        // Purchase timeout policies
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseCreationTimeoutPolicy>();
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseLastPaymentTimeoutPolicy>();
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseWithPaymentsTimeoutPolicy>();

        // Purchase payment status policies
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusCompletePolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusInvalidCardPolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusCardNoFoundsPolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusTimeoutPolicy>();
    }

    internal static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseTimeoutHandler, PurchaseTimeoutHandler>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IPurchasePaymentService, PurchasePaymentService>();
    }

    internal static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddPurchaseDto>, AddPurchaseValidator>();
        services.AddScoped<IValidator<PurchasePaymentDto>, PurchasePaymentValidator>();
    }
}