using MPay.Core.Entities;

namespace MPay.Core.Tests.Fakes;

public class CompletedPurchaseFake : AutoFaker<Purchase>
{
    public CompletedPurchaseFake()
    {
        RuleFor(x => x.Status, PurchaseStatus.Completed);
    }
}