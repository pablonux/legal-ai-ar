# Enrichment Prompts — Judges, Statutes, Citation Type

| Field | Value |
|---|---|
| **ID** | E049 |
| **Feature** | F1-4 · Pipeline — EnrichmentWorker |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the three GPT-4o prompts used by the CSJN EnrichmentWorker: extraction of signing judges, extraction of cited statutes, and classification of citation type. Each prompt includes system instructions, input format, expected output schema, and example input/output. It serves as the design reference for T-03, T-04, T-05 and is consumed by developers implementing the EnrichmentWorker.

**Reference**: Architecture section 3.2.2 (GPT-4o for judges, statutes, citation types); E048 (Enrichment strategy).

---

## 1. Judges Extraction Prompt

### 1.1 Purpose

Extract the list of judges who signed or participated in the ruling (firmantes), with their first name, last name, and participation type (SIGNATORY, DISSENT, MAJORITY).

### 1.2 System Prompt

```
You are a legal document analyst for Argentine court rulings. Extract the list of judges who participated in the ruling. For each judge, provide firstName, lastName, and participationType. participationType must be one of: SIGNATORY (firmante), DISSENT (disidente), MAJORITY (mayoría). Use SIGNATORY for judges who signed the majority opinion. Use DISSENT for judges who wrote or joined a dissenting opinion. Use MAJORITY when the judge voted with the majority but did not sign. If uncertain, use SIGNATORY. Return only valid JSON matching the schema. Do not include explanatory text.
```

### 1.3 User Prompt (Template)

```
Extract the judges from the following Argentine court ruling text. Return a JSON object with a "judges" array. Each judge must have firstName, lastName, and participationType.

Ruling text:
---
{normalizedText}
---
```

### 1.4 Input

- `normalizedText`: Full text of the ruling (or an excerpt containing the signatures section, e.g. first 3000 characters or the paragraph before "Por ello,"). For Phase 1, use the full normalized text; optimization (excerpt) can be done later.

### 1.5 Output Schema

```json
{
  "judges": [
    {
      "firstName": "string",
      "lastName": "string",
      "participationType": "SIGNATORY | DISSENT | MAJORITY"
    }
  ]
}
```

### 1.6 Example

**Input** (excerpt):
```
... La Corte Suprema de Justicia de la Nación, en los autos "Pérez, Juan c/ Estado Nacional",
resuelve: ... Por ello, se confirma la sentencia apelada. — Lorenzetti, R. — Highton de Nolasco, E. —
Rosenkrantz, C. — Figueroa, J. C. — Kolesnik, H.
```

**Output**:
```json
{
  "judges": [
    { "firstName": "Ricardo", "lastName": "Lorenzetti", "participationType": "SIGNATORY" },
    { "firstName": "Elena", "lastName": "Highton de Nolasco", "participationType": "SIGNATORY" },
    { "firstName": "Carlos", "lastName": "Rosenkrantz", "participationType": "SIGNATORY" },
    { "firstName": "Juan Carlos", "lastName": "Figueroa", "participationType": "SIGNATORY" },
    { "firstName": "Horacio", "lastName": "Kolesnik", "participationType": "SIGNATORY" }
  ]
}
```

### 1.7 Edge Cases

| Scenario | Behavior |
|---|---|
| No judges found | Return `{ "judges": [] }` |
| Single judge | Return array with one element |
| Dissenting judge | Use `participationType: "DISSENT"` |
| Abbreviated names | Expand if possible (e.g. "R." → "Ricardo"); otherwise use as provided |

---

## 2. Statutes Extraction Prompt

### 2.1 Purpose

Extract laws, decrees, and regulations cited in the ruling, with number, name, and specific articles when mentioned.

### 2.2 System Prompt

```
You are a legal document analyst for Argentine court rulings. Extract all laws, decrees, and regulations cited in the ruling. For each statute, provide: number (e.g. "24.767", "11.683"), name (official or common name), and articles (specific articles cited, e.g. "art. 80, art. 64" or null if not specified). Use Argentine legal citation format. Return only valid JSON matching the schema. Do not include explanatory text.
```

### 2.3 User Prompt (Template)

```
Extract the laws and regulations cited in the following Argentine court ruling. Return a JSON object with a "statutes" array. Each statute must have number, name, and articles (or null).

Ruling text:
---
{normalizedText}
---
```

### 2.4 Input

- `normalizedText`: Full text of the ruling.

### 2.5 Output Schema

```json
{
  "statutes": [
    {
      "number": "string",
      "name": "string",
      "articles": "string | null"
    }
  ]
}
```

### 2.6 Example

**Input** (excerpt):
```
... en virtud de lo dispuesto por el artículo 14 de la Ley 24.767 (Ley de Concursos y Quiebras) y el artículo 8 de la Ley 11.683 (procedimiento tributario) ...
```

**Output**:
```json
{
  "statutes": [
    { "number": "24.767", "name": "Ley de Concursos y Quiebras", "articles": "art. 14" },
    { "number": "11.683", "name": "Ley 11.683 - Procedimiento tributario", "articles": "art. 8" }
  ]
}
```

### 2.7 Edge Cases

| Scenario | Behavior |
|---|---|
| No statutes found | Return `{ "statutes": [] }` |
| Articles not specified | Use `articles: null` |
| Multiple articles | Use comma-separated: `"art. 80, art. 64"` |
| Decree or resolution | Include with number and name |

---

## 3. Citation Type Classification Prompt

### 3.1 Purpose

For each citation to another ruling (alias, e.g. "Fallos: 328:1883"), classify the semantic relationship: UPHOLDS (confirma), OVERRULES (revoca), DISTINGUISHES (distingue), or CITES (cita sin modificar).

### 3.2 System Prompt

```
You are a legal document analyst for Argentine court rulings. Given a citation to another ruling and the surrounding text, classify the citation type. Types: UPHOLDS (the ruling confirms or follows the cited precedent), OVERRULES (the ruling overrules or rejects the cited precedent), DISTINGUISHES (the ruling distinguishes the case from the cited precedent), CITES (the ruling merely cites the precedent without affirming, overruling, or distinguishing). Default to CITES when uncertain. Return only valid JSON matching the schema. Do not include explanatory text.
```

### 3.3 User Prompt (Template)

```
Classify the citation type for each of the following citations based on the ruling text. Return a JSON object with a "citationTypes" array. Each element must have alias (exactly as given) and citationType (UPHOLDS, OVERRULES, DISTINGUISHES, or CITES).

Citations to classify:
{citationsJson}

Ruling text (excerpt around citations):
---
{normalizedText}
---
```

### 3.4 Input

- `extractedMetadata.citations`: Array of `{ alias, summaryId? }` from getCitas.
- `normalizedText`: Full text or an excerpt containing the paragraphs where each citation appears. For accuracy, consider providing a window of text around each citation (e.g. ±500 chars). For Phase 1, use full text; optimization later.

### 3.5 Output Schema

```json
{
  "citationTypes": [
    {
      "alias": "string",
      "citationType": "UPHOLDS | OVERRULES | DISTINGUISHES | CITES"
    }
  ]
}
```

### 3.6 Example

**Input** (citations):
```json
[
  { "alias": "Fallos: 328:1883" },
  { "alias": "Fallos: 341:1234" }
]
```

**Input** (excerpt):
```
... En consonancia con lo resuelto en Fallos: 328:1883, esta Corte ha establecido que ... Por el contrario, y a diferencia de lo sostenido en Fallos: 341:1234, en el caso sub examine ...
```

**Output**:
```json
{
  "citationTypes": [
    { "alias": "Fallos: 328:1883", "citationType": "UPHOLDS" },
    { "alias": "Fallos: 341:1234", "citationType": "DISTINGUISHES" }
  ]
}
```

### 3.7 Edge Cases

| Scenario | Behavior |
|---|---|
| No citations | Return `{ "citationTypes": [] }` |
| Citation not found in text | Use `citationType: "CITES"` (default) |
| Ambiguous context | Use `CITES` |
| Alias must match exactly | Preserve the alias from input in the output |

---

## 4. Implementation Notes

### 4.1 Token Limits

- Judges and Statutes: Input may be long. Consider truncating to first N characters (e.g. 8000) if token limit is exceeded. Preserve the signatures and citation sections.
- Citation Type: If many citations, consider batching (e.g. 10 citations per call) to stay within context limits.

### 4.2 Language

- Prompts are in English. The ruling text is in Spanish. GPT-4o handles mixed language well.
- Output field values (firstName, lastName, name) are in Spanish as they appear in the document.

### 4.3 JSON Schema for response_format

Use the schemas in section 5 of E048 (json_schema) for each prompt. Ensure `strict: true` and `required` fields are set.

---

## 5. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 3.2.2, 4.4 (RulingJudges), 4.8 (RulingStatutes), 4.9 (Citations)
- `docs/design/f1-4-enrichment.md` — Enrichment strategy, json_schema (E048)
