using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Ai;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Chunking;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Infrastructure.Graph;
using LegalAiAr.Infrastructure.Health;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using LegalAiAr.Infrastructure.Queue;
using LegalAiAr.Infrastructure.Search;
using LegalAiAr.Infrastructure.Thesaurus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DbContext, repositories, Storage Queue services (publisher and receiver) and SqlGraphService.
    /// Replaces Azure Service Bus and Neo4j for Fase 1.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Configuration root. Uses AzureSql for connection string.</param>
    public static IServiceCollection AddLegalAiArInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["AzureSql:ConnectionString"]
            ?? configuration["ConnectionStrings:DefaultConnection"]
            ?? string.Empty;

        var isDevelopment = configuration["DOTNET_ENVIRONMENT"] == "Development"
            || configuration["ASPNETCORE_ENVIRONMENT"] == "Development";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
            if (isDevelopment)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Repositories
        services.AddScoped<IRulingRepository, RulingRepository>();
        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IKeywordRepository, KeywordRepository>();
        services.AddScoped<IStatuteRepository, StatuteRepository>();
        services.AddScoped<ICitationRepository, CitationRepository>();
        services.AddScoped<ICrawlerConfigRepository, CrawlerConfigRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIngestionJobRepository, IngestionJobRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IWorkerPauseStateRepository, WorkerPauseStateRepository>();
        services.AddScoped<IKbStatsRepository, KbStatsRepository>();
        services.AddScoped<IThesaurusRepository, ThesaurusRepository>();
        services.AddScoped<IJudicialProceedingRepository, JudicialProceedingRepository>();
        services.AddScoped<IEmbeddingConfigRepository, EmbeddingConfigRepository>();
        services.AddScoped<IGraphCommunityRepository, GraphCommunityRepository>();
        services.AddScoped<IDocumentStageLogRepository, DocumentStageLogRepository>();
        services.AddScoped<IRulingReprocessRequestRepository, RulingReprocessRequestRepository>();
        services.AddScoped<IRulingReprocessCompletionService, RulingReprocess.RulingReprocessCompletionService>();

        // Graph explorer
        services.AddScoped<IGraphExplorerRepository, GraphExplorerRepository>();

        // Pipeline options (queue prefix)
        services.AddOptions<PipelineOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.QueuePrefix = config["Pipeline:QueuePrefix"] ?? options.QueuePrefix;
            });

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<PipelineOptions>>().Value;
            return new PipelineQueueNames(opts.QueuePrefix);
        });

        services.AddSingleton<IWorkerSignalRPresenceTracker, WorkerSignalRPresenceTracker>();

        // Storage Queue: use AzureStorage or AzureBlob connection string (same Storage Account for Blob + Queue)
        services.AddOptions<StorageQueueOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.ConnectionString = config["AzureStorage:ConnectionString"]
                    ?? config["AzureBlob:ConnectionString"]
                    ?? string.Empty;
            });

        services.AddSingleton<EntityCacheService>();
        services.AddSingleton<ITextChunkingService, TextChunkingService>();
        services.AddSingleton<IQueuePublisher, StorageQueuePublisher>();
        services.AddSingleton<IQueueReceiver>(sp => new StorageQueueReceiver(
            sp.GetRequiredService<IOptions<StorageQueueOptions>>(),
            sp.GetRequiredService<ILogger<StorageQueueReceiver>>(),
            sp.GetService<IWorkerInfraNotifier>()));
        services.AddSingleton<IQueueMetricsService, StorageQueueMetricsService>();
        services.AddSingleton<IDlqService, StorageDlqService>();
        services.AddScoped<IGraphService, SqlGraphService>();
        services.AddScoped<ICommunityDetectionService, CommunityDetectionService>();
        services.AddScoped<ICommunitySummaryService, CommunitySummaryService>();
        services.AddScoped<IAuditService, Audit.AuditService>();

        // Azure AI Search (API and IndexerWorker; requires AzureSearch:Endpoint and AzureSearch:ApiKey)
        services.AddOptions<AzureSearchOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.Endpoint = config["AzureSearch:Endpoint"] ?? string.Empty;
                options.ApiKey = config["AzureSearch:ApiKey"] ?? string.Empty;
                options.RulingIndexName = config["AzureSearch:RulingIndexName"] ?? "rulings-by-ruling";
                options.ChunkIndexName = config["AzureSearch:ChunkIndexName"] ?? "rulings-by-chunk";
            });
        services.AddSingleton<ISearchService, AzureSearchService>();
        services.AddSingleton<ISearchIndexService, AzureSearchService>();

        // Azure Blob Storage (Crawler, Parser, Indexer; PDF storage)
        services.AddOptions<AzureBlobOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.ConnectionString = config["AzureBlob:ConnectionString"] ?? string.Empty;
                options.ContainerName = config["AzureBlob:ContainerName"] ?? "rulings-pdfs";
            });
        services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
        services.AddSingleton<IExternalDownloadCache, BlobExternalDownloadCache>();

        // Azure OpenAI — GPT-5 family: Chat (gpt-5), Mini (gpt-5-mini), Nano (gpt-5-nano), Embeddings (text-embedding-3-large)
        services.AddOptions<AzureOpenAiOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.Endpoint = config["AzureOpenAI:Endpoint"] ?? string.Empty;
                options.ApiKey = config["AzureOpenAI:ApiKey"] ?? string.Empty;
                options.EmbeddingDeploymentName = config["AzureOpenAI:EmbeddingDeploymentName"] ?? options.EmbeddingDeploymentName;
                options.ChatDeploymentName = config["AzureOpenAI:ChatDeploymentName"] ?? options.ChatDeploymentName;
                options.MiniDeploymentName = config["AzureOpenAI:MiniDeploymentName"] ?? options.MiniDeploymentName;
                options.NanoDeploymentName = config["AzureOpenAI:NanoDeploymentName"] ?? options.NanoDeploymentName;
            });
        services.AddSingleton<IEmbeddingService, AzureOpenAiEmbeddingService>();
        services.AddSingleton<IEnrichmentService, AzureOpenAiEnrichmentService>();
        services.AddSingleton<IChatService, AzureOpenAiChatService>();
        services.AddSingleton<IAgentChatService, AzureOpenAiAgentChatService>();
        services.AddScoped<IHealthCheckService, HealthCheckService>();

        // Chat pipeline — LLM services (GPT-4o-mini)
        services.AddSingleton<IGuardrailClassifier, AzureOpenAiGuardrailClassifier>();
        services.AddSingleton<IQueryEnricher, AzureOpenAiQueryEnricher>();
        services.AddSingleton<ISearchQueryPreprocessor, AzureOpenAiSearchQueryPreprocessor>();

        // Global search (multi-entity)
        services.AddScoped<IGlobalSearchService, GlobalSearchService>();

        // Ontology stats (DB counts for ontology viewer)
        services.AddScoped<LegalAiAr.Core.Interfaces.Services.IOntologyStatsProvider, OntologyStatsProvider>();

        // Thesaurus
        services.AddScoped<ISynonymMapGenerator, ThesaurusSynonymMapGenerator>();
        services.AddScoped<IKeywordNormalizationService, KeywordNormalizationService>();
        services.AddScoped<IThesaurusContextProvider, ThesaurusContextProvider>();
        services.AddHttpClient<SaijThesaurusCrawler>(client => client.Timeout = TimeSpan.FromMinutes(15));
        services.AddTransient<IThesaurusCrawler, SaijThesaurusCrawler>();
        services.AddScoped<IThesaurusIngestCatalogBootstrap, ThesaurusIngestCatalogBootstrap>();

        return services;
    }
}
