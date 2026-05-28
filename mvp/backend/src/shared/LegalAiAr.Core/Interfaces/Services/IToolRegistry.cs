using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Registry of all tools available to the agentic chat model.
/// Provides tool definitions for the model and dispatches execution to the matching <see cref="IChatTool"/>.
/// </summary>
public interface IToolRegistry
{
    IReadOnlyList<AgentToolDefinition> GetToolDefinitions();

    Task<string> ExecuteAsync(
        string toolName,
        string argumentsJson,
        ToolExecutionContext context,
        CancellationToken cancellationToken = default);
}
