namespace MPay.Infrastructure.Webhooks;

public class WebhooksOptions
{
    public string Url { get; init; }
    public int RetryLimit { get; init; }
    public int[] RetryIntervalsInSeconds { get; init; }
}