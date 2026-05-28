# Work Item — Research Folders

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Productivity & Organization

## User Story

As a **lawyer**, I want **to organize rulings, searches, and notes into folders (projects or cases)** so that **I can keep my research structured by matter and easily find everything related to a specific case**.

## Context

- **Current state**: There is no concept of user-owned collections. Every search is independent and results are not persistable.
- **Target state**: Users can create named folders (e.g. "Caso Garcia c/ Estado Nacional"). Within a folder they can:
  - Add rulings (from search results or detail page via "Add to folder" action).
  - Add saved searches.
  - Add notes (free text).
  - View all items in the folder with sorting and filtering.
  Folders are listed in a sidebar or dedicated "My research" page.
- **Data model**: `Folder` (id, userId, name, createdAt) → `FolderItem` (id, folderId, itemType [ruling/search/note], referenceId, addedAt, note).
- **Reference**: Westlaw folders, Lexis research folders, vLex collections.
