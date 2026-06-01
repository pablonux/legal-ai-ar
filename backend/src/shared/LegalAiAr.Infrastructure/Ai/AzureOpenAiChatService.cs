using System.ClientModel;
using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// RAG chat service for jurisprudential Q&amp;A. Uses the ChatDeploymentName model (GPT-5).
/// </summary>
public class AzureOpenAiChatService : IChatService
{
    private const int DefaultMaxOutputTokens = 2048;
    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiChatService> _logger;

    public AzureOpenAiChatService(IOptions<AzureOpenAiOptions> options, ILogger<AzureOpenAiChatService> logger)
    {
        _logger = logger;
        var opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.Endpoint))
            throw new InvalidOperationException("AzureOpenAI:Endpoint is required.");
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
            throw new InvalidOperationException("AzureOpenAI:ApiKey is required.");
        if (string.IsNullOrWhiteSpace(opts.ChatDeploymentName))
            throw new InvalidOperationException("AzureOpenAI:ChatDeploymentName is required.");

        var credential = new ApiKeyCredential(opts.ApiKey);
        var azureClient = new AzureOpenAIClient(new Uri(opts.Endpoint.TrimEnd('/') + "/"), credential);
        _chatClient = azureClient.GetChatClient(opts.ChatDeploymentName);
    }

    /// <inheritdoc />
    public async Task<string> ChatAsync(
        string systemPrompt,
        string userContent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentException("System prompt cannot be null or empty.", nameof(systemPrompt));
        if (string.IsNullOrWhiteSpace(userContent))
            throw new ArgumentException("User content cannot be null or empty.", nameof(userContent));

        var options = new ChatCompletionOptions
        {
            Temperature = 0.3f,
            MaxOutputTokenCount = DefaultMaxOutputTokens
        };

        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage(userContent)
        };

        _logger.LogDebug("Calling chat model for RAG chat");

        var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);

        var contentPart = response.Value.Content.FirstOrDefault();
        var text = contentPart?.Text?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            _logger.LogWarning("Chat model returned empty content for RAG chat");
            throw new InvalidOperationException("Azure OpenAI returned empty content.");
        }

        return text;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> StreamChatAsync(
        string systemPrompt,
        string userContent,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentException("System prompt cannot be null or empty.", nameof(systemPrompt));
        if (string.IsNullOrWhiteSpace(userContent))
            throw new ArgumentException("User content cannot be null or empty.", nameof(userContent));

        var options = new ChatCompletionOptions
        {
            Temperature = 0.3f,
            MaxOutputTokenCount = DefaultMaxOutputTokens
        };

        var messages = new ChatMessage[]
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage(userContent)
        };

        _logger.LogDebug("Calling chat model for RAG chat (streaming)");

        await foreach (var update in _chatClient.CompleteChatStreamingAsync(messages, options, cancellationToken))
        {
            foreach (var content in update.ContentUpdate)
            {
                var text = content?.Text;
                if (!string.IsNullOrEmpty(text))
                    yield return text;
            }
        }
    }
}
