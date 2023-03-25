namespace MPay.Core.Entities;

public class PurchasePayment : BaseEntity
{
    public string PurchaseId { get; set; }
    public int Ccv { get; set; }
    public long CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public DateTime CardExpiry { get; set; }
    public PurchasePaymentStatus Status { get; set; }
}