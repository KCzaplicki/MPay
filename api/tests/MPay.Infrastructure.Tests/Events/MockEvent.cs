using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Tests.Events;

public record MockEvent : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}