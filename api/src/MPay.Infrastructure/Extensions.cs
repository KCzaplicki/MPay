using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Abstractions.Common;
using MPay.Core.Configurations;
using MPay.Core.Repository;
using MPay.Infrastructure.Common;
using MPay.Infrastructure.DAL.Repositories;
using MPay.Infrastructure.DAL.UnitOfWork;
using MPay.Infrastructure.DAL;
using MPay.Infrastructure.Exceptions;
using MPay.Infrastructure.Services;
using MPay.Infrastructure.Validation;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Infrastructure;

internal static class Extensions
{
    internal static void AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<IClock, UtcClock>();
    }

    internal static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidationErrorMapper, ValidationErrorMapper>();
    }

    internal static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<PurchaseTimeoutService>();
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

    internal static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PurchaseTimeoutOptions>(configuration.GetSection(GetOptionsSectionName<PurchaseTimeoutOptions>()));
    }

    private static string GetOptionsSectionName<T>()
        => typeof(T).Name.Replace("Options", string.Empty);
}