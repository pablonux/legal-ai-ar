# Work Item — Activity History

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Productivity & Organization

## User Story

As a **lawyer**, I want **to see a chronological history of my activity (searches, viewed rulings, chats, annotations)** so that **I can quickly retrace my research steps and resume work from where I left off**.

## Context

- **Current state**: No activity tracking. Each session starts fresh with no memory of past user actions.
- **Target state**: An "Activity" or "Recientes" page showing a timeline of user actions:
  - Searches performed (with query text, clickable to re-run).
  - Rulings viewed (with title, clickable to revisit).
  - Chat conversations (with preview, clickable to resume).
  - Annotations created (with snippet, clickable to jump to ruling).
  Recent items also appear in a compact widget on the welcome page for quick access.
- **Data model**: `ActivityLog` (id, userId, actionType, referenceId, metadata, timestamp). Retention policy: e.g. last 90 days.
- **Reference**: vLex recent activity, Westlaw research trail, browser history pattern.
