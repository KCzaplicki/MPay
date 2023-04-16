using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace MPay.Infrastructure.Webhooks;

internal static class Extensions
{
    internal static void AddWebhooks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebhooksOptions>(configuration.GetSection(MPay.Infrastructure.Extensions.GetOptionsSectionName<WebhooksOptions>()));
        services.AddScoped<IWebhookClient, WebhookClient>();
        services.AddHttpClient<IWebhookClient, WebhookClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetService<IOptions<WebhooksOptions>>()?.Value ?? new WebhooksOptions();
                client.BaseAddress = new Uri(options.Url);
            })
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var options = serviceProvider.GetService<IOptions<WebhooksOptions>>()?.Value ?? new WebhooksOptions();
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(options.RetryLimit,
                        retryAttempt => TimeSpan.FromSeconds(options.RetryIntervalsInSeconds[retryAttempt - 1]));
            });
    }
}