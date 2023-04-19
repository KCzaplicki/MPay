using Humanizer;
using Microsoft.AspNetCore.Http;
using MPay.Core.DTO;

namespace MPay.Infrastructure.ErrorHandling;

public static class ErrorDetailsExtensions
{
    private const string PurchasePaymentFailedErrorCode = "PURCHASE_PAYMENT_FAILED";
    private const string PurchasePaymentFailedTitlePattern = "Payment with id '{0}' failed with status '{1}'";

    public static ErrorDetails MapFrom<T>(T from)
    {
        return from switch
        {
            PurchasePaymentResultDto purchasePaymentResult => MapFromPurchasePaymentResult(purchasePaymentResult),
            _ => throw new NotSupportedException($"Mapping from '{typeof(T).Name}' to ErrorDetails is not supported.")
        };
    }

    private static ErrorDetails MapFromPurchasePaymentResult(PurchasePaymentResultDto purchasePaymentResultDto)
    {
        return new ErrorDetails
        {
            Title = string.Format(PurchasePaymentFailedTitlePattern, purchasePaymentResultDto.PurchaseId,
                purchasePaymentResultDto.PurchasePaymentStatus),
            ErrorCode = PurchasePaymentFailedErrorCode,
            Status = StatusCodes.Status400BadRequest,
            Data = new Dictionary<string, object>
            {
                { nameof(purchasePaymentResultDto.PurchaseId).Underscore(), purchasePaymentResultDto.PurchaseId },
                {
                    nameof(purchasePaymentResultDto.PurchasePaymentId).Underscore(),
                    purchasePaymentResultDto.PurchasePaymentId
                },
                {
                    nameof(purchasePaymentResultDto.PurchasePaymentStatus).Underscore(),
                    purchasePaymentResultDto.PurchasePaymentStatus
                }
            }
        };
    }
}