# Work Item — Search by Statute Article

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Search & Research Enhancement

## User Story

As a **lawyer**, I want **to search for rulings that interpret or apply a specific article of a law (e.g. Art. 1071 Codigo Civil)** so that **I can find how courts have interpreted the exact legal provision I'm working with**.

## Context

- **Current state**: The enrichment pipeline extracts some statute references, and the chat tools include `search_by_statute`. However, there is no dedicated UI for statute-based browsing/filtering in the search page.
- **Target state**: The search page includes a "Search by statute" mode where the user selects or types a law name and article number. Results show rulings sorted by relevance that reference that provision, with the relevant passages highlighted.
- **Data dependency**: Requires robust statute extraction during enrichment (law name normalization, article number parsing). May need a curated catalog of Argentine legislation codes.
- **Reference**: vLex legislation-to-jurisprudence linking, Westlaw statute annotations.
