# F13 - W02 - Backend - EF Core Deadline Model and Migrations

> **Feature:** F13 - Deadline Management
> **Release:** 2.0 | **Sprint:** S06-S07
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Create the data model with Entity Framework Core 10 for the Deadline Management feature. Includes entities, Fluent API configuration, and the initial migration.

---

## Tasks

- [ ] Create C# entities in `Domain/Entities/`
- [ ] Create the Fluent API configuration in `Infrastructure/Persistence/Configurations/`
- [ ] Add the DbSet to `ApplicationDbContext`
- [ ] Create a migration with `dotnet ef migrations add {Name}`
- [ ] Verificar que la migration genera el SQL esperado
- [ ] Ejecutar migration en la base de desarrollo
- [ ] Create seed data if applicable

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
- Refer to the comprehensive documentation (F13-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Domain/Entities/{Entidad}.cs
src/Infrastructure/Persistence/Configurations/{Entidad}Configuration.cs
src/Infrastructure/Persistence/ApplicationDbContext.cs (modificar)
src/Infrastructure/Persistence/Migrations/{timestamp}_{Nombre}.cs
```

---

## Dependencies

- Depends on: F13-W01 (Comprehensive Documentation)

---

*F13 - W02 - Backend - EF Core Deadline Model and Migrations — Legal Ai Ar*
