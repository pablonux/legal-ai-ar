using System.ClientModel;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Layer 2 classifier: uses GPT-5-nano to classify ambiguous queries
/// that the rule-based classifier couldn't resolve with high confidence.
/// </summary>
public sealed class AzureOpenAiGuardrailClassifier : IGuardrailClassifier
{
    private const string ClassificationPrompt = """
        Classify the following user query for a legal jurisprudence assistant specialized in Argentine case law.
        Respond with ONLY one of: legal_query, greeting, clarification, out_of_scope, harmful

        Query: {0}
        """;

    private static readonly Dictionary<string, GuardrailCategory> CategoryMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["legal_query"] = GuardrailCategory.LegalQuery,
        ["greeting"] = GuardrailCategory.Greeting,
        ["clarification"] = GuardrailCategory.Clarification,
        ["out_of_scope"] = GuardrailCategory.OutOfScope,
        ["harmful"] = GuardrailCategory.Harmful,
    };

    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiGuardrailClassifier> _logger;

    public AzureOpenAiGuardrailClassifier(
        IOptions<AzureOpenAiOptions> options,
        ILogger<AzureOpenAiGuardrailClassifier> logger)
    {
        _logger = logger;
        var opts = options.Value;
        var client = new AzureOpenAIClient(
            new Uri(opts.Endpoint),
            new System.ClientModel.ApiKeyCredential(opts.ApiKey));
        _chatClient = client.GetChatClient(opts.NanoDeploymentName);
    }

    public async Task<GuardrailClassification> ClassifyAsync(
        string query, CancellationToken cancellationToken = default)
    {
        var prompt = string.Format(ClassificationPrompt, query);
        var chatOptions = new ChatCompletionOptions
        {
            Temperature = 0f,
            MaxOutputTokenCount = 10,
        };

        var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

        ClientResult<ChatCompletion> result = await _chatClient.CompleteChatAsync(
            messages, chatOptions, cancellationToken);

        var responseText = result.Value.Content[0].Text.Trim().ToLowerInvariant();

        _logger.LogDebug("Nano guardrail response: {Response}", responseText);

        if (CategoryMap.TryGetValue(responseText, out var category))
            return new GuardrailClassification(category, GuardrailSource.LlmClassifier, 0.80f);

        _logger.LogWarning("Unexpected guardrail classification response: {Response}", responseText);
        return new GuardrailClassification(GuardrailCategory.OutOfScope, GuardrailSource.LlmClassifier, 0.50f);
    }
}
