using Microsoft.Extensions.DependencyInjection;

namespace MPay.Tests.Shared.Common;

internal static class MockServiceProviderFactory
{
    public static ServiceProvider Create(Action<ServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        configureServices?.Invoke(services);
        return services.BuildServiceProvider();
    }
}