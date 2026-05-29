> ⚠️ **Imported from the MVP — pending review.** Ported from `mvp/docs/roadmap/plans/` (the M-08
> implementation plan) to preserve its content in the new structure. Not yet aligned to current
> naming (e.g. `Ruling` → `CaseLaw`, `LegalAiAr.*`) or re-validated against the current roadmap.
> Argentine legal/thesaurus terms are kept in Spanish (domain vocabulary). **Do not treat as
> definitive until reviewed.**

# SAIJ Thesaurus Ingestion — Implementation Plan

> Originally: *M-08 — Ingesta del Tesauro SAIJ*. Branch `feature/saij-thesaurus`.

---

## 1. Context

The SAIJ Argentine Law Thesaurus is a controlled vocabulary from the *Sistema Argentino de
Información Jurídica* (Ministry of Justice). It organizes ~12,000 descriptors across 27 thematic
branches with hierarchical (TG/TE), synonymy (USE/UP), and associative (TR) relations. It is the
authoritative source of Argentine legal terminology.

**Data sources:**

- TemaTres 3.5 REST API: `http://vocabularios.saij.gob.ar/saij/services.php`
  - Supports JSON output (`&output=json`)
  - Endpoints: `fetchTopTerms`, `fetchDown`, `fetchTerm`, `fetchAlt`, `fetchRelated`, `fetchDirectTerms`, `letter`
- PDFs (backup): alphabetical list and systematic list

**Thesaurus relations** (codes are the Spanish TemaTres codes):

| Code | Meaning | Example |
|------|---------|---------|
| TG | Broader term (parent) | "abandono de trabajo" → TG: "despido con causa" |
| TE | Narrower term (child) | "aborto" → TE: "aborto no punible" |
| UP | Used-for (non-preferred synonym) | "aborto" ← UP: "delito de aborto" |
| USE | Redirect to preferred | "cuatrerismo" → USE: "abigeato" |
| TR | Related term | "aborto" → TR: "interrupción del embarazo" |
| UPAB | Synonym abbreviation | "ABL" → USE: "alumbrado, barrido y limpieza" |

---

## 2. Objectives

1. **Model the thesaurus as a first-class entity** in the KB (like Rulings, Courts, Judges).
2. **Dedicated ingestion pipeline** that consumes the TemaTres API and persists to Azure SQL.
3. **Generate synonym maps** for Azure AI Search automatically from the USE/UP relations.
4. **Enrich search**: use the thesaurus to expand queries and improve LLM preprocessing.
5. **Link ruling keywords** to thesaurus descriptors (normalization).

---

## 3. Data model

### 3.1 New entities (Azure SQL)

```
ThesaurusTerm
├── Id (int, PK, auto)
├── ExternalId (int, unique) ← TemaTres ID
├── PreferredLabel (nvarchar 500) ← preferred descriptor
├── IsPreferred (bit) ← true if descriptor, false if non-preferred
├── BranchCode (nvarchar 50, nullable) ← branch code (e.g. "01.03")
├── BranchName (nvarchar 200, nullable) ← "Derecho laboral"
├── Depth (int) ← depth in the hierarchy
├── CreatedAt (datetime2)
├── UpdatedAt (datetime2)

ThesaurusRelation
├── Id (int, PK, auto)
├── SourceTermId (int, FK → ThesaurusTerm.Id)
├── TargetTermId (int, FK → ThesaurusTerm.Id)
├── RelationType (nvarchar 10) ← 'BT' (broader), 'NT' (narrower), 'UF' (use for), 'RT' (related)
├── UNIQUE (SourceTermId, TargetTermId, RelationType)

RulingKeywordMapping (ruling ↔ thesaurus link)
├── RulingId (uniqueidentifier, FK → Ruling.Id)
├── ThesaurusTermId (int, FK → ThesaurusTerm.Id)
├── MatchType (nvarchar 20) ← 'exact', 'synonym', 'broader'
├── PK (RulingId, ThesaurusTermId)
```

### 3.2 SKOS relations → SQL

| TemaTres | RelationType | Direction |
|----------|-------------|-----------|
| TG (parent) | BT (broader term) | child → parent |
| TE (child) | NT (narrower term) | parent → child |
| UP (synonym) | UF (use for) | preferred → non-preferred |
| TR (related) | RT (related term) | bidirectional |

---

## 4. Implementation phases

### Phase 1: Data model and ingestion (effort: 3–4 days)

#### F-THES-1: Model and migration

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-00 | Design documentation | `docs/design/thesaurus-data-model.md` |
| T-01 | `ThesaurusTerm`, `ThesaurusRelation` entities in Core | `[ ] DEV` |
| T-02 | EF Core configurations and migration | `[ ] DEV` |
| T-03 | `IThesaurusRepository` + implementation | `[ ] DEV` |

#### F-THES-2: Thesaurus crawler

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-04 | `IThesaurusCrawler` interface in Core | `[ ] DEV` |
| T-05 | `SaijThesaurusCrawler` — consumes the TemaTres API as JSON | `[ ] DEV` |
| T-06 | CLI tool `LegalAiAr.IngestThesaurus` | `[ ] DEV` |

**Crawling strategy:**

1. `fetchTopTerms` → get the 27 root branches.
2. For each root, `fetchDown` recursively until the hierarchy is exhausted.
3. For each term, `fetchAlt` to get synonyms (UP).
4. For each term, `fetchRelated` to get related terms (TR).
5. Throttling: 200 ms between requests (courtesy to the SAIJ server).
6. Idempotency: upsert by `ExternalId`, `ON CONFLICT UPDATE`.

**Expected output:** ~12,000 preferred terms + ~5,000 synonyms + ~20,000 relations.

### Phase 2: Synonym maps and search (effort: 2 days)

#### F-THES-3: Synonym map generation

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-07 | Synonym-map generator from UF/RT relations | `[ ] DEV` |
| T-08 | Integrate into `SetupSearch` — create/update synonym map in Azure AI Search | `[ ] DEV` |
| T-09 | Configure index fields to use the synonym map | `[ ] DEV` |

**Azure AI Search Synonym Map format** (Solr format):

```
despido, cesantía, distracto, desvinculación
cuatrerismo, abigeato, hurto de ganado, hurto campestre
recurso extraordinario federal, REF
```

USE/UP relations produce equivalence lines. TR relations can be added as one-directional expansions:

```
libertad de expresión => libertad de expresión, libertad de prensa, derecho a la información
```

**Azure Basic tier limit:** 3 synonym maps × 5,000 rules each.

### Phase 3: Linking with rulings (effort: 2 days)

#### F-THES-4: Keyword normalization

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-10 | `KeywordNormalizationService` — maps ruling keywords to thesaurus descriptors | `[ ] DEV` |
| T-11 | Backfill tool to normalize existing keywords (~8,300 rulings) | `[ ] DEV` |
| T-12 | Integrate normalization into the IndexerWorker pipeline | `[ ] DEV` |

**Matching strategy:**

1. Exact match (keyword = PreferredLabel).
2. Synonym match (keyword = label of a UP of the descriptor).
3. Fuzzy match (Levenshtein ≤ 2 + same TG) for spelling variants.

### Phase 4: LLM-preprocessing enrichment (effort: 1 day)

#### F-THES-5: Query expansion with the thesaurus

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-13 | Update `SearchQueryPreprocessor` to consult the thesaurus before the LLM | `[ ] DEV` |
| T-14 | Inject thesaurus context into the preprocessor prompt | `[ ] DEV` |

**Improved flow:**

```
Query "despido"
  → Thesaurus lookup: broader="despido con causa", synonyms=["cesantía","distracto"], related=["indemnización por despido"]
  → LLM prompt includes this context
  → keywordQuery: "despido cesantía distracto desvinculación indemnización"
  → semanticQuery: "Jurisprudencia sobre despido laboral, incluyendo cesantía y distracto, con indemnización por despido sin justa causa"
```

### Phase 5: API and frontend (effort: 1–2 days)

#### F-THES-6: Thesaurus exposure

| Task | Description | Deliverable |
|------|-------------|-------------|
| T-15 | `GET /api/thesaurus/search?q=` — descriptor autocomplete | `[ ] DEV` |
| T-16 | `GET /api/thesaurus/{id}` — descriptor detail with relations | `[ ] DEV` |
| T-17 | Frontend: keyword autocomplete in the search filter | `[ ] DEV` |
| T-18 | Frontend: descriptor chips in ruling detail | `[ ] DEV` |

---

## 5. Summary

| Phase | Scope | Effort | Impact |
|-------|-------|--------|--------|
| 1 | Model + ingestion | 3–4 days | Foundational |
| 2 | Synonym maps | 2 days | High — direct improvement to search recall |
| 3 | Linking with rulings | 2 days | High — keyword normalization |
| 4 | LLM preprocessing | 1 day | Medium — more precise query expansion |
| 5 | API + frontend | 1–2 days | UX — thesaurus autocomplete and navigation |

**Estimated total:** 9–11 development days.

---

## 6. Risks

| Risk | Mitigation |
|------|------------|
| SAIJ API unavailable or throttled | Backup: parse downloaded PDFs. Local cache of responses. |
| 5,000-rule-per-synonym-map limit (Basic tier) | Prioritize most-used synonyms. Group by thematic branch if needed. |
| Ruling keywords don't match the thesaurus | Fuzzy match + manual review of unmapped ones |
| Thesaurus is updated (last: 2025-06-03) | TemaTres `termsSince` endpoint allows incremental ingestion |

---

*SAIJ Thesaurus Ingestion — Legal Ai Ar (imported MVP plan, pending review)*
