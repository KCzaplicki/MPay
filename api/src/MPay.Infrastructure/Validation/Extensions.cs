using Microsoft.Extensions.DependencyInjection;

namespace MPay.Infrastructure.Validation;

internal static class Extensions
{
    internal static void AddValidation(this IServiceCollection services) 
        => services.AddScoped<IValidationErrorMapper, ValidationErrorMapper>();
}