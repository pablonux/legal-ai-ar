using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;

public class BulkDocumentActionHandler : IRequestHandler<BulkDocumentActionCommand, BulkDocumentActionResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IIngestionJobRepository _ingestionJobRepository;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public BulkDocumentActionHandler(
        IDocumentRepository documentRepository,
        IIngestionJobRepository ingestionJobRepository,
        IQueuePublisher publisher,
        PipelineQueueNames queueNames)
    {
        _documentRepository = documentRepository;
        _ingestionJobRepository = ingestionJobRepository;
        _publisher = publisher;
        _queueNames = queueNames;
    }

    public async Task<BulkDocumentActionResult> Handle(
        BulkDocumentActionCommand request, CancellationToken cancellationToken)
    {
        if (request.Action == BulkDocumentAction.RequeuePending)
        {
            var result = await RequeueByStatusAsync(request, DocumentStatus.Pending, cancellationToken);
            if (result.AffectedCount > 0)
                await _ingestionJobRepository.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);
            return result;
        }

        if (request.Action == BulkDocumentAction.Reprocess)
        {
            var count = await _documentRepository.BulkUpdateStatusAsync(
                request.JobId, request.Stage,
                DocumentStatus.Failed, DocumentStatus.Pending,
                cancellationToken);

            if (count == 0)
                return new BulkDocumentActionResult(0, "No failed documents found");

            await _ingestionJobRepository.DecrementDocumentsFailedAsync(request.JobId, count, cancellationToken);
            await _ingestionJobRepository.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);

            var requeued = await RequeueByStatusAsync(request, DocumentStatus.Pending, cancellationToken);
            return new BulkDocumentActionResult(
                requeued.AffectedCount,
                $"{count} failed documents reset and {requeued.AffectedCount} requeued at {request.Stage}");
        }

        if (request.Action == BulkDocumentAction.Discard)
        {
            var count = await _documentRepository.BulkUpdateStatusAsync(
                request.JobId, request.Stage,
                DocumentStatus.Failed, DocumentStatus.Discarded,
                cancellationToken);
            if (count > 0)
                await _ingestionJobRepository.DecrementDocumentsFailedAsync(request.JobId, count, cancellationToken);
            return new BulkDocumentActionResult(count, $"{count} documents at {request.Stage} discarded");
        }

        throw new ArgumentOutOfRangeException(nameof(request.Action));
    }

    private async Task<BulkDocumentActionResult> RequeueByStatusAsync(
        BulkDocumentActionCommand request, DocumentStatus status, CancellationToken cancellationToken)
    {
        var docs = await _documentRepository.GetByJobAsync(
            request.JobId, request.Stage, status,
            skip: 0, take: 10_000, cancellationToken: cancellationToken);

        var queued = 0;
        foreach (var doc in docs)
        {
            if (await DocumentStageQueuePublisher.TryPublishForStageAsync(
                    doc, request.Stage, _publisher, _queueNames, cancellationToken))
                queued++;
        }

        return new BulkDocumentActionResult(queued, $"{queued} {status} documents at {request.Stage} requeued");
    }
}
