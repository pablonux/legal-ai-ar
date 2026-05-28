# Work Item — Search Alerts

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Search & Research Enhancement

## User Story

As a **lawyer**, I want **to save a search query and receive notifications when new rulings matching my criteria are ingested** so that **I stay up to date on jurisprudence relevant to my active cases without manually re-running searches**.

## Context

- **Current state**: Search is stateless — the user runs a query, views results, and leaves. There is no mechanism to persist queries or notify about new matches.
- **Target state**: A "Save alert" button on search results stores the query + filters. A background job runs saved searches periodically (e.g. daily) against newly indexed rulings. When new matches are found, the user is notified via in-app notification and optionally email.
- **Dependencies**: Requires the notification system planned in Phase 3 roadmap, plus a scheduler for periodic query re-execution.
- **Reference**: Westlaw WestClip alerts, Lexis alerts, vLex search monitoring.
