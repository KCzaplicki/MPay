using Microsoft.Extensions.DependencyInjection;

namespace MPay.Infrastructure.Validation;

internal static class Extensions
{
    internal static void AddValidationErrorsHandling(this IServiceCollection services)
    {
        services.AddScoped<IValidationErrorMapper, ValidationErrorMapper>();
    }
}