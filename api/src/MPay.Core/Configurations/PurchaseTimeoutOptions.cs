namespace MPay.Core.Configurations;

public class PurchaseTimeoutOptions
{
    public int IntervalInSeconds { get; init; }

    public int PurchaseCreationTimeoutInMinutes { get; init; }

    public int PurchaseLastPaymentTimeoutInMinutes { get; init; }

    public int PurchaseWithPaymentsTimeoutInMinutes { get; init; }
}