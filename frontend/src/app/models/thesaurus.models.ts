export interface ThesaurusTerm {
  id: number;
  label: string;
  branch: string | null;
  depth: number;
}

export interface ThesaurusRelation {
  id: number;
  label: string;
}

export interface ThesaurusTermDetail extends ThesaurusTerm {
  synonyms: ThesaurusRelation[];
  broaderTerms: ThesaurusRelation[];
  narrowerTerms: ThesaurusRelation[];
  relatedTerms: ThesaurusRelation[];
}
