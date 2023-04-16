using FluentValidation.Results;
using Humanizer;

namespace MPay.Infrastructure.Validation;

internal class ValidationErrorMapper : IValidationErrorMapper
{
    private static readonly string[] IgnoredPlaceholderValues =
        { "PropertyName", "PropertyValue", "ComparisonProperty", "TotalLength" };

    public ValidationErrorDetails Map(IList<ValidationFailure> errors)
    {
        var validationErrors = errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(MapPropertyName, g => g.Select(CreateValidationError).ToArray());

        return new ValidationErrorDetails
        {
            Errors = validationErrors
        };
    }

    private static ValidationError CreateValidationError(ValidationFailure validationFailure)
    {
        return new(MapErrorCode(validationFailure), MapErrorParameters(validationFailure),
            validationFailure.ErrorMessage);
    }

    private static string MapPropertyName(IGrouping<string, ValidationFailure> grouping)
    {
        return grouping.Key.Camelize();
    }

    private static Dictionary<string, object> MapErrorParameters(ValidationFailure validationFailure)
    {
        return validationFailure.FormattedMessagePlaceholderValues
            .Where(v => !IgnoredPlaceholderValues.Contains(v.Key))
            .ToDictionary(p => p.Key, p => p.Value);
    }

    private static string MapErrorCode(ValidationFailure e)
    {
        return e.ErrorCode.Underscore().Replace("_validator", "_error").ToUpperInvariant();
    }
}