import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import type { ThesaurusTerm, ThesaurusTermDetail } from '../models/thesaurus.models';

@Injectable({ providedIn: 'root' })
export class ThesaurusService {
  private readonly baseUrl = `${environment.apiUrl}/api/thesaurus`;

  constructor(private http: HttpClient) {}

  search(query: string, limit = 10): Observable<ThesaurusTerm[]> {
    const params = new HttpParams()
      .set('q', query)
      .set('limit', limit.toString());
    return this.http.get<ThesaurusTerm[]>(`${this.baseUrl}/search`, { params });
  }

  getById(id: number): Observable<ThesaurusTermDetail> {
    return this.http.get<ThesaurusTermDetail>(`${this.baseUrl}/${id}`);
  }

  getRoots(): Observable<ThesaurusTerm[]> {
    return this.http.get<ThesaurusTerm[]>(`${this.baseUrl}/roots`);
  }

  getChildren(id: number): Observable<ThesaurusTerm[]> {
    return this.http.get<ThesaurusTerm[]>(`${this.baseUrl}/${id}/children`);
  }
}
