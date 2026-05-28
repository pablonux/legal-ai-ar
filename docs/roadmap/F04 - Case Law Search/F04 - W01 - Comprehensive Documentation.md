# F04 - W01 - Comprehensive Documentation

> **Feature:** F04 - Case Law Search
> **Release:** 1.0 | **Sprint:** S03
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

Semantic search of court rulings with filters by court, court venue, date, and topic keywords.

---

## 2. Architecture Diagram

```mermaid
graph TB
    subgraph "Case Law Search"
        FE[Frontend Angular 19]
        BE[Backend .NET 10]
        DB[(Azure SQL + Graph)]
        AIS[Azure AI Search]
    end
    FE -->|HTTP/SignalR| BE
    BE --> DB
    BE --> AIS
```

---

## 3. Data Model

> Define the specific data model during the W01 implementation.
> Refer to the ontology in `docs/ontology/argentine-legal-ontology.md` for the base classes.

---

## 4. API Endpoints

| Method | Endpoint | Request Body | Response |
|--------|----------|-------------|----------|
| POST | `/api/search/case-law` | `{query, filters: {tribunal?, fuero?, dateFrom?, dateTo?, keywords?[]}, page, pageSize}` | `{total, items: [{id, caption, tribunal, fecha, excerpt, citedArticles[], score}], facets: {tribunal: [], fuero: []}}` |

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
| F04-W01 | Comprehensive Documentation | doc | S03 |
| F04-W02 | Backend - AI Search Index for Case Law | backend | S03 |
| F04-W03 | Backend - POST Search Case Law Endpoint | backend | S03 |
| F04-W04 | Frontend - Case Law Search Page | frontend | S03 |
| F04-W05 | Frontend - Ruling Result Card | frontend | S03 |
| F04-W06 | Testing - Case Law Search Tests | testing | S03 |

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

*F04 - Case Law Search — Comprehensive Documentation — Legal Ai Ar*
