using System.Net;

namespace MPay.API.IntegrationTests.Endpoints;

public class APIEndpointsTests : BaseEndpointsTest
{
    public APIEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task HomeEndpoint_ReturnsApiName()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.GetResultAsync<string>();
        content.Should().Be("MPay API");
    }

    [Fact]
    public async Task ConfigurationEndpoint_ReturnsFeaturesConfiguration()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/configuration");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var configuration = await response.GetResultAsync<Dictionary<string, object>>();
        configuration.Should().NotBeNull();
        configuration.Should().HaveCount(2);
        configuration.Should().ContainKey("webhooks");
        configuration.Should().ContainKey("purchaseTimeout");
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthcheckStatus()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/healthz");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.GetResultAsync<string>();
        content.Should().Be("Healthy");
    }
}