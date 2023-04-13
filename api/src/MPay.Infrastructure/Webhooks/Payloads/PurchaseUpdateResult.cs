namespace MPay.Infrastructure.Webhooks.Payloads;

public record PurchaseUpdateResult(string Id, PurchaseUpdateStatus Status, DateTime UpdatedAt) : IWebhookPayload;