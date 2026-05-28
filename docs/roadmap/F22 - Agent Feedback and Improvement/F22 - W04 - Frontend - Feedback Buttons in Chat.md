# F22 - W04 - Frontend - Feedback Buttons in Chat

> **Feature:** F22 - Agent Feedback and Improvement
> **Release:** 4.0 | **Sprint:** S11
> **Type:** frontend | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Fullstack Dev (Frontend)

---

## Description

Implement the chat interface with the AI agents. Support for markdown, response streaming, and cited sources.

---

## Tasks

- [ ] Create `ChatComponent` with a message area and input
- [ ] Implement markdown rendering with ngx-markdown
- [ ] Support response streaming (tokens appear progressively)
- [ ] Mostrar indicador de "pensando" mientras el agente procesa
- [ ] Renderizar fuentes citadas como chips clickeables al final de cada respuesta
- [ ] Implement auto-scroll to the new message
- [ ] Support Ctrl+Enter to send
- [ ] Per-message "Copy" button
- [ ] Write unit tests

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
- Refer to the comprehensive documentation (F22-W01) for mockups and acceptance criteria

---

## Files to Create/Modify

```
src/app/features/agentes/chat/chat.component.ts
src/app/features/agentes/chat/chat.component.html
src/app/features/agentes/chat/message-bubble/message-bubble.component.ts
src/app/features/agentes/chat/source-chip/source-chip.component.ts
src/app/features/agentes/services/chat.service.ts
```

---

## Dependencies

- Depends on: F22-W01 (Comprehensive Documentation)

---

*F22 - W04 - Frontend - Feedback Buttons in Chat — Legal Ai Ar*
