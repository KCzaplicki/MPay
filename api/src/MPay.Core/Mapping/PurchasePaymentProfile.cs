using AutoMapper;
using MPay.Core.DTO;
using MPay.Core.Entities;

namespace MPay.Core.Mapping;

internal class PurchasePaymentProfile : Profile
{
    public PurchasePaymentProfile()
    {
        CreateMap<PurchasePaymentDto, PurchasePayment>();
    }
}