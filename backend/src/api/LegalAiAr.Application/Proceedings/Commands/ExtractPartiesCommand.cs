using System.Text.Json;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record ExtractPartiesCommand(int BatchSize = 100) : IRequest<ExtractPartiesResult>;

public record ExtractPartiesResult(
    int ProceedingsProcessed,
    int PartiesCreated,
    int HeuristicExtractions,
    int LlmExtractions,
    int Skipped,
    int Failed);

public class ExtractPartiesHandler
    : IRequestHandler<ExtractPartiesCommand, ExtractPartiesResult>
{
    private readonly IJudicialProceedingRepository _proceedings;
    private readonly IPersonRepository _persons;
    private readonly IEnrichmentService _enrichment;
    private readonly ILogger<ExtractPartiesHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExtractPartiesHandler(
        IJudicialProceedingRepository proceedings,
        IPersonRepository persons,
        IEnrichmentService enrichment,
        ILogger<ExtractPartiesHandler> logger)
    {
        _proceedings = proceedings;
        _persons = persons;
        _enrichment = enrichment;
        _logger = logger;
    }

    public async Task<ExtractPartiesResult> Handle(
        ExtractPartiesCommand request, CancellationToken cancellationToken)
    {
        int processed = 0, partiesCreated = 0, heuristic = 0, llm = 0, skipped = 0, failed = 0;
        int lastId = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _proceedings.GetProceedingsWithoutPartiesAsync(
                lastId, request.BatchSize, cancellationToken);

            if (batch.Count == 0) break;

            foreach (var proc in batch)
            {
                lastId = proc.Id;
                processed++;

                var caseTitle = proc.DisplayName ?? "";
                if (string.IsNullOrWhiteSpace(caseTitle))
                {
                    skipped++;
                    continue;
                }

                try
                {
                    var parties = TryHeuristicExtraction(caseTitle);
                    if (parties.Count > 0)
                    {
                        heuristic++;
                    }
                    else
                    {
                        parties = await TryLlmExtraction(caseTitle, cancellationToken);
                        if (parties.Count > 0) llm++;
                        else { skipped++; continue; }
                    }

                    foreach (var party in parties)
                    {
                        var personType = party.PersonType switch
                        {
                            "Legal" => PersonType.LegalPrivate,
                            "State" => PersonType.StateEntity,
                            _ => PersonType.Physical
                        };

                        var person = await _persons.GetOrCreateAsync(
                            party.Name, cancellationToken: cancellationToken);

                        if (person.PersonType == PersonType.Physical && personType != PersonType.Physical)
                        {
                            person.PersonType = personType;
                        }

                        var role = Enum.TryParse<PartyRole>(party.Role, true, out var parsed)
                            ? parsed
                            : PartyRole.PLAINTIFF;

                        await _proceedings.AddPartyIfNotExistsAsync(
                            proc.Id, person.Id, role, cancellationToken);
                        partiesCreated++;
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogWarning(ex,
                        "Failed to extract parties for proceeding {ProceedingId} ({CaseTitle})",
                        proc.Id, caseTitle);
                }
            }

            _logger.LogInformation(
                "Party extraction progress: {Processed} proceedings, {Parties} parties, {Heuristic} heuristic, {Llm} LLM, {Skipped} skipped, {Failed} failed",
                processed, partiesCreated, heuristic, llm, skipped, failed);
        }

        return new ExtractPartiesResult(processed, partiesCreated, heuristic, llm, skipped, failed);
    }

    private static List<PartyExtractionDto> TryHeuristicExtraction(string caseTitle)
    {
        var result = new List<PartyExtractionDto>();

        var heuristicParties = HeuristicCaratulaParser.ExtractParties(caseTitle);
        foreach (var p in heuristicParties)
        {
            result.Add(new PartyExtractionDto(p.Name, p.PartyRole, p.PersonType));
        }

        return result;
    }

    private async Task<List<PartyExtractionDto>> TryLlmExtraction(
        string caseTitle, CancellationToken cancellationToken)
    {
        var json = await _enrichment.GetStructuredOutputAsync(
            CaratulaExtractionPrompt.SystemPrompt,
            CaratulaExtractionPrompt.BuildUserPrompt(caseTitle),
            CaratulaExtractionPrompt.SchemaName,
            CaratulaExtractionPrompt.JsonSchema,
            cancellationToken);

        var result = JsonSerializer.Deserialize<CaratulaExtractionResponse>(json, JsonOptions);
        if (result?.Parties is null or { Count: 0 })
            return [];

        return result.Parties;
    }

    private sealed record CaratulaExtractionResponse(List<PartyExtractionDto>? Parties);
    internal record PartyExtractionDto(string Name, string Role, string PersonType);
}

/// <summary>
/// Mirrors Worker.Enrichment CaratulaParser for use in Application layer.
/// </summary>
internal static class HeuristicCaratulaParser
{
    public static List<(string Name, string PartyRole, string PersonType)> ExtractParties(string caseTitle)
    {
        var result = new List<(string, string, string)>();

        var match = System.Text.RegularExpressions.Regex.Match(caseTitle,
            @"^(?<plaintiff>.+?)\s+(?:c[/.]|contra)\s+(?<defendant>.+?)(?:\s+s[/.]|$)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |
            System.Text.RegularExpressions.RegexOptions.Singleline);

        if (!match.Success)
            return result;

        var plaintiff = CleanName(match.Groups["plaintiff"].Value);
        var defendant = CleanName(match.Groups["defendant"].Value);

        if (plaintiff.Length >= 2)
            result.Add((plaintiff, "PLAINTIFF", ClassifyType(plaintiff)));
        if (defendant.Length >= 2)
            result.Add((defendant, "DEFENDANT", ClassifyType(defendant)));

        return result;
    }

    private static string CleanName(string raw) =>
        System.Text.RegularExpressions.Regex.Replace(raw.Trim().TrimEnd('.', ',', ';', '-'), @"\s{2,}", " ").Trim();

    private static string ClassifyType(string name) =>
        System.Text.RegularExpressions.Regex.IsMatch(name,
            @"(?:S\.?A\.?|S\.?R\.?L\.?|Estado|Gobierno|Municipalidad|Provincia|Nación|AFIP|ANSES|BCRA|Ministerio)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase) ? "Legal" : "Physical";
}

internal static class CaratulaExtractionPrompt
{
    public const string SchemaName = "caratula_parties";

    public const string SystemPrompt = """
        Sos un analista jurídico argentino. Dada la carátula (título) de un expediente judicial, extraé las partes procesales.

        La carátula típica tiene el formato "Actor c/ Demandado s/ materia", pero puede variar.

        Para cada parte, determiná:
        - name: nombre completo de la parte (persona o entidad).
        - role: "PLAINTIFF" (actor/recurrente/querellante) o "DEFENDANT" (demandado/recurrido/imputado).
        - personType: "Physical" (persona humana), "Legal" (empresa/asociación), o "State" (Estado/organismo público).

        Si la carátula no permite identificar partes (ej. "Acordada 15/2024"), retorná parties como array vacío.
        No inventes partes que no estén en el texto.
        """;

    public static string BuildUserPrompt(string caseTitle) =>
        $"Extraé las partes procesales de la siguiente carátula:\n\n\"{caseTitle}\"";

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "parties": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "name": { "type": "string" },
                  "role": { "type": "string", "enum": ["PLAINTIFF", "DEFENDANT", "THIRD_PARTY"] },
                  "personType": { "type": "string", "enum": ["Physical", "Legal", "State"] }
                },
                "required": ["name", "role", "personType"],
                "additionalProperties": false
              }
            }
          },
          "required": ["parties"],
          "additionalProperties": false
        }
        """;
}
