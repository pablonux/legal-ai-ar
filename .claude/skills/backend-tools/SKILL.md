---
name: backend-tools
description: "Standard backend development tools for Legal Ai Ar: .http files to test endpoints, EF Core migration commands, CQRS scaffolding (Command/Query + Handler + Validator), and test data/golden set conventions. Use when asked to create a .http file, generate a migration, scaffold a CQRS, create test fixtures, or any backend tooling task. Also applies with 'test endpoint', 'migration', 'scaffold', 'test data', 'fixture', 'seed', 'golden set'."
---

# Backend Tools — Legal Ai Ar

Standard tools and templates for .NET 10 backend development.

---

## 1. .http files — testing endpoints

REST Client files for VS Code/Cursor that allow testing endpoints without Postman.

### Location

```
backend/src/api/LegalAiAr.Api/http/
├── legal-norms.http
├── case-law.http
├── chat.http
├── case-files.http
├── admin.http
└── _variables.http       # Shared variables
```

### Variables template

```http
### _variables.http — Shared variables

@baseUrl = https://localhost:7001/api
@token = {{$dotenv TOKEN}}
@contentType = application/json
```

### Endpoint template

```http
### {Endpoint description}
# Feature: F{XX} | Work Item: W{YY}

{METHOD} {{baseUrl}}/{route}
Authorization: Bearer {{token}}
Content-Type: {{contentType}}

{JSON body if applicable}

###
```

### Full example

```http
### Search legal norms by text
# Feature: F03 | Work Item: W03

GET {{baseUrl}}/legal-norms/search?query=ley+de+sociedades&page=1&pageSize=10
Authorization: Bearer {{token}}

###

### Create chat conversation
# Feature: F08 | Work Item: W03

POST {{baseUrl}}/chat/conversations
Authorization: Bearer {{token}}
Content-Type: {{contentType}}

{
  "agentId": "normativo",
  "message": "¿Cuál es la ley vigente de sociedades comerciales?"
}

###
```

> Note: the `message` value above is example end-user input, so it stays in Spanish. Field names and routes are in English.

### Conventions

- One file per feature or group of related endpoints
- `###` separator between requests
- Comment with the reference Feature and Work Item
- Use `_variables.http` variables for baseUrl and token
- Include requests for: happy path, error 400, error 404, no auth (401)

---

## 2. EF Core migrations

### Standard commands

```bash
# Create migration (from the backend root)
cd backend
dotnet ef migrations add {MigrationName} \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api

# Apply migrations
dotnet ef database update \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api

# Revert the last migration
dotnet ef migrations remove \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api

# Migration SQL script (for review or manual deploy)
dotnet ef migrations script {PreviousMigration} {NewMigration} \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api \
  --output migrations/{MigrationName}.sql
```

### Migration naming

Format: `{YYYYMMDD}_{Description}` in PascalCase.

```
20260601_AddCaseFilesTable
20260601_AddLegalNormSearchIndex
20260602_AlterRulingAddSummaryField
20260605_SeedNormTypes
```

### Conventions

- One migration per logical change (don't mix create table + seed data)
- Entity configuration in `Infrastructure/Persistence/Configurations/{Entity}Configuration.cs` with `IEntityTypeConfiguration<T>`
- Never modify already-applied migrations — create a new one
- Include reference data seeds (enums, catalogs) in dedicated migrations
- Verify that `dotnet ef migrations list` shows the new migration

---

## 3. CQRS scaffolding

Template to create the Command/Query + Handler + Validator + Endpoint set that follows the project's standard pattern.

### For a Query (read)

**1. Query** — `Application/{Feature}/Queries/{Name}Query.cs`
```csharp
namespace LegalAiAr.Application.{Feature}.Queries;

public record {Name}Query(
    {Parameters}
) : IRequest<{ResultDto}>;
```

**2. Handler** — `Application/{Feature}/Queries/{Name}QueryHandler.cs`
```csharp
namespace LegalAiAr.Application.{Feature}.Queries;

public class {Name}QueryHandler(
    {IDependency} dependency
) : IRequestHandler<{Name}Query, {ResultDto}>
{
    public async Task<{ResultDto}> Handle(
        {Name}Query request,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**3. Validator** — `Application/{Feature}/Validators/{Name}QueryValidator.cs`
```csharp
namespace LegalAiAr.Application.{Feature}.Validators;

public class {Name}QueryValidator : AbstractValidator<{Name}Query>
{
    public {Name}QueryValidator()
    {
        // Validation rules
    }
}
```

**4. Endpoint** — add to `Api/Endpoints/{Feature}Endpoints.cs`
```csharp
group.MapGet("/{route}", async (
    [AsParameters] {Name}Query query,
    ISender sender,
    CancellationToken ct) =>
{
    var result = await sender.Send(query, ct);
    return Results.Ok(result);
})
.WithName("{Name}")
.WithOpenApi()
.RequireAuthorization();
```

### For a Command (write)

Same pattern but with `IRequest<{ResultDto}>` or `IRequest` (no return), and `MapPost`/`MapPut`/`MapDelete` in the endpoint.

### Associated test files

For each Handler, create:
- `tests/LegalAiAr.Application.Tests/{Feature}/{Name}QueryHandlerTests.cs`
- Test naming: `{Method}_{Scenario}_{ExpectedResult}`

---

## 4. Test Data and Golden Sets

### Locations

```
backend/tests/
├── LegalAiAr.Application.Tests/
│   └── TestData/                    # Fixtures by feature
│       ├── LegalNorms/
│       │   ├── legal-norm-valid.json
│       │   └── decree-norm-in-force.json
│       └── Rulings/
│           └── ruling-csjn-example.json
└── LegalAiAr.AgentEvals/
    └── GoldenSets/                  # For AI agent evaluation
        ├── normativo/
        │   ├── golden-set.json
        │   └── expected-responses.json
        ├── jurisprudencial/
        └── procesal/
```

### JSON fixture template

```json
{
  "$schema": "Fixture for {Feature} tests",
  "description": "{Scenario it represents}",
  "entity": "{EntityName}",
  "data": {
    "id": "00000000-0000-0000-0000-000000000001",
    "title": "{Representative value}",
    "createdAt": "2026-01-15T00:00:00Z"
  }
}
```

### Golden set template (AI evaluation)

```json
{
  "$schema": "Golden set for {name} agent",
  "version": "1.0",
  "agent": "{normativo|jurisprudencial|procesal}",
  "cases": [
    {
      "id": "GS-001",
      "question": "{User question}",
      "context": "{Additional context if applicable}",
      "expectedAnswer": "{Summary of what it should answer}",
      "expectedTools": ["{tool1}", "{tool2}"],
      "expectedCitations": ["{norm or ruling it should cite}"],
      "metrics": {
        "minRelevance": 0.8,
        "minCitationPrecision": 0.9,
        "mustCiteSources": true
      }
    }
  ]
}
```

> The `agent` value uses the project-wide agent identifiers (`normativo`, `jurisprudencial`, `procesal`), and `question` values may be example end-user input in Spanish. All JSON keys are in English.

### Conventions

- Fixtures in JSON, one file per scenario
- Descriptive kebab-case names: `legal-norm-valid.json`, not `test1.json`
- Fixture IDs are fixed GUIDs (reproducible)
- Versioned golden sets — increment `version` when modifying
- Realistic data (Argentine norms, rulings, case files), not lorem ipsum
- Do not include real client data or confidential information
