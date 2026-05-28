# Chat Tools — Tool Catalog

| Field | Value |
|---|---|
| **ID** | E217 |
| **Feature** | F1-17 · Chat Tools — Function Calling |
| **Date** | 2026-03-20 |

---

## Purpose

Complete catalog of all tools available to the jurisprudential chat assistant. For each tool: name, description, JSON schema, backend service mapping, result format, and usage guidance. Serves as the reference for implementing T-06 through T-12 of F1-17.

**Reference**: E215 (architecture), E076 (RAG design), architecture §4–5.

---

## Tool Summary

| # | Tool | Category | Backend Service | Phase |
|---|---|---|---|---|
| 1 | `search_rulings` | Search | `ISearchService`, `IEmbeddingService` | T-06 |
| 2 | `get_ruling_detail` | Detail | `IRulingRepository` | T-07 |
| 3 | `get_ruling_citations` | Navigation | `IGraphService`, `IRulingRepository` | T-08 |
| 4 | `get_related_rulings` | Navigation | `ISearchService` | T-09 |
| 5 | `search_by_statute` | Search | `IStatuteRepository` (new method) | T-10 |
| 6 | `count_rulings` | Aggregation | `IRulingRepository` (new method) | T-11 |
| 7 | `list_courts` | Discovery | `ICourtRepository` | T-12 |
| 8 | `list_judges` | Discovery | `IJudgeRepository` | T-12 |

---

## 1. search_rulings

**When to use**: User asks for rulings matching specific criteria (topic, date range, court, jurisdiction, keywords). This is the primary search tool — the model should invoke it as its first action for most jurisprudential queries.

### JSON Schema

```json
{
  "name": "search_rulings",
  "description": "Search jurisprudential rulings with optional filters. Returns ruling metadata and summaries. Use for any query about rulings matching specific criteria.",
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
        "description": "Earliest ruling date (YYYY-MM-DD)."
      },
      "dateTo": {
        "type": "string",
        "format": "date",
        "description": "Latest ruling date (YYYY-MM-DD)."
      },
      "jurisdictionArea": {
        "type": "string",
        "description": "Jurisdiction area (e.g. Penal, Civil, Laboral, Contencioso Administrativo)."
      },
      "instance": {
        "type": "string",
        "description": "Court instance (e.g. CSJN, Camara, Primera Instancia)."
      },
      "courtName": {
        "type": "string",
        "description": "Court name."
      },
      "keywords": {
        "type": "array",
        "items": { "type": "string" },
        "description": "Keyword descriptions to filter by."
      },
      "topK": {
        "type": "integer",
        "description": "Max results to return (1-20). Default: 5."
      }
    },
    "required": ["query"]
  }
}
```

### Backend Mapping

1. Embed `query` via `IEmbeddingService.GenerateAsync`.
2. Build `SearchFilters` from optional params: `new SearchFilters(jurisdictionArea, instance, CourtName: courtName, DateFrom: dateFrom, DateTo: dateTo, Keywords: keywords)`.
3. Call `ISearchService.SearchAsync(embedding, query, filters, page: 1, pageSize: topK ?? 5)`.
4. Format each result.

### Result Format

```
[search_rulings: {N} results for "{query}"]

1. Case: {CaseTitle} | ID: {RulingId} | Date: {RulingDate:yyyy-MM-dd}
   Court: {Court} | Area: {JurisdictionArea} / {Instance}
   Summary: {Summary}

2. ...
```

If no results: `[search_rulings: 0 results for "{query}"] No rulings found matching the criteria.`

---

## 2. get_ruling_detail

**When to use**: User asks for full details about a specific ruling already mentioned in the conversation (judges, statutes, keywords, unconstitutionality, etc.).

### JSON Schema

```json
{
  "name": "get_ruling_detail",
  "description": "Get full metadata for a specific ruling: court, judges, keywords, cited statutes, summary, holding. Use when the user asks for details about a specific ruling.",
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

### Backend Mapping

1. Parse `rulingId` as `Guid`.
2. Call `IRulingRepository.GetByIdAsync(id)` — loads navigation properties (Court, RulingJudges→Judge, RulingKeywords→Keyword, RulingStatutes→Statute, OutboundCitations).
3. Format complete metadata.

### Result Format

```
[get_ruling_detail: {CaseTitle}]

Case: {CaseTitle}
ID: {Id}
Date: {RulingDate:yyyy-MM-dd}
Court: {Court.Name} ({Court.JurisdictionArea} / {Court.Instance})
Case Number: {CaseNumber}
Direction: {RulingDirection}
Subject Area: {SubjectArea}
Unconstitutional: {IsUnconstitutional}

Summary:
{Summary}

Holding:
{Holding}

Judges:
- {FirstName} {LastName} ({ParticipationType})
- ...

Keywords:
- {Description}
- ...

Cited Statutes:
- {Name} (No. {Number}) — Articles: {Articles}
- ...

Outbound Citations ({count}):
- {ExternalAlias} → {TargetRuling.CaseTitle ?? "unresolved"} ({CitationType})
- ...
```

If not found: `[get_ruling_detail: not found] No ruling found with ID {rulingId}.`

---

## 3. get_ruling_citations

**When to use**: User asks about citation history, which rulings cite a given ruling, or what a ruling cites. Useful for precedent chain analysis.

### JSON Schema

```json
{
  "name": "get_ruling_citations",
  "description": "Get citation relationships for a ruling. Returns rulings that cite it (inbound) and/or rulings it cites (outbound). Use for precedent chain analysis.",
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
        "description": "Citation direction. Default: both."
      }
    },
    "required": ["rulingId"]
  }
}
```

### Backend Mapping

1. Parse `rulingId` as `Guid`.
2. Based on `direction` (default `"both"`):
   - `"outbound"` or `"both"`: Call `IGraphService.GetOutboundCitationsAsync(rulingId)`.
   - `"inbound"` or `"both"`: Call `IGraphService.GetInboundCitationsAsync(rulingId)`.
3. For each citation with a resolved `TargetRulingId`, fetch ruling metadata via `IRulingRepository.GetChatMetadataBatchAsync` (batch call for efficiency).
4. Format results.

### Result Format

```
[get_ruling_citations: {rulingId}]

Outbound Citations (this ruling cites):
1. {ExternalAlias} → Case: {TargetCaseTitle} | ID: {TargetRulingId} | Type: {CitationType}
2. {ExternalAlias} → (unresolved)
...

Inbound Citations (cited by):
1. Case: {SourceCaseTitle} | ID: {SourceRulingId} | Date: {Date} | Type: {CitationType}
...

Total: {outbound} outbound, {inbound} inbound.
```

---

## 4. get_related_rulings

**When to use**: User asks for rulings similar to a specific ruling, or wants to explore related jurisprudence.

### JSON Schema

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
        "description": "Max results (1-10). Default: 5."
      }
    },
    "required": ["rulingId"]
  }
}
```

### Backend Mapping

1. Call `ISearchService.SearchRelatedAsync(rulingId, topK ?? 5)`.
2. Format each `SearchResultItem`.

### Result Format

```
[get_related_rulings: {topK} results for ruling {rulingId}]

1. Case: {CaseTitle} | ID: {RulingId} | Date: {RulingDate} | Score: {Score:F3}
   Court: {Court} | Summary: {Summary}

2. ...
```

---

## 5. search_by_statute

**When to use**: User asks about rulings that cite a specific law, statute, or article (e.g. "art. 75 inc. 22 de la CN", "Ley 24.240").

### JSON Schema

```json
{
  "name": "search_by_statute",
  "description": "Find rulings that cite a specific law or statute. Use when the user asks about rulings applying or interpreting a particular legal norm.",
  "parameters": {
    "type": "object",
    "properties": {
      "statuteName": {
        "type": "string",
        "description": "Name or description of the statute/law. Supports partial match."
      },
      "statuteNumber": {
        "type": "string",
        "description": "Official number of the statute (e.g. 26.994, 24.240)."
      },
      "articles": {
        "type": "string",
        "description": "Specific articles cited (e.g. art. 75 inc. 22). Narrows results."
      },
      "topK": {
        "type": "integer",
        "description": "Max results (1-20). Default: 10."
      }
    },
    "required": ["statuteName"]
  }
}
```

### Backend Mapping

1. **New method**: `IStatuteRepository.FindRulingsByStatuteAsync(statuteName, statuteNumber?, articles?, topK)`.
2. SQL query joins `Statutes` → `RulingStatutes` → `Rulings`:
   - `WHERE s.Name LIKE '%{statuteName}%'` (case-insensitive)
   - Optional: `AND s.Number = '{statuteNumber}'`
   - Optional: `AND rs.Articles LIKE '%{articles}%'`
   - `ORDER BY r.RulingDate DESC`
   - `TOP {topK}`
3. Returns list of `(Ruling, StatuteName, Articles)` tuples.

### Result Format

```
[search_by_statute: {N} rulings citing "{statuteName}"]

1. Case: {CaseTitle} | ID: {RulingId} | Date: {RulingDate}
   Court: {Court}
   Statute: {StatuteName} (No. {Number}) — Articles: {Articles}
   Summary: {Summary}

2. ...
```

---

## 6. count_rulings

**When to use**: User asks quantitative questions ("how many", "cuántos") about rulings matching criteria.

### JSON Schema

```json
{
  "name": "count_rulings",
  "description": "Count rulings matching filters. Use for quantitative questions. Returns a count, not the rulings themselves.",
  "parameters": {
    "type": "object",
    "properties": {
      "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area." },
      "instance": { "type": "string", "description": "Filter by instance." },
      "courtName": { "type": "string", "description": "Filter by court name." },
      "dateFrom": { "type": "string", "format": "date", "description": "Earliest date." },
      "dateTo": { "type": "string", "format": "date", "description": "Latest date." },
      "isUnconstitutional": { "type": "boolean", "description": "Filter by unconstitutionality flag." },
      "keywords": { "type": "array", "items": { "type": "string" }, "description": "Filter by keywords." }
    }
  }
}
```

### Backend Mapping

1. **New method**: `IRulingRepository.CountAsync(CountFilters filters)`.
2. SQL `SELECT COUNT(*) FROM Rulings r` with dynamic `WHERE` clauses:
   - `r.JurisdictionArea`, `r.Instance` via `Court` join, `r.RulingDate` range, `r.IsUnconstitutional`.
   - Keywords: join `RulingKeywords rk` → `Keywords k` with `k.Description IN (...)`.
   - Court name: join `Courts c` with `c.Name LIKE '%{courtName}%'`.

### Result Format

```
[count_rulings: {count} rulings]
Filters applied: {jurisdictionArea}, {instance}, {dateRange}, ...
```

---

## 7. list_courts

**When to use**: User asks what courts are available, or which courts have rulings on a topic.

### JSON Schema

```json
{
  "name": "list_courts",
  "description": "List courts in the knowledge base. Use when the user asks about available courts or needs to discover court names for filtering.",
  "parameters": {
    "type": "object",
    "properties": {
      "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area." },
      "instance": { "type": "string", "description": "Filter by instance." }
    }
  }
}
```

### Backend Mapping

1. Query `Courts` table with optional `WHERE` filters on `JurisdictionArea` and `Instance`.
2. Cap at 50 results, ordered by `Name`.

### Result Format

```
[list_courts: {N} courts]

1. {Name} | Area: {JurisdictionArea} | Territory: {Territory} | Instance: {Instance}
2. ...
```

---

## 8. list_judges

**When to use**: User asks about judges who have participated in rulings, or wants to find judges by court.

### JSON Schema

```json
{
  "name": "list_judges",
  "description": "List judges who have participated in rulings. Use when the user asks about judges or wants to find who has ruled on specific topics.",
  "parameters": {
    "type": "object",
    "properties": {
      "courtName": { "type": "string", "description": "Filter by court name." }
    }
  }
}
```

### Backend Mapping

1. Query `Judges` + `RulingJudges` (+ optional `Courts` join for court filter).
2. Group by judge, count ruling participations.
3. Order by ruling count descending. Cap at 50 results.

### Result Format

```
[list_judges: {N} judges]

1. {FirstName} {LastName} | Rulings: {Count}
2. ...
```

---

## Parameter Validation Rules

| Rule | Behavior |
|---|---|
| `rulingId` is not a valid UUID | Return error: `"Invalid ruling ID format."` |
| `dateFrom` / `dateTo` not valid dates | Ignore invalid values, log warning |
| `topK` < 1 or > 20 | Clamp to [1, 20] |
| `query` is empty (search_rulings) | Return error: `"Query is required."` |
| `statuteName` is empty (search_by_statute) | Return error: `"Statute name is required."` |

---

## References

- `docs/design/f1-17-chat-tools-architecture.md` — agentic loop architecture (E215)
- `docs/architecture/legal-ai-ar-architecture.md` — data model §4, search indexes §4.12
- `docs/architecture/legal-ai-ar-specs.md` — API endpoints §7, repositories §2.2
- `docs/design/f1-7-chat-rag.md` — current RAG pipeline (E076)
- WI files: `WI-2026-03-20-tool-search-rulings.md`, `WI-2026-03-20-tool-ruling-detail-nav.md`, `WI-2026-03-20-tool-search-statute.md`, `WI-2026-03-20-tool-aggregation.md`
