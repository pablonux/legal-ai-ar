using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;

/// <summary>
/// Handles UpdateCrawlerConfigCommand: updates IsEnabled and returns updated crawler DTO.
/// </summary>
public class UpdateCrawlerConfigHandler : IRequestHandler<UpdateCrawlerConfigCommand, CrawlerConfigDto>
{
    private readonly ICrawlerConfigRepository _configs;

    public UpdateCrawlerConfigHandler(ICrawlerConfigRepository configs)
    {
        _configs = configs;
    }

    public async Task<CrawlerConfigDto> Handle(UpdateCrawlerConfigCommand request, CancellationToken cancellationToken)
    {
        var config = await _configs.GetBySourceIdAsync(request.SourceId, cancellationToken)
            ?? throw new NotFoundException($"Crawler configuration for source {request.SourceId} not found.");

        await _configs.UpdateIsEnabledAsync(request.SourceId, request.IsEnabled, cancellationToken);

        var updated = await _configs.GetBySourceIdAsync(request.SourceId, cancellationToken)
            ?? throw new NotFoundException($"Crawler configuration for source {request.SourceId} not found.");

        return MapToDto(updated);
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
