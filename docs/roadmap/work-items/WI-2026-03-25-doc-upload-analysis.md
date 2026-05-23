# Work Item — Document Upload & AI Analysis

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Document Analysis

## User Story

As a **lawyer**, I want **to upload a PDF document (ruling, contract, or brief) and have the AI summarize it, extract key entities, and answer questions about its content** so that **I can quickly understand and work with documents I receive outside the platform's indexed corpus**.

## Context

- **Current state**: The platform only works with rulings ingested through the crawler pipeline. Users cannot analyze their own documents.
- **Target state**: An "Upload document" feature allows the user to submit a PDF. The system extracts text (using PdfPig, already in the stack), generates a summary, extracts entities (parties, dates, courts, cited articles), and makes the document available for Q&A via the assistant. Uploaded documents are private to the user.
- **Storage**: User-uploaded documents stored in a separate Blob container, indexed in a per-user or session-scoped search index.
- **Reference**: CoCounsel document analysis, CaseMark document Q&A, StrongSuit upload & analyze.
