# Work Item — Interactive Citation Graph

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Search & Research Enhancement

## User Story

As a **lawyer**, I want **to explore an interactive graph visualization showing how rulings cite each other** so that **I can discover citation chains, identify landmark cases, and understand the network of precedents around a legal topic**.

## Context

- **Current state**: The roadmap (Phase 3) already plans Neo4j graph and interactive visualization. This WI specifies the citation-network use case.
- **Target state**: From any ruling detail page, the user can open a citation graph view showing direct and indirect citations as a node-link diagram. Nodes are colored by treatment (followed/overruled/distinguished). Users can click nodes to navigate, expand neighbors, and filter by court or date range.
- **Technology**: Neo4j for graph storage, D3.js or a similar library on the Angular frontend for interactive rendering.
- **Reference**: Vaquill AI citation tree, Lexsphere citation network, vLex visual graph.
