# Angular schematics — `schema-la`

Legal Ai Ar uses the **`schema-la`** schematic collection to generate feature scaffolding aligned with
[PwC Internal Application Architecture §6.2](../../docs/standards/pwc-internal-app-architecture.md#62-adaptive-feature-layering)
(thin vs full slice) and workspace libraries from F00-W03.

## Prerequisites

From `frontend/`:

```bash
npm install
npm run link-schemas
```

`link-schemas` compiles `schema-la/` (TypeScript → `schema-la/dist/`). The collection is registered in
`angular.json` under `cli.schematicCollections`.

## Generators

| Schematic      | Command                                                                    | Purpose                                     |
| -------------- | -------------------------------------------------------------------------- | ------------------------------------------- |
| `thin-feature` | `ng g schema-la:thin-feature {name} --route={path}`                        | `pages/`, `components/`, `data/`, routes    |
| `full-feature` | `ng g schema-la:full-feature {name} --route={path}`                        | `domain/`, `api/`, `application/`, `views/` |
| `ui-wrapper`   | `ng g schema-la:ui-wrapper {name}`                                         | Wrapper in `projects/ui/src/lib/`           |
| `api-service`  | `ng g schema-la:api-service {name} --domain={segment} --feature={feature}` | HTTP stub in `api/` or `data/`              |

### Options

**thin-feature / full-feature**

| Option       | Required | Description                                             |
| ------------ | -------- | ------------------------------------------------------- |
| `name`       | yes      | Feature name in English (`health-check`, `workspaces`)  |
| `route`      | yes      | Spanish UI route segment (`salud`, `espacios`)          |
| `skipRoutes` | no       | Skip patching `src/app/app.routes.ts` (default `false`) |

**ui-wrapper**

| Option | Required | Description                              |
| ------ | -------- | ---------------------------------------- |
| `name` | yes      | Component name in English (`data-table`) |

**api-service**

| Option    | Required | Description                                          |
| --------- | -------- | ---------------------------------------------------- |
| `name`    | yes      | API client class base name (`rulings`)               |
| `domain`  | yes      | API path segment after `/api/` (`rulings`, `search`) |
| `feature` | yes      | Existing feature folder under `src/app/features/`    |
| `slice`   | no       | `thin` → `data/`; `full` → `api/` (default `full`)   |

## Examples

```bash
cd frontend
npm run link-schemas

ng g schema-la:thin-feature statutes --route=ordenamiento
ng g schema-la:full-feature workspaces --route=espacios
ng g schema-la:ui-wrapper data-table
ng g schema-la:api-service rulings --domain=rulings --feature=search --slice=thin
```

## Placeholder feature

**`health-check`** at route `/salud` demonstrates the thin slice (generated layout, hand-tuned Spanish
labels). It lazy-loads via `loadChildren` in `app.routes.ts`.

## Conventions

- **Code** (files, classes, routes in TypeScript): English.
- **UI labels** in generated page templates: Spanish placeholders; customize after generation.
- Generated components import **`@legal-ai-ar/ui`** (`UiSectionComponent` or new wrappers).
- Route registration inserts before `// --- Backward compatibility redirects` in `app.routes.ts`.

---

_schema-la — Legal Ai Ar_
