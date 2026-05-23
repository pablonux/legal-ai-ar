# Feature — Chat Pipeline (Multi-Stage Architecture)

**Created**: 2026-03-20  
**Status**: Draft  
**Type**: Chat assistant quality & safety

## Summary

Evolve the jurisprudential chat assistant from a single-stage agentic loop into a multi-stage pipeline with dedicated guardrails, query enrichment, and response validation. The pipeline wraps the existing agentic executor (F1-17) with safety and quality layers critical for a legal-domain assistant.

## Motivation

The current chat pipeline (`ChatQueryHandler` with agentic loop + tools) is functionally capable but lacks the safety layers expected of a professional legal assistant:

- **No input moderation**: The model can be steered to answer questions outside its scope (non-legal topics, personal legal advice, prompt injection). The only guardrails are a system prompt instruction ("no llames herramientas para saludos") and Azure content filters mapped at the infrastructure layer but not handled at the application level.
- **No output validation**: Ruling IDs cited in responses are not verified against the database. The model can hallucinate case titles, invent statutes, or generate responses without legal disclaimers.
- **No intent understanding**: The agentic executor receives the raw user query without classification. A greeting, a complex multi-faceted legal question, and an out-of-scope request all enter the same pipeline.
- **No response normalization**: Citation format consistency, disclaimer presence, and structural quality depend entirely on the model's adherence to the system prompt.

For a legal-domain application, these gaps represent both quality and liability risks.

## Target Architecture

```
User Query
    │
    ▼
┌─────────────────────────────────┐
│  1. INPUT GUARDRAIL             │  Rules + lightweight classifier
│     Scope check, injection      │  Latency: ~200-500ms
│     detection, PII scan         │
└────────────┬────────────────────┘
             │ (reject → friendly response + capability examples)
             ▼
┌─────────────────────────────────┐
│  2. QUERY ENRICHER              │  Rules + NER / optional LLM-mini
│     Intent classification       │  Latency: ~100-300ms
│     Entity extraction           │
└────────────┬────────────────────┘
             │ (enriched query context)
             ▼
┌─────────────────────────────────┐
│  3. AGENTIC EXECUTOR            │  Existing: GPT-4o + tools
│     ChatQueryHandler loop       │  Latency: 2-5s
│     (F1-17 — already built)     │
└────────────┬────────────────────┘
             │ (raw response text)
             ▼
┌─────────────────────────────────┐
│  4. OUTPUT GUARDRAIL            │  Rules + DB validation
│     Citation verification       │  Latency: ~100-200ms
│     Hallucination flags         │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│  5. RESPONSE FINALIZER          │  Deterministic post-processing
│     Citation normalization      │  Latency: <50ms
│     Disclaimer injection        │
└────────────┬────────────────────┘
             │
             ▼
        SSE Stream → Client
```

### Design Principles

| Principle | Rationale |
|-----------|-----------|
| **Minimize LLM calls** | Each extra GPT-4o call adds ~1-3s latency and ~$0.01-0.03 cost. Stages 1, 2, 4, 5 should be rule-based or use a lightweight model (GPT-4o-mini). |
| **Preserve streaming** | The SSE streaming contract with the frontend must be maintained. Guardrails operate on the query (pre-stream) or on accumulated response text (post-stream). |
| **Pipeline as middleware** | Each stage is an `IChatPipelineStage` that can be enabled/disabled via configuration, allowing incremental rollout and A/B testing. |
| **Fail-open for non-critical stages** | Query enricher failure should not block the request; executor proceeds with the raw query. Input/output guardrails are fail-closed (block if uncertain). |

## Scope

- **In scope**: Input guardrail, query enricher, output guardrail, response finalizer, pipeline orchestration, SSE integration, configuration.
- **Out of scope**: Multi-turn conversation memory (separate feature), token budget management, frontend UX changes beyond existing tool chips, individual tool additions.

## Dependencies

| WI | Depends on |
|----|------------|
| WI-2026-03-20-chat-input-guardrail | None — can be built independently |
| WI-2026-03-20-chat-query-enricher | WI-2026-03-20-chat-input-guardrail (pipeline infra) |
| WI-2026-03-20-chat-output-guardrail | Existing agentic executor (F1-17) |
| WI-2026-03-20-chat-response-finalizer | WI-2026-03-20-chat-output-guardrail |

## Work Items

| WI | Title | Status |
|----|-------|--------|
| [WI-2026-03-20-chat-input-guardrail](./WI-2026-03-20-chat-input-guardrail.md) | Input Guardrail — Moderation, scope enforcement, prompt injection detection | Draft |
| [WI-2026-03-20-chat-query-enricher](./WI-2026-03-20-chat-query-enricher.md) | Query Enricher — Intent classification and entity extraction | Draft |
| [WI-2026-03-20-chat-output-guardrail](./WI-2026-03-20-chat-output-guardrail.md) | Output Guardrail — Citation validation, hallucination detection, disclaimers | Draft |
| [WI-2026-03-20-chat-response-finalizer](./WI-2026-03-20-chat-response-finalizer.md) | Response Finalizer — Post-processing, normalization, disclaimer injection | Draft |
