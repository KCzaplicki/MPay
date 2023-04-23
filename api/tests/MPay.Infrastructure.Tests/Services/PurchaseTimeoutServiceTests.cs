using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MPay.Abstractions.FeatureFlags;
using MPay.Core.Configurations;
using MPay.Core.Services;
using MPay.Infrastructure.Services;
using MPay.Tests.Shared.Common;

namespace MPay.Infrastructure.Tests.Services;

public class PurchaseTimeoutServiceTests
{
    private const int WorkerTimeoutInSeconds = 5;
    
    private readonly ILogger<PurchaseTimeoutService> _logger;
    
    public PurchaseTimeoutServiceTests()
    {
        _logger = new Mock<ILogger<PurchaseTimeoutService>>().Object;
    }
    
    [Fact]
    public async Task ExecuteAsync_CallPurchaseTimeoutHandler()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(WorkerTimeoutInSeconds));
        var purchaseTimeoutHandler = new Mock<IPurchaseTimeoutHandler>();
        purchaseTimeoutHandler.Setup(x => x.ExecuteAsync())
            .Callback(() => cancellationTokenSource.Cancel());
        var featureFlagsService = CreateMockFeatureFlagsService();
        var serviceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(featureFlagsService.Object);
            services.AddSingleton(purchaseTimeoutHandler.Object);
        });
        var purchaseTimeoutOptions = CreatePurchaseTimeoutOptions();
        var purchaseTimeoutService = new PurchaseTimeoutService(serviceProvider, purchaseTimeoutOptions, _logger);
        
        // Act
        await purchaseTimeoutService.StartAsync(cancellationTokenSource.Token);
        
        // Assert
        purchaseTimeoutHandler.Verify(x => x.ExecuteAsync(), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task ExecuteAsync_DoesNotCallPurchaseTimeoutHandler_WhenFeatureFlagDisabled()
    {
        // Arrange
        var purchaseTimeoutHandler = new Mock<IPurchaseTimeoutHandler>();
        var featureFlagsService = CreateMockFeatureFlagsService(false);
        var serviceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(featureFlagsService.Object);
        });
        var purchaseTimeoutOptions = CreatePurchaseTimeoutOptions();
        var purchaseTimeoutService = new PurchaseTimeoutService(serviceProvider, purchaseTimeoutOptions, _logger);
        
        // Act
        await purchaseTimeoutService.StartAsync(CancellationToken.None);
        
        // Assert
        purchaseTimeoutHandler.Verify(x => x.ExecuteAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentException_WhenFeatureFlagsServiceNotRegistered()
    {
        // Arrange
        var serviceProvider = MockServiceProviderFactory.Create();
        var purchaseTimeoutOptions = CreatePurchaseTimeoutOptions();
        var purchaseTimeoutService = new PurchaseTimeoutService(serviceProvider, purchaseTimeoutOptions, _logger);
        
        // Act
        Func<Task> startAsyncTask = async () => await purchaseTimeoutService.StartAsync(CancellationToken.None);
        
        // Assert
        await startAsyncTask.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentException_WhenPurchaseTimeoutHandlerNotRegistered()
    {
        // Arrange
        var featureFlagsService = CreateMockFeatureFlagsService();
        var serviceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(featureFlagsService.Object);
        });
        var purchaseTimeoutOptions = CreatePurchaseTimeoutOptions();
        var purchaseTimeoutService = new PurchaseTimeoutService(serviceProvider, purchaseTimeoutOptions, _logger);
        
        // Act
        Func<Task> startAsyncTask = async () => await purchaseTimeoutService.StartAsync(CancellationToken.None);
        
        // Assert
        await startAsyncTask.Should().ThrowAsync<ArgumentException>();
    }
    
    private static IOptions<PurchaseTimeoutOptions> CreatePurchaseTimeoutOptions(int intervalInSeconds = 0)
    {
        var purchaseTimeoutOptions = Options.Create(new PurchaseTimeoutOptions
        {
            IntervalInSeconds = intervalInSeconds
        });
        return purchaseTimeoutOptions;
    }

    private static Mock<IFeatureFlagsService> CreateMockFeatureFlagsService(bool purchaseTimeoutEnabled = true)
    {
        var featureFlagsService = new Mock<IFeatureFlagsService>();
        featureFlagsService.Setup(x => x.IsEnabled(FeatureFlag.PurchaseTimeout))
            .Returns(purchaseTimeoutEnabled);
        return featureFlagsService;
    }
}