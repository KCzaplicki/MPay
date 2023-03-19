namespace MPay.API.Endpoints;

internal static class HomeEndpoint
{
    internal static void MapHomeEndpoints(this WebApplication app) =>
        app.MapGet("/", () => "MPay API");
}
