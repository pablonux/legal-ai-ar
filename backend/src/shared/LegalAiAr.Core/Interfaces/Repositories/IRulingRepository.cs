using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IRulingRepository
{
    Task<Ruling?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches only the fields required to build a RAG chat context for multiple rulings in a single query.
    /// Much lighter than GetByIdAsync — no judges, keywords, statutes or citations.
    /// </summary>
    Task<Dictionary<Guid, RulingChatMetadata>> GetChatMetadataBatchAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Idempotency lookup by content hash. Returns a <b>minimal</b> detached <see cref="Ruling"/> (only <c>Id</c> and <c>ContentHash</c> are populated)
    /// to avoid reading wide columns (e.g. <c>FullText</c>) — see <c>IX_Rulings_ContentHash</c> in the database.
    /// </summary>
    Task<Ruling?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default);
    Task<bool> ExistsByContentHashAsync(string contentHash, CancellationToken cancellationToken = default);
    Task<bool> ExistsByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default);
    Task<Ruling?> FindByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default);
    Task AddAsync(Ruling ruling, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rulings where CaseNumber or ExternalAlias matches the given normalized value.
    /// Used for citation resolution: matching ExternalAlias to indexed rulings.
    /// </summary>
    /// <returns>Matching rulings. Empty if none; multiple if ambiguous (caller should not resolve).</returns>
    Task<IReadOnlyList<Ruling>> FindByCaseNumberOrExternalAliasAsync(
        string normalizedValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts rulings matching the given filters using SQL COUNT(*).
    /// </summary>
    Task<int> CountAsync(CountFilters filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves case titles for a batch of ruling IDs. Used by the output guardrail
    /// to validate citations in model responses.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> GetCaseTitlesByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a ruling by CSJN AnalysisId. Used for reverse citation resolution (getCitantes).
    /// </summary>
    Task<Ruling?> FindByAnalysisIdAsync(string analysisId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves many analysis ids in one query. Keys are analysis ids as stored on <see cref="Ruling.AnalysisId"/>.
    /// When multiple rulings share an analysis id, the first row returned by the provider wins.
    /// </summary>
    Task<IReadOnlyDictionary<string, Guid>> FindRulingIdsByAnalysisIdsAsync(
        IEnumerable<string> analysisIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a page of rulings with full navigation properties for message reconstruction.
    /// Keyset-paginated by Id (pass lastId = Guid.Empty for the first page).
    /// </summary>
    Task<IReadOnlyList<Ruling>> GetPageForRequeueAsync(
        Guid lastId,
        int pageSize,
        bool onlyMissingOntology,
        int? sourceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns CSJN rulings that have FullText but no ProsecutorOpinion yet.
    /// Keyset-paginated by Id.
    /// </summary>
    Task<IReadOnlyList<Ruling>> GetCsjnRulingsWithoutProsecutorOpinionAsync(
        Guid lastId,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a ProsecutorOpinion entity to the context and saves.
    /// </summary>
    Task AddProsecutorOpinionAsync(ProsecutorOpinion opinion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all ProsecutorOpinion rows. Returns the count of deleted rows.
    /// </summary>
    Task<int> DeleteAllProsecutorOpinionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns rulings that have FullText but no RatioDecidendi yet.
    /// Keyset-paginated by Id.
    /// </summary>
    Task<IReadOnlyList<Ruling>> GetRulingsWithoutDoctrineAsync(
        Guid lastId,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates RatioDecidendi and DoctrinaLegal on an existing ruling and saves.
    /// </summary>
    Task UpdateDoctrineFieldsAsync(
        Guid rulingId,
        string? ratioDecidendi,
        string? doctrinaLegal,
        CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Guid rulingId, RulingStatus status, CancellationToken cancellationToken = default);
}
