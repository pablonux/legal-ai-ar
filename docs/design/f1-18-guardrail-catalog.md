# Chat Pipeline — Guardrail & Enrichment Catalog

| Field | Value |
|---|---|
| **ID** | E234 |
| **Feature** | F1-18 · Chat Pipeline — Multi-Stage Architecture |
| **Date** | 2026-03-20 |

---

## Purpose

This document specifies the complete catalog of classification categories, detection patterns, rejection templates, intent taxonomy, entity categories, and validation rules used by the chat pipeline stages (E232). It serves as the authoritative reference for implementing the input guardrail (T-02/T-03), query enricher (T-05/T-06), and output guardrail (T-08/T-09).

**Audience**: Developers implementing T-02 through T-10 of F1-18.

**Reference**: E232 (Pipeline architecture), E215 (Chat Tools architecture), `ChatQueryHandler` system prompt.

---

## 1. Input Guardrail — Classification Categories

### 1.1 Category Definitions

| Category | Definition | Action | Fail mode |
|----------|-----------|--------|-----------|
| `legal_query` | Query about Argentine jurisprudence, rulings, courts, judges, statutes, or legal topics within the system's knowledge base | Proceed to enricher → executor | — |
| `greeting` | Salutation, small talk, or "what can you do" type query with no legal substance | Return greeting template (no executor call) | — |
| `clarification` | Ambiguous query that could be legal but is too vague to process effectively | Return clarification template (no executor call) | — |
| `out_of_scope` | Non-legal question (technology, recipes, sports, general knowledge, math, etc.) | Return out-of-scope template | — |
| `harmful` | Prompt injection, jailbreak attempt, system prompt extraction, request for personal legal advice, or ethically inappropriate content | Return harmful template | — |

### 1.2 Classification Decision Matrix

| Signal | → Category |
|--------|-----------|
| Matches prompt injection regex | `harmful` |
| Matches PII pattern in personal-advice context | `harmful` |
| Matches greeting regex exclusively | `greeting` |
| Contains legal keywords + structured question | `legal_query` |
| Contains no legal keywords and no harmful patterns | `out_of_scope` (or defer to LLM) |
| Contains legal keywords but too vague (< 3 words) | `clarification` |

---

## 2. Input Guardrail — Detection Patterns

### 2.1 Prompt Injection Patterns

All patterns are case-insensitive. Applied to the full query text.

| ID | Pattern (regex) | Description |
|----|----------------|-------------|
| PI-01 | `ignor[aá]\s+.*(?:instrucciones\|prompt\|reglas\|sistema)` | "Ignorá las instrucciones anteriores" |
| PI-02 | `(?:ahora\s+)?(?:sos\|eres\|serás\|actua\s+como)\s+(?:un\|una\|el\|la)` | "Ahora sos un..." / "Actua como un..." |
| PI-03 | `repet[ií]\s+.*(?:instrucciones\|prompt\|sistema\|reglas)` | "Repetí tus instrucciones" |
| PI-04 | `(?:cu[áa]l(?:es)?\s+(?:son\|es)\s+tu\|mostr[áa].*tu)\s+(?:prompt\|instrucciones\|sistema\|reglas)` | "Cuáles son tus instrucciones" |
| PI-05 | `(?:olvidá\|olvidate\|olvida)\s+.*(?:anterior\|previo\|reglas)` | "Olvidate de lo anterior" |
| PI-06 | `(?:ignore\|disregard\|forget)\s+.*(?:previous\|prior\|above\|instructions)` | English injection attempts |
| PI-07 | `(?:you\s+are\s+now\|act\s+as\|pretend\s+you)` | English role-override attempts |
| PI-08 | `system\s*:\s*` | Attempt to inject system message |
| PI-09 | `\[INST\]\|\[\/INST\]\|<\|im_start\|>\|<\|im_end\|>` | Chat template injection markers |

### 2.2 Personal Legal Advice Patterns

| ID | Pattern (regex) | Description |
|----|----------------|-------------|
| PLA-01 | `(?:qu[ée]\s+(?:deber[ií]a\|tengo\s+que\|puedo)\s+hacer\s+(?:en\|con)\s+mi\s+caso)` | "¿Qué debería hacer en mi caso?" |
| PLA-02 | `(?:me\s+conviene\|me\s+recomend[áa]s\|aconse[jg][áa]me)` | "¿Me conviene apelar?" |
| PLA-03 | `(?:mi\s+(?:abogado\|caso\|situaci[oó]n\|problema)\s+)` | References to personal legal situation |
| PLA-04 | `(?:cómo\s+(?:demando\|denuncio\|inicio\s+(?:un\s+)?juicio))` | "¿Cómo inicio un juicio?" |

### 2.3 PII Patterns

| ID | Pattern (regex) | Description |
|----|----------------|-------------|
| PII-01 | `\b\d{2}[-.]?\d{8}[-.]?\d{1}\b` | Argentine CUIT/CUIL format (XX-XXXXXXXX-X) |
| PII-02 | `\b(?:DNI\|D\.N\.I\.?)\s*:?\s*\d{7,8}\b` | DNI with prefix |
| PII-03 | `\b\d{7,8}\b` (only in context of `mi caso`, `mi DNI`, etc.) | Standalone DNI number in personal context |

PII detection is logged for audit but does not block the query in v1. If PII is detected alongside a personal-advice pattern (PLA-*), the query is classified as `harmful`.

### 2.4 Greeting Patterns

| ID | Pattern (regex) | Description |
|----|----------------|-------------|
| GR-01 | `^(?:hola\|buenas?\s+(?:tardes?\|noches?\|d[ií]as?)\|hey\|hi\|buen[oa]s?)[\s!?.]*$` | Pure greeting |
| GR-02 | `^(?:qu[ée]\s+(?:pod[ée]s\|pued[ée]s)\s+hacer\|(?:para\s+)?qu[ée]\s+serv[ií]s\|ayuda\|help)[\s?]*$` | "¿Qué podés hacer?" |
| GR-03 | `^(?:gracias\|muchas\s+gracias\|ok\|dale\|perfecto\|genial)[\s!.]*$` | Thanks/acknowledgment |

### 2.5 Legal Scope Keywords

Presence of any of these keywords (case-insensitive) is a strong positive signal for `legal_query`:

```
fallo, fallos, jurisprudencia, sentencia, resolución, tribunal, corte, cámara, 
juzgado, juez, jueces, CSJN, recurso, apelación, casación, amparo, habeas corpus,
inconstitucionalidad, constitución, constitucional, código, ley, decreto, norma, 
artículo, art., demanda, demandante, demandado, actor, acción, competencia,
jurisdicción, penal, civil, laboral, comercial, contencioso, administrativo,
cautelar, embargo, libertad condicional, prisión preventiva, despido, indemnización,
daños, perjuicios, responsabilidad, prescripción, caducidad, cosa juzgada,
litispendencia, nulidad, debido proceso, garantía, derecho, precedente, doctrina
```

---

## 3. Input Guardrail — Rejection Templates

### 3.1 Greeting Response

```
¡Hola! Soy el asistente jurídico de Legal AI AR. Estoy especializado en jurisprudencia argentina y puedo ayudarte con:

- **Buscar fallos judiciales** por tema, tribunal, fecha, jurisdicción o palabras clave
- **Consultar detalles de un fallo** específico: jueces, normas citadas, sumario, resolución
- **Explorar cadenas de precedentes**: qué fallos citan a otro y qué fallos son citados
- **Buscar fallos que apliquen una norma**: por ley, artículo o código
- **Obtener estadísticas**: cantidad de fallos por tribunal, jurisdicción, materia

¿En qué puedo ayudarte?
```

### 3.2 Out-of-Scope Response

```
No puedo ayudarte con ese tema. Soy un asistente especializado exclusivamente en jurisprudencia argentina.

Puedo responder consultas como:
- "¿Qué fallos de la CSJN tratan sobre libertad de expresión?"
- "Mostrá los detalles del fallo Ekmekdjian c/ Sofovich"
- "¿Qué fallos citan el art. 14 de la Constitución Nacional?"
- "¿Cuántos fallos de inconstitucionalidad hay en materia penal?"

¿Tenés alguna consulta jurisprudencial?
```

### 3.3 Harmful Response

```
No puedo procesar esta consulta. Soy un asistente de información jurisprudencial y no puedo:

- Brindar asesoramiento legal personalizado
- Recomendar acciones legales para tu situación particular
- Modificar mi funcionamiento o instrucciones
- Responder consultas fuera del ámbito jurisprudencial argentino

Si necesitás asesoramiento legal, te recomendamos consultar con un profesional del derecho. Si necesitás información sobre fallos judiciales argentinos, estoy para ayudarte.
```

### 3.4 Clarification Response

```
Tu consulta es un poco amplia. ¿Podrías ser más específico para que pueda ayudarte mejor?

Por ejemplo:
- Sobre un tema legal concreto: "Fallos sobre despido injustificado en la CSJN"
- Sobre una norma específica: "Fallos que aplican la Ley 20.744 de contrato de trabajo"
- Sobre un fallo en particular: "Detalles del fallo Arriola sobre tenencia de estupefacientes"
- Sobre precedentes: "¿Qué fallos posteriores citaron Ekmekdjian c/ Sofovich?"
```

### 3.5 ContentFilter Response

Used when Azure OpenAI returns `FinishReason.ContentFilter`:

```
No puedo procesar esta consulta debido a restricciones de contenido. Por favor reformulá tu pregunta de otra manera.

Recordá que puedo ayudarte con consultas sobre jurisprudencia argentina, como buscar fallos, explorar precedentes o consultar normas citadas en sentencias.
```

---

## 4. Query Enricher — Intent Taxonomy

### 4.1 Intent Definitions

| Intent | Description | Expected primary tool |
|--------|------------|----------------------|
| `search` | Find rulings matching criteria (topic, date range, court, keywords) | `search_rulings` |
| `detail` | Get detailed information about a specific, identified ruling | `get_ruling_detail` |
| `comparison` | Compare legal positions, rulings, or judicial criteria across cases | `search_rulings` (multiple calls) |
| `statistics` | Quantitative question about ruling counts, distributions, frequencies | `count_rulings` |
| `precedent_exploration` | Navigate citation chains — who cites whom, influence analysis | `get_ruling_citations`, `get_related_rulings` |
| `statute_research` | Find rulings that cite, apply, or interpret a specific law or article | `search_by_statute` |
| `general` | General legal question not fitting specific categories | `search_rulings` (broad) |

### 4.2 Rule-Based Intent Detection Patterns

| Intent | Trigger patterns (regex, case-insensitive) |
|--------|-------------------------------------------|
| `statistics` | `cu[áa]ntos?\b`, `cantidad\b`, `porcentaje\b`, `estad[ií]sticas?\b`, `total\s+de\b` |
| `statute_research` | `(?:art(?:[ií]culo)?\.?\s*\d+)`, `(?:ley\s+\d[\d.]*)`, `(?:c[óo]digo\s+(?:penal\|civil\|comercial\|procesal))`, `(?:decreto\s+\d+)`, `(?:constituci[óo]n\s+nacional\|CN\b)` |
| `precedent_exploration` | `cit[aáe]\w*\b`, `citado\s+por\b`, `precedente\b`, `cadena\b`, `influen\w+\b`, `posterior\w*\s+(?:a\|al\|que)\b` |
| `detail` | `detalle\w*\b`, `mostr[áa]\w*\b.*fallo`, `informaci[óo]n\s+(?:del?\|sobre)\s+(?:el\s+)?fallo`, `qui[ée]nes?\s+(?:firmaron\|votaron)` |
| `comparison` | `compar[áa]\w*\b`, `diferencia\w*\b`, `vs\.?\b`, `(?:en\s+)?contra(?:posici[óo]n\|ste)\b` |
| `search` | Default when legal keywords are present but no specific intent marker |
| `general` | Fallback when no specific pattern matches |

### 4.3 Intent Confidence

Rule-based detection assigns confidence:
- **High (0.9+)**: Strong pattern match (e.g., "¿cuántos fallos..." → `statistics`)
- **Medium (0.6-0.8)**: Keyword match without structural certainty
- **Low (< 0.6)**: Ambiguous → trigger LLM fallback if enabled

---

## 5. Query Enricher — Entity Categories

### 5.1 Temporal Entities

| Pattern type | Regex | Example | Extracted value |
|-------------|-------|---------|-----------------|
| Year range | `entre\s+(\d{4})\s+y\s+(\d{4})` | "entre 2020 y 2024" | `{ from: 2020, to: 2024 }` |
| Since year | `desde\s+(?:el\s+)?(\d{4})` | "desde 2020" | `{ from: 2020 }` |
| Until year | `hasta\s+(?:el\s+)?(\d{4})` | "hasta 2023" | `{ to: 2023 }` |
| Last N years | `[úu]ltimos?\s+(\d+)\s+a[ñn]os?` | "últimos 5 años" | `{ from: currentYear - 5 }` |
| Specific year | `\b((?:19|20)\d{2})\b` (in date context) | "fallos de 2023" | `{ from: 2023, to: 2023 }` |
| Date range | `(\d{1,2})[/\-](\d{1,2})[/\-](\d{4})` | "15/03/2024" | Parsed date |

### 5.2 Institutional Entities

| Entity type | Detection approach | Examples |
|-------------|-------------------|----------|
| Supreme Court | Keyword: `CSJN`, `Corte Suprema`, `Suprema Corte` | "fallos de la CSJN" |
| Federal chambers | Keyword: `Cámara Federal`, `C.Fed.`, abbreviations | "Cámara Federal de Apelaciones" |
| Jurisdictions | Dictionary: `Nacional`, `Buenos Aires`, `CABA`, provinces | "jurisdicción de CABA" |
| Instances | Keyword: `primera instancia`, `cámara`, `casación` | "fallos de cámara" |

Known court/jurisdiction dictionary (seed from `Courts` table values):

```
CSJN, Cámara Nacional de Apelaciones en lo Civil, Cámara Nacional de Apelaciones en lo Penal,
Cámara Federal de Casación Penal, Cámara Nacional de Casación en lo Criminal y Correccional,
Cámara Nacional Electoral, Suprema Corte de Justicia de Buenos Aires, ...
```

### 5.3 Normative Entities

| Pattern | Regex | Example | Extracted |
|---------|-------|---------|-----------|
| Article + law | `art(?:[ií]culo)?\.?\s*(\d+)\s+(?:de\s+la\s+)?(?:CN\|Constituci[óo]n\s+Nacional)` | "art. 14 CN" | `{ statute: "CN", articles: ["14"] }` |
| Numbered law | `[Ll]ey\s+(\d[\d.]*)` | "Ley 20.744" | `{ statute: "20.744" }` |
| Named code | `[Cc][óo]digo\s+(Penal\|Civil\|Comercial\|Procesal\s+\w+)` | "Código Penal" | `{ statute: "Código Penal" }` |
| Article + code | `art(?:[ií]culo)?\.?\s*(\d+)\s+(?:del?\s+)?[Cc][óo]digo\s+(\w+)` | "art. 79 del Código Penal" | `{ statute: "Código Penal", articles: ["79"] }` |
| Decree | `[Dd]ecreto\s+(\d+[/\-]?\d*)` | "Decreto 70/2023" | `{ statute: "Decreto 70/2023" }` |

### 5.4 Case Identifier Entities

| Pattern | Regex | Example |
|---------|-------|---------|
| "X c/ Y" format | `([A-ZÁÉÍÓÚÑ][\wáéíóúñ.]+(?:\s+[\wáéíóúñ.]+)*)\s+c[/\.]\s+([A-ZÁÉÍÓÚÑ][\wáéíóúñ.]+(?:\s+[\wáéíóúñ.]+)*)` | "Ekmekdjian c/ Sofovich" |
| "fallo X" reference | `fallo\s+["']?([A-ZÁÉÍÓÚÑ][\wáéíóúñ]+(?:\s+[\wáéíóúñ]+)*)["']?` | "fallo Arriola" |
| Fallos citation | `Fallos:\s*(\d+):(\d+)` | "Fallos: 328:1883" |

---

## 6. Query Enricher — LLM Fallback Prompt

When rule-based extraction is insufficient (confidence < 0.6), the enricher calls GPT-4o-mini with:

```
Analyze the following query for a legal jurisprudence search system. Return a JSON object with:
- "intent": one of "search", "detail", "comparison", "statistics", "precedent_exploration", "statute_research", "general"
- "entities": object with arrays for each category found:
  - "temporal": date ranges or years mentioned (e.g., [{"from": 2020, "to": 2024}])
  - "courts": court names or abbreviations (e.g., ["CSJN", "Cámara Federal"])
  - "statutes": laws or articles (e.g., [{"statute": "CN", "articles": ["14"]}])
  - "cases": case names (e.g., ["Ekmekdjian c/ Sofovich"])
  - "topics": legal topics or keywords (e.g., ["libertad de expresión"])

Query: {query}

Respond ONLY with the JSON object, no explanation.
```

Temperature: 0. MaxOutputTokens: 200. Response format: JSON.

---

## 7. Output Guardrail — Validation Rules

### 7.1 Citation Validation Rules

| ID | Rule | Severity | Check |
|----|------|----------|-------|
| CV-01 | Ruling ID exists in database | Critical | `SELECT 1 FROM Rulings WHERE Id = @id` |
| CV-02 | Case title matches database record | Warning | Fuzzy match (Levenshtein ratio ≥ 0.85) between response title and DB `CaseTitle` |
| CV-03 | Ruling ID was returned by a tool call | Warning | `id ∈ ToolExecutionContext.ResolvedRulingIds` |
| CV-04 | Citation format is valid | Info | Regex parse succeeds, GUID is well-formed |

### 7.2 Severity Levels

| Severity | Meaning | Action (moderate strictness) |
|----------|---------|------------------------------|
| Critical | Fabricated citation — ID does not exist | Append warning to response |
| Warning | Potential issue — title mismatch or ungrounded citation | Log + append note |
| Info | Minor format issue | Log only |

### 7.3 Strictness Modes

| Mode | Critical issues | Warning issues | Info issues |
|------|:---:|:---:|:---:|
| `lenient` | Log warning | Log info | Log debug |
| `moderate` | Append warning text to response | Log warning | Log debug |
| `strict` | Append warning + strong disclaimer | Append note | Log info |

### 7.4 Warning Text Templates

**Some citations unverified** (appended when CV-01 fails for any citation):
```
---
⚠️ *Nota: Algunas referencias en esta respuesta no pudieron ser verificadas contra la base de datos de fallos. Recomendamos verificar los fallos citados accediendo directamente a sus detalles.*
```

**All citations unverified** (appended when CV-01 fails for all citations):
```
---
⚠️ *Advertencia: Las referencias citadas en esta respuesta no pudieron ser verificadas. La información puede no ser precisa. Recomendamos realizar una búsqueda directa para confirmar los fallos mencionados.*
```

### 7.5 Legal Disclaimer

Appended when the response contains substantive legal analysis (detected by: response length > 200 chars AND at least one citation present):

```
---
*Esta información es de carácter referencial y no constituye asesoramiento legal. Consultá con un profesional del derecho para tu caso particular.*
```

---

## 8. SSE Validation Event Schema

### 8.1 Event Format

```
event: validation
data: {"status":"passed|warnings","citationsChecked":N,"valid":N,"warnings":N,"details":[...]}
```

### 8.2 Status Values

| Status | Condition |
|--------|-----------|
| `passed` | All citations valid (CV-01 passed for all) |
| `warnings` | At least one citation failed CV-01, CV-02, or CV-03 |

### 8.3 Details Array

Each entry in `details` is a string describing a specific validation issue:

```json
{
  "status": "warnings",
  "citationsChecked": 3,
  "valid": 2,
  "warnings": 1,
  "details": [
    "Ruling ID a1b2c3d4-... not found in database"
  ]
}
```

---

## 9. References

- `docs/design/f1-18-chat-pipeline-architecture.md` — pipeline architecture (E232)
- `docs/design/f1-18-chat-pipeline-flow.mermaid` — pipeline sequence diagram (E233)
- `docs/design/f1-17-chat-tools-architecture.md` — agentic loop (E215)
- `docs/design/f1-17-tool-catalog.md` — tool definitions (E217)
- `ChatQueryHandler.SystemPrompt` — current legal assistant prompt
- `chat-view.component.ts` — frontend `CITATION_REGEX`
