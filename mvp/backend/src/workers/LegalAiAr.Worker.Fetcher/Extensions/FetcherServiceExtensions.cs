using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Crawling.Sources;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Fetcher.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Worker.Fetcher.Extensions;

public static class FetcherServiceExtensions
{
    public static IServiceCollection AddFetcherServices(this IServiceCollection services)
    {
        services.AddScoped<IStrategyResolver<IFetchStrategy>>(sp =>
        {
            var blobStorage = sp.GetRequiredService<IBlobStorageService>();
            var builder = new StrategyResolverBuilder<IFetchStrategy>();

            builder.Register(EntityType.Ruling, 1,
                new CrawlerSourceFetchAdapter(sp.GetRequiredService<CsjnAcuerdoDiscoverySource>(), blobStorage));

            builder.Register(EntityType.Statute, 2,
                new CrawlerSourceFetchAdapter(sp.GetRequiredService<SaijLegislationCrawlerSource>(), blobStorage));

            builder.Register(EntityType.Ruling, 3,
                new CrawlerSourceFetchAdapter(sp.GetRequiredService<SaijRulingCrawlerSource>(), blobStorage));

            return builder.Build();
        });

        return services;
    }
}
