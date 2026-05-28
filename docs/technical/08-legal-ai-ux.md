# 08 — UX de IA Legal

> **Proyecto:** Legal Ai Ar | **Categoría:** AI User Experience
> **Estado:** No definido — todos los ítems son nuevos
> **Última actualización:** Mayo 2026

---

## 1. Descripción

La experiencia de usuario con IA en un contexto legal tiene requerimientos específicos: el abogado necesita confiar en las respuestas, verificar las fuentes rápidamente, entender las limitaciones del sistema, y dar feedback para mejorar la calidad. Una UX de IA legal bien diseñada reduce la fricción entre la respuesta del agente y la acción del profesional.

---

## 2. Decisiones Técnicas

### 2.1 Streaming de respuestas

| Alternativa | Pros | Contras | Decisión |
|---|---|---|---|
| **Request-Response (batch)** | Simple. Sin estado. Fácil de cachear. | El usuario espera 5-15s sin feedback. Sensación de "colgado". | Descartado para chat |
| **Server-Sent Events (SSE)** | Streaming unidireccional. Nativo HTTP. Reconexión automática. Funciona con proxies/CDN. Simple de implementar en .NET. | Solo server→client. No bidireccional. | **Elegido** |
| **WebSocket** | Bidireccional. Baja latencia. | Más complejo. Problemas con proxies. Requiere gestión de conexión. Innecesario para chat (el usuario no envía mientras el agente responde). | Para notificaciones (SignalR), no para chat |
| **Long Polling** | Compatible con todo. Simple. | Ineficiente. Alta latencia. No es streaming real. | Descartado |

**Decisión:** SSE para streaming de respuestas de agentes. SignalR (WebSocket) para notificaciones push (alertas de plazos, nuevas normas).

### 2.2 Formato de stream

```typescript
// Eventos SSE del endpoint /api/chat/stream
interface ChatStreamEvent {
  type: 'thinking' | 'tool_call' | 'content' | 'citation' | 'done' | 'error';
  data: unknown;
}

// Ejemplos de eventos:
// Agente pensando (muestra indicador al usuario)
{ type: 'thinking', data: { step: 'Buscando normas relevantes...' } }

// Agente invocó una herramienta
{ type: 'tool_call', data: { tool: 'SearchNormas', query: 'art 245 LCT' } }

// Token de respuesta (streaming de texto)
{ type: 'content', data: { text: 'Según el artículo 245 de la ' } }

// Cita detectada
{ type: 'citation', data: { ref: 'Ley 20.744, Art. 245', url: '/normas/20744/245', type: 'norma' } }

// Respuesta completa
{ type: 'done', data: { tokens: { input: 2340, output: 856 }, latency_ms: 3200 } }
```

---

## 3. Citación Inline con Links a Fuentes

### 3.1 Formato de citación

Las citas aparecen como badges numerados en el texto de la respuesta, con popover al hacer hover y link directo a la fuente:

```
La indemnización por despido sin justa causa se calcula como un mes de sueldo 
por cada año de antigüedad [1], tomando como base la mejor remuneración mensual,
normal y habitual [1]. La Corte Suprema estableció que el tope no puede reducir 
la indemnización a menos del 67% del salario [2].

───────
Fuentes:
[1] Ley 20.744, Art. 245 — Contrato de Trabajo → /normas/20744/articulos/245
[2] CSJN, "Vizzoti c/ AMSA", 14/09/2004 → /jurisprudencia/12345
```

### 3.2 Componente Angular de citación

```typescript
// Estructura del componente de citación
interface Citation {
  index: number;           // [1], [2], etc.
  type: 'norma' | 'jurisprudencia' | 'doctrina' | 'expediente';
  reference: string;       // "Ley 20.744, Art. 245"
  url: string;             // ruta interna en la app
  snippet: string;         // extracto del texto fuente (para popover)
  confidence: number;      // 0-1, score de relevancia
  verified: boolean;       // si pasó el check de citación
}
```

---

## 4. Confidence Score Visible

### 4.1 Niveles de confianza

| Nivel | Indicador visual | Significado | Cuándo se muestra |
|---|---|---|---|
| **Alta** (≥ 0.85) | Badge verde | Respuesta basada en fuentes encontradas y verificadas | Norma vigente encontrada, citas verificadas |
| **Media** (0.60-0.84) | Badge amarillo | Respuesta basada en fuentes parciales o potencialmente desactualizadas | Pocas fuentes encontradas, o posible actualización reciente |
| **Baja** (< 0.60) | Badge rojo + warning | Respuesta con baja confianza, requiere verificación humana | Poca evidencia en la KB, tema fuera de cobertura |
| **Sin fuentes** | Banner de warning | No se encontraron fuentes en la KB | Query fuera de scope o KB incompleta |

### 4.2 Cálculo del confidence score

```
confidence = weighted_average(
    0.35 * retrieval_score,       // Score del mejor documento recuperado
    0.25 * citation_accuracy,     // % de citas verificadas como correctas
    0.20 * context_coverage,      // % de la respuesta soportada por contexto
    0.10 * source_recency,        // Qué tan recientes son las fuentes
    0.10 * source_count           // Cantidad de fuentes concordantes
)
```

---

## 5. Explicabilidad de Respuestas

### 5.1 Panel "Cómo llegué a esta respuesta"

Sección expandible que muestra el razonamiento del agente:

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

### 5.2 Implementación

El panel de explicabilidad se construye con los eventos `thinking` y `tool_call` del stream SSE. No requiere una llamada adicional al LLM; solo se formatean los pasos que el agente ya ejecutó.

---

## 6. Sugerencias de Follow-up

### 6.1 Generación de sugerencias

Al final de cada respuesta, el agente sugiere 2-3 preguntas de follow-up relevantes:

```
───────
Podés preguntarme:
→ ¿Cómo se calcula el tope indemnizatorio?
→ ¿Qué pasa si el empleador no paga la indemnización?
→ ¿Cuáles son los plazos para reclamar judicialmente?
```

| Alternativa | Pros | Contras | Decisión |
|---|---|---|---|
| **LLM genera sugerencias** | Contextuales. Relevantes. Naturales. | Costo extra (~200 tokens). Puede sugerir cosas que la KB no cubre. | **Elegido** |
| **Sugerencias predefinidas** | Sin costo. Controladas. | No son contextuales. Pueden ser irrelevantes. | Descartado |
| **Sugerencias basadas en historial** | Personalizadas al usuario. | Requiere datos de uso. Frío en usuarios nuevos. | Complemento futuro |

### 6.2 Prompt de generación

```yaml
# Agregado al final del prompt del agente
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

### 7.1 UI de feedback

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

### 7.2 Cómo se usa el feedback

| Volumen de feedback negativo | Acción |
|---|---|
| < 5% | Normal. Monitoreo pasivo. |
| 5-15% | Investigar: ¿es un tema específico? ¿un agente? Revisar prompts. |
| 15-25% | Alerta al tech lead. Análisis exhaustivo. Posible rollback de cambio reciente. |
| > 25% | Alerta crítica. Considerar desactivar agente afectado. Investigación inmediata. |

---

## 8. Progressive Disclosure de Contexto

### 8.1 Niveles de detalle

La respuesta del agente se presenta en capas que el usuario puede expandir:

| Nivel | Qué se muestra | Default |
|---|---|---|
| **Respuesta** | Texto principal con citas inline [1][2] | Visible |
| **Fuentes** | Lista de fuentes con links | Visible (colapsada) |
| **Razonamiento** | Panel "Cómo llegué a esta respuesta" | Colapsado |
| **Texto fuente** | Texto completo de cada fuente citada | Click para expandir |
| **Metadata técnica** | Tokens, latencia, modelo, versión prompt | Solo en modo dev |

---

## 9. Ítems Pendientes de Definición

- [ ] Implementar endpoint SSE para streaming de respuestas
- [ ] Diseñar componente Angular de citación con popover
- [ ] Implementar cálculo de confidence score en el backend
- [ ] Diseñar UI de confidence score (badges, colores, tooltips)
- [ ] Implementar panel de explicabilidad (expandible)
- [ ] Implementar generación de sugerencias de follow-up
- [ ] Diseñar componente de feedback (thumbs + motivo)
- [ ] Definir wireframes completos de la experiencia de chat
- [ ] Integrar progressive disclosure en el componente de respuesta
- [ ] Definir accesibilidad: screen reader para citaciones, keyboard navigation

---

## 10. Referencias

- [Server-Sent Events — MDN](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)
- [Azure SignalR Service](https://learn.microsoft.com/en-us/azure/azure-signalr/)
- [Angular — EventSource](https://angular.io/guide/http-stream-response)
- [Nielsen Norman Group — AI UX Guidelines](https://www.nngroup.com/articles/ai-ux-getting-started/)

---

*08 — UX de IA Legal — Legal Ai Ar*
