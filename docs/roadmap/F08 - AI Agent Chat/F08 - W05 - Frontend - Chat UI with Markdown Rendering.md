# F08 - W05 - Frontend - Chat UI con Markdown Rendering

> **Feature:** F08 - Chat con Agentes IA
> **Release:** 2.0 | **Sprint:** S05-S06
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar la interfaz de chat con los agentes IA. Soporte para markdown, streaming de respuestas y fuentes citadas.

---

## Tareas

- [ ] Crear `ChatComponent` con área de mensajes y input
- [ ] Implementar rendering de markdown con ngx-markdown
- [ ] Soportar streaming de respuesta (tokens aparecen progresivamente)
- [ ] Mostrar indicador de "pensando" mientras el agente procesa
- [ ] Renderizar fuentes citadas como chips clickeables al final de cada respuesta
- [ ] Implementar scroll automático al nuevo mensaje
- [ ] Soportar Ctrl+Enter para enviar
- [ ] Botón "Copiar" por mensaje
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
- Referir a la documentación integral (F08-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/features/agentes/chat/chat.component.ts
src/app/features/agentes/chat/chat.component.html
src/app/features/agentes/chat/message-bubble/message-bubble.component.ts
src/app/features/agentes/chat/source-chip/source-chip.component.ts
src/app/features/agentes/services/chat.service.ts
```

---

## Dependencias

- Depende de: F08-W01 (Documentación integral)

---

*F08 - W05 - Frontend - Chat UI con Markdown Rendering — Legal Ai Ar*
