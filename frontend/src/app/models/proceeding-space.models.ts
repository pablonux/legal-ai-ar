export interface ProceedingListItem {
  id: number;
  caseNumber: string;
  displayName: string | null;
  jurisdictionArea: string | null;
  processType: string | null;
  legalBranch: string | null;
  status: string | null;
  courtName: string | null;
  rulingCount: number;
  firstRulingDate: string | null;
  lastRulingDate: string | null;
}

export interface ProceedingPage {
  items: ProceedingListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ProceedingDetail {
  id: number;
  caseNumber: string;
  displayName: string | null;
  jurisdictionArea: string | null;
  processType: string | null;
  processSubtype: string | null;
  legalBranch: string | null;
  status: string | null;
  courtName: string | null;
  courtId: number | null;
  rulingCount: number;
  firstRulingDate: string | null;
  lastRulingDate: string | null;
  rulings: ProceedingRulingItem[];
  parties: ProceedingPartyItem[];
  representations: LegalRepresentationItem[];
}

export interface ProceedingRulingItem {
  id: string;
  caseTitle: string;
  rulingDate: string;
  courtName: string | null;
  instance: string | null;
}

export interface ProceedingPartyItem {
  personId: number;
  personName: string;
  role: string;
}

export interface LegalRepresentationItem {
  lawyerId: number;
  lawyerName: string;
  partyId: number;
  partyName: string;
}

export interface ProceduralRemedyDto {
  id: number;
  remedyType: string;
  filingDate: string | null;
  resolutionDate: string | null;
  outcome: string | null;
  resolvingRulingId: string | null;
  resolvingRulingTitle: string | null;
  appealedRulingId: string | null;
  appealedRulingTitle: string | null;
  courtAQuoName: string | null;
  courtAdQuemName: string | null;
}

export interface AppealChainNode {
  rulingId: string;
  caseTitle: string;
  rulingDate: string;
  instance: string | null;
  courtName: string;
  remediesFromHere: ProceduralRemedyDto[];
}

export interface AppealChain {
  proceedingId: number;
  caseNumber: string;
  displayName: string | null;
  nodes: AppealChainNode[];
}
