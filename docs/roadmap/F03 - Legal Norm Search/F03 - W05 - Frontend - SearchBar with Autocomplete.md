# F03 - W05 - Frontend - SearchBar con Autocompletado

> **Feature:** F03 - Busqueda de Normas
> **Release:** 1.0 | **Sprint:** S02-S03
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar componente SearchBar reutilizable con autocompletado (debounce 300ms) y soporte para búsqueda por Enter o click.

---

## Tareas

- [ ] Crear `SearchBarComponent` como standalone component en `shared/components/`
- [ ] Implementar autocompletado con `MatAutocomplete` y debounce 300ms
- [ ] Conectar al endpoint de sugerencias (`/api/buscar/sugerencias`)
- [ ] Emitir evento `search` con el query al presionar Enter o click en buscar
- [ ] Soportar placeholder configurable via @Input
- [ ] Agregar loading spinner mientras carga sugerencias
- [ ] Escribir tests unitarios del componente

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
- Referir a la documentación integral (F03-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/shared/components/search-bar/search-bar.component.ts
src/app/shared/components/search-bar/search-bar.component.html
src/app/shared/components/search-bar/search-bar.component.spec.ts
```

---

## Dependencias

- Depende de: F03-W01 (Documentación integral)

---

*F03 - W05 - Frontend - SearchBar con Autocompletado — Legal Ai Ar*
