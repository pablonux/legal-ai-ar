# Chat Tools — Agentic Loop Architecture

| Field | Value |
|---|---|
| **ID** | E215 |
| **Feature** | F1-17 · Chat Tools — Function Calling |
| **Date** | 2026-03-20 |

---

## Purpose

This document specifies the architecture for transforming the jurisprudential chat assistant from a single-pass RAG pipeline into an agentic assistant with OpenAI function calling (tools). It covers the `IAgentChatService` contract, `IToolRegistry` pattern, tool execution context, agentic loop mechanics, SSE protocol extension, error handling, and configuration.

**Audience**: Developers implementing T-01 through T-05 of F1-17.

**Reference**: E076 (Chat RAG design), E077 (Chat flow), architecture §5 (Chat RAG flow), `AzureOpenAiChatService` implementation (`Azure.AI.OpenAI` 2.1.0).

---

## 1. Current State

The existing chat pipeline (`ChatQueryHandler`, E079–E082) operates as a single-pass RAG:

```
query → embed → dual search (chunks + rulings) → build context → GPT-4o stream → SSE
```

Key limitations:
- The model cannot request additional information. It answers exclusively from the initial top-K retrieval.
- No filters are applied during RAG search (unlike the UI search which supports `SearchFilters`).
- The model cannot navigate the citation graph, look up statute references, or aggregate data.

The current `IChatService` accepts only `systemPrompt` and `userContent` strings. `AzureOpenAiChatService` sends two `ChatMessage` items (system + user) with `Temperature = 0.3f` and `MaxOutputTokenCount = 2048`. No tool definitions are passed.

---

## 2. Target Architecture

### 2.1 Overview

```
query
  │
  ├── 1. ChatQueryHandler invokes IAgentChatService with tool definitions
  │
  ├── 2. GPT-4o receives system prompt + user query + tool schemas
  │         │
  │         ├── 2a. Model returns text → stream to client (done)
  │         │
  │         └── 2b. Model returns tool_calls → execute tools → append results → re-invoke
  │                   │
  │                   └── repeat (max N iterations) until text response
  │
  └── 3. SSE stream: text chunks + tool_start/tool_end events
```

### 2.2 Layer Responsibilities

| Layer | Component | Responsibility |
|---|---|---|
| **Core** | `IAgentChatService` | Interface for tool-aware chat streaming. Returns typed events (text, tool call, done). |
| **Core** | `IToolRegistry` | Interface for registering and resolving tool definitions + handlers. |
| **Infrastructure** | `AzureOpenAiAgentChatService` | Wraps `Azure.AI.OpenAI` `ChatClient` with tool definitions. Handles `StreamingChatToolCallUpdate` accumulation. |
| **Application** | `ChatQueryHandler` (refactored) | Orchestrates the agentic loop: invoke model → detect tool_calls → execute → re-invoke. |
| **Application** | Tool handlers | One per tool (e.g. `SearchRulingsTool`). Registered in `IToolRegistry` at startup. |
| **API** | `ChatController` (extended) | Emits `tool_start`/`tool_end` SSE events alongside text chunks. |

---

## 3. IAgentChatService

### 3.1 Interface

```csharp
public interface IAgentChatService
{
    IAsyncEnumerable<AgentChatEvent> StreamWithToolsAsync(
        IReadOnlyList<ChatMessage> messages,
        IReadOnlyList<AgentToolDefinition> tools,
        AgentChatOptions options,
        CancellationToken cancellationToken = default);
}
```

Unlike `IChatService.StreamChatAsync` (which accepts `string systemPrompt, string userContent`), this interface:
- Accepts a full `ChatMessage` list (enables multi-turn with tool results appended).
- Accepts tool definitions.
- Returns typed `AgentChatEvent` instead of raw strings.

### 3.2 AgentChatEvent

```csharp
public abstract record AgentChatEvent;

public sealed record TextChunkEvent(string Text) : AgentChatEvent;

public sealed record ToolCallRequestEvent(
    string ToolCallId,
    string ToolName,
    string ArgumentsJson) : AgentChatEvent;

public sealed record DoneEvent(
    ChatFinishReason FinishReason,
    ChatTokenUsage? Usage) : AgentChatEvent;
```

- `TextChunkEvent`: Streamed text fragment (same as current `yield return chunk`).
- `ToolCallRequestEvent`: Model requests a tool invocation. `ArgumentsJson` is the raw JSON string from the model.
- `DoneEvent`: Model finished generating. `FinishReason` is `Stop` for normal completion, `ToolCalls` when the model wants tool results.

### 3.3 AgentToolDefinition

```csharp
public sealed record AgentToolDefinition(
    string Name,
    string Description,
    BinaryData ParametersSchema);
```

Wraps the tool's name, description, and JSON schema for parameters. Maps directly to `ChatTool.CreateFunctionTool(name, description, schema)` in `Azure.AI.OpenAI` 2.1.0.

### 3.4 AgentChatOptions

```csharp
public sealed record AgentChatOptions
{
    public float Temperature { get; init; } = 0.3f;
    public int MaxOutputTokens { get; init; } = 2048;
}
```

### 3.5 Implementation Notes (AzureOpenAiAgentChatService)

The implementation wraps `ChatClient.CompleteChatStreamingAsync`:

1. Convert `AgentToolDefinition` list to `ChatTool` list via `ChatTool.CreateFunctionTool`.
2. Build `ChatCompletionOptions` with `Temperature`, `MaxOutputTokenCount`, and `Tools`.
3. Stream via `CompleteChatStreamingAsync`.
4. **Tool call accumulation**: During streaming, `StreamingChatCompletionUpdate.ToolCallUpdates` delivers tool call fragments incrementally (`Id`, `FunctionName`, `FunctionArgumentsUpdate`). The service must accumulate these fragments into complete `ToolCallRequestEvent` objects. Accumulation strategy:
   - Maintain a `Dictionary<int, (string Id, string Name, StringBuilder Args)>` keyed by tool call index.
   - On each update: append `FunctionArgumentsUpdate` to the corresponding builder.
   - On stream completion with `FinishReason.ToolCalls`: yield accumulated `ToolCallRequestEvent` for each entry, then yield `DoneEvent`.
5. If the model returns text (no tool calls): yield `TextChunkEvent` for each content update, then yield `DoneEvent` with `FinishReason.Stop`.

**Registration**: `IAgentChatService` is registered as **singleton** alongside the existing `IChatService` (both use the same `ChatClient` from `AzureOpenAiOptions`). `IChatService` remains for non-tool use cases.

---

## 4. IToolRegistry

### 4.1 Interface

```csharp
public interface IToolRegistry
{
    IReadOnlyList<AgentToolDefinition> GetToolDefinitions();
    
    Task<string> ExecuteAsync(
        string toolName,
        string argumentsJson,
        ToolExecutionContext context,
        CancellationToken cancellationToken = default);
}
```

### 4.2 ToolExecutionContext

```csharp
public sealed class ToolExecutionContext
{
    public IServiceProvider Services { get; init; } = null!;
    public HashSet<Guid> ResolvedRulingIds { get; } = new();
}
```

- `Services`: Scoped `IServiceProvider` from the HTTP request. Tools resolve their dependencies from this (e.g. `ISearchService`, `IRulingRepository`).
- `ResolvedRulingIds`: Per-request state to track already-fetched ruling IDs and avoid redundant lookups across multiple tool calls.

### 4.3 Tool Registration

```csharp
public interface IChatTool
{
    string Name { get; }
    string Description { get; }
    BinaryData ParametersSchema { get; }
    Task<string> ExecuteAsync(string argumentsJson, ToolExecutionContext context, CancellationToken ct);
}
```

Each tool implements `IChatTool`. At startup, `AddChatTools()` registers all tool implementations and builds the `IToolRegistry`:

```csharp
services.AddSingleton<IChatTool, SearchRulingsTool>();
services.AddSingleton<IChatTool, GetRulingDetailTool>();
// ... all 8 tools
services.AddSingleton<IToolRegistry>(sp =>
    new ToolRegistry(sp.GetServices<IChatTool>()));
```

`ToolRegistry` stores tool definitions and dispatches `ExecuteAsync` to the matching `IChatTool` by name.

### 4.4 Tool Result Format

Each tool returns a **string** (the result as text). This string is passed back to the model as `ChatMessage.CreateToolMessage(toolCallId, result)`. The format should be structured but human-readable (the model processes it as text):

```
[search_rulings: 5 results]

1. Case: Smith v. Jones | ID: a1b2c3d4-... | Date: 2024-03-15 | Civil / CSJN
   Summary: ...

2. Case: Doe c/ Estado | ID: b2c3d4e5-... | Date: 2023-11-20 | Penal / Cámara
   Summary: ...
```

---

## 5. Agentic Loop (ChatQueryHandler)

### 5.1 Loop Mechanics

```
1. Build initial messages: [system prompt, user query]
2. Invoke IAgentChatService.StreamWithToolsAsync(messages, tools, options)
3. Consume events:
   a. TextChunkEvent → yield to SSE stream
   b. ToolCallRequestEvent → accumulate in pending list
   c. DoneEvent:
      - If FinishReason == Stop → done, yield [DONE]
      - If FinishReason == ToolCalls:
        i.  Emit SSE tool_start for each pending tool call
        ii. Execute each tool via IToolRegistry.ExecuteAsync
        iii.Emit SSE tool_end for each completed tool
        iv. Append assistant message (with tool_calls) + tool result messages to message list
        v.  Increment iteration counter
        vi. If iterations < max → goto step 2
        vii.If iterations >= max → yield error message, done
```

### 5.2 Configuration

| Setting | Default | Config key |
|---|---|---|
| Max iterations | 5 | `ChatTools:MaxIterations` |
| Per-tool timeout | 30s | `ChatTools:ToolTimeoutSeconds` |
| Temperature | 0.3 | (inherited from current chat) |
| Max output tokens | 2048 | (inherited from current chat) |

### 5.3 System Prompt Extension

The system prompt is extended with a tools preamble:

```
{existing legal assistant prompt}

## Available tools

You have access to the following tools. Use them when you need specific information that goes beyond the initial context. Call tools when the user asks for:
- Filtered search (by date, court, jurisdiction, keywords)
- Details about a specific ruling (judges, statutes, full metadata)
- Citation relationships (what rulings cite or are cited by a ruling)
- Semantically similar rulings
- Rulings that cite a specific law or statute
- Counts or statistics about rulings
- Lists of courts or judges

Do not call tools for general questions that can be answered from the conversation context. When you receive tool results, use them to formulate your response following all existing citation rules.
```

The actual tool schemas are passed via `ChatCompletionOptions.Tools`, not in the prompt text. The preamble above guides the model's tool-calling behavior.

### 5.4 Initial Context Strategy

**Decision**: The handler no longer performs the initial dual search (embed → chunks + rulings). Instead, the model decides whether to call `search_rulings` based on the user query. This:
- Eliminates redundant searches when the model can answer from conversation context.
- Allows the model to apply filters the initial RAG pass never used.
- Reduces latency for follow-up questions that don't need new searches.

If the model does not call any tool and produces a text response directly, the response is streamed as-is. This may happen for greetings, clarifications, or when the model determines the query doesn't require jurisprudential data.

---

## 6. SSE Protocol Extension

### 6.1 Event Types

| Event type | Format | When |
|---|---|---|
| (default, no event field) | `data: {text}\n\n` | Text chunk from model |
| `tool_start` | `event: tool_start\ndata: {"tool":"name"}\n\n` | Tool execution begins |
| `tool_end` | `event: tool_end\ndata: {"tool":"name","resultCount":N}\n\n` | Tool execution completed |
| (default) | `data: [DONE]\n\n` | Stream ends |

### 6.2 Backward Compatibility

SSE clients that ignore the `event:` field will:
- Receive text chunks as before (no change).
- Receive `{"tool":"name"}` and `{"tool":"name","resultCount":N}` as `data:` events, which they can ignore or display as text. This is safe: the frontend will be updated (T-14) to parse these.
- Receive `[DONE]` as before.

### 6.3 Example Stream

```
event: tool_start
data: {"tool":"search_rulings"}

event: tool_end
data: {"tool":"search_rulings","resultCount":5}

data: Según la jurisprudencia relevante, en el fallo
data:  {caso: "Smith v. Jones", id: "a1b2c3..."}
data:  la Corte sostuvo que...
data: [DONE]
```

---

## 7. Error Handling

| Scenario | Behavior |
|---|---|
| Tool execution throws exception | Catch, return error message as tool result to model: `"Error executing {toolName}: {message}"`. Model decides how to proceed (may retry with different params or explain to user). |
| Tool execution timeout | Cancel via per-tool `CancellationTokenSource` with configured timeout. Return timeout error as tool result. |
| Max iterations reached | Append a system message: `"Maximum tool iterations reached. Provide your best answer with the information gathered so far."` Re-invoke once more with no tools. |
| Invalid tool name from model | Return error as tool result: `"Unknown tool: {name}. Available tools: ..."`. |
| Malformed tool arguments | Catch `JsonException`, return error as tool result: `"Invalid arguments for {name}: {message}"`. |
| IAgentChatService failure | Log error, return 500 ProblemDetails (same as current GPT-4o error handling). |

---

## 8. Observability

Each tool invocation is logged at `Information` level:

```
Tool invoked: {ToolName} | Args: {ArgsJson} | Duration: {ElapsedMs}ms | ResultSize: {ResultChars} chars | Iteration: {Iteration}/{MaxIterations}
```

Structured logging fields enable querying by tool name, duration, and iteration depth in Application Insights or equivalent.

---

## 9. Decisions

| # | Decision | Rationale |
|---|---|---|
| D-01 | New `IAgentChatService` interface (not extending `IChatService`) | Clean separation. `IChatService` stays simple for non-tool uses (e.g. enrichment). Tool-aware streaming has fundamentally different return types (`AgentChatEvent` vs `string`). |
| D-02 | Tools registered as singletons | Tool definitions are static (JSON schemas don't change at runtime). Tool handlers resolve scoped services from `ToolExecutionContext.Services` at execution time. |
| D-03 | No initial RAG search; model invokes `search_rulings` | Avoids redundant searches. The model can apply filters. Reduces latency for non-search queries. Trade-off: first search adds one roundtrip. Acceptable given the latency profile (~200ms for search vs ~2s total response). |
| D-04 | Tool results as plain text, not JSON | The model processes tool results as text context. Structured but readable format is more effective than raw JSON for GPT-4o reasoning. |
| D-05 | Max 5 iterations | Prevents runaway loops. Typical queries need 1–2 iterations. Cap is configurable. |

---

## 10. References

- `docs/design/f1-7-chat-rag.md` — current RAG pipeline design (E076)
- `docs/design/f1-7-chat-flow.mermaid` — current sequence diagram (E077)
- `docs/design/f1-7-rag-prompt.md` — current legal RAG prompt (E078)
- `docs/architecture/legal-ai-ar-architecture.md` — architecture §5 (Chat RAG flow)
- `AzureOpenAiChatService` — current implementation using `Azure.AI.OpenAI` 2.1.0
- `Azure.AI.OpenAI` 2.1.0 — `ChatTool.CreateFunctionTool`, `StreamingChatToolCallUpdate`, `ChatFinishReason.ToolCalls`
