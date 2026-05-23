# Work Item — Judge Profile

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Analytics & Profiles

## User Story

As a **lawyer**, I want **to view a detailed profile for each judge showing their ruling tendencies, frequent topics, dissent rate, and reversal rate** so that **I can better understand the judge assigned to my case and tailor my legal strategy accordingly**.

## Context

- **Current state**: Judges appear in ruling metadata and the dashboard shows top judges by volume, but there is no dedicated judge profile page with analytical depth.
- **Target state**: A judge profile page (`/jueces/:id`) showing:
  - **Overview**: Name, court, active period, total rulings count.
  - **Topic breakdown**: What legal topics the judge rules on most (pie/bar chart).
  - **Tendency**: Percentage of rulings favorable to plaintiff vs defendant (where classifiable).
  - **Dissent rate**: How often the judge votes in dissent vs majority.
  - **Reversal rate**: How often the judge's rulings are reversed on appeal.
  - **Timeline**: Ruling activity over time.
  - **Recent rulings**: List with links to ruling detail.
- **Data source**: Computed from existing ruling metadata + enrichment data. May require additional enrichment to classify ruling outcomes.
- **Reference**: vLex Vincent judge profiling, Lex Machina judge analytics, Westlaw Edge judge comparison.
