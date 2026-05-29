# Frontend Architecture

> The Angular single-page application: structure, routing/navigation, authentication, the streaming
> chat client, and the graph explorer.
>
> This document describes the SPA as currently implemented. Route paths and UI labels are in Spanish
> (end-user contact layer); everything else follows the English code convention.

---

## 1. Overview

The SPA is an **Angular standalone-components** app (no NgModules), with **lazy-loaded** feature
routes behind a shared shell, and **cookie-based** authentication (`withCredentials`). Notable
libraries in use today:

| Concern | Library |
|---------|---------|
| Framework | Angular (standalone components, signals) |
| Graph visualization | **Cytoscape** |
| Markdown rendering (chat answers) | **marked** |
| Base theming | Angular Material prebuilt theme + custom global styles (PwC design) |

> **Target design system.** The adopted UI design system is **PwC AppKit 4**
> ([`docs/appkit4/`](../appkit4/README.md)); the current styling predates full AppKit adoption (Angular
> Material + custom CSS). New UI should follow AppKit — see the designer skill and
> [08 — Legal AI UX](08-legal-ai-ux.md).

---

## 2. Project structure

```
src/app/
├── app.config.ts        # providers: router, HttpClient (+ interceptors), animations, APP_INITIALIZER, locale
├── app.routes.ts        # route table (shell + lazy features)
├── layout/
│   ├── shell/           # app chrome: sidebar navigation + content outlet
│   └── status-bar/
├── features/            # one folder per feature area (lazy-loaded)
│   ├── welcome/  search/  proceedings/  catalogs/  statutes/
│   ├── chat/  graph-explorer/  ontology/  kb-dashboard/
│   ├── auth/   admin/
├── services/            # singleton data/services (+ services/admin/)
├── guards/              # authGuard
├── interceptors/        # platform-credentials, error
├── shared/              # components, directives, pipes, adapters, services
└── models/
```

---

## 3. Routing & navigation

Navigation is organized around the lawyer's mental model — **ontological spaces** plus **tools** plus
**admin** — rendered by the shell sidebar (`ShellComponent`). All app routes sit under the shell and
are protected by `authGuard`.

| Route | View | Group |
|-------|------|-------|
| `/bienvenida` | Welcome | — |
| `/jurisprudencia` · `/jurisprudencia/resultados` · `/jurisprudencia/:id` | Ruling search → results → detail | Ontological space |
| `/organismos` · `/organismos/:id` | Courts list / detail | Ontological space |
| `/sujetos` · `/sujetos/:id` | Persons list / detail | Ontological space |
| `/vocabulario` · `/vocabulario/:id` | Thesaurus list / detail | Ontological space |
| `/ordenamiento` · `/ordenamiento/piramide` · `/ordenamiento/:id` | Statutes list / normative pyramid / detail | Ontological space |
| `/procesos` · `/procesos/:id` | Proceedings list / detail (`/causas` redirects here) | Ontological space |
| `/asistente` | AI chat assistant | Tool |
| `/explorador` | Graph explorer | Tool |
| `/estadisticas` | KB dashboard | Tool |
| `/ontologia` | Ontology viewer | Tool |
| `/admin/**` | Ingestion, DLQ, reprocess queue, users, jobs, entity-sources | Admin |

Most routes are lazy (`loadComponent`); a few high-traffic ones are eager. The admin area has its own
nested layout (`AdminLayoutComponent`).

---

## 4. Authentication

Cookie-session model aligned with the platform `id_token` (see
[GCaaS Hosting](../deployment/gcaas-hosting.md)):

- **`authGuard`** (`CanActivateFn`) checks `AuthService.isAuthenticated()`; on failure it redirects to
  the configured `platformAuthFailurePath` (default `sesion-requerida`).
- **`platformCredentialsInterceptor`** adds `withCredentials: true` to API calls when
  `environment.usePlatformCredentials` is set (corporate/GCaaS host), so the `id_token` cookie is sent.
- **`errorInterceptor`** centralizes HTTP error handling.
- `SessionRequiredComponent` (`/sesion-requerida`) handles the unauthenticated state; `login` redirects
  to `/bienvenida`. There is **no MSAL / app-owned login** — authentication is platform-managed.

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

Singleton services under `services/` wrap the API per domain: `auth`, `chat`, `ruling`, `court`,
`person`, `statute`, `thesaurus`, `proceeding`, `ontology`, `graph-explorer`, `stats`,
`global-search`, `command-palette`, `recent-searches`, `onboarding`, plus `services/admin/`
(`crawler`, `dlq`, `ruling-reprocess`, `user`). A shared `notification.service` handles toasts.

---

## 8. Configuration (environments)

Per-environment files (`environment.ts` / `.development` / `.staging` / `.prod`) expose a typed model:

| Key | Dev value | Purpose |
|-----|-----------|---------|
| `apiUrl` | `http://localhost:5088` | API base URL |
| `usePlatformCredentials` | `false` (dev) | Send `id_token` cookie (`withCredentials`) on a corporate host |
| `platformLoginUrl` | `''` | Where to send the user to re-authenticate |
| `platformAuthFailurePath` | `sesion-requerida` | Local route on auth failure |
| `production` | per env | Angular production flag |

---

## 9. Related documentation

- [16 — Chat, RAG & Agent Pipeline](16-chat-rag-agents.md) — the backend the chat client streams from
- [08 — Legal AI UX](08-legal-ai-ux.md) — streaming, inline citation, feedback UX
- [AppKit 4 (UI library)](../appkit4/README.md) — the target design system
- [GCaaS Hosting](../deployment/gcaas-hosting.md) — platform auth / session model the SPA relies on
- [Developer Onboarding](../onboarding/README.md) — running the SPA locally

---

*Frontend Architecture — Legal Ai Ar*
