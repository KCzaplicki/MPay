using FluentValidation;

namespace MPay.Core.Validators;

internal class AddPurchaseValidator : AbstractValidator<AddPurchaseDto>
{
    public AddPurchaseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}