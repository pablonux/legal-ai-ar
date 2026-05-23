# Repository Structure and Conventions

| Field | Value |
|---|---|
| **ID** | E001 |
| **Feature** | F0-1 · Repository and project structure |
| **Date** | 2026-03-09 |

---

## Purpose

This document defines design decisions for the Legal AI AR monorepo structure, naming conventions, and branch and PR policy. It serves as a reference for tasks T-01 through T-06 of feature F0-1 and for maintaining consistency throughout the project lifecycle. It is consumed by developers, code reviewers, and the CI pipeline.

---

## 1. Monorepo Structure

### 1.1 Repository Root

```
legal-ai-ar/                    ← repository root
├── backend/                   ← .NET solution
├── frontend/                  ← Angular project
├── infra/                     ← Deploy scripts and configuration (optional; existing Azure services)
├── docs/                      ← documentation
│   ├── architecture/          ← architecture and specs
│   ├── roadmap/               ← development roadmap
│   ├── prompts/                ← agent prompts (Cursor skills in .cursor/skills/)
│   └── design/                ← roadmap design deliverables
├── README.md
├── .gitignore
├── .editorconfig
└── docker-compose.yml         ← SQL Server + Neo4j local
```

**Decision**: Monorepo with two application roots (`backend/`, `frontend/`) and a shared infrastructure folder.

**Justification**: A single repository simplifies versioning, CI/CD, and traceability between API and frontend changes. The `infra/` folder (optional) may contain deploy scripts and configuration; Azure services are used as existing, without IaC (Bicep/Terraform).

### 1.2 Base Documents

| File | Role |
|---|---|
| `README.md` | Overview and onboarding (root) |
| `docs/architecture/legal-ai-ar-architecture.md` | Technical architecture, data model, ADRs, pipeline, API |
| `docs/architecture/legal-ai-ar-specs.md` | Development specifications: Clean Architecture, CQRS, file structure |
| `docs/roadmap/ROADMAP.md` | Features, tasks and deliverables with DEV/AUD checks |

---

## 2. Naming Conventions

### 2.1 .NET Projects

**Pattern**: `LegalAiAr.{Component}.{Layer}`

| Project | Purpose |
|---|---|
| `LegalAiAr.Core` | Entities, enums, interfaces, message contracts |
| `LegalAiAr.Infrastructure` | Provider implementations (EF Core, Azure Search, Blob, Neo4j, Service Bus, Azure OpenAI) |
| `LegalAiAr.Application` | CQRS handlers, DTOs, validations |
| `LegalAiAr.Api` | ASP.NET Core Web API |
| `LegalAiAr.Worker.Crawler` | Download worker from sources |
| `LegalAiAr.Worker.Parser` | Extraction and normalization worker |
| `LegalAiAr.Worker.Enrichment` | Enrichment worker with GPT-4o |
| `LegalAiAr.Worker.Indexer` | Knowledge Base indexing worker |

**Justification**: The `LegalAiAr` prefix avoids collisions with NuGet packages. The `{Component}.{Layer}` structure reflects Clean Architecture separation of concerns.

### 2.2 Code and Identifiers

| Scope | Convention |
|---|---|
| Code (C#, TypeScript) | English: classes, methods, variables, schemas |
| User interface | Spanish: labels, messages. Visible routes in English (`/search`, `/rulings/:id`) |
| Configuration files | English (camelCase in JSON, PascalCase in C#) |

*Source: architecture section 1, language policy.*

### 2.3 Folders and Files

| Type | Convention |
|---|---|
| .NET folders | PascalCase (e.g., `Entities/`, `Repositories/`) |
| .NET files | PascalCase (e.g., `RulingRepository.cs`) |
| Angular folders | kebab-case (e.g., `search-home/`, `ruling-detail/`) |
| Angular files | kebab-case with suffix (e.g., `search-home.component.ts`) |
| Design documents | kebab-case with feature prefix (e.g., `f0-1-repo-structure.md`) |

---

## 3. Branch Policy

### 3.1 Model: Trunk-based

**Main branch**: `main`

- It is the only integration branch.
- Must remain stable and deployable at all times.
- The CI pipeline runs on every push to `main` and on every PR to `main`.

### 3.2 Feature Branches

**Pattern**: `feature/{feature-id}-{short-description}`

Examples:
- `feature/F0-1-repo-structure`
- `feature/F1-2-crawler-csjn`
- `fix/parser-null-reference`

**Rules**:
- Short-lived branches (days, not weeks).
- One branch per feature or fix.
- Create from `main` and merge back to `main`.

### 3.3 Prohibited Branches

- `develop` is not used as an integration branch.
- Long release branches are not maintained (deploy is direct from `main` via CD).

**Justification**: Trunk-based reduces merge complexity and accelerates feedback. Suitable for small teams and automated pipelines (ADR-010, Container Apps).

---

## 4. Pull Request Policy

### 4.1 Prerequisites

- Green CI pipeline (build + tests).
- No conflicts with `main`.
- Description indicating the feature/task and main changes.

### 4.2 Review

- **Minimum 1 approver** before merge. When there is only one active contributor, this requirement is waived; the merge may be performed after green CI.
- The author cannot approve their own PR (except when they are the only contributor).
- Review comments must be resolved before merge.

### 4.3 Merge Strategy

- **Squash merge** to `main`.
- The resulting commit message must follow the format: `{feature-id}: {description}` (e.g., `F0-1: monorepo structure and conventions`).

**Justification**: Squash merge keeps a linear history on `main` and facilitates rollback by commit. A single approver is sufficient for a small internal team.

### 4.4 Recommended Size

- Focused PRs (< 400 lines of diff when possible).
- If a feature generates a very large PR, split into incremental PRs that keep the system stable.

---

## 5. Summary of Decisions

| # | Decision | Justification |
|---|---|---|
| 1 | Monorepo with backend, frontend, infra and docs | Unified versioning, simplified CI/CD |
| 2 | LegalAiAr prefix in .NET projects | Avoid collisions, ownership clarity |
| 3 | Code in English, UI in Spanish | Technical consistency; UX in user language |
| 4 | Trunk-based with `main` as only integration branch | Lower complexity, fast feedback |
| 5 | Feature branches with pattern `feature/{id}-{desc}` | Traceability with legal-ai-ar-roadmap |
| 6 | 1 approver + squash merge | Balance between quality and speed for internal team. Exception: single contributor may merge after green CI. |

---

## References

- `docs/architecture/legal-ai-ar-architecture.md` — section 1 (.NET projects), ADR-001 (Azure stack)
- `docs/architecture/legal-ai-ar-specs.md` — section 2 (repository structure)
- `docs/roadmap/ROADMAP.md` — F0-1, tasks T-01 through T-07
