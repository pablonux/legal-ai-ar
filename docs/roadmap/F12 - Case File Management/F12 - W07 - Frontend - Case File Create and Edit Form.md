# F12 - W07 - Frontend - Case File Create and Edit Form

> **Feature:** F12 - Case File Management
> **Release:** 2.0 | **Sprint:** S05-S06
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement a typed reactive form for Case File Management. Includes validations, dynamic controls, and visual feedback.

---

## Tasks

- [ ] Create the form component as a standalone component
- [ ] Implement a Typed Reactive Form with validations
- [ ] Add controls: inputs, selects, datepickers, textareas per the model
- [ ] Implement visual validation (mat-error with descriptive messages)
- [ ] Conectar al service para submit (POST/PUT)
- [ ] Manejar estados: loading, success, error
- [ ] Support edit mode (prefill with existing data)
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
- Refer to the comprehensive documentation (F12-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/features/{feature}/{feature}-form/{feature}-form.component.ts
src/app/features/{feature}/{feature}-form/{feature}-form.component.html
src/app/features/{feature}/{feature}-form/{feature}-form.component.spec.ts
```

---

## Dependencies

- Depends on: F12-W01 (Comprehensive Documentation)

---

*F12 - W07 - Frontend - Case File Create and Edit Form — Legal Ai Ar*
