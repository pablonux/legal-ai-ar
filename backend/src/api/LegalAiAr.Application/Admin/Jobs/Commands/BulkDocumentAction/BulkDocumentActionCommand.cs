using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;

/// <summary>
/// Bulk update documents in a job at a specific stage.
/// Supports reprocess (Failed → Pending) and discard (Failed → Discarded).
/// </summary>
public record BulkDocumentActionCommand(
    Guid JobId,
    PipelineStage Stage,
    BulkDocumentAction Action) : IRequest<BulkDocumentActionResult>;

public enum BulkDocumentAction
{
    Reprocess,
    Discard,
    RequeuePending
}

public record BulkDocumentActionResult(
    int AffectedCount,
    string Message);
