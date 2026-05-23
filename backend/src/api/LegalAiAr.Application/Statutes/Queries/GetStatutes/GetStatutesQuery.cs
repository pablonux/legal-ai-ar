using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Statutes.DTOs;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutes;

public record GetStatutesQuery(
    string? Search,
    NormType? NormType,
    NormativeLevel? NormativeLevel,
    LegalBranch? LegalBranch,
    bool? IsVigente,
    int Page = 1,
    int PageSize = 25) : IRequest<StatutePageDto>;

public record StatutePageDto(
    IReadOnlyList<StatuteListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
