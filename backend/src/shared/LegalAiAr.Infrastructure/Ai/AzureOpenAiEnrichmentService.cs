using System.ClientModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Calls GPT-5-nano for structured extraction (judges, statutes, prosecutor opinion).
/// Uses json_schema response_format for deterministic, parseable output.
/// </summary>
public class AzureOpenAiEnrichmentService : IEnrichmentService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiEnrichmentService> _logger;

    public AzureOpenAiEnrichmentService(IOptions<AzureOpenAiOptions> options, ILogger<AzureOpenAiEnrichmentService> logger)
    {
        _logger = logger;
        var opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.Endpoint))
            throw new InvalidOperationException("AzureOpenAI:Endpoint is required.");
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
            throw new InvalidOperationException("AzureOpenAI:ApiKey is required.");
        if (string.IsNullOrWhiteSpace(opts.NanoDeploymentName))
            throw new InvalidOperationException("AzureOpenAI:NanoDeploymentName is required.");

        var credential = new ApiKeyCredential(opts.ApiKey);
        var azureClient = new AzureOpenAIClient(new Uri(opts.Endpoint.TrimEnd('/') + "/"), credential);
        _chatClient = azureClient.GetChatClient(opts.NanoDeploymentName);
    }

    /// <inheritdoc />
    public async Task<string> GetStructuredOutputAsync(
        string systemPrompt,
        string userContent,
        string schemaName,
        string jsonSchema,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentException("System prompt cannot be null or empty.", nameof(systemPrompt));
        if (string.IsNullOrWhiteSpace(userContent))
            throw new ArgumentException("User content cannot be null or empty.", nameof(userContent));
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Schema name cannot be null or empty.", nameof(schemaName));
        if (string.IsNullOrWhiteSpace(jsonSchema))
            throw new ArgumentException("JSON schema cannot be null or empty.", nameof(jsonSchema));

        var responseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            jsonSchemaFormatName: schemaName,
            jsonSchema: BinaryData.FromString(jsonSchema),
            jsonSchemaIsStrict: true);

        var options = new ChatCompletionOptions
        {
            ResponseFormat = responseFormat,
            Temperature = 0f
        };

        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage(userContent)
        };

        _logger.LogDebug("Calling nano model for structured output with schema {SchemaName}", schemaName);

        var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);

        var contentPart = response.Value.Content.FirstOrDefault();
        var text = contentPart?.Text?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            _logger.LogWarning("Nano model returned empty content for schema {SchemaName}", schemaName);
            throw new InvalidOperationException("Azure OpenAI returned empty content.");
        }

        // Validate that the response is valid JSON
        using var _ = JsonDocument.Parse(text);

        return text;
    }
}
