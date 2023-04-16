using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.Core.Factories;

internal interface IPurchaseFactory
{
    Purchase Create(AddPurchaseDto addPurchaseDto);
}