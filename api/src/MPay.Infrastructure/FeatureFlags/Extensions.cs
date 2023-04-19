using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Abstractions.FeatureFlags;

namespace MPay.Infrastructure.FeatureFlags;

internal static class Extensions
{
    internal static void AddFeatureFlags(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FeatureFlagsOptions>(
            configuration.GetSection(Infrastructure.Extensions.GetOptionsSectionName<FeatureFlagsOptions>()));
        services.AddScoped<IFeatureFlagsService, FeatureFlagsService>();
    }
}