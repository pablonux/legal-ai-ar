using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobAudit;

public record GetJobAuditQuery(Guid JobId) : IRequest<JobAuditDto>;
