using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Proceedings.Queries;

public record GetProceedingsQuery(
    string? Search,
    ProcessType? ProcessType,
    LegalBranch? LegalBranch,
    int? CourtId,
    ProcessStatus? Status,
    int Page = 1,
    int PageSize = 25) : IRequest<ProceedingPageDto>;
