using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Stats.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Stats.Queries.GetKbStats;

public class GetKbStatsHandler : IRequestHandler<GetKbStatsQuery, KbStatsDto>
{
    private readonly IKbStatsRepository _stats;

    public GetKbStatsHandler(IKbStatsRepository stats) => _stats = stats;

    public async Task<KbStatsDto> Handle(GetKbStatsQuery request, CancellationToken cancellationToken)
    {
        var raw = await _stats.GetKbStatsAsync(cancellationToken);

        return new KbStatsDto(
            raw.TotalRulings,
            raw.TotalCourts,
            raw.TotalPersons,
            raw.TotalKeywords,
            raw.TotalStatutes,
            raw.TotalCitations,
            raw.EarliestRulingDate,
            raw.LatestRulingDate,
            raw.BySource.Select(s => new SourceStatDto(s.SourceId, s.Name, s.Count)).ToList(),
            raw.ByYear.Select(y => new YearStatDto(y.Year, y.Count)).ToList(),
            raw.TopCourts.Select(c => new CourtStatDto(c.CourtId, c.Name, c.JurisdictionArea, c.Count)).ToList(),
            raw.ByJurisdiction.Select(n => new NameCountDto(n.Name, n.Count)).ToList(),
            raw.ByInstance.Select(n => new NameCountDto(n.Name, n.Count)).ToList(),
            raw.BySubjectArea.Select(n => new NameCountDto(n.Name, n.Count)).ToList(),
            raw.TopKeywords.Select(n => new NameCountDto(n.Name, n.Count)).ToList(),
            new QualityStatsDto(
                raw.Quality.WithSummary,
                raw.Quality.WithHolding,
                raw.Quality.WithFullText,
                raw.Quality.WithKeywords,
                raw.Quality.WithPersons,
                raw.Quality.WithStatutes,
                raw.Quality.WithCitations));
    }
}
