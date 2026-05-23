using System.Text.RegularExpressions;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record ResolveCitationsCommand(int BatchSize = 200) : IRequest<ResolveCitationsResult>;

public record ResolveCitationsResult(
    int Processed,
    int ResolvedByVolumePage,
    int ResolvedByCaseNumber,
    int Unresolvable);

public partial class ResolveCitationsHandler
    : IRequestHandler<ResolveCitationsCommand, ResolveCitationsResult>
{
    private readonly ICitationRepository _citations;
    private readonly IRulingRepository _rulings;
    private readonly ILogger<ResolveCitationsHandler> _logger;

    public ResolveCitationsHandler(
        ICitationRepository citations,
        IRulingRepository rulings,
        ILogger<ResolveCitationsHandler> logger)
    {
        _citations = citations;
        _rulings = rulings;
        _logger = logger;
    }

    public async Task<ResolveCitationsResult> Handle(
        ResolveCitationsCommand request, CancellationToken cancellationToken)
    {
        int processed = 0, byVolumePage = 0, byCaseNumber = 0, unresolvable = 0;
        int lastId = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _citations.GetUnresolvedCitationsAsync(
                lastId, request.BatchSize, cancellationToken);

            if (batch.Count == 0) break;

            foreach (var citation in batch)
            {
                lastId = citation.Id;
                processed++;

                var resolved = await TryResolveByVolumePage(citation.ExternalAlias, cancellationToken);

                if (resolved is null)
                    resolved = await TryResolveByCaseNumber(citation.ExternalAlias, cancellationToken);

                if (resolved is not null)
                {
                    if (resolved.Value.Method == "VolumePage") byVolumePage++;
                    else byCaseNumber++;

                    await _citations.UpdateTargetRulingIdAsync(
                        citation.Id, resolved.Value.RulingId, cancellationToken);
                }
                else
                {
                    unresolvable++;
                }
            }

            await _citations.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Citation resolution progress: {Processed} processed, {ByVolumePage} by volume/page, {ByCaseNumber} by case number, {Unresolvable} unresolvable",
                processed, byVolumePage, byCaseNumber, unresolvable);
        }

        return new ResolveCitationsResult(processed, byVolumePage, byCaseNumber, unresolvable);
    }

    private async Task<ResolveResult?> TryResolveByVolumePage(
        string externalAlias, CancellationToken cancellationToken)
    {
        var match = FallosPattern().Match(externalAlias);
        if (!match.Success) return null;

        var volume = match.Groups["volume"].Value.Trim();
        var page = match.Groups["page"].Value.Trim();

        var rulingId = await _citations.FindRulingByVolumenPageAsync(volume, page, cancellationToken);
        return rulingId is not null
            ? new ResolveResult(rulingId.Value, "VolumePage")
            : null;
    }

    private async Task<ResolveResult?> TryResolveByCaseNumber(
        string externalAlias, CancellationToken cancellationToken)
    {
        var normalized = NormalizeAlias(externalAlias);
        if (string.IsNullOrWhiteSpace(normalized)) return null;

        var matches = await _rulings.FindByCaseNumberOrExternalAliasAsync(normalized, cancellationToken);

        return matches.Count == 1
            ? new ResolveResult(matches[0].Id, "CaseNumber")
            : null;
    }

    private static string NormalizeAlias(string alias)
    {
        var cleaned = alias.Trim();
        cleaned = Regex.Replace(cleaned, @"^(Fallos|CSJN|CSJ)\s*:?\s*", "", RegexOptions.IgnoreCase);
        cleaned = Regex.Replace(cleaned, @"\s+", " ");
        return cleaned.Trim();
    }

    [GeneratedRegex(@"Fallos\s*:?\s*(?<volume>\d+)\s*:\s*(?<page>\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex FallosPattern();

    private readonly record struct ResolveResult(Guid RulingId, string Method);
}
