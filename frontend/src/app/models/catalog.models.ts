export interface CourtListItem {
  id: number;
  name: string;
  jurisdictionArea: string;
  territory: string;
  instance: string;
  rulingCount: number;
  courtCategory?: string;
  fuero?: string;
  instanceLevel?: number;
  governmentLevel?: string;
}

export interface PersonListItem {
  id: number;
  displayName: string;
  courtName: string | null;
  rulingCount: number;
}

export interface CourtHierarchyNode {
  id: number;
  name: string;
  instance: string | null;
  instanceLevel: number | null;
}

export interface JudicialOfficeItem {
  personId: number;
  personName: string;
  position: string;
  startDate: string | null;
  endDate: string | null;
  isCurrent: boolean;
}

export interface CourtDetail extends CourtListItem {
  parentCourt: CourtHierarchyNode | null;
  childCourts: CourtHierarchyNode[];
  judicialOffices: JudicialOfficeItem[];
  topPersons: PersonListItem[];
}

export interface PersonRecentRuling {
  rulingId: string;
  caseTitle: string;
  rulingDate: string;
  instance: string | null;
  rulingRole: string;
}

export interface PersonOfficeItem {
  courtId: number;
  courtName: string;
  position: string;
  startDate: string | null;
  endDate: string | null;
  isCurrent: boolean;
}

export interface PersonProceedingItem {
  proceedingId: number;
  caseNumber: string;
  displayName: string | null;
  role: string;
}

export interface PersonDetail extends PersonListItem {
  personType: string;
  legalEntityType: string | null;
  recentRulings: PersonRecentRuling[];
  judicialOffices: PersonOfficeItem[];
  proceedings: PersonProceedingItem[];
}
