# F02 - W03 - Frontend - Layout Shell Sidebar y Navbar

> **Feature:** F02 - Dashboard Principal
> **Release:** 1.0 | **Sprint:** S02
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar el layout principal de la aplicación: shell con sidebar navegable, navbar con info de usuario y notificaciones, footer.

---

## Tareas

- [ ] Crear `LayoutComponent` (shell principal) como standalone component
- [ ] Crear `SidebarComponent` con navegación por feature (lazy routes)
- [ ] Crear `NavbarComponent` con: logo, usuario logueado, badge notificaciones, tema claro/oscuro
- [ ] Crear `FooterComponent` minimal
- [ ] Implementar responsive: sidebar colapsable en tablet
- [ ] Configurar rutas lazy-loaded por feature module
- [ ] Integrar con AuthService para mostrar nombre y rol del usuario

---

## Criterios de Aceptación

- [ ] El componente renderiza correctamente en desktop (>1200px) y tablet (768-1200px)
- [ ] Los estados de loading, error y empty se manejan correctamente
- [ ] La accesibilidad cumple WCAG 2.1 AA (verificar con axe DevTools)
- [ ] Los tests unitarios cubren > 80% del componente
- [ ] El componente es standalone y reutilizable donde corresponda

---

## Notas Técnicas

- Angular 19 con standalone components (sin NgModules)
- State management: Angular Signals + NgRx Signal Store para estado global
- UI: Angular Material 19 + Tailwind CSS 4
- Formularios: Typed Reactive Forms
- Referir a la documentación integral (F02-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/layout/layout.component.ts
src/app/layout/sidebar/sidebar.component.ts
src/app/layout/navbar/navbar.component.ts
src/app/layout/footer/footer.component.ts
src/app/app.routes.ts (modificar)
```

---

## Dependencias

- Depende de: F02-W01 (Documentación integral)

---

*F02 - W03 - Frontend - Layout Shell Sidebar y Navbar — Legal Ai Ar*
