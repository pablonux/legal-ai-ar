using System.Collections.Concurrent;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Thesaurus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Caching;

public class EntityCacheService
{
    private readonly ILogger<EntityCacheService> _logger;
    private volatile bool _isWarmedUp;

    private readonly ConcurrentDictionary<int, Keyword> _keywordByExternalCode = new();
    private readonly ConcurrentDictionary<string, Keyword> _keywordByDescription = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Statute> _statuteByNumber = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Court> _courtByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<int, Person> _personByCsjnMinistroId = new();
    private readonly ConcurrentDictionary<string, Person> _personByNameComposite = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>Keys are <see cref="ThesaurusLabelNormalizer.NormalizeForLookup"/> of preferred labels.</summary>
    private readonly ConcurrentDictionary<string, ThesaurusTerm> _thesaurusPreferredByLabel = new(StringComparer.Ordinal);
    /// <summary>UF synonym label (normalized) → preferred term id.</summary>
    private readonly ConcurrentDictionary<string, int> _thesaurusPreferredIdBySynonymLabel = new(StringComparer.Ordinal);

    public EntityCacheService(ILogger<EntityCacheService> logger)
    {
        _logger = logger;
    }

    public bool IsWarmedUp => _isWarmedUp;

    public bool TryGetKeywordByExternalCode(int externalCode, out Keyword? keyword) =>
        _keywordByExternalCode.TryGetValue(externalCode, out keyword);

    public bool TryGetKeywordByDescription(string description, out Keyword? keyword) =>
        _keywordByDescription.TryGetValue(description, out keyword);

    public void SetKeyword(Keyword keyword)
    {
        ArgumentNullException.ThrowIfNull(keyword);
        if (keyword.ExternalCode is { } ec)
        {
            _keywordByExternalCode[ec] = keyword;
        }

        _keywordByDescription[keyword.Description] = keyword;
    }

    public bool TryGetStatuteByNumber(string number, out Statute? statute) =>
        _statuteByNumber.TryGetValue(number, out statute);

    public void SetStatute(Statute statute)
    {
        ArgumentNullException.ThrowIfNull(statute);
        _statuteByNumber[statute.Number] = statute;
    }

    public bool TryGetCourtByName(string name, out Court? court) =>
        _courtByName.TryGetValue(name, out court);

    public void SetCourt(Court court)
    {
        ArgumentNullException.ThrowIfNull(court);
        _courtByName[court.Name] = court;
    }

    public bool TryGetPersonByCsjnMinistroId(int csjnMinistroId, out Person? person) =>
        _personByCsjnMinistroId.TryGetValue(csjnMinistroId, out person);

    public bool TryGetPersonByNameComposite(string compositeKey, out Person? person)
    {
        ArgumentNullException.ThrowIfNull(compositeKey);
        return _personByNameComposite.TryGetValue(NormalizePersonCompositeKey(compositeKey), out person);
    }

    public bool TryGetPersonByNameComposite(string? firstName, string? lastName, out Person? person) =>
        _personByNameComposite.TryGetValue(BuildPersonNameCompositeKey(firstName, lastName), out person);

    public void SetPerson(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        var compositeKey = BuildPersonNameCompositeKey(person.FirstName, person.LastName);
        _personByNameComposite[compositeKey] = person;

        if (person is { IsVerified: true, CsjnMinistroId: { } mid })
        {
            _personByCsjnMinistroId[mid] = person;
        }
    }

    public bool TryGetThesaurusPreferredByLabel(string label, out ThesaurusTerm? term)
    {
        term = null;
        if (string.IsNullOrWhiteSpace(label))
            return false;

        var key = ThesaurusLabelNormalizer.NormalizeForLookup(label);
        return key.Length > 0 && _thesaurusPreferredByLabel.TryGetValue(key, out term);
    }

    /// <summary>
    /// When the cache is warmed, attempts to resolve a label to a preferred thesaurus term id using
    /// in-memory preferred labels and UF synonym links. Returns false when warm-up has not run or
    /// the label is not present in the snapshot (caller should query the database).
    /// </summary>
    public bool TryResolvePreferredThesaurusTermId(string label, out int preferredTermId)
    {
        preferredTermId = default;
        if (!_isWarmedUp || string.IsNullOrWhiteSpace(label))
            return false;

        var key = ThesaurusLabelNormalizer.NormalizeForLookup(label);
        if (key.Length == 0)
            return false;

        if (_thesaurusPreferredByLabel.TryGetValue(key, out var preferred))
        {
            preferredTermId = preferred!.Id;
            return true;
        }

        return _thesaurusPreferredIdBySynonymLabel.TryGetValue(key, out preferredTermId);
    }

    public void SetThesaurusTerm(ThesaurusTerm term)
    {
        ArgumentNullException.ThrowIfNull(term);
        if (!term.IsPreferred)
            return;

        var key = ThesaurusLabelNormalizer.NormalizeForLookup(term.Label);
        if (key.Length == 0)
            return;

        _thesaurusPreferredByLabel[key] = term;
    }

    public async Task WarmUpAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _isWarmedUp = false;
        ClearCaches();

        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var keywords = await db.Keywords.AsNoTracking().ToListAsync(ct);
        foreach (var e in keywords)
        {
            SetKeyword(e);
        }

        var statutes = await db.Statutes.AsNoTracking().ToListAsync(ct);
        foreach (var e in statutes)
        {
            SetStatute(e);
        }

        var courts = await db.Courts.AsNoTracking().ToListAsync(ct);
        foreach (var e in courts)
        {
            SetCourt(e);
        }

        var persons = await db.Persons.AsNoTracking().ToListAsync(ct);
        foreach (var e in persons)
        {
            SetPerson(e);
        }

        var thesaurusTerms = await db.ThesaurusTerms.AsNoTracking().ToListAsync(ct);
        foreach (var e in thesaurusTerms)
        {
            SetThesaurusTerm(e);
        }

        var ufSynonymLinks = await db.ThesaurusRelations
            .AsNoTracking()
            .Where(r => r.RelationType == ThesaurusRelationType.UF)
            .Select(r => new { r.SourceTermId, SynonymLabel = r.TargetTerm.Label })
            .ToListAsync(ct);

        foreach (var link in ufSynonymLinks)
        {
            var synKey = ThesaurusLabelNormalizer.NormalizeForLookup(link.SynonymLabel);
            if (synKey.Length == 0)
                continue;
            _thesaurusPreferredIdBySynonymLabel[synKey] = link.SourceTermId;
        }

        _isWarmedUp = true;
        _logger.LogInformation(
            "Entity cache warm-up finished: loaded {KeywordCount} keywords, {StatuteCount} statutes, {CourtCount} courts, {PersonCount} persons, {ThesaurusTermCount} thesaurus terms, {ThesaurusUfLinkCount} UF synonym links.",
            keywords.Count,
            statutes.Count,
            courts.Count,
            persons.Count,
            thesaurusTerms.Count,
            ufSynonymLinks.Count);
    }

    private void ClearCaches()
    {
        _keywordByExternalCode.Clear();
        _keywordByDescription.Clear();
        _statuteByNumber.Clear();
        _courtByName.Clear();
        _personByCsjnMinistroId.Clear();
        _personByNameComposite.Clear();
        _thesaurusPreferredByLabel.Clear();
        _thesaurusPreferredIdBySynonymLabel.Clear();
    }

    public static string BuildPersonNameCompositeKey(string? firstName, string? lastName) =>
        $"{firstName?.Trim() ?? string.Empty}|{lastName?.Trim() ?? string.Empty}";

    private static string NormalizePersonCompositeKey(string compositeKey)
    {
        var span = compositeKey.AsSpan().Trim();
        var idx = span.IndexOf('|');
        if (idx < 0)
        {
            return $"{span.ToString().Trim()}|";
        }

        var first = span[..idx].Trim();
        var last = span[(idx + 1)..].Trim();
        return $"{first}|{last}";
    }
}
