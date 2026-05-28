# Work Item — Public API

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Integrations

## User Story

As a **law firm's IT administrator**, I want **a documented public API to integrate the platform's search and assistant capabilities into our internal systems** so that **our firm can embed legal research into our existing case management and document management workflows**.

## Context

- **Current state**: The API exists but is designed for internal frontend consumption. It has Swagger/OpenAPI docs but is not versioned, rate-limited, or documented for external use.
- **Target state**: A public-facing API with:
  - **Versioned endpoints**: `/api/v1/search`, `/api/v1/rulings/{id}`, `/api/v1/chat`, `/api/v1/stats`.
  - **API key authentication**: In addition to JWT, support API keys for server-to-server integration.
  - **Rate limiting**: Per-key rate limits to prevent abuse.
  - **Developer documentation**: Interactive API docs (Swagger UI), quick-start guide, code examples.
  - **Webhooks**: Optional webhook notifications for search alerts and new rulings.
- **Reference**: vLex API, Caseway API, Vaquill AI API access.
