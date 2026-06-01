using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class PersonRepository : IPersonRepository
{
    /// <summary>Must match EF configuration for <c>Person</c> — SQL Server rejects longer values (Persister payloads may carry full case titles as party names).</summary>
    private const int MaxDisplayNameLength = 400;
    private const int MaxFirstNameLength = 200;
    private const int MaxLastNameLength = 200;

    private readonly AppDbContext _context;
    private readonly EntityCacheService _cache;

    public PersonRepository(AppDbContext context, EntityCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Person> GetOrCreateAsync(string displayName, string? firstName = null, string? lastName = null, int? csjnMinistroId = null, CancellationToken cancellationToken = default)
    {
        displayName = NormalizeDisplayName(displayName);
        firstName = Truncate(firstName, MaxFirstNameLength);
        lastName = Truncate(lastName, MaxLastNameLength);

        if (_cache.IsWarmedUp)
        {
            Person? cached = null;
            if (csjnMinistroId.HasValue)
                _cache.TryGetPersonByCsjnMinistroId(csjnMinistroId.Value, out cached);

            if (cached == null && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                _cache.TryGetPersonByNameComposite(firstName, lastName, out cached);

            if (cached is { Id: > 0 })
            {
                var person = _context.Persons.Find(cached.Id);
                if (person != null)
                {
                    if (string.IsNullOrEmpty(person.FirstName) && !string.IsNullOrEmpty(firstName))
                    {
                        person.FirstName = firstName;
                        person.LastName = lastName;
                        person.DisplayName = displayName;
                    }
                    if (csjnMinistroId.HasValue && !person.CsjnMinistroId.HasValue)
                        person.CsjnMinistroId = csjnMinistroId;
                    return person;
                }
            }
        }

        if (csjnMinistroId.HasValue)
        {
            var byMinistroId = await _context.Persons
                .FirstOrDefaultAsync(p => p.CsjnMinistroId == csjnMinistroId, cancellationToken);
            if (byMinistroId != null)
            {
                if (string.IsNullOrEmpty(byMinistroId.FirstName) && !string.IsNullOrEmpty(firstName))
                {
                    byMinistroId.FirstName = firstName;
                    byMinistroId.LastName = lastName;
                    byMinistroId.DisplayName = displayName;
                }
                _cache.SetPerson(byMinistroId);
                return byMinistroId;
            }
        }

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
        {
            var byName = await _context.Persons
                .FirstOrDefaultAsync(p => p.FirstName == firstName && p.LastName == lastName, cancellationToken);
            if (byName != null)
            {
                if (csjnMinistroId.HasValue && !byName.CsjnMinistroId.HasValue)
                    byName.CsjnMinistroId = csjnMinistroId;
                _cache.SetPerson(byName);
                return byName;
            }
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            var byLastName = await _context.Persons
                .Where(p => p.LastName == lastName && p.CsjnMinistroId != null)
                .FirstOrDefaultAsync(cancellationToken);
            if (byLastName != null)
            {
                _cache.SetPerson(byLastName);
                return byLastName;
            }
        }

        var verified = csjnMinistroId.HasValue
            && CsjnMinisterDictionary.Ministers.ContainsKey(csjnMinistroId.Value);

        var newPerson = new Person
        {
            DisplayName = displayName,
            FirstName = firstName,
            LastName = lastName,
            CsjnMinistroId = csjnMinistroId,
            PersonType = PersonType.Physical,
            IsVerified = verified
        };
        await _context.Persons.AddAsync(newPerson, cancellationToken);
        _cache.SetPerson(newPerson);
        return newPerson;
    }

    public async Task<IReadOnlyList<PersonWithCount>> ListWithRulingCountAsync(
        string? courtName = null,
        int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Persons.Where(p => p.RulingParticipations.Any());

        if (!string.IsNullOrWhiteSpace(courtName))
            q = q.Where(p => p.RulingParticipations.Any(rp =>
                rp.Ruling.Court != null && EF.Functions.Like(rp.Ruling.Court.Name, $"%{courtName}%")));

        return await q
            .OrderByDescending(p => p.RulingParticipations.Count)
            .Take(maxResults)
            .Select(p => new PersonWithCount(p.Id, p.FirstName ?? "", p.LastName ?? "", p.RulingParticipations.Count))
            .ToListAsync(cancellationToken);
    }

    private static readonly RulingRole[] MagistrateFormationRoles =
    [
        RulingRole.SIGNATORY,
        RulingRole.DISSENT,
        RulingRole.CONCURRENCE,
        RulingRole.MAJORITY_AUTHOR
    ];

    public async Task<IReadOnlyList<PersonListItem>> SearchAsync(
        string? query = null,
        string? courtName = null,
        int limit = 50,
        PersonListView listView = PersonListView.All,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Person> q = listView switch
        {
            PersonListView.Magistrates => _context.Persons.Where(p =>
                p.RulingParticipations.Any(rp => MagistrateFormationRoles.Contains(rp.Role))),
            PersonListView.Parties => _context.Persons.Where(p => p.ProceedingParties.Any()),
            _ => _context.Persons.Where(p => p.RulingParticipations.Any())
        };

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(p => EF.Functions.Like(p.DisplayName, $"%{query}%"));

        if (!string.IsNullOrWhiteSpace(courtName))
        {
            q = listView switch
            {
                PersonListView.Parties => q.Where(p => p.ProceedingParties.Any(pp =>
                    pp.JudicialProceeding.Court != null &&
                    EF.Functions.Like(pp.JudicialProceeding.Court.Name, $"%{courtName}%"))),
                _ => q.Where(p => p.RulingParticipations.Any(rp =>
                    rp.Ruling.Court != null && EF.Functions.Like(rp.Ruling.Court.Name, $"%{courtName}%")))
            };
        }

        return listView switch
        {
            PersonListView.Magistrates => await q
                .OrderByDescending(p => p.RulingParticipations.Count(rp => MagistrateFormationRoles.Contains(rp.Role)))
                .Take(limit)
                .Select(p => new PersonListItem(
                    p.Id, p.DisplayName,
                    p.JudicialOffices.Where(jo => jo.IsCurrent).Select(jo => jo.Court.Name).FirstOrDefault(),
                    p.RulingParticipations.Count(rp => MagistrateFormationRoles.Contains(rp.Role))))
                .ToListAsync(cancellationToken),
            PersonListView.Parties => await q
                .OrderByDescending(p => p.ProceedingParties.Count)
                .Take(limit)
                .Select(p => new PersonListItem(
                    p.Id, p.DisplayName,
                    p.JudicialOffices.Where(jo => jo.IsCurrent).Select(jo => jo.Court.Name).FirstOrDefault(),
                    p.ProceedingParties.Count))
                .ToListAsync(cancellationToken),
            _ => await q
                .OrderByDescending(p => p.RulingParticipations.Count)
                .Take(limit)
                .Select(p => new PersonListItem(
                    p.Id, p.DisplayName,
                    p.JudicialOffices.Where(jo => jo.IsCurrent).Select(jo => jo.Court.Name).FirstOrDefault(),
                    p.RulingParticipations.Count))
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<PersonDetail?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await _context.Persons
            .Include(p => p.JudicialOffices).ThenInclude(jo => jo.Court)
            .Include(p => p.ProceedingParties).ThenInclude(pp => pp.JudicialProceeding)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (person == null) return null;

        var currentCourtName = person.JudicialOffices
            .Where(jo => jo.IsCurrent)
            .Select(jo => jo.Court.Name)
            .FirstOrDefault();

        var rulingCount = await _context.RulingParticipations
            .CountAsync(rp => rp.PersonId == id, cancellationToken);

        var recentRulings = await _context.RulingParticipations
            .Where(rp => rp.PersonId == id)
            .OrderByDescending(rp => rp.Ruling.RulingDate)
            .Take(10)
            .Select(rp => new PersonRecentRuling(
                rp.Ruling.Id,
                rp.Ruling.CaseTitle,
                rp.Ruling.RulingDate,
                rp.Ruling.Instance,
                rp.Role.ToString()))
            .ToListAsync(cancellationToken);

        return new PersonDetail(person, currentCourtName, rulingCount, recentRulings);
    }

    public async Task<int> BackfillCurrentCourtIdAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync("""
            WITH latest AS (
                SELECT
                    rp.PersonId,
                    r.CourtId,
                    ROW_NUMBER() OVER (PARTITION BY rp.PersonId ORDER BY r.RulingDate DESC, r.Id DESC) AS rn
                FROM RulingParticipations rp
                INNER JOIN Rulings r ON r.Id = rp.RulingId
            )
            UPDATE jo
            SET jo.IsCurrent = CASE WHEN latest.CourtId = jo.CourtId THEN 1 ELSE 0 END
            FROM JudicialOffices jo
            INNER JOIN latest ON latest.PersonId = jo.PersonId AND latest.rn = 1
            """, cancellationToken);
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }

    private static string NormalizeDisplayName(string displayName)
    {
        var t = Truncate(displayName.Trim(), MaxDisplayNameLength);
        return string.IsNullOrEmpty(t) ? "\u2014" : t;
    }
}
