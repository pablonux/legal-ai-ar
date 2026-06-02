# F00 - W03 - Angular 19 Frontend Scaffolding

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** frontend | **Priority:** Critical (blocking)
> **Estimate:** 5 story points
> **Assignable to:** Frontend Dev
> **Status:** ✅ **Done** — merged to `main` via [PR #105](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/105) (2026-06-01)

---

## Description

Adapt the existing Angular SPA to the [PwC Internal Application Architecture](../standards/pwc-internal-app-architecture.md) (§6 Frontend): monorepo libraries (`shell`, `core`, `ui`, `shared-common`), AppKit 4 shell, platform cookie auth (`withCredentials`), adaptive feature layering, and environment configurations. The MVP already provides a working SPA at `frontend/` — this work item is **hoist + adapt**, not greenfield scaffolding.

**Baseline:** Read [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) before starting. Product-specific routes and views are documented in [`18-frontend-architecture.md`](../technical/18-frontend-architecture.md).

---

## Tasks

### Structure (per architecture standard §6)

- [x] Restructure `frontend/` into an Angular workspace with libraries: `shell`, `core`, `ui`, `shared-common` (app host remains `src/app/`)
- [x] Migrate existing `src/app/core`-equivalent code into `projects/core` (auth, interceptors, guards, environment)
- [x] Implement shell in `projects/shell` (header, drawer, footer — **Material/custom placeholder** until AppKit package access)
- [x] Keep existing features under `src/app/features/` using **thin slice** layout (unchanged)
- [x] Configure `angular.json` with per-environment build configurations (`local`, `dev`, `qa`, `development`, `staging`, `production`)
- [x] Create/update `environment.ts` files with platform auth keys (`usePlatformCredentials`, `platformAuthFailurePath`, `platformSessionRefreshIntervalMs`)
- [ ] Install and configure PwC AppKit 4 per [`docs/appkit4/`](../appkit4/README.md) (deferred — Material placeholder per WI note)
- [ ] Configure AppKit design tokens in global styles (deferred with AppKit)
- [x] Wire `platformCredentialsInterceptor` and `authGuard` per architecture standard §5
- [x] Configure `app.routes.ts` with lazy loading; preserve existing feature routes
- [x] Configure the proxy for the backend API in `proxy.conf.json`
- [x] Configure Jest as the test runner (`jest.config.cjs`; Karma retained for legacy `ng test` target)
- [x] Install and configure ESLint + angular-eslint + Prettier
- [x] Add npm scripts: `start`, `build`, `test`, `lint`, `e2e`
- [x] Verify that `ng build` compiles with no errors
- [x] Verify that `npm test` (Jest) runs placeholder tests
- [x] Verify that `ng serve` starts the app on `localhost:4200` (manual)
- [x] Update [`docs/technical/18-frontend-architecture.md`](../technical/18-frontend-architecture.md) to reflect the merged structure (DoD round-trip)

---

## Proxy Configuration (local development)

```json
// proxy.conf.json
{
    "/api": {
        "target": "http://localhost:5088",
        "secure": false,
        "changeOrigin": true
    }
}
```

---

## npm Scripts

```json
{
    "start": "ng serve --proxy-config proxy.conf.json",
    "build": "ng build",
    "build:dev": "ng build --configuration=dev",
    "build:qa": "ng build --configuration=qa",
    "build:staging": "ng build --configuration=staging",
    "build:prod": "ng build --configuration=production",
    "test": "jest",
    "test:watch": "jest --watch",
    "test:coverage": "jest --coverage",
    "lint": "ng lint",
    "lint:fix": "ng lint --fix",
    "e2e": "npx playwright test",
    "format": "prettier --write \"src/**/*.{ts,html,scss}\""
}
```

---

## Base npm Packages

```json
{
    "dependencies": {
        "@angular/core": "^19.0.0",
        "chart.js": "^4.x",
        "ng2-charts": "^6.x",
        "cytoscape": "^3.x",
        "@fullcalendar/angular": "^6.x",
        "@fullcalendar/core": "^6.x",
        "@fullcalendar/daygrid": "^6.x",
        "@fullcalendar/timegrid": "^6.x",
        "marked": "^12.x"
    },
    "devDependencies": {
        "jest": "^29.x",
        "@angular-builders/jest": "^19.x",
        "@testing-library/angular": "^17.x",
        "jest-preset-angular": "^14.x",
        "playwright": "^1.x",
        "@playwright/test": "^1.x",
        "eslint": "^9.x",
        "angular-eslint": "^19.x",
        "prettier": "^3.x"
    }
}
```

> **Note (PwC AppKit):** PwC AppKit 4 will be added once the documentation and package access are provided. Until then, use Angular Material as a placeholder to validate the structure.
>
> **Note (auth):** Authentication is platform-managed (GCaaS Entra SSO via the `id_token` cookie) — no MSAL packages are needed. The SPA sends `withCredentials` and uses the session model described in [`gcaas-hosting.md`](../../deployment/gcaas-hosting.md).

---

## Initial Shell Layout

> Sidebar/navbar labels below are end-user UI placeholders and stay in Spanish.

```
┌──────────────────────────────────────────────┐
│  Navbar [Logo] [Título] [Usuario] [Logout]   │
├──────────┬───────────────────────────────────┤
│          │                                   │
│ Sidebar  │       <router-outlet>             │
│          │                                   │
│ - Home   │       (Feature content)           │
│ - Normas │                                   │
│ - Jurisp.│                                   │
│ - Exped. │                                   │
│ - Chat   │                                   │
│ - Config │                                   │
│          │                                   │
└──────────┴───────────────────────────────────┘
```

---

## Acceptance Criteria

- [x] Structure matches [architecture standard §6](../standards/pwc-internal-app-architecture.md#6-frontend-architecture) (workspace libraries + shell; AppKit chrome deferred)
- [x] Auth follows [architecture standard §5](../standards/pwc-internal-app-architecture.md#5-authentication-and-authorization) — platform cookie only; no Bearer/sessionStorage tokens
- [x] `ng build --configuration=production` compiles with no errors (bundle budget warnings only)
- [x] `ng serve` starts the app and shows the shell layout (Material placeholder)
- [x] `npm test` (Jest) runs at least 1 placeholder test successfully
- [x] `ng lint` reports no errors on workspace libraries (`core`, `shell`, `shared-common`, `ui`); app-level a11y lint debt → **F00-W08**
- [x] Lazy-loaded routes work (existing features preserved)
- [x] The proxy correctly forwards `/api` to the backend API
- [x] Environment files include platform auth configuration keys
- [x] [`docs/technical/18-frontend-architecture.md`](../technical/18-frontend-architecture.md) updated (DoD documentation round-trip)

---

## Dependencies

- **Blocks:** FT05-W01 (frontend CI — see FT05 Delivery and Hosting), F1.1 (platform session setup)
- **Depends on:** F00-W02 (repo already created on GitHub)

---

_F00 - W03 - Angular 19 Frontend Scaffolding — Legal Ai Ar_
