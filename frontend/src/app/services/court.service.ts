import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';
import type { CourtListItem, CourtDetail } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class CourtService {
  private readonly baseUrl = `${environment.apiUrl}/api/courts`;

  constructor(private http: HttpClient) {}

  search(query?: string, jurisdictionArea?: string, instance?: string, limit = 50): Observable<CourtListItem[]> {
    let params = new HttpParams();
    if (query) params = params.set('q', query);
    if (jurisdictionArea) params = params.set('jurisdictionArea', jurisdictionArea);
    if (instance) params = params.set('instance', instance);
    params = params.set('limit', limit.toString());
    return this.http.get<CourtListItem[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<CourtDetail> {
    return this.http.get<CourtDetail>(`${this.baseUrl}/${id}`);
  }
}
