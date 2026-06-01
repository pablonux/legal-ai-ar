using LegalAiAr.Application.Rulings.DTOs;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Common.Mappings;

/// <summary>
/// Static mapping methods for the Rulings domain.
/// Compile-time-safe, zero-reflection mapping methods for the Rulings domain.
/// </summary>
public static class RulingMapper
{
    public static RulingSearchResultDto ToSearchResultDto(SearchResultItem s) =>
        new(s.RulingId, s.CaseTitle, s.Summary, s.Holding, s.RulingDate,
            s.JurisdictionArea, s.Instance, s.Court, s.Keywords,
            s.RulingDirection, s.Score, s.Highlight,
            s.LegalBranch, s.PrecedentWeight, s.IsPlenario, s.IsLeadingCase);

    public static List<RulingSearchResultDto> ToSearchResultDtos(IReadOnlyList<SearchResultItem> items) =>
        items.Select(ToSearchResultDto).ToList();

    public static RelatedRulingDto ToRelatedDto(SearchResultItem s) =>
        new(s.RulingId, s.CaseTitle, s.RulingDate,
            s.JurisdictionArea, s.Instance, s.Score);

    public static List<RelatedRulingDto> ToRelatedDtos(IReadOnlyList<SearchResultItem> items) =>
        items.Select(ToRelatedDto).ToList();

    public static CourtDto ToCourtDto(Court c) =>
        new(c.Id, c.Name, c.JurisdictionArea, c.Territory, c.Instance,
            c.CourtCategory?.ToString(), c.Fuero?.ToString(), c.InstanceLevel, c.GovernmentLevel?.ToString());

    public static RulingDto ToDto(Ruling s) =>
        new(s.Id, s.SourceId, s.ExternalId, s.CaseTitle, s.CaseNumber, s.RulingDate,
            ToCourtDto(s.Court),
            s.JurisdictionArea, s.Instance, s.Jurisdiction,
            s.ResourceType, s.RulingDirection, s.SubjectArea,
            s.LegalBranch?.ToString(), s.PrecedentWeight?.ToString(),
            s.IsPlenario, s.IsLeadingCase, s.IsUnconstitutional,
            s.Summary, s.Holding, s.FullText, s.BlobPath,
            s.RatioDecidendi, s.DoctrinaLegal,
            s.IndexedAt,
            s.Status.ToString(),
            s.RulingParticipations
                .OrderBy(rp => rp.Person.DisplayName)
                .Select(rp => new PersonParticipationDto(
                    rp.PersonId,
                    rp.Person.DisplayName,
                    rp.Role.ToString()))
                .ToList(),
            s.RulingKeywords
                .OrderBy(rk => rk.SortOrder)
                .Select(rk => new KeywordDto(rk.Keyword.Id, rk.Keyword.Description, rk.Keyword.ThesaurusTermId))
                .ToList(),
            s.RulingStatutes
                .Select(rs => new StatuteDto(
                    rs.Statute.Number, rs.Statute.Name, rs.Articles, rs.Statute.Url,
                    rs.Statute.NormType?.ToString(), rs.Statute.NormativeLevel?.ToString(),
                    rs.Statute.LegalBranch?.ToString(), rs.Statute.IssuingBody,
                    rs.Statute.SanctionDate?.ToString("yyyy-MM-dd"),
                    rs.Statute.EffectiveFrom?.ToString("yyyy-MM-dd"),
                    rs.Statute.EffectiveTo?.ToString("yyyy-MM-dd")))
                .ToList(),
            s.OutboundCitations
                .Select(c => new CitationDto(
                    c.ExternalAlias, c.CitationType.ToString(),
                    c.TargetRulingId, c.TargetRuling != null ? c.TargetRuling.CaseTitle : null))
                .ToList(),
            s.Votes
                .OrderByDescending(v => v.VoteType == Core.Enums.VoteType.Majority)
                .ThenBy(v => v.VoteType.ToString())
                .Select(v => new VoteDto(
                    v.Id,
                    v.VoteType.ToString(),
                    v.Pages,
                    v.Summary,
                    v.Participations
                        .Select(p => p.Person.DisplayName)
                        .OrderBy(n => n)
                        .ToList()))
                .ToList(),
            s.LegalDoctrines
                .Select(d => new LegalDoctrineDto(
                    d.Id, d.Statement, d.Topic, d.IsOverruled,
                    d.OverruledByRulingId, d.OverruledByRuling?.CaseTitle))
                .ToList(),
            s.ProsecutorOpinion is { } po
                ? new ProsecutorOpinionDto(po.ProsecutorName, po.Summary, po.RecommendedDirection, po.AgreedWithCourt)
                : null);
}
