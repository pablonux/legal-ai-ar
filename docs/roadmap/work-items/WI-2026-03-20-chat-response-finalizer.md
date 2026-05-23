# Work Item — Response Finalizer

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Pipeline](./FEATURE-chat-pipeline.md)

## User Story

As a **legal professional using the chat assistant**, I want **the assistant's responses to have consistent formatting, normalized citations, and appropriate legal disclaimers** so that **every response maintains professional quality regardless of how the model chose to format its output**.

## Context

- **Current state**: Response formatting depends entirely on GPT-4o's adherence to the system prompt. Citation format (`{caso: "...", id: "..."}`) is sometimes inconsistent (extra spaces, missing quotes, slightly varied patterns). There is no legal disclaimer appended to responses. The frontend parses citations with a regex and renders the rest as plain text — any formatting irregularities propagate directly to the user.
- **Target state**: A deterministic post-processing stage that normalizes the response text before it reaches the client. No LLM calls — purely rule-based string processing.
- **Latency budget**: <50ms. This stage is pure string manipulation.

## Acceptance Criteria

1. **Citation normalization**: Parse all citation patterns in the response and normalize them to the canonical format: `{caso: "Exact Case Title", id: "uuid"}`. Handle common model variations:
   - Extra whitespace: `{ caso: "..." , id: "..." }` → normalized
   - Single quotes: `{caso: '...' , id: '...'}` → double quotes
   - Missing or extra properties: strip unexpected fields, flag missing required fields
   - Broken citations (partial, truncated): attempt repair if possible, flag otherwise
2. **Disclaimer injection**: If the output guardrail determines a disclaimer is needed (substantive legal content), append a standard disclaimer block at the end of the response:
   ```
   ---
   *Esta información es de carácter referencial y no constituye asesoramiento legal. Consultá con un profesional del derecho para tu caso particular.*
   ```
   The disclaimer text is configurable. Injection is conditional (not appended to greetings or error messages).
3. **Markdown cleanup**: Fix common markdown artifacts from model output:
   - Unclosed bold/italic markers
   - Broken list formatting
   - Excessive newlines (>2 consecutive → 2)
   - Proper heading hierarchy (no orphan `#` without text)
4. **Response structure enforcement** (optional, configurable): If enabled, ensure responses with substantive content follow a consistent structure:
   - Summary paragraph
   - Referenced rulings (with citations)
   - Analysis
   - (Disclaimer if applicable)
   
   This is a soft enforcement — the finalizer adds section markers only if the model's output is clearly unstructured.
5. **Empty response handling**: If the agentic executor produces an empty or whitespace-only response (model failure), replace with a standard fallback message: "No pude generar una respuesta. Por favor intentá reformular tu consulta."
6. **SSE integration**: The finalizer processes text chunks. Two modes:
   - **Chunk mode**: Process each `ChatTextChunk` as it flows through (normalize citations inline). Minimal latency impact.
   - **Batch mode**: Accumulate full response, process, then emit. Used when structural enforcement is enabled.
   Default: chunk mode for minimal latency.
7. **Configuration**: 
   - `ChatPipeline:ResponseFinalizer:Enabled` — master toggle
   - `ChatPipeline:ResponseFinalizer:DisclaimerEnabled` — disclaimer injection toggle
   - `ChatPipeline:ResponseFinalizer:DisclaimerText` — customizable disclaimer text
   - `ChatPipeline:ResponseFinalizer:StructureEnforcement` — structural enforcement toggle (default: off)
8. **Observability**: Log finalization actions (citations normalized count, disclaimer injected, markdown fixes applied) at `Debug` level. Aggregated metrics at `Information` level per request.

## Out of Scope

- Content rewriting or paraphrasing (this is string processing, not LLM)
- Translation or multi-language formatting
- PDF/document export formatting
- Frontend-side markdown rendering improvements

## Technical Notes

- **Citation regex**: Must align with the frontend's `CITATION_REGEX` and the output guardrail's parser. Consider extracting to a shared utility class `CitationParser` with methods: `Parse(text) → Citation[]`, `Normalize(text) → string`, `Validate(citation, knownIds) → bool`.
- **Implementation**: A simple `IResponseFinalizer` interface with a `string Process(string responseText, FinalizerContext context)` method. `FinalizerContext` carries flags from the output guardrail (needs disclaimer, validation warnings to append, etc.).
- **Chunk-mode processing**: For SSE chunk mode, citation normalization can only work when a complete citation pattern is detected within the accumulated buffer. Maintain a small look-ahead buffer (~200 chars) to handle citations that span chunk boundaries.
- **Performance**: All operations are string-based. For typical response sizes (500-2000 chars), processing time is <5ms. The 50ms budget is conservative.
