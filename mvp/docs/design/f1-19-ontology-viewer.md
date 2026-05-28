# Ontology Viewer — UX and API Specification

| Field | Value |
|---|---|
| **ID** | E250–E253 |
| **Feature** | F1-19 · Visualizador interactivo de ontología legal |
| **Date** | 2026-04-30 |

---

## Purpose

This document specifies the interactive legal ontology viewer: a navigational hub that functions as a **map of the entire application**. Each ontology class that is implemented in the KB links directly to its corresponding UI space, allowing users to discover data through the ontological structure. The API serves the ontological model (classes, properties, relationships, controlled vocabularies) with live KB instance counts, and the frontend renders an interactive graph with Cytoscape.js plus a detail panel and taxonomy browser.

**Reference**: `docs/ontology/legal-ai-ar-ontology.md` (ontological model v2.1); `docs/architecture/legal-ai-ar-architecture.md` (architecture); `docs/architecture/legal-ai-ar-specs.md` (CQRS, Clean Architecture); `docs/design/f2-1-ui-restructure.md` (UI restructure).

---

## 1. Routes

| Route | Component | Description |
|---|---|---|
| `/ontologia` | OntologyPageComponent | Interactive ontology viewer with graph, detail panel and taxonomy browser |

**Navigation**: Sidebar link "Ontología" in the "Herramientas" group, below the ontological spaces (see `f2-1-ui-restructure.md` section 3).

---

## 2. API Endpoints

### 2.1 GET /api/ontology/classes

Returns the full class hierarchy with properties, relationships, and KB mapping metadata.

**Response** `200 OK`:

```json
{
  "classes": [
    {
      "id": "NormaJuridica",
      "name": "Norma Jurídica",
      "description": "Norma general, abstracta y coercitiva que integra el ordenamiento jurídico.",
      "namespace": "legar:NormaJuridica",
      "parentId": null,
      "category": "core",
      "kbEntity": "Statute",
      "kbRoute": null,
      "properties": [
        { "name": "identificador", "type": "string", "description": "Número o código oficial" },
        { "name": "tipo", "type": "NormType", "description": "Tipo de norma", "taxonomyId": "NormType" }
      ],
      "children": ["Constitucion", "Tratado", "Ley", "Decreto", "Resolucion", "Ordenanza", "Acordada"]
    }
  ]
}
```

### 2.2 GET /api/ontology/graph

Returns nodes and edges optimized for Cytoscape.js rendering.

**Response** `200 OK`:

```json
{
  "nodes": [
    {
      "id": "NormaJuridica",
      "label": "Norma Jurídica",
      "category": "core",
      "instanceCount": 1250,
      "kbRoute": null
    }
  ],
  "edges": [
    {
      "id": "e-NormaJuridica-Ley",
      "source": "NormaJuridica",
      "target": "Ley",
      "type": "is-a",
      "label": "is-a"
    },
    {
      "id": "e-NormaJuridica-deroga-NormaJuridica",
      "source": "NormaJuridica",
      "target": "NormaJuridica",
      "type": "relationship",
      "label": "deroga / modifica"
    }
  ]
}
```

Node `category` values: `core` (top-level class), `subclass`, `taxonomy` (controlled vocabulary), `kb-entity` (mapped to DB entity).

Edge `type` values: `is-a` (inheritance), `relationship` (domain relationship).

### 2.3 GET /api/ontology/stats

Returns instance counts per KB-mapped class with breakdown by taxonomy.

**Response** `200 OK`:

```json
{
  "entities": [
    {
      "classId": "Sentencia",
      "kbEntity": "Ruling",
      "totalCount": 1523,
      "breakdowns": [
        {
          "taxonomyId": "LegalBranch",
          "taxonomyName": "Rama del derecho",
          "values": [
            { "code": "PUB_CONST", "label": "Derecho constitucional", "count": 245 },
            { "code": "PRIV_CIVIL", "label": "Derecho civil", "count": 412 }
          ]
        }
      ]
    },
    {
      "classId": "Tribunal",
      "kbEntity": "Court",
      "totalCount": 87,
      "breakdowns": []
    }
  ]
}
```

### 2.4 GET /api/ontology/taxonomies/{taxonomyId}

Returns all values for a controlled vocabulary with instance counts.

**Path params**: `taxonomyId` — one of `LegalBranch`, `NormType`, `NormativeLevel`, `CourtType`, `PrecedentWeight`, `Fuero`, `InstanceLevel`, `GovernmentLevel`, `PersonType`, `ProcessType`, `ProcessStatus`, `LegalEntityType`.

**Response** `200 OK`:

```json
{
  "id": "LegalBranch",
  "name": "Rama del derecho",
  "description": "Ramas del derecho argentino organizadas en público, privado, social y digital.",
  "values": [
    {
      "code": "PUB_CONST",
      "label": "Derecho constitucional",
      "group": "Derecho público",
      "count": 245,
      "description": "CN 1994 — derechos y garantías, organización del Estado, control de constitucionalidad"
    }
  ]
}
```

**Error** `404`: Unknown taxonomy ID → ProblemDetails.

---

## 3. OntologyPageComponent (`/ontologia`)

### 3.1 Layout

Two-column layout: graph area (70%) + side panel (30%).

- **Header**: "Mapa de la Aplicación" title with subtitle "Navega la ontología legal argentina — cada nodo es un espacio de la KB."
- **Toolbar**: Layout toggle (hierarchical / force-directed), view toggle (graph / taxonomies), fit-to-screen button.
- **Graph area**: Cytoscape.js canvas filling the main area. Nodes that map to KB routes are **clickable entry points** — double-click navigates directly to the corresponding UI space.
- **Side panel**: Shows ClassDetailPanel when a node is selected, or TaxonomyBrowser in taxonomy view.

### 3.2 States

| State | UI |
|---|---|
| **loading** | Spinner centered in graph area. Side panel skeleton. |
| **loaded** | Graph rendered. Side panel shows instructions or selected class. |
| **error** | "Error al cargar la ontología. Intenta de nuevo." with retry button. |

---

## 4. OntologyGraphComponent

### 4.1 Graph Configuration

- **Library**: Cytoscape.js (via npm `cytoscape` package).
- **Layouts**:
  - `dagre` (hierarchical, default) — top-down class hierarchy.
  - `cose` (force-directed) — organic clustering by relationships.
- **Interaction**: Click node → emit `nodeSelected` event. Hover → tooltip with name + count. Zoom/pan enabled. Double-click → fit to node neighborhood.

### 4.2 Node Styling

| Category | Color | Shape | Size |
|---|---|---|---|
| `core` | `#2563eb` (blue-600) | Round rectangle | Base + log(instanceCount) |
| `subclass` | `#60a5fa` (blue-400) | Round rectangle | Base + log(instanceCount) |
| `taxonomy` | `#16a34a` (green-600) | Diamond | Fixed small |
| `kb-entity` | `#ea580c` (orange-600) | Ellipse | Base + log(instanceCount) |

Edge styling: `is-a` edges are solid gray; `relationship` edges are dashed with arrow and label.

### 4.3 Initial View

On load, show only the 10 core classes and their direct subclasses (depth 2). User can expand nodes to reveal deeper subclasses. Taxonomy nodes shown as small diamonds connected to their parent class property.

---

## 5. ClassDetailPanelComponent

### 5.1 Content

When a node is selected:

| Section | Content |
|---|---|
| **Name** | Class display name (e.g. "Norma Jurídica") |
| **Namespace** | `legar:NormaJuridica` |
| **Description** | From ontology model |
| **Properties** | Table: name, type, description. Types that reference taxonomies show as links. |
| **Relationships** | Outgoing and incoming relationships with target class name and cardinality. |
| **KB Mapping** | If mapped: entity name, instance count, "Ver datos →" link to the corresponding route. |
| **Taxonomy breakdown** | If the class has taxonomy breakdowns in stats, show a mini bar chart or sorted list with counts. |

### 5.2 KB Navigation Links

Each implemented ontology class maps to a UI space. The "Ver datos →" link in the detail panel navigates to the corresponding route. Double-clicking a `kb-entity` node in the graph also navigates directly.

| Ontology Class | KB Entity | Route | Status |
|---|---|---|---|
| Sentencia | Ruling | `/jurisprudencia` | Implementada |
| OrganoEstatal (Tribunal) | Court | `/organismos` | Implementada |
| SujetoDeDerecho | Person | `/sujetos` | Implementada |
| NormaJuridica | Statute | `/ordenamiento` | Planificada (F3) |
| ProcesoJudicial | JudicialProceeding | `/procesos` | Planificada (F3) |
| skos:Concept | ThesaurusTerm | `/vocabulario` | Implementada |

---

## 6. TaxonomyBrowserComponent

### 6.1 Layout

Accordion-style list of all controlled vocabularies. Each taxonomy expands to show:

- Taxonomy name and description
- Value table: code, label, group (if any), count, description
- Bar visualization for counts (proportional width)

### 6.2 Interaction

Click on a taxonomy value with count > 0 → navigate to search with that filter pre-applied (e.g. `/jurisprudencia/resultados?legalBranch=PUB_CONST`).

---

## 7. Backend Architecture

### 7.1 OntologyModelProvider

Singleton service registered in Application DI. Builds the ontology model in memory from code definitions (not from DB). Returns:

- Class hierarchy (from `docs/ontology/legal-ai-ar-ontology.md` section 2)
- Properties per class (sections 3.1–3.10)
- Relationships (section 4)
- Taxonomy definitions (section 5, mapped from existing C# enums)

### 7.2 Handlers

| Handler | Repository/Service | Notes |
|---|---|---|
| `GetOntologyClassesHandler` | `OntologyModelProvider` | Pure in-memory, no DB |
| `GetOntologyGraphHandler` | `OntologyModelProvider` + `IKbStatsRepository` | Model + counts for node sizing |
| `GetOntologyStatsHandler` | `AppDbContext` (direct) | COUNT queries with GROUP BY on enum columns |
| `GetTaxonomyValuesHandler` | `OntologyModelProvider` + `AppDbContext` | Model for metadata + DB for counts |

---

## 8. Error Handling

| Scenario | UX |
|---|---|
| 401 Unauthorized | Redirect to /login |
| 404 Unknown taxonomy | Toast/inline error in taxonomy browser |
| 500 / Network error | Full-page error state with retry |

---

## 9. Accessibility

- Cytoscape graph has `role="img"` with `aria-label` describing the ontology.
- Side panel content is fully keyboard-navigable.
- Taxonomy browser uses semantic headings and expandable sections with `aria-expanded`.
- Color-coded nodes also have distinct shapes for accessibility.

---

## 10. References

- `docs/ontology/legal-ai-ar-ontology.md` — Ontological model v2.1
- `docs/architecture/legal-ai-ar-architecture.md` — System architecture
- `docs/design/f1-19-ontology-graph-schema.mermaid` — Graph schema diagram (E251)
- `docs/design/f2-1-ui-restructure.md` — UI restructure spec (routes, sidebar)
