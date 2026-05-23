# Work Item — Specialized Chat Modes

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Advanced AI Assistant

## User Story

As a **lawyer**, I want **to select a specialized mode for the assistant (research, drafting, case analysis, quick question)** so that **the AI's behavior, tone, and output format are optimized for the specific task I'm performing**.

## Context

- **Current state**: The assistant has a single mode — general RAG Q&A with citation. All queries go through the same pipeline regardless of intent.
- **Target state**: The chat interface offers mode selection (tabs or dropdown) at the top:
  - **Investigacion**: Deep research with multiple sources, detailed citations, comprehensive answers.
  - **Analisis de fallo**: Upload or select a ruling, get structured breakdown (hechos, ratio decidendi, obiter dicta, holding).
  - **Redaccion**: Assisted drafting of legal documents with proper format and citations.
  - **Consulta rapida**: Concise, direct answers for quick lookups.
  Each mode adjusts the system prompt, tool selection, and output formatting.
- **Reference**: CoCounsel skill-based modes, Harvey AI specialized tasks.
