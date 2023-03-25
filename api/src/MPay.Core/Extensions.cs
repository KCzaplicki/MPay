using System.Runtime.CompilerServices;
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
}