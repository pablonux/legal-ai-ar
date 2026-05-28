# F21 - W05 - Frontend - Filters by Relationship Type

> **Feature:** F21 - Legal Graph Explorer
> **Release:** 4.0 | **Sprint:** S11
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement Filters by Relationship Type for the Legal Graph Explorer feature.

---

## Tasks

- [ ] Create a standalone component per the W01 design
- [ ] Implement UI logic with Angular Signals
- [ ] Connect to the corresponding service/API
- [ ] Handle states: loading, success, error, empty
- [ ] Apply styles with Angular Material + Tailwind
- [ ] Verify responsive (desktop + tablet)
- [ ] Verify accessibility (ARIA labels, focus management)
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
- Refer to the comprehensive documentation (F21-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/features/{feature}/{component}/{component}.component.ts
src/app/features/{feature}/{component}/{component}.component.html
src/app/features/{feature}/{component}/{component}.component.spec.ts
```

---

## Dependencies

- Depends on: F21-W01 (Comprehensive Documentation)

---

*F21 - W05 - Frontend - Filters by Relationship Type — Legal Ai Ar*
