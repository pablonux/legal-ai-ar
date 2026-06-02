import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';
import type { NeighborhoodResponse, EntitySearchResponse } from '../models/graph-explorer.models';

@Injectable({ providedIn: 'root' })
export class GraphExplorerService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/graph`;

  getNeighborhood(entityType: string, entityId: string): Observable<NeighborhoodResponse> {
    return this.http.get<NeighborhoodResponse>(`${this.base}/neighborhood/${entityType}/${entityId}`);
  }

  searchEntities(query: string, types?: string): Observable<EntitySearchResponse> {
    let params = new HttpParams().set('q', query);
    if (types) params = params.set('types', types);
    return this.http.get<EntitySearchResponse>(`${this.base}/search`, { params });
  }
}
