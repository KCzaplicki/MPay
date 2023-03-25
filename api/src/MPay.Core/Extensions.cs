using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPay.Core.DAL;

[assembly: InternalsVisibleTo("MPay.Api")]
namespace MPay.Core;

internal static class Extensions
{
    public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MPayDbContext>(
            options => options.UseSqlite(configuration.GetConnectionString("MPayConnectionString")));
    }

    public static void UseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MPayDbContext>();
        dbContext.Database.Migrate();
    }
}