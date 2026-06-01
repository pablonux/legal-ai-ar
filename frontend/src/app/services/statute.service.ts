import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import type { StatutePage, StatuteDetail, PyramidLevel } from '../models/statute.models';

@Injectable({ providedIn: 'root' })
export class StatuteService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/statutes`;

  search(opts: {
    q?: string;
    normType?: string;
    normativeLevel?: string;
    legalBranch?: string;
    isVigente?: boolean;
    page?: number;
    pageSize?: number;
  } = {}): Observable<StatutePage> {
    let params = new HttpParams();
    if (opts.q) params = params.set('q', opts.q);
    if (opts.normType) params = params.set('normType', opts.normType);
    if (opts.normativeLevel) params = params.set('normativeLevel', opts.normativeLevel);
    if (opts.legalBranch) params = params.set('legalBranch', opts.legalBranch);
    if (opts.isVigente !== undefined) params = params.set('isVigente', String(opts.isVigente));
    if (opts.page) params = params.set('page', String(opts.page));
    if (opts.pageSize) params = params.set('pageSize', String(opts.pageSize));
    return this.http.get<StatutePage>(this.base, { params });
  }

  getById(id: number): Observable<StatuteDetail> {
    return this.http.get<StatuteDetail>(`${this.base}/${id}`);
  }

  getPyramid(): Observable<PyramidLevel[]> {
    return this.http.get<PyramidLevel[]>(`${this.base}/pyramid`);
  }
}
