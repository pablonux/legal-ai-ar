using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IThesaurusRepository
{
    Task<ThesaurusTerm?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default);
    Task<ThesaurusTerm?> GetByLabelAsync(string label, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ThesaurusTerm>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default);

    /// <summary>Returns preferred terms at depth 0 (top-level branches).</summary>
    Task<IReadOnlyList<ThesaurusTerm>> GetRootTermsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the narrower (child) preferred terms for a given term, via NT relations.</summary>
    Task<IReadOnlyList<ThesaurusTerm>> GetChildrenAsync(int termId, CancellationToken cancellationToken = default);

    /// <summary>Gets a single preferred term by DB Id with all its relations eagerly loaded.</summary>
    Task<ThesaurusTerm?> GetByIdWithRelationsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches preferred terms matching the query and eagerly loads their relations.</summary>
    Task<IReadOnlyList<ThesaurusTerm>> SearchWithRelationsAsync(string query, int limit = 5, CancellationToken cancellationToken = default);

    /// <summary>Finds a preferred term matching the label (exact or via synonym redirect).</summary>
    Task<ThesaurusTerm?> ResolvePreferredTermAsync(string label, CancellationToken cancellationToken = default);

    /// <summary>Returns all synonym pairs (UF relations) for synonym map generation.</summary>
    Task<IReadOnlyList<(string Preferred, string NonPreferred)>> GetAllSynonymPairsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns all ExternalIds of preferred terms (for Phase 2 crawling).</summary>
    Task<IReadOnlyList<int>> GetAllExternalIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns full ExternalId → DbId mapping for relation building.</summary>
    Task<Dictionary<int, int>> GetExternalIdToDbIdMapAsync(CancellationToken cancellationToken = default);

    Task UpsertTermAsync(ThesaurusTerm term, CancellationToken cancellationToken = default);
    Task UpsertRelationAsync(int sourceTermId, int targetTermId, ThesaurusRelationType relationType, CancellationToken cancellationToken = default);
    Task<int> GetTermCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetRelationCountAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
