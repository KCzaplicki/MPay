namespace MPay.API.Endpoints;

internal static class HomeEndpoints
{
    internal static void MapHomeEndpoints(this WebApplication app) =>
        app.MapGet("/", () => "MPay API");
}
