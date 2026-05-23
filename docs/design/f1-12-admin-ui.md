# Admin Panel — UX Specification

| Field | Value |
|---|---|
| **ID** | E107 |
| **Feature** | F1-12 · Frontend — Panel de administración |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the UX of the admin panel: Dashboard, Crawlers, Jobs, DLQ and Users views, including layout, states, and available actions. It serves as the design reference for T-01 to T-06 and is consumed by Angular developers implementing the admin module.

**Reference**: Architecture section 6 (admin routes); E083 (API Admin); specs section 8.

---

## 1. Routes and Layout

| Route | Component | Description |
|---|---|---|
| `/admin` | DashboardComponent | Pipeline status overview |
| `/admin/crawlers` | CrawlersComponent | Crawler list, trigger, config |
| `/admin/jobs` | JobsComponent | Active/completed/failed jobs |
| `/admin/dlq` | DeadLetterQueueComponent | DLQ messages by queue |
| `/admin/users` | UsersComponent | User CRUD |

### 1.1 Shared Layout

- **Sidebar or tabs**: Navigation between admin views. Highlight current route.
- **Breadcrumb**: Admin > [Current view]
- **Header**: "Administración" or similar

---

## 2. Dashboard (`/admin`)

### 2.1 Layout

- **Cards per source**: One card per crawler (CSJN, SAIJ, PJN, SCBA). Each card shows:
  - Source name
  - Last crawled at (formatted)
  - Last status (success / partial / failed) — color-coded
  - Last document count
  - Queue length (if available)
- **Summary**: Total documents indexed, total errors (if available from API).

### 2.2 States

| State | UI |
|---|---|
| **loading** | Skeleton cards or spinner |
| **loaded** | Cards with data |
| **error** | Error message. Retry button. |

### 2.3 Actions

- **Link to Crawlers**: "Ver crawlers" or similar. Navigate to `/admin/crawlers`.
- **Link to Jobs**: "Ver jobs". Navigate to `/admin/jobs`.
- **Link to DLQ**: "Ver cola de errores". Navigate to `/admin/dlq`.

### 2.4 Data Source

`GET /api/admin/pipeline/status`

---

## 3. Crawlers (`/admin/crawlers`)

### 3.1 Layout

- **Table**: Columns: Source, Enabled, Last Crawled, Status, Documents, Actions.
- **Enabled**: Toggle or badge (Activo / Inactivo). Click to PATCH.
- **Actions**: "Ejecutar ahora" button per row.

### 3.2 Trigger Modal

On "Ejecutar ahora":
- **Modal**: "Ejecutar crawl para {sourceName}"
- **Type**: Radio or select — `incremental` (default), `by-range`.
- **Since** (if incremental): Date picker. Optional; default LastCrawledAt.
- **DateFrom** and **DateTo** (if by-range): Date pickers. Required; both must be set and DateFrom ≤ DateTo.
- **Buttons**: Cancel, Confirm.
- On Confirm: `POST /api/admin/crawlers/{sourceId}/run`. Close modal. Refresh list. Show toast on success/error.

### 3.3 Enable/Disable

- Toggle or switch in table. On change: `PATCH /api/admin/crawlers/{sourceId}` with `{ isEnabled: value }`.
- Confirm if disabling: "¿Deshabilitar fuente {name}? No se ejecutarán crawls hasta que la habilites de nuevo."

### 3.4 States

| State | UI |
|---|---|
| **loading** | Skeleton or spinner |
| **loaded** | Table with data |
| **error** | Error message. Retry. |
| **updating** | Disable toggle/button while PATCH in progress |

### 3.5 Data Source

`GET /api/admin/crawlers`

---

## 4. Jobs (`/admin/jobs`)

### 4.1 Layout

- **Table**: Columns: Source, Type, Triggered By, Started, Completed, Status, Discovered, Indexed, Failed, Actions.
- **Status**: Badge — running (blue), completed (green), partial (yellow), failed (red).
- **Polling**: Refresh every 30 seconds. Optional manual refresh button.

### 4.2 Phase 1

Phase 1: No IngestionJobs. API may return `[]` or synthetic jobs from CrawlerConfigs.
- If empty: "No hay jobs registrados. Los jobs se crearán al ejecutar crawls." (Phase 2)
- If synthetic: Show last crawl per source as "job".

### 4.3 States

| State | UI |
|---|---|
| **loading** | Skeleton or spinner |
| **loaded** | Table |
| **empty** | "No hay jobs" message |
| **error** | Error. Retry. |

### 4.4 Data Source

`GET /api/admin/jobs`. Poll every 30s.

---

## 5. Dead Letter Queue (`/admin/dlq`)

### 5.1 Layout

- **Tabs**: One tab per queue — Crawler, Parser, Enrichment, Indexer.
- **Per tab**: Table of messages. Columns: ID (truncated), Inserted, Dequeue Count, Body Preview, Actions.
- **Actions**: "Reencolar" button per message.

### 5.2 Requeue

On "Reencolar":
- **Confirm**: "¿Reencolar este mensaje? Volverá a la cola de origen y será procesado de nuevo."
- On Confirm: `POST /api/admin/dlq/{queue}/{id}/requeue`. Remove row from list (or refresh). Show toast.

### 5.3 States

| State | UI |
|---|---|
| **loading** | Spinner per tab |
| **loaded** | Table. Empty state: "No hay mensajes en esta cola." |
| **error** | Error. Retry. |

### 5.4 Data Source

`GET /api/admin/dlq?queue={crawler|parser|enrichment|indexer}`. One request per active tab.

---

## 6. Users (`/admin/users`)

### 6.1 Layout

- **Table**: Columns: Email, Display Name, Role, Active, Actions (Edit, Deactivate).
- **Header**: "Crear usuario" button.

### 6.2 Create User Modal

On "Crear usuario":
- **Form**: Email (required), Display Name (optional), Role (required: admin / lawyer / viewer).
- **Buttons**: Cancel, Create.
- On Create: `POST /api/admin/users`. Close modal. Refresh list. Show toast.

### 6.3 Edit User Modal

On "Editar":
- **Form**: Display Name, Role. (Email not editable in Phase 1, or editable with validation.)
- **Buttons**: Cancel, Save.
- On Save: `PUT /api/admin/users/{id}`. Close modal. Refresh.

### 6.4 Deactivate

On "Desactivar":
- **Confirm**: "¿Desactivar usuario {email}? No podrá iniciar sesión."
- On Confirm: `DELETE /api/admin/users/{id}`. Refresh. Show toast.

### 6.5 States

| State | UI |
|---|---|
| **loading** | Skeleton or spinner |
| **loaded** | Table |
| **empty** | "No hay usuarios. Crea el primero." |
| **error** | Error. Retry. |

### 6.6 Data Source

`GET /api/admin/users`

---

## 7. Error Handling

| Scenario | UX |
|---|---|
| 401 | Redirect to /login (handled by AuthInterceptor) |
| 404 | "Recurso no encontrado." |
| 500 | "Error del servidor. Intenta más tarde." + Retry |
| Network error | "Error de conexión." + Retry |

---

## 8. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 6 (admin routes)
- `docs/design/f1-8-api-admin.md` — API spec (E083)
- `docs/design/f1-12-admin-components.mermaid` — Component tree (E108)
