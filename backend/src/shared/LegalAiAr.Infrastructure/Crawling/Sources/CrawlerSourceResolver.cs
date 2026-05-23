using LegalAiAr.Core.Interfaces.Pipeline;

namespace LegalAiAr.Infrastructure.Crawling.Sources;

/// <summary>
/// Resolves crawler sources by source ID and optional discovery method.
/// CSJN=1 defaults to acuerdo-based HTTP discovery.
/// Type "fallos-destacados" routes to the curated-collection source.
/// </summary>
public class CrawlerSourceResolver : ICrawlerSourceResolver
{
    private const int CsjnSourceId = 1;
    private const int SaijLegislationSourceId = 2;
    private const int SaijRulingsSourceId = 3;
    internal const string FallosDestacadosType = "fallos-destacados";
    internal const string LegislationType = "legislacion";

    private readonly CsjnAcuerdoDiscoverySource _csjnAcuerdoSource;
    private readonly CsjnSumariosDiscoverySource _csjnSumariosSource;
    private readonly CsjnFallosDestacadosSource _csjnFallosDestacadosSource;
    private readonly SaijLegislationCrawlerSource _saijLegislationSource;
    private readonly SaijRulingCrawlerSource _saijRulingSource;

    public CrawlerSourceResolver(
        CsjnAcuerdoDiscoverySource csjnAcuerdoSource,
        CsjnSumariosDiscoverySource csjnSumariosSource,
        CsjnFallosDestacadosSource csjnFallosDestacadosSource,
        SaijLegislationCrawlerSource saijLegislationSource,
        SaijRulingCrawlerSource saijRulingSource)
    {
        _csjnAcuerdoSource = csjnAcuerdoSource;
        _csjnSumariosSource = csjnSumariosSource;
        _csjnFallosDestacadosSource = csjnFallosDestacadosSource;
        _saijLegislationSource = saijLegislationSource;
        _saijRulingSource = saijRulingSource;
    }

    /// <inheritdoc />
    public ICrawlerSource? GetSource(int sourceId, string? type = null)
    {
        if (sourceId == SaijLegislationSourceId)
            return _saijLegislationSource;

        if (sourceId == SaijRulingsSourceId)
            return _saijRulingSource;

        if (sourceId != CsjnSourceId)
            return null;

        if (string.Equals(type, FallosDestacadosType, StringComparison.OrdinalIgnoreCase))
            return _csjnFallosDestacadosSource;

        return _csjnAcuerdoSource;
    }

    /// <summary>
    /// Returns the CSJN sumarios discovery source for historical coverage (1863-2026).
    /// </summary>
    public ICrawlerSource? GetSumariosSource(int sourceId) =>
        sourceId == CsjnSourceId ? _csjnSumariosSource : null;
}
