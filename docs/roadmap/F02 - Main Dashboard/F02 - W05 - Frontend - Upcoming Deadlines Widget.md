# F02 - W05 - Frontend - Widget Plazos Proximos

> **Feature:** F02 - Dashboard Principal
> **Release:** 1.0 | **Sprint:** S02
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar Frontend - Widget Plazos Proximos para la feature F02 - Dashboard Principal.

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
- Referir a la documentación integral (F02-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/features/{feature}/{component}/{component}.component.ts
src/app/features/{feature}/{component}/{component}.component.html
src/app/features/{feature}/{component}/{component}.component.spec.ts
```

---

## Dependencias

- Depende de: F02-W01 (Documentación integral)

---

*F02 - W05 - Frontend - Widget Plazos Proximos — Legal Ai Ar*
