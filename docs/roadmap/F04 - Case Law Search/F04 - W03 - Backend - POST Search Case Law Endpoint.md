# F04 - W03 - Backend - POST Search Case Law Endpoint

> **Feature:** F04 - Case Law Search
> **Release:** 1.0 | **Sprint:** S03
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Implement the API endpoints for Case Law Search using ASP.NET Core 10 Minimal API.

---

## Tasks

- [ ] Create request and response DTOs in `Application/DTOs/`
- [ ] Create FluentValidation validators in `Application/Validators/`
- [ ] Implement the service/handler in `Application/Services/`
- [ ] Register the endpoints in `Api/Endpoints/{Feature}Endpoints.cs`
- [ ] Add role-based authorization where applicable
- [ ] Document the endpoints with `.WithOpenApi()`
- [ ] Write unit tests for the service
- [ ] Write integration tests for the endpoints

---

## Acceptance Criteria

- [ ] The implemented functionality meets the W01 acceptance criteria
- [ ] Tests pass
- [ ] The code is reviewed by at least 1 peer

---

## Technical Notes

- Framework: .NET 10 LTS with ASP.NET Core Minimal API
- ORM: Entity Framework Core 10
- Validation: FluentValidation 12.x
- Logging: Serilog with an Application Insights sink
- Refer to the comprehensive documentation (F04-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Application/DTOs/{Feature}Dto.cs
src/Application/Validators/{Feature}Validator.cs
src/Application/Services/{Feature}Service.cs
src/Api/Endpoints/{Feature}Endpoints.cs
tests/Application.Tests/{Feature}ServiceTests.cs
tests/Api.Tests/{Feature}EndpointTests.cs
```

---

## Dependencies

- Depends on: F04-W01 (Comprehensive Documentation)

---

*F04 - W03 - Backend - POST Search Case Law Endpoint — Legal Ai Ar*
