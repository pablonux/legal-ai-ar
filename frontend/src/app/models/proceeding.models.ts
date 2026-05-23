export interface ProceedingResponse {
  id: number;
  caseNumber: string;
  displayName: string | null;
  jurisdictionArea: string | null;
  rulings: ProceedingRuling[];
}

export interface ProceedingRuling {
  rulingId: string;
  caseTitle: string;
  rulingDate: string;
  courtName: string;
  instanceLevel: number | null;
  rulingDirection: string | null;
  isCurrent: boolean;
}

export interface ProsecutorOpinionResponse {
  prosecutorName: string;
  summary: string | null;
  recommendedDirection: string | null;
  agreedWithCourt: boolean;
}
