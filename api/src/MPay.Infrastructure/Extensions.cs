using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.Configurations;

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

    public static void ConfigureJson(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PurchaseTimeoutOptions>(configuration.GetSection(GetOptionsSectionName<PurchaseTimeoutOptions>()));
    }

    private static string GetOptionsSectionName<T>()
        => typeof(T).Name.Replace("Options", string.Empty);
}