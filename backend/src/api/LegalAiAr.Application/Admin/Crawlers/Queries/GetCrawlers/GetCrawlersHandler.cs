using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;

/// <summary>
/// Handles GetCrawlersQuery: returns all crawlers or single crawler by sourceId.
/// </summary>
public class GetCrawlersHandler : IRequestHandler<GetCrawlersQuery, IReadOnlyList<CrawlerConfigDto>>
{
    private readonly ICrawlerConfigRepository _configs;

    public GetCrawlersHandler(ICrawlerConfigRepository configs)
    {
        _configs = configs;
    }

    public async Task<IReadOnlyList<CrawlerConfigDto>> Handle(GetCrawlersQuery request, CancellationToken cancellationToken)
    {
        if (request.SourceId.HasValue)
        {
            var config = await _configs.GetBySourceIdAsync(request.SourceId.Value, cancellationToken)
                ?? throw new NotFoundException($"Crawler configuration for source {request.SourceId} not found.");

            return new[] { MapToDto(config) };
        }

        var all = await _configs.GetAllAsync(cancellationToken);
        return all.Select(MapToDto).ToList();
    }

    private static CrawlerConfigDto MapToDto(Core.Entities.CrawlerConfig config)
    {
        return new CrawlerConfigDto(
            SourceId: config.SourceId,
            SourceName: config.Source.Name,
            IsEnabled: config.IsEnabled,
            LastCrawledAt: config.LastCrawledAt,
            LastCrawledStatus: config.LastCrawledStatus,
            LastDocumentCount: config.LastDocumentCount);
    }
}
