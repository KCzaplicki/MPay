using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MPay.Infrastructure.DAL;

namespace MPay.API.IntegrationTests.Endpoints;

[CollectionDefinition("Sequential", DisableParallelization = true)]
public abstract class BaseEndpointsTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string IntegrationTestsEnvironmentName = "Testing";
    
    protected readonly WebApplicationFactory<Program> Factory;

    protected BaseEndpointsTest(WebApplicationFactory<Program> factory)
    {
        Factory = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(IntegrationTestsEnvironmentName);
            });
    }
    
    protected async Task InitializeDatabaseAsync(params object[] entities)
    {
        using var scope = Factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var mPayDbContext = scopedServices.GetRequiredService<MPayDbContext>();

        foreach (var entity in entities) await mPayDbContext.AddAsync(entity);

        await mPayDbContext.SaveChangesAsync();
    }
}