using System.Net;

namespace MPay.API.Endpoints;

internal static class HomeEndpoints
{
    private const string HomeTag = "Home";

    internal static void MapHomeEndpoints(this WebApplication app) =>
        app.MapGet("/", () => "MPay API")
            .WithTags(HomeTag)
            .WithDescription("API footprint")
            .Produces<string>()
            .WithOpenApi();
}
