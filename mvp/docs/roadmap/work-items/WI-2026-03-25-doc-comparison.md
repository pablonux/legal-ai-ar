# Work Item — Document Comparison

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Document Analysis

## User Story

As a **lawyer**, I want **to compare two documents side by side with differences highlighted** so that **I can quickly identify changes between versions of a brief, contract, or ruling**.

## Context

- **Current state**: No document comparison capability exists in the platform.
- **Target state**: A comparison view where the user selects two documents (from uploaded files or indexed rulings) and sees a side-by-side diff with additions, deletions, and modifications highlighted. The AI can optionally summarize the key differences.
- **Technical approach**: Text extraction + diff algorithm on the backend, rendered as a split-pane view on the frontend. Semantic diff (grouping related changes) is a stretch goal.
- **Reference**: Lexis+ AI clause comparison, StrongSuit contract redlining, standard legal document comparison tools.
