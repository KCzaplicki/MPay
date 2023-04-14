namespace MPay.Infrastructure.Webhooks.Payloads;

public enum PurchaseUpdateStatus
{
    Created = 1,
    Completed,
    Cancelled,
    Timeout,
}