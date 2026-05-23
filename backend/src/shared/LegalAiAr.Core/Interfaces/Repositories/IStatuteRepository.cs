using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IStatuteRepository
{
    Task<Statute?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Statute> GetOrCreateAsync(string number, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves many statutes in one round-trip where possible (cache + single SQL IN on <c>Number</c>).
    /// Keys are <c>Number</c> (ordinal). Caller should align <c>Name</c> on each payload row after lookup if it differs.
    /// </summary>
    Task<IReadOnlyDictionary<string, Statute>> GetOrCreateBatchAsync(
        IReadOnlyCollection<(string Number, string Name)> pairs,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StatuteWithCount> Items, int TotalCount)> SearchAsync(
        string? search,
        NormType? normType,
        NormativeLevel? normativeLevel,
        LegalBranch? legalBranch,
        bool? isVigente,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Statute?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<Dictionary<NormativeLevel, int>> GetCountsByNormativeLevelAsync(CancellationToken cancellationToken = default);

    Task<Dictionary<NormativeLevel, int>> GetVigenteCountsByNormativeLevelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rulings that cite a statute matching the given criteria.
    /// Joins Statutes → RulingStatutes → Rulings with LIKE matching on name/number.
    /// </summary>
    Task<IReadOnlyList<StatuteRulingResult>> FindRulingsByStatuteAsync(
        string statuteName,
        string? statuteNumber = null,
        string? articles = null,
        int topK = 10,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Statute>> GetUnclassifiedAsync(CancellationToken cancellationToken = default);

    Task<int> BackfillLegalBranchFromRulingsAsync(CancellationToken cancellationToken = default);

    Task<Statute?> FindBySaijIdAsync(string saijId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
