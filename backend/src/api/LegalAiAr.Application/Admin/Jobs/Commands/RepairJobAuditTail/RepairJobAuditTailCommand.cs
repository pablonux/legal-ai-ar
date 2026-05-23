using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RepairJobAuditTail;

public record RepairJobAuditTailCommand(Guid JobId) : IRequest<RepairJobAuditTailResult>;

public sealed class RepairJobAuditTailHandler : IRequestHandler<RepairJobAuditTailCommand, RepairJobAuditTailResult>
{
    private const string ErrorMessage =
        "Auditoría: Pending en Persister o Indexer sin avance — probable mensaje de cola perdido o skip tras fallo de red u operación del worker.";

    private const string ErrorType = "AuditOrphanPipeline";

    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;

    public RepairJobAuditTailHandler(IIngestionJobRepository jobs, IDocumentRepository documents)
    {
        _jobs = jobs;
        _documents = documents;
    }

    public async Task<RepairJobAuditTailResult> Handle(
        RepairJobAuditTailCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var candidates = await _documents.GetAuditRiskPipelineDocumentsForJobAsync(
            request.JobId, 500, cancellationToken);

        var toFail = candidates
            .Where(d =>
                d.Status == DocumentStatus.Pending
                && (d.CurrentStage == PipelineStage.Persister || d.CurrentStage == PipelineStage.Indexer))
            .ToList();

        if (toFail.Count == 0)
        {
            return new RepairJobAuditTailResult(
                0,
                "No hay documentos Pending en Persister o Indexer para este job.");
        }

        var affected = 0;
        foreach (var doc in toFail)
        {
            await _documents.SetFailedAsync(doc.Id, ErrorMessage, ErrorType, cancellationToken);
            await _jobs.IncrementDocumentsFailedAsync(job.Id, cancellationToken);
            affected++;
        }

        await _jobs.TryCompleteIfDoneAsync(job.Id, cancellationToken);

        return new RepairJobAuditTailResult(
            affected,
            $"Se marcaron {affected} documento(s) como Failed ({ErrorType}). Revisá el job y los contadores; podés volver a ejecutar «Auditar».");
    }
}
