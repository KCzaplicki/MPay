using System.Text.Json.Serialization;

namespace MPay.Core.DTO;

public record PurchasePaymentResultDto(string PurchaseId, string PurchasePaymentId, PurchasePaymentStatus PurchasePaymentStatus)
{
    [JsonIgnore]
    public bool IsCompleted => PurchasePaymentStatus == PurchasePaymentStatus.Completed;
}