# Work Item — Bookmarks / Favorites

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Productivity & Organization

## User Story

As a **lawyer**, I want **to bookmark rulings for quick access later** so that **I can build a personal library of frequently referenced jurisprudence without navigating through search every time**.

## Context

- **Current state**: No bookmarking or favorites functionality. Users must re-search to find previously viewed rulings.
- **Target state**: A bookmark icon on ruling cards (search results) and the ruling detail page. Bookmarked rulings appear in a "Favoritos" section accessible from the sidebar. Users can optionally add tags to bookmarks for categorization.
- **Data model**: `Bookmark` (id, userId, rulingId, tags[], createdAt).
- **Relationship to Research Folders**: Bookmarks are a lightweight, flat collection. Folders (WI research-folders) are structured containers. A ruling can be both bookmarked and in a folder.
- **Reference**: Standard feature across all legal research platforms.
