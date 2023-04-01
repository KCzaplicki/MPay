namespace MPay.Core.Factories;

internal interface IPurchaseFactory
{
    Purchase Create(AddPurchaseDto addPurchaseDto);
}