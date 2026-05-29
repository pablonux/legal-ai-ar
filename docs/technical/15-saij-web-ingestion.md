# SAIJ Web Ingestion (Legislation & Jurisprudence)

> How **legislation** (laws, decrees, resolutions) and **jurisprudence** (rulings from Cámaras
> Federales/Nacionales and other courts) are ingested from the **SAIJ** open data API.
>
> This document describes the ingestion as currently implemented. Class names follow the current code;
> Argentine legal terms and SAIJ API parameters are kept verbatim. See
> [13 — SAIJ Thesaurus Ingestion](13-saij-thesaurus-ingestion.md) for the separate SAIJ *thesaurus*
> (TemaTres) pipeline.

---

## 1. Context

The *Sistema Argentino de Información Jurídica* (SAIJ) exposes a public **JSON API** at
`https://api.saij.gob.ar/api/`. Two crawler sources consume it, both feeding the shared six-stage
ingestion pipeline:

| Source class | `SourceId` | Document type | Covers |
|--------------|:----------:|---------------|--------|
| `SaijLegislationCrawlerSource` | **2** | legislation | National laws, decrees, resolutions, *decisiones administrativas*, *acordadas* |
| `SaijRulingCrawlerSource` | **3** | jurisprudence | Rulings from Cámaras Federales, Nacionales and other courts |

Unlike the CSJN source (which mixes a JSON API with PDFs and needs LLM enrichment), **SAIJ content is
self-contained JSON** — the API already returns structured metadata and text, so ingestion is lighter
and needs **no AI calls**. SAIJ rulings are de-duplicated by `ExternalId` (`SaijId`) to avoid overlap
with CSJN rulings.

---

## 2. Discovery

Both sources implement `ICrawlerSource.DiscoverAsync`, paginating with `offset`/`limit` (page size 25,
1500 ms throttle) and yielding batches of `DiscoveredDocument`. They parse a `SaijSearchResult`
(`Results`, `Total`) and stop when a page returns fewer than `limit` rows or `MaxDocuments` is reached.

**Legislation search URL:**

```
documentos?tipo=legislacion&limit={limit}&offset={offset}&orden=fecha-publicacion:desc
          [&subtipo={normType}] [&fecha-desde={yyyy-MM-dd}]
```

(`subtipo` ∈ `ley`, `decreto`, `resolucion`, `decision_administrativa`, `acordada`.)

**Jurisprudence search URL:**

```
documentos?tipo=jurisprudencia&limit={limit}&offset={offset}&orden=fecha-resolucion:desc
          [&tribunal={tribunal}] [&fecha-desde={since}]
```

The document detail is fetched per id via `documentos/{documentId}`. The `DocumentType` field of the
`CrawlerMessage` carries the optional `subtipo`/`tribunal` filter; `Since` maps to `fecha-desde`.
Content hashing uses the JSON serialized as UTF-8 (no PDF download).

---

## 3. Pipeline flow

SAIJ documents traverse the same stages as any source
(Discoverer → Fetcher → Parser → Enricher → Persister → Indexer; see
[14 — CSJN Ruling Ingestion §2](14-csjn-ruling-ingestion.md)), with SAIJ-specific strategies resolved
per `SourceId` + `EntityType`:

| Stage | Legislation | Jurisprudence |
|-------|-------------|---------------|
| **Parser** | `SaijLegislationParser` — parses the legislation JSON into statute metadata + text | `SaijRulingParser` — parses the jurisprudence JSON into ruling metadata + text; dedups by `SaijId` |
| **Enricher** | `SaijLegislationEnrichStrategy` — **no AI**; builds `PersisterPayload` with `StatutePayloadData`, chunks text, uploads to blob | `SaijRulingEnrichStrategy` — **no AI**; builds `PersisterPayload` with `RulingData`, chunks text, uploads to blob |
| **Persister** | `StatutePersistStrategy` — `GetOrCreate` + update via `IStatuteRepository` | `RulingPersistStrategy` — persists the ruling graph |
| **Indexer** | `StatuteIndexStrategy` — **pass-through** for now (statutes are not yet search-indexed) | `RulingIndexStrategy` — embeddings + Azure AI Search, same as CSJN rulings |

Because the API data is structured, the Enricher skips the LLM gap-filling used for CSJN; it only
normalizes, chunks, and forwards. SAIJ rulings end up in the same `rulings-by-ruling` /
`rulings-by-chunk` indexes as CSJN rulings, so search spans both sources transparently.

---

## 4. Data model — legislation

Legislation is stored as `Statute` (rulings reuse the `Ruling` entity documented in
[doc 14 §4](14-csjn-ruling-ingestion.md)):

`Statute` — `Id`, `Number`, `Name`, `Url` / `OfficialUrl`, `SaijId`, `FullText`,
`NormType`, `NormativeLevel`, `LegalBranch`, `IssuingBody` / `IssuingOrganId`,
`SanctionDate`, `PublicationDate`, `EffectiveFrom` / `EffectiveTo`, `Status`, and a computed
`IsVigente`.

Enumerations:

- **`NormType`** — `CONSTITUTION`, `TREATY`, `LAW`, `DECREE`, `DNU`, `RESOLUTION`, `ACORDADA`, …
- **`NormativeLevel`** — position in the Argentine normative pyramid (Kelsen hierarchy; lower value = higher rank).
- **`StatuteStatus`** — `Vigente`, `ModificadaParcialmente`, `Derogada`, `VetoTotal`, `VetoParcial`.

Relations:

- **`RulingStatute`** (+ `RulingStatuteArticle`) — links a ruling to the statutes/articles it cites.
- **`NormRelation`** — directed norm-to-norm edge (`SourceStatuteId`, `TargetStatuteId`, `RelationType`, optional `EffectiveDate`), with `NormRelationType` ∈ `DEROGATES`, `AMENDS`, `REGULATES`, `COMPLEMENTS`, … — the basis for amendment/derogation chains.

---

## 5. Classification

`StatuteClassifier.ClassifyIfNeeded` is a **rule-based** classifier: when `NormType` /
`NormativeLevel` are not already set, it infers them from the norm's name/number patterns (e.g. "Ley",
"Decreto", "DNU"), placing each norm in the normative pyramid without an LLM call.

---

## 6. Configuration

| Section | Key | Default | Purpose |
|---------|-----|---------|---------|
| `SaijCrawler` | `BaseUrl` | `https://api.saij.gob.ar/api/` | SAIJ API base |
| `SaijCrawler` | `ThrottlingDelayMs` | 1500 | Min delay between API requests |
| `SaijCrawler` | `PageSize` | 25 | Pagination page size |

(Generic per-stage `Worker` queue tuning applies as for any source.)

---

## 7. Notes & current limitations

- **No LLM cost** for SAIJ ingestion — the API provides structured data, so enrichment is purely
  mechanical (mapping + chunking).
- **Statute search** is not enabled yet: `StatuteIndexStrategy` is a pass-through; statutes are
  queryable via SQL but not yet in Azure AI Search. SAIJ **rulings** are fully indexed.
- **Cross-source dedup**: SAIJ rulings check `SaijId` against existing rulings to avoid duplicating
  CSJN coverage.
- The `NormRelation` graph (derogations/amendments) is modeled but populated as the corresponding
  detail becomes available from the source.

---

## 8. Related documentation

- [14 — CSJN Ruling Ingestion](14-csjn-ruling-ingestion.md) — the rulings pipeline and shared stages
- [13 — SAIJ Thesaurus Ingestion](13-saij-thesaurus-ingestion.md) — the SAIJ controlled vocabulary
- [04 — Document Ingestion & Processing](04-ingestion-processing.md) — the cross-source framework
- [01 — RAG & Retrieval](01-rag-retrieval.md) — how indexed rulings are searched
- [Argentine Legal Ontology](../ontology/argentine-legal-ontology.md) — norms, hierarchy, and relations

---

*SAIJ Web Ingestion — Legal Ai Ar*
