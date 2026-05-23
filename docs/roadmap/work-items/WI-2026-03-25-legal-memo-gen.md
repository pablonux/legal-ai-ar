# Work Item — Legal Memo Generation

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Search & Research Enhancement

## User Story

As a **lawyer**, I want **to generate a structured legal research memorandum from a query or topic** so that **I can quickly produce a document with issue analysis, relevant rulings, arguments, and citations ready for use in briefs or internal review**.

## Context

- **Current state**: The RAG assistant answers questions with cited text, but outputs are conversational chat messages — not structured legal documents.
- **Target state**: A "Generate memo" action (from search results or the assistant) produces a formatted memorandum with sections: Issue, Brief Answer, Discussion (with cited rulings), and Conclusion. The memo is rendered in-app and exportable to Word/PDF.
- **AI approach**: Multi-step generation — first retrieve relevant rulings via search, then use a structured prompt to generate each memo section with proper citations.
- **Reference**: CoCounsel Deep Research, Lexis+ AI research memoranda, Harvey AI memo generation.
