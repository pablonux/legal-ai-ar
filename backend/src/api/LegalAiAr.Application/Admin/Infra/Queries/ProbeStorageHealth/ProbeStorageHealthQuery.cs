using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Infra.Queries.ProbeStorageHealth;

public record ProbeStorageHealthQuery : IRequest<ProbeStorageHealthResultDto>;

public record ProbeStorageHealthResultDto(
    bool DiscovererOk,
    bool FetcherOk,
    string? DiscovererError,
    string? FetcherError);

public sealed class ProbeStorageHealthHandler : IRequestHandler<ProbeStorageHealthQuery, ProbeStorageHealthResultDto>
{
    private readonly IQueueMetricsService _queueMetrics;
    private readonly PipelineQueueNames _queueNames;

    public ProbeStorageHealthHandler(IQueueMetricsService queueMetrics, PipelineQueueNames queueNames)
    {
        _queueMetrics = queueMetrics;
        _queueNames = queueNames;
    }

    public async Task<ProbeStorageHealthResultDto> Handle(
        ProbeStorageHealthQuery request,
        CancellationToken cancellationToken)
    {
        var d = await _queueMetrics.TryProbeQueueAsync(_queueNames.Discoverer, cancellationToken);
        var f = await _queueMetrics.TryProbeQueueAsync(_queueNames.Fetcher, cancellationToken);
        return new ProbeStorageHealthResultDto(d.Ok, f.Ok, d.Error, f.Error);
    }
}
