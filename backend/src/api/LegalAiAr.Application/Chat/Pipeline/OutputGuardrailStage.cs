using LegalAiAr.Application.Chat.Utilities;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Post-stream stage: validates citations in the accumulated response against the database,
/// checks tool-grounding, emits a validation SSE event, and appends warning text when issues
/// are detected. Fail-closed: on validation failure, warnings are appended.
/// </summary>
public sealed class OutputGuardrailStage : IChatPipelineStage
{
    private const string SomeCitationsWarning =
        "\n\n---\n⚠️ *Nota: Algunas referencias en esta respuesta no pudieron ser verificadas " +
        "contra la base de datos de fallos. Recomendamos verificar los fallos citados accediendo " +
        "directamente a sus detalles.*";

    private const string AllCitationsWarning =
        "\n\n---\n⚠️ *Advertencia: Las referencias citadas en esta respuesta no pudieron ser verificadas. " +
        "La información puede no ser precisa. Recomendamos realizar una búsqueda directa para " +
        "confirmar los fallos mencionados.*";

    private readonly ILogger<OutputGuardrailStage> _logger;

    public OutputGuardrailStage(ILogger<OutputGuardrailStage> logger)
    {
        _logger = logger;
    }

    public string Name => "OutputGuardrail";
    public ChatPipelinePhase Phase => ChatPipelinePhase.PostStream;

    public bool IsEnabled(ChatPipelineOptions options) =>
        options.OutputGuardrail.Enabled;

    public async Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context, CancellationToken cancellationToken = default)
    {
        var responseText = context.AccumulatedResponse.ToString();
        var citations = CitationParser.Parse(responseText);

        if (citations.Count == 0)
        {
            _logger.LogDebug("No citations found in response, skipping validation");
            return ChatPipelineResult.Continue();
        }

        var rulingIds = citations.Select(c => c.RulingId).Distinct().ToList();
        var rulingRepository = context.ToolContext.Services.GetRequiredService<IRulingRepository>();

        var knownRulings = await rulingRepository.GetCaseTitlesByIdsAsync(rulingIds, cancellationToken);
        var resolvedIds = context.ToolContext.ResolvedRulingIds;

        var validations = citations
            .Select(c => CitationParser.Validate(c, knownRulings, resolvedIds))
            .ToList();

        var criticalCount = validations.Count(v => v.Severity == CitationSeverity.Critical);
        var warningCount = validations.Count(v => v.Severity == CitationSeverity.Warning);
        var validCount = validations.Count(v => v.Severity == CitationSeverity.None);

        foreach (var v in validations.Where(v => v.Issues.Count > 0))
            _logger.LogWarning("Citation validation issue: {Issues}", string.Join("; ", v.Issues));

        var status = criticalCount > 0 || warningCount > 0 ? "warnings" : "passed";
        var details = validations
            .SelectMany(v => v.Issues)
            .ToList();

        var validationEvent = new ChatValidationEvent(
            status, citations.Count, validCount, criticalCount + warningCount, details);

        context.NeedsDisclaimer = responseText.Length > 200 && citations.Count > 0;

        var events = new List<ChatStreamEvent> { validationEvent };

        var strictness = GetStrictness(context);
        if (criticalCount > 0 && strictness != "lenient")
        {
            var warningText = criticalCount == citations.Count
                ? AllCitationsWarning : SomeCitationsWarning;
            events.Add(new ChatTextChunk(warningText));
        }

        _logger.LogInformation(
            "Citation validation: {Total} checked, {Valid} valid, {Critical} critical, {Warnings} warnings",
            citations.Count, validCount, criticalCount, warningCount);

        return ChatPipelineResult.Append(events.ToArray());
    }

    private static string GetStrictness(ChatPipelineContext context) =>
        context.Options.OutputGuardrail.Strictness;
}
