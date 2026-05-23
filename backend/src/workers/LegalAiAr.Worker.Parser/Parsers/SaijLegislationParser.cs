using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Parses SAIJ legislation JSON (uploaded by crawler as raw bytes in blob).
/// Extracts metadata, text, and persists the Statute entity directly
/// (legislation does not need the enrichment/indexer pipeline — it's self-contained).
/// </summary>
public partial class SaijLegislationParser
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IStatuteRepository _statuteRepo;
    private readonly ICitationRepository _citationRepo;
    private readonly ISearchIndexService _searchIndex;
    private readonly ILogger<SaijLegislationParser> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public SaijLegislationParser(
        IBlobStorageService blobStorage,
        IStatuteRepository statuteRepo,
        ICitationRepository citationRepo,
        ISearchIndexService searchIndex,
        ILogger<SaijLegislationParser> logger)
    {
        _blobStorage = blobStorage;
        _statuteRepo = statuteRepo;
        _citationRepo = citationRepo;
        _searchIndex = searchIndex;
        _logger = logger;
    }

    public async Task ProcessAsync(ParserMessage message, CancellationToken ct)
    {
        _logger.LogInformation("SAIJ Legislation parser: processing {DocumentId}", message.DocumentId);

        await using var blobStream = await _blobStorage.DownloadAsync(message.BlobPathPdf, ct);
        var doc = await JsonSerializer.DeserializeAsync<SaijDocument>(blobStream, JsonOpts, ct);

        if (doc is null)
        {
            _logger.LogWarning("SAIJ document {DocumentId} deserialized as null, skipping", message.DocumentId);
            return;
        }

        var number = doc.NroNorma ?? doc.IdNorma ?? message.DocumentId;
        var name = doc.TituloSumario ?? doc.TituloResumido ?? doc.Titulo ?? $"Norma {number}";
        var normType = MapNormType(doc.TipoNorma);
        var normLevel = InferNormativeLevel(normType);
        var fullText = StripHtml(doc.TextoOriginal ?? doc.TextoActualizado);
        var status = MapStatus(doc.EstadoVigencia);

        Core.Entities.Statute? statute = null;

        if (!string.IsNullOrEmpty(doc.IdInfojus))
            statute = await FindBySaijIdAsync(doc.IdInfojus, ct);

        statute ??= await _statuteRepo.GetOrCreateAsync(number, name, ct);

        statute.Name = name;
        statute.NormType = normType;
        statute.NormativeLevel = normLevel;
        statute.SaijId = doc.IdInfojus ?? message.DocumentId;
        statute.IssuingBodyName = doc.OrganismoOrigen;
        statute.FullText = fullText;
        statute.Status = status;
        statute.OfficialUrl = doc.UrlOriginal;

        if (DateOnly.TryParse(doc.FechaSancion, out var sanctionDate))
            statute.SanctionDate = sanctionDate;
        if (DateOnly.TryParse(doc.FechaPublicacion, out var pubDate))
            statute.PublicationDate = pubDate;

        await _statuteRepo.SaveChangesAsync(ct);

        var rulingCount = statute.RulingStatutes?.Count ?? 0;
        var indexInput = new StatuteIndexInput(
            StatuteId: statute.Id,
            Number: statute.Number,
            Name: statute.Name,
            NormType: statute.NormType?.ToString(),
            NormativeLevel: statute.NormativeLevel?.ToString(),
            LegalBranch: statute.LegalBranch?.ToString(),
            IssuingBody: statute.IssuingBodyName ?? statute.IssuingBody,
            SanctionDate: statute.SanctionDate,
            PublicationDate: statute.PublicationDate,
            Status: statute.Status?.ToString(),
            IsVigente: statute.IsVigente,
            FullText: statute.FullText,
            SaijId: statute.SaijId,
            RulingCount: rulingCount);

        await _searchIndex.IndexStatuteAsync(indexInput, ct);

        var linked = await _citationRepo.LinkStatuteToRulingsAsync(
            statute.Id, statute.Number, statute.Name, ct);
        if (linked > 0)
            _logger.LogInformation("Linked {Count} existing citations to Statute {StatuteId}", linked, statute.Id);

        _logger.LogInformation(
            "SAIJ Legislation: persisted + indexed Statute Id={StatuteId} Number={Number} SaijId={SaijId}",
            statute.Id, statute.Number, statute.SaijId);
    }

    private async Task<Core.Entities.Statute?> FindBySaijIdAsync(string saijId, CancellationToken ct)
    {
        try
        {
            return await _statuteRepo.FindBySaijIdAsync(saijId, ct);
        }
        catch
        {
            return null;
        }
    }

    private static NormType? MapNormType(string? tipo)
    {
        if (string.IsNullOrEmpty(tipo)) return null;
        var lower = tipo.ToLowerInvariant().Trim();
        return lower switch
        {
            "ley" => NormType.LAW,
            "decreto" => NormType.DECREE,
            "resolucion" or "resolución" => NormType.RESOLUTION,
            "decision administrativa" or "decisión administrativa" => NormType.RESOLUTION,
            "acordada" => NormType.ACORDADA,
            "decreto de necesidad y urgencia" or "dnu" => NormType.DNU,
            "tratado" => NormType.TREATY,
            "constitucion" or "constitución" => NormType.CONSTITUTION,
            "ordenanza" => NormType.ORDINANCE,
            _ => null,
        };
    }

    private static NormativeLevel? InferNormativeLevel(NormType? normType)
    {
        return normType switch
        {
            NormType.CONSTITUTION => NormativeLevel.CONSTITUTIONAL,
            NormType.TREATY => NormativeLevel.SUPRALEGAL,
            NormType.LAW or NormType.DNU => NormativeLevel.LEGAL,
            NormType.DECREE or NormType.RESOLUTION or NormType.ACORDADA => NormativeLevel.REGULATORY,
            NormType.ORDINANCE => NormativeLevel.INDIVIDUAL,
            _ => null,
        };
    }

    private static StatuteStatus? MapStatus(string? estado)
    {
        if (string.IsNullOrEmpty(estado)) return null;
        var lower = estado.ToLowerInvariant().Trim();
        if (lower.Contains("vigente") && !lower.Contains("no vigente"))
            return lower.Contains("modific") ? StatuteStatus.ModificadaParcialmente : StatuteStatus.Vigente;
        if (lower.Contains("derogad")) return StatuteStatus.Derogada;
        if (lower.Contains("veto total")) return StatuteStatus.VetoTotal;
        if (lower.Contains("veto parcial")) return StatuteStatus.VetoParcial;
        if (lower.Contains("no vigente")) return StatuteStatus.Derogada;
        return null;
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return null;
        var text = HtmlTagRegex().Replace(html, "");
        text = System.Net.WebUtility.HtmlDecode(text);
        return text.Trim();
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    private sealed class SaijDocument
    {
        public string? Id { get; set; }
        public string? IdInfojus { get; set; }
        public string? IdNorma { get; set; }
        public string? NroNorma { get; set; }
        public string? TipoNorma { get; set; }
        public string? Titulo { get; set; }
        public string? TituloSumario { get; set; }
        public string? TituloResumido { get; set; }
        public string? TextoOriginal { get; set; }
        public string? TextoActualizado { get; set; }
        public string? FechaSancion { get; set; }
        public string? FechaPublicacion { get; set; }
        public string? OrganismoOrigen { get; set; }
        public string? EstadoVigencia { get; set; }
        public string? UrlOriginal { get; set; }
    }
}
