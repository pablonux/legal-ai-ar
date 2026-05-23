using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Statutes.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Statutes.Queries.GetStatuteById;

public class GetStatuteByIdHandler : IRequestHandler<GetStatuteByIdQuery, StatuteDetailDto?>
{
    private readonly IStatuteRepository _repo;

    public GetStatuteByIdHandler(IStatuteRepository repo) => _repo = repo;

    public async Task<StatuteDetailDto?> Handle(GetStatuteByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _repo.GetWithDetailsAsync(request.Id, cancellationToken);
        if (s is null) return null;

        var recentRulings = s.RulingStatutes
            .OrderByDescending(rs => rs.Ruling.RulingDate)
            .Take(10)
            .Select(rs => new StatuteRulingDto(
                rs.Ruling.Id,
                rs.Ruling.CaseTitle,
                rs.Ruling.RulingDate,
                rs.Ruling.Court?.Name,
                rs.Articles))
            .ToList();

        var relations = s.OutboundNormRelations
            .Select(nr => new NormRelationDto(
                nr.TargetStatuteId, nr.TargetStatute.Number, nr.TargetStatute.Name,
                nr.RelationType.ToString(), true))
            .Concat(s.InboundNormRelations
                .Select(nr => new NormRelationDto(
                    nr.SourceStatuteId, nr.SourceStatute.Number, nr.SourceStatute.Name,
                    nr.RelationType.ToString(), false)))
            .ToList();

        return new StatuteDetailDto(
            s.Id, s.Number, s.Name, s.Url,
            s.NormType, s.NormativeLevel, s.LegalBranch,
            s.IssuingBody, s.IssuingBodyName, s.SanctionDate,
            s.PublicationDate, s.EffectiveFrom, s.EffectiveTo,
            s.OfficialUrl, s.SaijId, s.Status?.ToString(),
            s.FullText != null, s.IsVigente, s.RulingStatutes.Count,
            recentRulings, relations);
    }
}
