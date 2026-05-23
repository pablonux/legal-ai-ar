export type GraphEntityType = 'ruling' | 'court' | 'person' | 'statute' | 'keyword' | 'proceeding';

export interface GraphEntityNode {
  id: string;
  entityType: GraphEntityType;
  label: string;
  subtitle?: string;
  properties?: Record<string, string>;
}

export interface GraphEntityEdge {
  id: string;
  source: string;
  target: string;
  type: string;
  label?: string;
}

export interface NeighborhoodResponse {
  center: GraphEntityNode;
  nodes: GraphEntityNode[];
  edges: GraphEntityEdge[];
}

export interface EntitySearchResult {
  id: string;
  entityType: GraphEntityType;
  label: string;
  subtitle?: string;
}

export interface EntitySearchResponse {
  results: EntitySearchResult[];
}

export type GraphEdgeType = 'cites' | 'citedBy' | 'citesStatute' | 'signedBy' | 'issuedBy' | 'hasKeyword' | 'normRelation' | 'memberOf' | 'belongsTo' | 'partyOf' | 'adjudicatedAt';

export interface LayerVisibility {
  nodes: Record<GraphEntityType, boolean>;
  edges: Record<string, boolean>;
}

export const ENTITY_TYPE_CONFIG: Record<GraphEntityType, { color: string; shape: string; label: string; icon: string }> = {
  ruling:  { color: '#ea580c', shape: 'round-rectangle', label: 'Fallos', icon: 'M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z' },
  court:   { color: '#2563eb', shape: 'hexagon',         label: 'Tribunales', icon: 'M3 21h18M5 21V7l8-4v18M19 21V11l-6-4' },
  person:  { color: '#16a34a', shape: 'ellipse',          label: 'Personas', icon: 'M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2' },
  statute: { color: '#7c3aed', shape: 'round-rectangle', label: 'Normas', icon: 'M4 19.5A2.5 2.5 0 016.5 17H20' },
  keyword:    { color: '#6b7280', shape: 'round-rectangle', label: 'Keywords', icon: 'M20.59 13.41l-7.17 7.17a2 2 0 01-2.83 0L2 12V2h10l8.59 8.59a2 2 0 010 2.82z' },
  proceeding: { color: '#0891b2', shape: 'diamond',         label: 'Procesos', icon: 'M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z' },
};

export const EDGE_TYPE_CONFIG: Record<string, { color: string; style: string; label: string }> = {
  cites:        { color: '#ea580c', style: 'solid',  label: 'Citas' },
  citedBy:      { color: '#ea580c', style: 'solid',  label: 'Citado por' },
  citesStatute: { color: '#7c3aed', style: 'dashed', label: 'Normas citadas' },
  signedBy:     { color: '#16a34a', style: 'dashed', label: 'Firmantes' },
  issuedBy:     { color: '#2563eb', style: 'solid',  label: 'Tribunal' },
  hasKeyword:   { color: '#6b7280', style: 'dotted', label: 'Keywords' },
  normRelation: { color: '#7c3aed', style: 'dashed', label: 'Rel. normativa' },
  memberOf:      { color: '#16a34a', style: 'solid',  label: 'Miembro de' },
  belongsTo:     { color: '#0891b2', style: 'solid',  label: 'Pertenece a' },
  partyOf:       { color: '#0891b2', style: 'dashed', label: 'Parte en' },
  adjudicatedAt: { color: '#2563eb', style: 'dashed', label: 'Radicado en' },
};
