using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobProcessingDocuments;

public record ReconcileJobProcessingDocumentsCommand(
    Guid JobId,
    int MinAgeMinutes = 15,
    PipelineStage? Stage = null) : IRequest<ReconcileJobProcessingDocumentsResultDto>;

public record ReconcileJobProcessingDocumentsResultDto(int RowsReset);

public sealed class ReconcileJobProcessingDocumentsHandler
    : IRequestHandler<ReconcileJobProcessingDocumentsCommand, ReconcileJobProcessingDocumentsResultDto>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;

    public ReconcileJobProcessingDocumentsHandler(IIngestionJobRepository jobs, IDocumentRepository documents)
    {
        _jobs = jobs;
        _documents = documents;
    }

    public async Task<ReconcileJobProcessingDocumentsResultDto> Handle(
        ReconcileJobProcessingDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var minAge = Math.Clamp(request.MinAgeMinutes, 0, 24 * 60);
        var n = await _documents.ResetStaleProcessingToPendingForJobAsync(
            request.JobId,
            request.Stage,
            minAge,
            cancellationToken);

        return new ReconcileJobProcessingDocumentsResultDto(n);
    }
}
