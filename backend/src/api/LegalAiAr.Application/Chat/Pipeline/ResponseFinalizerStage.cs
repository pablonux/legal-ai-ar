using System.Text.RegularExpressions;
using LegalAiAr.Application.Chat.Utilities;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Post-stream stage: normalizes citations, cleans up markdown, handles empty responses,
/// and injects legal disclaimer when appropriate. Fail-open.
/// </summary>
public sealed class ResponseFinalizerStage : IChatPipelineStage
{
    private static readonly Regex ExcessiveNewlines = new(@"\n{4,}", RegexOptions.Compiled);
    private static readonly Regex OrphanHeading = new(@"^#{1,3}\s*$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex UnclosedBold = new(@"\*\*(?![*\s])([^*]+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    private const string EmptyResponseFallback =
        "No pude generar una respuesta. Por favor intentá reformular tu consulta.";

    private readonly ChatPipelineOptions _options;
    private readonly ILogger<ResponseFinalizerStage> _logger;

    public ResponseFinalizerStage(
        IOptions<ChatPipelineOptions> options,
        ILogger<ResponseFinalizerStage> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string Name => "ResponseFinalizer";
    public ChatPipelinePhase Phase => ChatPipelinePhase.PostStream;

    public bool IsEnabled(ChatPipelineOptions options) =>
        options.ResponseFinalizer.Enabled;

    public Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context, CancellationToken cancellationToken = default)
    {
        var responseText = context.AccumulatedResponse.ToString();
        var events = new List<ChatStreamEvent>();

        if (string.IsNullOrWhiteSpace(responseText))
        {
            _logger.LogWarning("Empty response detected, injecting fallback");
            events.Add(new ChatTextChunk(EmptyResponseFallback));
            return Task.FromResult(ChatPipelineResult.Append(events.ToArray()));
        }

        var normalized = CitationParser.Normalize(responseText);
        var cleaned = CleanMarkdown(normalized, _options.ResponseFinalizer.StructureEnforcement);

        if (cleaned != responseText)
        {
            _logger.LogDebug("Response normalized: {OriginalLen} → {CleanedLen} chars",
                responseText.Length, cleaned.Length);
            events.Add(new ChatNormalizedResponse(cleaned));
        }

        if (context.NeedsDisclaimer && _options.ResponseFinalizer.DisclaimerEnabled)
        {
            events.Add(new ChatTextChunk(
                "\n\n---\n*" + _options.ResponseFinalizer.DisclaimerText + "*"));
            _logger.LogDebug("Legal disclaimer injected");
        }

        return Task.FromResult(events.Count > 0
            ? ChatPipelineResult.Append(events.ToArray())
            : ChatPipelineResult.Continue());
    }

    private static string CleanMarkdown(string text, bool structureEnforcement)
    {
        var result = ExcessiveNewlines.Replace(text, "\n\n\n");
        result = OrphanHeading.Replace(result, "");
        if (structureEnforcement)
            result = UnclosedBold.Replace(result, "**$1**");
        return result;
    }
}
