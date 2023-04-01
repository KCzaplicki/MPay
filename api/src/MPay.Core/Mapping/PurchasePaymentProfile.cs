using AutoMapper;

namespace MPay.Core.Mapping;

internal class PurchasePaymentProfile : Profile
{
    public PurchasePaymentProfile()
    {
        CreateMap<PurchasePaymentDto, PurchasePayment>();
    }
}