# FT03 - W02 - Frontend - Angular Material Theming Claro y Oscuro

> **Feature:** FT03 - Tema y Accesibilidad
> **Release:** Transversal | **Sprint:** S02
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar Frontend - Angular Material Theming Claro y Oscuro para la feature FT03 - Tema y Accesibilidad.

---

## Tareas

- [ ] Crear componente standalone según diseño del W01
- [ ] Implementar lógica de UI con Angular Signals
- [ ] Conectar al service/API correspondiente
- [ ] Manejar estados: loading, success, error, empty
- [ ] Aplicar estilos con Angular Material + Tailwind
- [ ] Verificar responsive (desktop + tablet)
- [ ] Verificar accesibilidad (ARIA labels, focus management)
- [ ] Escribir tests unitarios

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
- Referir a la documentación integral (FT03-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/features/{feature}/{component}/{component}.component.ts
src/app/features/{feature}/{component}/{component}.component.html
src/app/features/{feature}/{component}/{component}.component.spec.ts
```

---

## Dependencias

- Depende de: FT03-W01 (Documentación integral)

---

*FT03 - W02 - Frontend - Angular Material Theming Claro y Oscuro — Legal Ai Ar*
