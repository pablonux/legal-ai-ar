using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IJudicialProceedingRepository
{
    Task<(IReadOnlyList<JudicialProceeding> Items, int TotalCount)> SearchAsync(
        string? search,
        ProcessType? processType,
        LegalBranch? legalBranch,
        int? courtId,
        ProcessStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<JudicialProceeding?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<JudicialProceeding?> GetByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default);

    Task<JudicialProceeding?> FindByCaseNumberAsync(string caseNumber, string? jurisdictionArea, CancellationToken cancellationToken = default);

    Task AddAsync(JudicialProceeding proceeding, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Groups unlinked rulings by CaseNumber+JurisdictionArea, creates proceedings,
    /// and links rulings. Returns (proceedingsCreated, rulingsLinked).
    /// </summary>
    Task<(int ProceedingsCreated, int RulingsLinked)> LinkUnlinkedRulingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Infers ProcessType, ProcessStatus, CourtId, and LegalBranch for proceedings
    /// that have linked rulings but are missing these fields.
    /// Returns total proceedings updated.
    /// </summary>
    Task<int> BackfillProceedingFieldsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns proceedings that have no ProceedingParty records yet.
    /// Keyset-paginated by Id.
    /// </summary>
    Task<IReadOnlyList<JudicialProceeding>> GetProceedingsWithoutPartiesAsync(
        int lastId, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a ProceedingParty if the combination (proceedingId, personId, role) doesn't exist.
    /// </summary>
    Task AddPartyIfNotExistsAsync(int proceedingId, int personId, PartyRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a proceeding with its rulings (ordered chronologically) and procedural remedies,
    /// suitable for building the appeal chain visualization.
    /// </summary>
    Task<(JudicialProceeding Proceeding, IReadOnlyList<ProceduralRemedy> Remedies)?> GetAppealChainAsync(
        int proceedingId, CancellationToken cancellationToken = default);
}
