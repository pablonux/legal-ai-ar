import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';

export interface GlobalSearchItem {
  entityType: string;
  id: string;
  title: string;
  subtitle: string | null;
  route: string;
}

export interface GlobalSearchResult {
  items: GlobalSearchItem[];
  totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class GlobalSearchService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/search`;

  search(query: string, maxPerEntity = 5): Observable<GlobalSearchResult> {
    const params = new HttpParams()
      .set('q', query)
      .set('maxPerEntity', String(maxPerEntity));
    return this.http.get<GlobalSearchResult>(this.baseUrl, { params });
  }
}
