# Work Item — Annotations & Highlighting

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Productivity & Organization

## User Story

As a **lawyer**, I want **to highlight passages in a ruling's text and add personal annotations** so that **I can mark the most relevant sections for my case and record my analysis directly alongside the source material**.

## Context

- **Current state**: Ruling full text is displayed as read-only. No interaction beyond reading is possible.
- **Target state**: On the ruling detail page (full text view), users can:
  - Select text and highlight it (with color options: yellow, green, blue, red).
  - Add a note/annotation to any highlight.
  - View all their annotations across rulings in a dedicated "My annotations" page.
  - Filter annotations by folder, tag, or date.
  Annotations are private to the user (collaboration features extend this in a separate WI).
- **Data model**: `Annotation` (id, userId, rulingId, startOffset, endOffset, highlightColor, noteText, createdAt).
- **Reference**: Syllo annotations, Litigation Ready smart annotations, vLex highlighting.
