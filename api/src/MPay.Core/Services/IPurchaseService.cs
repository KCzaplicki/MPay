using MPay.Core.DTO;

namespace MPay.Core.Services;

public interface IPurchaseService
{
    Task<string> AddAsync(AddPurchaseDto addPurchaseDto);
    Task<PurchaseDto> GetPendingAsync(string id);
    Task CancelPurchaseAsync(string id);
}