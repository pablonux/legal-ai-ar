# Work Item — Query Enricher

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Pipeline](./FEATURE-chat-pipeline.md)

## User Story

As a **legal professional using the chat assistant**, I want **my queries to be automatically analyzed for intent and relevant entities before execution** so that **the agentic executor receives enriched context that improves tool selection, search precision, and response quality**.

## Context

- **Current state**: `ChatQueryHandler` passes the raw user query directly to GPT-4o with tool definitions. The model must infer intent, extract entities (dates, courts, laws, case names), and decide which tools to call — all in a single LLM invocation. For complex queries ("Fallos de la CSJN sobre libertad de expresión entre 2020 y 2024 que citen el art. 14 CN"), the model sometimes makes suboptimal tool calls (e.g., omitting date filters, calling `search_rulings` without the statute reference).
- **Target state**: A lightweight pre-processing stage that classifies the query intent and extracts structured entities, passing this enriched context to the executor's system prompt. This helps GPT-4o make better first-call tool decisions, reducing unnecessary iterations.
- **Latency budget**: 100-300ms. For simple queries (single intent, no entities), this stage should be near-instant (rule-based). For complex queries, a fast LLM call is acceptable.
- **This is NOT a planner**: The enricher does not decide which tools to call or in what order. It provides structured metadata that the agentic executor (GPT-4o) uses to plan better. The model retains full autonomy over tool selection.

## Acceptance Criteria

1. **Intent classification**: The enricher identifies the primary intent of the query:
   - `search` — User wants to find rulings matching criteria
   - `detail` — User wants detailed information about a specific ruling
   - `comparison` — User wants to compare rulings or legal positions
   - `statistics` — User wants quantitative information (counts, distributions)
   - `precedent_exploration` — User wants to navigate citation chains
   - `statute_research` — User wants rulings related to a specific law/article
   - `general` — General legal question not fitting specific categories
2. **Entity extraction**: The enricher identifies structured entities when present:
   - **Temporal**: Date ranges, years, periods ("entre 2020 y 2024", "últimos 5 años")
   - **Institutional**: Court names, jurisdictions, instances ("CSJN", "Cámara Federal", "CABA")
   - **Normative**: Law references, article numbers ("art. 14 CN", "Ley 26.994", "Código Penal art. 79")
   - **Case identifiers**: Case names or partial references ("Ekmekdjian c/ Sofovich", "fallo Arriola")
   - **Subject matter**: Legal topics/keywords ("despido injustificado", "libertad de expresión")
3. **Enriched context injection**: The extracted intent and entities are formatted and injected into the agentic executor's message list as an additional system message (after the main system prompt), providing structured guidance without overriding the model's autonomy.
4. **Graceful degradation**: If the enricher fails or times out, the pipeline proceeds with the raw query. No enrichment is better than delayed enrichment.
5. **Rule-based fast path**: Simple queries with obvious single intent and no complex entities bypass the LLM classifier entirely (regex + keyword matching). Only ambiguous or entity-rich queries trigger the LLM classification.
6. **Configuration**: Enabled/disabled via `ChatPipeline:QueryEnricher` configuration. Entity extraction categories are configurable.
7. **Observability**: Log extracted intent, entity count, and processing latency at `Information` level.

## Out of Scope

- Query rewriting or reformulation (the enricher informs, does not transform)
- Conversation context analysis (multi-turn is a separate feature)
- Custom NER model training (use rule-based + LLM-mini)
- Caching of enrichment results

## Technical Notes

- **Classification approach (recommended)**:
  1. **Rule-based layer**: Keyword/regex patterns for common intents ("¿cuántos" → `statistics`, "art." / "ley" → `statute_research`, "citar" / "citado por" → `precedent_exploration`). Handles ~60-70% of queries without LLM.
  2. **LLM fallback**: For ambiguous queries, a GPT-4o-mini call with a structured output prompt returning JSON: `{ "intent": "...", "entities": { "temporal": [...], "courts": [...], "statutes": [...], "cases": [...], "topics": [...] } }`.
- **Context injection format** (example appended to messages):
  ```
  [Query Analysis]
  Intent: statute_research
  Entities detected:
  - Court: CSJN
  - Statute: art. 14 CN (Constitución Nacional)
  - Period: 2020-2024
  - Topic: libertad de expresión
  Consider using search_by_statute for the normative reference and search_rulings with court and date filters for the temporal/institutional constraints.
  ```
- The enricher runs **after** the input guardrail (only enriches validated queries) and **before** the agentic executor.
