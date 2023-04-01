namespace MPay.Core.DTO;

public record PurchaseDto(string Id, string Name, string Description, DateTime CreatedAt, decimal Price, string Currency);