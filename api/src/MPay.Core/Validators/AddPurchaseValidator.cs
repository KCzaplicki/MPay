using FluentValidation;

namespace MPay.Core.Validators;

internal class AddPurchaseValidator : AbstractValidator<AddPurchaseDto>
{
    public AddPurchaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(512).WithMessage("Description must be less than 512 characters");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .PrecisionScale(10, 2, true).WithMessage("Price must be less than 10 digits and have 2 decimal places");
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");
    }
}