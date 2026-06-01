using System.Text.Json;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record ExtractDoctrineCommand(int BatchSize = 50) : IRequest<ExtractDoctrineResult>;
public record ExtractDoctrineResult(int Processed, int Extracted, int Skipped, int Failed);

public class ExtractDoctrineHandler
    : IRequestHandler<ExtractDoctrineCommand, ExtractDoctrineResult>
{
    private readonly IRulingRepository _rulings;
    private readonly IEnrichmentService _enrichment;
    private readonly ILogger<ExtractDoctrineHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExtractDoctrineHandler(
        IRulingRepository rulings,
        IEnrichmentService enrichment,
        ILogger<ExtractDoctrineHandler> logger)
    {
        _rulings = rulings;
        _enrichment = enrichment;
        _logger = logger;
    }

    public async Task<ExtractDoctrineResult> Handle(
        ExtractDoctrineCommand request, CancellationToken cancellationToken)
    {
        int processed = 0, extracted = 0, skipped = 0, failed = 0;
        var lastId = Guid.Empty;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _rulings.GetRulingsWithoutDoctrineAsync(
                lastId, request.BatchSize, cancellationToken);

            if (batch.Count == 0) break;

            foreach (var ruling in batch)
            {
                lastId = ruling.Id;
                processed++;

                if (string.IsNullOrWhiteSpace(ruling.FullText) &&
                    string.IsNullOrWhiteSpace(ruling.Summary) &&
                    string.IsNullOrWhiteSpace(ruling.Holding))
                {
                    skipped++;
                    continue;
                }

                try
                {
                    var userPrompt = DoctrineExtractionPrompt.BuildUserPrompt(
                        ruling.FullText, ruling.Summary, ruling.Holding, ruling.CaseTitle);
                    var json = await _enrichment.GetStructuredOutputAsync(
                        DoctrineExtractionPrompt.SystemPrompt,
                        userPrompt,
                        DoctrineExtractionPrompt.SchemaName,
                        DoctrineExtractionPrompt.JsonSchema,
                        cancellationToken);

                    var result = JsonSerializer.Deserialize<DoctrineExtractionResult>(json, JsonOptions);
                    if (result is null ||
                        (string.IsNullOrWhiteSpace(result.RatioDecidendi) &&
                         string.IsNullOrWhiteSpace(result.DoctrinaLegal)))
                    {
                        skipped++;
                        continue;
                    }

                    await _rulings.UpdateDoctrineFieldsAsync(
                        ruling.Id,
                        Truncate(result.RatioDecidendi, 8000),
                        Truncate(result.DoctrinaLegal, 4000),
                        cancellationToken);
                    extracted++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogWarning(ex, "Failed to extract doctrine for ruling {RulingId}", ruling.Id);
                }
            }

            _logger.LogInformation(
                "Doctrine extraction progress: {Processed} processed, {Extracted} extracted, {Skipped} skipped, {Failed} failed",
                processed, extracted, skipped, failed);
        }

        return new ExtractDoctrineResult(processed, extracted, skipped, failed);
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is { Length: > 0 } ? value[..Math.Min(value.Length, maxLength)] : value;

    private sealed record DoctrineExtractionResult(
        string? RatioDecidendi,
        string? DoctrinaLegal);
}

internal static class DoctrineExtractionPrompt
{
    public const string SchemaName = "doctrine_extraction";

    public const string SystemPrompt = """
        Sos un analista de jurisprudencia argentina especializado en doctrina judicial. Dado el texto de un fallo judicial, extraé:

        1. ratioDecidendi: La fundamentación esencial que sostiene la decisión del tribunal. Incluí los argumentos jurídicos centrales, las normas invocadas y el razonamiento lógico-jurídico que conduce al fallo. Extensión: 1 a 3 párrafos.

        2. doctrinaLegal: El criterio jurídico que el fallo sienta para casos futuros. Formulalo como una regla general y abstracta, en 1 a 2 oraciones, como si fuera una entrada de un repertorio de jurisprudencia. Ejemplo: "La tenencia de estupefacientes para consumo personal no es punible cuando no afecta derechos de terceros."

        Guías:
        - Si el fallo no contiene argumentación sustantiva (ej. resoluciones de trámite, inadmisibilidades sin fundamento), retorná ambos campos como string vacío.
        - Usá lenguaje técnico jurídico preciso.
        - No transcribas literalmente; sintetizá manteniendo la esencia.
        - Si hay disidencias, enfocate en la posición mayoritaria.
        """;

    public static string BuildUserPrompt(
        string? fullText, string? summary, string? holding, string caseTitle)
    {
        var parts = new List<string> { $"Carátula: {caseTitle}" };

        if (!string.IsNullOrWhiteSpace(holding))
            parts.Add($"Resolución (holding):\n{holding}");

        if (!string.IsNullOrWhiteSpace(summary))
            parts.Add($"Resumen:\n{Truncate(summary, 3000)}");

        if (!string.IsNullOrWhiteSpace(fullText))
            parts.Add($"Texto del fallo:\n---\n{Truncate(fullText, 10000)}\n---");

        return $"""
            Analizá el siguiente fallo judicial argentino y extraé ratioDecidendi y doctrinaLegal según el schema.

            {string.Join("\n\n", parts)}
            """;
    }

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "ratioDecidendi": { "type": "string" },
            "doctrinaLegal": { "type": "string" }
          },
          "required": ["ratioDecidendi", "doctrinaLegal"],
          "additionalProperties": false
        }
        """;

    private static string Truncate(string? text, int maxChars)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxChars)
            return text ?? "";
        return text[..maxChars] + "\n\n[... truncado ...]";
    }
}
