namespace MPay.Core.Exceptions;

public class PurchaseNotFoundException : MPayException
{
    public string Id { get; set; }

    public PurchaseNotFoundException(string id) : base($"Purchase with id '{id}' not found.")
    {
        Id = id;
    }
}