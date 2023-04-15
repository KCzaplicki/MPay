namespace MPay.Infrastructure.FeatureFlags;

public class FeatureFlagsOptions
{
    public bool Webhooks { get; init; }
    public bool PurchaseTimeout { get; init; }
}