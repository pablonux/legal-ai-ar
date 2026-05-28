using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Infrastructure.Crawling.Options;
using LegalAiAr.Infrastructure.Crawling.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>
/// Extension methods for registering CrawlerWorker services.
/// </summary>
public static class CrawlerServiceExtensions
{
    /// <summary>
    /// Adds crawler sources, options and HttpClient for CSJN.
    /// Registers three CSJN sources: acuerdo (HTTP-only, default), sumarios (HTTP-only, historical),
    /// and selenium (legacy fallback).
    /// </summary>
    public static IServiceCollection AddCrawlerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CsjnCrawlerOptions>()
            .Bind(configuration.GetSection(CsjnCrawlerOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<CsjnDiscoveryHttpGate>();

        services.AddHttpClient(nameof(CsjnAcuerdoDiscoverySource), (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CsjnCrawlerOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient(nameof(CsjnSumariosDiscoverySource), (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CsjnCrawlerOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient(nameof(CsjnFallosDestacadosSource), (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CsjnCrawlerOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(120);
        });

        services.AddScoped<CsjnAcuerdoDiscoverySource>();
        services.AddScoped<CsjnSumariosDiscoverySource>();
        services.AddScoped<CsjnFallosDestacadosSource>();

        services.AddOptions<SaijCrawlerOptions>()
            .Bind(configuration.GetSection(SaijCrawlerOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient(nameof(SaijLegislationCrawlerSource), (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<SaijCrawlerOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddScoped<SaijLegislationCrawlerSource>();

        services.AddHttpClient(nameof(SaijRulingCrawlerSource), (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<SaijCrawlerOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddScoped<SaijRulingCrawlerSource>();

        services.AddScoped<ICrawlerSourceResolver, CrawlerSourceResolver>();

        return services;
    }
}
