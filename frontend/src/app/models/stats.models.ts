export interface KbStats {
  totalRulings: number;
  totalCourts: number;
  totalPersons: number;
  totalKeywords: number;
  totalStatutes: number;
  totalCitations: number;
  earliestRulingDate: string | null;
  latestRulingDate: string | null;
  bySource: SourceStat[];
  byYear: YearStat[];
  topCourts: CourtStat[];
  byJurisdiction: NameCount[];
  byInstance: NameCount[];
  bySubjectArea: NameCount[];
  topKeywords: NameCount[];
  quality: QualityStats;
}

export interface SourceStat {
  sourceId: number;
  name: string;
  count: number;
}

export interface YearStat {
  year: number;
  count: number;
}

export interface CourtStat {
  courtId: number;
  name: string;
  jurisdictionArea: string;
  count: number;
}

export interface NameCount {
  name: string;
  count: number;
}

export interface QualityStats {
  withSummary: number;
  withHolding: number;
  withFullText: number;
  withKeywords: number;
  withPersons: number;
  withStatutes: number;
  withCitations: number;
}
