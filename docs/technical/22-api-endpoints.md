# API Endpoints — Minimal API Groups

> **Status:** F00-W10 merged to `main` ([PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112), 2026-06-02).
> Wave 1–2 public HTTP types live in `LegalAiAr.Contracts`; endpoints use `IEndpoint` discovery under
> `LegalAiAr.Api/Endpoints/`.

## LegalAiAr.Contracts

Public request/response types for the HTTP API (no references to Application, Infrastructure, or Core).

```
backend/src/shared/LegalAiAr.Contracts/
├── Requests/{Feature}/     # e.g. Rulings/SearchRulingsRequest
└── Responses/{Feature}/    # e.g. Auth/MeResponse, Statutes/StatutePageResponse
```

| Layer           | Role                                                                                    |
| --------------- | --------------------------------------------------------------------------------------- |
| **Contracts**   | Stable OpenAPI schemas; referenced by Api and Application                               |
| **Api**         | Binds Contract requests; maps to commands/queries; optional `IValidator<T>` on requests |
| **Application** | Commands, queries, handlers; maps domain/Core types to Contract responses               |

**Migrated in W10 (wave 1–2):** `auth`, `rulings`, `statutes`, `courts`, `persons`, `thesaurus`, `search`.
Admin, chat, ontology, graph, proceedings, and stats still use `Api/Models` or Application DTOs until migrated incrementally.

**Transactional side effects (F00-W12):** domain events and the outbox are not exposed as HTTP endpoints. See
[23 — Transactional Outbox & Domain Events](./23-outbox-domain-events.md).

**Versioning (R1):** additive changes only — new optional properties or new endpoints; no breaking renames or removals without a new API version.

## Discovery

- Registration: `AddLegalAiArEndpoints()` / `MapLegalAiArEndpoints()` in `Program.cs`
- Group prefix: `api/{groupName}` (lowercase; nested groups use slashes, e.g. `admin/users`)
- Default authorization: authenticated user (fallback policy)
- Exceptions: `GET /api/health/live` is anonymous; `admin/ruling-reprocess` group uses `AdminOnly`
- Per-route override: proceedings admin maintenance routes use `RequireAuthorization("AdminOnly")`

## Endpoint groups

| Group                    | Routes (summary)                                                     | Policy                              |
| ------------------------ | -------------------------------------------------------------------- | ----------------------------------- |
| `auth`                   | `GET /me`, `POST /logout`                                            | Authenticated                       |
| `health`                 | `GET /live` (anonymous), `GET /`                                     | Mixed                               |
| `rulings`                | search, by id, document, related, facets                             | Authenticated                       |
| `statutes`               | list, pyramid, by id                                                 | Authenticated                       |
| `courts`                 | list, by id                                                          | Authenticated                       |
| `persons`                | list, by id                                                          | Authenticated                       |
| `thesaurus`              | search, roots, children, by id                                       | Authenticated                       |
| `chat`                   | `POST /` (SSE)                                                       | Authenticated                       |
| `search`                 | `GET /` global search                                                | Authenticated                       |
| `stats`                  | `GET /kb`                                                            | Authenticated                       |
| `ontology`               | classes, graph, stats, taxonomies                                    | Authenticated                       |
| `graph`                  | neighborhood, entity search                                          | Authenticated                       |
| `proceedings`            | list, detail, by ruling, appeal chain; admin maintenance POST/DELETE | Mixed (`AdminOnly` on admin routes) |
| `admin/users`            | CRUD                                                                 | Authenticated                       |
| `admin/dlq`              | list, requeue                                                        | Authenticated                       |
| `admin/crawlers`         | list, by id, run, patch                                              | Authenticated                       |
| `admin/infra`            | storage probe                                                        | Authenticated                       |
| `admin/workers`          | pause, resume, status                                                | Authenticated                       |
| `admin/ruling-reprocess` | list, enqueue, retry                                                 | **AdminOnly**                       |
| `admin`                  | pipeline, jobs, documents                                            | Authenticated                       |

Implementation: one sealed class per route in `LegalAiAr.Api/Endpoints/**`, implementing `IEndpoint`.
See [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) §4.4.

Product-level route tables (legacy) remain in [`10-system-architecture.md`](10-system-architecture.md) §5; prefer this doc for group layout.
