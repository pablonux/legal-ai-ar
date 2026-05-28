# Work Item — Advanced Export

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Productivity & Organization

## User Story

As a **lawyer**, I want **to export rulings, search results, memos, and annotations to Word and PDF with proper legal formatting** so that **I can use the platform's output directly in my briefs, filings, and client communications**.

## Context

- **Current state**: No export functionality. Users must manually copy text from the browser.
- **Target state**: Export actions available throughout the app:
  - **Ruling detail**: Export ruling as formatted Word/PDF (with metadata header, full text, and user annotations if any).
  - **Search results**: Export result list as a table (CSV/Excel) or as individual ruling summaries (Word/PDF).
  - **Chat/Memo**: Export assistant conversation or generated memo as Word/PDF.
  - **Annotations**: Export all annotations for a ruling or folder as a summary document.
  Format follows Argentine legal document conventions (A4, legal fonts, proper headers).
- **Technical approach**: Backend endpoint generates documents using a templating library (e.g. DocX for .NET for Word, a PDF library for PDF).
- **Reference**: Westlaw print/export, Lexis download, CoCounsel export.
