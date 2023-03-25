namespace MPay.Core.Services;

public interface IPurchaseService
{
    Task<string> AddAsync(AddPurchaseDto addPurchaseDto);
    Task<PurchaseDto> GetPendingAsync(string id);
}