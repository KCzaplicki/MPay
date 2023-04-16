using MPay.Core.Entities;

namespace MPay.Core.Policies.PurchaseTimeout;

public interface IPurchaseTimeoutPolicy
{
    bool CanApply(Purchase purchase);
}