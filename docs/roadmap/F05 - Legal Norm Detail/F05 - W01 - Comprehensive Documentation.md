# F05 - W01 - Comprehensive Documentation

> **Feature:** F05 - Legal Norm Detail
> **Release:** 1.0 | **Sprint:** S03-S04
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

Complete view of a legal norm: metadata, navigable articles, amendment history, relationship graph.

---

## 2. Architecture Diagram

```mermaid
graph TB
    subgraph "Detalle de Norma"
        TAB1[Tab: Info General]
        TAB2[Tab: Articulado]
        TAB3[Tab: Historial]
        TAB4[Tab: Grafo]
    end
    subgraph Backend
        E1[GET /api/legal-norms/id]
        E2[GET /api/legal-norms/id/articulos]
        E3[GET /api/legal-norms/id/historial]
        E4[GET /api/legal-norms/id/grafo]
    end
    subgraph "Azure SQL"
        RT[Tablas Relacionales]
        GT[Graph Tables - MATCH]
    end
    TAB1 --> E1 --> RT
    TAB2 --> E2 --> RT
    TAB3 --> E3 --> GT
    TAB4 --> E4 --> GT
```

---

## 3. Data Model

> Define the specific data model during the W01 implementation.
> Refer to the ontology in `docs/ontology/argentine-legal-ontology.md` for the base classes.

---

## 4. API Endpoints

| Method | Endpoint | Params | Response |
|--------|----------|--------|----------|
| GET | `/api/legal-norms/{id}` | - | `{id, numero, denominacion, fechaSancion, estaVigente, ramaDelDerecho, textoCompleto, ...}` |
| GET | `/api/legal-norms/{id}/articulos` | `?page=1&pageSize=50` | `{total, items: [{numero, texto, vigente, incisos[]}]}` |
| GET | `/api/legal-norms/{id}/grafo` | `?profundidad=2` | `{nodos: [{id, tipo, label}], edges: [{source, target, tipo}]}` |
| GET | `/api/legal-norms/{id}/historial` | - | `{modificaciones: [{normaModificatoria, fecha, tipo}]}` |

---

## 5. UI / UX Description

> Define the UI mockups during implementation. Follow the Angular Material 19 + Tailwind CSS 4 guidelines.
> Refer to `docs/roadmap/features.md` for the functional UI description.

---

## 6. Acceptance Criteria

- [ ] The functionality described in the Description section is fully implemented
- [ ] The API endpoints return the expected data
- [ ] The UI is responsive and functional on desktop and tablet
- [ ] Unit tests cover > 80% of the new code
- [ ] The CI build passes with no errors
- [ ] The functionality is accessible (WCAG 2.1 AA)

---

## 7. Dependencies

- **Depends on:** F01 (Auth)
- **Refer to features.md** for detailed dependencies between features

---

## 8. Technical Notes

- Stack: Angular 19 (standalone components, signals) + .NET 10 Minimal API
- Database: Azure SQL with EF Core 10 + Graph Tables
- Search: Azure AI Search with hybrid scoring
- Auth: platform-managed Microsoft Entra SSO via `id_token` cookie (no MSAL); the API validates it (`Auth:Platform`)
- Real-time communication: SignalR
- Storage: Azure Blob Storage for documents
- Refer to the ontology (`docs/ontology/argentine-legal-ontology.md`) for the domain model

---

## 9. Work Items of this Feature

| ID | Name | Type | Sprint |
|----|--------|------|--------|
| F05-W01 | Comprehensive Documentation | doc | S03-S04 |
| F05-W02 | Backend - GET Legal Norm Detail Endpoint | backend | S03-S04 |
| F05-W03 | Backend - GET Legal Norm Graph SQL Graph Endpoint | backend | S03-S04 |
| F05-W04 | Backend - GET Legal Norm Articles Paged Endpoint | backend | S03-S04 |
| F05-W05 | Frontend - Legal Norm Detail Page with Tabs | frontend | S03-S04 |
| F05-W06 | Frontend - Relationship Graph Visualization | frontend | S03-S04 |
| F05-W07 | Frontend - Amendments Timeline | frontend | S03-S04 |
| F05-W08 | Testing - Legal Norm Detail Tests | testing | S03-S04 |

---

## 10. Definition of Done

- [ ] Code reviewed by at least 1 peer (PR approved)
- [ ] Unit tests with > 80% coverage
- [ ] Integration tests for endpoints
- [ ] No errors in the CI build
- [ ] API documentation updated (Swagger/OpenAPI)
- [ ] Angular components documented with JSDoc
- [ ] Accessibility validated (WCAG 2.1 AA)
- [ ] Responsive verified on desktop and tablet
- [ ] Performance: load time < 3 sec, API response < 2 sec
- [ ] Feature flag configured (if applicable)

---

*F05 - Legal Norm Detail — Comprehensive Documentation — Legal Ai Ar*
