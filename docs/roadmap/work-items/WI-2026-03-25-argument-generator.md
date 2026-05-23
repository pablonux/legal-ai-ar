# Work Item — Legal Argument Generator

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Advanced AI Assistant

## User Story

As a **lawyer**, I want **the assistant to generate structured arguments for and against a legal position, each backed by relevant jurisprudence** so that **I can prepare more thorough briefs and anticipate opposing counsel's arguments**.

## Context

- **Current state**: The assistant answers questions and provides cited rulings, but does not structure responses as pro/con legal arguments.
- **Target state**: A dedicated "Generate arguments" action where the user describes a legal issue and position. The AI produces:
  - **Arguments in favor**: Each with supporting rulings, relevant passages, and strength assessment.
  - **Arguments against**: Counterarguments with their own supporting jurisprudence.
  - **Suggested rebuttal**: How to counter the opposing arguments.
  Output is structured, exportable, and each citation links to the ruling detail page.
- **Reference**: CoCounsel argument analysis, StrongSuit legal argument builder.
