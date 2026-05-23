using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Infrastructure.Crawling.Sources;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Discoverer.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Worker.Discoverer.Extensions;

public static class DiscovererServiceExtensions
{
    public static IServiceCollection AddDiscovererServices(this IServiceCollection services)
    {
        services.AddScoped<IStrategyResolver<IDiscoverStrategy>>(sp =>
        {
            var builder = new StrategyResolverBuilder<IDiscoverStrategy>();

            // CSJN Rulings (Source 1) -> routes to acuerdo or fallos-destacados based on type
            builder.Register(EntityType.Ruling, 1,
                new CsjnDiscoverAdapter(
                    sp.GetRequiredService<CsjnAcuerdoDiscoverySource>(),
                    sp.GetRequiredService<CsjnFallosDestacadosSource>()));

            // SAIJ Legislation/Statutes (Source 2) -> SaijLegislationCrawlerSource
            builder.Register(EntityType.Statute, 2,
                new CrawlerSourceDiscoverAdapter(sp.GetRequiredService<SaijLegislationCrawlerSource>()));

            // SAIJ Rulings (Source 3) -> SaijRulingCrawlerSource
            builder.Register(EntityType.Ruling, 3,
                new CrawlerSourceDiscoverAdapter(sp.GetRequiredService<SaijRulingCrawlerSource>()));

            return builder.Build();
        });

        return services;
    }
}
