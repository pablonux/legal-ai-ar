using System.Text;
using System.Text.Json;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Processes ParserMessage for CSJN: fetches API metadata, extracts PDF text from Blob,
/// builds EnrichmentMessage and publishes to {prefix}-enrichment.
/// Phase 1: CSJN (SourceId=1) only.
/// </summary>
public class ParserProcessor : IParserProcessor
{
    private const int CsjnSourceId = 1;
    private const int SaijLegislationSourceId = 2;
    private const int SaijRulingsSourceId = 3;
    private readonly CsjnApiParser _csjnApiParser;
    private readonly SaijLegislationParser _saijParser;
    private readonly SaijRulingParser _saijRulingParser;
    private readonly IBlobPdfExtractor _blobPdfExtractor;
    private readonly IBlobStorageService _blobStorage;
    private readonly IQueuePublisher _queuePublisher;
    private readonly PipelineQueueNames _queueNames;
    private readonly ILogger<ParserProcessor> _logger;

    public ParserProcessor(
        CsjnApiParser csjnApiParser,
        SaijLegislationParser saijParser,
        SaijRulingParser saijRulingParser,
        IBlobPdfExtractor blobPdfExtractor,
        IBlobStorageService blobStorage,
        IQueuePublisher queuePublisher,
        PipelineQueueNames queueNames,
        ILogger<ParserProcessor> logger)
    {
        _csjnApiParser = csjnApiParser;
        _saijParser = saijParser;
        _saijRulingParser = saijRulingParser;
        _blobPdfExtractor = blobPdfExtractor;
        _blobStorage = blobStorage;
        _queuePublisher = queuePublisher;
        _queueNames = queueNames;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ProcessAsync(ParserMessage message, CancellationToken cancellationToken = default)
    {
        if (message.SourceId == SaijLegislationSourceId)
        {
            await _saijParser.ProcessAsync(message, cancellationToken);
            return;
        }

        if (message.SourceId == SaijRulingsSourceId)
        {
            await _saijRulingParser.ProcessAsync(message, cancellationToken);
            return;
        }

        if (message.SourceId != CsjnSourceId)
        {
            throw new DomainException($"ParserProcessor supports CSJN ({CsjnSourceId}), SAIJ Legislation ({SaijLegislationSourceId}), SAIJ Rulings ({SaijRulingsSourceId}). SourceId={message.SourceId} not supported.");
        }

        if (string.IsNullOrWhiteSpace(message.AnalysisId))
        {
            throw new ArgumentException("AnalysisId is required for CSJN.", nameof(message));
        }

        _logger.LogDebug("Processing document {DocumentId} from CSJN", message.DocumentId);

        var apiMetadata = await _csjnApiParser.FetchMetadataAsync(
            message.DocumentId,
            message.AnalysisId,
            cancellationToken,
            useCache: message.UseCache,
            rulingDateHint: message.RulingDateHint,
            caseNumberHint: message.CaseNumberHint);

        var normalizedText = await _blobPdfExtractor.ExtractTextAsync(
            message.BlobPathPdf,
            cancellationToken);

        var documentType = BlobPathHelper.GetDocumentTypeFromPath(message.BlobPathPdf) ?? "ruling";
        var finalPdfPath = BlobPathHelper.BuildPdfPath(
            documentType,
            message.SourceId,
            message.DocumentId,
            apiMetadata.RulingDate);

        if (!string.Equals(message.BlobPathPdf, finalPdfPath, StringComparison.OrdinalIgnoreCase))
        {
            await _blobStorage.MoveAsync(message.BlobPathPdf, finalPdfPath, cancellationToken);
            _logger.LogInformation(
                "Moved PDF to path by document date: {DocumentId} from {From} to {To}",
                message.DocumentId, message.BlobPathPdf, finalPdfPath);
        }

        var textBlobPath = BlobPathHelper.ToTextPath(finalPdfPath);
        var extractedMetadata = CsjnMetadataMapper.ToExtractedMetadata(apiMetadata);
        var missingFields = CsjnMetadataMapper.GetMissingFields(message.SourceId, apiMetadata);

        var metadataBlobPath = BlobPathHelper.ToMetadataPath(finalPdfPath);
        var metadataJson = JsonSerializer.Serialize(extractedMetadata, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var textBytes = Encoding.UTF8.GetBytes(normalizedText);
        var metaBytes = Encoding.UTF8.GetBytes(metadataJson);
        await Task.WhenAll(
            _blobStorage.UploadAsync(textBlobPath, new MemoryStream(textBytes), cancellationToken),
            _blobStorage.UploadAsync(metadataBlobPath, new MemoryStream(metaBytes), cancellationToken));

        _logger.LogInformation("Uploaded normalized text to blob {TextBlobPath} ({Size} chars)",
            textBlobPath, normalizedText.Length);
        _logger.LogDebug("Uploaded metadata to blob {MetadataBlobPath} ({Size} bytes)",
            metadataBlobPath, metadataJson.Length);

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
            UseCache: message.UseCache);

        await _queuePublisher.PublishAsync(_queueNames.Enricher, enrichmentMessage, cancellationToken);

        _logger.LogInformation(
            "Published EnrichmentMessage for document {DocumentId} to {Queue}",
            message.DocumentId,
            _queueNames.Enricher);
    }
}
