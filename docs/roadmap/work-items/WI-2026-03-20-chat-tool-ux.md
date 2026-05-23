# Work Item — Frontend: Tool Execution Feedback UX

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Tools](./FEATURE-chat-tools.md)

## User Story

As a **user**, I want **to see visual feedback when the assistant is searching, loading ruling details, or executing other tools** so that **I understand what's happening during longer responses and don't think the assistant is stuck**.

## Context

- **Current state**: The frontend `ChatService` reads SSE `data:` events and renders text chunks in real time. There is no concept of intermediate states — the response is either streaming text or done.
- **Target state**: The SSE stream includes typed events for tool execution phases. The frontend renders inline status indicators (e.g. "Buscando fallos...", "Consultando citas...") that transition to the tool's results as the assistant continues generating text.
- **SSE protocol extension**:
  - `event: tool_start\ndata: {"tool":"search_rulings","args":{...}}\n\n` — tool invocation begins
  - `event: tool_end\ndata: {"tool":"search_rulings","resultCount":5}\n\n` — tool execution completed
  - `data: {text}\n\n` — text chunks (unchanged)
  - `data: [DONE]\n\n` — stream ends (unchanged)

## Acceptance Criteria

1. **SSE parsing**: `ChatService` (Angular) parses `event:` field in SSE stream. Events without explicit type default to text chunks (backward compatible).
2. **Tool status component**: A lightweight inline component (e.g. `ToolStatusChipComponent`) that shows tool name with a spinner during execution and a checkmark + result summary on completion.
3. **Status mapping**: Friendly Spanish labels for tool names:
   - `search_rulings` → "Buscando jurisprudencia..."
   - `get_ruling_detail` → "Consultando detalle del fallo..."
   - `get_ruling_citations` → "Navegando citas..."
   - `get_related_rulings` → "Buscando fallos similares..."
   - `search_by_statute` → "Buscando por normativa..."
   - `count_rulings` → "Contando fallos..."
   - `list_courts` → "Consultando tribunales..."
   - `list_judges` → "Consultando jueces..."
4. **Inline rendering**: Tool status chips appear inline within the chat message flow, between text blocks. Multiple tools in sequence show multiple chips.
5. **Error state**: If a tool fails, the chip shows a warning icon. The assistant's text response handles the explanation.
6. **Backward compatible**: If the backend does not send `event:` fields (e.g. older version), the frontend works exactly as before.
7. **Responsive**: Chips are compact and work on mobile viewports.

## Out of Scope

- Showing raw tool parameters or full result payloads to the user
- User ability to cancel individual tool executions
- Tool execution history panel
