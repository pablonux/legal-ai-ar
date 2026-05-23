# Work Item — Citador AI

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Search & Research Enhancement

## User Story

As a **lawyer**, I want **to see whether a ruling is still valid, has been overruled, distinguished, or consistently followed by subsequent rulings** so that **I can assess the strength and current authority of the jurisprudence I rely on in my arguments**.

## Context

- **Current state**: The ruling detail page shows related rulings and citations, but does not classify the *treatment* a ruling has received (positive, negative, caution, neutral).
- **Target state**: Each ruling displays a treatment indicator (e.g. green = followed, red = overruled, yellow = distinguished) and a list of citing rulings grouped by treatment type. An AI pipeline analyzes citation context to classify treatment automatically.
- **Data dependency**: Requires the citation relationships already captured during enrichment, plus a new NLP classification step that reads the citing paragraph context.
- **Reference**: Westlaw KeyCite, Lexis Shepard's, Vaquill AI citation analysis, Lexsphere AI Citator.
