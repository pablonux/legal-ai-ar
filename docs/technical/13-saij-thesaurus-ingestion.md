# SAIJ Thesaurus Ingestion

> How the **SAIJ legal thesaurus** (*Tesauro SAIJ de Derecho Argentino*) is ingested into the
> Knowledge Base and used to enrich search: a controlled vocabulary that powers synonym maps, keyword
> normalization, and query expansion.
>
> This document describes the ingestion as currently implemented. Argentine legal terms and the
> TemaTres relation codes are kept verbatim.

---

## 1. Context

The SAIJ Thesaurus is a controlled vocabulary from the *Sistema Argentino de Información Jurídica*
(Ministry of Justice). It organizes legal descriptors across thematic branches with hierarchical
(TG/TE), synonymy (USE/UP) and associative (TR) relations, and is the authoritative source of
Argentine legal terminology.

It is `Source` **Id = 6** (`EntityType.Thesaurus`). Unlike rulings, the thesaurus is **not** processed
through the six-stage queue pipeline — it is crawled directly into SQL by a dedicated job, because it
is a single bounded vocabulary fetched in one pass rather than a stream of documents.

**Data source — TemaTres API:** `http://vocabularios.saij.gob.ar/saij/services.php`, called as
`?task={task}&output=json&arg={termId}`. Four tasks are used:

| Task | Purpose |
|------|---------|
| `fetchTopTerms` | The top-level thematic branches |
| `fetchDown` | Children of a term (hierarchy descent) |
| `fetchAlt` | Alternative labels / synonyms (non-preferred terms) |
| `fetchRelated` | Associatively related terms |

---

## 2. Data model

Two entities plus a SKOS-style relation enum:

**`ThesaurusTerm`**

- `Id` (int, PK) · `ExternalId` (TemaTres `term_id`) · `Label`
- `IsPreferred` (true = accepted descriptor; false = USE redirect / synonym)
- `BranchName` (top-level thematic branch, e.g. "Derecho laboral"; null for non-preferred)
- `Depth` (0 = top term) · `CreatedAtUtc` / `UpdatedAtUtc`
- navigation: `RelationsAsSource`, `RelationsAsTarget`

**`ThesaurusRelation`** — directed edge between two terms

- `SourceTermId`, `TargetTermId`, `RelationType`, `CreatedAt`

**`ThesaurusRelationType`** (SKOS-style; mapped from the SAIJ/TemaTres codes):

| Enum | SAIJ code | Meaning |
|------|-----------|---------|
| `BT` | TG | Broader term (parent in hierarchy) |
| `NT` | TE | Narrower term (child in hierarchy) |
| `UF` | UP | Use-for (source is the preferred form of the target synonym) |
| `RT` | TR | Related term (associative) |

A separate link connects ruling **keywords** to thesaurus terms (see §5).

---

## 3. Ingestion — three-phase crawl

`SaijThesaurusCrawler` (`IThesaurusCrawler.CrawlAsync`) ingests the full vocabulary in three phases,
throttled at 150 ms between API calls and reporting progress via a callback:

**Phase 1 — Hierarchy (preferred terms).**
`fetchTopTerms` returns the branches; for each, the crawler upserts the preferred term at depth 0 and
recurses with `fetchDown`, upserting each child with increasing depth. Hierarchy edges
(child → parent) are buffered for phase 3. `SaveChanges` persists the preferred terms.

**Phase 2 — Synonyms and related terms.**
For every preferred term (`GetAllExternalIdsAsync`), `fetchAlt` collects synonyms (non-preferred
labels) and `fetchRelated` collects associative pairs. Non-preferred terms are then upserted as
`ThesaurusTerm` rows (`IsPreferred = false`). Progress is reported every 200 terms.

**Phase 3 — Relations.**
Using a full `ExternalId → DbId` map (`GetExternalIdToDbIdMapAsync`), the buffered pairs are
materialized into `ThesaurusRelation` rows: hierarchy as `BT`/`NT`, synonyms as `UF`, related as
`RT`.

The crawl is **idempotent** (upsert by `ExternalId`) and resilient (per-call retry with logging).

### Ingestion job

`StartThesaurusIngestJobCommand` / `StartThesaurusIngestJobHandler` create an `IngestionJob` row
(`EntityType.Thesaurus`, `SourceId = 6`) and run the crawl — plus optional keyword normalization — on
a thread-pool thread. A guard (`HasActiveJobAsync`) prevents concurrent thesaurus jobs. The job is
triggered from the admin surface (`StartThesaurusIngestJobRequest`).

---

## 4. Synonym maps for search

`ThesaurusSynonymMapGenerator` (`ISynonymMapGenerator.GenerateSolrRulesAsync`) turns the `UF`
relations into a **Solr-format synonym map** for Azure AI Search: all non-preferred labels are grouped
under their preferred term as equivalence lines, e.g.

```
despido, cesantía, distracto, desvinculación
cuatrerismo, abigeato, hurto de ganado
```

Labels are normalized before grouping. The generated rules are applied to the search index so a query
for a synonym also matches the preferred descriptor (and vice versa), improving recall.

---

## 5. Keyword normalization

`KeywordNormalizationService` (`IKeywordNormalizationService.ResolveAsync`) maps a ruling's free-text
keyword to a thesaurus term Id (exact / synonym / normalized-label match). The resolved term is stored
as a link between the ruling `Keyword` and the `ThesaurusTerm`, normalizing heterogeneous keyword text
onto the controlled vocabulary. It can run as part of the ingestion job (backfill) and for new
keywords.

---

## 6. Query expansion

`ThesaurusContextProvider` (`IThesaurusContextProvider.GetContextAsync`) looks up a user query in the
thesaurus and returns context — broader term, synonyms, and related terms — that is injected into the
search query preprocessing so the LLM and the keyword/semantic queries can expand a term into its
vocabulary neighborhood. For example, *"despido"* expands toward *"despido con causa"* (broader),
*"cesantía" / "distracto"* (synonyms), and *"indemnización por despido"* (related).

---

## 7. API

`ThesaurusController` exposes the vocabulary under `api/thesaurus`:

| Endpoint | Purpose |
|----------|---------|
| `GET /api/thesaurus/search?q=` | Descriptor autocomplete / search |
| `GET /api/thesaurus/roots` | Top-level branches |
| `GET /api/thesaurus/{id}/children` | Children of a term |
| `GET /api/thesaurus/{id}` | Term detail with its relations |

Responses use `ThesaurusTermDto` / `ThesaurusTermDetailDto`, served by CQRS queries
(`SearchThesaurus`, `GetThesaurusRoots`, `GetThesaurusChildren`, `GetThesaurusById`).

---

## 8. Persistence & configuration

- EF Core configurations `ThesaurusTermConfiguration` / `ThesaurusRelationConfiguration`; migrations
  add the thesaurus tables, the keyword↔thesaurus link, and the `SourceId = 6` thesaurus source.
- `ThesaurusRepository` provides the upserts and the `ExternalId → DbId` maps used by the crawler.
- The crawler's base URL and 150 ms throttle are defined in code; the broader SAIJ web crawlers
  (jurisprudence/legislation) have their own options.

---

## 9. Related documentation

- [01 — RAG & Retrieval](01-rag-retrieval.md) — how synonym maps and query expansion feed search
- [04 — Document Ingestion & Processing](04-ingestion-processing.md) — the document ingestion framework
- [14 — CSJN Ruling Ingestion](14-csjn-ruling-ingestion.md) — the rulings pipeline that consumes normalized keywords
- [Argentine Legal Ontology](../ontology/argentine-legal-ontology.md) — domain model and controlled vocabulary

---

*SAIJ Thesaurus Ingestion — Legal Ai Ar*
