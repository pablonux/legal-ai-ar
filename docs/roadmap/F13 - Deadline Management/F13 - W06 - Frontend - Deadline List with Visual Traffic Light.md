# F13 - W06 - Frontend - Lista de Plazos con Semaforo Visual

> **Feature:** F13 - Gestion de Plazos
> **Release:** 2.0 | **Sprint:** S06-S07
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar vista de lista con Angular Material DataTable para Gestion de Plazos. Incluye sort, filter, paginación server-side.

---

## Tareas

- [ ] Crear componente de lista como standalone component
- [ ] Implementar `MatTable` con columnas según el modelo
- [ ] Agregar `MatSort` para ordenamiento por columnas
- [ ] Agregar `MatPaginator` con paginación server-side
- [ ] Implementar filtros con controles de formulario
- [ ] Conectar al service/API correspondiente
- [ ] Manejar estados: loading, empty, error
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
- Referir a la documentación integral (F13-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/features/{feature}/{feature}-list/{feature}-list.component.ts
src/app/features/{feature}/{feature}-list/{feature}-list.component.html
src/app/features/{feature}/services/{feature}.service.ts
src/app/features/{feature}/{feature}-list/{feature}-list.component.spec.ts
```

---

## Dependencias

- Depende de: F13-W01 (Documentación integral)

---

*F13 - W06 - Frontend - Lista de Plazos con Semaforo Visual — Legal Ai Ar*
