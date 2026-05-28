# F21 - W03 - Frontend - Interactive Graph Component D3 or Cytoscape

> **Feature:** F21 - Legal Graph Explorer
> **Release:** 4.0 | **Sprint:** S11
> **Type:** frontend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement an interactive legal relationship graph visualization using D3.js or Cytoscape.js.

---

## Tasks

- [ ] Evaluar D3.js force-directed vs Cytoscape.js (elegir uno)
- [ ] Create an Angular wrapper component for the chosen library
- [ ] Implement rendering of nodes colored by type
- [ ] Implement rendering of edges labeled by relationship type
- [ ] Support zoom, pan, and node drag
- [ ] Implement node click to show detail
- [ ] Implement node expansion (load neighbors on-demand)
- [ ] Optimizar para grafos de hasta 200 nodos
- [ ] Write tests

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
src/app/shared/components/legal-graph/legal-graph.component.ts
src/app/shared/components/legal-graph/legal-graph.component.html
src/app/shared/components/legal-graph/graph.types.ts
src/app/shared/components/legal-graph/legal-graph.component.spec.ts
```

---

## Dependencies

- Depends on: F21-W01 (Comprehensive Documentation)

---

*F21 - W03 - Frontend - Interactive Graph Component D3 or Cytoscape — Legal Ai Ar*
