# F09 - W04 - Backend - Integration with AI Search and SQL Graph

> **Feature:** F09 - Regulatory Agent
> **Release:** 2.0 | **Sprint:** S06
> **Type:** backend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Configure the Azure AI Search index for Regulatory Agent. Includes schema definition, scoring profile, suggester, and skillset.

---

## Tasks

- [ ] Define the index schema (fields, types, attributes)
- [ ] Create the scoring profile with weights for BM25 and vectors
- [ ] Configure the suggester for autocomplete
- [ ] Configure the skillset for PDF text extraction (if applicable)
- [ ] Configure the vector projections
- [ ] Create the indexer connected to Azure SQL as the data source
- [ ] Implement the SearchService in C# with Azure.Search.Documents
- [ ] Test searches with sample data

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
- Refer to the comprehensive documentation (F09-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Infrastructure/Search/{Feature}SearchService.cs
src/Infrastructure/Search/IndexDefinitions/{feature}-index.json
tests/Infrastructure.Tests/Search/{Feature}SearchServiceTests.cs
```

---

## Dependencies

- Depends on: F09-W01 (Comprehensive Documentation)

---

*F09 - W04 - Backend - Integration with AI Search and SQL Graph — Legal Ai Ar*
