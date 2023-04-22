using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MPay.Core.Configurations;
using MPay.Infrastructure.Services;
using Serilog;

[assembly: InternalsVisibleTo("MPay.Api")]
[assembly: InternalsVisibleTo("MPay.Core.Tests")]
[assembly: InternalsVisibleTo("MPay.Infrastructure.Tests")]
[assembly: InternalsVisibleTo("MPay.Tests.Shared")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace MPay.Infrastructure;

internal static class Extensions
{
    internal static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<PurchaseTimeoutService>();
    }

    internal static void AddLogger(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        hostBuilder.UseSerilog();
    }

    internal static void UseLogger(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }

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

    internal static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    internal static void ConfigureJson(this IServiceCollection services)
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
        services.Configure<PurchaseTimeoutOptions>(
            configuration.GetSection(GetOptionsSectionName<PurchaseTimeoutOptions>()));
    }

    internal static string GetOptionsSectionName<T>()
    {
        return typeof(T).Name.Replace("Options", string.Empty);
    }
}