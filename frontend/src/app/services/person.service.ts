import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';
import type { PersonListItem, PersonDetail } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class PersonService {
  private readonly baseUrl = `${environment.apiUrl}/api/persons`;

  constructor(private http: HttpClient) {}

  /**
   * @param vista `magistrados` | `partes` | omit/`todos` — matches API query `vista`.
   */
  search(
    query?: string,
    court?: string,
    limit = 50,
    vista?: 'todos' | 'magistrados' | 'partes'
  ): Observable<PersonListItem[]> {
    let params = new HttpParams();
    if (query) params = params.set('q', query);
    if (court) params = params.set('court', court);
    if (vista && vista !== 'todos') params = params.set('vista', vista);
    params = params.set('limit', limit.toString());
    return this.http.get<PersonListItem[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<PersonDetail> {
    return this.http.get<PersonDetail>(`${this.baseUrl}/${id}`);
  }
}
