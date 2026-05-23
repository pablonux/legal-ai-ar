using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocumentsSummary;

public record GetJobDocumentsSummaryQuery(Guid JobId) : IRequest<JobDocumentsSummaryDto>;
