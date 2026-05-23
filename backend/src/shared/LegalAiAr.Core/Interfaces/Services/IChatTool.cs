using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Contract for a single tool available to the agentic chat model.
/// Each implementation is registered as a singleton; scoped dependencies are resolved
/// from <see cref="ToolExecutionContext.Services"/> at execution time.
/// </summary>
public interface IChatTool
{
    string Name { get; }
    string Description { get; }
    string ParametersSchema { get; }
    Task<string> ExecuteAsync(string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken);
}
