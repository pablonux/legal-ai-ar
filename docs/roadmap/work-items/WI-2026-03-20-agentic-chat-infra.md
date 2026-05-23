# Work Item — Agentic Chat Infrastructure

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **developer**, I want **the chat pipeline to support an agentic loop with OpenAI function calling** so that **the assistant can dynamically invoke tools based on user queries instead of relying on a single RAG pass**.

## Context

- **Current state**: `ChatQueryHandler` performs one embedding + dual vector search → builds static context → streams GPT-4o response. `AzureOpenAiChatService.StreamChatAsync` sends system+user messages only, no tool definitions. `IChatService` has no tool-aware overloads.
- **Target state**: An agentic loop where GPT-4o receives tool definitions, can emit `tool_calls` in the streaming response, the backend executes those tools against existing services (`ISearchService`, `IRulingRepository`, `IGraphService`, etc.), appends results as tool messages, and re-invokes the model — repeating until the model produces a final text response streamed to the client.
- **Azure OpenAI SDK**: `Azure.AI.OpenAI` already supports `ChatTool`, `ChatToolCall`, and `ChatMessageContentPart` for function calling. No new SDK dependency needed.
- **Streaming**: The SSE contract with the frontend (`data: {text}\n\n` / `data: [DONE]\n\n`) must remain compatible. Tool execution happens server-side; the client sees text chunks and optional status events.

## Acceptance Criteria

1. **`IChatService` extension**: New overload or new interface `IAgentChatService` that accepts `IReadOnlyList<ChatTool>` definitions and supports the agentic loop (tool_calls → execute → re-invoke).
2. **Tool registry**: A mechanism to register tool definitions and their execution handlers (e.g. `IToolRegistry` with `RegisterTool(name, schema, handler)`). Each tool has a JSON schema for parameters and an async execution function.
3. **Agentic loop**: The handler iterates: stream response → if tool_calls, execute tools, append tool results, re-invoke model → repeat until final text response. Max iterations capped (e.g. 5) to prevent runaway loops.
4. **Streaming adaptation**: During tool execution, emit SSE status events (e.g. `event: tool_start\ndata: {"tool":"search_rulings"}\n\n`) so the frontend can show progress. Final text chunks continue as `data: {text}\n\n`.
5. **`ChatQueryHandler` refactor**: Replace the current single-pass RAG with the agentic loop. The initial RAG context injection becomes optional or is replaced by the model invoking `search_rulings` as its first tool call.
6. **System prompt update**: Extend the system prompt to describe available tools and when to use them, maintaining the existing citation format and language rules.
7. **Error handling**: Tool execution failures are returned to the model as error messages (not thrown), letting the model decide how to proceed.
8. **Logging**: Log each tool invocation (name, parameters, duration, result size) for observability.

## Out of Scope

- Individual tool implementations (separate WIs)
- Multi-turn conversation history / memory
- Token budget management across tool iterations (deferred optimization)

## Technical Notes

- The `Azure.AI.OpenAI` SDK `ChatCompletionOptions.Tools` accepts `ChatTool.CreateFunctionTool(name, description, schema)`. Tool calls arrive as `ChatToolCall` in `StreamingChatCompletionUpdate.ToolCallUpdates`.
- Consider a `ToolExecutionContext` that carries `CancellationToken`, `IServiceProvider`, and per-request state (e.g. already-fetched ruling IDs to avoid redundant lookups).
- The max-iterations cap and per-tool timeout are configurable via `appsettings.json`.
