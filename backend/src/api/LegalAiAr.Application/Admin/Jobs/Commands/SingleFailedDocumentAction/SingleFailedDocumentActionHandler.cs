using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using BulkDocAction = LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction.BulkDocumentAction;

namespace LegalAiAr.Application.Admin.Jobs.Commands.SingleFailedDocumentAction;

public class SingleFailedDocumentActionHandler : IRequestHandler<SingleFailedDocumentActionCommand, SingleFailedDocumentActionResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IIngestionJobRepository _ingestionJobRepository;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public SingleFailedDocumentActionHandler(
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

    public async Task<SingleFailedDocumentActionResult> Handle(
        SingleFailedDocumentActionCommand request, CancellationToken cancellationToken)
    {
        if (request.Action is not (BulkDocAction.Reprocess or BulkDocAction.Discard))
            throw new DomainException("Only Reprocess and Discard are supported for a single document.");

        var doc = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new NotFoundException($"Document {request.DocumentId} not found.");

        if (doc.IngestionJobId != request.JobId)
            throw new DomainException("Document does not belong to this job.");

        if (doc.Status != DocumentStatus.Failed)
            throw new DomainException($"Document must be in Failed status (current: {doc.Status}).");

        if (request.Action == BulkDocAction.Discard)
        {
            var discarded = await _documentRepository.TryTransitionSingleFailedAsync(
                request.DocumentId, request.JobId, DocumentStatus.Discarded, cancellationToken);
            if (!discarded)
                throw new DomainException("Could not discard document (concurrent update?).");
            await _ingestionJobRepository.DecrementDocumentsFailedAsync(request.JobId, 1, cancellationToken);
            return new SingleFailedDocumentActionResult(1, "Document discarded.");
        }

        var stage = doc.CurrentStage;
        var updated = await _documentRepository.TryTransitionSingleFailedAsync(
            request.DocumentId, request.JobId, DocumentStatus.Pending, cancellationToken);
        if (!updated)
            throw new DomainException("Could not reset document to Pending (concurrent update?).");

        await _ingestionJobRepository.DecrementDocumentsFailedAsync(request.JobId, 1, cancellationToken);
        await _ingestionJobRepository.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);

        var published = await DocumentStageQueuePublisher.TryPublishForStageAsync(
            doc, stage, _publisher, _queueNames, cancellationToken);
        if (!published)
            throw new DomainException($"Cannot requeue failures at stage {stage}.");

        return new SingleFailedDocumentActionResult(1, $"Document requeued at {stage}.");
    }
}
