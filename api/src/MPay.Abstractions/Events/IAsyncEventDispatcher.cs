namespace MPay.Abstractions.Events;

public interface IAsyncEventDispatcher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
}