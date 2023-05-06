using System.Net;
using System.Text.Json;

namespace MPay.API.IntegrationTests.Endpoints;

public class APIEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public APIEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomeEndpoint_ReturnsApiName()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("MPay API");
    }
    
    [Fact]
    public async Task ConfigurationEndpoint_ReturnsFeaturesConfiguration()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/configuration");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var configuration = JsonSerializer.Deserialize<Dictionary<string, bool>>(content);
        configuration.Should().NotBeNull();
        configuration.Should().HaveCount(2);
        configuration.Should().ContainKey("webhooks");
        configuration.Should().ContainKey("purchaseTimeout");
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthcheckStatus()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/healthz");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}