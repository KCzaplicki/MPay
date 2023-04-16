using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MPay.Abstractions.Events;
using MPay.Core.DTO;
using MPay.Core.Events;
using MPay.Core.Events.Handlers;
using MPay.Core.Factories;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Policies.PurchaseTimeout;
using MPay.Core.Services;
using MPay.Core.Validators;
using PurchaseTimeoutHandler = MPay.Core.Services.PurchaseTimeoutHandler;

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

    internal static void AddEvents(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler<PurchaseCreated>, PurchaseCreatedHandler>();
        services.AddScoped<IEventHandler<PurchaseCancelled>, PurchaseCancelledHandler>();
        services.AddScoped<IEventHandler<PurchaseTimeout>, Events.Handlers.PurchaseTimeoutHandler>();
        services.AddScoped<IEventHandler<PurchaseCompleted>, PurchaseCompletedHandler>();
    }
}