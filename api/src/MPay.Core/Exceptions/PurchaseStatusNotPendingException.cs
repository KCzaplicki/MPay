namespace MPay.Core.Exceptions;

public class PurchaseStatusNotPendingException : MPayException
{
    public string Id { get; init; }

    public PurchaseStatusNotPendingException(string id) : base($"Purchase with id '{id}' is not in pending status.")
    {
        Id = id;
    }
}