using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Stores tool definitions and dispatches execution to the matching <see cref="IChatTool"/> by name.
/// Built at startup from all registered <see cref="IChatTool"/> implementations.
/// </summary>
public sealed class ToolRegistry : IToolRegistry
{
    private readonly IReadOnlyDictionary<string, IChatTool> _tools;
    private readonly IReadOnlyList<AgentToolDefinition> _definitions;
    private readonly ILogger<ToolRegistry> _logger;

    public ToolRegistry(IEnumerable<IChatTool> tools, ILogger<ToolRegistry> logger)
    {
        _logger = logger;
        var toolList = tools.ToList();
        _tools = toolList.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        _definitions = toolList
            .Select(t => new AgentToolDefinition(t.Name, t.Description, t.ParametersSchema))
            .ToList();

        _logger.LogInformation("ToolRegistry initialized with {Count} tools: {Names}",
            _definitions.Count, string.Join(", ", _tools.Keys));
    }

    public IReadOnlyList<AgentToolDefinition> GetToolDefinitions() => _definitions;

    public async Task<string> ExecuteAsync(
        string toolName,
        string argumentsJson,
        ToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_tools.TryGetValue(toolName, out var tool))
        {
            _logger.LogWarning("Unknown tool requested: {ToolName}", toolName);
            return $"Error: Unknown tool '{toolName}'. Available tools: {string.Join(", ", _tools.Keys)}";
        }

        try
        {
            return await tool.ExecuteAsync(argumentsJson, context, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tool {ToolName} execution failed", toolName);
            return $"Error executing {toolName}: {ex.Message}";
        }
    }
}
