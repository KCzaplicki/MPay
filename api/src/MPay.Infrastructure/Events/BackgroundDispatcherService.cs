using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Events;

public class BackgroundDispatcherService : BackgroundService
{
    private readonly IEventChannel _eventChannel;
    private readonly ILogger<BackgroundDispatcherService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundDispatcherService(IServiceProvider serviceProvider, IEventChannel eventChannel,
        ILogger<BackgroundDispatcherService> logger)
    {
        _serviceProvider = serviceProvider;
        _eventChannel = eventChannel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background dispatcher service is starting.");

        await using var scope = _serviceProvider.CreateAsyncScope();

        await foreach (var @event in _eventChannel.Reader.ReadAllAsync(stoppingToken))
            try
            {
                var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(eventHandlerType);
                var tasks = new List<Task>();

                foreach (var eventHandler in eventHandlers)
                {
                    var handleAsyncMethod = eventHandlerType.GetMethod(nameof(IEventHandler<IEvent>.HandleAsync));
                    var task = (Task)handleAsyncMethod.Invoke(eventHandler, new object[] { @event });
                    tasks.Add(task);

                    _logger.LogInformation(
                        $"Event '{@event.GetType().Name}' is being handled by '{eventHandler.GetType().Name}'. Event payload: '{@event}'.");
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling event.");
            }

        _logger.LogInformation("Background dispatcher service is stopping.");
    }
}