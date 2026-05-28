# Work Item — Tools: Aggregation & Discovery

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **legal professional**, I want **the assistant to count rulings matching criteria and list available courts and judges** so that **I can answer quantitative questions and discover what data is available in the knowledge base**.

## Context

- **Current state**: The assistant cannot answer quantitative questions ("¿cuántos fallos hay sobre X?") because it only sees top-K results. Court and judge catalogs exist in SQL (`Courts`, `Judges` tables) but are not accessible from chat.
- **Target state**: Three lightweight tools for aggregation and discovery:
  - `count_rulings` — count matching rulings (same filters as search_rulings)
  - `list_courts` — enumerate courts, optionally filtered
  - `list_judges` — enumerate judges, optionally filtered
- **Typical prompts**: "¿Cuántos fallos de inconstitucionalidad hay en materia penal?", "¿Qué tribunales tienen jurisprudencia sobre derecho ambiental?", "¿Qué jueces han intervenido en fallos sobre libertad de prensa?".

## Tool Definitions

### count_rulings

```json
{
  "name": "count_rulings",
  "description": "Count rulings matching the given filters. Use when the user asks 'how many' or 'cuántos' rulings match criteria. Returns a count, not the rulings themselves.",
  "parameters": {
    "type": "object",
    "properties": {
      "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area. Optional." },
      "instance": { "type": "string", "description": "Filter by instance. Optional." },
      "courtName": { "type": "string", "description": "Filter by court name. Optional." },
      "dateFrom": { "type": "string", "format": "date", "description": "Earliest date. Optional." },
      "dateTo": { "type": "string", "format": "date", "description": "Latest date. Optional." },
      "isUnconstitutional": { "type": "boolean", "description": "Filter by unconstitutionality flag. Optional." },
      "keywords": { "type": "array", "items": { "type": "string" }, "description": "Filter by keywords. Optional." }
    }
  }
}
```

### list_courts

```json
{
  "name": "list_courts",
  "description": "List courts available in the knowledge base, optionally filtered by jurisdiction or instance. Use when the user asks what courts are available or which courts have rulings on a topic.",
  "parameters": {
    "type": "object",
    "properties": {
      "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area. Optional." },
      "instance": { "type": "string", "description": "Filter by instance. Optional." }
    }
  }
}
```

### list_judges

```json
{
  "name": "list_judges",
  "description": "List judges who have participated in rulings, optionally filtered by court. Use when the user asks about judges or wants to know who has ruled on a topic.",
  "parameters": {
    "type": "object",
    "properties": {
      "courtName": { "type": "string", "description": "Filter by court name. Optional." }
    }
  }
}
```

## Acceptance Criteria

1. **count_rulings**: New repository method `CountAsync(filters)` on `IRulingRepository` that returns `int`. Applies same filter logic as search but only counts. Tool returns `{"count": N, "filters_applied": {...}}`.
2. **list_courts**: Query `Courts` table with optional jurisdiction/instance filters. Returns list of `{id, name, jurisdictionArea, territory, instance}`. Cap at 50 results.
3. **list_judges**: Query `Judges` + `RulingJudges` with optional court filter. Returns list of `{id, firstName, lastName, rulingCount}`. Cap at 50 results, ordered by ruling count desc.
4. **Empty results**: Each tool returns a clear message when no matches found.
5. **Performance**: count_rulings should use SQL `COUNT(*)` not load entities. list_courts/judges should be lightweight queries.
6. **Integration tests**: Each tool returns expected output with seeded data.

## Out of Scope

- Keyword-based counting (requires search index facets or fulltext — SQL keyword join only)
- Cross-tabulation / pivot queries (e.g. "rulings per court per year")
- Exporting results
