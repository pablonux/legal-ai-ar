# F00 - W08 - Code Quality Configuration (Linting, Formatting, EditorConfig)

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** devops | **Priority:** Medium
> **Estimate:** 3 story points
> **Assignable to:** Frontend Dev (Angular linting) + Backend Dev (.NET analyzers)

---

## Description

Configure all code-quality tools to ensure consistency between developers: EditorConfig, ESLint, Prettier, Roslyn Analyzers, **NetArchTest architecture tests**, husky pre-commit hooks, and PR/Issue templates. Enforce the quality gates defined in [PwC Internal Application Architecture §9](../standards/pwc-internal-app-architecture.md#9-testing-and-quality-gates).

---

## Status — done (adapted)

Many items already existed from MVP integration (`.editorconfig`, ESLint flat config, Prettier, PR/issue templates). W08 added analyzers, architecture tests, husky/commitlint, CI gates, and relaxed legacy MVP lint rules under `src/app/features/**`.

| Area                                                | State                                                               |
| --------------------------------------------------- | ------------------------------------------------------------------- |
| `.editorconfig` (root)                              | ✅ Already present                                                  |
| PR / issue templates                                | ✅ Already present                                                  |
| `TreatWarningsAsErrors` + NetAnalyzers + Roslynator | ✅ `Directory.Build.props`                                          |
| `.globalconfig`                                     | ✅ Added (xUnit1051, doc-comment debt suppressed)                   |
| `LegalAiAr.ArchitectureTests` (NetArchTest)         | ✅ Added — 6 rules                                                  |
| Husky + lint-staged + commitlint                    | ✅ Root `package.json` + `.husky/`                                  |
| `CODEOWNERS`                                        | ✅ Added (placeholder org handle)                                   |
| Full app ESLint strict                              | ⚠️ Legacy MVP features use relaxed overrides; workspace libs strict |
| CI format verify                                    | ✅ `dotnet format --verify-no-changes` in `ci-backend.yml`          |
| CI full frontend lint                               | ✅ `npm run lint` in `ci-frontend.yml`                              |

---

## Tasks

### Shared

- [x] Root `.editorconfig` — already present
- [x] Husky + lint-staged + commitlint — root `package.json`, `.husky/pre-commit`, `.husky/commit-msg`
- [x] `.github/PULL_REQUEST_TEMPLATE.md` — already present
- [x] `.github/ISSUE_TEMPLATE/bug_report.md` — already present
- [x] `.github/ISSUE_TEMPLATE/work_item.md` — already present
- [x] `.github/CODEOWNERS` — added (update handles when team is defined)

### Backend (.NET)

- [x] `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `Directory.Build.props`
- [x] `<Nullable>enable</Nullable>` — already present
- [x] `<ImplicitUsings>enable</ImplicitUsings>` — already present
- [x] `Microsoft.CodeAnalysis.NetAnalyzers` + `Roslynator.Analyzers`
- [x] `.globalconfig` at repo root
- [x] `LegalAiAr.ArchitectureTests` with NetArchTest.Rules (§9.2 layer rules)
- [x] Architecture tests run via `dotnet test LegalAiAr.sln` in CI

### Frontend (Angular)

- [x] ESLint (`angular-eslint` flat config) — extended rules + legacy MVP overrides
- [x] Prettier — `.prettierrc` / `.prettierignore` already present
- [x] `lint-staged` on pre-commit (frontend + markdown)
- [x] `strict: true` in `tsconfig.json` — already present
- [ ] `eslint-plugin-import` ordering — deferred (flat config + Angular CLI; low ROI for W08)

### IDE

- [x] `.vscode/settings.json` — format on save, ESLint fix, Prettier for TS/HTML

---

## Acceptance Criteria

- [x] `.editorconfig` applies rules in VS Code / Visual Studio
- [x] ESLint detects style errors in TypeScript and templates
- [x] Prettier formats on save (VS Code settings)
- [x] Pre-commit hook runs lint-staged (after `npm install` at repo root)
- [x] Commits validated by commitlint (Conventional Commits)
- [x] PR template shown on GitHub (already configured)
- [x] CODEOWNERS file present (update handles before enforcing)
- [x] Backend builds with `TreatWarningsAsErrors` and 0 warnings
- [x] NetArchTest architecture tests pass in CI

---

## Dependencies

- **Depends on:** F00-W02, F00-W03
- **Blocks:** None

---

_F00 - W08 - Code Quality Configuration — Legal Ai Ar_
