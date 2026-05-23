using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;

public class RunCrawlerHandler : IRequestHandler<RunCrawlerCommand, RunCrawlerResult>
{
    private readonly IQueuePublisher _publisher;
    private readonly ICrawlerConfigRepository _configs;
    private readonly PipelineQueueNames _queueNames;

    public RunCrawlerHandler(IQueuePublisher publisher, ICrawlerConfigRepository configs, PipelineQueueNames queueNames)
    {
        _publisher = publisher;
        _configs = configs;
        _queueNames = queueNames;
    }

    public async Task<RunCrawlerResult> Handle(RunCrawlerCommand request, CancellationToken cancellationToken)
    {
        var config = await _configs.GetBySourceIdAsync(request.SourceId, cancellationToken)
            ?? throw new NotFoundException($"Crawler configuration for source {request.SourceId} not found.");

        if (!config.IsEnabled)
            throw new DomainException($"Source {config.Source.Name} is disabled. Enable it before triggering a crawl.");

        var isFallosDestacados = request.Type.Equals("fallos-destacados", StringComparison.OrdinalIgnoreCase);

        if (request.Type.Equals("incremental", StringComparison.OrdinalIgnoreCase)
            && !request.Since.HasValue
            && !config.LastCrawledAt.HasValue)
        {
            throw new DomainException(
                "For incremental crawl, 'since' date is required when the source has never been crawled (no LastCrawledAt).");
        }

        var since = isFallosDestacados ? null : ResolveSince(request, config);
        var isByRange = request.Type.Equals("by-range", StringComparison.OrdinalIgnoreCase);

        var entityType = ResolveEntityType(request.DocumentType);

        var message = new DiscovererMessage(
            EntityType: entityType,
            SourceId: request.SourceId,
            Type: request.Type.ToLowerInvariant(),
            Since: since,
            DateFrom: isByRange ? request.DateFrom : null,
            DateTo: isByRange ? request.DateTo : null,
            UseCache: request.UseCache,
            Reprocess: request.Reprocess,
            MaxDocuments: request.MaxDocuments,
            SkipDocuments: request.SkipDocuments);

        await _publisher.PublishAsync(_queueNames.Discoverer, message, cancellationToken);

        return new RunCrawlerResult(
            Success: true,
            Message: $"Crawl triggered for source {config.Source.Name}");
    }

    private static EntityType ResolveEntityType(string documentType)
    {
        return documentType.ToLowerInvariant() switch
        {
            "statute" or "legislation" => EntityType.Statute,
            _ => EntityType.Ruling
        };
    }

    private static DateOnly? ResolveSince(RunCrawlerCommand request, Core.Entities.CrawlerConfig config)
    {
        if (request.Type.Equals("by-range", StringComparison.OrdinalIgnoreCase))
            return null;

        if (request.Since.HasValue)
            return request.Since.Value;

        if (config.LastCrawledAt.HasValue)
            return DateOnly.FromDateTime(config.LastCrawledAt.Value);

        return null;
    }
}
