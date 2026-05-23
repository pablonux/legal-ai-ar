using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Graph;

/// <summary>
/// SQL-based graph service. Uses Azure SQL with recursive CTEs for multi-hop traversal.
/// Relationship data lives in Citations, RulingParticipations, RulingKeywords, RulingStatutes.
/// Upsert operations are no-ops (Indexer persists via repositories).
/// </summary>
public class SqlGraphService : IGraphService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SqlGraphService> _logger;

    public SqlGraphService(AppDbContext context, ILogger<SqlGraphService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Citation>> GetOutboundCitationsAsync(Guid rulingId, CancellationToken cancellationToken = default)
    {
        return await _context.Citations
            .Include(c => c.TargetRuling)
            .Where(c => c.SourceRulingId == rulingId)
            .OrderBy(c => c.ExternalAlias)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Citation>> GetInboundCitationsAsync(Guid rulingId, CancellationToken cancellationToken = default)
    {
        return await _context.Citations
            .Include(c => c.SourceRuling)
            .Where(c => c.TargetRulingId == rulingId)
            .OrderBy(c => c.ExternalAlias)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CitationChainNode>> GetCitationChainAsync(
        Guid rulingId, int maxDepth = 3, CancellationToken cancellationToken = default)
    {
        maxDepth = Math.Clamp(maxDepth, 1, 5);

        const string sql = """
            WITH CitationCTE AS (
                SELECT r.Id AS RulingId, r.CaseTitle, r.RulingDate, co.Name AS CourtName,
                       r.LegalBranch, CAST('ROOT' AS NVARCHAR(50)) AS CitationType,
                       0 AS Depth, CAST(NULL AS UNIQUEIDENTIFIER) AS ParentRulingId,
                       CAST(r.Id AS NVARCHAR(MAX)) AS Path
                FROM Rulings r
                LEFT JOIN Courts co ON co.Id = r.CourtId
                WHERE r.Id = @rulingId

                UNION ALL

                SELECT r2.Id, r2.CaseTitle, r2.RulingDate, co2.Name,
                       r2.LegalBranch, c.CitationType,
                       cte.Depth + 1, c.SourceRulingId,
                       cte.Path + ',' + CAST(r2.Id AS NVARCHAR(MAX))
                FROM CitationCTE cte
                JOIN Citations c ON c.SourceRulingId = cte.RulingId AND c.TargetRulingId IS NOT NULL
                JOIN Rulings r2 ON r2.Id = c.TargetRulingId
                LEFT JOIN Courts co2 ON co2.Id = r2.CourtId
                WHERE cte.Depth < @maxDepth
                  AND CHARINDEX(CAST(r2.Id AS NVARCHAR(MAX)), cte.Path) = 0
            )
            SELECT RulingId, CaseTitle, RulingDate, CourtName, LegalBranch, CitationType, Depth, ParentRulingId
            FROM CitationCTE
            ORDER BY Depth, CaseTitle
            """;

        var conn = _context.Database.GetDbConnection();
        await _context.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@rulingId", rulingId));
            cmd.Parameters.Add(new SqlParameter("@maxDepth", maxDepth));

            var results = new List<CitationChainNode>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new CitationChainNode(
                    RulingId: reader.GetGuid(0),
                    CaseTitle: reader.GetString(1),
                    RulingDate: DateOnly.FromDateTime(reader.GetDateTime(2)),
                    CourtName: reader.IsDBNull(3) ? null : reader.GetString(3),
                    LegalBranch: reader.IsDBNull(4) ? null : reader.GetString(4),
                    CitationType: reader.GetString(5),
                    Depth: reader.GetInt32(6),
                    ParentRulingId: reader.IsDBNull(7) ? null : reader.GetGuid(7)));
            }
            return results;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    public async Task<RulingGraphNeighborhood?> GetGraphNeighborhoodAsync(
        Guid rulingId, CancellationToken cancellationToken = default)
    {
        var ruling = await _context.Rulings
            .Include(r => r.Court)
            .Include(r => r.RulingParticipations).ThenInclude(rp => rp.Person)
            .Include(r => r.RulingKeywords).ThenInclude(rk => rk.Keyword)
            .Include(r => r.RulingStatutes).ThenInclude(rs => rs.Statute)
            .Include(r => r.OutboundCitations).ThenInclude(c => c.TargetRuling).ThenInclude(r => r!.Court)
            .Include(r => r.InboundCitations).ThenInclude(c => c.SourceRuling).ThenInclude(r => r.Court)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == rulingId, cancellationToken);

        if (ruling is null) return null;

        return new RulingGraphNeighborhood(
            CenterRulingId: ruling.Id,
            CaseTitle: ruling.CaseTitle,
            RulingDate: ruling.RulingDate,
            CourtName: ruling.Court?.Name,
            Persons: ruling.RulingParticipations
                .Select(rp => new NeighborhoodPerson(rp.PersonId, rp.Person.DisplayName, rp.Role.ToString()))
                .ToList(),
            Keywords: ruling.RulingKeywords
                .OrderBy(rk => rk.SortOrder)
                .Select(rk => new NeighborhoodKeyword(rk.KeywordId, rk.Keyword.Description))
                .ToList(),
            Statutes: ruling.RulingStatutes
                .Select(rs => new NeighborhoodStatute(rs.StatuteId, rs.Statute.Number, rs.Statute.Name, rs.Articles))
                .ToList(),
            OutboundCitations: ruling.OutboundCitations
                .Where(c => c.TargetRuling != null)
                .Select(c => new NeighborhoodCitation(c.TargetRulingId!.Value, c.TargetRuling!.CaseTitle, c.TargetRuling.RulingDate, c.TargetRuling.Court?.Name, c.CitationType.ToString()))
                .ToList(),
            InboundCitations: ruling.InboundCitations
                .Select(c => new NeighborhoodCitation(c.SourceRulingId, c.SourceRuling.CaseTitle, c.SourceRuling.RulingDate, c.SourceRuling.Court?.Name, c.CitationType.ToString()))
                .ToList());
    }

    public async Task<SharedEntitiesResult> GetSharedEntitiesAsync(
        IReadOnlyList<Guid> rulingIds, CancellationToken cancellationToken = default)
    {
        if (rulingIds.Count < 2)
            return new SharedEntitiesResult([], [], []);

        var idSet = rulingIds.ToHashSet();

        var persons = await _context.RulingParticipations
            .Include(rp => rp.Person)
            .Where(rp => idSet.Contains(rp.RulingId))
            .GroupBy(rp => new { rp.PersonId, rp.Person.DisplayName })
            .Where(g => g.Count() > 1)
            .Select(g => new SharedEntity(g.Key.PersonId, g.Key.DisplayName, g.Count()))
            .OrderByDescending(e => e.SharedCount)
            .ToListAsync(cancellationToken);

        var keywords = await _context.RulingKeywords
            .Include(rk => rk.Keyword)
            .Where(rk => idSet.Contains(rk.RulingId))
            .GroupBy(rk => new { rk.KeywordId, rk.Keyword.Description })
            .Where(g => g.Count() > 1)
            .Select(g => new SharedEntity(g.Key.KeywordId, g.Key.Description, g.Count()))
            .OrderByDescending(e => e.SharedCount)
            .ToListAsync(cancellationToken);

        var statutes = await _context.RulingStatutes
            .Include(rs => rs.Statute)
            .Where(rs => idSet.Contains(rs.RulingId))
            .GroupBy(rs => new { rs.StatuteId, Name = rs.Statute.Number + " " + rs.Statute.Name })
            .Where(g => g.Count() > 1)
            .Select(g => new SharedEntity(g.Key.StatuteId, g.Key.Name, g.Count()))
            .OrderByDescending(e => e.SharedCount)
            .ToListAsync(cancellationToken);

        return new SharedEntitiesResult(persons, keywords, statutes);
    }

    public async Task<IReadOnlyList<CitationPathStep>?> GetCitationPathAsync(
        Guid sourceRulingId, Guid targetRulingId, int maxDepth = 5, CancellationToken cancellationToken = default)
    {
        maxDepth = Math.Clamp(maxDepth, 1, 8);

        const string sql = """
            WITH PathCTE AS (
                SELECT r.Id AS RulingId, r.CaseTitle, r.RulingDate, co.Name AS CourtName,
                       CAST('START' AS NVARCHAR(50)) AS CitationType, 0 AS StepIndex,
                       CAST(r.Id AS NVARCHAR(MAX)) AS Path
                FROM Rulings r
                LEFT JOIN Courts co ON co.Id = r.CourtId
                WHERE r.Id = @sourceId

                UNION ALL

                SELECT r2.Id, r2.CaseTitle, r2.RulingDate, co2.Name,
                       c.CitationType, p.StepIndex + 1,
                       p.Path + ',' + CAST(r2.Id AS NVARCHAR(MAX))
                FROM PathCTE p
                JOIN Citations c ON c.SourceRulingId = p.RulingId AND c.TargetRulingId IS NOT NULL
                JOIN Rulings r2 ON r2.Id = c.TargetRulingId
                LEFT JOIN Courts co2 ON co2.Id = r2.CourtId
                WHERE p.StepIndex < @maxDepth
                  AND CHARINDEX(CAST(r2.Id AS NVARCHAR(MAX)), p.Path) = 0
            )
            SELECT TOP 1 Path
            FROM PathCTE
            WHERE RulingId = @targetId
            ORDER BY StepIndex
            """;

        var conn = _context.Database.GetDbConnection();
        await _context.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@sourceId", sourceRulingId));
            cmd.Parameters.Add(new SqlParameter("@targetId", targetRulingId));
            cmd.Parameters.Add(new SqlParameter("@maxDepth", maxDepth));

            var pathStr = (string?)await cmd.ExecuteScalarAsync(cancellationToken);
            if (pathStr is null) return null;

            var pathIds = pathStr.Split(',').Select(Guid.Parse).ToList();

            var rulings = await _context.Rulings
                .Include(r => r.Court)
                .Where(r => pathIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, cancellationToken);

            var citations = await _context.Citations
                .Where(c => pathIds.Contains(c.SourceRulingId) && c.TargetRulingId.HasValue && pathIds.Contains(c.TargetRulingId.Value))
                .ToListAsync(cancellationToken);

            var steps = new List<CitationPathStep>();
            for (var i = 0; i < pathIds.Count; i++)
            {
                var id = pathIds[i];
                if (!rulings.TryGetValue(id, out var r)) continue;

                string? citType = null;
                if (i > 0)
                {
                    var prev = pathIds[i - 1];
                    citType = citations.FirstOrDefault(c => c.SourceRulingId == prev && c.TargetRulingId == id)?.CitationType.ToString();
                }

                steps.Add(new CitationPathStep(id, r.CaseTitle, r.RulingDate, r.Court?.Name, citType, i));
            }
            return steps;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    public async Task<PersonRulingNetwork?> GetPersonRulingNetworkAsync(
        int personId, int topN = 50, CancellationToken cancellationToken = default)
    {
        var person = await _context.Persons.FindAsync([personId], cancellationToken);
        if (person is null) return null;

        var participations = await _context.RulingParticipations
            .Include(rp => rp.Ruling).ThenInclude(r => r.Court)
            .Where(rp => rp.PersonId == personId)
            .OrderByDescending(rp => rp.Ruling.RulingDate)
            .Take(topN)
            .ToListAsync(cancellationToken);

        var groups = participations
            .GroupBy(rp => new { LegalBranch = rp.Ruling.LegalBranch?.ToString(), Role = rp.Role.ToString() })
            .Select(g => new PersonRulingGroup(
                g.Key.LegalBranch,
                g.Key.Role,
                g.Count(),
                g.Select(rp => new PersonRulingEntry(rp.RulingId, rp.Ruling.CaseTitle, rp.Ruling.RulingDate, rp.Ruling.Court?.Name)).ToList()))
            .OrderByDescending(g => g.Count)
            .ToList();

        return new PersonRulingNetwork(personId, person.DisplayName, participations.Count, groups);
    }

    /// <remarks>No-op: Ruling is already persisted by Indexer via IRulingRepository.</remarks>
    /// <remarks>No-op: Ruling is already persisted by Indexer via IRulingRepository.</remarks>
    public Task UpsertRulingNodeAsync(Guid id, string caseTitle, DateOnly rulingDate, string? jurisdictionArea, string? instance, string? rulingDirection, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Person is already persisted by Indexer via IPersonRepository.</remarks>
    public Task UpsertPersonNodeAsync(int id, string displayName, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Court is already persisted by Indexer via ICourtRepository.</remarks>
    public Task UpsertCourtNodeAsync(int id, string name, string jurisdictionArea, string territory, string instance, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Keyword is already persisted by Indexer via IKeywordRepository.</remarks>
    public Task UpsertKeywordNodeAsync(int id, string description, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Statute is already persisted by Indexer via IStatuteRepository.</remarks>
    public Task UpsertStatuteNodeAsync(string number, string name, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: RulingParticipations is already persisted by Indexer when creating the ruling.</remarks>
    public Task CreateSignedByRelationshipAsync(Guid rulingId, int personId, RulingRole role, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Citations is already persisted by Indexer via ICitationRepository.</remarks>
    public Task CreateCitesRelationshipAsync(Guid sourceRulingId, Guid targetRulingId, CitationType citationType, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: RulingKeywords is already persisted by Indexer when creating the ruling.</remarks>
    public Task CreateHasKeywordRelationshipAsync(Guid rulingId, int keywordId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: RulingStatutes is already persisted by Indexer when creating the ruling.</remarks>
    public Task CreateCitesStatuteRelationshipAsync(Guid rulingId, int statuteId, string? articles, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: Rulings.CourtId is already set by Indexer when creating the ruling.</remarks>
    public Task CreateIssuedByRelationshipAsync(Guid rulingId, int courtId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    /// <remarks>No-op: JudicialOffices is already set by Indexer.</remarks>
    public Task CreateMemberOfRelationshipAsync(int personId, int courtId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
