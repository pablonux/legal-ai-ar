# Work Item — Court Profile

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Analytics & Profiles

## User Story

As a **lawyer**, I want **to view analytics for each court including resolution times, case volume, topic distribution, and trends** so that **I can set realistic expectations for case timelines and understand each court's focus areas**.

## Context

- **Current state**: The KB dashboard shows top courts by volume, but there is no per-court analytical view.
- **Target state**: A court profile page (`/tribunales/:id`) showing:
  - **Overview**: Court name, jurisdiction, composition (judges), total rulings.
  - **Volume over time**: Timeline chart of rulings per month/year.
  - **Topic distribution**: What legal topics the court handles most.
  - **Average resolution time**: Estimated time from filing to ruling (where dates allow).
  - **Judges**: List of judges with links to their profiles.
  - **Recent rulings**: List with links.
- **Reference**: Westlaw court analytics, Lexis Context court profiles, Lex Machina court dashboards.
