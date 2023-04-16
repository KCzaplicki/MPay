using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MPay.Abstractions.Common;
using MPay.Abstractions.Events;
using MPay.Abstractions.FeatureFlags;
using MPay.Core.Configurations;
using MPay.Infrastructure.Common;
using MPay.Infrastructure.Events;
using MPay.Infrastructure.FeatureFlags;
using MPay.Infrastructure.Services;
using MPay.Infrastructure.Validation;
using MPay.Infrastructure.Webhooks;
using Serilog;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Infrastructure;

internal static class Extensions
{
    private const string CorsOriginsSectionName = "CorsOrigins";
    private const string CorsOriginsSeparator = ";";
    private const string CorsEnableAllOrigins = "*";

    public static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors();
    }
    
    public static void UseCors(this WebApplication app, IConfiguration configuration)
    {
        var corsOrigins = configuration.GetSection(CorsOriginsSectionName).Get<string>();
        if (corsOrigins is null)
        {
            return;
        }    
            
        var corsOriginsArray = corsOrigins.Split(CorsOriginsSeparator).ToArray();
        var enableAll = corsOriginsArray.Contains(CorsEnableAllOrigins) && corsOriginsArray.Length == 1;
        
        app.UseCors(builder =>
        {
            if (enableAll) 
            {
                builder.AllowAnyOrigin();
            }
            else
            {
                builder.WithOrigins(corsOriginsArray);
            }
            
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
    }
    
    internal static void AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<IClock, UtcClock>();
    }

    internal static void AddFeatureFlags(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FeatureFlagsOptions>(configuration.GetSection(GetOptionsSectionName<FeatureFlagsOptions>()));
        services.AddScoped<IFeatureFlagsService, FeatureFlagsService>();
    }
    
    internal static void AddWebhooks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebhooksOptions>(configuration.GetSection(GetOptionsSectionName<WebhooksOptions>()));
        services.AddScoped<IWebhookClient, WebhookClient>();
        services.AddHttpClient<IWebhookClient, WebhookClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetService<IOptions<WebhooksOptions>>()?.Value ?? new WebhooksOptions();
            client.BaseAddress = new Uri(options.Url);
        });
    }

    internal static void AddEventsBackgroundHandling(this IServiceCollection services)
    {
        services.AddSingleton<IAsyncEventDispatcher, AsyncEventDispatcher>();
        services.AddSingleton<IEventChannel, EventChannel>();
        services.AddHostedService<BackgroundDispatcherService>();
    }

    internal static void AddValidationErrorsHandling(this IServiceCollection services)
    {
        services.AddScoped<IValidationErrorMapper, ValidationErrorMapper>();
    }

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