# F03 - W07 - Frontend - Results List with Highlight

> **Feature:** F03 - Legal Norm Search
> **Release:** 1.0 | **Sprint:** S02-S03
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement a list view with an Angular Material DataTable for Legal Norm Search. Includes sort, filter, and server-side pagination.

---

## Tasks

- [ ] Create the list component as a standalone component
- [ ] Implement `MatTable` with columns per the model
- [ ] Add `MatSort` for column sorting
- [ ] Add `MatPaginator` with server-side pagination
- [ ] Implement filters with form controls
- [ ] Connect to the corresponding service/API
- [ ] Manejar estados: loading, empty, error
- [ ] Write unit tests

---

## Acceptance Criteria

- [ ] The component renders correctly on desktop (>1200px) and tablet (768-1200px)
- [ ] The loading, error, and empty states are handled correctly
- [ ] Accessibility meets WCAG 2.1 AA (verify with axe DevTools)
- [ ] Unit tests cover > 80% of the component
- [ ] The component is standalone and reusable where applicable

---

## Technical Notes

- Angular 19 with standalone components (no NgModules)
- State management: Angular Signals + NgRx Signal Store for global state
- UI: Angular Material 19 + Tailwind CSS 4
- Forms: Typed Reactive Forms
- Refer to the comprehensive documentation (F03-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/features/{feature}/{feature}-list/{feature}-list.component.ts
src/app/features/{feature}/{feature}-list/{feature}-list.component.html
src/app/features/{feature}/services/{feature}.service.ts
src/app/features/{feature}/{feature}-list/{feature}-list.component.spec.ts
```

---

## Dependencies

- Depends on: F03-W01 (Comprehensive Documentation)

---

*F03 - W07 - Frontend - Results List with Highlight — Legal Ai Ar*
