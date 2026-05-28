# Legal Ai Ar — Instructions for Claude

## Project identity

Legal Ai Ar is a Legal Knowledge Base system with AI Agents for the Argentine legal domain. It centralizes legislation, case law, and legal doctrine, combining hybrid search (BM25 + vectors) with specialized agents built on Semantic Kernel.

## Tech stack

- **Frontend**: Angular 19 (standalone components), Tailwind CSS 4, Cytoscape.js
- **Backend**: .NET 10, Clean Architecture (4 layers), Minimal API, EF Core 10
- **AI/RAG**: Azure OpenAI (GPT-5o, GPT-5o-mini), Semantic Kernel, text-embedding-3-large (3072d)
- **Data**: Azure SQL (relational + Graph Tables), Azure AI Search (hybrid search), Azure Blob Storage
- **Messaging**: Azure Storage Queues, Azure SignalR Service
- **Hosting**: Azure App Service
- **CI/CD**: GitHub Actions

## Monorepo structure

```
legal-ai-ar/
├── mvp/                          # Existing MVP code
│   ├── backend/
│   │   ├── src/
│   │   │   ├── api/
│   │   │   │   ├── LegalAiAr.Api/           # ASP.NET Core 10 (Minimal API)
│   │   │   │   └── LegalAiAr.Application/   # CQRS, handlers, services
│   │   │   ├── shared/
│   │   │   │   ├── LegalAiAr.Core/          # Entities, enums, interfaces
│   │   │   │   └── LegalAiAr.Infrastructure/ # EF Core, Azure services, AI
│   │   │   ├── workers/                      # 6 BackgroundService workers
│   │   │   └── tools/                        # 10 auxiliary CLI tools
│   │   ├── tests/                            # 8 test projects (xUnit + NSubstitute)
│   │   ├── LegalAiAr.sln
│   │   ├── Directory.Packages.props          # Central Package Management
│   │   └── global.json                       # .NET 10
│   └── frontend/                             # Angular 19 SPA
├── docs/
│   ├── roadmap/                              # Features, work items, backlog
│   │   ├── features.md                       # Full roadmap (v2.0)
│   │   ├── backlog.md
│   │   ├── gap-analysis-mvp-vs-plan.md
│   │   └── F00..F23, FT01..FT04/            # One folder per feature
│   ├── technical/                            # 9 technical documents
│   └── ontology/                             # Legal domain model
└── README.md
```

## Code conventions

### Backend (.NET)

- Clean Architecture: Core → Application → Infrastructure → Api
- Core does NOT reference any other project
- Project names: `LegalAiAr.{Layer}` (not LegalKB)
- CQRS pattern with MediatR in Application
- Minimal API (no Controllers) in Api
- Central Package Management via `Directory.Packages.props`
- Tests with xUnit + NSubstitute + FluentAssertions

### Frontend (Angular)

- Standalone components (no NgModules)
- Features lazy-loaded by route
- Singleton services in `core/`
- Shared components in `shared/`
- Signals for reactive state

### Azure naming

- Resources: `{service}-legal-ai-ar-{environment}` (e.g., `sql-legal-ai-ar-dev`)
- Database: `LegalAiAr`
- Storage containers: kebab-case

## Project releases

| Release | Name | Status |
|---------|------|--------|
| R0.0 | Preparation | In progress |
| R1.0 | Foundation | Pending |
| R2.0 | Agents | Pending |
| R3.0 | Risk | Pending |
| R4.0 | Operations | Pending |

## Reference documentation

Before generating code or work items, consult:

- `docs/roadmap/features.md` — Full roadmap with all features, endpoints, and KPIs
- `docs/roadmap/gap-analysis-mvp-vs-plan.md` — What exists in the MVP vs. what is missing
- `docs/technical/` — Technical decisions on RAG, agents, prompts, ingestion, evaluation, security, observability, UX, and data management
- `docs/ontology/` — Argentine legal domain model (classes, properties, relationships, sources)

## Important rules

- **Do not touch code directly**: NEVER create, edit, or delete source code files unless the user explicitly requests it. Instead, indicate which file to create or modify, in which path, and provide the code for the user to place. Correct example: "Create the `LegalNormService.cs` class in `mvp/backend/src/api/LegalAiAr.Application/Services/` with the following code: ..." Incorrect example: creating the file directly with Write/Edit.
- **Language rule (strict)**:
  - **Everything in English**: code (variables, methods, classes, interfaces, enums), file names, components, tables, indexes, storage, repos, URLs, endpoints, DTOs, domain entities, commits, technical documentation, work items, code comments.
  - **Spanish only for**: end-user facing content in the app (UI labels, error messages, tooltips, user-visible content) and the entire presentation layer facing the end user.
  - **Two exceptions that stay in Spanish**: the content of the agents' YAML prompts, and the labels in the mockups (both are the end-user contact layer).
  - **Dev chat**: can be in Spanish or English interchangeably.
  - Example: the class is `LegalNorm`, the table is `LegalNorms`, the endpoint is `/api/legal-norms`, but the UI label reads "Normas Legales".
- **Monorepo**: the project uses a single repository `legal-ai-ar`; never create separate repos.
- **Work items**: each work item follows the project's standard template (see `docs/roadmap/F00*/F00 - W01*` as an example). Work item content must be in English.
- **Consistency**: when referring to the project use "Legal Ai Ar" (display name) or "LegalAiAr" (.NET namespace); never "LegalKB".
- **New projects to add**: `LegalAiAr.Agents` (Semantic Kernel) in `src/shared/` and `LegalAiAr.AgentEvals` in `tests/`.
- **Infrastructure**: CI/CD, IaC (Bicep), Azure Key Vault, Docker are managed outside the feature roadmap.
