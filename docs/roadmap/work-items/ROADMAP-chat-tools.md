# ROADMAP — F1-17 · Chat Tools (Function Calling)

> **Source**: [FEATURE-chat-tools.md](./FEATURE-chat-tools.md) · 6 WIs  
> **Architect output** — ready for Manager to merge into ROADMAP.md  
> **Deliverable range**: E215–E231 (3 design + 14 development)  
> **Phase**: Phase 1 — MVP (extends F1-7 Chat RAG)

---

### F1-17 · Chat Tools — Function Calling for Jurisprudential Assistant

**Objective**: Transform the chat assistant from a single-pass RAG pipeline into an agentic assistant with OpenAI function calling. The model dynamically invokes tools (filtered search, ruling detail, citation graph, statute lookup, aggregation) to gather precise information before composing the response. Frontend shows inline tool execution feedback.

**Dependencies**: F1-7 (Chat RAG) `[x] DEV`. No Phase 2+ dependencies.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E215 | `docs/design/f1-17-chat-tools-architecture.md` — agentic loop design: `IAgentChatService` contract, `IToolRegistry` pattern, tool execution context, max-iterations cap, error handling strategy, SSE protocol extension (`tool_start`/`tool_end` events), token budget considerations | `[ ]` | `[ ]` |
| E216 | `docs/design/f1-17-agentic-loop.mermaid` — sequence diagram: user query → model invocation with tool definitions → tool_calls in stream → server-side tool execution → tool results appended → re-invoke model → repeat until text response → SSE stream to client | `[ ]` | `[ ]` |
| E217 | `docs/design/f1-17-tool-catalog.md` — complete catalog of all tools: name, description, JSON schema, backend service mapping, result format. Tools: `search_rulings`, `get_ruling_detail`, `get_ruling_citations`, `get_related_rulings`, `search_by_statute`, `count_rulings`, `list_courts`, `list_judges` | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `IAgentChatService` interface in Core with tool-aware streaming overload: accepts `IReadOnlyList<ChatTool>` definitions, returns `IAsyncEnumerable` of typed events (text chunk, tool call request, done)
- [ ] **T-02** Implement `AzureOpenAiAgentChatService` in Infrastructure: wraps `Azure.AI.OpenAI` `ChatClient` with tool definitions in `ChatCompletionOptions.Tools`, handles `StreamingChatToolCallUpdate` accumulation, returns structured events
- [ ] **T-03** Implement `IToolRegistry` and `ToolRegistration` in Application: register tool name → JSON schema + async execution handler. `ToolExecutionContext` carries `CancellationToken`, `IServiceProvider`, and per-request state
- [ ] **T-04** Implement agentic loop in refactored `ChatQueryHandler`: invoke model → if tool_calls, resolve from registry, execute tools, append tool results as `ChatMessage` with role `Tool`, re-invoke → repeat until text response or max iterations (configurable, default 5). Log each tool invocation (name, params, duration, result size)
- [ ] **T-05** Extend SSE protocol in `ChatController`: emit `event: tool_start\ndata: {"tool":"name","args":{...}}\n\n` when tool execution begins and `event: tool_end\ndata: {"tool":"name","resultCount":N}\n\n` when complete. Text chunks remain `data: {text}\n\n`. Backward compatible: clients ignoring `event:` field still work
- [ ] **T-06** Implement `search_rulings` tool: embed query via `IEmbeddingService`, build `SearchFilters` from params (dateFrom, dateTo, jurisdictionArea, instance, courtName, keywords), call `ISearchService.SearchAsync`, format top-K results as structured text with rulingId for citations
- [ ] **T-07** Implement `get_ruling_detail` tool: call `IRulingRepository.GetByIdAsync` with navigation properties, format complete metadata (court, judges with participation type, keywords, statutes with articles, summary, holding, dates, rulingDirection, isUnconstitutional)
- [ ] **T-08** Implement `get_ruling_citations` tool: call `IGraphService.GetInboundCitationsAsync` and/or `GetOutboundCitationsAsync` based on `direction` param, resolve target ruling metadata, format as list with caseTitle, rulingId, date, citationType, externalAlias
- [ ] **T-09** Implement `get_related_rulings` tool: call `ISearchService.SearchRelatedAsync`, format results with caseTitle, rulingId, date, court, summary, score
- [ ] **T-10** Implement `search_by_statute` tool: add `FindRulingsByStatuteAsync(statuteName, statuteNumber?, articles?, topK)` to `IStatuteRepository` (joins Statutes → RulingStatutes → Rulings with LIKE matching), implement tool handler that calls it and formats results
- [ ] **T-11** Implement `count_rulings` tool: add `CountAsync(filters)` to `IRulingRepository` using SQL COUNT(*) with same filter logic as search (jurisdictionArea, instance, courtName, dateFrom, dateTo, isUnconstitutional, keywords via RulingKeywords join). Tool returns `{"count": N, "filters_applied": {...}}`
- [ ] **T-12** Implement `list_courts` and `list_judges` tools: `list_courts` queries Courts table with optional jurisdictionArea/instance filters (cap 50). `list_judges` queries Judges + RulingJudges with optional courtName filter, returns ordered by ruling count desc (cap 50)
- [ ] **T-13** Update system prompt: describe available tools, when to use each, maintain existing citation format `{caso: "...", id: "..."}` and Spanish language rules. Instruct model to prefer `search_rulings` over relying on initial context when user asks for filtered/specific results
- [ ] **T-14** Frontend: extend `ChatService` SSE parsing to detect `event:` field in SSE stream. Route `tool_start`/`tool_end` events to a new observable or callback. Default (no event field) remains text chunk. Backward compatible with non-tool responses
- [ ] **T-15** Frontend: implement `ToolStatusChipComponent` — inline chip with spinner during execution, checkmark on completion, warning on error. Spanish labels per tool name (e.g. `search_rulings` → "Buscando jurisprudencia...")
- [ ] **T-16** Frontend: integrate `ToolStatusChipComponent` in `ChatViewComponent` — chips appear inline in message flow between text blocks. Multiple sequential tools show multiple chips. Responsive for mobile
- [ ] **T-17** Write unit tests: tool registry registration and resolution, each tool handler with mocked services (ISearchService, IRulingRepository, IGraphService, IStatuteRepository), agentic loop with simulated tool_calls, SSE event serialization

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E218 | `IAgentChatService` interface in Core + `AzureOpenAiAgentChatService` implementation handling tool definitions and `StreamingChatToolCallUpdate` accumulation | `[ ]` | `[ ]` |
| E219 | `IToolRegistry` with `ToolRegistration` and `ToolExecutionContext` — tools registered at startup, resolved and executed at runtime | `[ ]` | `[ ]` |
| E220 | Agentic loop in `ChatQueryHandler`: tool_calls → execute → re-invoke cycle with configurable max-iteration cap (default 5) and per-tool logging | `[ ]` | `[ ]` |
| E221 | SSE protocol extended: `ChatController` emits `tool_start`/`tool_end` typed events. Backward compatible with existing clients | `[ ]` | `[ ]` |
| E222 | `search_rulings` tool: filtered hybrid search via `ISearchService.SearchAsync` + `IEmbeddingService`, results formatted with rulingId for model citations | `[ ]` | `[ ]` |
| E223 | `get_ruling_detail` tool: full ruling metadata via `IRulingRepository.GetByIdAsync` including judges, keywords, statutes, citations | `[ ]` | `[ ]` |
| E224 | `get_ruling_citations` tool: inbound/outbound citation traversal via `IGraphService` with resolved target metadata | `[ ]` | `[ ]` |
| E225 | `get_related_rulings` tool: semantic similarity via `ISearchService.SearchRelatedAsync` | `[ ]` | `[ ]` |
| E226 | `search_by_statute` tool + `IStatuteRepository.FindRulingsByStatuteAsync` with LIKE matching on statute name/number and optional article filter | `[ ]` | `[ ]` |
| E227 | `count_rulings` tool + `IRulingRepository.CountAsync` with SQL COUNT and filter logic. `list_courts` and `list_judges` tools with capped results | `[ ]` | `[ ]` |
| E228 | System prompt updated with tool descriptions, usage instructions, and citation format preservation | `[ ]` | `[ ]` |
| E229 | Frontend `ChatService` parsing `tool_start`/`tool_end` SSE events with backward compatibility | `[ ]` | `[ ]` |
| E230 | `ToolStatusChipComponent` with spinner, completion state, and Spanish labels rendered inline in `ChatViewComponent` | `[ ]` | `[ ]` |
| E231 | Unit tests: tool registry, all 8 tool handlers with mocked services, agentic loop, SSE event serialization | `[ ]` | `[ ]` |

---

### Implementation sequence

Tasks have the following dependency chain:

```
T-00 (design — gate)
  │
  ├── T-01 + T-02 (IAgentChatService interface + implementation)
  │     │
  │     └── T-03 (IToolRegistry)
  │           │
  │           ├── T-04 (agentic loop in ChatQueryHandler)
  │           │     │
  │           │     └── T-05 (SSE protocol extension)
  │           │           │
  │           │           ├── T-14 (frontend SSE parsing)
  │           │           │     └── T-15 + T-16 (ToolStatusChip + integration)
  │           │           │
  │           │           └── T-13 (system prompt update)
  │           │
  │           ├── T-06 (search_rulings) ─┐
  │           ├── T-07 (get_ruling_detail) │
  │           ├── T-08 (get_ruling_citations) ├── can be parallelized
  │           ├── T-09 (get_related_rulings) │
  │           ├── T-10 (search_by_statute) │
  │           ├── T-11 (count_rulings) │
  │           └── T-12 (list_courts/judges) ─┘
  │
  └── T-17 (unit tests — after all tools implemented)
```

**Recommended execution order**:

1. **Wave 1** (foundation): T-00 → T-01 + T-02 → T-03 → T-04 → T-05
2. **Wave 2** (tools — parallelizable): T-06, T-07, T-08, T-09, T-10, T-11, T-12
3. **Wave 3** (integration): T-13 + T-14 → T-15 + T-16
4. **Wave 4** (quality): T-17

### Summary

| Metric | Value |
|--------|-------|
| Design deliverables | 3 (E215–E217) |
| Development deliverables | 14 (E218–E231) |
| Total deliverables | 17 |
| Development tasks | 17 (T-01 to T-17) |
| New interfaces in Core | 2 (`IAgentChatService`, `IToolRegistry`) |
| New repository methods | 3 (`FindRulingsByStatuteAsync`, `CountAsync`, list queries) |
| New Infrastructure service | 1 (`AzureOpenAiAgentChatService`) |
| New frontend components | 1 (`ToolStatusChipComponent`) |
| Schema changes | 0 |
| Tool count | 8 |
