# F05 - W06 - Frontend - Visualizacion Grafo de Relaciones

> **Feature:** F05 - Detalle de Norma
> **Release:** 1.0 | **Sprint:** S03-S04
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar visualización interactiva de grafo de relaciones legales usando D3.js o Cytoscape.js.

---

## Tareas

- [ ] Evaluar D3.js force-directed vs Cytoscape.js (elegir uno)
- [ ] Crear componente Angular wrapper para la librería elegida
- [ ] Implementar rendering de nodos coloreados por tipo
- [ ] Implementar rendering de edges etiquetados por tipo de relación
- [ ] Soportar zoom, pan y drag de nodos
- [ ] Implementar click en nodo para mostrar detalle
- [ ] Implementar expansión de nodo (cargar vecinos on-demand)
- [ ] Optimizar para grafos de hasta 200 nodos
- [ ] Escribir tests

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
- Referir a la documentación integral (F05-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/shared/components/legal-graph/legal-graph.component.ts
src/app/shared/components/legal-graph/legal-graph.component.html
src/app/shared/components/legal-graph/graph.types.ts
src/app/shared/components/legal-graph/legal-graph.component.spec.ts
```

---

## Dependencias

- Depende de: F05-W01 (Documentación integral)

---

*F05 - W06 - Frontend - Visualizacion Grafo de Relaciones — Legal Ai Ar*
