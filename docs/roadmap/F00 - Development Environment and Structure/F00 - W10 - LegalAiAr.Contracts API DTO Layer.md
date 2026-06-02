# F00 - W10 - LegalAiAr.Contracts API DTO Layer

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Backend Dev

---

## Description

Introduce **`LegalAiAr.Contracts`** as a dedicated project for public HTTP request/response types, per
[PwC Internal Application Architecture §4.1 and §3](../../standards/pwc-internal-app-architecture.md#31-layer-responsibilities).
Today DTOs live scattered in `LegalAiAr.Application` feature folders. Extracting them stabilizes the API
contract, simplifies OpenAPI generation, and keeps Application focused on use cases.

This WI migrates DTOs for **waves 1–2** endpoints (auth + read catalogs) migrated in F00-W09; remaining
features migrate incrementally as their controllers become endpoints.

---

## Implementation Tasks

> Broken down on 2026-06-02. Estimate: ~28 tasks, ~8–12h of development.

### 1. Project: LegalAiAr.Contracts shell

- [x] **Create**: `backend/src/shared/LegalAiAr.Contracts/LegalAiAr.Contracts.csproj` — `Microsoft.NET.Sdk`, no `ProjectReference` to LegalAiAr.\*
- [x] **Modify**: `backend/LegalAiAr.sln` — add project under `shared`
- [x] **Modify**: `backend/src/api/LegalAiAr.Application/LegalAiAr.Application.csproj` — reference Contracts
- [x] **Modify**: `backend/src/api/LegalAiAr.Api/LegalAiAr.Api.csproj` — reference Contracts
- [x] **Create**: `backend/tests/LegalAiAr.ArchitectureTests/ContractsArchitectureTests.cs`
    - `Contracts_should_not_reference_other_LegalAiAr_assemblies` (assembly references)
- [x] **Modify**: `LegalAiAr.ArchitectureTests.csproj` — `ProjectReference` to Contracts

### 2. Auth: responses

- [x] **Move**: `MeResponse`, `LogoutResponse` → `Contracts/Responses/Auth/`
- [x] **Modify**: `Endpoints/Auth/GetAuthMe.cs`, `PostAuthLogout.cs` — Contracts usings
- [x] **Delete**: `Api/Models/Auth/*.cs`

### 3. Rulings: requests + validation

- [x] **Move**: `SearchRulingsRequest`, `SearchFiltersRequest` → `Contracts/Requests/Rulings/`
- [x] **Modify**: `Api/Mapping/SearchRulingsRequestMapper.cs` — Contracts namespace
- [x] **Modify**: `Endpoints/Rulings/PostRulingsSearch.cs` — `Produces` + Contracts param type
- [x] **Create**: `Api/Validators/Rulings/SearchRulingsRequestValidator.cs`
    - `Page` ≥ 1, `PageSize` 1–50, aligned with `SearchRulingsQueryValidator`
- [x] **Register**: validator in DI (same pattern as other `IValidator<>` in Api)
- [x] **Delete**: `Api/Models/SearchRulingsRequest.cs`

### 4. Rulings: responses

- [x] **Move** to `Contracts/Responses/Rulings/`: `RulingSearchResultDto`, `RelatedRulingDto`, `SearchRulingsResult`, and `RulingDto` tree (`CourtDto`, `CitationDto`, etc.)
- [x] **Create**: `SearchFacetsResponse`, `FacetValueResponse` in Contracts
- [x] **Create**: `Application/Mapping/Rulings/SearchFacetsMapper.cs` — `Core.Models.SearchFacets` → response
- [x] **Modify**: rulings handlers/endpoints — Contracts usings; facets mapped to `SearchFacetsResponse`
- [x] **Modify**: endpoints `GetRulingsFacets`, `GetRulingById`, `GetRulingRelated`, `PostRulingsSearch` — `.Produces<>` Contracts
- [x] **Delete**: emptied `Application/Rulings/DTOs/` files

### 5. Statutes: responses + enum mapping

- [x] **Move** to `Contracts/Responses/Statutes/`: statute DTOs with enum fields as `string?` / `string`
- [x] **Move**: `StatutePageDto` → `StatutePageResponse` in Contracts; keep `GetStatutesQuery` in Application with Core enums
- [x] **Create**: `Application/Mapping/StatuteContractMapper.cs`
- [x] **Modify**: statute handlers + endpoints `GetStatutesList`, `GetStatuteById`, `GetStatutesPyramid`
- [x] **Delete**: `Application/Statutes/DTOs/StatuteDtos.cs` when empty; remove `StatutePageDto` from query file

### 6. Catalogs + Thesaurus + Search

- [x] **Move**: `CatalogDtos.cs` → `Contracts/Responses/Catalogs/`
- [x] **Move**: thesaurus DTOs → `Contracts/Responses/Thesaurus/`
- [x] **Move**: global search DTOs → `Contracts/Responses/Search/`
- [x] **Modify**: handlers + endpoints for `courts`, `persons`, `thesaurus`, `search`
- [x] **Delete**: corresponding Application DTO files

### 7. OpenAPI + documentation

- [x] **Verify**: Swagger — single schema per Contract type
- [x] **Modify**: `docs/technical/22-api-endpoints.md` — Contracts layout + additive versioning policy (R1)
- [x] **Modify**: `docs/technical/README.md` — doc 22 in index
- [x] **Modify**: work item Tasks/AC `[x]`; `docs/roadmap/STATUS.md` on merge

### 8. Tests

- [x] **Update**: `SearchRulingsHandlerTests`, `GetRelatedRulingsHandlerTests`, and other tests with old namespaces
- [x] **Create** (optional): Contracts isolation architecture test

### 9. Verification

- [x] `dotnet build` — 0 warnings (`TreatWarningsAsErrors`)
- [x] `dotnet test` — green, including `LegalAiAr.ArchitectureTests`
- [x] Smoke: `GET /api/auth/me`, `POST /api/rulings/search`, `GET /api/statutes`, `GET /api/search?q=...`
- [x] Swagger: types under `LegalAiAr.Contracts.*`
- [x] Work item acceptance criteria + DoD

---

## Project layout (target)

```
LegalAiAr.Contracts/
├── Requests/
│   ├── Auth/
│   ├── Rulings/
│   ├── Statutes/
│   └── ...
└── Responses/
    ├── Auth/
    ├── Rulings/
    └── ...
```

---

## Acceptance Criteria

- [x] `LegalAiAr.Contracts` builds independently; no upward references
- [x] Auth + primary catalog endpoints use Contract types at the HTTP boundary (post W09 wave 1–2)
- [x] Application layer does not expose HTTP-specific types in handler signatures (handlers use commands/queries)
- [x] NetArchTest enforces Contracts isolation
- [x] `dotnet build` zero warnings; tests green
- [x] Swagger shows stable schema names from Contracts
- [x] Architecture standard §16 backend checklist — Contracts row satisfied
- [x] Documentation round-trip complete (DoD)

---

## Dependencies

- **Blocks:** Clean OpenAPI contract for R1 features; frontend typed clients generated from stable DTOs
- **Prerequisites:** F00-W02 (solution structure), F00-W08 (architecture tests); **recommended after F00-W09 wave 1** (auth endpoints exist as Minimal API)

---

---

## Completion

- **Merged to `main`:** [PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112) (2026-06-02). DoD documentation round-trip complete.

---

_F00 - W10 - LegalAiAr.Contracts API DTO Layer — Legal Ai Ar_
