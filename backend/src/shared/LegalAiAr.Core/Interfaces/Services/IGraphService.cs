using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Graph operations for rulings, persons, courts, keywords, statutes and relationships.
/// SqlGraphService queries Azure SQL with recursive CTEs for multi-hop traversal.
/// </summary>
public interface IGraphService
{
    Task<IReadOnlyList<Citation>> GetOutboundCitationsAsync(Guid rulingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Citation>> GetInboundCitationsAsync(Guid rulingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// N-hop citation chain via recursive CTE. Returns all reachable rulings up to <paramref name="maxDepth"/> hops.
    /// </summary>
    Task<IReadOnlyList<CitationChainNode>> GetCitationChainAsync(Guid rulingId, int maxDepth = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Full graph neighborhood: ruling's persons, statutes, keywords, court, plus cited/citing rulings.
    /// </summary>
    Task<RulingGraphNeighborhood?> GetGraphNeighborhoodAsync(Guid rulingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities shared across a set of ruling IDs (common persons, keywords, statutes).
    /// </summary>
    Task<SharedEntitiesResult> GetSharedEntitiesAsync(IReadOnlyList<Guid> rulingIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shortest citation path between two rulings via recursive CTE with path tracking.
    /// </summary>
    Task<IReadOnlyList<CitationPathStep>?> GetCitationPathAsync(Guid sourceRulingId, Guid targetRulingId, int maxDepth = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Person ruling network: rulings signed by a person grouped by LegalBranch and RulingRole.
    /// </summary>
    Task<PersonRulingNetwork?> GetPersonRulingNetworkAsync(int personId, int topN = 50, CancellationToken cancellationToken = default);

    Task UpsertRulingNodeAsync(Guid id, string caseTitle, DateOnly rulingDate, string? jurisdictionArea, string? instance, string? rulingDirection, CancellationToken cancellationToken = default);
    Task UpsertPersonNodeAsync(int id, string displayName, CancellationToken cancellationToken = default);
    Task UpsertCourtNodeAsync(int id, string name, string jurisdictionArea, string territory, string instance, CancellationToken cancellationToken = default);
    Task UpsertKeywordNodeAsync(int id, string description, CancellationToken cancellationToken = default);
    Task UpsertStatuteNodeAsync(string number, string name, CancellationToken cancellationToken = default);
    Task CreateSignedByRelationshipAsync(Guid rulingId, int personId, RulingRole role, CancellationToken cancellationToken = default);
    Task CreateCitesRelationshipAsync(Guid sourceRulingId, Guid targetRulingId, CitationType citationType, CancellationToken cancellationToken = default);
    Task CreateHasKeywordRelationshipAsync(Guid rulingId, int keywordId, CancellationToken cancellationToken = default);
    Task CreateCitesStatuteRelationshipAsync(Guid rulingId, int statuteId, string? articles, CancellationToken cancellationToken = default);
    Task CreateIssuedByRelationshipAsync(Guid rulingId, int courtId, CancellationToken cancellationToken = default);
    Task CreateMemberOfRelationshipAsync(int personId, int courtId, CancellationToken cancellationToken = default);
}
