# Architect Output — Chat Pipeline (Multi-Stage Architecture)

**Date**: 2026-03-20
**Feature**: F1-18
**Phase**: Phase 1 — MVP
**Source WIs**: WI-2026-03-20-chat-input-guardrail, WI-2026-03-20-chat-query-enricher, WI-2026-03-20-chat-output-guardrail, WI-2026-03-20-chat-response-finalizer
**Deliverable range**: E232–E249
**Prerequisite**: F1-17 closed (agentic loop + tools)

> **Note**: Moved from Phase 3 (F3-6) to Phase 1 (F1-18) per user request on 2026-03-20.

---

## Instructions for Manager

1. Update ROADMAP.md deliverable numbering line:

```
**Deliverable numbering**: canonical range E001–E206, E207–E214 (F0-3), E215–E231 (F1-17), E232–E249 (F3-6). Sequential without gaps or duplicates.
```

2. Insert the following section **after F3-5 · Authentication — Entra ID** and **before Phase 4 — Full coverage**.

3. Update Summary table: Phase 3 adds +3 design deliverables and +15 development deliverables (18 total).

---

## Roadmap Section to Merge

```markdown
---

### F3-6 · Chat Pipeline — Multi-Stage Architecture

**Objective**: Wrap the agentic chat executor (F1-17) with safety and quality layers: input moderation, query enrichment, output validation and response normalization. Transform the single-stage agentic loop into a professional-grade legal assistant pipeline with guardrails, citation verification and legal disclaimers.

**Prerequisite**: F1-17 closed.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E232 | `docs/design/f3-6-chat-pipeline-architecture.md` — pipeline stages design: `IChatPipelineStage` interface, `ChatPipelineOrchestrator`, stage ordering, fail-open vs fail-closed semantics, SSE streaming integration (pre-stream / post-stream / chunk-mode), `ChatPipeline` configuration model, GPT-4o-mini deployment decision | `[ ]` | `[ ]` |
| E233 | `docs/design/f3-6-chat-pipeline-flow.mermaid` — sequence diagram: user query → input guardrail (classify) → query enricher (intent + entities) → agentic executor (F1-17 loop) → output guardrail (citation validation) → response finalizer (normalize + disclaimer) → SSE stream → client | `[ ]` | `[ ]` |
| E234 | `docs/design/f3-6-guardrail-catalog.md` — input guardrail classification categories (`legal_query`, `greeting`, `clarification`, `out_of_scope`, `harmful`), prompt injection patterns, PII patterns, rejection response templates (Spanish), query enricher intent taxonomy (`search`, `detail`, `comparison`, `statistics`, `precedent_exploration`, `statute_research`, `general`), entity categories, output guardrail validation rules and severity levels | `[ ]` | `[ ]` |

#### Development tasks

**Pipeline infrastructure**

- [x] **T-01** Implement `IChatPipelineStage` interface in Core with `ProcessAsync` contract. Implement `ChatPipelineOrchestrator` in Application that chains stages around the existing `ChatQueryHandler` agentic loop. Add `ChatPipelineOptions` configuration model with per-stage enabled/disabled toggles. Register pipeline in DI (`Program.cs`)

**Input guardrail (WI: chat-input-guardrail)**

- [x] **T-02** Implement `InputGuardrailStage`: rule-based fast layer (regex patterns for prompt injection detection, PII patterns for DNI/CUIT, scope keywords for legal domain), LLM classifier layer (`IGuardrailClassifier` interface in Core + `AzureOpenAiGuardrailClassifier` in Infrastructure using GPT-4o-mini with focused classification prompt). Two-layer execution: rules first (<10ms), LLM only if rules are inconclusive
- [x] **T-03** Implement rejection template system: `IGuardrailTemplateProvider` with configurable Spanish-language templates per classification category. Templates include explanation + 3-4 capability examples. Greeting handler returns friendly response with capability summary without invoking executor. All rejections streamed via SSE as normal text responses
- [x] **T-04** Handle `AgentFinishReason.ContentFilter` in `ChatQueryHandler`: when Azure returns `ContentFilter`, emit a user-friendly Spanish message ("No puedo procesar esta consulta. Por favor reformulá tu pregunta.") instead of silently ending the stream

**Query enricher (WI: chat-query-enricher)**

- [x] **T-05** Implement `QueryEnricherStage`: rule-based intent classifier (keyword/regex patterns: "¿cuántos" → `statistics`, "art."/"ley" → `statute_research`, "citar"/"citado por" → `precedent_exploration`, etc.) and rule-based entity extractor (regex for date ranges, court abbreviations, law references, "c/" case name patterns). Fast path handles ~60-70% of queries without LLM call
- [x] **T-06** Implement LLM enrichment fallback for ambiguous/complex queries: GPT-4o-mini call with structured output prompt returning JSON `{ intent, entities: { temporal, courts, statutes, cases, topics } }`. Implement context injection: append enriched metadata as additional system message to executor's message list. Graceful degradation: enricher failure → proceed with raw query

**Output guardrail (WI: chat-output-guardrail)**

- [x] **T-07** Implement `CitationParser` shared utility in Application: `Parse(text) → Citation[]` extracting `{caso: "...", id: "..."}` patterns, `Normalize(text) → string` converting citation variants to canonical format, `Validate(citation, knownIds) → ValidationResult`. Align regex with frontend `CITATION_REGEX`
- [x] **T-08** Implement `OutputGuardrailStage`: accumulate response text from `TextChunkEvent` yields, parse citations via `CitationParser`, batch-validate ruling IDs against database (`IRulingRepository` — single `SELECT Id, CaseTitle FROM Rulings WHERE Id IN (@ids)`), check title consistency (fuzzy match), check tool-grounding against `ToolExecutionContext.ResolvedRulingIds`. Produce `CitationValidationResult` with per-citation status
- [x] **T-09** Implement SSE validation event: emit `event: validation\ndata: {"status":"passed"|"warnings","details":[...]}\n\n` after last text chunk and before `[DONE]`. Append warning text to stream when serious issues detected ("Nota: Algunas referencias no pudieron ser verificadas..."). Enforce disclaimer on substantive legal responses. Strictness configurable: `lenient` (log only), `moderate` (append warnings), `strict` (block + warn)

**Response finalizer (WI: chat-response-finalizer)**

- [x] **T-10** Implement `ResponseFinalizerStage`: citation normalization in chunk-mode with 200-char look-ahead buffer for cross-chunk citations (extra whitespace, single quotes → double quotes, broken citations), markdown cleanup (unclosed markers, excessive newlines, orphan headings), empty response fallback ("No pude generar una respuesta. Por favor intentá reformular tu consulta."), conditional disclaimer injection using configurable template text

**Cross-cutting**

- [x] **T-11** Add `ChatPipeline` configuration section to `appsettings.json` with sub-sections: `InputGuardrail` (Enabled, UseLlmClassifier, ClassifierModel), `QueryEnricher` (Enabled, UseLlmFallback), `OutputGuardrail` (Enabled, Strictness, DisclaimerText), `ResponseFinalizer` (Enabled, DisclaimerEnabled, StructureEnforcement). Add `AzureOpenAI:MiniDeploymentName` for GPT-4o-mini deployment
- [x] **T-12** Unit tests: `ChatPipelineOrchestrator` stage chaining and short-circuit on guardrail rejection, `InputGuardrailStage` classification per category (rule-based + LLM mock), rejection template rendering, `QueryEnricherStage` intent detection and entity extraction, `CitationParser` parse/normalize/validate, `OutputGuardrailStage` with valid/invalid/mixed citations, `ResponseFinalizerStage` normalization and disclaimer injection. Mocked `IGuardrailClassifier` and `IRulingRepository`
- [x] **T-13** Frontend: extend `ChatService` to parse `event: validation` SSE event. Extend `ChatViewComponent` message model with optional `validation` field. Render validation status (checkmark for passed, warning icon for warnings with tooltip showing details) below assistant message

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E235 | `IChatPipelineStage` interface, `ChatPipelineOrchestrator` chaining stages around agentic executor, `ChatPipelineOptions` configuration model, DI registration | `[ ]` | `[ ]` |
| E236 | Input guardrail rule-based layer: prompt injection detection, PII patterns, scope keywords. Classifying queries into `legal_query`, `greeting`, `clarification`, `out_of_scope`, `harmful` | `[ ]` | `[ ]` |
| E237 | Input guardrail LLM classifier: `IGuardrailClassifier` + `AzureOpenAiGuardrailClassifier` using GPT-4o-mini for ambiguous queries | `[ ]` | `[ ]` |
| E238 | Rejection templates: configurable Spanish-language responses per category with capability examples. Greeting handler bypassing executor | `[ ]` | `[ ]` |
| E239 | `ContentFilter` finish reason handled in `ChatQueryHandler` with user-friendly Spanish message | `[ ]` | `[ ]` |
| E240 | Query enricher rule-based layer: intent classification + entity extraction via keyword/regex patterns | `[ ]` | `[ ]` |
| E241 | Query enricher LLM fallback: GPT-4o-mini structured output for complex queries. Context injection as system message to executor | `[ ]` | `[ ]` |
| E242 | `CitationParser` shared utility: `Parse`, `Normalize`, `Validate` methods aligned with frontend `CITATION_REGEX` | `[ ]` | `[ ]` |
| E243 | Output guardrail: citation ID batch validation against database, title consistency check, tool-grounding check via `ResolvedRulingIds` | `[ ]` | `[ ]` |
| E244 | SSE `validation` event emitted after response. Warning text appended for unverified citations. Legal disclaimer enforced on substantive responses. Strictness configurable | `[ ]` | `[ ]` |
| E245 | Response finalizer: citation normalization (chunk-mode with look-ahead buffer), markdown cleanup, empty response fallback, conditional disclaimer injection | `[ ]` | `[ ]` |
| E246 | `ChatPipeline` configuration section in `appsettings.json` with per-stage toggles and `AzureOpenAI:MiniDeploymentName` | `[ ]` | `[ ]` |
| E247 | Unit tests: pipeline orchestrator, input guardrail (rules + LLM mock), query enricher (rules + LLM mock), citation parser, output guardrail, response finalizer | `[ ]` | `[ ]` |
| E248 | Frontend `ChatService` parsing `validation` SSE event. `ChatViewComponent` rendering validation status with icon and tooltip | `[ ]` | `[ ]` |
| E249 | ADR-014 updated: GPT-4o-mini deployment added for lightweight classification tasks (guardrail, enricher) | `[ ]` | `[ ]` |
```

---

## Architecture Decision Update

Add to ADR-014 (or create ADR-015):

| ID | Decision | Detail |
|---|---|---|
| ADR-015 | Chat pipeline stages | Input guardrail (fail-closed, rules + GPT-4o-mini), Query enricher (fail-open, rules + GPT-4o-mini), Output guardrail (fail-closed, rules + DB), Response finalizer (deterministic string processing). Pipeline as middleware around agentic executor. |

---

## Summary Deltas

| Metric | Before | After | Delta |
|---|---|---|---|
| Phase 3 features | 6 (F3-0 to F3-5) | 7 (F3-0 to F3-6) | +1 |
| Phase 3 design deliverables | 14 | 17 | +3 |
| Phase 3 dev deliverables | 17 | 32 | +15 |
| Phase 3 total deliverables | 31 | 49 | +18 |
| Global total deliverables | 232 | 250 | +18 |
| Deliverable range | E001–E231 | E001–E249 | +18 |
