# F13 - W01 - Comprehensive Documentation

> **Feature:** F13 - Deadline Management
> **Release:** 2.0 | **Sprint:** S06-S07
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

Registration and tracking of procedural deadlines. Automatic business-day calculation. Configurable alerts. Statuses with a visual traffic light.

---

## 2. Architecture Diagram

```mermaid
graph TB
    subgraph "Deadline Management"
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

### Tablas (Azure SQL)

**PlazoProcesal**

| Column | Type | Nullable | Description |
|---------|------|:--------:|-------------|
| Id | int (PK, identity) | No | ID autogenerado |
| ExpedienteId | int (FK) | No | FK a Expediente |
| Description | nvarchar(500) | No | Deadline description |
| TipoPlazo | nvarchar(50) | No | perentorio/ordenatorio |
| StartDate | date | No | Day from which counting starts |
| BusinessDays | int | No | Number of business days |
| FechaVencimiento | date | No | Fecha calculada de vencimiento |
| Estado | nvarchar(50) | No | pendiente/proximo/vencido/cumplido |
| FulfilledAt | date | Yes | Date it was fulfilled |
| AlertDays | nvarchar(50) | No | E.g.: "3,2,1" (days before to alert) |
| CreadoPor | nvarchar(128) | No | EntraObjectId |
| CreatedAt | datetime2 | No | Timestamp |

**FeriadoJudicial**

| Column | Type | Nullable | Description |
|---------|------|:--------:|-------------|
| Id | int (PK) | No | ID |
| Date | date | No | Holiday date |
| Description | nvarchar(200) | No | Holiday name |
| Tipo | nvarchar(50) | No | nacional/judicial |
| Jurisdiccion | nvarchar(50) | No | nacional/CABA/BsAs/etc |

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
| F13-W01 | Comprehensive Documentation | doc | S06-S07 |
| F13-W02 | Backend - EF Core Deadline Model and Migrations | backend | S06-S07 |
| F13-W03 | Backend - Deadline CRUD Endpoints | backend | S06-S07 |
| F13-W04 | Backend - Azure Function Daily Deadline Evaluation | backend | S06-S07 |
| F13-W05 | Backend - Alert Generation via Storage Queue | backend | S06-S07 |
| F13-W06 | Frontend - Deadline List with Visual Traffic Light | frontend | S06-S07 |
| F13-W07 | Frontend - Deadline Create Form with Business Days Calculation | frontend | S06-S07 |
| F13-W08 | Testing - Deadline and Business Days Calculation Tests | testing | S06-S07 |

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

*F13 - Deadline Management — Comprehensive Documentation — Legal Ai Ar*
