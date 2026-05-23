# Work Item — Tool: search_rulings

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **legal professional**, I want **the assistant to perform targeted searches with filters (date range, court, jurisdiction area, instance, keywords)** so that **I get precise jurisprudential results through conversation without manually constructing search queries**.

## Context

- **Current state**: The RAG pass does a vector-only search with fixed top-K (10 chunks, 5 rulings). No filters are applied. The UI search (`RulingsController.Search`) already supports `SearchFilters` (JurisdictionArea, Instance, CourtName, DateFrom, DateTo, Keywords) via `ISearchService.SearchAsync`.
- **Target state**: The assistant invokes `search_rulings` with model-extracted parameters, calling the same `ISearchService.SearchAsync` + `IEmbeddingService.GenerateAsync` pipeline the UI uses. Results are formatted as structured context and returned to the model for reasoning.
- **Typical prompts**: "Buscame fallos de la CSJN sobre libertad de expresión entre 2020 y 2024", "Jurisprudencia reciente en materia ambiental de la Cámara Federal", "Fallos sobre despido discriminatorio con keyword 'discriminación'".

## Tool Definition

```json
{
  "name": "search_rulings",
  "description": "Search jurisprudential rulings with optional filters. Use when the user asks for rulings matching specific criteria (court, date range, topic, jurisdiction). Returns ruling metadata and summaries.",
  "parameters": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "Natural language search query describing the legal topic or question."
      },
      "dateFrom": {
        "type": "string",
        "format": "date",
        "description": "Filter: earliest ruling date (YYYY-MM-DD). Optional."
      },
      "dateTo": {
        "type": "string",
        "format": "date",
        "description": "Filter: latest ruling date (YYYY-MM-DD). Optional."
      },
      "jurisdictionArea": {
        "type": "string",
        "description": "Filter: jurisdiction area (e.g. 'Penal', 'Civil', 'Laboral'). Optional."
      },
      "instance": {
        "type": "string",
        "description": "Filter: court instance (e.g. 'CSJN', 'Cámara', 'Primera Instancia'). Optional."
      },
      "courtName": {
        "type": "string",
        "description": "Filter: court name. Optional."
      },
      "keywords": {
        "type": "array",
        "items": { "type": "string" },
        "description": "Filter: keyword descriptions to match. Optional."
      },
      "topK": {
        "type": "integer",
        "description": "Max results to return (1-20). Default: 5.",
        "default": 5
      }
    },
    "required": ["query"]
  }
}
```

## Acceptance Criteria

1. **Tool registered** in `IToolRegistry` with the schema above.
2. **Execution**: Embeds `query` via `IEmbeddingService`, builds `SearchFilters` from optional params, calls `ISearchService.SearchAsync`, returns top-K results formatted as structured text (caseTitle, id, date, court, summary, holding).
3. **Result format**: Each result includes `rulingId` (UUID) so the model can cite it using the existing `{caso: "...", id: "..."}` format.
4. **Edge cases**: Empty query → error message to model. No results → explicit "No se encontraron fallos" message. Invalid date format → ignored with warning.
5. **Integration test**: Tool execution with mocked services returns expected structured output.

## Out of Scope

- Full-text search within ruling body (only metadata + summary fields)
- Pagination within tool results (single page, topK cap)
