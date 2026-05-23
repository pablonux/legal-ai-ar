using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class IngestionJobRepository : IIngestionJobRepository
{
    private readonly AppDbContext _context;

    public IngestionJobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IngestionJob> AddAsync(IngestionJob job, CancellationToken cancellationToken = default)
    {
        _context.IngestionJobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task<IngestionJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.IngestionJobs
            .Include(j => j.Source)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<IngestionJob>> GetAllAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.IngestionJobs
            .Include(j => j.Source)
            .OrderByDescending(j => j.StartedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task IncrementDocumentsDiscoveredAsync(Guid jobId, int count, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsDiscovered = DocumentsDiscovered + {0} WHERE Id = {1}",
            new object[] { count, jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsCrawledAsync(Guid jobId, int count = 1, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsCrawled = DocumentsCrawled + {0} WHERE Id = {1}",
            new object[] { count, jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsParsedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsParsed = DocumentsParsed + 1 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsEnrichedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsEnriched = DocumentsEnriched + 1 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsPersistedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsPersisted = DocumentsPersisted + 1 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsIndexedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsIndexed = DocumentsIndexed + 1 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task IncrementDocumentsFailedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsFailed = DocumentsFailed + 1 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task DecrementDocumentsFailedAsync(Guid jobId, int amount, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
            return;

        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE IngestionJobs
            SET DocumentsFailed = CASE WHEN DocumentsFailed >= {0} THEN DocumentsFailed - {0} ELSE 0 END
            WHERE Id = {1}
            """,
            [amount, jobId],
            cancellationToken);
    }

    public async Task SetDocumentsSkippedAsync(Guid jobId, int count, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsSkipped = {0} WHERE Id = {1}",
            new object[] { count, jobId },
            cancellationToken);
    }

    public async Task SetTotalSearchResultsAsync(Guid jobId, int total, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET TotalSearchResults = {0} WHERE Id = {1}",
            new object[] { total, jobId },
            cancellationToken);
    }

    public async Task ResetDocumentsDiscoveredAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DocumentsDiscovered = 0 WHERE Id = {0}",
            new object[] { jobId },
            cancellationToken);
    }

    public async Task UpdateCompletionAsync(Guid jobId, int documentsDiscovered, string status, string? errorSummary, CancellationToken cancellationToken = default)
    {
        var job = await _context.IngestionJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
            return;

        // documentsDiscovered: used when job is cancelled/failed before any page fetch (overwrite). Otherwise leave as-is (incremental updates).
        if (documentsDiscovered >= 0 && status is "cancelled" or "failed")
            job.DocumentsDiscovered = documentsDiscovered;
        job.CompletedAt = DateTime.UtcNow;
        job.Status = status;
        job.ErrorSummary = errorSummary;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetDiscoveryBatchPublishedAsync(Guid jobId, bool published, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE IngestionJobs SET DiscoveryBatchPublished = {0} WHERE Id = {1}",
            new object[] { published, jobId },
            cancellationToken);
    }

    public async Task SetInfrastructureDegradedAsync(
        Guid jobId,
        bool degraded,
        DateTime? degradedSinceUtc,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE IngestionJobs
            SET InfrastructureDegraded = {degraded},
                DegradedSinceUtc = {degradedSinceUtc},
                DegradedReason = {reason}
            WHERE Id = {jobId}
            """,
            cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid jobId, string status, CancellationToken cancellationToken = default)
    {
        var job = await _context.IngestionJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
            return;
        job.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResumeProcessingIfTerminalAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE IngestionJobs
            SET Status = {0}, CompletedAt = NULL
            WHERE Id = {1} AND Status IN ({2}, {3}, {4})
            """,
            ["processing", jobId, "success", "completed", "partial"],
            cancellationToken);
    }

    private static readonly string[] ActiveStatuses = ["running", "pending", "partial", "processing", "discovered"];

    public async Task<bool> HasActiveJobAsync(EntityType entityType, int sourceId, CancellationToken cancellationToken = default)
    {
        return await _context.IngestionJobs
            .AnyAsync(j => j.EntityType == entityType
                && j.SourceId == sourceId
                && ActiveStatuses.Contains(j.Status), cancellationToken);
    }

    public async Task<bool> HasActiveJobOtherThanAsync(
        EntityType entityType,
        int sourceId,
        Guid excludeJobId,
        CancellationToken cancellationToken = default)
    {
        return await _context.IngestionJobs
            .AnyAsync(j => j.EntityType == entityType
                && j.SourceId == sourceId
                && j.Id != excludeJobId
                && ActiveStatuses.Contains(j.Status), cancellationToken);
    }

    public async Task<IReadOnlyList<IngestionJob>> GetByEntityAndSourceAsync(
        EntityType? entityType = null,
        int? sourceId = null,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.IngestionJobs.Include(j => j.Source).AsQueryable();

        if (entityType.HasValue)
            query = query.Where(j => j.EntityType == entityType.Value);
        if (sourceId.HasValue)
            query = query.Where(j => j.SourceId == sourceId.Value);

        return await query
            .OrderByDescending(j => j.StartedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CancelActiveJobsAsync(EntityType entityType, int sourceId, CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE IngestionJobs
            SET Status = 'cancelled', CompletedAt = {0}
            WHERE EntityType = {1} AND SourceId = {2} AND Status IN ('running', 'pending', 'partial')
            """,
            [DateTime.UtcNow, entityType.ToString(), sourceId],
            cancellationToken);
    }

    public async Task<bool> TryCompleteIfDoneAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _context.IngestionJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null || job.Status != "processing")
            return false;

        var hasOutstanding = await _context.Documents.AnyAsync(
            d => d.IngestionJobId == jobId
                && (d.Status == DocumentStatus.Pending || d.Status == DocumentStatus.Processing),
            cancellationToken);
        if (hasOutstanding)
            return false;

        // Counters may have been updated via raw SQL (e.g. reconcile); read fresh values from the store.
        var snap = await _context.IngestionJobs.AsNoTracking()
            .Where(j => j.Id == jobId)
            .Select(j => new
            {
                j.DocumentsDiscovered,
                j.DocumentsSkipped,
                j.DocumentsIndexed,
                j.DocumentsFailed,
            })
            .FirstAsync(cancellationToken);

        var expected = snap.DocumentsDiscovered - snap.DocumentsSkipped;
        if (expected <= 0 || snap.DocumentsIndexed + snap.DocumentsFailed < expected)
            return false;

        job.Status = "success";
        job.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IngestionPipelineCounters?> ReconcilePipelineCountersFromDocumentsAsync(
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.IngestionJobs.AnyAsync(j => j.Id == jobId, cancellationToken);
        if (!exists)
            return null;

        await _context.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE IngestionJobs
            SET
                DocumentsCrawled = (
                    SELECT COUNT(*)
                    FROM Documents
                    WHERE IngestionJobId = {jobId}
                      AND (
                        Status = 'Completed'
                        OR (
                            Status NOT IN ('Discarded', 'Cancelled')
                            AND (
                                CurrentStage IN ('Parser', 'Enricher', 'Persister', 'Indexer')
                                OR (Status = 'Failed' AND CurrentStage IN ('Parser', 'Enricher', 'Persister', 'Indexer'))
                            )
                        )
                    )
                ),
                DocumentsParsed = (
                    SELECT COUNT(*)
                    FROM Documents
                    WHERE IngestionJobId = {jobId}
                      AND (
                        Status = 'Completed'
                        OR (
                            Status NOT IN ('Discarded', 'Cancelled')
                            AND (
                                CurrentStage IN ('Enricher', 'Persister', 'Indexer')
                                OR (Status = 'Failed' AND CurrentStage IN ('Enricher', 'Persister', 'Indexer'))
                            )
                        )
                    )
                ),
                DocumentsEnriched = (
                    SELECT COUNT(*)
                    FROM Documents
                    WHERE IngestionJobId = {jobId}
                      AND (
                        Status = 'Completed'
                        OR (
                            Status NOT IN ('Discarded', 'Cancelled')
                            AND (
                                CurrentStage IN ('Persister', 'Indexer')
                                OR (Status = 'Failed' AND CurrentStage IN ('Persister', 'Indexer'))
                            )
                        )
                    )
                ),
                DocumentsPersisted = (
                    SELECT COUNT(*)
                    FROM Documents
                    WHERE IngestionJobId = {jobId}
                      AND (
                        Status = 'Completed'
                        OR (
                            Status NOT IN ('Discarded', 'Cancelled')
                            AND (
                                CurrentStage = 'Indexer'
                                OR (Status = 'Failed' AND CurrentStage = 'Indexer')
                            )
                        )
                    )
                ),
                DocumentsIndexed = (
                    SELECT COUNT(*) FROM Documents WHERE IngestionJobId = {jobId} AND Status = 'Completed'
                ),
                DocumentsFailed = (
                    SELECT COUNT(*) FROM Documents WHERE IngestionJobId = {jobId} AND Status = 'Failed'
                )
            WHERE Id = {jobId}
            """,
            cancellationToken);

        return await _context.IngestionJobs.AsNoTracking()
            .Where(j => j.Id == jobId)
            .Select(j => new IngestionPipelineCounters(
                j.DocumentsCrawled,
                j.DocumentsParsed,
                j.DocumentsEnriched,
                j.DocumentsPersisted,
                j.DocumentsIndexed,
                j.DocumentsFailed))
            .FirstAsync(cancellationToken);
    }

    public async Task FinalizeThesaurusIngestJobAsync(
        Guid jobId,
        bool success,
        string? errorSummary,
        CancellationToken cancellationToken = default)
    {
        var job = await _context.IngestionJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
            return;

        job.CompletedAt = DateTime.UtcNow;
        job.Status = success ? "success" : "failed";
        job.ErrorSummary = errorSummary;

        if (success)
        {
            job.DocumentsDiscovered = 1;
            job.DocumentsSkipped = 0;
            job.DocumentsCrawled = 1;
            job.DocumentsParsed = 1;
            job.DocumentsEnriched = 1;
            job.DocumentsPersisted = 1;
            job.DocumentsIndexed = 1;
            job.DocumentsFailed = 0;
        }
        else
        {
            if (job.DocumentsDiscovered < 1)
                job.DocumentsDiscovered = 1;
            job.DocumentsFailed = Math.Max(1, job.DocumentsFailed);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
