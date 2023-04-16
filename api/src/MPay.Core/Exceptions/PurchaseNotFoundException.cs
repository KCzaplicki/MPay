using MPay.Abstractions.Exceptions;

namespace MPay.Core.Exceptions;

public class PurchaseNotFoundException : MPayException
{
    public PurchaseNotFoundException(string id) : base($"Purchase with id '{id}' not found.")
    {
        Id = id;
    }

    public string Id { get; }
}