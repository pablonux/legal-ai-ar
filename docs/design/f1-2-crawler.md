# CSJN Crawler — Discovery Strategy and Resilience

| Field | Value |
|---|---|
| **ID** | E029 |
| **Feature** | F1-2 · Pipeline — CrawlerWorker (CSJN) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the CSJN discovery strategy for the CrawlerWorker, including Selenium-based discovery, incremental vs by-range crawl modes, throttling for rate limiting (R-002), and defensive versioning against breaking API changes (R-001). It serves as the design reference for tasks T-01 to T-10 of F1-2 and is consumed by developers implementing `CsjnCrawlerSource` and `CrawlerWorkerService`.

---

## 1. Discovery Strategy

### 1.1 Why Selenium

The CSJN web portal (`sjconsulta.csjn.gov.ar`) does not expose a working REST API for listing rulings. Pure HTTP approaches (e.g. POST to search forms) do not work reliably with the current portal. **Selenium** (or equivalent headless browser) is required for discovery.

**Reference**: Architecture section 3.2.1; validated against `legal-ai-buscar-fallos` (FallosSearchService).

### 1.2 Discovery Flow

| Step | Action | Detail |
|---|---|---|
| 1 | Navigate | `fallos/consulta.html` |
| 2 | Set date range | `fechaDesde` and `fechaHasta` (format: `dd/MM/yyyy`) in the search form |
| 3 | Submit | Click "Buscar" |
| 4 | Wait for results | After Buscar, wait for either results page (#divTotal, "Fallos: N") or error (div.alert-danger). paginarFallos only works if results page was shown (0 or N records). |
| 5 | Paginate | `fallos/paginarFallos.html?jtStartIndex={page*10}` — page size: 10 |
| 6 | Parse | Each `Record` yields `idAnalisis` (for API calls) and `Codigo` (document ID for PDF download) |

**Post-search outcomes**: (a) Results with N records → paginate; (b) Results with 0 records → paginarFallos returns empty, done; (c) Error (e.g. "máximo 5000 resultados") → throw CsjnSearchErrorException, cannot paginate.

### 1.3 Incremental vs By-Range Crawl

| Mode | `fechaDesde` | `fechaHasta` |
|---|---|---|
| **Incremental** | `LastCrawledAt` from `CrawlerConfigs` for the source, or `since` from request | Today |
| **By-Range** | `dateFrom` from request | `dateTo` from request |

The crawl message (`CrawlerMessage`) specifies `type: "incremental" | "by-range"`. For incremental, optionally `since`; the CrawlerWorker reads `CrawlerConfigs.LastCrawledAt` when `since` is not provided. For by-range, `dateFrom` and `dateTo` are required in the request.

**Reference**: Architecture section 4.14 (CrawlerConfigs).

---

## 2. Throttling — Rate Limiting Mitigation (R-002)

CSJN endpoints may apply undocumented rate limiting. Throttling reduces the risk of 429 responses or IP blocking.

### 2.1 Configuration (Phase 1 — Environment Variables Only)

| Variable | Type | Default | Description |
|---|---|---|---|
| `CsjnCrawler__ThrottlingDelayMs` | int | 2000 | Minimum delay in milliseconds between consecutive requests (discovery pages, API calls, PDF downloads). |
| `CsjnCrawler__ThrottlingBackoffMultiplier` | double | 2.0 | Multiplier for exponential backoff when a 429 or rate-limit response is received. Delay = `ThrottlingDelayMs * (Multiplier ^ attempt)`. |
| `CsjnCrawler__ThrottlingMaxRetries` | int | 3 | Maximum retries per request when rate limited. After this, the request fails and contributes to crawl failure. |

**Decision**: Phase 1 uses environment variables only. Per-source throttling (e.g. in `CrawlerConfigs`) is deferred to Phase 2.

### 2.2 Application

- **Between requests**: Wait `ThrottlingDelayMs` after each successful HTTP request (discovery pagination, API calls, PDF download) before the next one.
- **On 429 or rate-limit response**: Apply exponential backoff: wait `ThrottlingDelayMs * (ThrottlingBackoffMultiplier ^ attempt)` before retry. Retry up to `ThrottlingMaxRetries` times.
- **Scope**: Throttling applies to all CSJN-related HTTP calls within a single crawl execution (discovery + metadata + PDF retrieval).

**Reference**: R-002 (Undocumented rate limiting on CSJN endpoints).

---

## 3. Defensive Versioning — Breaking Changes (R-001)

CSJN may change response schemas without notice. The crawler must detect breaking changes and fail gracefully.

### 3.1 Response Validation

Before processing each discovery page or API response:

1. **Schema check**: Validate that required fields exist and have expected types (e.g. `idAnalisis`, `Codigo` in pagination response; expected structure in API responses).
2. **Null/empty handling**: Treat missing optional fields as null; required fields missing → schema violation.

### 3.2 Handling New or Removed Fields

| Scenario | Behavior |
|---|---|
| **New field** | Ignore. Do not fail. Use only known fields. |
| **Removed optional field** | Treat as null. Continue. |
| **Removed required field** | Schema violation → trigger failure path (see 3.3). |
| **Type change** | Schema violation → trigger failure path. |

### 3.3 Failure Path

When a schema violation or breaking change is detected:

1. **Stop the crawl** for the current source.
2. **Log** a structured error with: source ID, document/page where failure occurred, expected vs actual schema, timestamp.
3. **Move the message** to the Dead Letter Queue (`queue-crawler-dlq`).
4. **Do not** update `CrawlerConfigs.LastCrawledAt` (crawl is incomplete).
5. **Do not** publish partial results to `queue-parser`.

**Decision**: Fail fast. Do not skip pages or continue with degraded data. This ensures data integrity and forces manual investigation when the CSJN API changes.

**Reference**: R-001 (Breaking changes in CSJN API without notice).

---

## 4. Deduplication

Before publishing a document to `queue-parser`, the CrawlerWorker checks if `ContentHash` (SHA-256 of PDF) already exists in `Rulings`. If it exists, the document is skipped (not published). This is detailed in E031 (`f1-2-deduplicacion.md`).

---

## 5. End-to-End Flow Summary

```
Receive CrawlerMessage (sourceId, type, since?, dateFrom?, dateTo?)
    │
    ├─► Resolve date range (incremental: LastCrawledAt / since; by-range: dateFrom, dateTo from message)
    │
    ├─► Selenium: navigate fallos/consulta.html, set dates, click Buscar
    │
    ├─► For each page (paginarFallos.html, jtStartIndex = page*10):
    │       ├─► Validate response schema (defensive versioning)
    │       │       └─► If violation: stop, log, DLQ
    │       ├─► Throttle: wait ThrottlingDelayMs
    │       ├─► Parse idAnalisis, Codigo per Record
    │       └─► For each new document:
    │               ├─► Download PDF (with throttle)
    │               ├─► Compute ContentHash
    │               ├─► Deduplicate: if exists in Rulings → skip
    │               ├─► Upload PDF to Blob
    │               └─► Publish ParserMessage to queue-parser
    │
    └─► Update CrawlerConfigs (LastCrawledAt, LastCrawledStatus, LastDocumentCount)
```

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Throttling default 2000 ms | Conservative default to avoid rate limiting; configurable for tuning. |
| D2 | On schema violation: stop, log, DLQ | Ensures data integrity; avoids partial or corrupted ingestion. |
| D3 | Throttling via environment variables only (Phase 1) | Simpler implementation; per-source config deferred to Phase 2. |

---

## 7. Assumptions

- The CSJN portal URLs and form structure (`fallos/consulta.html`, `fallos/paginarFallos.html`) remain stable for Phase 1. If they change, the crawler will fail and require code update.
- Selenium/WebDriver is configured with headless Chrome or Chromium. The Dockerfile must include the browser (see architecture section 9).
- The pagination response format (XML or JSON with `Record`, `idAnalisis`, `Codigo`) is as documented. Schema validation will catch format changes.

---

## 8. Docker Build

The CrawlerWorker Dockerfile (`backend/src/workers/LegalAiAr.Worker.Crawler/Dockerfile`) builds a multi-stage image:

- **Build stage**: .NET 8 SDK, restores and publishes the worker.
- **Runtime stage**: .NET 8 runtime + Chromium for Selenium headless discovery.

**Build** (from `backend/`):

```bash
docker build -f src/workers/LegalAiAr.Worker.Crawler/Dockerfile -t legalaiar-crawler .
```

**Requirements**: Network access to NuGet.org during build. If `NU1301` occurs, retry or build from an environment with stable outbound HTTPS (e.g. CI).

**Runtime**: Set `CHROME_PATH=/usr/bin/chromium` (default in image). Configure connection strings and queues via environment variables per `docs/design/f0-2-environment-variables.md`.

---

## 9. References

- `docs/architecture/legal-ai-ar-architecture.md` — sections 3.2.1 (Discovery), 3.2.2 (Metadata and PDF retrieval), 3.4 (CrawlerWorker), 4.14 (CrawlerConfigs)
- `docs/architecture/legal-ai-ar-architecture.md` — Open technical risks R-001, R-002
- `docs/design/f0-2-environment-variables.md` — environment variable conventions
- `docs/design/f1-2-deduplicacion.md` — deduplication strategy (E031)
