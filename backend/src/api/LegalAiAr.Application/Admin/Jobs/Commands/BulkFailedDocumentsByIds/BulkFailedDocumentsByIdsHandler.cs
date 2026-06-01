using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Application.Admin.Jobs.Commands.SingleFailedDocumentAction;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using BulkDocAction = LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction.BulkDocumentAction;

namespace LegalAiAr.Application.Admin.Jobs.Commands.BulkFailedDocumentsByIds;

public sealed class BulkFailedDocumentsByIdsHandler : IRequestHandler<BulkFailedDocumentsByIdsCommand, BulkDocumentActionResult>
{
    private readonly IMediator _mediator;

    public BulkFailedDocumentsByIdsHandler(IMediator mediator) => _mediator = mediator;

    public async Task<BulkDocumentActionResult> Handle(
        BulkFailedDocumentsByIdsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Action is not (BulkDocAction.Reprocess or BulkDocAction.Discard))
            throw new DomainException("Only Reprocess and Discard are supported for bulk by ids.");

        var distinct = request.DocumentIds.Distinct().ToList();
        if (distinct.Count == 0)
            return new BulkDocumentActionResult(0, "No se enviaron documentos.");

        var ok = 0;
        var failures = new List<string>();
        foreach (var documentId in distinct)
        {
            try
            {
                await _mediator.Send(
                    new SingleFailedDocumentActionCommand(request.JobId, documentId, request.Action),
                    cancellationToken);
                ok++;
            }
            catch (Exception ex)
            {
                failures.Add($"{documentId:N}: {ex.Message}");
            }
        }

        var msg = failures.Count == 0
            ? $"{ok} documento(s) procesado(s)."
            : $"{ok} documento(s) procesado(s); {failures.Count} error(es). " + string.Join(" ", failures.Take(3));

        return new BulkDocumentActionResult(ok, msg);
    }
}
