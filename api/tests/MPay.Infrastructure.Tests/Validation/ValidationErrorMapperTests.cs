using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MPay.Infrastructure.Validation;

namespace MPay.Infrastructure.Tests.Validation;

public class ValidationErrorMapperTests
{
    public static IEnumerable<object[]> ValidationFailureTestData()
    {
        yield return new object[]
        {
            "property", "PropertyMockValidator", new Dictionary<string, object>
            {
                { "parameter", "value" }
            },
            "PROPERTY_MOCK_ERROR"
        };
        yield return new object[]
        {
            "property2", "Property2Validator", new Dictionary<string, object>(), "PROPERTY2_ERROR"
        };
    }

    [Fact]
    public void Map_MapManyValidationExceptions()
    {
        // Arrange
        const int errorsCount = 3;
        var validationFailures = new AutoFaker<ValidationFailure>().Generate(errorsCount);
        var validationErrorMapper = new ValidationErrorMapper();

        // Act
        var errorDetails = validationErrorMapper.Map(validationFailures);

        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Errors.Should().NotBeEmpty();
        errorDetails.Errors.Should().HaveCount(errorsCount);
    }

    [Theory]
    [MemberData(nameof(ValidationFailureTestData))]
    public void Map_MapValidationExceptionToValidationErrorDetails(string propertyName, string errorCode,
        Dictionary<string, object> parameters, string expectedErrorCode)
    {
        // Arrange
        var validationFailure = CreateValidationFailure(propertyName, errorCode, parameters);
        var validationErrorMapper = new ValidationErrorMapper();

        // Act
        var errorDetails = validationErrorMapper.Map(new List<ValidationFailure> { validationFailure });

        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        errorDetails.Title.Should().Be("One or more validation errors occurred.");
        errorDetails.Errors.Should().NotBeEmpty();
        errorDetails.Errors.Should().HaveCount(1);
        errorDetails.Errors[propertyName].Should().NotBeEmpty();
        errorDetails.Errors[propertyName].Should().HaveCount(1);
        errorDetails.Errors[propertyName][0].ErrorCode.Should().Be(expectedErrorCode);
        errorDetails.Errors[propertyName][0].Parameters.Should().BeEquivalentTo(parameters);
        errorDetails.Errors[propertyName][0].Message.Should().Be(validationFailure.ErrorMessage);
    }

    [Fact]
    public void Map_MapWithoutIgnoredParameters()
    {
        // Arrange
        var validParameter = AutoFaker.Generate<KeyValuePair<string, string>>();
        var validationFailure = CreateValidationFailure(parameters: new Dictionary<string, object>
        {
            { validParameter.Key, validParameter.Value },
            { "PropertyName", "PropertyName" },
            { "PropertyValue", "PropertyValue" },
            { "ComparisonProperty", "ComparisonProperty" },
            { "TotalLength", "TotalLength" }
        });
        var validationErrorMapper = new ValidationErrorMapper();

        // Act
        var errorDetails = validationErrorMapper.Map(new List<ValidationFailure> { validationFailure });

        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Errors.Should().NotBeEmpty();

        var error = errorDetails.Errors.First().Value[0];
        error.Parameters.Should().HaveCount(1);
        error.Parameters.Should().ContainKey(validParameter.Key);
        error.Parameters[validParameter.Key].Should().Be(validParameter.Value);
    }

    private static ValidationFailure CreateValidationFailure(string? propertyName = null, string? errorCode = null,
        IDictionary<string, object>? parameters = null)
    {
        return new AutoFaker<ValidationFailure>()
            .RuleFor(x => x.PropertyName, x => propertyName ?? x.Random.String())
            .RuleFor(x => x.ErrorCode, x => errorCode ?? x.Random.String())
            .RuleFor(x => x.FormattedMessagePlaceholderValues, () => parameters ?? new Dictionary<string, object>())
            .Generate();
    }
}