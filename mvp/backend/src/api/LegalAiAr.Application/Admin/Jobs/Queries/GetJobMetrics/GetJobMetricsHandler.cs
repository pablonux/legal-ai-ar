using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobMetrics;

public class GetJobMetricsHandler : IRequestHandler<GetJobMetricsQuery, JobMetricsDto>
{
    private readonly IDocumentStageLogRepository _stageLogRepo;
    private readonly IIngestionJobRepository _jobRepo;

    public GetJobMetricsHandler(
        IDocumentStageLogRepository stageLogRepo,
        IIngestionJobRepository jobRepo)
    {
        _stageLogRepo = stageLogRepo;
        _jobRepo = jobRepo;
    }

    public async Task<JobMetricsDto> Handle(GetJobMetricsQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepo.GetByIdAsync(request.JobId, cancellationToken);
        var stages = new Dictionary<string, StageMetricsDto>();

        foreach (var stage in Enum.GetValues<PipelineStage>())
        {
            var metrics = await _stageLogRepo.GetStageMetricsAsync(request.JobId, stage, cancellationToken);
            if (metrics.Count == 0) continue;

            var elapsed = await _stageLogRepo.GetStageElapsedAsync(request.JobId, stage, cancellationToken);
            var docsPerMinute = elapsed > 0 ? metrics.Count / (elapsed / 60_000.0) : (double?)null;

            stages[stage.ToString()] = new StageMetricsDto(
                metrics.Count,
                metrics.AvgDurationMs,
                metrics.P50DurationMs,
                metrics.P95DurationMs,
                metrics.MaxDurationMs,
                metrics.MinDurationMs,
                docsPerMinute);
        }

        double? overallDocsPerMinute = null;
        double? totalElapsedMs = null;

        if (job is not null)
        {
            var startedAt = job.StartedAt;
            var endedAt = job.CompletedAt ?? DateTime.UtcNow;
            totalElapsedMs = (endedAt - startedAt).TotalMilliseconds;

            if (totalElapsedMs > 0 && job.DocumentsIndexed > 0)
                overallDocsPerMinute = job.DocumentsIndexed / (totalElapsedMs.Value / 60_000.0);
        }

        return new JobMetricsDto(request.JobId, stages, overallDocsPerMinute, totalElapsedMs);
    }
}
