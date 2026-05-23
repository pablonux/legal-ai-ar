/**
 * API models for Rulings — match backend DTOs (camelCase JSON).
 * Reference: docs/design/f1-6-api-rulings.md, LegalAiAr.Application Rulings DTOs.
 */

export interface SearchFiltersRequest {
  jurisdictionArea?: string;
  instance?: string;
  courtId?: number;
  court?: string;
  dateFrom?: string; // ISO 8601 YYYY-MM-DD
  dateTo?: string;
  keywords?: string[];
  subjectArea?: string;
  resourceType?: string;
  isUnconstitutional?: boolean;
  courtType?: string;
  fuero?: string;
  legalBranch?: string;
  precedentWeight?: string;
}

export interface SearchRulingsRequest {
  query?: string;
  filters?: SearchFiltersRequest;
  page?: number;
  pageSize?: number;
}

export interface RulingSearchResult {
  id: string;
  caseTitle: string;
  summary?: string;
  holding?: string;
  rulingDate: string;
  jurisdictionArea?: string;
  instance?: string;
  court?: string;
  keywords: string[];
  rulingDirection?: string;
  relevanceScore: number;
  highlightedText?: string;
  subjectArea?: string;
  resourceType?: string;
  isUnconstitutional?: boolean;
  legalBranch?: string;
  precedentWeight?: string;
  isPlenario?: boolean;
  isLeadingCase?: boolean;
}

export interface SearchRulingsResult {
  totalCount: number;
  page: number;
  pageSize: number;
  results: RulingSearchResult[];
}

export interface Court {
  id: number;
  name: string;
  jurisdictionArea: string;
  territory: string;
  instance: string;
  courtCategory?: string;
  fuero?: string;
  instanceLevel?: number;
  governmentLevel?: string;
}

export interface PersonParticipation {
  personId: number;
  displayName: string;
  rulingRole: string;
}

export interface Keyword {
  id: number;
  description: string;
  thesaurusTermId?: number | null;
}

export interface Statute {
  number: string;
  name: string;
  articles?: string;
  url?: string;
  normType?: string;
  normativeLevel?: string;
  legalBranch?: string;
  issuingBody?: string;
  sanctionDate?: string;
  effectiveFrom?: string;
  effectiveTo?: string;
}

export interface Citation {
  externalAlias: string;
  citationType: string;
  targetRulingId?: string;
  targetCaseTitle?: string;
}

export interface ProsecutorOpinion {
  prosecutorName: string;
  summary?: string;
  recommendedDirection?: string;
  agreedWithCourt: boolean;
}

export interface RulingDetail {
  id: string;
  sourceId: number;
  externalId: string;
  caseTitle: string;
  caseNumber?: string;
  rulingDate: string;
  court: Court;
  jurisdictionArea?: string;
  instance?: string;
  jurisdiction?: string;
  resourceType?: string;
  rulingDirection?: string;
  subjectArea?: string;
  legalBranch?: string;
  precedentWeight?: string;
  isPlenario: boolean;
  isLeadingCase: boolean;
  isUnconstitutional: boolean;
  summary?: string;
  holding?: string;
  fullText?: string;
  blobPath?: string;
  ratioDecidendi?: string;
  doctrinaLegal?: string;
  indexedAt: string;
  status: string;
  persons: PersonParticipation[];
  keywords: Keyword[];
  statutes: Statute[];
  citations: Citation[];
  votes: Vote[];
  doctrines: LegalDoctrineItem[];
  prosecutorOpinion?: ProsecutorOpinion;
}

export interface Vote {
  id: number;
  voteType: string;
  pages?: string;
  summary?: string;
  judges: string[];
}

export interface LegalDoctrineItem {
  id: number;
  statement: string;
  topic?: string;
  isOverruled: boolean;
  overruledByRulingId?: string;
  overruledByRulingTitle?: string;
}

export interface RelatedRuling {
  id: string;
  caseTitle: string;
  rulingDate: string;
  jurisdictionArea?: string;
  instance?: string;
  similarityScore: number;
}

/**
 * Input for RulingCardComponent — works with RulingSearchResult and RelatedRuling.
 * Used in search results (relevanceScore) and related rulings (similarityScore).
 */
export interface FacetValue {
  value: string;
  count: number;
}

export interface SearchFacets {
  jurisdictionAreas: FacetValue[];
  instances: FacetValue[];
  courts: FacetValue[];
  courtTypes: FacetValue[];
  fueros: FacetValue[];
  subjectAreas: FacetValue[];
  legalBranches: FacetValue[];
  precedentWeights: FacetValue[];
  resourceTypes: FacetValue[];
}

export interface RulingCardData {
  id: string;
  caseTitle: string;
  rulingDate: string;
  jurisdictionArea?: string;
  instance?: string;
  summary?: string;
  relevanceScore?: number;
  similarityScore?: number;
  highlightedText?: string;
  subjectArea?: string;
  resourceType?: string;
  isUnconstitutional?: boolean;
  legalBranch?: string;
  precedentWeight?: string;
  isPlenario?: boolean;
  isLeadingCase?: boolean;
}
