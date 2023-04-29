using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using MPay.Infrastructure.Validation;
using MPay.Tests.Shared.Common;
using ContextFactory = MPay.Tests.Shared.Common.MockEndpointFilterInvocationContextFactory;
using NextFactory = MPay.Tests.Shared.Common.MockEndpointFilterDelegateFactory;

namespace MPay.Infrastructure.Tests.Validation;

public class ValidationEndpointFilterTests
{
    [Fact]
    public async Task InvokeAsync_ValidateArguments()
    {
        // Arrange
        var mockDto = AutoFaker.Generate<MockDto>();
        var context = ContextFactory.Create(mockDto);
        var mockNext = NextFactory.Create();
        var validator = CreateMockValidator();
        var mockServiceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(validator.Object);
        });
        var validationErrorMapper = new Mock<IValidationErrorMapper>();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object);

        // Assert
        validator.Verify(x => x.Validate(mockDto), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsValidationErrorDetails_WhenValidationFailed()
    {
        // Arrange
        var mockDto = AutoFaker.Generate<MockDto>();
        var context = ContextFactory.Create(mockDto);
        var mockNext = NextFactory.Create();
        var validator = CreateMockValidator(false);
        var mockServiceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(validator.Object);
        });
        var validationErrorMapper = CreateMockValidationErrorMapper();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        var result =
            await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object) as
                JsonHttpResult<ValidationErrorDetails>;

        // Assert
        validator.Verify(x => x.Validate(mockDto), Times.Once);
        validationErrorMapper.Verify(x => x.Map(It.IsAny<IList<ValidationFailure>>()), Times.Once);
        result.Should().NotBeNull();
        result?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Should().BeOfType<JsonHttpResult<ValidationErrorDetails>>();
    }

    [Fact]
    public async Task InvokeAsync_NotProcessNext_WhenValidationFailed()
    {
        // Arrange
        var mockDto = AutoFaker.Generate<MockDto>();
        var context = ContextFactory.Create(mockDto);
        var mockNext = NextFactory.Create();
        var validator = CreateMockValidator(false);
        var mockServiceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(validator.Object);
        });
        var validationErrorMapper = CreateMockValidationErrorMapper();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object);

        // Assert
        mockNext.Verify(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ProcessNext_WhenValidationPassed()
    {
        // Arrange
        var mockDto = AutoFaker.Generate<MockDto>();
        var context = ContextFactory.Create(mockDto);
        var mockNext = NextFactory.Create();
        var validator = CreateMockValidator();
        var mockServiceProvider = MockServiceProviderFactory.Create(services =>
        {
            services.AddSingleton(validator.Object);
        });
        var validationErrorMapper = new Mock<IValidationErrorMapper>();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object);

        // Assert
        validator.Verify(x => x.Validate(mockDto), Times.Once);
        mockNext.Verify(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_SkipValidation_WhenEmptyArguments()
    {
        // Arrange
        var context = ContextFactory.Create();
        var mockNext = NextFactory.Create();
        var mockServiceProvider = MockServiceProviderFactory.Create();
        var validationErrorMapper = CreateMockValidationErrorMapper();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object);

        // Assert
        mockNext.Verify(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_SkipValidation_WhenValidatorNotRegistered()
    {
        // Arrange
        var mockDto = AutoFaker.Generate<MockDto>();
        var context = ContextFactory.Create(mockDto);
        var mockNext = NextFactory.Create();
        var mockServiceProvider = MockServiceProviderFactory.Create();
        var validationErrorMapper = CreateMockValidationErrorMapper();
        var validationEndpointFilter = new ValidationEndpointFilter(mockServiceProvider, validationErrorMapper.Object);

        // Act
        await validationEndpointFilter.InvokeAsync(context.Object, mockNext.Object);

        // Assert
        mockNext.Verify(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
    }

    private static Mock<IValidationErrorMapper> CreateMockValidationErrorMapper()
    {
        var validationErrorMapper = new Mock<IValidationErrorMapper>();
        validationErrorMapper.Setup(x => x.Map(It.IsAny<IList<ValidationFailure>>()))
            .Returns(new ValidationErrorDetails());
        return validationErrorMapper;
    }

    private static Mock<IValidator<MockDto>> CreateMockValidator(bool isValid = true)
    {
        var validator = new Mock<IValidator<MockDto>>();
        validator.Setup(x => x.Validate(It.IsAny<MockDto>()))
            .Returns(() =>
                new ValidationResult(isValid
                    ? Array.Empty<ValidationFailure>()
                    : new[] { AutoFaker.Generate<ValidationFailure>() }));
        return validator;
    }
}