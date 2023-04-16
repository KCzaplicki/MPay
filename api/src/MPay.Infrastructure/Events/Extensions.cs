using Microsoft.Extensions.DependencyInjection;
using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Events;

internal static class Extensions
{
    internal static void AddEventsBackgroundHandling(this IServiceCollection services)
    {
        services.AddSingleton<IAsyncEventDispatcher, AsyncEventDispatcher>();
        services.AddSingleton<IEventChannel, EventChannel>();
        services.AddHostedService<BackgroundDispatcherService>();
    }
}