using MPay.Core.DTO;

namespace MPay.Core.Tests.Fakes;

internal sealed class AddPurchaseDtoFake : AutoFaker<AddPurchaseDto>
{
    public AddPurchaseDtoFake()
    {
        RuleFor(x => x.Price, f => f.Random.Decimal(1, 1000));
        RuleFor(x => x.Currency, f => f.Random.String2(3).ToUpperInvariant());
    }
}