# Work Item — Tools: Ruling Detail & Navigation

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **legal professional**, I want **the assistant to retrieve full ruling details and navigate citation relationships** so that **I can explore jurisprudence in depth — seeing judges, cited statutes, keywords, and how rulings cite each other — through natural conversation**.

## Context

- **Current state**: The RAG context includes only summary-level metadata (caseTitle, summary, holding, date, court). Full ruling detail (judges, keywords, statutes, citations) is available via `IRulingRepository.GetByIdAsync` and `IGraphService.GetInbound/OutboundCitationsAsync`, but not exposed to the chat.
- **Target state**: Three tools that the assistant invokes when it needs to drill down:
  - `get_ruling_detail` — full metadata for a specific ruling
  - `get_ruling_citations` — inbound/outbound citation graph
  - `get_related_rulings` — semantically similar rulings
- **Typical prompts**: "Dame más detalles sobre ese fallo", "¿Qué jueces firmaron?", "¿Qué normas se citaron?", "¿Qué fallos posteriores lo citaron?", "Mostrá fallos similares a este".

## Tool Definitions

### get_ruling_detail

```json
{
  "name": "get_ruling_detail",
  "description": "Get full metadata for a specific ruling: court, judges, keywords, cited statutes, summary, holding, dates. Use when the user asks for details about a specific ruling already mentioned in the conversation.",
  "parameters": {
    "type": "object",
    "properties": {
      "rulingId": {
        "type": "string",
        "format": "uuid",
        "description": "UUID of the ruling to retrieve."
      }
    },
    "required": ["rulingId"]
  }
}
```

### get_ruling_citations

```json
{
  "name": "get_ruling_citations",
  "description": "Get citation relationships for a ruling. Returns rulings that cite it (inbound) and/or rulings it cites (outbound). Use when the user asks about citation history, precedent chains, or influence of a ruling.",
  "parameters": {
    "type": "object",
    "properties": {
      "rulingId": {
        "type": "string",
        "format": "uuid",
        "description": "UUID of the ruling to get citations for."
      },
      "direction": {
        "type": "string",
        "enum": ["inbound", "outbound", "both"],
        "description": "Direction of citations. 'inbound' = rulings that cite this one, 'outbound' = rulings this one cites, 'both' = all.",
        "default": "both"
      }
    },
    "required": ["rulingId"]
  }
}
```

### get_related_rulings

```json
{
  "name": "get_related_rulings",
  "description": "Find rulings semantically similar to a given ruling. Use when the user asks for related or similar jurisprudence.",
  "parameters": {
    "type": "object",
    "properties": {
      "rulingId": {
        "type": "string",
        "format": "uuid",
        "description": "UUID of the reference ruling."
      },
      "topK": {
        "type": "integer",
        "description": "Max results (1-10). Default: 5.",
        "default": 5
      }
    },
    "required": ["rulingId"]
  }
}
```

## Acceptance Criteria

1. **get_ruling_detail**: Calls `IRulingRepository.GetByIdAsync`, returns structured output with: caseTitle, rulingId, date, court, jurisdictionArea, instance, rulingDirection, isUnconstitutional, summary, holding, judges (name + participation type), keywords, cited statutes (name + articles), subjectArea.
2. **get_ruling_citations**: Calls `IGraphService.GetInboundCitationsAsync` / `GetOutboundCitationsAsync`, resolves target ruling metadata, returns list of citing/cited rulings with caseTitle, rulingId, date, citationType, externalAlias.
3. **get_related_rulings**: Calls `ISearchService.SearchRelatedAsync`, returns list with caseTitle, rulingId, date, court, summary, score.
4. **Not found**: If ruling ID does not exist, returns clear error message to the model (not exception).
5. **Citation enablement**: All returned ruling IDs use UUID format so the model can cite them with `{caso: "...", id: "..."}`.
6. **Integration tests**: Each tool returns expected output with mocked services.

## Out of Scope

- Full text retrieval (FullText field can be very large; only summary/holding are returned)
- Recursive citation traversal (depth > 1)
