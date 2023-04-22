using AutoBogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MPay.Abstractions.Events;
using MPay.Infrastructure.Events;
using MPay.Tests.Shared.Common;

namespace MPay.Infrastructure.Tests.Events;

public class BackgroundDispatcherServiceTests
{
    private readonly ILogger<BackgroundDispatcherService> _logger;

    public BackgroundDispatcherServiceTests()
    {
        _logger = new Mock<ILogger<BackgroundDispatcherService>>().Object;
    }
    
    [Fact]
    public async Task ExecuteAsync_ReadsEventFromEventChannel()
    {
        // Arrange
        var @event = new Mock<IEvent>();
        var serviceProvider = MockServiceProviderFactory.Create();
        var channel = CreateMockEventChannel(@event.Object);
        var backgroundDispatcherService = new BackgroundDispatcherService(serviceProvider, channel.Object, _logger);
        
        // Act
        await backgroundDispatcherService.StartAsync(CancellationToken.None);

        // Assert
        channel.Verify(x => x.Reader.ReadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesEvent_WhenEventHandlerRegistered()
    {
        // Arrange
        var @event = AutoFaker.Generate<MockEvent>();
        var eventHandler = new Mock<IEventHandler<MockEvent>>();
        var serviceProvider = MockServiceProviderFactory.Create(services => services.AddSingleton(eventHandler.Object));
        var channel = CreateMockEventChannel(@event);
        var backgroundDispatcherService = new BackgroundDispatcherService(serviceProvider, channel.Object, _logger);
        
        // Act
        await backgroundDispatcherService.StartAsync(CancellationToken.None);
        
        // Assert
        eventHandler.Verify(x => x.HandleAsync(@event), Times.Once);
    }
    
    [Fact]
    public async Task ExecuteAsync_DoesNotHandleEvent_WhenEventHandlerNotRegistered()
    {
        // Arrange
        var @event = AutoFaker.Generate<MockEvent>();
        var eventHandler = new Mock<IEventHandler<MockEvent>>();
        var serviceProvider = MockServiceProviderFactory.Create();
        var channel = CreateMockEventChannel(@event);
        var backgroundDispatcherService = new BackgroundDispatcherService(serviceProvider, channel.Object, _logger);
        
        // Act
        await backgroundDispatcherService.StartAsync(CancellationToken.None);
        
        // Assert
        eventHandler.Verify(x => x.HandleAsync(@event), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ProcessOtherEvents_WhenHandlerThrowsException()
    {
        // Arrange
        var throwingEvent = AutoFaker.Generate<MockEvent>();
        var properEvent = AutoFaker.Generate<MockEvent>();
        var eventHandler = new Mock<IEventHandler<MockEvent>>();
        eventHandler.Setup(x => x.HandleAsync(throwingEvent)).Throws<Exception>();
        var serviceProvider = MockServiceProviderFactory.Create(services => services.AddSingleton(eventHandler.Object));
        var channel = CreateMockEventChannel(throwingEvent, properEvent);
        var backgroundDispatcherService = new BackgroundDispatcherService(serviceProvider, channel.Object, _logger);
        
        // Act
        await backgroundDispatcherService.StartAsync(CancellationToken.None);
        
        // Assert
        eventHandler.Verify(x => x.HandleAsync(throwingEvent), Times.Once);
        eventHandler.Verify(x => x.HandleAsync(properEvent), Times.Once);
    }

    private static Mock<IEventChannel> CreateMockEventChannel(params IEvent[] events)
    {
        var channel = new Mock<IEventChannel>();
        channel.Setup(x => x.Reader.ReadAllAsync(It.IsAny<CancellationToken>()))
            .Returns(events.ToAsyncEnumerable);
        return channel;
    }
}