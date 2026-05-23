using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.RulingReprocess.Queries.ListRulingReprocessRequests;

public record ListRulingReprocessRequestsQuery(
    RulingReprocessRequestStatus? Status,
    int Page = 1,
    int PageSize = 50) : IRequest<RulingReprocessListResult>;
