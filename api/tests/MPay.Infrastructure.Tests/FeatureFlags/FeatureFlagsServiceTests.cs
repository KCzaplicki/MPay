using Microsoft.Extensions.Options;
using MPay.Abstractions.FeatureFlags;
using MPay.Infrastructure.FeatureFlags;

namespace MPay.Infrastructure.Tests.FeatureFlags;

public class FeatureFlagsServiceTests
{
    [Fact]
    public void IsEnabled_ReturnsTrue_WhenFeatureFlagIsEnabled()
    {
        // Arrange
        var featureFlagsOptions = new FeatureFlagsOptions
        {
            Webhooks = true
        };
        var featureFlagsService = new FeatureFlagsService(Options.Create(featureFlagsOptions));

        // Act
        var result = featureFlagsService.IsEnabled(FeatureFlag.Webhooks);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsEnabled_ReturnsFalse_WhenFeatureFlagIsDisabled()
    {
        // Arrange
        var featureFlagsOptions = new FeatureFlagsOptions
        {
            Webhooks = false
        };
        var featureFlagsService = new FeatureFlagsService(Options.Create(featureFlagsOptions));

        // Act
        var result = featureFlagsService.IsEnabled(FeatureFlag.Webhooks);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsEnabled_ThrowsException_WhenFeatureFlagIsNotDefined()
    {
        // Arrange
        var featureFlagsOptions = new FeatureFlagsOptions();
        var featureFlagsService = new FeatureFlagsService(Options.Create(featureFlagsOptions));

        // Act
        Action action = () => featureFlagsService.IsEnabled(default);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetConfiguration_ReturnsFeatureFlagsDictionary()
    {
        // Arrange
        var featureFlagsOptions = new FeatureFlagsOptions
        {
            Webhooks = true,
            PurchaseTimeout = false
        };
        var featureFlagsService = new FeatureFlagsService(Options.Create(featureFlagsOptions));
        
        // Act
        var result = featureFlagsService.GetConfiguration();
        
        // Assert
        result.Should().BeEquivalentTo(new Dictionary<FeatureFlag, bool>
        {
            {FeatureFlag.Webhooks, true},
            {FeatureFlag.PurchaseTimeout, false}
        });
    }
}