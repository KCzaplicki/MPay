using MPay.Abstractions.Exceptions;

namespace MPay.Core.Exceptions;

public class PurchaseStatusNotPendingException : MPayException
{
    public PurchaseStatusNotPendingException(string id) : base($"Purchase with id '{id}' is not in pending status.")
    {
        Id = id;
    }

    public string Id { get; }
}