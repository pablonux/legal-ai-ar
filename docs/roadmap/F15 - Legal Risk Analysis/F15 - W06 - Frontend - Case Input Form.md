# F15 - W06 - Frontend - Case Input Form

> **Feature:** F15 - Legal Risk Analysis
> **Release:** 3.0 | **Sprint:** S08-S09
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement a typed reactive form for Legal Risk Analysis. Includes validations, dynamic controls, and visual feedback.

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
- Refer to the comprehensive documentation (F15-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/features/{feature}/{feature}-form/{feature}-form.component.ts
src/app/features/{feature}/{feature}-form/{feature}-form.component.html
src/app/features/{feature}/{feature}-form/{feature}-form.component.spec.ts
```

---

## Dependencies

- Depends on: F15-W01 (Comprehensive Documentation)

---

*F15 - W06 - Frontend - Case Input Form — Legal Ai Ar*
