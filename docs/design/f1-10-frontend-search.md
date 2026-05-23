# Frontend Search — UX Specification

| Field | Value |
|---|---|
| **ID** | E095 |
| **Feature** | F1-10 · Frontend — Búsqueda y detalle de fallos |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the UX of the search module: SearchHome, SearchResults and RulingDetail views, including fields, states (loading, empty, error), and filter behavior. It serves as the design reference for T-01 to T-08 and is consumed by Angular developers implementing the search flow.

**Reference**: Architecture section 6 (Angular SPA); E067 (API Rulings); specs section 8.

---

## 1. Routes

| Route | Component | Description |
|---|---|---|
| `/buscar` | SearchHomeComponent | Search entry: query input + filters |
| `/buscar/resultados` | SearchResultsComponent | Paginated results list |
| `/fallos/:id` | RulingDetailComponent | Full ruling details |

**Navigation flow**: `/buscar` → submit → `/buscar/resultados?query=...&page=1` → click result → `/fallos/{id}`.

---

## 2. SearchHomeComponent (`/buscar`)

### 2.1 Layout

- **Search bar**: Prominent text input, placeholder "Buscar fallos en lenguaje natural...". Submit on Enter or button click.
- **Filters**: Collapsible panel (e.g. "Filtros avanzados"). Initially collapsed. Filters: jurisdictionArea, instance, courtId, dateFrom, dateTo, keywords.
- **Search button**: Primary CTA. Disabled when query is empty.

### 2.2 Fields

| Field | Type | UI Control | Notes |
|---|---|---|---|
| query | string | Text input | Required. Max 1000 chars. |
| jurisdictionArea | string | Select or autocomplete | Optional. Facet values from API or static list. |
| instance | string | Select or autocomplete | Optional. |
| courtId | int | Select | Optional. Courts dropdown. |
| dateFrom | date | Date picker | Optional. ISO 8601. |
| dateTo | date | Date picker | Optional. Must be >= dateFrom. |
| keywords | string[] | Multi-select or tag input | Optional. |

### 2.3 Validation

- Query required before submit.
- dateFrom ≤ dateTo. Show error if invalid.
- Clear filters: button to reset all filters to empty.

### 2.4 Submit Behavior

On submit: navigate to `/buscar/resultados` with query params (`query`, `page=1`, and any filters). SearchResultsComponent reads params and calls API.

---

## 3. SearchResultsComponent (`/buscar/resultados`)

### 3.1 Layout

- **Breadcrumb**: Buscar > Resultados
- **Query summary**: "Resultados para: {query}" (and active filters if any)
- **Results list**: Cards (RulingCardComponent) with pagination below
- **Filters sidebar** (optional): Same filters as SearchHome, pre-filled. Changing filters triggers new search (page reset to 1).

### 3.2 RulingCard Fields (per result)

| Field | Display |
|---|---|
| caseTitle | Title (link to `/fallos/{id}`) |
| rulingDate | Formatted date (e.g. dd/MM/yyyy) |
| jurisdictionArea, instance | Subtitle |
| summary | Truncated (~150 chars) with ellipsis |
| relevanceScore | Optional badge (e.g. "85% relevancia") |
| highlightedText | Optional. Relevant fragment in different style (e.g. highlighted) |

### 3.3 States

| State | UI |
|---|---|
| **idle** | Initial. No results shown. Prompt to search. |
| **loading** | Spinner or skeleton cards. Disable pagination. |
| **results** | Show cards. Pagination active. |
| **empty** | "No se encontraron fallos para tu búsqueda. Intenta con otros términos o filtros." |
| **error** | Error message (e.g. "Error al buscar. Intenta de nuevo."). Retry button. |

### 3.4 Pagination

- **Controls**: Previous, Next. Page numbers (e.g. 1 2 3 ... 5) if many pages.
- **Info**: "Mostrando X–Y de Z resultados"
- **Page size**: Default 10. Optional selector: 10, 25, 50 (max per API).
- **URL sync**: Query params `page`, `pageSize` reflect current state. Back/forward preserves state.

### 3.5 Empty Query

If user navigates to `/buscar/resultados` without `query`: redirect to `/buscar` or show empty state with prompt to search.

---

## 4. RulingDetailComponent (`/fallos/:id`)

### 4.1 Layout

Sections in order:

1. **Header**: caseTitle, caseNumber, rulingDate, court name
2. **Summary**: summary (full text)
3. **Holding**: holding (full text)
4. **Metadata**: jurisdictionArea, instance, jurisdiction, resourceType, rulingDirection, subjectArea, isUnconstitutional (badge if true)
5. **Judges**: List of "FirstName LastName (participationType)"
6. **Keywords**: Tags or list
7. **Statutes**: List with number, name, articles. Link if url present
8. **Citations**: List with externalAlias, citationType, link to `/fallos/{targetRulingId}` if targetRulingId present
9. **Full text**: Collapsible or expandable. fullText content
10. **Related rulings**: Section with links to related rulings (GET /api/rulings/{id}/related)

### 4.2 Fields Mapping

| API Field | Display |
|---|---|
| caseTitle | H1 |
| caseNumber | Subtitle |
| rulingDate | Formatted date |
| court.name | Court label |
| summary | Paragraph |
| holding | Paragraph |
| judges | "Apellido, Nombre (SIGNATORY/DISSENT/MAJORITY)" |
| keywords | Tag list |
| statutes | "Ley X — art. Y" with optional link |
| citations | "Fallos: 328:1883 (CITES)" — link if targetRulingId |
| fullText | Long text, collapsible |

### 4.3 States

| State | UI |
|---|---|
| **loading** | Spinner or skeleton |
| **loaded** | Full content |
| **error** | "Fallos no encontrado" (404) or "Error al cargar" (5xx). Link back to buscar |
| **not found** | 404. "El fallo solicitado no existe." |

### 4.4 Related Rulings

- **Section title**: "Fallos relacionados"
- **Content**: Cards or list with caseTitle, rulingDate, similarityScore. Link to `/fallos/{id}`.
- **Limit**: 10 (default from API). Optional "Ver más" if API supports.

---

## 5. Filter Behavior

### 5.1 Persistence

- Filters applied on SearchHome are passed via query params to SearchResults.
- SearchResults can show filters in sidebar. Editing filters triggers new search (page=1).
- Browser back/forward preserves query params.

### 5.2 Facet Values

Phase 1: Filters use free text or static options. Phase 2: Populate jurisdictionArea, instance, keywords from API facets (if endpoint exists).

---

## 6. Error Handling

| Scenario | UX |
|---|---|
| 401 Unauthorized | Redirect to /login. Store intended URL for post-login redirect. |
| 404 Ruling not found | RulingDetail: "Fallos no encontrado" with link to /buscar |
| 500 / Network error | Generic message. Retry button. |
| Validation error (400) | Inline validation message (e.g. dateFrom > dateTo) |

---

## 7. Accessibility

- Semantic HTML (headings, lists, links)
- Form labels associated with inputs
- Loading states announced (aria-live or similar)
- Keyboard navigation for filters and pagination

---

## 8. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 6 (Angular SPA)
- `docs/design/f1-6-api-rulings.md` — API schemas (E067)
- `docs/design/f1-10-components.mermaid` — Component tree (E096)
- `docs/design/f1-10-state-flow.mermaid` — State diagram (E097)
