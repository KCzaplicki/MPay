using AutoMapper;

namespace MPay.Core.Mapping;

internal class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        CreateMap<AddPurchaseDto, Purchase>();
        CreateMap<Purchase, PurchaseDto>();
    }
}