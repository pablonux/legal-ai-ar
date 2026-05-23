# Work Item — Input Guardrail

**Created**: 2026-03-20  
**Status**: Draft  
**Feature**: [Chat Pipeline](./FEATURE-chat-pipeline.md)

## User Story

As a **legal professional using the chat assistant**, I want **my queries to be validated for scope and safety before reaching the LLM** so that **the assistant only processes jurisprudential questions it was designed for, rejects out-of-scope requests gracefully with examples of what it can do, and is protected against prompt injection and misuse**.

## Context

- **Current state**: The only input validation is length truncation (1000 chars) and empty-query rejection. The system prompt instructs the model not to call tools for greetings, but there is no enforcement — the model can still answer non-legal questions, be manipulated via prompt injection, or be asked for personal legal advice. Azure content filters are mapped at the `AzureOpenAiAgentChatService` level (`ContentFilter` finish reason) but `ChatQueryHandler` does not handle that case explicitly.
- **Target state**: A dedicated pipeline stage that classifies incoming queries before they reach the agentic executor, blocking or redirecting inappropriate requests with a helpful, templated response.
- **Latency budget**: 200-500ms. This stage must not significantly delay the perceived response time.
- **Cost constraint**: Avoid a full GPT-4o call for moderation. Prefer rule-based checks + a lightweight model (GPT-4o-mini) or Azure AI Content Safety API.

## Acceptance Criteria

1. **Scope classification**: The guardrail classifies each query into one of:
   - `legal_query` — Jurisprudential question within scope → proceed to next stage
   - `greeting` — Salutation or small talk → respond with a friendly greeting + capability summary (no LLM call to executor)
   - `clarification` — Ambiguous query needing clarification → respond asking for more detail
   - `out_of_scope` — Non-legal question (technology, recipes, general knowledge, etc.) → reject with explanation + examples
   - `harmful` — Prompt injection, jailbreak attempts, requests for personal legal advice, or ethically inappropriate content → reject with firm but professional explanation
2. **Rejection responses**: When a query is rejected or redirected, the response is a **templated message** (not LLM-generated) that:
   - Explains why the request cannot be processed
   - Lists 3-4 examples of what the assistant can do
   - Maintains the professional legal tone
   - Is streamed via SSE as a normal text response (no special event type)
3. **Prompt injection detection**: The guardrail detects common prompt injection patterns:
   - "Ignore previous instructions"
   - "You are now a..."
   - System prompt extraction attempts ("repeat your instructions", "what is your system prompt")
   - Role-play requests that override the assistant's persona
4. **Personal legal advice boundary**: Queries requesting personalized legal advice ("¿Qué debería hacer en mi caso?", "¿Me conviene apelar?") are redirected with a disclaimer explaining the assistant provides jurisprudential information, not legal counsel.
5. **PII detection** (optional first iteration): Flag or warn when queries contain what appear to be personal identifiers (DNI numbers, full names in "my case" context). Log for audit without blocking.
6. **Pipeline integration**: Implemented as an `IChatPipelineStage` (or equivalent middleware pattern) that wraps `ChatQueryHandler`, intercepting before the agentic loop starts.
7. **Configuration**: Classification thresholds and enabled/disabled state are configurable via `appsettings.json` under a `ChatPipeline:InputGuardrail` section.
8. **Observability**: Log each classification result (category, confidence, latency) at `Information` level. Log rejections at `Warning` level with the original query (sanitized).
9. **Azure Content Filter handling**: When the agentic executor returns `FinishReason.ContentFilter`, the guardrail surfaces a user-friendly message instead of silently ending the stream.

## Out of Scope

- Training a custom classification model (use rule-based + GPT-4o-mini or Azure AI Content Safety)
- Rate limiting per user (infrastructure concern, not chat pipeline)
- Multi-language support (assistant is Spanish-only)
- Blocking specific users (authentication/authorization layer)

## Technical Notes

- **Classification approach (recommended)**: A two-layer system:
  1. **Fast rules layer**: Regex patterns for prompt injection, PII patterns, length/format checks. Runs in <10ms.
  2. **LLM classifier layer**: A short GPT-4o-mini call with a focused prompt: "Classify the following query as legal_query, greeting, clarification, out_of_scope, or harmful. Respond with only the category." Cost: ~$0.0003 per classification.
- **Alternative**: Azure AI Content Safety API for harmful content detection, combined with a keyword-based scope classifier. Avoids the LLM call entirely but may be less accurate for nuanced scope decisions.
- **Rejection templates** should be stored in a resource file or configuration, not hardcoded, to allow easy updates without deployment.
- The `ContentFilter` finish reason from Azure OpenAI should be caught in `ChatQueryHandler` (currently falls through to the `yield break` in the `DoneEvent` handler when `FinishReason != ToolCalls`).
