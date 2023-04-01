using Microsoft.Extensions.DependencyInjection;

namespace MPay.Infrastructure.Services;

internal static class Extensions
{
    internal static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<PurchaseTimeoutService>();
    }

}