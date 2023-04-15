using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MPay.Abstractions.FeatureFlags;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Webhooks;

internal class WebhookClient : IWebhookClient
{
    private readonly IFeatureFlagsService _featureFlagsService;
    private readonly ILogger<WebhookClient> _logger;
    private readonly HttpClient _httpClient;
    
    public WebhookClient(IHttpClientFactory httpClientFactory, IFeatureFlagsService featureFlagsService, ILogger<WebhookClient> logger)
    {
        _featureFlagsService = featureFlagsService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("WebhookClient");
    }
    
    public async Task<bool> SendAsync<TPayload>(TPayload payload) where TPayload : class, IWebhookPayload
    {
        if (!_featureFlagsService.IsEnabled(FeatureFlag.Webhooks))
        {
            return true;
        }
        
        var payloadJson = new StringContent(JsonSerializer.Serialize(payload), 
            Encoding.UTF8, MediaTypeNames.Application.Json);

        using var response = await _httpClient.PostAsync(string.Empty, payloadJson);

        _logger.LogInformation($"Webhook request sent. Payload: '{payloadJson}'. Response status code: '{response}'.");
        
        return response.IsSuccessStatusCode;
    }
}