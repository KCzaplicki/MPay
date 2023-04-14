namespace MPay.Infrastructure.Webhooks.Payloads;

public record PurchaseUpdateResultDto(string Id, PurchaseUpdateStatus Status, DateTime UpdatedAt) : IWebhookPayload;