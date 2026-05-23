using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Pre-stream stage: classifies user queries using a two-layer approach
/// (rule-based fast path + optional LLM classifier). Non-legal queries are
/// short-circuited with a rejection template.
/// </summary>
public sealed class InputGuardrailStage : IChatPipelineStage
{
    private readonly RuleBasedGuardrailClassifier _ruleClassifier;
    private readonly IGuardrailClassifier? _llmClassifier;
    private readonly IGuardrailTemplateProvider _templates;
    private readonly ILogger<InputGuardrailStage> _logger;

    public InputGuardrailStage(
        RuleBasedGuardrailClassifier ruleClassifier,
        IGuardrailTemplateProvider templates,
        ILogger<InputGuardrailStage> logger,
        IGuardrailClassifier? llmClassifier = null)
    {
        _ruleClassifier = ruleClassifier;
        _templates = templates;
        _logger = logger;
        _llmClassifier = llmClassifier;
    }

    public string Name => "InputGuardrail";
    public ChatPipelinePhase Phase => ChatPipelinePhase.PreStream;

    public bool IsEnabled(ChatPipelineOptions options) =>
        options.InputGuardrail.Enabled;

    public async Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context, CancellationToken cancellationToken = default)
    {
        var query = context.OriginalQuery;

        // Layer 1: Rule-based (< 10ms)
        var classification = await _ruleClassifier.ClassifyAsync(query, cancellationToken);

        _logger.LogDebug(
            "Rule-based classification: {Category} (confidence: {Confidence})",
            classification.Category, classification.Confidence);

        // If rules returned null confidence → inconclusive, try LLM (when enabled)
        if (classification.Confidence is null
            && _llmClassifier is not null
            && context.Options.InputGuardrail.UseLlmClassifier)
        {
            try
            {
                classification = await _llmClassifier.ClassifyAsync(query, cancellationToken);
                _logger.LogDebug(
                    "LLM classification: {Category} (confidence: {Confidence})",
                    classification.Category, classification.Confidence);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "LLM classifier failed, falling back to rule-based result");
            }
        }

        context.InputClassification = classification;
        context.ClassifiedIntent = classification.Category.ToString();

        if (classification.Category == GuardrailCategory.LegalQuery)
            return ChatPipelineResult.Continue();

        // Non-legal: short-circuit with rejection template
        var template = _templates.GetTemplate(classification.Category);
        if (string.IsNullOrEmpty(template))
            return ChatPipelineResult.Continue();

        _logger.LogInformation(
            "Query rejected by input guardrail: {Category} (source: {Source}, confidence: {Confidence})",
            classification.Category, classification.Source, classification.Confidence);

        return ChatPipelineResult.ShortCircuit(new ChatTextChunk(template));
    }
}
