namespace MPay.Core.DTO;

public record PurchasePaymentDto(string CardHolderName, long CardNumber, int Ccv, DateTime CardExpiry);