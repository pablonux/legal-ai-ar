# F03 - W02 - Backend - AI Search Index for Legal Norms

> **Feature:** F03 - Legal Norm Search
> **Release:** 1.0 | **Sprint:** S02-S03
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Configure the Azure AI Search index for Legal Norm Search. Includes schema definition, scoring profile, suggester, and skillset.

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
- Refer to the comprehensive documentation (F03-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Infrastructure/Search/{Feature}SearchService.cs
src/Infrastructure/Search/IndexDefinitions/{feature}-index.json
tests/Infrastructure.Tests/Search/{Feature}SearchServiceTests.cs
```

---

## Dependencies

- Depends on: F03-W01 (Comprehensive Documentation)

---

*F03 - W02 - Backend - AI Search Index for Legal Norms — Legal Ai Ar*
