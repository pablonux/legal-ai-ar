# Chat Pipeline — Multi-Stage Architecture

| Field | Value |
|---|---|
| **ID** | E232 |
| **Feature** | F1-18 · Chat Pipeline — Multi-Stage Architecture |
| **Date** | 2026-03-20 |

---

## Purpose

This document specifies the architecture for wrapping the existing agentic chat executor (F1-17) with safety and quality layers: input moderation, query enrichment, output validation and response normalization. It covers the `IChatPipelineStage` abstraction, `ChatPipelineOrchestrator` design, stage ordering and semantics, SSE streaming integration, configuration model, and the GPT-4o-mini deployment decision.

**Audience**: Developers implementing T-01 through T-13 of F1-18.

**Reference**: E215 (Chat Tools architecture), E216 (Agentic loop), `ChatQueryHandler` implementation, `AzureOpenAiAgentChatService`, architecture §5 (Chat RAG flow).

---

## 1. Current State

The chat pipeline (`ChatQueryHandler`, F1-17) operates as a single-stage agentic loop:

```
query → validate length → build messages → agentic loop (GPT-4o + tools) → SSE stream
```

Key gaps:
- **No input moderation**: Any query reaches the LLM. The system prompt instructs the model not to call tools for greetings, but there is no enforcement against out-of-scope questions, prompt injection, or requests for personal legal advice.
- **No output validation**: Ruling IDs cited in responses are not verified against the database. The model can hallucinate case titles, mix up IDs, or omit disclaimers.
- **No intent understanding**: The raw user query enters the agentic loop without classification. A greeting, a complex legal question, and an injection attempt follow the same path.
- **No response normalization**: Citation format consistency and disclaimer presence depend entirely on model adherence to the system prompt.
- **ContentFilter not handled**: `AgentFinishReason.ContentFilter` falls through to `yield break` silently.

---

## 2. Target Architecture

### 2.1 Pipeline Overview

```
User Query
    │
    ├── 1. InputGuardrailStage (pre-stream, fail-closed)
    │       Classify: legal_query | greeting | clarification | out_of_scope | harmful
    │       Reject → templated SSE response
    │
    ├── 2. QueryEnricherStage (pre-stream, fail-open)
    │       Extract intent + entities → inject as system message
    │
    ├── 3. Agentic Executor (existing ChatQueryHandler loop)
    │       GPT-4o + tools → SSE text chunks + tool events
    │
    ├── 4. OutputGuardrailStage (post-stream, fail-closed)
    │       Validate citations against DB + tool results
    │       Emit SSE validation event
    │
    └── 5. ResponseFinalizerStage (chunk-mode, fail-open)
            Normalize citations, cleanup markdown, inject disclaimer
```

### 2.2 Design Principles

| Principle | Implementation |
|-----------|---------------|
| **Minimize LLM calls** | Stages 1 and 2 use rule-based fast paths; LLM (GPT-4o-mini) only for ambiguous cases. Stages 4 and 5 are fully deterministic (DB + string processing). |
| **Preserve streaming** | Pre-stream stages (1, 2) execute before streaming starts. Post-stream stage (4) executes after the last text chunk. Chunk-mode stage (5) processes text inline during streaming. |
| **Pipeline as middleware** | Each stage implements `IChatPipelineStage`. Stages are enabled/disabled via configuration. The orchestrator chains them around the existing executor. |
| **Fail-open vs fail-closed** | Input guardrail and output guardrail are fail-closed (block if uncertain). Query enricher and response finalizer are fail-open (failure → proceed without enhancement). |

---

## 3. IChatPipelineStage

### 3.1 Interface

```csharp
public interface IChatPipelineStage
{
    string Name { get; }
    ChatPipelinePhase Phase { get; }
    bool IsEnabled(ChatPipelineOptions options);

    Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context,
        CancellationToken cancellationToken = default);
}
```

### 3.2 ChatPipelinePhase

```csharp
public enum ChatPipelinePhase
{
    PreStream,
    ChunkMode,
    PostStream
}
```

- `PreStream`: Executes before the agentic loop starts. Can short-circuit the pipeline (e.g., guardrail rejection).
- `ChunkMode`: Processes each `ChatStreamEvent` as it flows through the pipeline. Runs inline during streaming.
- `PostStream`: Executes after the agentic loop completes and all text has been accumulated. Can append events to the stream.

### 3.3 ChatPipelineContext

```csharp
public sealed class ChatPipelineContext
{
    public string OriginalQuery { get; init; } = string.Empty;
    public string? ClassifiedIntent { get; set; }
    public GuardrailClassification? InputClassification { get; set; }
    public QueryEnrichment? Enrichment { get; set; }
    public List<AgentChatMessage> Messages { get; init; } = new();
    public ToolExecutionContext ToolContext { get; init; } = null!;
    public StringBuilder AccumulatedResponse { get; } = new();
    public List<ChatStreamEvent> OutputEvents { get; } = new();
    public bool IsShortCircuited { get; set; }
}
```

The context is shared across all stages and carries state from one stage to the next.

### 3.4 ChatPipelineResult

```csharp
public sealed record ChatPipelineResult(
    bool ShouldContinue,
    IReadOnlyList<ChatStreamEvent>? ImmediateEvents = null);
```

- `ShouldContinue = true`: Pipeline proceeds to the next stage or to the executor.
- `ShouldContinue = false`: Pipeline short-circuits. `ImmediateEvents` (if any) are streamed to the client as the response (e.g., rejection template text).

---

## 4. ChatPipelineOrchestrator

### 4.1 Responsibility

The orchestrator replaces the direct invocation of the agentic loop in `ChatController`. It:

1. Constructs the `ChatPipelineContext` from the incoming `ChatQueryCommand`.
2. Executes all `PreStream` stages in order. If any returns `ShouldContinue = false`, yields the immediate events and terminates.
3. Invokes the agentic executor (`ChatQueryHandler.Handle`), piping each `ChatStreamEvent` through `ChunkMode` stages.
4. After the executor completes, executes all `PostStream` stages.
5. Yields any appended events from post-stream stages (validation, disclaimers).

### 4.2 Integration with ChatQueryHandler

The orchestrator does **not** replace `ChatQueryHandler`. Instead, it wraps it:

```csharp
public class ChatPipelineOrchestrator : IStreamRequestHandler<ChatQueryCommand, ChatStreamEvent>
{
    private readonly IEnumerable<IChatPipelineStage> _stages;
    private readonly ChatQueryHandler _executor;
    private readonly ChatPipelineOptions _options;
    // ...

    public async IAsyncEnumerable<ChatStreamEvent> Handle(
        ChatQueryCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var context = BuildContext(request);

        // 1. Pre-stream stages
        foreach (var stage in _stages.Where(s => s.Phase == ChatPipelinePhase.PreStream && s.IsEnabled(_options)))
        {
            var result = await stage.ProcessAsync(context, cancellationToken);
            if (!result.ShouldContinue)
            {
                if (result.ImmediateEvents is not null)
                    foreach (var evt in result.ImmediateEvents)
                        yield return evt;
                yield break;
            }
        }

        // 2. Agentic executor with chunk-mode processing
        var chunkStages = _stages
            .Where(s => s.Phase == ChatPipelinePhase.ChunkMode && s.IsEnabled(_options))
            .ToList();

        await foreach (var evt in _executor.Handle(
            request with { Query = context.OriginalQuery, Messages = context.Messages },
            cancellationToken))
        {
            if (evt is ChatTextChunk textChunk)
                context.AccumulatedResponse.Append(textChunk.Text);

            yield return evt;
        }

        // 3. Post-stream stages
        foreach (var stage in _stages.Where(s => s.Phase == ChatPipelinePhase.PostStream && s.IsEnabled(_options)))
        {
            var result = await stage.ProcessAsync(context, cancellationToken);
            if (result.ImmediateEvents is not null)
                foreach (var evt in result.ImmediateEvents)
                    yield return evt;
        }
    }
}
```

### 4.3 Stage Ordering

| Order | Stage | Phase | Fail mode |
|-------|-------|-------|-----------|
| 1 | `InputGuardrailStage` | PreStream | Closed |
| 2 | `QueryEnricherStage` | PreStream | Open |
| 3 | `ResponseFinalizerStage` | ChunkMode | Open |
| 4 | `OutputGuardrailStage` | PostStream | Closed |

Stage ordering is fixed by convention (registered in DI in this order). The orchestrator iterates them by phase.

### 4.4 DI Registration

```csharp
services.AddSingleton<IChatPipelineStage, InputGuardrailStage>();
services.AddSingleton<IChatPipelineStage, QueryEnricherStage>();
services.AddSingleton<IChatPipelineStage, ResponseFinalizerStage>();
services.AddSingleton<IChatPipelineStage, OutputGuardrailStage>();
services.AddScoped<ChatPipelineOrchestrator>();
services.Configure<ChatPipelineOptions>(config.GetSection("ChatPipeline"));
```

---

## 5. Input Guardrail Stage

### 5.1 Two-Layer Classification

**Layer 1 — Rule-based (< 10ms)**:
- Regex patterns for prompt injection: `ignore.*(?:previous|prior).*instructions`, `you are now`, `repeat.*(?:system|instructions)`, `act as`, `pretend you`
- PII patterns: Argentine DNI (`\b\d{7,8}\b` near personal context keywords), CUIT (`\b\d{2}-\d{8}-\d\b`)
- Scope keywords: known legal terms → `legal_query`, greeting patterns → `greeting`
- If a pattern matches with high confidence, classification is returned immediately.

**Layer 2 — LLM classifier (200-500ms, only if Layer 1 is inconclusive)**:
- `IGuardrailClassifier` interface in Core:

```csharp
public interface IGuardrailClassifier
{
    Task<GuardrailClassification> ClassifyAsync(
        string query, CancellationToken cancellationToken = default);
}
```

- `AzureOpenAiGuardrailClassifier` in Infrastructure using GPT-4o-mini with a focused prompt:

```
Classify the following user query for a legal jurisprudence assistant. 
Respond with ONLY one of: legal_query, greeting, clarification, out_of_scope, harmful

Query: {query}
```

- Temperature: 0, MaxOutputTokens: 10.

### 5.2 GuardrailClassification

```csharp
public sealed record GuardrailClassification(
    GuardrailCategory Category,
    GuardrailSource Source,
    float? Confidence);

public enum GuardrailCategory
{
    LegalQuery,
    Greeting,
    Clarification,
    OutOfScope,
    Harmful
}

public enum GuardrailSource
{
    RuleBased,
    LlmClassifier
}
```

### 5.3 Rejection Templates

Templates are stored in `IGuardrailTemplateProvider` (configurable, not hardcoded):

| Category | Template (Spanish) |
|----------|-------------------|
| `Greeting` | "¡Hola! Soy el asistente jurídico de Legal AI AR. Puedo ayudarte con:\n\n- Buscar fallos judiciales por tema, tribunal, fecha o norma\n- Obtener detalles de un fallo específico\n- Explorar cadenas de precedentes y citas\n- Consultar qué fallos aplican una ley o artículo\n\n¿En qué puedo ayudarte?" |
| `OutOfScope` | "No puedo ayudarte con ese tema. Soy un asistente especializado en jurisprudencia argentina.\n\nPuedo ayudarte con consultas como:\n- \"¿Qué fallos de la CSJN tratan sobre libertad de expresión?\"\n- \"Mostrá los detalles del fallo Ekmekdjian c/ Sofovich\"\n- \"¿Qué fallos citan el art. 14 de la Constitución Nacional?\"\n- \"¿Cuántos fallos de inconstitucionalidad hay en materia penal?\"" |
| `Harmful` | "No puedo procesar esta consulta. Soy un asistente de información jurisprudencial y no puedo:\n- Brindar asesoramiento legal personalizado\n- Modificar mi funcionamiento o instrucciones\n- Responder consultas fuera del ámbito jurisprudencial\n\nSi necesitás información sobre fallos judiciales argentinos, estoy para ayudarte." |
| `Clarification` | "Tu consulta es un poco ambigua. ¿Podrías ser más específico?\n\nPor ejemplo, podés preguntar:\n- Sobre un tema legal concreto: \"Fallos sobre despido injustificado\"\n- Sobre una norma: \"Fallos que aplican la Ley 20.744\"\n- Sobre un fallo específico: \"Detalles del fallo Arriola\"" |

### 5.4 ContentFilter Handling

When the agentic executor returns `AgentFinishReason.ContentFilter`, the orchestrator emits:

```
data: No puedo procesar esta consulta debido a restricciones de contenido. Por favor reformulá tu pregunta.
data: [DONE]
```

This is implemented in `ChatQueryHandler` directly (T-04), not in a pipeline stage, since it occurs during execution.

---

## 6. Query Enricher Stage

### 6.1 Intent Taxonomy

| Intent | Trigger patterns | Primary tool suggestion |
|--------|-----------------|----------------------|
| `search` | Default for legal queries without specific markers | `search_rulings` |
| `detail` | "detalles de", "mostrá el fallo", reference to a specific case | `get_ruling_detail` |
| `comparison` | "comparar", "diferencia entre", "vs" | `search_rulings` (multiple) |
| `statistics` | "cuántos", "cantidad", "porcentaje" | `count_rulings` |
| `precedent_exploration` | "citar", "citado por", "precedente", "cadena" | `get_ruling_citations` |
| `statute_research` | "art.", "artículo", "ley", "código", "norma" | `search_by_statute` |
| `general` | Fallback | No specific suggestion |

### 6.2 Entity Extraction

| Category | Pattern examples | Regex approach |
|----------|-----------------|---------------|
| Temporal | "entre 2020 y 2024", "últimos 5 años", "desde 2023" | `\b(19|20)\d{2}\b`, `entre\s+\d{4}\s+y\s+\d{4}` |
| Institutional | "CSJN", "Cámara Federal", "CABA", "La Plata" | Keyword dictionary of known courts and jurisdictions |
| Normative | "art. 14 CN", "Ley 26.994", "Código Penal" | `(?:art(?:ículo)?\.?\s*\d+)`, `(?:ley\s+\d[\d.]+)` |
| Case identifiers | "Ekmekdjian c/ Sofovich", "fallo Arriola" | `\w+\s+c[/\.]\s+\w+` pattern for "X c/ Y" format |
| Subject matter | Legal topic keywords | Not regex-extracted; passed to LLM if needed |

### 6.3 Context Injection Format

When enrichment succeeds, the enricher appends a system message to the `ChatPipelineContext.Messages` list (after the main system prompt, before the user query):

```
[Query Analysis]
Intent: statute_research
Entities detected:
- Court: CSJN
- Statute: art. 14 CN (Constitución Nacional)
- Period: 2020-2024
- Topic: libertad de expresión
Consider using search_by_statute for the normative reference and search_rulings with court and date filters.
```

### 6.4 Graceful Degradation

If the enricher fails (timeout, LLM error, parse error), it logs the error at `Warning` level and sets `context.Enrichment = null`. The pipeline proceeds with the raw query. The executor handles unenriched queries identically to the current behavior.

---

## 7. Output Guardrail Stage

### 7.1 Citation Validation Pipeline

```
1. Parse citations from accumulated response via CitationParser
2. Collect unique ruling IDs
3. Batch query: SELECT Id, CaseTitle FROM Rulings WHERE Id IN (@ids)
4. For each citation:
   a. Check ID exists in DB → flag if not
   b. Compare caseTitle with DB (fuzzy match, threshold 0.85) → flag if mismatch
   c. Check ID in ToolExecutionContext.ResolvedRulingIds → flag if not grounded
5. Produce CitationValidationResult
```

### 7.2 CitationParser

Shared utility in `LegalAiAr.Application.Chat.Utilities`:

```csharp
public static class CitationParser
{
    public static IReadOnlyList<Citation> Parse(string text);
    public static string Normalize(string text);
    public static CitationValidation Validate(
        Citation citation, IReadOnlyDictionary<Guid, string> knownRulings);
}

public sealed record Citation(string CaseTitle, Guid RulingId, int StartIndex, int EndIndex);

public sealed record CitationValidation(
    Citation Citation,
    bool IdExists,
    bool TitleMatches,
    bool IsToolGrounded);
```

Regex pattern (aligned with frontend `CITATION_REGEX`):

```regex
\{caso:\s*["']([^"']+)["']\s*,\s*id:\s*["']([0-9a-f-]+)["']\s*\}
```

### 7.3 Validation Result Handling

| Scenario | Strictness: lenient | Strictness: moderate | Strictness: strict |
|----------|:---:|:---:|:---:|
| All valid | Log info | Log info | Log info |
| Title mismatch | Log warning | Append correction | Append correction |
| ID not in DB | Log warning | Append warning | Append warning + disclaimer |
| Not tool-grounded | Log warning | Append warning | Append warning + disclaimer |
| All invalid | Log error | Append strong disclaimer | Append disclaimer + regeneration note |

### 7.4 SSE Validation Event

Emitted after the last `ChatTextChunk` and before `[DONE]`:

```
event: validation
data: {"status":"passed","citationsChecked":3,"valid":3,"warnings":0}

data: [DONE]
```

Or with warnings:

```
event: validation
data: {"status":"warnings","citationsChecked":3,"valid":2,"warnings":1,"details":["Ruling ID abc123 not found in database"]}

data: Nota: Algunas referencias en esta respuesta no pudieron ser verificadas contra la base de datos. Recomendamos verificar los fallos citados directamente.
data: [DONE]
```

---

## 8. Response Finalizer Stage

### 8.1 Chunk-Mode Processing

The finalizer operates in chunk mode, processing each `ChatTextChunk` as it flows through the pipeline. To handle citations that span chunk boundaries, it maintains a 200-character look-ahead buffer:

```
1. Append incoming chunk to buffer
2. Scan buffer for complete citation patterns
3. For each complete citation: normalize format, emit normalized text
4. Emit all text before the first incomplete citation (or all text if no incomplete pattern)
5. Retain any trailing incomplete pattern in buffer for next chunk
6. On stream end: flush buffer as-is
```

### 8.2 Citation Normalization Rules

| Input variation | Normalized output |
|----------------|------------------|
| `{ caso: "Title" , id: "uuid" }` | `{caso: "Title", id: "uuid"}` |
| `{caso: 'Title', id: 'uuid'}` | `{caso: "Title", id: "uuid"}` |
| `{caso:"Title",id:"uuid"}` | `{caso: "Title", id: "uuid"}` |

### 8.3 Disclaimer Injection

Conditional: only appended when the output guardrail flags the response as containing substantive legal content (not greetings, clarifications, or error messages). The disclaimer text is configurable:

Default:
```
---
*Esta información es de carácter referencial y no constituye asesoramiento legal. Consultá con un profesional del derecho para tu caso particular.*
```

### 8.4 Markdown Cleanup

- Collapse `\n{3,}` → `\n\n`
- Close unclosed `**` and `*` markers at paragraph boundaries
- Remove orphan `#` markers without heading text

### 8.5 Empty Response Fallback

If `AccumulatedResponse` is empty or whitespace-only after the executor completes, emit:

```
data: No pude generar una respuesta para tu consulta. Por favor intentá reformular tu pregunta o ser más específico sobre lo que necesitás.
data: [DONE]
```

---

## 9. Configuration

### 9.1 ChatPipelineOptions

```csharp
public sealed class ChatPipelineOptions
{
    public InputGuardrailOptions InputGuardrail { get; set; } = new();
    public QueryEnricherOptions QueryEnricher { get; set; } = new();
    public OutputGuardrailOptions OutputGuardrail { get; set; } = new();
    public ResponseFinalizerOptions ResponseFinalizer { get; set; } = new();
}

public sealed class InputGuardrailOptions
{
    public bool Enabled { get; set; } = true;
    public bool UseLlmClassifier { get; set; } = true;
}

public sealed class QueryEnricherOptions
{
    public bool Enabled { get; set; } = true;
    public bool UseLlmFallback { get; set; } = true;
}

public sealed class OutputGuardrailOptions
{
    public bool Enabled { get; set; } = true;
    public string Strictness { get; set; } = "moderate";
}

public sealed class ResponseFinalizerOptions
{
    public bool Enabled { get; set; } = true;
    public bool DisclaimerEnabled { get; set; } = true;
    public string DisclaimerText { get; set; } = "Esta información es de carácter referencial y no constituye asesoramiento legal. Consultá con un profesional del derecho para tu caso particular.";
    public bool StructureEnforcement { get; set; } = false;
}
```

### 9.2 appsettings.json

```json
{
  "ChatPipeline": {
    "InputGuardrail": {
      "Enabled": true,
      "UseLlmClassifier": true
    },
    "QueryEnricher": {
      "Enabled": true,
      "UseLlmFallback": true
    },
    "OutputGuardrail": {
      "Enabled": true,
      "Strictness": "moderate"
    },
    "ResponseFinalizer": {
      "Enabled": true,
      "DisclaimerEnabled": true,
      "StructureEnforcement": false
    }
  },
  "AzureOpenAI": {
    "MiniDeploymentName": "gpt-4o-mini"
  }
}
```

---

## 10. GPT-4o-mini Deployment Decision

### 10.1 Decision

**Use GPT-4o-mini** for the input guardrail classifier and query enricher LLM fallback. Add a new deployment `gpt-4o-mini` in the existing Azure OpenAI resource.

### 10.2 Rationale

| Factor | GPT-4o | GPT-4o-mini |
|--------|--------|-------------|
| Cost per 1M input tokens | ~$2.50 | ~$0.15 |
| Cost per classification (~50 tokens) | ~$0.000125 | ~$0.0000075 |
| Latency (classification) | ~800ms | ~300ms |
| Accuracy for classification | Overkill | Sufficient |

For binary/categorical classification tasks (5 categories, short prompt), GPT-4o-mini provides equivalent accuracy at 17x lower cost and ~2.5x lower latency.

### 10.3 Infrastructure Impact

- New deployment in Azure OpenAI resource: `gpt-4o-mini` (model: `gpt-4o-mini`)
- New config key: `AzureOpenAI:MiniDeploymentName`
- `AzureOpenAiGuardrailClassifier` creates a separate `ChatClient` using the mini deployment
- ADR-014 updated to include GPT-4o-mini for lightweight classification tasks

---

## 11. Observability

| Stage | Log level | Fields |
|-------|-----------|--------|
| Input guardrail | Information | Category, Source (rule/LLM), Confidence, LatencyMs |
| Input guardrail (rejection) | Warning | Category, OriginalQuery (truncated to 200 chars) |
| Query enricher | Information | Intent, EntityCount, LatencyMs, Source (rule/LLM) |
| Query enricher (failure) | Warning | ErrorMessage, LatencyMs |
| Output guardrail | Information | CitationsChecked, ValidCount, WarningCount, LatencyMs |
| Output guardrail (issues) | Warning | InvalidCitations (IDs), MismatchedTitles |
| Response finalizer | Debug | NormalizedCount, DisclaimerInjected, MarkdownFixes |

---

## 12. Decisions

| # | Decision | Rationale |
|---|---|---|
| D-01 | Pipeline wraps executor, does not replace it | `ChatQueryHandler` remains unchanged. Pipeline stages are additive. Zero regression risk on existing chat functionality. Each stage can be disabled independently. |
| D-02 | `IChatPipelineStage` as single interface with `Phase` discriminator | Simpler than separate `IPreStreamStage` / `IChunkStage` / `IPostStreamStage` interfaces. The orchestrator filters by phase. |
| D-03 | GPT-4o-mini for classification, not GPT-4o | 17x cheaper, 2.5x faster, equivalent accuracy for categorical classification. See section 10. |
| D-04 | Rule-based first, LLM second | Minimizes LLM calls (60-70% of queries classified by rules). Rules are deterministic, auditable, and instant. |
| D-05 | Stream-then-append for output guardrail | Preserves streaming UX. Validation runs after the response is complete (~100ms). No perceived latency increase for the user during response generation. |
| D-06 | CitationParser as static utility, not a service | Citation parsing is stateless string processing. No dependencies to inject. Reusable across output guardrail and response finalizer. |
| D-07 | Fail-closed for guardrails, fail-open for enrichment | Safety-critical stages (input/output guardrails) block on failure. Enhancement stages (enricher/finalizer) degrade gracefully — the system works without them, just with less quality. |

---

## 13. References

- `docs/design/f1-17-chat-tools-architecture.md` — agentic loop design (E215)
- `docs/design/f1-17-agentic-loop.mermaid` — current sequence diagram (E216)
- `docs/architecture/legal-ai-ar-architecture.md` — architecture §5 (Chat RAG flow), ADR-014
- `ChatQueryHandler` — current agentic executor implementation
- `AzureOpenAiAgentChatService` — `AgentFinishReason.ContentFilter` mapping
- `chat-view.component.ts` — frontend `CITATION_REGEX` and tool chip rendering
