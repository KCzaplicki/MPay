using FluentValidation.Results;

namespace MPay.Infrastructure.Validation;

public interface IValidationErrorMapper
{
    ValidationErrorDetails Map(IList<ValidationFailure> errors);
}