using FluentValidation;
using MPay.Abstractions.Common;
using MPay.Core.DTO;

namespace MPay.Core.Validators;

internal class PurchasePaymentValidator : AbstractValidator<PurchasePaymentDto>
{
    public PurchasePaymentValidator(IClock clock)
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required")
            .InclusiveBetween(1000000000000000, 9999999999999999).WithMessage("Card number must be 16 digits long");
        RuleFor(x => x.CardExpiry)
            .NotEmpty().WithMessage("Card expiry is required")
            .GreaterThan(clock.Today.AddDays(-1)).WithMessage("Card expiry must be in the future");
        RuleFor(x => x.Ccv)
            .NotEmpty().WithMessage("CCV is required")
            .InclusiveBetween(100, 999).WithMessage("CCV must be 3 digits long");
        RuleFor(x => x.CardHolderName)
            .NotEmpty().WithMessage("Card holder name is required")
            .MaximumLength(200).WithMessage("Card holder name must be less than 200 characters long");
    }
}