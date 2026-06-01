using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Search;

internal class GlobalSearchService : IGlobalSearchService
{
    private readonly AppDbContext _db;

    public GlobalSearchService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<GlobalSearchItem>> SearchAsync(
        string query, int maxPerEntity = 5, CancellationToken cancellationToken = default)
    {
        var q = query.Trim();
        if (string.IsNullOrEmpty(q))
            return [];

        var pattern = $"%{q}%";

        var tasks = new[]
        {
            SearchRulingsAsync(pattern, maxPerEntity, cancellationToken),
            SearchCourtsAsync(pattern, maxPerEntity, cancellationToken),
            SearchPersonsAsync(pattern, maxPerEntity, cancellationToken),
            SearchStatutesAsync(pattern, maxPerEntity, cancellationToken),
            SearchProceedingsAsync(pattern, maxPerEntity, cancellationToken),
            SearchThesaurusAsync(pattern, maxPerEntity, cancellationToken),
        };

        await Task.WhenAll(tasks);
        return tasks.SelectMany(t => t.Result).ToList();
    }

    private async Task<List<GlobalSearchItem>> SearchRulingsAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.Rulings
            .Where(r => EF.Functions.Like(r.CaseTitle, pattern))
            .OrderByDescending(r => r.RulingDate)
            .Take(max)
            .Select(r => new GlobalSearchItem(
                "ruling", r.Id.ToString(), r.CaseTitle,
                r.Court.Name + " · " + r.RulingDate.ToString("dd/MM/yyyy"),
                "/jurisprudencia/" + r.Id))
            .ToListAsync(ct);
    }

    private async Task<List<GlobalSearchItem>> SearchCourtsAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.Courts
            .Where(c => EF.Functions.Like(c.Name, pattern))
            .OrderBy(c => c.Name)
            .Take(max)
            .Select(c => new GlobalSearchItem(
                "court", c.Id.ToString(), c.Name,
                c.JurisdictionArea + " · " + c.Instance,
                "/organismos/" + c.Id))
            .ToListAsync(ct);
    }

    private async Task<List<GlobalSearchItem>> SearchPersonsAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.Persons
            .Where(p => EF.Functions.Like(p.DisplayName, pattern))
            .OrderBy(p => p.DisplayName)
            .Take(max)
            .Select(p => new GlobalSearchItem(
                "person", p.Id.ToString(), p.DisplayName,
                p.PersonType.ToString(),
                "/sujetos/" + p.Id))
            .ToListAsync(ct);
    }

    private async Task<List<GlobalSearchItem>> SearchStatutesAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.Statutes
            .Where(s => EF.Functions.Like(s.Name, pattern) || EF.Functions.Like(s.Number, pattern))
            .OrderBy(s => s.Name)
            .Take(max)
            .Select(s => new GlobalSearchItem(
                "statute", s.Id.ToString(), s.Name,
                s.Number,
                "/ordenamiento/" + s.Id))
            .ToListAsync(ct);
    }

    private async Task<List<GlobalSearchItem>> SearchProceedingsAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.JudicialProceedings
            .Where(p => EF.Functions.Like(p.CaseNumber, pattern)
                     || (p.DisplayName != null && EF.Functions.Like(p.DisplayName, pattern)))
            .OrderByDescending(p => p.LastRulingDate)
            .Take(max)
            .Select(p => new GlobalSearchItem(
                "proceeding", p.Id.ToString(), p.DisplayName ?? p.CaseNumber,
                p.CaseNumber,
                "/procesos/" + p.Id))
            .ToListAsync(ct);
    }

    private async Task<List<GlobalSearchItem>> SearchThesaurusAsync(string pattern, int max, CancellationToken ct)
    {
        return await _db.ThesaurusTerms
            .Where(t => EF.Functions.Like(t.Label, pattern))
            .OrderBy(t => t.Label)
            .Take(max)
            .Select(t => new GlobalSearchItem(
                "thesaurus", t.Id.ToString(), t.Label,
                "Vocabulario jurídico",
                "/vocabulario/" + t.Id))
            .ToListAsync(ct);
    }
}
