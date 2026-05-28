# FT01 - W07 - Testing - Notification Tests

> **Feature:** FT01 - Real-Time Notifications
> **Release:** Transversal | **Sprint:** S03-S04
> **Type:** testing | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (QA)

---

## Description

Write unit, integration, and E2E tests for the Real-Time Notifications feature.

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

- Depends on: all work items of feature FT01 completed

---

*FT01 - W07 - Testing - Notification Tests — Legal Ai Ar*
