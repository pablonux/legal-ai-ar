import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import type {
  OntologyClassesResponse,
  OntologyGraphResponse,
  OntologyStatsResponse,
  TaxonomyResponse
} from '../models/ontology.models';

@Injectable({ providedIn: 'root' })
export class OntologyService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/ontology`;

  getClasses(): Observable<OntologyClassesResponse> {
    return this.http.get<OntologyClassesResponse>(`${this.base}/classes`);
  }

  getGraph(): Observable<OntologyGraphResponse> {
    return this.http.get<OntologyGraphResponse>(`${this.base}/graph`);
  }

  getStats(): Observable<OntologyStatsResponse> {
    return this.http.get<OntologyStatsResponse>(`${this.base}/stats`);
  }

  getTaxonomy(taxonomyId: string): Observable<TaxonomyResponse> {
    return this.http.get<TaxonomyResponse>(`${this.base}/taxonomies/${taxonomyId}`);
  }
}
