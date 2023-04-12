using MPay.Abstractions.Events;

namespace MPay.Core.Events;

public record PurchaseCancelled(string Id, DateTime CancelledAt) : IEvent;