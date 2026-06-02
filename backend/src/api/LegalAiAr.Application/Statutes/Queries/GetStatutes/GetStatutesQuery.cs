using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutes;

public record GetStatutesQuery(
    string? Search,
    NormType? NormType,
    NormativeLevel? NormativeLevel,
    LegalBranch? LegalBranch,
    bool? IsVigente,
    int Page = 1,
    int PageSize = 25) : IRequest<StatutePageResponse>;
