namespace MPay.Core.Entities;

public class Purchase : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime? CompletedAt { get; set; }
    public PurchaseStatus Status { get; set; }

    public IList<PurchasePayment> Payments { get; set; }
}