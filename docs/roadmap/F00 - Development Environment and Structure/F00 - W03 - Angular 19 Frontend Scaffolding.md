# F00 - W03 - Angular 19 Frontend Scaffolding

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** frontend | **Priority:** Critical (blocking)
> **Estimate:** 5 story points
> **Assignable to:** Frontend Dev

---

## Description

Initialize the Angular 19 project inside the monorepo with the defined folder structure, configure PwC AppKit 4 (or a placeholder until the library is available), and leave the project ready for feature development.

---

## Tasks

- [ ] Generate the Angular 19 project with `ng new` (standalone, SCSS, routing)
- [ ] Move the generated project into the `frontend/` folder
- [ ] Configure the folder structure: `core/`, `shared/`, `features/`, `layout/`
- [ ] Configure `angular.json` with per-environment build configurations (dev, qa, staging, prod)
- [ ] Create `environment.ts` files for each environment
- [ ] Install and configure PwC AppKit 4 (or a placeholder with Angular Material temporarily)
- [ ] Configure base SCSS: `_variables.scss`, `_mixins.scss`, `_typography.scss`
- [ ] Create the `AppComponent` with a router-outlet
- [ ] Create the `LayoutComponent` (shell with a sidebar placeholder and navbar)
- [ ] Configure `app.routes.ts` with lazy loading for placeholder features
- [ ] Configure the proxy for the backend API in `proxy.conf.json`
- [ ] Configure Jest as the test runner (replace Karma)
- [ ] Install and configure ESLint + angular-eslint
- [ ] Install and configure Prettier
- [ ] Add npm scripts: `start`, `build`, `test`, `lint`, `e2e`
- [ ] Verify that `ng build` compiles with no errors
- [ ] Verify that `ng test` runs placeholder tests
- [ ] Verify that `ng serve` starts the app on `localhost:4200`

---

## Proxy Configuration (local development)

```json
// proxy.conf.json
{
  "/api": {
    "target": "https://localhost:5001",
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

- [ ] `ng build --configuration=production` compiles with no errors or warnings
- [ ] `ng serve` starts the app and shows the shell layout with a sidebar
- [ ] `jest` runs at least 1 placeholder test successfully
- [ ] `ng lint` reports no errors
- [ ] Lazy-loaded routes work (at least 1 placeholder feature)
- [ ] The proxy correctly forwards to the backend API
- [ ] The 5 environment files are configured

---

## Dependencies

- **Blocks:** F00-W04 (frontend CI), F01-W04 (frontend platform session setup)
- **Depends on:** F00-W02 (repo already created on GitHub)

---

*F00 - W03 - Angular 19 Frontend Scaffolding — Legal Ai Ar*
