# Work Item — Output Guardrail

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Pipeline](./FEATURE-chat-pipeline.md)

## User Story

As a **legal professional using the chat assistant**, I want **the assistant's responses to be validated for factual accuracy and completeness before I see them** so that **I can trust that cited rulings actually exist, case titles match their IDs, and no fabricated legal information reaches me**.

## Context

- **Current state**: GPT-4o generates the final response with inline citations (`{caso: "...", id: "..."}`) and the frontend renders them as links. There is **no validation** that the cited ruling IDs exist in the database, that case titles match the actual records, or that mentioned statutes were actually returned by tools. The model can hallucinate case titles, mix up IDs, or reference statutes it was not provided via tools. For a legal-domain application, unverified citations are a significant liability.
- **Target state**: A post-execution validation stage that accumulates the streamed response, parses citations, validates them against the database, and flags or corrects discrepancies before the response is finalized.
- **Streaming compatibility**: This stage must work with SSE streaming. Two strategies are possible:
  1. **Buffer-then-stream**: Accumulate the full response, validate, then stream. Adds latency but is simpler.
  2. **Stream-then-append**: Stream text in real-time, then append validation results (corrections/warnings) as a final block. Preserves perceived responsiveness.
- **Latency budget**: 100-200ms for validation after response completion (DB lookups are fast for a small set of IDs).

## Acceptance Criteria

1. **Citation extraction**: Parse all citations from the accumulated response text using the `{caso: "...", id: "..."}` pattern (same regex the frontend uses).
2. **ID existence validation**: For each extracted ruling ID, verify it exists in the rulings database via a batch query (`IRulingRepository` or equivalent). IDs not found are flagged.
3. **Title consistency check**: For validated IDs, compare the `caseTitle` in the response against the actual `caseTitle` in the database. Flag mismatches (fuzzy matching with a threshold to tolerate minor formatting differences).
4. **Tool-grounding check**: Verify that every ruling ID cited in the response was actually returned by a tool call during the agentic loop execution. Citations referencing IDs that were never retrieved by any tool are flagged as potential hallucinations.
5. **Statute grounding** (best-effort): If the response references specific laws or articles, verify they appeared in tool results. Flag ungrounded normative references.
6. **Validation result handling**:
   - **All citations valid**: Response passes through unmodified.
   - **Minor issues** (title mismatch): Append a subtle correction note or silently correct the title in the stream.
   - **Serious issues** (non-existent ID, ungrounded citation): Append a warning to the response: "Nota: Algunas referencias en esta respuesta no pudieron ser verificadas. Recomendamos verificar los fallos citados directamente."
   - **All citations invalid**: Append a strong disclaimer and log at `Warning` level.
7. **Disclaimer enforcement**: If the response contains substantive legal analysis (not just greetings or clarifications), ensure it ends with (or includes) a standard disclaimer that the information is for reference purposes and does not constitute legal advice.
8. **SSE integration**: Validation events are emitted as a new SSE event type `event: validation\ndata: {"status":"passed"|"warnings","details":[...]}\n\n` after the last text chunk and before `[DONE]`. The frontend can render validation status in the message UI.
9. **Observability**: Log validation results (total citations checked, valid count, flagged count, latency) at `Information` level. Log specific validation failures at `Warning` level.
10. **Configuration**: Enabled/disabled via `ChatPipeline:OutputGuardrail`. Strictness level configurable: `lenient` (log only), `moderate` (append warnings), `strict` (block responses with ungrounded citations and request re-generation).

## Out of Scope

- Automated response regeneration on validation failure (deferred — v1 appends warnings)
- Semantic accuracy of the model's legal analysis (only factual grounding is checked)
- Citation completeness (whether the model cited all relevant rulings from tool results)
- Frontend rendering of validation events (separate UX work item if needed)

## Technical Notes

- **ToolExecutionContext tracking**: The existing `ToolExecutionContext.ResolvedRulingIds` already tracks ruling IDs fetched during tool execution. This set is the ground truth for the tool-grounding check. Ensure all tool handlers populate it consistently.
- **Batch validation query**: A single `SELECT Id, CaseTitle FROM Rulings WHERE Id IN (@ids)` is efficient for the typical 3-10 citations per response. No N+1 concern.
- **Citation regex**: Reuse or align with the frontend's `CITATION_REGEX` pattern to ensure consistent parsing. Consider extracting it to a shared constant or configuration.
- **Streaming strategy recommendation**: Use **stream-then-append** for v1. The user sees the response in real-time (no perceived latency increase). Validation results arrive as a final SSE event (~100ms after last text chunk). This is the simplest to implement without disrupting the existing streaming pipeline.
- **State propagation**: The output guardrail needs access to:
  1. The accumulated response text (capture from `TextChunkEvent` yields)
  2. The `ToolExecutionContext.ResolvedRulingIds` set (pass through pipeline)
  3. The tool results (or at minimum, the ruling IDs returned by each tool)
