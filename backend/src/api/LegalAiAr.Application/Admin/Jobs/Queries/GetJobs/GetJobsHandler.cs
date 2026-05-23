using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobs;

/// <summary>
/// Handles GetJobsQuery. Returns real data from IngestionJobs when available;
/// falls back to synthetic jobs from CrawlerConfigs when IngestionJobs table is empty.
/// </summary>
public class GetJobsHandler : IRequestHandler<GetJobsQuery, IReadOnlyList<JobDto>>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly ICrawlerConfigRepository _configs;
    private readonly IDocumentRepository _documents;

    public GetJobsHandler(
        IIngestionJobRepository jobs,
        ICrawlerConfigRepository configs,
        IDocumentRepository documents)
    {
        _jobs = jobs;
        _configs = configs;
        _documents = documents;
    }

    public async Task<IReadOnlyList<JobDto>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        var ingestionJobs = await _jobs.GetAllAsync(limit: 100, cancellationToken);

        if (ingestionJobs.Count > 0)
        {
            var outstanding = await _documents.CountOutstandingDocumentsByJobIdsAsync(
                ingestionJobs.Select(j => j.Id).ToList(),
                cancellationToken);

            return ingestionJobs
                .Select(j => new JobDto(
                    Id: j.Id.ToString(),
                    SourceId: j.SourceId,
                    SourceName: j.Source.Name,
                    DocumentType: j.EntityType.ToString(),
                    Type: j.Type,
                    TriggeredBy: j.TriggeredBy,
                    StartedAt: j.StartedAt,
                    CompletedAt: j.CompletedAt,
                    Status: j.Status,
                    TotalSearchResults: j.TotalSearchResults,
                    DocumentsDiscovered: j.DocumentsDiscovered,
                    DocumentsCrawled: j.DocumentsCrawled,
                    DocumentsParsed: j.DocumentsParsed,
                    DocumentsEnriched: j.DocumentsEnriched,
                    DocumentsPersisted: j.DocumentsPersisted,
                    DocumentsIndexed: j.DocumentsIndexed,
                    DocumentsSkipped: j.DocumentsSkipped,
                    DocumentsFailed: j.DocumentsFailed,
                    OutstandingDocuments: outstanding.TryGetValue(j.Id, out var o) ? o : 0,
                    ErrorSummary: j.ErrorSummary,
                    CreationLog: j.CreationLog,
                    DateFrom: j.DateFrom?.ToString("yyyy-MM-dd"),
                    DateTo: j.DateTo?.ToString("yyyy-MM-dd"),
                    ParentJobId: j.ParentJobId?.ToString(),
                    PartitionIndex: j.PartitionIndex,
                    PartitionTotal: j.PartitionTotal,
                    InfrastructureDegraded: j.InfrastructureDegraded,
                    DegradedSinceUtc: j.DegradedSinceUtc,
                    DegradedReason: j.DegradedReason,
                    DiscoveryBatchPublished: j.DiscoveryBatchPublished))
                .ToList();
        }

        var configs = await _configs.GetAllAsync(cancellationToken);
        return configs
            .Where(c => c.LastCrawledAt.HasValue)
            .Select(c => new JobDto(
                Id: $"synthetic-{c.SourceId}",
                SourceId: c.SourceId,
                SourceName: c.Source.Name,
                DocumentType: "ruling",
                Type: "incremental",
                TriggeredBy: "admin",
                StartedAt: EstimateStartedAt(c.LastCrawledAt!.Value),
                CompletedAt: c.LastCrawledAt,
                Status: c.LastCrawledStatus ?? "unknown",
                TotalSearchResults: null,
                DocumentsDiscovered: c.LastDocumentCount ?? 0,
                DocumentsCrawled: c.LastDocumentCount ?? 0,
                DocumentsParsed: c.LastDocumentCount ?? 0,
                DocumentsEnriched: c.LastDocumentCount ?? 0,
                DocumentsPersisted: c.LastDocumentCount ?? 0,
                DocumentsIndexed: c.LastDocumentCount ?? 0,
                DocumentsSkipped: 0,
                DocumentsFailed: 0,
                OutstandingDocuments: 0,
                ErrorSummary: null,
                CreationLog: "synthetic"))
            .OrderByDescending(j => j.CompletedAt)
            .ToList();
    }

    private static DateTime EstimateStartedAt(DateTime completedAt)
    {
        return completedAt.AddMinutes(-30);
    }
}
