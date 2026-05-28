# Knowledge Graph Explorer — UX and API Specification

| Field | Value |
|---|---|
| **ID** | E260–E263 |
| **Feature** | F1-20 · Explorador interactivo de relaciones de la KB |
| **Date** | 2026-04-30 |

---

## Purpose

This document specifies the Knowledge Graph Explorer: an interactive graph visualization of real KB entity relationships (rulings, courts, persons, statutes, proceedings, citations, norm relations). Users can start from any entity and progressively expand the graph by clicking nodes. Entry points: a new "Grafo" tab in ruling detail and a standalone `/explorador` page with entity search.

**Reference**: `docs/ontology/legal-ai-ar-ontology.md` (ontological model v2.1); `docs/design/f1-19-ontology-viewer.md` (schema viewer); `docs/design/f2-1-ui-restructure.md` (UI restructure); `docs/architecture/legal-ai-ar-architecture.md`.

---

## 1. Routes

| Route | Component | Description |
|---|---|---|
| `/explorador` | GraphExplorerPageComponent | Standalone graph explorer with entity search and layers panel |
| `/jurisprudencia/:id` | RulingDetailComponent (new "Grafo" tab) | Inline graph starting from the current ruling |

---

## 2. API Endpoints

### 2.1 GET /api/graph/neighborhood/{entityType}/{entityId}

Returns 1-hop neighborhood for any entity. Progressive expansion: the frontend calls this repeatedly as the user double-clicks nodes.

**Path params**:
- `entityType`: `ruling` | `court` | `person` | `statute` | `keyword` | `proceeding`
- `entityId`: string (Guid for rulings, int for others)

**Response** `200 OK`: `NeighborhoodResponse` with center node, neighbor nodes, and edges.

**Edges loaded per entity type**:

| Center | Outbound edges | Inbound edges |
|---|---|---|
| ruling | cites (→ ruling), citesStatute (→ statute), signedBy (→ person), issuedBy (→ court), hasKeyword (→ keyword), inProceeding (→ proceeding) | citedBy (← ruling, max 20) |
| court | — | issuedBy (← rulings, top 20 recent), memberOf (← persons) |
| person | memberOf (→ court) | signedBy (← rulings, top 20), party (← proceedings, top 20) |
| statute | normRelation (→/← statutes) | citesStatute (← rulings, top 20) |
| keyword | — | hasKeyword (← rulings, top 20) |
| proceeding | filedAt (→ court), hasParty (→ person) | inProceeding (← rulings) |

### 2.2 GET /api/graph/search?q={query}&types={types}

Searches entities by name for the "add to graph" feature.

- `q`: free text (min 2 chars)
- `types`: optional comma-separated filter (`ruling,court,person,statute,proceeding`)
- **Response**: list of `EntitySearchResult` (max 20)

---

## 3. Node and Edge Types

### 3.1 Node styling

| entityType | Color | Shape | Label source |
|---|---|---|---|
| ruling | `#ea580c` (coral) | round-rectangle | caseTitle |
| court | `#2563eb` (blue) | hexagon | name |
| person | `#16a34a` (green) | ellipse | "LastName, FirstName" |
| statute | `#7c3aed` (violet) | round-rectangle | "Ley {number}" |
| keyword | `#6b7280` (gray) | round-rectangle (small) | description |
| proceeding | `#0891b2` (cyan) | diamond | fileNumber |

### 3.2 Edge styling

| Edge type | Style | Label | Arrow |
|---|---|---|---|
| cites / citedBy | solid coral | CitationType (CITES, UPHOLDS, etc.) | directed |
| citesStatute | dashed violet | articles (e.g. "art. 14") | directed |
| signedBy | dashed green | ParticipationType (MAJORITY, DISSENT) | directed |
| issuedBy | solid blue | — | directed |
| hasKeyword | dotted gray | — | directed |
| normRelation | dashed violet | NormRelationType (DEROGATES, AMENDS) | directed |
| memberOf | solid green | — | directed |
| inProceeding | solid cyan | — | directed |
| filedAt | dashed cyan | — | directed |
| hasParty | dashed cyan | role (ACTOR, DEMANDADO) | directed |

---

## 4. GraphExplorerComponent

### 4.1 Behavior

- **Inputs**: `initialEntityType`, `initialEntityId`
- **On init**: loads neighborhood for initial entity, renders graph
- **Double-click node**: loads neighborhood for that node, merges into existing graph (no duplicates)
- **Single-click node**: emits `entitySelected` for external panels
- **Layout**: `cose` (force-directed) by default for instance graphs. Fit-to-screen button.
- **State**: internal `Map` of nodes and edges, accumulated across expansions

### 4.2 Visual indicators

- Center/root node: thicker border
- Expanded nodes: subtle glow or border indicator
- Loading state: spinner overlay on the expanding node
- Node tooltip on hover: label + subtitle

---

## 5. GraphLayersPanel (standalone page only)

### 5.1 Controls

| Control | Behavior |
|---|---|
| Node type toggles | Checkboxes for: Fallos, Tribunales, Personas, Normas, Procesos, Keywords. Hides/shows nodes + connected edges. |
| "Quitar nodo" button | Visible when a node is selected. Removes it and its exclusive edges. |
| "Limpiar grafo" button | Resets to center node only. |
| Node/edge counters | "X nodos · Y relaciones" |

### 5.2 Interaction

Layer toggles apply CSS visibility via Cytoscape selectors — data stays in memory for instant re-show.

---

## 6. EntitySearchPopover

- Text input with 300ms debounce
- Calls `GET /api/graph/search?q=...`
- Results grouped by type with icons
- Click result → add to graph + load its neighborhood
- Used in standalone page header and as floating action in the graph

---

## 7. Integration in RulingDetailComponent

New tab "Grafo" after "Relaciones":
- Shows `GraphExplorerComponent` with `initialEntityType="ruling"` and `initialEntityId` of the current ruling
- Simplified: no layers panel, no entity search
- Click on a ruling node → navigate to `/jurisprudencia/{id}`
- Click on court → navigate to `/organismos/{id}`
- Click on person → navigate to `/sujetos/{id}`
- Click on proceeding → navigate to `/procesos/{id}` (when F3 is implemented)

---

## 8. Error Handling

| Scenario | UX |
|---|---|
| 404 entity not found | Toast "Entidad no encontrada" |
| Network error on expand | Toast with retry, node reverts to unexpanded state |
| Empty neighborhood | Node marked as "leaf" (no expand indicator) |

---

## 9. References

- `docs/ontology/legal-ai-ar-ontology.md` — Ontological model v2.1
- `docs/design/f1-19-ontology-viewer.md` — Schema viewer (separate feature)
- `docs/design/f1-20-graph-data-model.mermaid` — Entity graph data model
- `docs/design/f2-1-ui-restructure.md` — UI restructure spec (routes)
