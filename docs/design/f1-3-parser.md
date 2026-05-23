# CSJN Parser — Field Mapping from API Endpoints to ExtractedMetadata

| Field | Value |
|---|---|
| **ID** | E038 |
| **Feature** | F1-3 · Pipeline — ParserWorker (CSJN) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the field-by-field mapping from each CSJN REST endpoint response to the `ExtractedMetadata` structure used in `EnrichmentMessage`. It serves as the design reference for `CsjnApiParser` (T-01, T-02) and is consumed by developers implementing the ParserWorker. **HTTP to sjconsulta is performed by the Fetcher** (`CsjnSjconsultaJsonTransport.PrimeSjconsultaCachesAsync`), which writes JSON into the same blob cache keys the parser used for write-through. The Parser worker merges those cached responses into `ExtractedMetadata` when `CsjnApi:OutboundHttpEnabled` is `false` (default in worker `appsettings.json`); set `true` only if the parser must call sjconsulta directly (for example local tools without a prior fetch step).

---

## 1. Endpoint Overview

The ParserWorker receives a `ParserMessage` with `documentId` (Codigo from discovery), `analysisId` (idAnalisis from discovery), and `blobPathPdf`. In production, the **Fetcher** has already populated blob cache for the sjconsulta endpoints listed below (same URLs and `CachePathHelper.ApiCacheKey` keys as before). `CsjnApiParser` reads that JSON (and the PDF from Blob Storage) and merges into metadata before publishing to `queue-enrichment`.

| Endpoint | Parameter | Purpose |
|---|---|---|
| `GET abrirAnalisis?idAnalisis={id}` | `analysisId` | Case-level metadata |
| `GET getAllDocumentos?idAnalisis={id}` | `analysisId` | Document list (confirms PDF download code) |
| `GET getSumariosAnalisis?idAnalisis={id}` | `analysisId` | Keywords and holding |
| `GET getCitas?idDocumento={id}` | `documentId` | Citations to other rulings |
| `GET getCitantes?idDocumento={id}` | `documentId` | Rulings that cite this document |

**Reference**: Architecture section 3.2.2 (Metadata and PDF retrieval).

---

## 2. Mapping by Endpoint

### 2.1 abrirAnalisis

**URL**: `abrirAnalisis?idAnalisis={analysisId}`

**Response fields → ExtractedMetadata / ApiMetadata**

| API field (typical) | ExtractedMetadata / ApiMetadata field | Notes |
|---|---|---|
| `caseTitle` / `tituloCausa` | `caseTitle` | Official case title. Use as primary. |
| `rulingDate` / `fecha` | `rulingDate` | Date of the ruling. Parse to `DateOnly`. |
| `jurisdiction` / `jurisdiccion` | `jurisdiction` | E.g. `APELACION EXTRAORDINARIA`. CSJN-specific. |
| `resourceType` / `tipoRecurso` | `resourceType` | E.g. `RECURSO EXTRAORDINARIO FEDERAL`. |
| `rulingDirection` / `sentido` | `rulingDirection` | E.g. `UPHOLDS`, `OVERRULES`, `GRANTS`. Map CSJN values to canonical enum. |
| `subjectArea` / `materia` | `subjectArea` | E.g. `Tributario - Bancario`. |
| `isUnconstitutional` / `inconstitucional` | `isUnconstitutional` | Boolean. |
| `featuredRuling` / `falloDestacado` / `sintesis` | `summary` | Summary text. May be in a nested structure. |
| — | `court` | Not in abrirAnalisis. Derive from `instance` or fixed `CSJN` for CSJN rulings. |
| — | `jurisdictionArea` | May be derivable from `subjectArea` or `resourceType`. Use if available; otherwise leave for Enrichment. |
| — | `instance` | Typically `CSJN` for this source. |

**Rulings table mapping**: `CaseTitle`, `RulingDate`, `Jurisdiction`, `ResourceType`, `RulingDirection`, `SubjectArea`, `IsUnconstitutional`, `Summary`.

---

### 2.2 getAllDocumentos

**URL**: `getAllDocumentos?idAnalisis={analysisId}`

**Response fields → ExtractedMetadata / ApiMetadata**

| API field (typical) | ExtractedMetadata / ApiMetadata field | Notes |
|---|---|---|
| Document list with `codigo` / `id` | — | Confirms `documentId` (Codigo) for PDF download. Used to validate that the document exists. No direct mapping to ExtractedMetadata. |
| `tipoDocumento` | — | Document type. Optional for logging. |

**Purpose**: Validate that the document is available and obtain any document-level metadata if present. The PDF download uses `documentos/verDocumentoById.html?idDocumento={documentId}`. No fields map directly to `ExtractedMetadata`; this endpoint supports the pipeline flow.

---

### 2.3 getSumariosAnalisis

**URL**: `getSumariosAnalisis?idAnalisis={id}`

**Response fields → ExtractedMetadata / ApiMetadata**

| API field (typical) | ExtractedMetadata / ApiMetadata field | Notes |
|---|---|---|
| `keywords` / `palabrasClave` / `valores` | `keywords` | Array of `{ externalCode, description }`. Map to `keywords: [{ externalCode, description }]`. |
| `holding` / `considerando` / `sintesis` | `holding` | Main holding text. Prefer the primary or featured summary. |
| `summary` (if not from abrirAnalisis) | `summary` | Use if `abrirAnalisis` did not provide a summary. Merge with abrirAnalisis summary if both exist. |

**Rulings table mapping**: `Holding`, `Keywords` (via `RulingKeywords` with `ExternalCode`, `SortOrder`).

**Keywords format**: The API returns `{ codigoValor/externalCode, descripcion/description }[]`. Map to `keywords: [{ externalCode, description }]` for CSJN. The EnrichmentWorker passes these through to IndexerMessage. The architecture EnrichmentMessage schema shows `keywords: ["string"]` for non-CSJN; for CSJN the parser preserves `externalCode` for the Keywords table (CSJN thesaurus).

---

### 2.4 getCitas

**URL**: `getCitas?idDocumento={documentId}`

**Response fields → ExtractedMetadata / ApiMetadata**

| API field (typical) | ExtractedMetadata / ApiMetadata field | Notes |
|---|---|---|
| `alias` / `cita` / `referencia` | `citations[].alias` | E.g. `Fallos: 328:1883`. Preserve original format. |
| `summaryId` / `idSumario` | `citations[].summaryId` | CSJN summary ID for future linking. Nullable. |

**ExtractedMetadata format**: `citations: [{ alias: string, summaryId?: number }]`.

**Rulings table mapping**: `Citations` table: `ExternalAlias`, `CsjnSummaryId`. `CitationType` is filled by EnrichmentWorker (GPT-4o).

---

### 2.5 getCitantes

**URL**: `getCitantes?idDocumento={documentId}`

**Response fields → ExtractedMetadata / ApiMetadata**

| API field (typical) | ExtractedMetadata / ApiMetadata field | Notes |
|---|---|---|
| `analysisId` / `idAnalisis` | `citedBy[].analysisId` | Ruling that cites this document. |
| `caseNumber` / `numeroCausa` | `citedBy[].caseNumber` | E.g. `CAF 003507/2024/CS001`. |

**Purpose**: Inbound citations (rulings that cite this one). Used for graph relationships and retroactive resolution. Stored in `apiMetadata.citedBy` for reference. Not directly mapped to `ExtractedMetadata` for EnrichmentMessage — the EnrichmentWorker and IndexerWorker use this for graph construction. The parser includes it in the message payload when propagating to Enrichment; the exact structure is in ParserMessage `apiMetadata.citedBy`.

**Note**: `citedBy` is informational for the graph. The primary output for the Indexer is outbound `citations` (getCitas). `citedBy` can be passed through for future use (e.g. bidirectional graph edges).

---

## 3. Merged ExtractedMetadata Structure

After calling all five endpoints, the parser merges responses into a single structure:

| Field | Source endpoint(s) | Type |
|---|---|---|
| `caseTitle` | abrirAnalisis | string |
| `rulingDate` | abrirAnalisis | DateOnly |
| `jurisdiction` | abrirAnalisis | string |
| `resourceType` | abrirAnalisis | string |
| `rulingDirection` | abrirAnalisis | string |
| `subjectArea` | abrirAnalisis | string |
| `isUnconstitutional` | abrirAnalisis | bool |
| `summary` | abrirAnalisis, getSumariosAnalisis (fallback) | string |
| `holding` | getSumariosAnalisis | string |
| `keywords` | getSumariosAnalisis | `{ externalCode, description }[]` or `string[]` per schema |
| `citations` | getCitas | `{ alias, summaryId? }[]` |
| `citedBy` | getCitantes | `{ analysisId, caseNumber }[]` (in apiMetadata) |
| `court` | Derived (CSJN) | string |
| `jurisdictionArea` | abrirAnalisis (if available) or derived | string |
| `instance` | Derived (CSJN) | string |

---

## 4. EnrichmentMessage Output

The ParserWorker publishes `EnrichmentMessage` with:

- `extractedMetadata`: Merged structure from sections 2 and 3. The `keywords` format for EnrichmentMessage is `["string"]` per architecture; for CSJN the parser should include `externalCode` in a structure the EnrichmentWorker can pass through (e.g. a richer DTO that serializes to the IndexerMessage format).
- `normalizedText`: Full text from PDF (PdfPig + normalization).
- `missingFields`: Calculated per E041. Typically `["judges", "cited_statutes", "citation_types"]` for CSJN.

---

## 5. API Response Shape Assumptions

The CSJN sjconsulta API response structure is not fully documented. The field names above (e.g. `tituloCausa`, `codigoValor`) are typical for similar Argentine judicial APIs. Implementers must:

1. Inspect actual API responses (e.g. via browser dev tools or test fixtures).
2. Add defensive null checks and type validation.
3. Map actual field names to this specification.
4. Document any deviations in the implementation.

**Reference**: R-001 (Breaking changes in CSJN API). Apply defensive versioning as in E029.

---

## 6. References

- `docs/architecture/legal-ai-ar-architecture.md` — sections 3.2.2 (Metadata and PDF retrieval), 3.4 (ParserWorker), 8 (Message schemas)
- `docs/design/f1-3-missing-fields.md` — missingFields logic (E041)
- `docs/design/f1-3-normalizacion.md` — text normalization (E040)
