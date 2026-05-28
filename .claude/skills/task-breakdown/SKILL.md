---
name: task-breakdown
description: "Breaks down an assigned work item into concrete, technical implementation tasks. Replaces the generic work item tasks with a detailed checklist that includes files, paths, and specific steps. Use when the user says 'break down tasks', 'work item breakdown', 'create tasks for W0X', 'what do I need to do to implement this work item', or when a work item is assigned to a dev to implement."
---

# Task Breakdown — Legal Ai Ar

Breaks down a roadmap work item into concrete, technical, sequential implementation tasks that a developer can follow step by step.

## When to use

- When a work item is assigned to a developer to implement
- Before starting to code, to plan the work
- When the work item tasks are generic and need technical detail

## Before generating

1. Read the full work item (.md)
2. Read the W01 (Comprehensive Documentation) of the same feature for context
3. Read `docs/roadmap/features.md` to understand the feature's endpoints, DTOs, and KPIs
4. Read the relevant technical docs in `docs/technical/`
5. If it involves entities, read `docs/ontology/argentine-legal-ontology.md`

## What it generates

Replaces the work item's `## Tasks` section with a detailed technical breakdown. Each task is a concrete step that results in a created or modified file.

## Output format

The work item's `## Tasks` section is rewritten with this format:

```markdown
## Implementation Tasks

> Broken down on {date}. Estimate: {N} tasks, ~{X}h of development.

### 1. {Category}: {Short description}

- [ ] **{Action}**: `{Path/File.cs}` in `LegalAiAr.{Layer}`
  - {Detail of what to create/modify: classes, methods, interfaces}
  - {Dependencies or considerations}

### 2. {Category}: {Short description}

- [ ] **Create**: `{Path/File.cs}` in `LegalAiAr.{Layer}`
  - {Detail}
- [ ] **Create**: `{Path/OtherFile.cs}` in `LegalAiAr.{Layer}`
  - {Detail}

### N. Verification

- [ ] `dotnet build` compiles with no errors or warnings
- [ ] `dotnet test` passes all tests (new and existing)
- [ ] Endpoints respond correctly (test with .http or Swagger)
- [ ] Work item acceptance criteria verified
```

## Standard categories

Use these categories to group the tasks, in this order:

1. **Model**: entities, enums, value objects in Core
2. **Persistence**: DbContext, EF configuration, migrations in Infrastructure
3. **Services**: interfaces in Core, implementations in Infrastructure or Application
4. **CQRS**: Commands, Queries, Handlers in Application
5. **Validation**: FluentValidation validators in Application
6. **Endpoints**: Minimal API endpoints in Api
7. **Frontend**: components, services, models in Angular
8. **AI/Agents**: plugins, prompts, orchestration in Agents
9. **Tests**: unit, integration, E2E
10. **Verification**: build, test, acceptance criteria

Not all categories apply to every work item. Use only the relevant ones.

## Rules

- Each task must result in a concrete file with a full path relative to `mvp/`
- Indicate the .NET project (`LegalAiAr.{Layer}`) or the Angular folder
- Tasks must follow a logical dependency order (Core first, then Application, then Api)
- Respect Clean Architecture: dependencies point inward
- Include test tasks at the end (before Verification)
- The Verification section always goes last
- Be specific: "Create `ILegalNormSearchService.cs` with method `Task<SearchResult> SearchAsync(SearchQuery query, CancellationToken ct)`" is better than "Create service interface"
- Full paths: `backend/src/shared/LegalAiAr.Core/Interfaces/Services/ILegalNormSearchService.cs`
- Name classes, methods, and interfaces with real English names (not `{Feature}Service`)

## Example

For a "Legal norm search endpoint" work item (code names and work item content in English):

```markdown
## Implementation Tasks

> Broken down on 2026-05-28. Estimate: 12 tasks, ~6h of development.

### 1. Model: search DTOs

- [ ] **Create**: `backend/src/api/LegalAiAr.Application/DTOs/LegalNorms/LegalNormSearchRequestDto.cs`
  - Record with: Query (string), LawBranch (enum?), DateFrom/DateTo (DateTime?), Page (int), PageSize (int)
- [ ] **Create**: `backend/src/api/LegalAiAr.Application/DTOs/LegalNorms/LegalNormSearchResultDto.cs`
  - Record with: Items (List<LegalNormSummaryDto>), TotalCount, Page, PageSize
- [ ] **Create**: `backend/src/api/LegalAiAr.Application/DTOs/LegalNorms/LegalNormSummaryDto.cs`
  - Record with: Id, Title, NormType, Number, PublicationDate, ValidityStatus, Score

### 2. Services: hybrid search

- [ ] **Create**: `backend/src/shared/LegalAiAr.Core/Interfaces/Services/ILegalNormSearchService.cs`
  - Interface with method: Task<LegalNormSearchResult> SearchAsync(LegalNormSearchQuery query, CancellationToken ct)
- [ ] **Create**: `backend/src/shared/LegalAiAr.Infrastructure/Services/Search/LegalNormSearchService.cs`
  - Implementation using Azure AI Search client (hybrid: BM25 + vector)
  - Inject SearchIndexClient and ITextEmbeddingService

### 3. CQRS: query handler

- [ ] **Create**: `backend/src/api/LegalAiAr.Application/LegalNorms/Queries/SearchLegalNormsQuery.cs`
  - Record implementing IRequest<LegalNormSearchResultDto>
- [ ] **Create**: `backend/src/api/LegalAiAr.Application/LegalNorms/Queries/SearchLegalNormsQueryHandler.cs`
  - Handler using ILegalNormSearchService and mapping to DTOs

### 4. Validation

- [ ] **Create**: `backend/src/api/LegalAiAr.Application/LegalNorms/Validators/SearchLegalNormsQueryValidator.cs`
  - PageSize between 1-50, Query not empty, valid dates

### 5. Endpoints

- [ ] **Create/Modify**: `backend/src/api/LegalAiAr.Api/Endpoints/LegalNormEndpoints.cs`
  - GET /api/legal-norms/search with query params mapped to SearchLegalNormsQuery
  - .RequireAuthorization(), .WithOpenApi(), .WithTags("LegalNorms")

### 6. Tests

- [ ] **Create**: `backend/tests/LegalAiAr.Application.Tests/LegalNorms/SearchLegalNormsQueryHandlerTests.cs`
  - Tests: successful search, empty query returns error, correct pagination
- [ ] **Create**: `backend/tests/LegalAiAr.Api.Tests/Endpoints/LegalNormEndpointsTests.cs`
  - Endpoint integration tests

### 7. Verification

- [ ] `dotnet build` compiles with no errors or warnings
- [ ] `dotnet test` passes all tests
- [ ] GET /api/legal-norms/search?query=ley+de+sociedades returns 200 with results
- [ ] Work item acceptance criteria verified
```

## How to apply the result

The result replaces the existing `## Tasks` section in the work item .md. The Description, Acceptance Criteria, Dependencies, and the rest of the work item are not touched.
