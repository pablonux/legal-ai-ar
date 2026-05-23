# Work Item — Tabular Data Extraction

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Document Analysis

## User Story

As a **lawyer**, I want **to extract structured data from multiple rulings or documents into a table (e.g. parties, amounts, dates, outcomes, courts)** so that **I can analyze patterns across cases and export the data for reports or spreadsheets**.

## Context

- **Current state**: Ruling metadata is available individually on detail pages, but there is no way to aggregate or extract custom fields across multiple rulings into a structured table.
- **Target state**: From search results, the user selects multiple rulings and chooses "Extract to table". They pick which fields to extract (or define custom fields via natural language). The system uses AI to extract values from each ruling and presents a table that can be sorted, filtered, and exported to CSV/Excel.
- **Reference**: CaseMark tabular extraction, Caseway Synthium DataHub, traditional legal analytics data export.
