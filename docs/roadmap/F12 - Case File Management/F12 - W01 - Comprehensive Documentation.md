# F12 - W01 - Comprehensive Documentation

> **Feature:** F12 - Case File Management
> **Release:** 2.0 | **Sprint:** S05-S06
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

CRUD completo de expedientes judiciales y administrativos. Movimientos, documentos adjuntos, partes, abogado responsable.

---

## 2. Architecture Diagram

```mermaid
erDiagram
    Expediente ||--o{ Movimiento : tiene
    Expediente ||--o{ DocumentoAdjunto : tiene
    Expediente ||--o{ PlazoProcesal : tiene
    Expediente }o--|| OrganoJudicial : tramitaEn
    Expediente }o--|| Abogado : responsable
    Expediente }o--o{ SujetoDeDeRecho : partes

    Expediente {
        int Id PK
        string NumeroExpediente
        string Caratula
        string Fuero
        string Estado
        date FechaInicio
        int AbogadoId FK
        int TribunalId FK
    }
    Movimiento {
        int Id PK
        int ExpedienteId FK
        date Fecha
        string Tipo
        string Description
    }
    DocumentoAdjunto {
        int Id PK
        int ExpedienteId FK
        string Nombre
        string BlobUrl
        date FechaSubida
    }
```

---

## 3. Data Model

### Tablas (Azure SQL)

**Expediente**

| Column | Type | Nullable | Description |
|---------|------|:--------:|-------------|
| Id | int (PK, identity) | No | ID autogenerado |
| CaseFileNumber | nvarchar(100) | No | Unique case file number |
| Caption | nvarchar(500) | No | Case caption |
| TipoProcesal | nvarchar(50) | No | civil/penal/laboral/familia/contencioso |
| CourtVenue | nvarchar(50) | No | Court venue |
| Estado | nvarchar(50) | No | iniciado/en_tramite/en_sentencia/apelado/concluido |
| FechaInicio | date | No | Fecha de inicio |
| ClosedAt | date | Yes | Closing date (if applicable) |
| Jurisdiccion | nvarchar(50) | No | federal/provincial/CABA |
| CourtId | int (FK) | Yes | FK to JudicialBody (Graph Node) |
| AbogadoResponsableId | int (FK) | No | FK a UsuarioPreferencias |
| Notes | nvarchar(max) | Yes | Internal notes |
| CreatedAt | datetime2 | No | Creation timestamp |
| UpdatedAt | datetime2 | No | Last update timestamp |

**Movimiento**

| Column | Type | Nullable | Description |
|---------|------|:--------:|-------------|
| Id | int (PK, identity) | No | ID autogenerado |
| ExpedienteId | int (FK) | No | FK a Expediente |
| Fecha | date | No | Fecha del movimiento |
| Tipo | nvarchar(50) | No | demanda/contestacion/prueba/sentencia/recurso/otro |
| Description | nvarchar(2000) | No | Movement description |
| CreadoPor | nvarchar(128) | No | EntraObjectId del creador |
| CreatedAt | datetime2 | No | Timestamp |

**DocumentoAdjunto**

| Column | Type | Nullable | Description |
|---------|------|:--------:|-------------|
| Id | int (PK, identity) | No | ID autogenerado |
| ExpedienteId | int (FK) | No | FK a Expediente |
| NombreArchivo | nvarchar(256) | No | Nombre original del archivo |
| BlobUrl | nvarchar(1000) | No | URL en Blob Storage |
| ContentType | nvarchar(100) | No | MIME type |
| SizeBytes | bigint | No | Size in bytes |
| SubidoPor | nvarchar(128) | No | EntraObjectId |
| FechaSubida | datetime2 | No | Timestamp |

---

## 4. API Endpoints

| Method | Endpoint | Request | Response |
|--------|----------|---------|----------|
| GET | `/api/case-files` | `?fuero=laboral&estado=en_tramite&abogado=id&page=1&pageSize=20` | `{total, items: [Expediente]}` |
| POST | `/api/case-files` | `{numeroExpediente, caption, fuero, ...}` | `{id, ...}` (201 Created) |
| GET | `/api/case-files/{id}` | - | `Expediente completo` |
| PUT | `/api/case-files/{id}` | `{campos a actualizar}` | `Expediente actualizado` |
| DELETE | `/api/case-files/{id}` | - | 204 No Content |
| GET | `/api/case-files/{id}/movimientos` | `?page&pageSize` | `{total, items: [Movimiento]}` |
| POST | `/api/case-files/{id}/movimientos` | `{fecha, tipo, descripcion}` | `Movimiento` (201) |
| GET | `/api/case-files/{id}/documentos` | - | `[DocumentoAdjunto]` |
| POST | `/api/case-files/{id}/documentos` | `multipart/form-data` | `DocumentoAdjunto` (201) |

---

## 5. UI / UX Description

### Pantallas

1. **Case file list** — DataTable with columns: Case File No., Caption, Court Venue, Status, Lawyer, Start Date. Top filters. "+ Nuevo expediente" button.

2. **Detalle de expediente** — Tabs: Info General | Movimientos | Documentos | Plazos | Notas

3. **Form** — Reactive form with: Case File No., Caption, Procedure Type (select), Court Venue (select), Jurisdiction (select), Court (autocomplete), Responsible Lawyer (select), Notes (textarea).

```
┌─────────────────────────────────────────────────────────┐
│  Expedientes                              [+ Nuevo]     │
├─────────────────────────────────────────────────────────┤
│  Fuero: [Todos ▼]  Estado: [Todos ▼]  Abogado: [Todos]│
├──────┬──────────────┬────────┬──────────┬───────┬──────┤
│  N°  │ Carátula     │ Fuero  │ Estado   │ Abog. │ Fecha│
├──────┼──────────────┼────────┼──────────┼───────┼──────┤
│ 1234 │ García c/... │ Civil  │ En trám. │ JPérez│ 03/24│
│ 5678 │ López s/...  │ Laboral│ Sentencia│ MRuiz │ 01/25│
│ 9012 │ ARCA c/...   │ Tribut.│ Apelado  │ JPérez│ 06/25│
├──────┴──────────────┴────────┴──────────┴───────┴──────┤
│  ◀ 1 2 3 ... 8 ▶                     20 resultados/pág│
└─────────────────────────────────────────────────────────┘
```

---

## 6. Acceptance Criteria

- [ ] Se pueden crear, leer, actualizar y eliminar expedientes
- [ ] La lista de expedientes permite filtrar por fuero, estado y abogado responsable
- [ ] La lista permite ordenar por cualquier columna
- [ ] Pagination works correctly
- [ ] Movements can be added to a case file with date, type, and description
- [ ] Movements are shown in a chronological timeline
- [ ] Se pueden subir documentos (PDF, DOCX) que se almacenan en Blob Storage
- [ ] Los documentos se pueden descargar
- [ ] Los administrativos pueden leer expedientes y agregar movimientos pero NO eliminar
- [ ] The search field searches by case file number and caption

---

## 7. Dependencies

- **Depends on:** F01 (Auth)
- **Blocks:** F13 (Plazos), F14 (Calendario)
- **NuGet:** Azure.Storage.Blobs (para documentos adjuntos)
- **npm:** @angular/material (DataTable, Forms)

---

## 8. Technical Notes

- Usar EF Core 10 con Fluent API para configurar las relaciones
- Los documentos se suben a Blob Storage con nombre: `expedientes/{id}/{guid}_{filename}`
- Use short-lived SAS tokens for download URLs
- Implement soft delete for case files (IsDeleted field + EF Core global filter)
- Caption search uses SQL Server full-text search `CONTAINS`
- Los movimientos son append-only (no se editan ni eliminan)

---

## 9. Work Items of this Feature

| ID | Name | Type | Sprint |
|----|--------|------|--------|
| F12-W01 | Comprehensive Documentation | doc | S05-S06 |
| F12-W02 | Backend - EF Core Case File Model and Migrations | backend | S05-S06 |
| F12-W03 | Backend - Case File CRUD Endpoints | backend | S05-S06 |
| F12-W04 | Backend - Movements and Documents Subresources | backend | S05-S06 |
| F12-W05 | Backend - Upload Documents to Blob Storage | backend | S05-S06 |
| F12-W06 | Frontend - Case File List with DataTable | frontend | S05-S06 |
| F12-W07 | Frontend - Case File Create and Edit Form | frontend | S05-S06 |
| F12-W08 | Frontend - Case File Detail with Tabs | frontend | S05-S06 |
| F12-W09 | Frontend - Movements Timeline | frontend | S05-S06 |
| F12-W10 | Frontend - Attached Documents Management | frontend | S05-S06 |
| F12-W11 | Testing - Case File CRUD Tests | testing | S05-S06 |

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

*F12 - Case File Management — Comprehensive Documentation — Legal Ai Ar*
