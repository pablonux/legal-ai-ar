export interface StatuteListItem {
  id: number;
  number: string;
  name: string;
  normType: string | null;
  normativeLevel: string | null;
  legalBranch: string | null;
  issuingBody: string | null;
  sanctionDate: string | null;
  status: string | null;
  isVigente: boolean;
  rulingCount: number;
}

export interface StatutePage {
  items: StatuteListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface StatuteDetail {
  id: number;
  number: string;
  name: string;
  url: string | null;
  normType: string | null;
  normativeLevel: string | null;
  legalBranch: string | null;
  issuingBody: string | null;
  issuingBodyName: string | null;
  sanctionDate: string | null;
  publicationDate: string | null;
  effectiveFrom: string | null;
  effectiveTo: string | null;
  officialUrl: string | null;
  saijId: string | null;
  status: string | null;
  hasFullText: boolean;
  isVigente: boolean;
  rulingCount: number;
  recentRulings: StatuteRuling[];
  relations: NormRelation[];
}

export interface StatuteRuling {
  rulingId: string;
  caseTitle: string;
  rulingDate: string;
  courtName: string | null;
  articles: string | null;
}

export interface NormRelation {
  relatedStatuteId: number;
  relatedStatuteNumber: string;
  relatedStatuteName: string;
  relationType: string;
  isOutbound: boolean;
}

export interface PyramidLevel {
  level: string;
  label: string;
  count: number;
  vigenteCount: number;
}
