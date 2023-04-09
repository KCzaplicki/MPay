using MPay.Core.DTO;

namespace MPay.Core.Tests.Fakes;

internal sealed class PurchasePaymentDtoFake : AutoFaker<PurchasePaymentDto>
{
    public PurchasePaymentDtoFake()
    {
        RuleFor(x => x.CardHolderName, f => f.Person.FullName);
        RuleFor(x => x.CardNumber, f => f.Random.Long(1000000000000000, 9999999999999999));
        RuleFor(x => x.Ccv, f => f.Random.Int(100, 999));
        RuleFor(x => x.CardExpiry, f => f.Date.Future());
    }
}