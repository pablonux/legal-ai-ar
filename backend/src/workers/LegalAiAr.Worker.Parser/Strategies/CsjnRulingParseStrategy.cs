using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Worker.Parser.Parsers;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Strategies;

/// <summary>
/// CSJN ruling parse strategy: fetches metadata from 8 CSJN API endpoints,
/// extracts text from the downloaded PDF, uploads text + metadata blobs,
/// and produces an EnrichmentMessage with dynamic missing fields.
/// </summary>
public sealed class CsjnRulingParseStrategy : IParseStrategy
{
    private readonly CsjnApiParser _csjnApiParser;
    private readonly IBlobPdfExtractor _blobPdfExtractor;
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<CsjnRulingParseStrategy> _logger;

    private static readonly JsonSerializerOptions MetadataJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public CsjnRulingParseStrategy(
        CsjnApiParser csjnApiParser,
        IBlobPdfExtractor blobPdfExtractor,
        IBlobStorageService blobStorage,
        ILogger<CsjnRulingParseStrategy> logger)
    {
        _csjnApiParser = csjnApiParser;
        _blobPdfExtractor = blobPdfExtractor;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<ParseResult> ParseAsync(ParserMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message.AnalysisId))
            throw new ArgumentException("AnalysisId is required for CSJN.", nameof(message));

        _logger.LogDebug("CSJN parse: processing {DocumentId}", message.DocumentId);

        var apiMetadata = await _csjnApiParser.FetchMetadataAsync(
            message.DocumentId,
            message.AnalysisId,
            cancellationToken,
            useCache: message.UseCache,
            rulingDateHint: message.RulingDateHint,
            caseNumberHint: message.CaseNumberHint);

        var normalizedText = await _blobPdfExtractor.ExtractTextAsync(
            message.BlobPathPdf, cancellationToken);

        var documentType = BlobPathHelper.GetDocumentTypeFromPath(message.BlobPathPdf) ?? "ruling";
        var finalPdfPath = BlobPathHelper.BuildPdfPath(
            documentType, message.SourceId, message.DocumentId, apiMetadata.RulingDate);

        if (!string.Equals(message.BlobPathPdf, finalPdfPath, StringComparison.OrdinalIgnoreCase))
        {
            await _blobStorage.MoveAsync(message.BlobPathPdf, finalPdfPath, cancellationToken);
            _logger.LogInformation("Moved PDF: {From} -> {To}", message.BlobPathPdf, finalPdfPath);
        }

        var textBlobPath = BlobPathHelper.ToTextPath(finalPdfPath);
        using (var textStream = new MemoryStream(Encoding.UTF8.GetBytes(normalizedText)))
        {
            await _blobStorage.UploadAsync(textBlobPath, textStream, cancellationToken);
        }
        _logger.LogInformation("Uploaded text to {Path} ({Chars} chars)", textBlobPath, normalizedText.Length);

        var extractedMetadata = CsjnMetadataMapper.ToExtractedMetadata(apiMetadata);
        var missingFields = CsjnMetadataMapper.GetMissingFields(message.SourceId, apiMetadata);

        var metadataBlobPath = BlobPathHelper.ToMetadataPath(finalPdfPath);
        var metadataJson = JsonSerializer.Serialize(extractedMetadata, MetadataJsonOptions);
        using (var metaStream = new MemoryStream(Encoding.UTF8.GetBytes(metadataJson)))
        {
            await _blobStorage.UploadAsync(metadataBlobPath, metaStream, cancellationToken);
        }

        // EnrichmentMessage carries only lightweight metadata; full text is in blob
        var placeholder = new ExtractedMetadata(
            CaseTitle: extractedMetadata.CaseTitle,
            RulingDate: extractedMetadata.RulingDate,
            CaseNumber: extractedMetadata.CaseNumber,
            Court: extractedMetadata.Court,
            JurisdictionArea: extractedMetadata.JurisdictionArea,
            Instance: extractedMetadata.Instance,
            RulingDirection: extractedMetadata.RulingDirection,
            Summary: null,
            Holding: null,
            Keywords: [],
            Citations: []);

        var enrichmentMessage = new EnrichmentMessage(
            DocumentId: message.DocumentId,
            SourceId: message.SourceId,
            NormalizedText: "",
            ExtractedMetadata: placeholder,
            MissingFields: missingFields,
            BlobPath: finalPdfPath,
            ContentHash: message.ContentHash,
            TextBlobPath: textBlobPath,
            MetadataBlobPath: metadataBlobPath,
            IngestionJobId: message.IngestionJobId,
            AnalysisId: message.AnalysisId,
            Reprocess: message.Reprocess,
            UseCache: message.UseCache,
            PipelineDocumentId: message.PipelineDocumentId,
            EntityType: message.EntityType);

        _logger.LogInformation("CSJN parse complete for {DocumentId}, {MissingCount} missing fields",
            message.DocumentId, missingFields.Count);

        return new ParseResult(enrichmentMessage);
    }
}
