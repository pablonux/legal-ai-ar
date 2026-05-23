using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Parses SAIJ jurisprudencia JSON and persists as Ruling entity.
/// Handles deduplication by ExternalId (SaijId) to avoid duplicates with CSJN source.
/// </summary>
public partial class SaijRulingParser
{
    private const int SaijRulingsSourceId = 3;

    private readonly IBlobStorageService _blobStorage;
    private readonly IRulingRepository _rulingRepo;
    private readonly ISearchIndexService _searchIndex;
    private readonly ILogger<SaijRulingParser> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public SaijRulingParser(
        IBlobStorageService blobStorage,
        IRulingRepository rulingRepo,
        ISearchIndexService searchIndex,
        ILogger<SaijRulingParser> logger)
    {
        _blobStorage = blobStorage;
        _rulingRepo = rulingRepo;
        _searchIndex = searchIndex;
        _logger = logger;
    }

    public async Task ProcessAsync(ParserMessage message, CancellationToken ct)
    {
        _logger.LogInformation("SAIJ Ruling parser: processing {DocumentId}", message.DocumentId);

        await using var blobStream = await _blobStorage.DownloadAsync(message.BlobPathPdf, ct);
        var doc = await JsonSerializer.DeserializeAsync<SaijRulingDocument>(blobStream, JsonOpts, ct);

        if (doc is null)
        {
            _logger.LogWarning("SAIJ ruling {DocumentId} deserialized as null, skipping", message.DocumentId);
            return;
        }

        var externalId = doc.IdInfojus ?? message.DocumentId;
        var existing = await _rulingRepo.FindByExternalIdAsync(SaijRulingsSourceId, externalId, ct);
        if (existing is not null)
        {
            _logger.LogInformation("SAIJ Ruling {ExternalId} already exists (Id={RulingId}), skipping", externalId, existing.Id);
            return;
        }

        var caseTitle = doc.Caratula ?? doc.TituloSumario ?? $"SAIJ-{externalId}";
        var caseNumber = doc.NroExpediente ?? doc.NroInterno;
        var fullText = StripHtml(doc.TextoFallo ?? doc.TextoOriginal);
        var summary = doc.Sumario ?? doc.TituloSumario;

        DateOnly rulingDate = default;
        if (DateOnly.TryParse(doc.FechaResolucion, out var parsed))
            rulingDate = parsed;
        else if (DateOnly.TryParse(doc.FechaPublicacion, out var pub))
            rulingDate = pub;

        var ruling = new Ruling
        {
            SourceId = SaijRulingsSourceId,
            ExternalId = externalId,
            CaseTitle = caseTitle,
            CaseNumber = caseNumber,
            RulingDate = rulingDate,
            CourtId = 1, // default; will be resolved during enrichment
            JurisdictionArea = doc.Jurisdiccion,
            Instance = doc.Instancia,
            ResourceType = doc.TipoRecurso,
            Summary = summary,
            FullText = fullText,
            BlobPath = message.BlobPathPdf,
            Status = RulingStatus.Pending,
            IndexedAt = DateTime.UtcNow,
        };

        if (!string.IsNullOrEmpty(doc.RamaDerecho))
            ruling.LegalBranch = MapLegalBranch(doc.RamaDerecho);

        await _rulingRepo.AddAsync(ruling, ct);

        _logger.LogInformation(
            "SAIJ Ruling: persisted Id={RulingId} ExternalId={ExternalId} Title={Title}",
            ruling.Id, externalId, caseTitle);
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return null;
        var text = HtmlTagRegex().Replace(html, " ");
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    private static LegalBranch? MapLegalBranch(string? rama) => rama?.ToLowerInvariant() switch
    {
        "derecho civil" or "civil" => LegalBranch.PRIV_CIVIL,
        "derecho penal" or "penal" => LegalBranch.PUB_PENAL,
        "derecho laboral" or "laboral" => LegalBranch.PRIV_LAB,
        "derecho administrativo" or "administrativo" => LegalBranch.PUB_ADMIN,
        "derecho constitucional" or "constitucional" => LegalBranch.PUB_CONST,
        "derecho comercial" or "comercial" => LegalBranch.PRIV_COM,
        "derecho tributario" or "tributario" => LegalBranch.PUB_TRIB,
        "derecho de familia" or "familia" => LegalBranch.SOC_FAM,
        _ => null,
    };

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    private sealed class SaijRulingDocument
    {
        public string? IdInfojus { get; set; }
        public string? Caratula { get; set; }
        public string? TituloSumario { get; set; }
        public string? NroExpediente { get; set; }
        public string? NroInterno { get; set; }
        public string? FechaResolucion { get; set; }
        public string? FechaPublicacion { get; set; }
        public string? Tribunal { get; set; }
        public string? Jurisdiccion { get; set; }
        public string? Instancia { get; set; }
        public string? TipoRecurso { get; set; }
        public string? RamaDerecho { get; set; }
        public string? Sumario { get; set; }
        public string? TextoFallo { get; set; }
        public string? TextoOriginal { get; set; }
        public string? UrlOriginal { get; set; }
    }
}
