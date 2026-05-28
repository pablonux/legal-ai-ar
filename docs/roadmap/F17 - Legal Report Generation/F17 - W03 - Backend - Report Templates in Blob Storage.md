# F17 - W03 - Backend - Report Templates in Blob Storage

> **Feature:** F17 - Legal Report Generation
> **Release:** 3.0 | **Sprint:** S08-S09
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Implement document upload/download to Azure Blob Storage for Legal Report Generation.

---

## Tasks

- [ ] Create BlobStorageService in `Infrastructure/Storage/`
- [ ] Implement upload with name: `{container}/{id}/{guid}_{filename}`
- [ ] Implement download with a short-lived SAS token
- [ ] Configure the container and access policy
- [ ] Add the connection string to Key Vault
- [ ] Write tests with Azurite (local emulator)

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
- Refer to the comprehensive documentation (F17-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Infrastructure/Storage/BlobStorageService.cs
src/Infrastructure/Storage/IBlobStorageService.cs
tests/Infrastructure.Tests/Storage/BlobStorageServiceTests.cs
```

---

## Dependencies

- Depends on: F17-W01 (Comprehensive Documentation)

---

*F17 - W03 - Backend - Report Templates in Blob Storage — Legal Ai Ar*
