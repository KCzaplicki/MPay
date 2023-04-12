using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MPay.Core.Configurations;
using MPay.Core.Services;

namespace MPay.Infrastructure.Services;

internal class PurchaseTimeoutService : BackgroundService
{
    private readonly ILogger<PurchaseTimeoutService> _logger;
    private readonly PurchaseTimeoutOptions _options;

    private readonly IServiceProvider _serviceProvider;

    public PurchaseTimeoutService(IServiceProvider serviceProvider, IOptions<PurchaseTimeoutOptions> options, ILogger<PurchaseTimeoutService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var purchaseTimeoutHandler = scope.ServiceProvider.GetService<IPurchaseTimeoutHandler>() ?? 
                                        throw new ArgumentException($"Implementation of interface '{nameof(IPurchaseTimeoutHandler)}' can't be found.");

        _logger.LogInformation("Purchase timeout service is starting.");

        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
            await purchaseTimeoutHandler.ExecuteAsync();
            
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Purchase timeout service is stopping.");
                break;
            }
        }
    }
}