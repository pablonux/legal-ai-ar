using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.RulingReprocess.Commands.EnqueueRulingReprocess;

public record EnqueueRulingReprocessCommand(
    Guid RulingId,
    string RequestedBy,
    bool UseCache = false) : IRequest<EnqueueRulingReprocessResult>;
