namespace LegalAiAr.Core.Models;

/// <summary>
/// Defines a tool available to the agentic chat model. Maps to
/// <c>ChatTool.CreateFunctionTool(name, description, parametersSchema)</c>
/// in the Infrastructure layer.
/// </summary>
/// <param name="Name">Unique tool name (e.g. "search_rulings").</param>
/// <param name="Description">Human-readable description for the model.</param>
/// <param name="ParametersSchema">JSON Schema for the tool's parameters (raw JSON string).</param>
public sealed record AgentToolDefinition(
    string Name,
    string Description,
    string ParametersSchema);
