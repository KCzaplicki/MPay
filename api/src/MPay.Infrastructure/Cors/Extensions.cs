using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MPay.Infrastructure.Cors;

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
        if (corsOrigins is null) return;

        var corsOriginsArray = corsOrigins.Split(CorsOriginsSeparator).ToArray();
        var enableAll = corsOriginsArray.Contains(CorsEnableAllOrigins) && corsOriginsArray.Length == 1;

        app.UseCors(builder =>
        {
            if (enableAll)
                builder.AllowAnyOrigin();
            else
                builder.WithOrigins(corsOriginsArray);

            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
    }
}