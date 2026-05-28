using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.ReprocessNextFailedDocuments;

public sealed class ReprocessNextFailedDocumentsHandler
    : IRequestHandler<ReprocessNextFailedDocumentsCommand, BulkDocumentActionResult>
{
    private const int MinTake = 1;
    private const int MaxTake = 50;

    private readonly IDocumentRepository _documentRepository;
    private readonly IIngestionJobRepository _ingestionJobRepository;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public ReprocessNextFailedDocumentsHandler(
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
        ReprocessNextFailedDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, MinTake, MaxTake);

        if (request.Stage == PipelineStage.Discoverer)
        {
            return new BulkDocumentActionResult(
                0,
                "La etapa Discoverer no se puede reprocesar desde admin (sin publicación de cola).");
        }

        var candidates =
            await _documentRepository.GetFailedDocumentsOldestFirstAsync(
                request.JobId, request.Stage, take, cancellationToken);

        if (candidates.Count == 0)
            return new BulkDocumentActionResult(0, "No hay documentos Failed en esa etapa para reprocesar.");

        var transitioned = new List<Document>(candidates.Count);
        foreach (var doc in candidates)
        {
            if (doc.CurrentStage != request.Stage || doc.Status != DocumentStatus.Failed)
                continue;

            var ok = await _documentRepository.TryTransitionSingleFailedAsync(
                doc.Id, request.JobId, DocumentStatus.Pending, cancellationToken);
            if (ok)
                transitioned.Add(doc);
        }

        if (transitioned.Count == 0)
        {
            return new BulkDocumentActionResult(
                0, "Ningún documento pudo pasar de Failed a Pending (¿concurrencia?).");
        }

        await _ingestionJobRepository.DecrementDocumentsFailedAsync(
            request.JobId, transitioned.Count, cancellationToken);
        await _ingestionJobRepository.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);

        var queued = 0;
        foreach (var doc in transitioned)
        {
            var published = await DocumentStageQueuePublisher.TryPublishForStageAsync(
                doc, request.Stage, _publisher, _queueNames, cancellationToken);
            if (!published)
            {
                throw new DomainException(
                    $"No se pudo encolar el documento {doc.Id} en la etapa {request.Stage}.");
            }

            queued++;
        }

        return new BulkDocumentActionResult(
            queued,
            $"{transitioned.Count} documentos reabiertos; {queued} mensajes publicados en {request.Stage}.");
    }
}
