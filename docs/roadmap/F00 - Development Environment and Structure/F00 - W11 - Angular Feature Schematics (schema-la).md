# F00 - W11 - Angular Feature Schematics (schema-la)

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** frontend | **Priority:** Medium
> **Estimate:** 3 story points
> **Assignable to:** Frontend Dev

---

## Description

Create **`schema-la`** Angular schematics to generate feature scaffolding that matches
[PwC Internal Application Architecture §6.2 and §6.5](../../standards/pwc-internal-app-architecture.md#62-adaptive-feature-layering)
(thin slice vs full slice). After F00-W03 establishes the monorepo libraries (`shell`, `core`, `ui`,
`shared-common`), schematics enforce consistency for R1+ feature development and reduce copy-paste.

---

## Implementation Tasks

> Broken down on 2026-06-02. Estimate: 18 tasks, ~6–8h.

### 1. Schematic project setup

- [x] **Create**: `frontend/schema-la/package.json`, `collection.json`, `tsconfig.json`
- [x] **Modify**: `frontend/package.json` — `schema-la` file dep, `link-schemas` script
- [x] **Modify**: `frontend/angular.json` — `cli.schematicCollections` includes `schema-la`

### 2. Shared schematic utilities

- [x] **Create**: `frontend/schema-la/src/utils/names.ts`, `paths.ts`, `route-patch.ts`, `public-api-patch.ts`

### 3. Generator: `thin-feature`

- [x] **Create**: `frontend/schema-la/src/thin-feature/` — schema, index, templates (pages, data, routes)

### 4. Generator: `full-feature`

- [x] **Create**: `frontend/schema-la/src/full-feature/` — domain, api, application, views templates

### 5. Generator: `ui-wrapper`

- [x] **Create**: `frontend/schema-la/src/ui-wrapper/` — component template + `public-api.ts` patch

### 6. Generator: `api-service`

- [x] **Create**: `frontend/schema-la/src/api-service/` — `--domain`, `--feature`, `--slice`

### 7. Documentation

- [x] **Create**: `frontend/docs/schematics.md`
- [x] **Modify**: `docs/technical/18-frontend-architecture.md` — schematics section
- [x] **Modify**: `docs/standards/pwc-internal-app-architecture.md` §6.5 — `api-service` + link to schematics doc

### 8. Placeholder + validation

- [x] **Create**: `frontend/src/app/features/health-check/` (thin slice demo, route `/salud`)
- [x] **Modify**: `frontend/src/app/app.routes.ts` — lazy `healthCheckRoutes`

### 9. Verification

- [x] `npm run link-schemas` compiles collection
- [x] `ng build` passes with health-check feature
- [ ] Documentation round-trip (`STATUS.md` on merge)

---

## Usage (target)

```bash
cd frontend
npm run link-schemas

ng g schema-la:thin-feature statutes --route=ordenamiento
ng g schema-la:full-feature workspaces --route=espacios
ng g schema-la:ui-wrapper data-table
ng g schema-la:api-service rulings --domain=rulings --feature=search --slice=thin
```

---

## Acceptance Criteria

- [x] All four generators run without error on a clean workspace post F00-W03
- [x] Generated **thin** layout matches standard §6.2 thin slice folder structure
- [x] Generated **full** layout matches standard §6.2 full slice folder structure
- [x] Generated components use AppKit imports from `projects/ui` where applicable
- [x] `frontend/docs/schematics.md` documents every generator with options
- [x] Placeholder feature builds and lazy-loads
- [x] Architecture standard §16 frontend checklist — schematics row satisfied for new features
- [x] Documentation round-trip complete (DoD) — merged to `main` ([PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113))

---

## Dependencies

- **Blocks:** Consistent R1+ frontend feature scaffolding
- **Prerequisites:** **F00-W03** (Angular monorepo libraries must exist)

---

_F00 - W11 - Angular Feature Schematics (schema-la) — Legal Ai Ar_
