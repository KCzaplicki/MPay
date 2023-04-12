using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Events;

internal class AsyncEventDispatcher : IAsyncEventDispatcher
{
    private readonly IEventChannel _eventChannel;

    public AsyncEventDispatcher(IEventChannel eventChannel)
    {
        _eventChannel = eventChannel;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent 
        => await _eventChannel.Writer.WriteAsync(@event);
}