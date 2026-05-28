# F02 - W03 - Frontend - Shell Layout Sidebar and Navbar

> **Feature:** F02 - Main Dashboard
> **Release:** 1.0 | **Sprint:** S02
> **Type:** frontend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement the application's main layout: shell with a navigable sidebar, navbar with user info and notifications, footer.

---

## Tasks

- [ ] Create `LayoutComponent` (main shell) as a standalone component
- [ ] Create `SidebarComponent` with per-feature navigation (lazy routes)
- [ ] Create `NavbarComponent` with: logo, logged-in user, notifications badge, light/dark theme
- [ ] Create a minimal `FooterComponent`
- [ ] Implement responsive: collapsible sidebar on tablet
- [ ] Configure lazy-loaded routes per feature module
- [ ] Integrar con AuthService para mostrar nombre y rol del usuario

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
- Refer to the comprehensive documentation (F02-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/layout/layout.component.ts
src/app/layout/sidebar/sidebar.component.ts
src/app/layout/navbar/navbar.component.ts
src/app/layout/footer/footer.component.ts
src/app/app.routes.ts (modificar)
```

---

## Dependencies

- Depends on: F02-W01 (Comprehensive Documentation)

---

*F02 - W03 - Frontend - Shell Layout Sidebar and Navbar — Legal Ai Ar*
