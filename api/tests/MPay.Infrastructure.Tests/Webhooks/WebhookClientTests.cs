using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MPay.Abstractions.FeatureFlags;
using MPay.Infrastructure.Webhooks;
using MPay.Infrastructure.Webhooks.Payloads;

namespace MPay.Infrastructure.Tests.Webhooks;

public class WebhookClientTests
{
    private const string MockWebhookUrl = "http://mock/hook";
    private const string SendAsyncMethodName = "SendAsync";
    
    private readonly ILogger<WebhookClient> _logger;

    public WebhookClientTests()
    {
        _logger = new Mock<ILogger<WebhookClient>>().Object;
    }
    
    [Fact]
    public async Task SendAsync_SendPostRequest()
    {
        // Arrange
        var payload = new Mock<IWebhookPayload>();
        var featureFlagsService = CreateMockFeatureFlagsService();
        var httpClient = CreateMockHttpClient(out var httpMessageHandler);
        var webhookClient = new WebhookClient(httpClient, featureFlagsService.Object, _logger);
        
        // Act
        await webhookClient.SendAsync(payload.Object);
        
        // Assert
        httpMessageHandler.Protected().Verify(
            SendAsyncMethodName,
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_ReturnsTrue_WhenWebhookResponseIsSuccessful()
    {
        // Arrange
        var payload = new Mock<IWebhookPayload>();
        var featureFlagsService = CreateMockFeatureFlagsService();
        var httpClient = CreateMockHttpClient(out _);
        var webhookClient = new WebhookClient(httpClient, featureFlagsService.Object, _logger);
        
        // Act
        var result = await webhookClient.SendAsync(payload.Object);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ReturnsFalse_WhenWebhookResponseIsFailed()
    {
        // Arrange
        var payload = new Mock<IWebhookPayload>();
        var featureFlagsService = CreateMockFeatureFlagsService();
        var httpClient = CreateMockHttpClient(out _, HttpStatusCode.TooManyRequests);
        var webhookClient = new WebhookClient(httpClient, featureFlagsService.Object, _logger);
        
        // Act
        var result = await webhookClient.SendAsync(payload.Object);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task SendAsync_DoesNotSendRequest_WhenFeatureFlagDisabled()
    {
        // Arrange
        var payload = new Mock<IWebhookPayload>();
        var featureFlagsService = CreateMockFeatureFlagsService(false);
        var httpClient = CreateMockHttpClient(out var httpMessageHandler);
        var webhookClient = new WebhookClient(httpClient, featureFlagsService.Object, _logger);
        
        // Act
        await webhookClient.SendAsync(payload.Object);
        
        // Assert
        httpMessageHandler.Protected().Verify(
            SendAsyncMethodName,
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
    
    private static HttpClient CreateMockHttpClient(out Mock<HttpMessageHandler> httpMessageHandler, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(SendAsyncMethodName, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode
            });
        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClient.BaseAddress = new Uri(MockWebhookUrl);
        return httpClient;
    }

    private static Mock<IFeatureFlagsService> CreateMockFeatureFlagsService(bool webhooksEnabled = true)
    {
        var featureFlagsService = new Mock<IFeatureFlagsService>();
        featureFlagsService.Setup(x => x.IsEnabled(FeatureFlag.Webhooks))
            .Returns(webhooksEnabled);
        return featureFlagsService;
    }
}