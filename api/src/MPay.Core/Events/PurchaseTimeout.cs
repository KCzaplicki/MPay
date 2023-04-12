using MPay.Abstractions.Events;

namespace MPay.Core.Events;

public record PurchaseTimeout(string Id, DateTime TimeoutAt) : IEvent;