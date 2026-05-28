# F12 - W11 - Testing - Case File CRUD Tests

> **Feature:** F12 - Case File Management
> **Release:** 2.0 | **Sprint:** S05-S06
> **Type:** testing | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (QA)

---

## Description

Write unit, integration, and E2E tests for the Case File Management feature.

---

## Tasks

- [ ] Backend service unit tests (xUnit + Moq)
- [ ] API endpoint integration tests (WebApplicationFactory)
- [ ] Angular component unit tests (Jest)
- [ ] E2E tests of the full flow (Cypress)
- [ ] Verify coverage > 80%
- [ ] Verify all tests pass in CI

---

## Acceptance Criteria

- [ ] Test coverage > 80% on new code
- [ ] All tests pass in CI
- [ ] E2E tests cover the feature's full happy path
- [ ] Integration tests cover all of the feature's endpoints

---

## Technical Notes

- Backend: xUnit + Moq + WebApplicationFactory for integration tests
- Frontend: Jest for unit tests + Cypress for E2E
- CI: tests must run in the GitHub Actions / Azure DevOps pipeline

---

## Files to Create/Modify

```
tests/Application.Tests/{Feature}/
tests/Api.Tests/{Feature}/
src/app/features/{feature}/**/*.spec.ts
cypress/e2e/{feature}.cy.ts
```

---

## Dependencies

- Depends on: all work items of feature F12 completed

---

*F12 - W11 - Testing - Case File CRUD Tests — Legal Ai Ar*
