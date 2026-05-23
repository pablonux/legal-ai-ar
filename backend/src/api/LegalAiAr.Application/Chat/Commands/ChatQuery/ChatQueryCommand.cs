using LegalAiAr.Core.Models;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Chat.Commands.ChatQuery;

/// <summary>
/// Command for agentic jurisprudential chat. Streams typed events via SSE.
/// </summary>
/// <param name="Query">User's legal question. Max 1000 chars.</param>
/// <param name="PipelineMessages">Additional messages injected by pipeline stages (e.g. query enrichment).</param>
/// <param name="PipelineToolContext">Shared tool context from the pipeline orchestrator for cross-stage state.</param>
public record ChatQueryCommand(
    string Query,
    IReadOnlyList<AgentChatMessage>? PipelineMessages = null,
    ToolExecutionContext? PipelineToolContext = null
) : IStreamRequest<ChatStreamEvent>;
