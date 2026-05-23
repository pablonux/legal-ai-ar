using System.ClientModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Uses GPT-4o-mini to expand a user search query into keyword-optimized and
/// semantic-optimized variants for hybrid legal search in Argentine jurisprudence.
/// When thesaurus context is available, injects SAIJ descriptors and relations
/// so the LLM can produce more precise expansions.
/// </summary>
public sealed class AzureOpenAiSearchQueryPreprocessor : ISearchQueryPreprocessor
{
    private const string BaseSystemPrompt = """
        Sos un experto en derecho argentino. Tu tarea es reformular la consulta de búsqueda de un usuario para mejorar los resultados en un buscador de jurisprudencia argentina.

        Devolvé un JSON con dos campos:
        - "keywordQuery": la consulta expandida con sinónimos legales y términos relacionados, separados por espacios. NO uses operadores booleanos. Incluí abreviaturas comunes y variantes (ej: si dice "despido", agregá "cesantía distracto desvinculación"). Mantené breve (máx 30 palabras).
        - "semanticQuery": una oración descriptiva en español que capture la intención de búsqueda con contexto jurídico. Esto se usará para generar un embedding, así que debe ser natural y contextual (máx 50 palabras).

        Si la consulta ya es clara y específica, devolvé el texto original con mínimas modificaciones.
        Respondé SOLO con el JSON, sin explicación.
        """;

    private const string ThesaurusInstructions = """

        A continuación se proporcionan descriptores del Tesauro SAIJ de Derecho Argentino que son relevantes para la consulta del usuario. Usá esta información para:
        1. Incluir los sinónimos del tesauro en keywordQuery (son equivalencias oficiales del sistema jurídico argentino).
        2. Usar los términos genéricos (TG) y específicos (TE) para enriquecer la semanticQuery con contexto jerárquico.
        3. Considerar los términos relacionados (TR) como expansiones temáticas relevantes.
        Priorizá los términos del tesauro sobre sinónimos inventados.

        """;

    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiSearchQueryPreprocessor> _logger;

    public AzureOpenAiSearchQueryPreprocessor(
        IOptions<AzureOpenAiOptions> options,
        ILogger<AzureOpenAiSearchQueryPreprocessor> logger)
    {
        _logger = logger;
        var opts = options.Value;
        var client = new AzureOpenAIClient(
            new Uri(opts.Endpoint),
            new ApiKeyCredential(opts.ApiKey));
        _chatClient = client.GetChatClient(opts.NanoDeploymentName);
    }

    public async Task<PreprocessedQuery?> PreprocessAsync(string rawQuery, string? thesaurusContext = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatOptions = new ChatCompletionOptions
            {
                Temperature = 0f,
                MaxOutputTokenCount = 300,
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
            };

            var systemPrompt = string.IsNullOrWhiteSpace(thesaurusContext)
                ? BaseSystemPrompt
                : BaseSystemPrompt + ThesaurusInstructions + thesaurusContext;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(rawQuery)
            };

            var result = await _chatClient.CompleteChatAsync(messages, chatOptions, cancellationToken);
            var json = result.Value.Content[0].Text.Trim();

            _logger.LogDebug("Search query preprocessing result: {Json}", json);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var keywordQuery = root.TryGetProperty("keywordQuery", out var kw)
                ? kw.GetString()
                : null;
            var semanticQuery = root.TryGetProperty("semanticQuery", out var sq)
                ? sq.GetString()
                : null;

            if (string.IsNullOrWhiteSpace(keywordQuery) || string.IsNullOrWhiteSpace(semanticQuery))
            {
                _logger.LogWarning("LLM returned empty keyword or semantic query, falling back to raw query");
                return null;
            }

            return new PreprocessedQuery(keywordQuery, semanticQuery);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Search query preprocessing failed, falling back to raw query");
            return null;
        }
    }
}
