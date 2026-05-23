using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface ICitationRepository
{
    Task<Citation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Citation citation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets citations with TargetRulingId null where ExternalAlias matches any of the given normalized values.
    /// Used for inbound resolution when a newly indexed ruling may be the target of existing citations.
    /// </summary>
    Task<IReadOnlyList<Citation>> GetPendingByExternalAliasMatchAsync(
        IReadOnlyList<string> normalizedAliases,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets citations from the given source ruling with TargetRulingId null.
    /// Used for outbound resolution after persisting a new ruling.
    /// </summary>
    Task<IReadOnlyList<Citation>> GetPendingOutboundBySourceRulingAsync(
        Guid sourceRulingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates TargetRulingId for the given citation without saving.
    /// Call SaveChangesAsync to persist accumulated updates.
    /// </summary>
    Task UpdateTargetRulingIdAsync(int citationId, Guid targetRulingId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all unresolved citations (TargetRulingId IS NULL), keyset-paginated.
    /// </summary>
    Task<IReadOnlyList<Citation>> GetUnresolvedCitationsAsync(
        int lastId, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a ruling ID by matching Sumario Volume+Page (e.g. "328" + "1883").
    /// Returns null if no match or ambiguous.
    /// </summary>
    Task<Guid?> FindRulingByVolumenPageAsync(
        string volume, string page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Links unresolved RulingStatute entries to a specific statute,
    /// matching by number and/or norm name. Returns count of linked records.
    /// </summary>
    Task<int> LinkStatuteToRulingsAsync(int statuteId, string number, string name, CancellationToken cancellationToken = default);
}
