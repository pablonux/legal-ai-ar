export interface OntologyClass {
  id: string;
  name: string;
  description: string;
  namespace: string;
  parentId: string | null;
  category: 'core' | 'subclass' | 'taxonomy' | 'kb-entity';
  kbEntity: string | null;
  kbRoute: string | null;
  properties: OntologyProperty[];
  children: string[];
}

export interface OntologyProperty {
  name: string;
  type: string;
  description: string;
  taxonomyId: string | null;
}

export interface OntologyClassesResponse {
  classes: OntologyClass[];
}

export interface GraphNode {
  id: string;
  label: string;
  category: 'core' | 'subclass' | 'taxonomy' | 'kb-entity';
  instanceCount: number;
  kbRoute: string | null;
}

export interface GraphEdge {
  id: string;
  source: string;
  target: string;
  type: 'is-a' | 'relationship';
  label: string;
  instanceCount: number;
}

export interface OntologyGraphResponse {
  nodes: GraphNode[];
  edges: GraphEdge[];
}

export interface EntityStats {
  classId: string;
  kbEntity: string;
  totalCount: number;
  breakdowns: TaxonomyBreakdown[];
}

export interface TaxonomyBreakdown {
  taxonomyId: string;
  taxonomyName: string;
  values: TaxonomyValueCount[];
}

export interface TaxonomyValueCount {
  code: string;
  label: string;
  count: number;
}

export interface OntologyStatsResponse {
  entities: EntityStats[];
}

export interface TaxonomyValue {
  code: string;
  label: string;
  group: string | null;
  count: number;
  description: string | null;
}

export interface TaxonomyResponse {
  id: string;
  name: string;
  description: string;
  values: TaxonomyValue[];
}
