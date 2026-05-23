# Feature — Chat Tools (Function Calling)

**Created**: 2026-03-20  
**Status**: Draft  
**Type**: Chat assistant enhancements

## Summary

Transform the jurisprudential chat assistant from a single-pass RAG pipeline into an agentic assistant with function calling (tools). The model dynamically decides which tools to invoke based on the user's query — filtered search, ruling detail retrieval, citation graph navigation, statute lookup, and aggregation — instead of relying solely on the initial top-K vector retrieval.

## Motivation

The current RAG approach embeds the user query, retrieves top-K chunks/rulings, and streams a response. This works well for broad questions but falls short when users need:

- **Precision**: "Fallos de la CSJN sobre libertad de expresión entre 2020 y 2024" — requires search filters the RAG pass does not apply.
- **Depth**: "Mostrá los jueces y normas citadas en ese fallo" — requires full ruling detail, not just summary context.
- **Navigation**: "¿Qué fallos posteriores citaron Ekmekdjian c/ Sofovich?" — requires citation graph traversal.
- **Normative research**: "¿Qué fallos aplican el art. 75 inc. 22 de la CN?" — requires statute-based lookup.
- **Quantification**: "¿Cuántos fallos de inconstitucionalidad hay en materia penal?" — requires count aggregation.

Tools enable the assistant to gather exactly the information it needs, across multiple steps if necessary, before composing the final answer.

## Scope

- **In scope**: Agentic loop infrastructure, tool definitions (search, detail, citations, statute, aggregation), frontend UX for tool execution feedback, streaming adaptation.
- **Out of scope**: Multi-turn conversation history/memory, tool composition orchestration (T8 compare, T9 timeline from analysis — deferred), admin/write tools.

## Dependencies

| WI | Depends on |
|----|------------|
| WI-2026-03-20-agentic-chat-infra | None — foundational |
| WI-2026-03-20-tool-search-rulings | WI-2026-03-20-agentic-chat-infra |
| WI-2026-03-20-tool-ruling-detail-nav | WI-2026-03-20-agentic-chat-infra |
| WI-2026-03-20-tool-search-statute | WI-2026-03-20-agentic-chat-infra |
| WI-2026-03-20-tool-aggregation | WI-2026-03-20-agentic-chat-infra |
| WI-2026-03-20-chat-tool-ux | WI-2026-03-20-agentic-chat-infra |

## Work Items

| WI | Title | Status |
|----|-------|--------|
| [WI-2026-03-20-agentic-chat-infra](./WI-2026-03-20-agentic-chat-infra.md) | Agentic Chat Infrastructure — Function calling loop and streaming | Draft |
| [WI-2026-03-20-tool-search-rulings](./WI-2026-03-20-tool-search-rulings.md) | Tool: search_rulings — Filtered jurisprudential search | Draft |
| [WI-2026-03-20-tool-ruling-detail-nav](./WI-2026-03-20-tool-ruling-detail-nav.md) | Tools: get_ruling_detail + get_ruling_citations + get_related_rulings | Draft |
| [WI-2026-03-20-tool-search-statute](./WI-2026-03-20-tool-search-statute.md) | Tool: search_by_statute — Normative reference lookup | Draft |
| [WI-2026-03-20-tool-aggregation](./WI-2026-03-20-tool-aggregation.md) | Tools: count_rulings + list_courts + list_judges | Draft |
| [WI-2026-03-20-chat-tool-ux](./WI-2026-03-20-chat-tool-ux.md) | Frontend: Tool execution feedback UX | Draft |
