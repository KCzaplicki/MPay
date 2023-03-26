namespace MPay.Core.Exceptions;

public class PurchaseNotFoundException : MPayException
{
    public string Id { get; init; }

    public PurchaseNotFoundException(string id) : base($"Purchase with id '{id}' not found.")
    {
        Id = id;
    }
}