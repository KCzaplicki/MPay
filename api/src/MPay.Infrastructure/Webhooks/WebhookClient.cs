using System.Net.Mime;
using System.Text;
using System.Text.Json;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Webhooks;

internal class WebhookClient : IWebhookClient
{
    private readonly HttpClient _httpClient;

    public WebhookClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("WebhookClient");
    }
    
    public async Task<bool> SendAsync<TPayload>(TPayload payload) where TPayload : class, IWebhookPayload
    {
        var payloadJson = new StringContent(JsonSerializer.Serialize(payload), 
            Encoding.UTF8, MediaTypeNames.Application.Json);

        using var response = await _httpClient.PostAsync(string.Empty, payloadJson);

        return response.IsSuccessStatusCode;
    }
}