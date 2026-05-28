# PDF Text Normalization Rules

| Field | Value |
|---|---|
| **ID** | E040 |
| **Feature** | F1-3 · Pipeline — ParserWorker (CSJN) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the rules for normalizing text extracted from PDFs via PdfPig. The normalized text is used for chunking, embeddings and full-text search. It serves as the design reference for `PdfTextExtractor` and `ITextNormalizer` (T-03, T-04) and is consumed by developers implementing the ParserWorker.

**Reference**: ADR-007 (PdfPig. Post-extraction normalization); Architecture section 3.4 (ParserWorker).

---

## 1. Rule Summary

| # | Rule | Input example | Output example |
|---|---|---|---|
| 1 | Collapse multiple spaces | `"Texto   con    espacios"` | `"Texto con espacios"` |
| 2 | Fix spaced text | `"S u p r e m a   C o r t e"` | `"Suprema Corte"` |
| 3 | Remove image headers | Text from logo/stamp images | (removed) |
| 4 | Normalize line breaks | `"Línea1\r\nLínea2"` or `"\r\r\n"` | `"Línea1\nLínea2"` |
| 5 | Collapse multiple newlines | `"Párrafo1\n\n\n\nPárrafo2"` | `"Párrafo1\n\nPárrafo2"` |
| 6 | Trim whitespace per line | `"  texto  \n  más  "` | `"texto\nmás"` |


---

## 2. Rule Details

### 2.1 Collapse Multiple Spaces

**Problem**: PdfPig and some PDF layouts produce multiple consecutive spaces between words.

**Rule**: Replace any sequence of two or more whitespace characters (space, tab, non-breaking space) with a single space.

**Implementation**: Regex `\s{2,}` → ` ` (single space).

**Exceptions**: Do not collapse newlines in this step; handle them separately (2.4, 2.5).

---

### 2.2 Fix Spaced Text

**Problem**: Some PDFs use character spacing that renders as `S u p r e m a` instead of `Suprema`. Each letter is separated by spaces. This is common in headers, logos and styled text.

**Rule**: Detect and collapse sequences of single characters separated by single spaces, where the result forms a valid word pattern.

**Pattern**: `(\w )(\w )+(\w)` — a word character, followed by one or more `(space + word char)` pairs, ending with a word char.

**Implementation**:
- Option A: Replace `(\b\w) (\w) (\w)(?:\s|$)`-like patterns recursively. Simpler: `(\w) (\w)` → `\1\2` when both are letters and the result is a known word or when the pattern repeats (e.g. every other char is space).
- Option B: For each sequence matching `(letter space letter)+`, collapse to `(letters)` when the sequence is short (e.g. ≤ 50 chars) and the result looks like a word (no digits in the middle, etc.).
- Option C (recommended): Use regex to find `\b(\w)( \w)+\b` and replace with the concatenation of the captured letters. For example: `"S u p r e m a"` → match `S u p r e m a`, capture `S u p r e m a` → output `Suprema`.

**Regex**: `\b(\w)(\s\w)+\b` — replace with the concatenation of all `\w` (first + rest). In C#: `Regex.Replace(input, @"\b(\w)(\s\w)+\b", m => m.Value.Replace(" ", ""))` or equivalent.

**Edge cases**: 
- Preserve intentional spacing: `"art. 80"` should not become `"art.80"` — the pattern `(\w)(\s\w)+` with word chars only. Numbers and punctuation may break the pattern; ensure `\w` includes only letters and digits. For legal text, `\w` = `[a-zA-Z0-9_]` — `art` and `80` could match. Refine: use `([A-Za-z])( [A-Za-z])+` for letter-only spaced text to avoid collapsing `art. 80`.
- **Refined pattern**: `\b([A-Za-z])( [A-Za-z])+([A-Za-z])\b` — only collapse when all characters in the sequence are letters. This preserves `art. 80`, `1 2 3`, etc.

---

### 2.3 Remove Image Headers

**Problem**: PDFs may contain scanned images (logos, stamps, letterheads) that PdfPig extracts as garbage or placeholder text. These appear at the top of pages or in fixed positions.

**Rule**: 
- **By position**: Strip text that appears in the first N lines of each page (e.g. first 2–5 lines) when it matches a known pattern (e.g. court name, URL, "Página X").
- **By pattern**: Remove lines that match: `^Página\s+\d+`, `^www\.`, `^https?://`, repeated court names.
- **By length**: Very short lines (e.g. 1–3 chars) at page boundaries may be artifacts. Remove only if they match known header patterns.

**Implementation**:
- Option A: Maintain a list of known header patterns (regex) and remove matching lines.
- Option B: Use PdfPig's layout analysis to detect text blocks that are images (if available). PdfPig can distinguish between text and images in some cases.
- Option C: Strip the first N lines of each page (configurable, default 2). Simple but may remove real content.

**Recommended**: Combine pattern-based removal (regex for known headers) with optional configurable line stripping. Document the patterns in configuration.

**Patterns to remove** (examples):
- `^\s*Página\s+\d+\s*$`
- `^\s*https?://[\w.-]+\s*$`
- `^\s*www\.\S+\s*$`
- Lines that are exactly the court name repeated (e.g. "CORTE SUPREMA DE JUSTICIA DE LA NACIÓN" at top of every page)

---

### 2.4 Normalize Line Breaks

**Problem**: PDFs can contain `\r\n`, `\r`, `\n`, or mixed sequences.

**Rule**: Replace all line break sequences (`\r\n`, `\r`, `\n`, `\f`, `\v`) with a single `\n` (LF).

**Implementation**: Replace `\r\n` with `\n`, then `\r` with `\n`, then `\f` and `\v` with `\n`.

---

### 2.5 Collapse Multiple Newlines

**Problem**: Excessive blank lines between paragraphs (e.g. 4–5 newlines) add noise and consume tokens.

**Rule**: Replace any sequence of 3 or more newlines with exactly 2 newlines (one blank line between paragraphs).

**Implementation**: Regex `\n{3,}` → `\n\n`.

---

### 2.6 Trim Whitespace

**Rule**: 
- Trim leading and trailing whitespace from the entire text.
- Optionally trim trailing spaces from each line (to avoid `"word  \n"`).

**Implementation**: `text.Trim()`. Per-line: `string.Join("\n", text.Split('\n').Select(l => l.TrimEnd()))`. Leading per-line trim is optional and may remove intentional indentation.

---

## 3. Order of Application

Apply rules in this order to avoid unintended interactions:

1. **Normalize line breaks** (2.4) — unify before other rules
2. **Remove image headers** (2.3) — operate on line-based structure
3. **Collapse multiple newlines** (2.5)
4. **Collapse multiple spaces** (2.1)
5. **Fix spaced text** (2.2) — after space collapse
6. **Trim whitespace** (2.6)

---

## 4. Interface

```csharp
// LegalAiAr.Core/Interfaces/Pipeline/ITextNormalizer.cs
public interface ITextNormalizer
{
    string Normalize(string rawText);
}
```

The `PdfTextExtractor` uses PdfPig to extract text, then calls `ITextNormalizer.Normalize()` before returning.

---

## 5. Edge Cases and Assumptions

| Scenario | Behavior |
|---|---|
| **Empty or null input** | Return empty string. |
| **Unicode** | Preserve. No normalization of accents or diacritics. |
| **Mixed content** | Apply rules uniformly. No source-specific branching in Phase 1. |
| **Performance** | Normalization is applied to full text per document. For large PDFs (e.g. >1MB text), consider streaming if needed. |

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Letter-only spaced text pattern | Avoids collapsing `art. 80` or numeric sequences. |
| D2 | Max 2 newlines between paragraphs | Preserves paragraph structure while reducing noise. |
| D3 | Pattern-based header removal | More precise than fixed line count; configurable. |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — ADR-007, section 3.4 (ParserWorker)
- `docs/design/f1-3-parser.md` — ExtractedMetadata, EnrichmentMessage (E038)
- PdfPig documentation: https://github.com/UglyToad/PdfPig
