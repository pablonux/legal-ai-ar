using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobMetrics;

public record GetJobMetricsQuery(Guid JobId) : IRequest<JobMetricsDto>;

public record JobMetricsDto(
    Guid JobId,
    IReadOnlyDictionary<string, StageMetricsDto> Stages,
    double? OverallDocsPerMinute,
    double? TotalElapsedMs);

public record StageMetricsDto(
    int Count,
    double AvgDurationMs,
    double P50DurationMs,
    double P95DurationMs,
    double MaxDurationMs,
    double MinDurationMs,
    double? DocsPerMinute);
