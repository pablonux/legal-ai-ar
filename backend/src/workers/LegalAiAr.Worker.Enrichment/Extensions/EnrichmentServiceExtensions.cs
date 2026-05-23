using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Enrichment.Chunking;
using LegalAiAr.Worker.Enrichment.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Enrichment.Extensions;

public static class EnrichmentServiceExtensions
{
    public static IServiceCollection AddEnrichmentServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TextChunkingService>();
        services.AddScoped<CsjnEnrichmentStrategy>();

        services.AddScoped<IStrategyResolver<IEnrichStrategy>>(sp =>
        {
            var blobStorage = sp.GetRequiredService<IBlobStorageService>();
            var chunkingService = sp.GetRequiredService<TextChunkingService>();
            var builder = new StrategyResolverBuilder<IEnrichStrategy>();

            builder.Register(EntityType.Ruling, 1,
                new CsjnRulingEnrichStrategy(
                    sp.GetRequiredService<CsjnEnrichmentStrategy>(),
                    blobStorage,
                    sp.GetRequiredService<ILogger<CsjnRulingEnrichStrategy>>()));

            builder.Register(EntityType.Statute, 2,
                new SaijLegislationEnrichStrategy(
                    chunkingService,
                    blobStorage,
                    sp.GetRequiredService<ILogger<SaijLegislationEnrichStrategy>>()));

            builder.Register(EntityType.Ruling, 3,
                new SaijRulingEnrichStrategy(
                    chunkingService,
                    blobStorage,
                    sp.GetRequiredService<ILogger<SaijRulingEnrichStrategy>>()));

            return builder.Build();
        });

        // Backward compat: old resolver still used by old tools/requeue handlers
        services.AddScoped<IEnrichmentStrategyResolver>(sp =>
            new EnrichmentStrategyResolver(sp.GetRequiredService<CsjnEnrichmentStrategy>()));

        return services;
    }
}
