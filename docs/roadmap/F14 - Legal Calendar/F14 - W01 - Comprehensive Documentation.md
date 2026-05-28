# F14 - W01 - Comprehensive Documentation

> **Feature:** F14 - Legal Calendar
> **Release:** 2.0 | **Sprint:** S07
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

Calendar view (month/week/day) with deadlines, hearings, and due dates. Filters by lawyer, court venue, case file.

---

## 2. Architecture Diagram

```mermaid
graph TB
    subgraph "Calendario Legal"
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

> The specific endpoints will be defined based on the features document: `docs/roadmap/features.md`, API Endpoints section.

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
| F14-W01 | Comprehensive Documentation | doc | S07 |
| F14-W02 | Backend - GET Aggregated Calendar Endpoint | backend | S07 |
| F14-W03 | Frontend - FullCalendar Angular Integration | frontend | S07 |
| F14-W04 | Frontend - Calendar Filters and Navigation | frontend | S07 |
| F14-W05 | Testing - Calendar Tests | testing | S07 |

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

*F14 - Legal Calendar — Comprehensive Documentation — Legal Ai Ar*
