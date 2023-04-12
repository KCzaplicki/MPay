using MPay.Abstractions.Events;

namespace MPay.Core.Events;

public record PurchaseCreated(string Id, DateTime CreatedAt) : IEvent;