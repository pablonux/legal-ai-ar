using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RequeueMissingPipelineMessages;

public record RequeueMissingPipelineMessagesCommand(Guid JobId, PipelineStage? Stage = null)
    : IRequest<RequeueMissingPipelineMessagesResultDto>;

public record RequeueMissingPipelineMessagesResultDto(
    int TotalPublished,
    IReadOnlyDictionary<string, int> PublishedByStage);

public sealed class RequeueMissingPipelineMessagesHandler
    : IRequestHandler<RequeueMissingPipelineMessagesCommand, RequeueMissingPipelineMessagesResultDto>
{
    private static readonly PipelineStage[] Stages =
    [
        PipelineStage.Fetcher,
        PipelineStage.Parser,
        PipelineStage.Enricher,
        PipelineStage.Persister,
        PipelineStage.Indexer,
    ];

    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public RequeueMissingPipelineMessagesHandler(
        IIngestionJobRepository jobs,
        IDocumentRepository documents,
        IQueuePublisher publisher,
        PipelineQueueNames queueNames)
    {
        _jobs = jobs;
        _documents = documents;
        _publisher = publisher;
        _queueNames = queueNames;
    }

    public async Task<RequeueMissingPipelineMessagesResultDto> Handle(
        RequeueMissingPipelineMessagesCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var stages = request.Stage.HasValue ? new[] { request.Stage.Value } : Stages;
        var byStage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var total = 0;

        foreach (var stage in stages)
        {
            var published = await PublishPendingForStageAsync(request.JobId, stage, cancellationToken);
            byStage[stage.ToString()] = published;
            total += published;
        }

        return new RequeueMissingPipelineMessagesResultDto(total, byStage);
    }

    private async Task<int> PublishPendingForStageAsync(
        Guid jobId,
        PipelineStage stage,
        CancellationToken cancellationToken)
    {
        var published = 0;
        var skip = 0;
        const int take = 200;
        while (true)
        {
            var batch = await _documents.GetByJobAsync(jobId, stage, DocumentStatus.Pending, skip, take, cancellationToken);
            if (batch.Count == 0)
                break;

            foreach (var doc in batch)
            {
                if (await DocumentStageQueuePublisher.TryPublishForStageAsync(
                        doc,
                        stage,
                        _publisher,
                        _queueNames,
                        cancellationToken))
                    published++;
            }

            skip += batch.Count;
        }

        return published;
    }
}
