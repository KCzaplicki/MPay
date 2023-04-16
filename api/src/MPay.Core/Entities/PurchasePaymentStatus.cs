namespace MPay.Core.Entities;

public enum PurchasePaymentStatus
{
    Completed = 1,
    InvalidCard,
    NoFounds,
    Timeout
}