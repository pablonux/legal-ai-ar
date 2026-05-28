# Legal RAG Prompt — Chat Endpoint

| Field | Value |
|---|---|
| **ID** | E078 |
| **Feature** | F1-7 · API — Chat RAG |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the complete legal RAG prompt used by the Chat endpoint: system prompt, citation instructions, citation format, context structure, and handling of edge cases (no results, partial response). It serves as the design reference for T-03 and is consumed by developers implementing `ChatQueryHandler` and the GPT-4o integration.

**Reference**: E076 (Chat RAG pipeline); E077 (Chat flow); Architecture section 5.

---

## 1. System Prompt

```
You are a legal assistant specialized in Argentine jurisprudence. You answer questions about Argentine court rulings (fallos) using only the rulings and excerpts provided in the context below. Your responses must be in Spanish unless the user asks in another language.

Rules:
- Base your answers exclusively on the context provided. Do not invent or assume information not present in the context.
- When referencing a ruling, you MUST cite it using the exact format: {caso: "Case Title", id: "ruling-uuid"}
- Use the exact caseTitle and id from the context. Do not paraphrase case titles in citations.
- If the context does not contain relevant information to answer the question, say so clearly and do not speculate.
- Write in a professional, clear style appropriate for legal professionals.
- Keep responses concise. Prioritize accuracy over length.
```

---

## 2. Citation Instructions

### 2.1 Format

Every ruling referenced in the response must be cited using:

```
{caso: "caseTitle", id: "rulingId"}
```

| Field | Source | Example |
|---|---|---|
| `caseTitle` | Exact string from context (Case: ... \| ID: ...) | `"Smith v. Jones"` |
| `rulingId` | UUID from context (lowercase, with hyphens) | `"a1b2c3d4-e5f6-7890-abcd-ef1234567890"` |

### 2.2 Rules

| Rule | Detail |
|---|---|
| **Every referenced ruling** | Must include at least one citation with both `caso` and `id`. |
| **Exact match** | Use the exact `caseTitle` and `rulingId` from the context. Do not abbreviate or modify. |
| **Inline placement** | Place the citation immediately after the first mention of the ruling in the sentence. |
| **Multiple citations** | Each ruling can be cited multiple times; ensure at least one full citation per ruling. |

### 2.3 Examples (Spanish)

**Single citation:**
> En el fallo {caso: "Pérez, Juan c/ Estado Nacional", id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"} la Corte Suprema sostuvo que el principio de igualdad...

**Multiple citations:**
> Tanto en {caso: "Fallos: 328:1883", id: "b2c3d4e5-f6a7-8901-bcde-f12345678901"} como en {caso: "Doe c/ Ministerio Público", id: "c3d4e5f6-a7b8-9012-cdef-123456789012"} se estableció que...

**Citation at end of sentence:**
> La jurisprudencia ha sido constante en este punto {caso: "Smith v. Jones", id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"}.

---

## 3. Message Structure

### 3.1 Messages Sent to GPT-4o

| Role | Content |
|---|---|
| **system** | System prompt (section 1). |
| **user** | Concatenation of: (1) context block, (2) user query. |

### 3.2 User Message Template

```
## Contexto — Fallos y fragmentos relevantes

{context}

---

## Pregunta del usuario

{userQuery}
```

- `{context}`: Assembled per E076 section 3.3 (chunk excerpts + ruling metadata blocks).
- `{userQuery}`: The user's question (validated, max 1000 chars).

---

## 4. Edge Cases

### 4.1 No Relevant Results (Handler-Level)

**When**: Dual search returns no chunks and no rulings.

**Behavior**: The handler does **not** call GPT-4o. It returns a fixed message to the client:

> No se encontró jurisprudencia relevante para tu consulta. Intenta reformular la pregunta o usar términos más específicos.

**Reference**: E076 section 6.

### 4.2 Partial Response (Stream Interrupted)

**When**: Client disconnects, network error, or token limit reached mid-response.

**Behavior**:
- **Client disconnect**: Handler cancels `CancellationToken`. No retry. Client receives partial stream.
- **Token limit**: Model stops at max response tokens (2,048). Response may be truncated.

**Prompt instruction** (included in system prompt):
> If you approach the response length limit, prioritize completing any citation you have started. Do not leave citations incomplete (e.g. `{caso: "..."` without `id`). Prefer ending the response with a complete sentence and citation.

### 4.3 Context Insufficient for Answer

**When**: Context contains rulings but none directly address the user's question.

**Behavior**: Model should respond with something like:

> La jurisprudencia proporcionada no contiene información específica sobre [topic]. Los fallos incluidos abordan [brief summary of what they do address]. Te sugiero reformular la pregunta o ampliar los criterios de búsqueda.

The model must **not** invent rulings or conclusions. It may cite rulings from the context only to explain why they are not directly relevant.

### 4.4 Ambiguous or Off-Topic Query

**When**: User asks something outside jurisprudence (e.g. weather, math) or the query is too vague.

**Behavior**: Model should politely redirect:

> Soy un asistente especializado en jurisprudencia argentina. Puedo ayudarte con preguntas sobre fallos, doctrina judicial y derecho argentino. ¿Podrías reformular tu pregunta en ese ámbito?

---

## 5. Full Prompt Example

### 5.1 System Message

(As in section 1.)

### 5.2 User Message (Example)

```
## Contexto — Fallos y fragmentos relevantes

[Ruling: Pérez, Juan c/ Estado Nacional (a1b2c3d4-e5f6-7890-abcd-ef1234567890)]
El demandante alega que la norma aplicada viola el principio de igualdad ante la ley consagrado en el art. 16 de la Constitución Nacional. La Corte ha señalado en reiterada jurisprudencia que...

[Ruling: Doe c/ Ministerio Público (b2c3d4e5-f6a7-8901-bcde-f12345678901)]
Por ello, se confirma la sentencia de grado en los términos del considerando 8.

## Ruling metadata

Case: Pérez, Juan c/ Estado Nacional | ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890 | Date: 2024-03-15 | Civil / Primera Instancia
Summary: Recurso de inconstitucionalidad. El actor cuestiona la ley X por violar el principio de igualdad.
Holding: La Corte confirma que el principio de igualdad exige un tratamiento diferenciado cuando existen situaciones fácticas distintas.

Case: Doe c/ Ministerio Público | ID: b2c3d4e5-f6a7-8901-bcde-f12345678901 | Date: 2024-01-10 | Penal / Cámara
Summary: Casación. Recurso del Ministerio Público.
Holding: Se confirma la sentencia apelada.

---

## Pregunta del usuario

¿Qué dice la jurisprudencia de la Corte sobre el principio de igualdad?
```

### 5.3 Expected Response (Excerpt)

> La Corte Suprema ha desarrollado el principio de igualdad en numerosos fallos. En {caso: "Pérez, Juan c/ Estado Nacional", id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"} sostuvo que el principio de igualdad ante la ley (art. 16 CN) exige un tratamiento diferenciado cuando existen situaciones fácticas distintas. En ese fallo se señaló que la jurisprudencia ha sido constante en exigir...

---

## 6. References

- `docs/design/f1-7-chat-rag.md` — pipeline design, context format, citation format (E076)
- `docs/design/f1-7-chat-flow.mermaid` — flow sequence (E077)
- `docs/architecture/legal-ai-ar-architecture.md` — section 5 (Chat RAG)
