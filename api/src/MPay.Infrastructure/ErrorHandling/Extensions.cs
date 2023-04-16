using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MPay.Infrastructure.ErrorHandling;

internal static class Extensions
{
    internal static void AddErrorHandling(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddScoped<IExceptionMapper, ExceptionMapper>();
    }

    internal static void UseErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}