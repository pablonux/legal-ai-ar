using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;

/// <summary>
/// Command to update crawler configuration. Phase 1: only IsEnabled.
/// </summary>
/// <param name="SourceId">Source to update (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
/// <param name="IsEnabled">Enable or disable the source for crawling.</param>
public record UpdateCrawlerConfigCommand(int SourceId, bool IsEnabled) : IRequest<CrawlerConfigDto>;
