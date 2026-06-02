# Frontend Architecture

> The Angular single-page application: structure, routing/navigation, authentication, the streaming
> chat client, and the graph explorer.
>
> This document describes the SPA as currently implemented. Route paths and UI labels are in Spanish
> (end-user contact layer); everything else follows the English code convention.
>
> **Baseline:** Extends the [PwC Internal Application Architecture — Reference Guide](../standards/pwc-internal-app-architecture.md) (§6 Frontend). Workspace libraries (`core`, `shell`, `ui`, `shared-common`) were introduced in **F0.0-W03**; full AppKit 4 shell adoption and feature thin/full slices remain follow-up work (W11+).

---

## 1. Overview

The SPA is an **Angular 19** standalone-components app (no NgModules), with **lazy-loaded** feature
routes behind a shared shell, and **cookie-based** authentication (`withCredentials`). Notable
libraries in use today:

| Concern                           | Library                                                             |
| --------------------------------- | ------------------------------------------------------------------- |
| Framework                         | Angular 19 (standalone components, signals)                         |
| Graph visualization               | **Cytoscape**                                                       |
| Markdown rendering (chat answers) | **marked**                                                          |
| Base theming                      | Angular Material prebuilt theme + custom global styles (PwC design) |

> **Target design system.** The adopted UI design system is **PwC AppKit 4**
> ([`docs/appkit4/`](../appkit4/README.md)); the current shell uses Material + custom CSS as a
> placeholder until `@appkit4/angular-components` is wired. New UI should follow AppKit — see the
> designer skill and [08 — Legal AI UX](08-legal-ai-ux.md).

---

## 2. Workspace structure (F0.0-W03)

```
frontend/
├── src/app/                    # Application host (features, domain services, models)
│   ├── app.config.ts
│   ├── app.routes.ts
│   ├── features/               # Lazy-loaded product areas (thin slice — unchanged layout)
│   ├── services/               # Domain API clients (+ services/admin/)
│   └── models/
├── projects/
│   ├── core/                   # Platform auth, interceptors, guards, environments
│   ├── shell/                  # ShellComponent, StatusBarComponent
│   ├── ui/                     # Thin UI wrappers (e.g. UiSectionComponent)
│   └── shared-common/          # Pipes, directives, domain-agnostic components
├── proxy.conf.json             # Local /api → http://localhost:5088
├── jest.config.cjs             # Unit tests (Jest)
└── e2e/                        # Playwright smoke tests
```

### Path aliases (`tsconfig.json`)

| Alias                          | Target                                                               |
| ------------------------------ | -------------------------------------------------------------------- |
| `@legal-ai-ar/core`            | `projects/core/src/public-api.ts`                                    |
| `@legal-ai-ar/shell`           | `projects/shell/src/public-api.ts`                                   |
| `@legal-ai-ar/ui`              | `projects/ui/src/public-api.ts`                                      |
| `@legal-ai-ar/shared-common`   | `projects/shared-common/src/public-api.ts`                           |
| `@legal-ai-ar/shared-common/*` | Deep imports under `shared/`                                         |
| `@legal-ai-ar/app/*`           | `src/app/*` (used by shell/shared widgets that call domain services) |

> **Note:** `command-palette`, `entity-preview`, and `onboarding-tip` live in `shared-common` but still
> depend on domain services under `src/app/services/` via `@legal-ai-ar/app/*`. Extracting a dedicated
> Use **`schema-la`** schematics (F0.0-W11) for new features; a dedicated `features` library is optional in R1+.

---

## 3. Routing & navigation

Navigation is organized around the lawyer's mental model — **ontological spaces** plus **tools** plus
**admin** — rendered by `ShellComponent` (`@legal-ai-ar/shell`). All app routes sit under the shell and
are protected by `authGuard` (`@legal-ai-ar/core`).

| Route                                                                    | View                                                         | Group             |
| ------------------------------------------------------------------------ | ------------------------------------------------------------ | ----------------- |
| `/bienvenida`                                                            | Welcome                                                      | —                 |
| `/jurisprudencia` · `/jurisprudencia/resultados` · `/jurisprudencia/:id` | Ruling search → results → detail                             | Ontological space |
| `/organismos` · `/organismos/:id`                                        | Courts list / detail                                         | Ontological space |
| `/sujetos` · `/sujetos/:id`                                              | Persons list / detail                                        | Ontological space |
| `/vocabulario` · `/vocabulario/:id`                                      | Thesaurus list / detail                                      | Ontological space |
| `/ordenamiento` · `/ordenamiento/piramide` · `/ordenamiento/:id`         | Statutes list / normative pyramid / detail                   | Ontological space |
| `/procesos` · `/procesos/:id`                                            | Proceedings list / detail (`/causas` redirects here)         | Ontological space |
| `/asistente`                                                             | AI chat assistant                                            | Tool              |
| `/explorador`                                                            | Graph explorer                                               | Tool              |
| `/estadisticas`                                                          | KB dashboard                                                 | Tool              |
| `/ontologia`                                                             | Ontology viewer                                              | Tool              |
| `/admin/**`                                                              | Ingestion, DLQ, reprocess queue, users, jobs, entity-sources | Admin             |

Most routes are lazy (`loadComponent`); a few high-traffic ones are eager. The admin area has its own
nested layout (`AdminLayoutComponent`).

---

## 4. Authentication

Cookie-session model aligned with the platform `id_token` (see
[GCaaS Hosting](../deployment/gcaas-hosting.md)). Implemented in **`@legal-ai-ar/core`**:

- **`authGuard`** (`CanActivateFn`) checks `AuthService.isAuthenticated()`; on failure it redirects to
  `environment.platformAuthFailurePath` (default `sesion-requerida`).
- **`platformCredentialsInterceptor`** adds `withCredentials: true` when
  `environment.usePlatformCredentials` is true (GCaaS / corporate host).
- **`errorInterceptor`** clears session and navigates on HTTP 401.
- `SessionRequiredComponent` (`/sesion-requerida`) handles the unauthenticated state; `login` redirects
  to `/bienvenida`. There is **no MSAL / app-owned login**.

---

## 5. Chat client (SSE)

`ChatService.stream(query)` consumes `POST /api/chat` using **`fetch()` + `ReadableStream`**
(`getReader` + `TextDecoder`) rather than `EventSource`, because the endpoint is a POST. It parses the
SSE protocol into a typed `Observable<ChatStreamEvent>` with event types `chunk`, `tool_start`,
`tool_end`, `validation`, `complete`, `error` — mapping the server's `event:`/`data:` lines (see
[16 — Chat, RAG & Agent Pipeline §2](16-chat-rag-agents.md)). An `AbortSignal` cancels the stream.
Answer text (Markdown with `{caso, id}` citations) is rendered via **marked**.

---

## 6. Graph explorer & visualizations

The `graph-explorer` feature renders the legal graph with **Cytoscape** (`graph-explorer.service.ts`),
letting users explore citations, participations, and communities. The ontology viewer and KB dashboard
provide additional visual surfaces (`ontology`, `kb-dashboard`).

---

## 7. Services layer

Singleton services under `src/app/services/` wrap the API per domain: `chat`, `ruling`, `court`,
`person`, `statute`, `thesaurus`, `proceeding`, `ontology`, `graph-explorer`, `stats`,
`global-search`, `command-palette`, `recent-searches`, `onboarding`, plus `services/admin/`
(`crawler`, `dlq`, `ruling-reprocess`, `user`). **`AuthService`** lives in `@legal-ai-ar/core`.
`NotificationService` is in `@legal-ai-ar/shared-common`.

---

## 8. Configuration (environments)

Environment files live in `projects/core/src/lib/environments/` and are swapped per build configuration:

| Angular configuration   | File                         | Typical `apiUrl`                    | `usePlatformCredentials` |
| ----------------------- | ---------------------------- | ----------------------------------- | ------------------------ |
| `local` (default serve) | `environment.ts`             | `''` (same-origin `/api` via proxy) | `false`                  |
| `dev`                   | `environment.dev.ts`         | Azure dev API host                  | `false`                  |
| `qa`                    | `environment.qa.ts`          | Azure QA API host                   | `false`                  |
| `development`           | `environment.development.ts` | GCaaS backend URL                   | `true`                   |
| `staging`               | `environment.staging.ts`     | Staging API host                    | `false`                  |
| `production`            | `environment.prod.ts`        | Same-origin on GCaaS                | `true`                   |

Shared keys on `LegalAiArEnvironment`:

| Key                                                                       | Purpose                                                 |
| ------------------------------------------------------------------------- | ------------------------------------------------------- |
| `apiUrl`                                                                  | API base (empty = same origin; paths append `/api/...`) |
| `usePlatformCredentials`                                                  | Send `id_token` cookie on API calls                     |
| `platformAuthFailurePath`                                                 | Route when unauthenticated (default `sesion-requerida`) |
| `platformLoginUrl`                                                        | Platform SSO re-entry URL                               |
| `platformSessionRefreshIntervalMs`                                        | GCaaS session refresh interval (default 45 min)         |
| `gcaasEngagementId` / `platformSessionRefreshPath` / `platformLogoutPath` | GCaaS session URLs                                      |

**Local dev:** `npm start` runs `ng serve` with `proxy.conf.json` forwarding `/api` to
`http://localhost:5088` (see [Developer Onboarding](../onboarding/README.md)).

---

## 9. Schematics (`schema-la`) — F0.0-W11

The **`schema-la`** collection in `frontend/schema-la/` generates thin/full feature slices, UI wrappers,
and API client stubs per [architecture §6.2](../standards/pwc-internal-app-architecture.md#62-adaptive-feature-layering).

| Generator      | Output                                                           |
| -------------- | ---------------------------------------------------------------- |
| `thin-feature` | `src/app/features/{name}/` with `pages/`, `components/`, `data/` |
| `full-feature` | `domain/`, `api/`, `application/`, `views/`                      |
| `ui-wrapper`   | `projects/ui/src/lib/{name}/` + `public-api.ts` export           |
| `api-service`  | `{feature}/api/` or `data/` HTTP stub                            |

Setup and usage: [`frontend/docs/schematics.md`](../../frontend/docs/schematics.md). Demo feature:
`health-check` at `/salud`.

---

## 10. Tooling

| Script                | Command                                                                     |
| --------------------- | --------------------------------------------------------------------------- |
| Serve (local + proxy) | `npm start`                                                                 |
| Build                 | `npm run build` / `build:dev` / `build:qa` / `build:staging` / `build:prod` |
| Link schematics       | `npm run link-schemas`                                                      |
| Unit tests            | `npm test` (Jest; `projects/core` placeholder specs)                        |
| Lint                  | `ng lint` (libraries clean; app-level a11y debt → **F0.0-W08**)              |
| E2E smoke             | `npm run e2e` (Playwright)                                                  |
| Format                | `npm run format` (Prettier)                                                 |

---

## 11. Related documentation

- [16 — Chat, RAG & Agent Pipeline](16-chat-rag-agents.md) — the backend the chat client streams from
- [08 — Legal AI UX](08-legal-ai-ux.md) — streaming, inline citation, feedback UX
- [AppKit 4 (UI library)](../appkit4/README.md) — the target design system
- [GCaaS Hosting](../deployment/gcaas-hosting.md) — platform auth / session model the SPA relies on
- [Developer Onboarding](../onboarding/README.md) — running the SPA locally

---

_Frontend Architecture — Legal Ai Ar_
