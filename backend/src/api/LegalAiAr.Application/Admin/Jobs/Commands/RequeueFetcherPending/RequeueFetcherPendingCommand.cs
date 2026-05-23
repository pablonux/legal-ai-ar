using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RequeueFetcherPending;

public record RequeueFetcherPendingCommand(Guid JobId) : IRequest<RequeueFetcherPendingResultDto>;

public record RequeueFetcherPendingResultDto(int MessagesPublished);

public sealed class RequeueFetcherPendingHandler : IRequestHandler<RequeueFetcherPendingCommand, RequeueFetcherPendingResultDto>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public RequeueFetcherPendingHandler(
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

    public async Task<RequeueFetcherPendingResultDto> Handle(
        RequeueFetcherPendingCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var published = 0;
        var skip = 0;
        const int take = 200;
        while (true)
        {
            var batch = await _documents.GetByJobAsync(
                request.JobId,
                PipelineStage.Fetcher,
                DocumentStatus.Pending,
                skip,
                take,
                cancellationToken);
            if (batch.Count == 0)
                break;

            foreach (var doc in batch)
            {
                if (await DocumentStageQueuePublisher.TryPublishForStageAsync(
                        doc,
                        PipelineStage.Fetcher,
                        _publisher,
                        _queueNames,
                        cancellationToken))
                    published++;
            }

            skip += batch.Count;
        }

        return new RequeueFetcherPendingResultDto(published);
    }
}
