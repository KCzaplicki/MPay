using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Infrastructure;

internal static class Extensions
{
    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    internal static void UseSwagger(this WebApplication app)
    {
        SwaggerBuilderExtensions.UseSwagger(app);
        app.UseSwaggerUI();
    }

    internal static void AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    internal static void UseHealthCheck(this WebApplication app)
    {
        app.UseHealthChecks("/healthz");
    }
    
    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}