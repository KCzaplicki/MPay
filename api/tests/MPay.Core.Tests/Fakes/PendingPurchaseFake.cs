using MPay.Core.Entities;

namespace MPay.Core.Tests.Fakes;

internal sealed class PendingPurchaseFake : AutoFaker<Purchase>
{
    public PendingPurchaseFake()
    {
        RuleFor(x => x.Status, PurchaseStatus.Pending);
    }
}