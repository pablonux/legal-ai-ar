using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;

/// <summary>
/// Query to get crawler configurations. When SourceId is null, returns all; when set, returns single crawler (404 if not found).
/// </summary>
/// <param name="SourceId">Optional. When set, returns only that crawler. Null returns all.</param>
public record GetCrawlersQuery(int? SourceId = null) : IRequest<IReadOnlyList<CrawlerConfigDto>>;
