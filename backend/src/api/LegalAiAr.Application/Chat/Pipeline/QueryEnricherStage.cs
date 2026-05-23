using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Pre-stream stage: classifies intent and extracts entities from the user query.
/// Rule-based first; LLM fallback for ambiguous queries. Fail-open: on failure,
/// the pipeline proceeds with the raw query.
/// </summary>
public sealed class QueryEnricherStage : IChatPipelineStage
{
    private const float LlmFallbackThreshold = 0.6f;

    private readonly RuleBasedQueryEnricher _ruleEnricher;
    private readonly OntologyContextProvider _ontologyProvider;
    private readonly IQueryEnricher? _llmEnricher;
    private readonly ILogger<QueryEnricherStage> _logger;

    public QueryEnricherStage(
        RuleBasedQueryEnricher ruleEnricher,
        OntologyContextProvider ontologyProvider,
        ILogger<QueryEnricherStage> logger,
        IQueryEnricher? llmEnricher = null)
    {
        _ruleEnricher = ruleEnricher;
        _ontologyProvider = ontologyProvider;
        _logger = logger;
        _llmEnricher = llmEnricher;
    }

    public string Name => "QueryEnricher";
    public ChatPipelinePhase Phase => ChatPipelinePhase.PreStream;

    public bool IsEnabled(ChatPipelineOptions options) =>
        options.QueryEnricher.Enabled;

    public async Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context, CancellationToken cancellationToken = default)
    {
        var query = context.OriginalQuery;

        // Layer 1: Rule-based intent + entity extraction
        var (intent, confidence) = _ruleEnricher.ClassifyIntent(query);
        var enrichment = _ruleEnricher.ExtractEntities(query, intent, confidence);

        _logger.LogDebug(
            "Rule-based enrichment: intent={Intent}, confidence={Confidence}, entities={HasEntities}",
            intent, confidence, enrichment is not null);

        // Layer 2: LLM fallback for low-confidence or inconclusive results (when enabled)
        if ((confidence is null || confidence < LlmFallbackThreshold)
            && _llmEnricher is not null
            && context.Options.QueryEnricher.UseLlmFallback)
        {
            try
            {
                var llmEnrichment = await _llmEnricher.EnrichAsync(query, cancellationToken);
                if (llmEnrichment is not null)
                {
                    enrichment = llmEnrichment;
                    _logger.LogDebug(
                        "LLM enrichment: intent={Intent}, confidence={Confidence}",
                        llmEnrichment.Intent, llmEnrichment.Confidence);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "LLM enricher failed, proceeding with rule-based result");
            }
        }

        if (enrichment is not null)
        {
            context.Enrichment = enrichment;
            context.ClassifiedIntent ??= enrichment.Intent;

            var systemMessage = enrichment.ToSystemMessage();
            context.Messages.Add(AgentChatMessage.System(systemMessage));

            _logger.LogInformation(
                "Query enriched: intent={Intent}, source={Source}, courts={Courts}, statutes={Statutes}, temporal={Temporal}",
                enrichment.Intent, enrichment.Source,
                enrichment.Courts.Count, enrichment.Statutes.Count, enrichment.Temporal.Count);
        }
        else
        {
            _logger.LogDebug("No enrichment produced for query");
        }

        var reasoningFlow = _ontologyProvider.DetectReasoningFlow(query);
        if (reasoningFlow is not null)
        {
            var (flowId, label, instructions) = reasoningFlow.Value;
            context.Messages.Add(AgentChatMessage.System(
                $"[Legal reasoning flow: {label}]\n{instructions}"));
            _logger.LogInformation(
                "Ontology reasoning flow injected: {FlowId} ({Label})", flowId, label);
        }

        return ChatPipelineResult.Continue();
    }
}
