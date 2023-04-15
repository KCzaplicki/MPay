using Microsoft.Extensions.Options;
using MPay.Abstractions.FeatureFlags;

namespace MPay.Infrastructure.FeatureFlags;

public class FeatureFlagsService : IFeatureFlagsService
{
    private readonly IOptions<FeatureFlagsOptions> _options;

    public FeatureFlagsService(IOptions<FeatureFlagsOptions> options)
    {
        _options = options;
    }
    
    public bool IsEnabled(FeatureFlag featureFlag)
    {
        var featureFlagName = featureFlag.ToString();
        
        var featureFlagProperty = typeof(FeatureFlagsOptions).GetProperty(featureFlagName);
        if (featureFlagProperty is null)
        {
            throw new ArgumentException($"Feature flag '{featureFlagName}' is not defined.");
        }
        
        return featureFlagProperty.GetValue(_options.Value) is true;
    }

    public IDictionary<FeatureFlag, bool> GetConfiguration()
    {
        var featureFlagValues = Enum.GetValues<FeatureFlag>();

        return featureFlagValues.ToDictionary(featureFlagValue => featureFlagValue, IsEnabled);
    }
}