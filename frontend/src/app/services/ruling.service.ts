import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@legal-ai-ar/core';
import type {
  SearchRulingsRequest,
  SearchRulingsResult,
  SearchFacets,
  RulingDetail,
  RelatedRuling
} from '../models/ruling.models';

@Injectable({ providedIn: 'root' })
export class RulingService {
  private readonly baseUrl = `${environment.apiUrl}/api/rulings`;

  constructor(private http: HttpClient) {}

  /**
   * Hybrid semantic search over indexed rulings.
   * POST /api/rulings/search
   */
  search(request: SearchRulingsRequest): Observable<SearchRulingsResult> {
    return this.http.post<SearchRulingsResult>(`${this.baseUrl}/search`, request);
  }

  /**
   * Facet values for search filter dropdowns.
   * GET /api/rulings/facets
   */
  getFacets(): Observable<SearchFacets> {
    return this.http.get<SearchFacets>(`${this.baseUrl}/facets`);
  }

  /**
   * Full ruling details by ID.
   * GET /api/rulings/{id}
   */
  getById(id: string): Observable<RulingDetail> {
    return this.http.get<RulingDetail>(`${this.baseUrl}/${id}`);
  }

  /**
   * Fetches the PDF document blob and returns an object URL for use in an iframe.
   * GET /api/rulings/{id}/document
   */
  getDocumentBlobUrl(id: string): Observable<string> {
    return this.http
      .get(`${this.baseUrl}/${id}/document`, { responseType: 'blob' })
      .pipe(map(blob => URL.createObjectURL(blob)));
  }

  /**
   * Related rulings by semantic similarity.
   * GET /api/rulings/{id}/related?limit=10
   */
  getRelated(id: string, limit = 10): Observable<RelatedRuling[]> {
    const params = new HttpParams().set('limit', limit.toString());
    return this.http.get<RelatedRuling[]>(`${this.baseUrl}/${id}/related`, {
      params
    });
  }
}
