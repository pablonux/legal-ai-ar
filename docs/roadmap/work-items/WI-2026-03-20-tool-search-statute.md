# Work Item — Tool: search_by_statute

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **legal professional**, I want **the assistant to find rulings that cite a specific law or statute** so that **I can research how a norm has been interpreted and applied across jurisprudence**.

## Context

- **Current state**: The enrichment pipeline extracts statutes from rulings and stores them in the `Statutes` table with `RulingStatute` join (including cited articles). This data is available in SQL but not exposed to the chat assistant. The search index does not have a statute filter. Vector search may find topically related rulings but cannot guarantee they actually cite a specific norm.
- **Target state**: A `search_by_statute` tool that queries the `RulingStatute` + `Ruling` tables to find rulings that explicitly cite a given law, returning ruling metadata.
- **Typical prompts**: "¿Qué fallos aplican el art. 75 inc. 22 de la Constitución Nacional?", "Jurisprudencia sobre la Ley 26.994 (Código Civil y Comercial)", "Fallos que citen la Ley de Defensa del Consumidor".

## Tool Definition

```json
{
  "name": "search_by_statute",
  "description": "Find rulings that cite a specific law or statute. Use when the user asks about rulings that apply, interpret, or cite a particular legal norm or article.",
  "parameters": {
    "type": "object",
    "properties": {
      "statuteName": {
        "type": "string",
        "description": "Name or description of the statute/law to search for. Supports partial match."
      },
      "statuteNumber": {
        "type": "string",
        "description": "Official number of the statute (e.g. '26.994', '24.240'). Optional."
      },
      "articles": {
        "type": "string",
        "description": "Specific articles referenced (e.g. 'art. 75 inc. 22'). Optional — narrows results to rulings citing these articles."
      },
      "topK": {
        "type": "integer",
        "description": "Max results (1-20). Default: 10.",
        "default": 10
      }
    },
    "required": ["statuteName"]
  }
}
```

## Acceptance Criteria

1. **New repository method**: `IStatuteRepository` (or `IRulingRepository`) exposes a method to find rulings by statute name/number with optional article filter. Query joins `Statutes` → `RulingStatutes` → `Rulings` with `LIKE` matching on statute name/number and optional articles.
2. **Tool registered** in `IToolRegistry` with the schema above.
3. **Execution**: Calls the repository method, returns structured list of rulings (caseTitle, rulingId, date, court, summary, cited articles for that statute).
4. **Partial match**: Statute name search uses case-insensitive partial matching (e.g. "Defensa del Consumidor" matches "Ley de Defensa del Consumidor N° 24.240").
5. **No results**: Returns "No se encontraron fallos que citen esa norma" to the model.
6. **Integration test**: Tool execution with seeded statute data returns expected results.

## Out of Scope

- Full-text search within statute body/articles (only name/number matching)
- Statute detail retrieval (only rulings that cite it)
- Adding statute filter to Azure AI Search index (SQL-only for now)
