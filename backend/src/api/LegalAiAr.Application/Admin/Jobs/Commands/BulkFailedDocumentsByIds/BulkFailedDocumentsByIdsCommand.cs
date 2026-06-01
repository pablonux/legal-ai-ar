using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using BulkDocAction = LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction.BulkDocumentAction;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Commands.BulkFailedDocumentsByIds;

/// <summary>
/// Reprocess or discard many failed documents by explicit ids (admin UI filtered list).
/// </summary>
public record BulkFailedDocumentsByIdsCommand(
    Guid JobId,
    IReadOnlyList<Guid> DocumentIds,
    BulkDocAction Action) : IRequest<BulkDocumentActionResult>;
