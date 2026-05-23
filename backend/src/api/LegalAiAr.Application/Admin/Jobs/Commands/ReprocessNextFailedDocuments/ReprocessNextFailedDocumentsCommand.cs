using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.Jobs.Commands.ReprocessNextFailedDocuments;

/// <summary>
/// Reprocesses up to N oldest Failed documents at a stage for a job (Failed → Pending + queue publish).
/// </summary>
public record ReprocessNextFailedDocumentsCommand(
    Guid JobId,
    PipelineStage Stage,
    int Take = 10) : IRequest<BulkDocumentActionResult>;
