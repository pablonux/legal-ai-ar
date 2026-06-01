using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Messages;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Retroactive citation resolution: matches Citations with TargetRulingId=null to newly indexed rulings.
/// Inbound: Citations from other rulings that cite the new ruling.
/// Outbound: Citations from the new ruling to other rulings.
/// Per E056, E058: non-fatal — log and continue on failure.
/// </summary>
public class ResolveCitationsStep
{
    private readonly ICitationRepository _citationRepository;
    private readonly IRulingRepository _rulingRepository;
    private readonly ILogger<ResolveCitationsStep> _logger;

    public ResolveCitationsStep(
        ICitationRepository citationRepository,
        IRulingRepository rulingRepository,
        ILogger<ResolveCitationsStep> logger)
    {
        _citationRepository = citationRepository;
        _rulingRepository = rulingRepository;
        _logger = logger;
    }

    /// <summary>
    /// Resolves inbound and outbound citations for the newly indexed ruling.
    /// Does not throw; logs errors and continues.
    /// </summary>
    public async Task ExecuteAsync(
        Guid rulingId,
        IndexerMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await ResolveInboundAsync(rulingId, message, cancellationToken);
            await ResolveOutboundAsync(rulingId, message, cancellationToken);
            await _citationRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResolveCitationsStep failed for ruling {RulingId}. Citations remain unresolved.", rulingId);
        }
    }

    private async Task ResolveInboundAsync(
        Guid rulingId,
        IndexerMessage message,
        CancellationToken cancellationToken)
    {
        var candidates = BuildMatchCandidates(message.Ruling.CaseNumber, message.DocumentId);
        if (candidates.Count == 0)
            return;

        var pendingCitations = await _citationRepository.GetPendingByExternalAliasMatchAsync(candidates, cancellationToken);
        foreach (var citation in pendingCitations)
        {
            await _citationRepository.UpdateTargetRulingIdAsync(citation.Id, rulingId, cancellationToken);
            _logger.LogDebug(
                "Resolved inbound citation {CitationId}: ExternalAlias {ExternalAlias} -> TargetRulingId {RulingId}",
                citation.Id,
                citation.ExternalAlias,
                rulingId);
        }

        if (pendingCitations.Count > 0)
        {
            _logger.LogInformation(
                "Resolved {Count} inbound citation(s) for ruling {RulingId}",
                pendingCitations.Count,
                rulingId);
        }
    }

    private async Task ResolveOutboundAsync(
        Guid rulingId,
        IndexerMessage message,
        CancellationToken cancellationToken)
    {
        var pendingCitations = await _citationRepository.GetPendingOutboundBySourceRulingAsync(rulingId, cancellationToken);
        foreach (var citation in pendingCitations)
        {
            var normalizedAlias = citation.ExternalAlias.Trim().ToLowerInvariant();
            var matchingRulings = await _rulingRepository.FindByCaseNumberOrExternalAliasAsync(normalizedAlias, cancellationToken);

            if (matchingRulings.Count == 1)
            {
                await _citationRepository.UpdateTargetRulingIdAsync(citation.Id, matchingRulings[0].Id, cancellationToken);
                _logger.LogDebug(
                    "Resolved outbound citation {CitationId}: ExternalAlias {ExternalAlias} -> TargetRulingId {TargetId}",
                    citation.Id,
                    citation.ExternalAlias,
                    matchingRulings[0].Id);
            }
            else if (matchingRulings.Count > 1)
            {
                _logger.LogWarning(
                    "Ambiguous citation: ExternalAlias {ExternalAlias} matches {Count} rulings. Leaving TargetRulingId null.",
                    citation.ExternalAlias,
                    matchingRulings.Count);
            }
        }

        if (pendingCitations.Count > 0)
        {
            _logger.LogInformation(
                "Processed {Count} outbound citation(s) for ruling {RulingId}",
                pendingCitations.Count,
                rulingId);
        }
    }

    /// <summary>
    /// Builds normalized match candidates from CaseNumber and ExternalId.
    /// Used for inbound resolution: Citations.ExternalAlias matching our new ruling.
    /// </summary>
    private static IReadOnlyList<string> BuildMatchCandidates(string? caseNumber, string? externalId)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(caseNumber))
        {
            var normalized = caseNumber.Trim().ToLowerInvariant();
            set.Add(normalized);
            if (normalized.Contains(':'))
                set.Add("fallos: " + normalized);
        }

        if (!string.IsNullOrWhiteSpace(externalId))
        {
            var normalized = externalId.Trim().ToLowerInvariant();
            if (!set.Contains(normalized))
                set.Add(normalized);
        }

        return set.ToList();
    }
}
