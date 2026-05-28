# API Rulings — Endpoint Specification

| Field | Value |
|---|---|
| **ID** | E067 |
| **Feature** | F1-6 · API — Búsqueda semántica y detalle de fallos |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the three Rulings API endpoints: request/response schemas, filters, pagination, and error codes. It serves as the design reference for `RulingsController` and CQRS handlers (T-01 to T-09).

**Reference**: Architecture section 5 (API); specs section 7.

---

## 1. POST /api/rulings/search

Hybrid semantic search (vector + keyword) over the indexed rulings.

### 1.1 Request

| Method | Route | Content-Type |
|---|---|---|
| POST | `/api/rulings/search` | `application/json` |

**Body**:

```json
{
  "query": "string (required)",
  "filters": {
    "jurisdictionArea": "string (optional)",
    "instance": "string (optional)",
    "courtId": "int (optional)",
    "dateFrom": "date (optional, ISO 8601)",
    "dateTo": "date (optional, ISO 8601)",
    "keywords": ["string"] 
  },
  "page": 1,
  "pageSize": 10
}
```

| Field | Type | Required | Default | Constraints |
|---|---|---|---|---|
| query | string | Yes | — | Non-empty, max 1000 chars |
| filters | object | No | null | — |
| filters.jurisdictionArea | string | No | — | Exact or facet match |
| filters.instance | string | No | — | Exact or facet match |
| filters.courtId | int | No | — | FK to Courts |
| filters.dateFrom | date | No | — | ISO 8601 (YYYY-MM-DD) |
| filters.dateTo | date | No | — | ISO 8601 (YYYY-MM-DD) |
| filters.keywords | string[] | No | — | Any of these keywords |
| page | int | No | 1 | >= 1 |
| pageSize | int | No | 10 | 1–50 |

### 1.2 Response

**Success (200 OK)**:

```json
{
  "totalCount": 42,
  "page": 1,
  "pageSize": 10,
  "results": [
    {
      "id": "uuid",
      "caseTitle": "string",
      "summary": "string",
      "holding": "string",
      "rulingDate": "2024-03-15",
      "jurisdictionArea": "string",
      "instance": "string",
      "court": "string",
      "keywords": ["string"],
      "rulingDirection": "string",
      "relevanceScore": 0.85,
      "highlightedText": "string (optional, relevant fragment)"
    }
  ]
}
```

### 1.3 Filters

| Filter | Azure AI Search behavior |
|---|---|
| jurisdictionArea | Filter on `jurisdictionArea` (facetable) |
| instance | Filter on `instance` (facetable) |
| courtId | Filter on `court` (by court ID or name) |
| dateFrom | Filter `rulingDate >= dateFrom` |
| dateTo | Filter `rulingDate <= dateTo` |
| keywords | Filter `keywords` contains any of the values |

---

## 2. GET /api/rulings/{id}

Full ruling details including judges, keywords, citations, statutes.

### 2.1 Request

| Method | Route |
|---|---|
| GET | `/api/rulings/{id}` |

| Parameter | Type | Description |
|---|---|---|
| id | GUID | Ruling UUID |

### 2.2 Response

**Success (200 OK)**:

```json
{
  "id": "uuid",
  "sourceId": 1,
  "externalId": "string",
  "caseTitle": "string",
  "caseNumber": "string",
  "rulingDate": "2024-03-15",
  "court": {
    "id": 1,
    "name": "string",
    "jurisdictionArea": "string",
    "territory": "string",
    "instance": "string"
  },
  "jurisdictionArea": "string",
  "instance": "string",
  "jurisdiction": "string",
  "resourceType": "string",
  "rulingDirection": "string",
  "subjectArea": "string",
  "isUnconstitutional": false,
  "summary": "string",
  "holding": "string",
  "fullText": "string",
  "blobPath": "string",
  "indexedAt": "2024-03-20T10:00:00Z",
  "status": "indexed",
  "judges": [
    {
      "firstName": "string",
      "lastName": "string",
      "participationType": "SIGNATORY"
    }
  ],
  "keywords": [
    {
      "id": 1,
      "description": "string"
    }
  ],
  "statutes": [
    {
      "number": "string",
      "name": "string",
      "articles": "string",
      "url": "string"
    }
  ],
  "citations": [
    {
      "externalAlias": "string",
      "citationType": "CITES",
      "targetRulingId": "uuid | null",
      "targetCaseTitle": "string | null"
    }
  ]
}
```

**Not found (404)**:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Not Found",
  "status": 404,
  "detail": "Ruling not found.",
  "instance": "/api/rulings/{id}"
}
```

---

## 3. GET /api/rulings/{id}/related

Related rulings by semantic similarity (vector search from the ruling's embedding).

### 3.1 Request

| Method | Route |
|---|---|
| GET | `/api/rulings/{id}/related` |

| Parameter | Type | Description |
|---|---|---|
| id | GUID | Ruling UUID |

**Query parameters** (optional):

| Parameter | Type | Default | Description |
|---|---|---|---|
| limit | int | 10 | Max 1–20 |

### 3.2 Response

**Success (200 OK)**:

```json
[
  {
    "id": "uuid",
    "caseTitle": "string",
    "rulingDate": "2024-03-15",
    "jurisdictionArea": "string",
    "instance": "string",
    "similarityScore": 0.92
  }
]
```

**Not found (404)**: Same ProblemDetails as GET /api/rulings/{id}.

---

## 4. Error Codes

| HTTP | Scenario | Response |
|---|---|---|
| 400 | Bad request (validation) | ProblemDetails with `errors` array |
| 401 | Unauthorized (no/invalid token) | ProblemDetails |
| 404 | Ruling not found | ProblemDetails |
| 500 | Internal server error | ProblemDetails |

### 4.1 Validation Errors (400)

| Field | Error |
|---|---|
| query | Required, max 1000 chars |
| page | Must be >= 1 |
| pageSize | Must be between 1 and 50 |
| dateFrom | Invalid date format |
| dateTo | Invalid date format |
| dateFrom > dateTo | dateFrom must be before dateTo |

### 4.2 ProblemDetails Format (RFC 7807)

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Bad Request",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/rulings/search",
  "errors": {
    "query": ["The query field is required."],
    "pageSize": ["PageSize must be between 1 and 50."]
  }
}
```

---

## 5. Authentication

All three endpoints require authentication (Bearer token). Phase 1: all authenticated users have full access. Phase 2: `related` requires `admin` or `lawyer` role.

---

## 6. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 5
- `docs/architecture/legal-ai-ar-specs.md` — section 7
- RFC 7807 — Problem Details for HTTP APIs
