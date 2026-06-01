using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.RulingReprocess.Commands.RetryRulingReprocess;

public record RetryRulingReprocessCommand(Guid RequestId, string RequestedBy) : IRequest<EnqueueRulingReprocessResult>;
