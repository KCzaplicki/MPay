namespace MPay.Abstractions.FeatureFlags;

public interface IFeatureFlagsService
{
    bool IsEnabled(FeatureFlag featureFlag);

    IDictionary<FeatureFlag, bool> GetConfiguration();
}