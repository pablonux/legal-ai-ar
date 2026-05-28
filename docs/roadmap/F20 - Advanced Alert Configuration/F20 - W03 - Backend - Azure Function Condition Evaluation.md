# F20 - W03 - Backend - Azure Function Condition Evaluation

> **Feature:** F20 - Advanced Alert Configuration
> **Release:** 4.0 | **Sprint:** S10
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Implement an Azure Function (.NET 10 isolated worker) for alert condition evaluation.

---

## Tasks

- [ ] Create the function class in `Functions/`
- [ ] Configure the trigger (Timer/Queue/Blob as appropriate)
- [ ] Implement the business logic
- [ ] Configure output bindings (SQL, Queue, etc.)
- [ ] Add connection strings in local.settings.json
- [ ] Add secrets to Key Vault for production
- [ ] Write unit tests
- [ ] Test locally with Azure Functions Core Tools

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
- Refer to the comprehensive documentation (F20-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Functions/{NombreFunction}.cs
src/Functions/local.settings.json (modificar)
tests/Functions.Tests/{NombreFunction}Tests.cs
```

---

## Dependencies

- Depends on: F20-W01 (Comprehensive Documentation)

---

*F20 - W03 - Backend - Azure Function Condition Evaluation — Legal Ai Ar*
