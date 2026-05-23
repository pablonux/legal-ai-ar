# Work Item — Word / Office Plugin

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Integrations

## User Story

As a **lawyer**, I want **to search for jurisprudence and insert citations directly from within Microsoft Word** so that **I can reference rulings while drafting briefs without switching between applications**.

## Context

- **Current state**: The platform is web-only. Lawyers must switch between Word and the browser to copy citations.
- **Target state**: A Microsoft Word add-in (Office.js / VSTO) that provides:
  - A sidebar panel within Word to search the platform's ruling index.
  - One-click insertion of formatted citations into the active document.
  - Quick ruling summary preview without leaving Word.
  - Authentication via the same credentials as the web app.
- **Technical approach**: Office Web Add-in (cross-platform: Windows, Mac, Web) using the existing API endpoints.
- **Prerequisites**: Public API (WI api-publica) or at minimum stable, documented API endpoints.
- **Reference**: Westlaw Drafting Assistant, Lexis for Microsoft Office, CoCounsel Word integration.
