import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '@legal-ai-ar/core';
import type { ProceedingResponse } from '../models/proceeding.models';
import type { ProceedingPage, ProceedingDetail, AppealChain } from '../models/proceeding-space.models';

@Injectable({ providedIn: 'root' })
export class ProceedingService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/proceedings`;

  getByRuling(rulingId: string): Observable<ProceedingResponse | null> {
    return this.http.get<ProceedingResponse>(`${this.baseUrl}/by-ruling/${rulingId}`).pipe(
      catchError(() => of(null))
    );
  }

  search(opts: {
    q?: string;
    processType?: string;
    legalBranch?: string;
    courtId?: number;
    status?: string;
    page?: number;
    pageSize?: number;
  } = {}): Observable<ProceedingPage> {
    let params = new HttpParams();
    if (opts.q) params = params.set('q', opts.q);
    if (opts.processType) params = params.set('processType', opts.processType);
    if (opts.legalBranch) params = params.set('legalBranch', opts.legalBranch);
    if (opts.courtId) params = params.set('courtId', String(opts.courtId));
    if (opts.status) params = params.set('status', opts.status);
    if (opts.page) params = params.set('page', String(opts.page));
    if (opts.pageSize) params = params.set('pageSize', String(opts.pageSize));
    return this.http.get<ProceedingPage>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ProceedingDetail> {
    return this.http.get<ProceedingDetail>(`${this.baseUrl}/${id}`);
  }

  getAppealChain(id: number): Observable<AppealChain | null> {
    return this.http.get<AppealChain>(`${this.baseUrl}/${id}/appeal-chain`).pipe(
      catchError(() => of(null))
    );
  }
}
