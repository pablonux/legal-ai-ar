using LegalAiAr.Application.Mediation;
using BulkDocAction = LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction.BulkDocumentAction;

namespace LegalAiAr.Application.Admin.Jobs.Commands.SingleFailedDocumentAction;

public record SingleFailedDocumentActionCommand(
    Guid JobId,
    Guid DocumentId,
    BulkDocAction Action) : IRequest<SingleFailedDocumentActionResult>;

public record SingleFailedDocumentActionResult(int AffectedCount, string Message);
