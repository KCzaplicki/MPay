using AutoMapper;
using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.Core.Mapping;

internal class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        CreateMap<AddPurchaseDto, Purchase>();
        CreateMap<Purchase, PurchaseDto>();
    }
}