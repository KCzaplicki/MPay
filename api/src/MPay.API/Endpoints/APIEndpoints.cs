using MPay.Abstractions.FeatureFlags;

namespace MPay.API.Endpoints;

internal static class HomeEndpoints
{
    private const string APITag = "API";

    internal static void MapAPIEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "MPay API")
            .WithTags(APITag)
            .WithDescription("API footprint")
            .Produces<string>()
            .WithOpenApi();

        app.MapGet("/configuration",
                (IFeatureFlagsService featureFlagsService) => TypedResults.Ok(featureFlagsService.GetConfiguration()))
            .WithTags(APITag)
            .WithDescription("API configuration")
            .Produces<IDictionary<FeatureFlag, bool>>()
            .WithOpenApi();
    }
}