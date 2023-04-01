using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MPay.Core.Configurations;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Policies.PurchaseTimeout;
using MPay.Core.Validators;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Core;

internal static class Extensions
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddPurchaseDto>, AddPurchaseValidator>();
        services.AddScoped<IValidator<PurchasePaymentDto>, PurchasePaymentValidator>();
    }

    public static void AddPolicies(this IServiceCollection services)
    {
        // Purchase timeout policies
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseCreationTimeoutPolicy>(sp => 
            new PurchaseCreationTimeoutPolicy(sp.GetRequiredService<IOptions<PurchaseTimeoutOptions>>().Value.PurchaseCreationTimeoutInMinutes));
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseLastPaymentTimeoutPolicy>(sp => 
            new PurchaseLastPaymentTimeoutPolicy(sp.GetRequiredService<IOptions<PurchaseTimeoutOptions>>().Value.PurchaseLastPaymentTimeoutInMinutes));
        services.AddSingleton<IPurchaseTimeoutPolicy, PurchaseWithPaymentsTimeoutPolicy>(sp =>
            new PurchaseWithPaymentsTimeoutPolicy(sp.GetRequiredService<IOptions<PurchaseTimeoutOptions>>().Value.PurchaseWithPaymentsTimeoutInMinutes));

        // Purchase payment status policies
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusCompletePolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusInvalidCardPolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusCardNoFoundsPolicy>();
        services.AddSingleton<IPurchasePaymentStatusPolicy, PurchasePaymentStatusTimeoutPolicy>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseTimeoutHandler, PurchaseTimeoutHandler>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IPurchasePaymentService, PurchasePaymentService>();
    }
}