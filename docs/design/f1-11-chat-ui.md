# Chat RAG — UX Specification

| Field | Value |
|---|---|
| **ID** | E102 |
| **Feature** | F1-11 · Frontend — Chat RAG |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the UX of the jurisprudential chat: layout, message states (streaming, complete, error), citation rendering format, and SSE reconnection handling. It serves as the design reference for T-01 to T-05 and is consumed by Angular developers implementing the chat flow.

**Reference**: Architecture section 6; E076 (Chat RAG pipeline); E078 (RAG prompt, citation format); specs section 8.

---

## 1. Route and Layout

| Route | Component | Description |
|---|---|---|
| `/chat` | ChatViewComponent | RAG jurisprudential chat |

### 1.1 Layout

- **Header**: Title "Chat Jurisprudencial" (or similar)
- **Message list**: Scrollable area. User messages on right (or left); assistant messages on left (or right). Chronological order.
- **Input area**: Text input + Send button. At bottom. Disabled while assistant is streaming.
- **Scroll**: Auto-scroll to latest message when new content arrives (streaming or new message).

---

## 2. Message States

### 2.1 User Message

| State | UI |
|---|---|
| **sent** | Bubble with user text. Timestamp optional. |

### 2.2 Assistant Message

| State | UI |
|---|---|
| **streaming** | Bubble with partial text. Typing indicator (e.g. blinking cursor) at end. Citations render as links as they stream in (when pattern complete). |
| **complete** | Bubble with full text. All citations as links. No cursor. |
| **error** | Error message: "Error al generar la respuesta. Puedes reintentar." Retry button (resends same query). |

### 2.3 Empty / No Results

When API returns "no relevant jurisprudence" (no GPT call):

- Show assistant message: "No se encontró jurisprudencia relevante para tu consulta. Intenta reformular la pregunta o usar términos más específicos."
- State: **complete** (not error).

---

## 3. Citation Rendering

### 3.1 Format to Parse

The API streams plain text. Citations appear as:

```
{caso: "Case Title", id: "ruling-uuid"}
```

**Regex** (example): `/\{caso:\s*"([^"]+)"\s*,\s*id:\s*"([a-f0-9-]+)"\}/g`

**Alternative** (if caseTitle has no quotes): `/\{caso:\s*([^,]+)\s*,\s*id:\s*"([a-f0-9-]+)"\}/g` — less robust if caseTitle contains commas.

**Recommended**: Use quoted caseTitle. Parse `{caso: "X", id: "Y"}` and replace with link.

### 3.2 Render as Link

Replace each match with:

- **HTML**: `<a routerLink="/fallos/{id}">{caseTitle}</a>`
- **Angular**: Use `RouterLink` directive or `[routerLink]="['/fallos', id]"`. Display text: `caseTitle`.
- **Styling**: Underline, distinct color (e.g. primary). Optional icon (e.g. document).

### 3.3 Incomplete Citations

If stream ends mid-citation (e.g. `{caso: "Smith`), render as plain text. Do not create broken links.

---

## 4. SSE Reconnection

### 4.1 Stream Interruption

When the stream is interrupted (network error, timeout, server disconnect):

1. **Stop** appending to the current message.
2. **Show** partial content received so far (with citations parsed).
3. **Add** error state: "La conexión se interrumpió. Puedes reintentar."
4. **Offer** "Reintentar" button. On click: resend the same user query (new request).

### 4.2 Retry Behavior

- **User-initiated**: "Reintentar" sends `POST /api/chat` again with the same query.
- **No auto-retry**: Do not automatically resend. User must click retry.
- **Idempotency**: Each retry is a new request. API has no session; no "resume stream."

### 4.3 Server Error (5xx)

If the API returns 500 before streaming starts:

- Show error in place of assistant message.
- "Error del servidor. Intenta más tarde." + Retry button.

If error occurs mid-stream (rare; server may close connection):

- Same as 4.1: show partial + error + Retry.

---

## 5. Input Validation

| Validation | Behavior |
|---|---|
| Empty query | Disable Send. Or show inline error on submit. |
| Query too long | Truncate to 1000 chars (match API). Show warning. |
| Rate limit | If API returns 429: "Demasiadas solicitudes. Espera un momento." Disable input briefly. |

---

## 6. Accessibility

- **Focus**: After send, focus remains in input (or move to new message).
- **Screen readers**: Announce "Mensaje enviado" and "Respuesta recibida" (or streaming progress).
- **Keyboard**: Enter to send (Shift+Enter for newline if supported).

---

## 7. References

- `docs/design/f1-7-chat-rag.md` — pipeline, citation format (E076)
- `docs/design/f1-7-rag-prompt.md` — citation format (E078)
- `docs/design/f1-11-sse-flow.mermaid` — SSE sequence (E103)
