using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Blob;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Strategies;

/// <summary>
/// SAIJ legislation parse strategy: reads JSON from blob, extracts statute metadata,
/// uploads text + metadata blobs, and produces an EnrichmentMessage.
/// SAIJ legislation is self-contained — MissingFields is empty (no AI enrichment needed).
/// </summary>
public sealed partial class SaijLegislationParseStrategy : IParseStrategy
{
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<SaijLegislationParseStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions MetadataJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SaijLegislationParseStrategy(
        IBlobStorageService blobStorage,
        ILogger<SaijLegislationParseStrategy> logger)
    {
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<ParseResult> ParseAsync(ParserMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SAIJ Legislation parse: {DocumentId}", message.DocumentId);

        await using var blobStream = await _blobStorage.DownloadAsync(message.BlobPathPdf, cancellationToken);
        var doc = await JsonSerializer.DeserializeAsync<SaijDocument>(blobStream, JsonOpts, cancellationToken);

        if (doc is null)
            throw new InvalidOperationException($"SAIJ document {message.DocumentId} deserialized as null");

        var number = doc.NroNorma ?? doc.IdNorma ?? message.DocumentId;
        var name = doc.TituloSumario ?? doc.TituloResumido ?? doc.Titulo ?? $"Norma {number}";
        var fullText = StripHtml(doc.TextoOriginal ?? doc.TextoActualizado);

        DateOnly sanctionDate = default;
        if (DateOnly.TryParse(doc.FechaSancion, out var sd))
            sanctionDate = sd;
        else if (DateOnly.TryParse(doc.FechaPublicacion, out var pd))
            sanctionDate = pd;

        var textBlobPath = BlobPathHelper.ToTextPath(message.BlobPathPdf);
        if (!string.IsNullOrWhiteSpace(fullText))
        {
            using var textStream = new MemoryStream(Encoding.UTF8.GetBytes(fullText));
            await _blobStorage.UploadAsync(textBlobPath, textStream, cancellationToken);
        }

        var metadata = new ExtractedMetadata(
            CaseTitle: name,
            RulingDate: sanctionDate,
            CaseNumber: number,
            Court: doc.OrganismoOrigen,
            JurisdictionArea: null,
            Instance: null,
            RulingDirection: null,
            Summary: doc.TituloSumario,
            Holding: null,
            Keywords: [],
            Citations: []);

        var metadataBlobPath = BlobPathHelper.ToMetadataPath(message.BlobPathPdf);
        var metadataJson = JsonSerializer.Serialize(metadata, MetadataJsonOptions);
        using (var metaStream = new MemoryStream(Encoding.UTF8.GetBytes(metadataJson)))
        {
            await _blobStorage.UploadAsync(metadataBlobPath, metaStream, cancellationToken);
        }

        var enrichmentMessage = new EnrichmentMessage(
            DocumentId: message.DocumentId,
            SourceId: message.SourceId,
            NormalizedText: "",
            ExtractedMetadata: metadata,
            MissingFields: [],
            BlobPath: message.BlobPathPdf,
            ContentHash: message.ContentHash,
            TextBlobPath: textBlobPath,
            MetadataBlobPath: metadataBlobPath,
            IngestionJobId: message.IngestionJobId,
            Reprocess: message.Reprocess,
            UseCache: message.UseCache,
            PipelineDocumentId: message.PipelineDocumentId,
            EntityType: message.EntityType);

        _logger.LogInformation("SAIJ Legislation parsed: {Name} ({Number})", name, number);
        return new ParseResult(enrichmentMessage);
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
