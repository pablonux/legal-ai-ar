using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class DocumentStageLogRepository : IDocumentStageLogRepository
{
    private readonly AppDbContext _context;

    public DocumentStageLogRepository(AppDbContext context) => _context = context;

    public async Task LogStageAsync(DocumentStageLog log, CancellationToken ct = default)
    {
        _context.Set<DocumentStageLog>().Add(log);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DocumentStageLog>> GetByDocumentIdAsync(Guid documentId, CancellationToken ct = default)
    {
        return await _context.Set<DocumentStageLog>()
            .Where(l => l.DocumentId == documentId)
            .OrderBy(l => l.Stage)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<Guid, string?>> GetLastErrorWorkerInstanceByDocumentIdsAsync(
        IReadOnlyCollection<Guid> documentIds,
        CancellationToken ct = default)
    {
        if (documentIds.Count == 0)
            return new Dictionary<Guid, string?>();

        var logs = await _context.Set<DocumentStageLog>()
            .Where(l => documentIds.Contains(l.DocumentId) && l.ErrorMessage != null && l.ErrorMessage != "")
            .Select(l => new { l.DocumentId, l.CompletedAt, l.WorkerInstanceId })
            .ToListAsync(ct);

        return logs
            .GroupBy(x => x.DocumentId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(x => x.CompletedAt).First().WorkerInstanceId);
    }

    public async Task<StageMetrics> GetStageMetricsAsync(Guid jobId, PipelineStage stage, CancellationToken ct = default)
    {
        var durations = await _context.Set<DocumentStageLog>()
            .Where(l => l.Stage == stage)
            .Where(l => _context.Set<Document>()
                .Where(d => d.IngestionJobId == jobId)
                .Select(d => d.Id)
                .Contains(l.DocumentId))
            .Select(l => l.DurationMs)
            .OrderBy(d => d)
            .ToListAsync(ct);

        if (durations.Count == 0)
            return new StageMetrics(0, 0, 0, 0, 0, 0);

        var count = durations.Count;
        var avg = durations.Average();
        var p50 = Percentile(durations, 0.50);
        var p95 = Percentile(durations, 0.95);
        var max = durations[^1];
        var min = durations[0];

        return new StageMetrics(count, avg, p50, p95, max, min);
    }

    public async Task<double> GetStageElapsedAsync(Guid jobId, PipelineStage stage, CancellationToken ct = default)
    {
        var times = await _context.Set<DocumentStageLog>()
            .Where(l => l.Stage == stage)
            .Where(l => _context.Set<Document>()
                .Where(d => d.IngestionJobId == jobId)
                .Select(d => d.Id)
                .Contains(l.DocumentId))
            .GroupBy(_ => 1)
            .Select(g => new
            {
                First = g.Min(l => l.StartedAt),
                Last = g.Max(l => l.CompletedAt)
            })
            .FirstOrDefaultAsync(ct);

        if (times is null) return 0;
        return (times.Last - times.First).TotalMilliseconds;
    }

    private static double Percentile(List<int> sorted, double percentile)
    {
        var index = (int)Math.Ceiling(percentile * sorted.Count) - 1;
        return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
    }
}
