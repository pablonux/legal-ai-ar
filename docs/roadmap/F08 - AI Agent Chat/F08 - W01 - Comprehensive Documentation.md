# F08 - W01 - Comprehensive Documentation

> **Feature:** F08 - AI Agent Chat
> **Release:** 2.0 | **Sprint:** S05-S06
> **Type:** Documentation | **Priority:** Critical (blocking)
> **Estimate:** 3 story points

---

## 1. General Description

Chat interface to interact with specialized agents. Response streaming, source citation, conversation history.

---

## 2. Architecture Diagram

```mermaid
sequenceDiagram
    participant U as Usuario
    participant SPA as Angular SPA
    participant SR as SignalR
    participant API as .NET 10 API
    participant SK as Semantic Kernel
    participant ORC as Orquestador
    participant AGN as Agente Normativo
    participant AGJ as Agente Jurisprudencial
    participant AIS as AI Search
    participant SQL as Azure SQL Graph
    participant OAI as Azure OpenAI

    U->>SPA: Escribe consulta
    SPA->>API: POST /api/agents/chat
    API->>SK: Procesar consulta
    SK->>ORC: Classify intent
    ORC->>OAI: Clasificar (GPT-4o)
    OAI->>ORC: Tipo: normativo
    ORC->>AGN: Delegar consulta
    AGN->>AIS: BuscarNorma()
    AGN->>SQL: MATCH traversal
    AGN->>OAI: Generar respuesta (stream)
    OAI-->>SR: Token a token
    SR-->>SPA: Stream de respuesta
    SPA-->>U: Renderiza progresivo
```

---

## 3. Data Model

> Define the specific data model during the W01 implementation.
> Refer to the ontology in `docs/ontology/argentine-legal-ontology.md` for the base classes.

---

## 4. API Endpoints

> The specific endpoints will be defined based on the features document: `docs/roadmap/features.md`, API Endpoints section.

---

## 5. UI / UX Description

### Layout del chat

```
┌──────────────────────────────────────────┬─────────────┐
│  CONVERSACIÓN                            │  FUENTES    │
│                                          │             │
│  👤 ¿Cuál es el plazo de prescripción   │  📄 Art. 2562│
│     para un reclamo por daños?           │     CCCN    │
│                                          │             │
│  🤖 Según el art. 2562 del CCCN, el     │  ⚖️ CSJN    │
│     plazo de prescripción para la        │  "Gómez c/  │
│     acción por daños derivados de la     │   Estado"   │
│     responsabilidad civil es de **3      │  2024       │
│     años** contados desde...             │             │
│     [ver más]                            │             │
│                                          │             │
│  📎 Fuentes: Art. 2562 CCCN | Gómez c/  │             │
│     Estado Nacional (CSJN, 2024)         │             │
│                                          │             │
├──────────────────────────────────────────┤             │
│  [Escribe tu consulta...        ] [Enviar]│             │
└──────────────────────────────────────────┴─────────────┘
```

---

## 6. Acceptance Criteria

- [ ] El usuario puede escribir una consulta y recibir respuesta del agente
- [ ] La respuesta se renderiza progresivamente (streaming token a token)
- [ ] Cada respuesta incluye fuentes citadas (normas, fallos) como links clickeables
- [ ] Las fuentes llevan a la vista de detalle de la norma/fallo correspondiente
- [ ] El historial de conversaciones persiste entre sesiones
- [ ] El markdown en las respuestas se renderiza correctamente (headers, listas, code blocks)
- [ ] El indicador de "pensando" se muestra mientras el agente procesa
- [ ] A conversation can be exported to .docx
- [ ] Solo los usuarios con rol "abogado" tienen acceso al chat

---

## 7. Dependencies

- **Depends on:** F01 (Auth), F03 (functional legal norm search), F04 (functional case law search)
- **Blocks:** F09, F10, F11 (specialized agents), F15 (risk analysis)
- **NuGet:** Microsoft.SemanticKernel, Microsoft.AspNetCore.SignalR
- **npm:** @microsoft/signalr, ngx-markdown

---

## 8. Technical Notes

- Semantic Kernel SDK v1.x para .NET 10
- The orchestrator uses an intent-classification prompt to route to the correct agent
- SignalR con protocolo MessagePack para mejor performance en streaming
- Cada mensaje del agente incluye metadata: `{sources: [{tipo, id, titulo, url}]}`
- El historial se almacena en Azure SQL (tabla Conversacion + tabla Mensaje)
- Limit the context to the last 10 messages to avoid exceeding the token limit
- Use `IChatCompletionService` with streaming for response generation

---

## 9. Work Items of this Feature

| ID | Name | Type | Sprint |
|----|--------|------|--------|
| F08-W01 | Comprehensive Documentation | doc | S05-S06 |
| F08-W02 | Backend - Semantic Kernel Setup and Orchestrator | backend | S05-S06 |
| F08-W03 | Backend - POST Chat with SignalR Streaming Endpoint | backend | S05-S06 |
| F08-W04 | Backend - Conversation Persistence | backend | S05-S06 |
| F08-W05 | Frontend - Chat UI with Markdown Rendering | frontend | S05-S06 |
| F08-W06 | Frontend - SignalR Client for Streaming | frontend | S05-S06 |
| F08-W07 | Frontend - Cited Sources Panel | frontend | S05-S06 |
| F08-W08 | Frontend - Conversation History | frontend | S05-S06 |
| F08-W09 | Testing - E2E Chat Tests | testing | S05-S06 |

---

## 10. Definition of Done

- [ ] Code reviewed by at least 1 peer (PR approved)
- [ ] Unit tests with > 80% coverage
- [ ] Integration tests for endpoints
- [ ] No errors in the CI build
- [ ] API documentation updated (Swagger/OpenAPI)
- [ ] Angular components documented with JSDoc
- [ ] Accessibility validated (WCAG 2.1 AA)
- [ ] Responsive verified on desktop and tablet
- [ ] Performance: load time < 3 sec, API response < 2 sec
- [ ] Feature flag configured (if applicable)

---

*F08 - AI Agent Chat — Comprehensive Documentation — Legal Ai Ar*
