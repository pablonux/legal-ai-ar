# F03 - W05 - Frontend - SearchBar with Autocomplete

> **Feature:** F03 - Legal Norm Search
> **Release:** 1.0 | **Sprint:** S02-S03
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement a reusable SearchBar component with autocomplete (300ms debounce) and support for search by Enter or click.

---

## Tasks

- [ ] Create `SearchBarComponent` as a standalone component in `shared/components/`
- [ ] Implement autocomplete with `MatAutocomplete` and a 300ms debounce
- [ ] Conectar al endpoint de sugerencias (`/api/buscar/sugerencias`)
- [ ] Emitir evento `search` con el query al presionar Enter o click en buscar
- [ ] Support a configurable placeholder via @Input
- [ ] Add a loading spinner while suggestions load
- [ ] Write unit tests del componente

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
src/app/shared/components/search-bar/search-bar.component.ts
src/app/shared/components/search-bar/search-bar.component.html
src/app/shared/components/search-bar/search-bar.component.spec.ts
```

---

## Dependencies

- Depends on: F03-W01 (Comprehensive Documentation)

---

*F03 - W05 - Frontend - SearchBar with Autocomplete — Legal Ai Ar*
