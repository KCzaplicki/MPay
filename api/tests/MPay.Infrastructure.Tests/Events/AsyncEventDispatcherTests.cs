using MPay.Abstractions.Events;
using MPay.Infrastructure.Events;

namespace MPay.Infrastructure.Tests.Events;

public class AsyncEventDispatcherTests
{
    [Fact]
    public async Task PublishAsync_WritesEventToEventChannel()
    {
        // Arrange
        var @event = new Mock<IEvent>();
        var channel = new Mock<IEventChannel>();
        channel.Setup(x => x.Writer.WriteAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()));
        var eventDispatcher = new AsyncEventDispatcher(channel.Object);
        
        // Act
        await eventDispatcher.PublishAsync(@event.Object);

        // Assert
        channel.Verify(x => x.Writer.WriteAsync(@event.Object, It.IsAny<CancellationToken>()), Times.Once);
    }
}