using System.Text.Json;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record ExtractProsecutorOpinionsCommand(int BatchSize = 50) : IRequest<ExtractProsecutorOpinionsResult>;
public record ExtractProsecutorOpinionsResult(int Processed, int Extracted, int Skipped, int Failed);

public class ExtractProsecutorOpinionsHandler
    : IRequestHandler<ExtractProsecutorOpinionsCommand, ExtractProsecutorOpinionsResult>
{
    private readonly IRulingRepository _rulings;
    private readonly IEnrichmentService _enrichment;
    private readonly ILogger<ExtractProsecutorOpinionsHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExtractProsecutorOpinionsHandler(
        IRulingRepository rulings,
        IEnrichmentService enrichment,
        ILogger<ExtractProsecutorOpinionsHandler> logger)
    {
        _rulings = rulings;
        _enrichment = enrichment;
        _logger = logger;
    }

    public async Task<ExtractProsecutorOpinionsResult> Handle(
        ExtractProsecutorOpinionsCommand request, CancellationToken cancellationToken)
    {
        int processed = 0, extracted = 0, skipped = 0, failed = 0;
        var lastId = Guid.Empty;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _rulings.GetCsjnRulingsWithoutProsecutorOpinionAsync(
                lastId, request.BatchSize, cancellationToken);

            if (batch.Count == 0) break;

            foreach (var ruling in batch)
            {
                lastId = ruling.Id;
                processed++;

                try
                {
                    var userPrompt = ProsecutorOpinionPromptBuilder.BuildUserPrompt(ruling.FullText!);
                    var json = await _enrichment.GetStructuredOutputAsync(
                        ProsecutorOpinionPromptBuilder.SystemPrompt,
                        userPrompt,
                        ProsecutorOpinionPromptBuilder.SchemaName,
                        ProsecutorOpinionPromptBuilder.JsonSchema,
                        cancellationToken);

                    var result = JsonSerializer.Deserialize<ProsecutorExtractionResult>(json, JsonOptions);
                    if (result is null || !result.HasDictamen)
                    {
                        skipped++;
                        continue;
                    }

                    var opinion = new ProsecutorOpinion
                    {
                        RulingId = ruling.Id,
                        ProsecutorName = Truncate(result.ProsecutorName ?? "", 200) ?? string.Empty,
                        Summary = Truncate(result.Summary, 4000),
                        RecommendedDirection = Truncate(result.RecommendedDirection, 500),
                        AgreedWithCourt = result.AgreedWithCourt,
                        ExtractedAt = DateTime.UtcNow
                    };

                    await _rulings.AddProsecutorOpinionAsync(opinion, cancellationToken);
                    extracted++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogWarning(ex, "Failed to extract prosecutor opinion for ruling {RulingId}", ruling.Id);
                }
            }

            _logger.LogInformation(
                "Prosecutor extraction progress: {Processed} processed, {Extracted} extracted, {Skipped} skipped, {Failed} failed",
                processed, extracted, skipped, failed);
        }

        return new ExtractProsecutorOpinionsResult(processed, extracted, skipped, failed);
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is { Length: > 0 } ? value[..Math.Min(value.Length, maxLength)] : value;

    private sealed record ProsecutorExtractionResult(
        bool HasDictamen,
        string? ProsecutorName,
        string? Summary,
        string? RecommendedDirection,
        bool AgreedWithCourt = false);
}

/// <summary>
/// Prompt builder for prosecutor opinion extraction — mirrors Worker.Enrichment prompt
/// but is available in the Application layer without referencing the Worker project.
/// </summary>
internal static class ProsecutorOpinionPromptBuilder
{
    public const string SchemaName = "prosecutor_opinion";

    public const string SystemPrompt = """
        Sos un analista de documentos jurídicos argentinos. Dado el texto de un fallo de la CSJN, determiná si contiene un dictamen del Procurador General / Fiscal de la Nación. Si existe, extraé la información solicitada. Si el texto no contiene dictamen del Procurador, retorná hasDictamen: false y dejá los demás campos vacíos.

        Guías:
        - El dictamen suele aparecer al inicio del texto, antes del "Y VISTOS" o "AUTOS Y VISTOS".
        - Buscá frases como "El Procurador General", "El señor Procurador Fiscal", "Dictamen del Procurador", "A fs. ... obra el dictamen".
        - prosecutorName: nombre completo del procurador/fiscal.
        - summary: resumen de 2-3 oraciones de la opinión del procurador.
        - recommendedDirection: qué recomendó (ej. "hacer lugar al recurso", "rechazar la queja", "declarar procedente").
        - agreedWithCourt: true si la recomendación del procurador coincide con la decisión final de la Corte (misma dirección: ambos hacen lugar, ambos rechazan, etc.), false si difieren o no se puede determinar.
        - Si no podés determinar algún campo con certeza, dejalo como string vacío (o false para agreedWithCourt).
        """;

    public static string BuildUserPrompt(string normalizedText)
    {
        var truncated = normalizedText.Length <= 12000
            ? normalizedText
            : normalizedText[..12000] + "\n\n[... truncado ...]";
        return $"""
            Analizá el siguiente texto de un fallo de la CSJN y determiná si contiene un dictamen del Procurador General / Fiscal. Retorná un objeto JSON según el schema.

            Texto del fallo:
            ---
            {truncated}
            ---
            """;
    }

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "hasDictamen": { "type": "boolean" },
            "prosecutorName": { "type": "string" },
            "summary": { "type": "string" },
            "recommendedDirection": { "type": "string" },
            "agreedWithCourt": { "type": "boolean" }
          },
          "required": ["hasDictamen", "prosecutorName", "summary", "recommendedDirection", "agreedWithCourt"],
          "additionalProperties": false
        }
        """;
}
