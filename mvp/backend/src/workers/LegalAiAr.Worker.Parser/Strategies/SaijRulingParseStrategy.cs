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
/// SAIJ ruling parse strategy: reads jurisprudencia JSON from blob,
/// extracts ruling metadata, uploads text + metadata blobs,
/// and produces an EnrichmentMessage.
/// SAIJ rulings are self-contained — MissingFields is empty.
/// </summary>
public sealed partial class SaijRulingParseStrategy : IParseStrategy
{
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<SaijRulingParseStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions MetadataJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SaijRulingParseStrategy(
        IBlobStorageService blobStorage,
        ILogger<SaijRulingParseStrategy> logger)
    {
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<ParseResult> ParseAsync(ParserMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SAIJ Ruling parse: {DocumentId}", message.DocumentId);

        await using var blobStream = await _blobStorage.DownloadAsync(message.BlobPathPdf, cancellationToken);
        var doc = await JsonSerializer.DeserializeAsync<SaijRulingDocument>(blobStream, JsonOpts, cancellationToken);

        if (doc is null)
            throw new InvalidOperationException($"SAIJ ruling {message.DocumentId} deserialized as null");

        var externalId = doc.IdInfojus ?? message.DocumentId;
        var caseTitle = doc.Caratula ?? doc.TituloSumario ?? $"SAIJ-{externalId}";
        var caseNumber = doc.NroExpediente ?? doc.NroInterno;
        var fullText = StripHtml(doc.TextoFallo ?? doc.TextoOriginal);
        var summary = doc.Sumario ?? doc.TituloSumario;

        DateOnly rulingDate = default;
        if (DateOnly.TryParse(doc.FechaResolucion, out var parsed))
            rulingDate = parsed;
        else if (DateOnly.TryParse(doc.FechaPublicacion, out var pub))
            rulingDate = pub;

        var textBlobPath = BlobPathHelper.ToTextPath(message.BlobPathPdf);
        if (!string.IsNullOrWhiteSpace(fullText))
        {
            using var textStream = new MemoryStream(Encoding.UTF8.GetBytes(fullText));
            await _blobStorage.UploadAsync(textBlobPath, textStream, cancellationToken);
        }

        var metadata = new ExtractedMetadata(
            CaseTitle: caseTitle,
            RulingDate: rulingDate,
            CaseNumber: caseNumber,
            Court: doc.Tribunal,
            JurisdictionArea: doc.Jurisdiccion,
            Instance: doc.Instancia,
            RulingDirection: null,
            Summary: summary,
            Holding: null,
            Keywords: [],
            Citations: [],
            ResourceType: doc.TipoRecurso);

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

        _logger.LogInformation("SAIJ Ruling parsed: {Title} ({CaseNumber})", caseTitle, caseNumber);
        return new ParseResult(enrichmentMessage);
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return null;
        var text = HtmlTagRegex().Replace(html, " ");
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

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
