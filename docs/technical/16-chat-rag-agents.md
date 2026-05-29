# Chat, RAG & Agent Pipeline

> How the jurisprudential **chat** works end to end: a tool-calling **agentic RAG** loop wrapped by a
> guardrail pipeline, streamed to the SPA over Server-Sent Events.
>
> This document describes the chat as currently implemented. Class names follow the current code. The
> agent **system prompt** is kept in Spanish (it is end-user-facing contact content).

---

## 1. Overview

A user question is answered by an **agentic loop**: the model (Azure OpenAI chat deployment) is given a
catalog of tools, decides which to call, the backend executes them against the Knowledge Base, and the
model composes a cited answer from the tool results. Retrieval is therefore **tool-driven RAG** — there
is no fixed "retrieve-then-generate" step; the model retrieves on demand.

The loop is wrapped by a configurable **pipeline** of guardrail/enrichment stages and streamed to the
client as it is produced.

> **Single agent today; multi-agent is the target.** The current implementation is **one** tool-calling
> agent with 13 tools. The roadmap (R2.0) introduces specialized agents (Regulatory / Case Law /
> Procedural) orchestrated by a router on Semantic Kernel; the tool catalog and pipeline below are the
> foundation for that evolution.

---

## 2. API & streaming

**`POST /api/chat`** (`ChatController`) with body `{ "query": "..." }` returns a
`text/event-stream`. Event types:

| SSE | Payload | Meaning |
|-----|---------|---------|
| `data: {text}` | text fragment (newlines re-emitted as `\ndata:`) | A streamed chunk of the answer |
| `event: tool_start` | `{ "tool": "search_rulings" }` | The agent began executing a tool |
| `event: tool_end` | `{ "tool": "...", "resultCount": N }` | Tool finished |
| `event: validation` | `{ "status","citationsChecked","valid","warnings","details":[] }` | Output guardrail result |
| `event: normalized` | text | Finalized/normalized answer |
| `data: [DONE]` | — | Stream complete |

Streaming uses the mediator's `CreateStream`; the SPA consumes events with `withCredentials`.

---

## 3. Pipeline architecture

`ChatPipelineOrchestrator` (an `IStreamRequestHandler`) wraps the agentic executor with stages that run
in three phases (`ChatPipelinePhase`). With no stages enabled it degrades to a transparent
pass-through.

| Phase | When | Stages (current) |
|-------|------|------------------|
| **PreStream** | Before the agentic loop; may short-circuit | `InputGuardrailStage`, `QueryEnricherStage` |
| **ChunkMode** | Inline on each stream event | *(reserved; none enabled by default)* |
| **PostStream** | After the loop, on the accumulated answer | `OutputGuardrailStage`, `ResponseFinalizerStage` |

- **`InputGuardrailStage`** — classifies the query with a **two-layer** approach (rule-based fast path
  + optional LLM classifier). Non-legal queries are short-circuited with a rejection template
  (`GuardrailTemplateProvider`, `RuleBasedGuardrailClassifier`). Fail-closed.
- **`QueryEnricherStage`** — intent classification + entity extraction (rule-based first, LLM fallback
  for ambiguous queries via `RuleBasedQueryEnricher` + `OntologyContextProvider`). **Fail-open**: on
  error the raw query proceeds.
- **`OutputGuardrailStage`** — validates the citations in the accumulated answer **against the
  database**, checks tool-grounding, emits the `validation` SSE event, and appends warning text when
  issues are found. **Fail-closed**.
- **`ResponseFinalizerStage`** — normalizes citations, cleans up Markdown, handles empty responses, and
  injects the legal **disclaimer**. Fail-open.

Each stage is independently toggled via the `ChatPipeline` config section.

---

## 4. The agentic executor

`ChatQueryHandler` runs the loop:

1. Seed the conversation with the **system prompt** (legal-assistant instructions) + the user query.
2. Get the tool definitions from `IToolRegistry` and call
   `IAgentChatService.StreamWithToolsAsync(messages, tools, options)`.
3. Stream `AgentChatEvent`s: `TextChunkEvent` (forwarded as answer text), `ToolCallRequestEvent`
   (buffered), `DoneEvent` (carries `AgentFinishReason` + `AgentTokenUsage`).
4. If the model finished with `ToolCalls`: append the assistant tool-call message, **execute each tool**
   (`ToolRegistry.ExecuteAsync`, per-tool timeout 30 s), append tool-result messages, and re-invoke.
5. Repeat until the model returns text (`Stop`) or **`MaxIterations`** (default 5) is reached.

`AzureOpenAiAgentChatService` wraps the Azure OpenAI `ChatClient` (the configured chat deployment —
GPT-4o / GPT-5o per environment) with **function calling**, accumulating streamed tool-call deltas and
emitting the typed events above. Model options: `Temperature` 0.3, `MaxOutputTokens` 2048.

**System-prompt rules (excerpt, Spanish — end-user contact layer):** the agent must always use
`search_rulings` for jurisprudential questions, must **not** invent information, must cite rulings with
the exact format `{caso: "Título", id: "uuid"}` using the verbatim title/id from tool results, mention
the Procurador's *dictamen* and the official reference (e.g. `Fallos: 340:1256`) when present, and
always close with: *"Esta información es de carácter referencial y no constituye asesoramiento legal."*

---

## 5. Tool catalog

Tools implement `IChatTool` (`Name`, `Description`, `ParametersSchema` JSON, `ExecuteAsync`) and are
registered in `ToolRegistry`:

| Tool | Purpose |
|------|---------|
| `search_rulings` | **Primary** — hybrid semantic search over rulings with filters (court, date, jurisdiction, keywords) |
| `search_chunks` | Hybrid (semantic + text) search over ruling **passages**; optional `rulingId` scope — for exact reasoning/quotes |
| `search_by_statute` | Rulings that cite a given law/norm (name, number, articles) |
| `search_communities` | Thematic **graph communities** (clusters with generated summaries) — for global/trend questions |
| `count_rulings` | Count rulings matching filters (quantitative questions) |
| `get_ruling_detail` | Full metadata of a ruling (judges, statutes, keywords, sumario, holding) |
| `get_ruling_citations` | Citation relations (cites / cited-by), multi-hop via `depth` |
| `get_related_rulings` | Semantically similar rulings |
| `get_case_history` | Instance chain of a case file (confirmed/overturned on appeal) |
| `explore_graph` | Full graph context of a ruling (persons, keywords, statutes, court, citations) |
| `find_connection` | Shortest citation path between two rulings |
| `list_courts` / `list_persons` | Enumerate courts / participants in the KB |

The system prompt encodes a **tool-use strategy** (e.g. global/thematic → `search_communities` then
`search_rulings`; exact arguments → `search_chunks`; precedent chains → `get_ruling_citations` /
`get_related_rulings`; tools can be chained).

---

## 6. Retrieval & RAG

Retrieval is performed by the tools, combining the system's retrieval surfaces:

- **Hybrid search** (`search_rulings`, `search_chunks`) — BM25 + 3072-dim vectors over the
  `rulings-by-ruling` / `rulings-by-chunk` Azure AI Search indexes (see
  [01 — RAG & Retrieval](01-rag-retrieval.md) and [14 — CSJN Ruling Ingestion §3.6](14-csjn-ruling-ingestion.md)).
- **GraphRAG — global** (`search_communities`) — community detection clusters with LLM summaries for
  high-level/thematic questions.
- **GraphRAG — local / graph traversal** (`explore_graph`, `find_connection`, `get_ruling_citations`) —
  over the SQL legal graph (citations, participations, statutes, `ChunkEntityMention`).

Because retrieval is tool-driven, the model can **chain** surfaces (e.g. find rulings, then pull exact
passages from the most relevant ones), and answers are grounded in returned data only.

---

## 7. Guardrails & citation integrity

Two layers protect quality (see also [06 — AI Security & Compliance](06-ai-security-compliance.md)):

- **Input** — rule-based + optional LLM classifier reject out-of-scope queries before any model/tool
  cost is incurred.
- **Output** — citations in the answer are parsed (`CitationParser`) and validated **against the
  database**; tool-grounding is checked; a `validation` event reports `citationsChecked` / `valid` /
  `warnings`, and warnings are appended to the answer. The finalizer then normalizes citations and adds
  the disclaimer.

---

## 8. Configuration

| Section | Key | Default | Purpose |
|---------|-----|---------|---------|
| `ChatTools` | `MaxIterations` | 5 | Max agentic loop iterations |
| `ChatTools` | `ToolTimeoutSeconds` | 30 | Per-tool execution timeout |
| `ChatPipeline` | `InputGuardrail.Enabled` / `UseLlmClassifier` | true / true | Input guardrail + LLM layer |
| `ChatPipeline` | `QueryEnricher.Enabled` / `UseLlmFallback` | true / true | Query enrichment |
| `ChatPipeline` | `OutputGuardrail.Enabled` / `Strictness` | true / `moderate` | Citation validation |
| `ChatPipeline` | `ResponseFinalizer.Enabled` / `DisclaimerEnabled` / `StructureEnforcement` | true / true / false | Finalization |
| `AzureOpenAI` | `ChatDeploymentName` | — | Chat model deployment |

---

## 9. Related documentation

- [01 — RAG & Retrieval](01-rag-retrieval.md) — hybrid search, GraphRAG, contextual retrieval
- [02 — Agentic Architecture](02-agentic-architecture.md) — target multi-agent design (router + specialized agents)
- [03 — Prompt Engineering](03-prompt-engineering.md) — prompt management and structured output
- [08 — Legal AI UX](08-legal-ai-ux.md) — streaming, inline citation, feedback in the chat UI
- [14 — CSJN Ruling Ingestion](14-csjn-ruling-ingestion.md) — how the searched rulings get indexed

---

*Chat, RAG & Agent Pipeline — Legal Ai Ar*
