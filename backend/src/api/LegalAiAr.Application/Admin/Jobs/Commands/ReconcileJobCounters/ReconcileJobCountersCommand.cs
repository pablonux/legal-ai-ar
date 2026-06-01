using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobCounters;

public record ReconcileJobCountersCommand(Guid JobId) : IRequest<ReconcileJobCountersResultDto>;

public sealed class ReconcileJobCountersHandler : IRequestHandler<ReconcileJobCountersCommand, ReconcileJobCountersResultDto>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;

    public ReconcileJobCountersHandler(IIngestionJobRepository jobs, IDocumentRepository documents)
    {
        _jobs = jobs;
        _documents = documents;
    }

    public async Task<ReconcileJobCountersResultDto> Handle(
        ReconcileJobCountersCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        if (await _documents.HasPendingDocumentsAsync(request.JobId, cancellationToken))
        {
            throw new DomainException(
                "No se pueden rearmar los contadores mientras existan documentos Pending o Processing para este job.");
        }

        var previous = new IngestionPipelineCounters(
            job.DocumentsCrawled,
            job.DocumentsParsed,
            job.DocumentsEnriched,
            job.DocumentsPersisted,
            job.DocumentsIndexed,
            job.DocumentsFailed);

        var updated = await _jobs.ReconcilePipelineCountersFromDocumentsAsync(request.JobId, cancellationToken);
        if (updated is null)
            throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var completionApplied = await _jobs.TryCompleteIfDoneAsync(request.JobId, cancellationToken);

        return new ReconcileJobCountersResultDto(previous, updated.Value, completionApplied);
    }
}
