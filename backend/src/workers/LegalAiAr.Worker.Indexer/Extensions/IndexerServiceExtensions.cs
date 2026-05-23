using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Indexer.Chunking;
using LegalAiAr.Worker.Indexer.Steps;
using LegalAiAr.Worker.Indexer.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Extensions;

public static class IndexerServiceExtensions
{
    public static IServiceCollection AddIndexerStrategies(this IServiceCollection services)
    {
        services.AddScoped<IStrategyResolver<IIndexStrategy>>(sp =>
        {
            var builder = new StrategyResolverBuilder<IIndexStrategy>();

            var rulingStrategy = new RulingIndexStrategy(
                sp.GetRequiredService<IBlobStorageService>(),
                sp.GetRequiredService<IRulingRepository>(),
                sp.GetRequiredService<ICourtRepository>(),
                sp.GetRequiredService<IEmbeddingConfigRepository>(),
                sp.GetRequiredService<TextChunkingService>(),
                sp.GetRequiredService<UploadBlobStep>(),
                sp.GetRequiredService<GenerateEmbeddingsStep>(),
                sp.GetRequiredService<IndexSearchStep>(),
                sp.GetRequiredService<ExtractChunkMentionsStep>(),
                sp.GetRequiredService<ResolveCitationsStep>(),
                sp.GetRequiredService<ILogger<RulingIndexStrategy>>());

            builder.Register(EntityType.Ruling, 1, rulingStrategy);
            builder.Register(EntityType.Ruling, 3, rulingStrategy);

            var statuteStrategy = new StatuteIndexStrategy(
                sp.GetRequiredService<ILogger<StatuteIndexStrategy>>());

            builder.Register(EntityType.Statute, 2, statuteStrategy);

            return builder.Build();
        });

        return services;
    }
}
