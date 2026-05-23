using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>Registers CSJN sjconsulta JSON HTTP client, options, and request gate.</summary>
public static class CsjnSjconsultaServiceExtensions
{
    public static IServiceCollection AddCsjnSjconsultaApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CsjnApiOptions>()
            .Bind(configuration.GetSection(CsjnApiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<CsjnApiRequestGate>();

        services.AddHttpClient<CsjnSjconsultaJsonTransport>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CsjnApiOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(options.Value.RequestTimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", "LegalAiAr-CSJN/1.0");
        });

        return services;
    }
}
