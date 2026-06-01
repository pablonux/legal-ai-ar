using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Admin.Jobs.Commands.StartThesaurusIngestJob;

/// <summary>
/// Creates an ingestion job row and runs the TemaTres crawl (and optional keyword normalization) on a thread-pool thread.
/// </summary>
public sealed class StartThesaurusIngestJobHandler : IRequestHandler<StartThesaurusIngestJobCommand, StartThesaurusIngestJobResult>
{
    public const int ThesaurusSourceId = 6;

    private readonly IIngestionJobRepository _jobs;
    private readonly IThesaurusIngestCatalogBootstrap _catalogBootstrap;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StartThesaurusIngestJobHandler> _logger;

    public StartThesaurusIngestJobHandler(
        IIngestionJobRepository jobs,
        IThesaurusIngestCatalogBootstrap catalogBootstrap,
        IServiceScopeFactory scopeFactory,
        ILogger<StartThesaurusIngestJobHandler> logger)
    {
        _jobs = jobs;
        _catalogBootstrap = catalogBootstrap;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<StartThesaurusIngestJobResult> Handle(StartThesaurusIngestJobCommand request, CancellationToken cancellationToken)
    {
        if (await _jobs.HasActiveJobAsync(EntityType.Thesaurus, ThesaurusSourceId, cancellationToken))
        {
            throw new DomainException(
                "Ya hay un job de tesauro en curso para esta fuente. Esperá a que termine o cancelalo antes de iniciar otro.");
        }

        await _catalogBootstrap.EnsureAsync(cancellationToken);

        var job = new IngestionJob
        {
            Id = Guid.NewGuid(),
            SourceId = ThesaurusSourceId,
            EntityType = EntityType.Thesaurus,
            Type = "thesaurus",
            TriggeredBy = "admin",
            StartedAt = DateTime.UtcNow,
            Status = "processing",
            DocumentsDiscovered = 1,
            DiscoveryBatchPublished = true,
            CreationLog = "api:thesaurus-ingest",
        };

        await _jobs.AddAsync(job, cancellationToken);

        var jobId = job.Id;
        var normalize = request.NormalizeKeywords;

        _ = Task.Run(() => RunIngestAsync(jobId, normalize, _scopeFactory, _logger), CancellationToken.None);

        return new StartThesaurusIngestJobResult(
            Success: true,
            Message: "Job de tesauro iniciado. El progreso aparece en el panel de ingesta.",
            JobId: jobId);
    }

    private static async Task RunIngestAsync(
        Guid jobId,
        bool normalizeKeywords,
        IServiceScopeFactory scopeFactory,
        ILogger logger)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var jobs = scope.ServiceProvider.GetRequiredService<IIngestionJobRepository>();
            var crawler = scope.ServiceProvider.GetRequiredService<IThesaurusCrawler>();

            logger.LogInformation("Thesaurus ingest job {JobId}: starting API crawl", jobId);
            await crawler.CrawlAsync(
                onProgress: msg => logger.LogInformation("[Thesaurus {JobId}] {Message}", jobId, msg),
                cancellationToken: CancellationToken.None);

            if (normalizeKeywords)
            {
                var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();
                logger.LogInformation("Thesaurus ingest job {JobId}: starting keyword normalization", jobId);
                await normalizer.NormalizeAllAsync(
                    onProgress: msg => logger.LogInformation("[Thesaurus {JobId}] {Message}", jobId, msg),
                    cancellationToken: CancellationToken.None);
            }

            await jobs.FinalizeThesaurusIngestJobAsync(jobId, success: true, errorSummary: null, CancellationToken.None);
            logger.LogInformation("Thesaurus ingest job {JobId}: completed successfully", jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Thesaurus ingest job {JobId}: failed", jobId);
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var jobs = scope.ServiceProvider.GetRequiredService<IIngestionJobRepository>();
                var msg = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;
                await jobs.FinalizeThesaurusIngestJobAsync(jobId, success: false, errorSummary: msg, CancellationToken.None);
            }
            catch (Exception inner)
            {
                logger.LogError(inner, "Thesaurus ingest job {JobId}: could not persist failure status", jobId);
            }
        }
    }
}
