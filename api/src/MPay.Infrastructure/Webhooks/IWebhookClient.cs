using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Webhooks;

public interface IWebhookClient
{
    Task<bool> SendAsync<TPayload>(TPayload payload) where TPayload : class, IWebhookPayload;
}