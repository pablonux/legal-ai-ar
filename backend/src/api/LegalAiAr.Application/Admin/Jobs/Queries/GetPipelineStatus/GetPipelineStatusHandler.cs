using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetPipelineStatus;

public class GetPipelineStatusHandler : IRequestHandler<GetPipelineStatusQuery, GetPipelineStatusResult>
{
    private readonly ICrawlerConfigRepository _configs;
    private readonly IQueueMetricsService _queueMetrics;
    private readonly PipelineQueueNames _queueNames;

    public GetPipelineStatusHandler(ICrawlerConfigRepository configs, IQueueMetricsService queueMetrics, PipelineQueueNames queueNames)
    {
        _configs = configs;
        _queueMetrics = queueMetrics;
        _queueNames = queueNames;
    }

    public async Task<GetPipelineStatusResult> Handle(GetPipelineStatusQuery request, CancellationToken cancellationToken)
    {
        var configs = await _configs.GetAllAsync(cancellationToken);

        var sources = new List<PipelineSourceStatusDto>();
        foreach (var c in configs)
        {
            var queueLength = await _queueMetrics.GetApproximateMessageCountAsync(_queueNames.Discoverer, cancellationToken);

            sources.Add(new PipelineSourceStatusDto(
                SourceId: c.SourceId,
                SourceName: c.Source.Name,
                LastCrawledAt: c.LastCrawledAt,
                LastCrawledStatus: c.LastCrawledStatus,
                LastDocumentCount: c.LastDocumentCount,
                QueueLength: queueLength));
        }

        return new GetPipelineStatusResult(sources);
    }

}
