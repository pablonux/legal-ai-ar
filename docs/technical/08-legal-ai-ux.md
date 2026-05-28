# 08 — Legal AI UX

> **Project:** Legal Ai Ar | **Category:** AI User Experience
> **Status:** Not defined — all items are new
> **Last updated:** May 2026

---

## 1. Description

The AI user experience in a legal context has specific requirements: the lawyer needs to trust the answers, verify sources quickly, understand the system's limitations, and give feedback to improve quality. A well-designed legal AI UX reduces the friction between the agent's answer and the professional's action.

---

## 2. Technical Decisions

### 2.1 Response streaming

| Alternative | Pros | Cons | Decision |
|---|---|---|---|
| **Request-Response (batch)** | Simple. Stateless. Easy to cache. | The user waits 5-15s with no feedback. Feels "hung". | Discarded for chat |
| **Server-Sent Events (SSE)** | Unidirectional streaming. Native HTTP. Automatic reconnection. Works with proxies/CDN. Simple to implement in .NET. | Server→client only. Not bidirectional. | **Chosen** |
| **WebSocket** | Bidirectional. Low latency. | More complex. Issues with proxies. Requires connection management. Unnecessary for chat (the user does not send while the agent answers). | For notifications (SignalR), not for chat |
| **Long Polling** | Compatible with everything. Simple. | Inefficient. High latency. Not true streaming. | Discarded |

**Decision:** SSE for streaming agent answers. SignalR (WebSocket) for push notifications (deadline alerts, new norms).

### 2.2 Stream format

> Event `type` values are code enums (English). The `step` text shown to the user is in Spanish (end-user facing).

```typescript
// SSE events from the /api/chat/stream endpoint
interface ChatStreamEvent {
  type: 'thinking' | 'tool_call' | 'content' | 'citation' | 'done' | 'error';
  data: unknown;
}

// Event examples:
// Agent thinking (shows an indicator to the user)
{ type: 'thinking', data: { step: 'Buscando normas relevantes...' } }

// Agent invoked a tool
{ type: 'tool_call', data: { tool: 'SearchLegalNorms', query: 'art 245 LCT' } }

// Response token (text streaming)
{ type: 'content', data: { text: 'Según el artículo 245 de la ' } }

// Detected citation
{ type: 'citation', data: { ref: 'Ley 20.744, Art. 245', url: '/legal-norms/20744/245', type: 'norm' } }

// Complete response
{ type: 'done', data: { tokens: { input: 2340, output: 856 }, latencyMs: 3200 } }
```

---

## 3. Inline Citation with Links to Sources

### 3.1 Citation format

Citations appear as numbered badges in the answer text, with a popover on hover and a direct link to the source. The rendered text is shown to the user in Spanish:

```
La indemnización por despido sin justa causa se calcula como un mes de sueldo
por cada año de antigüedad [1], tomando como base la mejor remuneración mensual,
normal y habitual [1]. La Corte Suprema estableció que el tope no puede reducir
la indemnización a menos del 67% del salario [2].

───────
Fuentes:
[1] Ley 20.744, Art. 245 — Contrato de Trabajo → /legal-norms/20744/articles/245
[2] CSJN, "Vizzoti c/ AMSA", 14/09/2004 → /case-law/12345
```

### 3.2 Angular citation component

```typescript
// Structure of the citation component
interface Citation {
  index: number;           // [1], [2], etc.
  type: 'norm' | 'caseLaw' | 'doctrine' | 'caseFile';
  reference: string;       // "Ley 20.744, Art. 245"
  url: string;             // internal app route
  snippet: string;         // excerpt of the source text (for the popover)
  confidence: number;      // 0-1, relevance score
  verified: boolean;       // whether it passed the citation check
}
```

---

## 4. Visible Confidence Score

### 4.1 Confidence levels

| Level | Visual indicator | Meaning | When it is shown |
|---|---|---|---|
| **High** (≥ 0.85) | Green badge | Answer based on found and verified sources | Norm in force found, citations verified |
| **Medium** (0.60-0.84) | Yellow badge | Answer based on partial or potentially outdated sources | Few sources found, or a possible recent update |
| **Low** (< 0.60) | Red badge + warning | Low-confidence answer, requires human verification | Little evidence in the KB, topic out of coverage |
| **No sources** | Warning banner | No sources found in the KB | Query out of scope or incomplete KB |

### 4.2 Confidence score calculation

```
confidence = weighted_average(
    0.35 * retrieval_score,       // Score of the best retrieved document
    0.25 * citation_accuracy,     // % of citations verified as correct
    0.20 * context_coverage,      // % of the answer supported by context
    0.10 * source_recency,        // How recent the sources are
    0.10 * source_count           // Number of concordant sources
)
```

---

## 5. Answer Explainability

### 5.1 "How I reached this answer" panel

An expandable section that shows the agent's reasoning. The content is shown to the user in Spanish:

```
▼ Cómo llegué a esta respuesta

1. Entendí tu consulta como: "indemnización por despido sin causa, 8 años de antigüedad"
2. Busqué en la base de normas → encontré 12 resultados relevantes
3. Busqué en jurisprudencia → encontré 8 fallos relevantes
4. Consulté el grafo legal → encontré que el Art. 245 fue modificado por Ley 25.877
5. Seleccioné las 5 fuentes más relevantes para responder
6. Verifiqué que todas las citas existen en la base de conocimiento ✓

Tiempo de procesamiento: 3.2s | Fuentes consultadas: 20 | Citadas: 5
```

### 5.2 Implementation

The explainability panel is built from the `thinking` and `tool_call` events of the SSE stream. It does not require an additional LLM call; it only formats the steps the agent already executed.

---

## 6. Follow-up Suggestions

### 6.1 Suggestion generation

At the end of each answer, the agent suggests 2-3 relevant follow-up questions. They are shown to the user in Spanish:

```
───────
Podés preguntarme:
→ ¿Cómo se calcula el tope indemnizatorio?
→ ¿Qué pasa si el empleador no paga la indemnización?
→ ¿Cuáles son los plazos para reclamar judicialmente?
```

| Alternative | Pros | Cons | Decision |
|---|---|---|---|
| **LLM generates suggestions** | Contextual. Relevant. Natural. | Extra cost (~200 tokens). May suggest things the KB does not cover. | **Chosen** |
| **Predefined suggestions** | No cost. Controlled. | Not contextual. May be irrelevant. | Discarded |
| **History-based suggestions** | Personalized to the user. | Requires usage data. Cold for new users. | Future complement |

### 6.2 Generation prompt

> Agent prompt content, kept in Spanish.

```yaml
# Appended to the end of the agent prompt
follow_up_instruction: |
  Al final de tu respuesta, sugiere exactamente 3 preguntas de follow-up que
  el usuario podría querer hacer basándose en el tema tratado. Las preguntas
  deben ser:
  - Relevantes al contexto de la conversación
  - Cubiertas por la base de conocimiento (normas, jurisprudencia, o procesal)
  - Formuladas como pregunta directa en segunda persona
  Formato: una línea por sugerencia, precedida por "→ "
```

---

## 7. Feedback Loop

### 7.1 Feedback UI

The UI labels are end-user facing, so they are shown in Spanish:

```
┌─────────────────────────────────────────────┐
│  [Respuesta del agente...]                  │
│                                             │
│  ───────                                    │
│  ¿Te resultó útil?  [👍] [👎]              │
│                                             │
│  [Si 👎] ¿Qué estuvo mal?                  │
│  ○ Información incorrecta                   │
│  ○ Respuesta incompleta                     │
│  ○ Norma derogada / desactualizada          │
│  ○ Cita inventada                           │
│  ○ No entendió mi pregunta                  │
│  ○ Otro: [________________]                 │
│                                             │
│  [Enviar feedback]                          │
└─────────────────────────────────────────────┘
```

### 7.2 How feedback is used

| Negative feedback volume | Action |
|---|---|
| < 5% | Normal. Passive monitoring. |
| 5-15% | Investigate: is it a specific topic? an agent? Review prompts. |
| 15-25% | Alert the tech lead. Thorough analysis. Possible rollback of a recent change. |
| > 25% | Critical alert. Consider disabling the affected agent. Immediate investigation. |

---

## 8. Progressive Disclosure of Context

### 8.1 Detail levels

The agent's answer is presented in layers the user can expand:

| Level | What is shown | Default |
|---|---|---|
| **Answer** | Main text with inline citations [1][2] | Visible |
| **Sources** | List of sources with links | Visible (collapsed) |
| **Reasoning** | "How I reached this answer" panel | Collapsed |
| **Source text** | Full text of each cited source | Click to expand |
| **Technical metadata** | Tokens, latency, model, prompt version | Dev mode only |

---

## 9. Items Pending Definition

- [ ] Implement the SSE endpoint for response streaming
- [ ] Design the Angular citation component with a popover
- [ ] Implement confidence score calculation in the backend
- [ ] Design the confidence score UI (badges, colors, tooltips)
- [ ] Implement the explainability panel (expandable)
- [ ] Implement follow-up suggestion generation
- [ ] Design the feedback component (thumbs + reason)
- [ ] Define complete wireframes of the chat experience
- [ ] Integrate progressive disclosure into the answer component
- [ ] Define accessibility: screen reader for citations, keyboard navigation

---

## 10. References

- [Server-Sent Events — MDN](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)
- [Azure SignalR Service](https://learn.microsoft.com/en-us/azure/azure-signalr/)
- [Angular — EventSource](https://angular.io/guide/http-stream-response)
- [Nielsen Norman Group — AI UX Guidelines](https://www.nngroup.com/articles/ai-ux-getting-started/)

---

*08 — Legal AI UX — Legal Ai Ar*
