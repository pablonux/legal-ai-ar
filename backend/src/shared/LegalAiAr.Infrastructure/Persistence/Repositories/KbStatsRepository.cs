using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class KbStatsRepository : IKbStatsRepository
{
    private readonly AppDbContext _ctx;

    public KbStatsRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<KbStatsRaw> GetKbStatsAsync(CancellationToken ct = default)
    {
        var totalRulings = await _ctx.Rulings.CountAsync(ct);
        var totalCourts = await _ctx.Courts.CountAsync(c => c.Rulings.Any(), ct);
        var totalPersons = await _ctx.Persons.CountAsync(ct);
        var totalKeywords = await _ctx.Keywords.CountAsync(ct);
        var totalStatutes = await _ctx.Statutes.CountAsync(ct);
        var totalCitations = await _ctx.Citations.CountAsync(ct);

        var earliest = totalRulings > 0
            ? await _ctx.Rulings.MinAsync(r => (DateOnly?)r.RulingDate, ct)
            : null;
        var latest = totalRulings > 0
            ? await _ctx.Rulings.MaxAsync(r => (DateOnly?)r.RulingDate, ct)
            : null;

        // Group by FK IDs only (EF Core can translate these), then resolve names in memory
        var sourceCountsById = await _ctx.Rulings
            .GroupBy(r => r.SourceId)
            .Select(g => new { SourceId = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var sourceNames = await _ctx.Sources
            .Where(s => sourceCountsById.Select(x => x.SourceId).Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name, ct);
        var bySource = sourceCountsById
            .Select(x => new SourceCount(x.SourceId, sourceNames.GetValueOrDefault(x.SourceId, "?"), x.Count))
            .OrderByDescending(x => x.Count).ToList();

        var byYear = (await _ctx.Rulings
            .GroupBy(r => r.RulingDate.Year)
            .Select(g => new { Year = g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .Select(x => new YearCount(x.Year, x.Count))
            .OrderBy(x => x.Year).ToList();

        var courtCountsById = await _ctx.Rulings
            .GroupBy(r => r.CourtId)
            .Select(g => new { CourtId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(15)
            .ToListAsync(ct);
        var courtIds = courtCountsById.Select(x => x.CourtId).ToList();
        var courtLookup = await _ctx.Courts
            .Where(c => courtIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => new { c.Name, c.JurisdictionArea }, ct);
        var topCourts = courtCountsById
            .Select(x => new CourtCount(x.CourtId,
                courtLookup.GetValueOrDefault(x.CourtId)?.Name ?? "?",
                courtLookup.GetValueOrDefault(x.CourtId)?.JurisdictionArea ?? "?",
                x.Count))
            .ToList();

        var byJurisdiction = (await _ctx.Rulings
            .Where(r => r.JurisdictionArea != null)
            .GroupBy(r => r.JurisdictionArea!)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .Select(x => new NameCount(x.Name, x.Count))
            .OrderByDescending(x => x.Count).ToList();

        var byInstance = (await _ctx.Rulings
            .Where(r => r.Instance != null)
            .GroupBy(r => r.Instance!)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .Select(x => new NameCount(x.Name, x.Count))
            .OrderByDescending(x => x.Count).ToList();

        var bySubjectArea = (await _ctx.Rulings
            .Where(r => r.SubjectArea != null)
            .GroupBy(r => r.SubjectArea!)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .Select(x => new NameCount(x.Name, x.Count))
            .OrderByDescending(x => x.Count).Take(15).ToList();

        var keywordCountsById = await _ctx.RulingKeywords
            .GroupBy(rk => rk.KeywordId)
            .Select(g => new { KeywordId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(20)
            .ToListAsync(ct);
        var kwIds = keywordCountsById.Select(x => x.KeywordId).ToList();
        var kwNames = await _ctx.Keywords
            .Where(k => kwIds.Contains(k.Id))
            .ToDictionaryAsync(k => k.Id, k => k.Description, ct);
        var topKeywords = keywordCountsById
            .Select(x => new NameCount(kwNames.GetValueOrDefault(x.KeywordId, "?"), x.Count))
            .ToList();

        var withSummary = await _ctx.Rulings.CountAsync(r => r.Summary != null, ct);
        var withHolding = await _ctx.Rulings.CountAsync(r => r.Holding != null, ct);
        var withFullText = await _ctx.Rulings.CountAsync(r => r.FullText != null, ct);
        var withKeywords = await _ctx.Rulings.CountAsync(r => r.RulingKeywords.Any(), ct);
        var withPersons = await _ctx.Rulings.CountAsync(r => r.RulingParticipations.Any(), ct);
        var withStatutes = await _ctx.Rulings.CountAsync(r => r.RulingStatutes.Any(), ct);
        var withCitations = await _ctx.Rulings.CountAsync(r => r.OutboundCitations.Any(), ct);

        var quality = new QualityCounts(
            withSummary, withHolding, withFullText,
            withKeywords, withPersons, withStatutes, withCitations);

        return new KbStatsRaw(
            totalRulings, totalCourts, totalPersons, totalKeywords,
            totalStatutes, totalCitations, earliest, latest,
            bySource, byYear, topCourts, byJurisdiction,
            byInstance, bySubjectArea, topKeywords, quality);
    }
}
