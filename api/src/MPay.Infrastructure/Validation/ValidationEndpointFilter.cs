﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using FluentValidation.Results;

namespace MPay.Infrastructure.Validation;

internal class ValidationEndpointFilter : IEndpointFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidationErrorMapper _mapper;

    public ValidationEndpointFilter(IServiceProvider serviceProvider, IValidationErrorMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var parameter in context.Arguments)
        {
            var validator = GetValidator(parameter);
            if (validator is null)
            {
                continue;
            }

            var result = Validate(validator, parameter);
            if (!result.IsValid)
            {
                var validationErrorDetails = _mapper.Map(result.Errors);

                return TypedResults.Json(validationErrorDetails, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        return await next(context);
    }
    
    private static ValidationResult Validate(object validator, object parameter)
    {
        var validatorGenericType = typeof(IValidator<>).MakeGenericType(parameter.GetType());
        var validateMethodInfo = validatorGenericType.GetMethod("Validate");
        var result = validateMethodInfo.Invoke(validator, new[] { parameter });

        return result as ValidationResult;
    }

    private object? GetValidator(object? parameter)
    {
        var validatorGenericType = typeof(IValidator<>).MakeGenericType(parameter.GetType());
     
        return _serviceProvider.GetService(validatorGenericType);
    }
}