# Retroactive Citation Resolution Algorithm

| Field | Value |
|---|---|
| **ID** | E058 |
| **Feature** | F1-5 · Pipeline — IndexerWorker |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the algorithm for retroactive citation resolution in the IndexerWorker: matching `ExternalAlias` to newly indexed rulings, conditions for a match, and handling of ambiguous citations. It serves as the design reference for `ResolveCitationsStep` (T-08) and unit tests (T-11).

**Reference**: Architecture section 4.10 (Retroactive citation resolution); E056 (IndexerWorker steps).

---

## 1. Overview

When a ruling cites another ruling that was not yet indexed at ingestion time, `Citations.TargetRulingId` is null. When the cited ruling is later indexed, the IndexerWorker resolves these pending citations by matching `ExternalAlias` to the new ruling and setting `TargetRulingId`.

**Two directions**:

1. **Inbound**: Citations from *other* rulings that cite the *newly indexed* ruling. We find Citations where ExternalAlias matches the new ruling and set TargetRulingId.
2. **Outbound**: Citations from the *newly indexed* ruling to *other* rulings. We check if each cited ruling exists in the KB and set TargetRulingId if found.

---

## 2. Match Conditions

### 2.1 ExternalAlias Formats

CSJN and other sources use various citation formats:

| Format | Example |
|---|---|
| Fallos (volume:page) | `Fallos: 328:1883` |
| Case number | `CAF 9548/2021/CA1-CS1` |
| Short reference | `328:1883` |
| Other | Source-specific |

### 2.2 Matching Rules

| NewRuling field | ExternalAlias match condition |
|---|---|
| `CaseNumber` | Exact match (case-insensitive trim). E.g. ExternalAlias `CAF 9548/2021/CA1-CS1` matches Ruling.CaseNumber `CAF 9548/2021/CA1-CS1`. |
| Volume:Page | Parse ExternalAlias for `{volume}:{page}` pattern (e.g. `328:1883`). Match Ruling if CaseNumber or a dedicated Volume/Page field contains equivalent. *Note: Rulings table has no Volume/Page; CaseNumber may encode it. Use heuristics.* |
| Fallos format | `Fallos: 328:1883` → extract `328:1883`. Match against CaseNumber or ExternalId when source provides volume/page. |

**Decision**: Primary match is `ExternalAlias` equals `Ruling.CaseNumber` (normalized: trim, case-insensitive). Secondary: parse `Fallos: X:Y` or `X:Y` and match when Ruling has corresponding volume/page in CaseNumber or a parseable format. For Phase 1, implement exact CaseNumber match and `Fallos: X:Y` → extract `X:Y` and match against CaseNumber containing that pattern (e.g. `CAF ... 328:1883` or similar). Refinement based on real CSJN data.

### 2.3 Normalization

- Trim whitespace from both sides.
- Case-insensitive comparison.
- Ignore minor punctuation differences (e.g. `Fallos: 328:1883` vs `328:1883`) by extracting the numeric part when applicable.

---

## 3. Inbound Resolution Algorithm

```
Input: NewRuling (Id, CaseNumber, ExternalId, SourceId)
Output: Updated Citations rows

1. Build match candidates from NewRuling:
   - candidate1 = CaseNumber (trimmed, lowercased)
   - candidate2 = extract "X:Y" from CaseNumber if pattern exists
   - candidate3 = for CSJN, ExternalId might encode volume/page

2. Query: SELECT * FROM Citations
          WHERE TargetRulingId IS NULL
          AND (
            LOWER(TRIM(ExternalAlias)) = candidate1
            OR ExternalAlias LIKE '%' + candidate2 + '%'  -- when candidate2 exists
            OR LOWER(TRIM(ExternalAlias)) IN (candidate1, 'Fallos: ' + candidate2, candidate2)
          )

3. For each matching Citation:
   UPDATE Citations SET TargetRulingId = NewRuling.Id WHERE Id = Citation.Id
```

**Simplified for Phase 1**: Match when `LOWER(TRIM(ExternalAlias)) = LOWER(TRIM(Ruling.CaseNumber))` OR when ExternalAlias is `Fallos: X:Y` and we can derive X:Y from the new ruling (e.g. from CaseNumber or a known pattern). If Rulings has no Volume/Page, use CaseNumber as the primary match key. For `Fallos: 328:1883`, also try matching Citations where ExternalAlias equals `Fallos: 328:1883` against a ruling whose CaseNumber or ExternalId contains `328` and `1883`. Exact implementation may require CSJN-specific parsing.

---

## 4. Outbound Resolution Algorithm

```
Input: NewRuling.Id, Citations from message (each with externalAlias)
Output: Updated Citations rows (for citations from NewRuling)

1. For each citation in the message (NewRuling cites these):
   a. Search Rulings where CaseNumber matches citation.externalAlias
      OR ExternalAlias/volume-page matches
   b. If exactly one Ruling found: UPDATE Citations SET TargetRulingId = found.Id
      WHERE SourceRulingId = NewRuling.Id AND ExternalAlias = citation.externalAlias
   c. If zero or multiple: leave TargetRulingId = null (future resolution or ambiguous)
```

---

## 5. Ambiguous Citations

| Scenario | Behavior |
|---|---|
| **Multiple matches** | Do not update. Leave TargetRulingId = null. Log warning. |
| **No match** | Leave TargetRulingId = null. Normal for rulings not yet indexed. |
| **Single match** | Update TargetRulingId. |

**Rationale**: Updating with multiple matches would incorrectly link to one ruling. Safer to leave null and allow manual resolution or future algorithm improvement.

---

## 6. Test Cases

### 6.1 Inbound — Exact CaseNumber Match

| Given | When | Then |
|---|---|---|
| Citation(SourceRulingId=A, ExternalAlias="CAF 9548/2021/CA1-CS1", TargetRulingId=null) | NewRuling(B, CaseNumber="CAF 9548/2021/CA1-CS1") indexed | Citation.TargetRulingId = B |

### 6.2 Inbound — Fallos Format Match

| Given | When | Then |
|---|---|---|
| Citation(SourceRulingId=A, ExternalAlias="Fallos: 328:1883", TargetRulingId=null) | NewRuling(B, CaseNumber contains "328" and "1883" or equivalent) indexed | Citation.TargetRulingId = B (if match logic supports) |

*Note*: Requires definition of how CaseNumber encodes volume/page for CSJN. If no mapping, this test may not apply in Phase 1.

### 6.3 Inbound — No Match

| Given | When | Then |
|---|---|---|
| Citation(SourceRulingId=A, ExternalAlias="Fallos: 999:9999", TargetRulingId=null) | NewRuling(B, CaseNumber="CAF 9548/2021") indexed | Citation.TargetRulingId remains null |

### 6.4 Inbound — Multiple Matches (Ambiguous)

| Given | When | Then |
|---|---|---|
| Two Rulings with same CaseNumber (data anomaly) | NewRuling matches both | Do not update. Leave TargetRulingId = null. Log warning. |

### 6.5 Outbound — Target Exists

| Given | When | Then |
|---|---|---|
| NewRuling A cites "Fallos: 328:1883"; Ruling B exists with matching CaseNumber | Index NewRuling A | Citation from A: TargetRulingId = B |

### 6.6 Outbound — Target Not Yet Indexed

| Given | When | Then |
|---|---|---|
| NewRuling A cites "Fallos: 328:1883"; no Ruling matches | Index NewRuling A | Citation from A: TargetRulingId = null (remains for future resolution) |

---

## 7. Implementation Notes

### 7.1 Query Performance

- Index on `Citations.TargetRulingId` (for `WHERE TargetRulingId IS NULL`).
- Index on `Rulings.CaseNumber` for outbound lookup.

### 7.2 Phase 1 Scope

- Implement CaseNumber exact match (normalized).
- Add Fallos `X:Y` matching when Ruling has parseable volume/page (or defer to Phase 2 if schema lacks it).

---

## 8. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 4.10
- `docs/design/f1-5-indexer.md` — ResolveCitationsStep (E056)
