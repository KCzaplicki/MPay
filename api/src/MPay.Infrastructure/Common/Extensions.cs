using Microsoft.Extensions.DependencyInjection;
using MPay.Abstractions.Common;

namespace MPay.Infrastructure.Common;

internal static class Extensions
{
    internal static void AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<IClock, UtcClock>();
    }
}