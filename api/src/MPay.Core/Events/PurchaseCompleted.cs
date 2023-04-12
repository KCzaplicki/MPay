using MPay.Abstractions.Events;

namespace MPay.Core.Events;

public record PurchaseCompleted(string Id, DateTime CompletedAt) : IEvent;