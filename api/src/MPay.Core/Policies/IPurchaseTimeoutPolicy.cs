namespace MPay.Core.Policies;

public interface IPurchaseTimeoutPolicy
{
    bool CanApply(Purchase purchase);
}