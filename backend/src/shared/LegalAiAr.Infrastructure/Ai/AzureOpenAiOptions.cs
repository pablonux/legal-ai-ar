namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Configuration options for Azure OpenAI Service.
/// </summary>
public class AzureOpenAiOptions
{
    public const string SectionName = "AzureOpenAI";

    public string Endpoint { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Embeddings model (text-embedding-3-large, 3072 dims).
    /// </summary>
    public string EmbeddingDeploymentName { get; set; } = "azure.text-embedding-3-large";

    /// <summary>
    /// Heavy model for user-facing chat and agentic tool-calling (GPT-5).
    /// </summary>
    public string ChatDeploymentName { get; set; } = "azure.gpt-5";

    /// <summary>
    /// Mid-tier model reserved for quality-sensitive tasks (GPT-5-mini).
    /// </summary>
    public string MiniDeploymentName { get; set; } = "azure.gpt-5-mini";

    /// <summary>
    /// Lightweight model for structured extraction, guardrail classification
    /// and query preprocessing (GPT-5-nano).
    /// </summary>
    public string NanoDeploymentName { get; set; } = "azure.gpt-5-nano";
}
