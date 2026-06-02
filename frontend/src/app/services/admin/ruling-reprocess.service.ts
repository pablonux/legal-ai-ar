import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';

const BASE = `${environment.apiUrl}/api/admin/ruling-reprocess`;

export interface RulingReprocessRequestItem {
  id: string;
  rulingId: string;
  documentId: string;
  status: string;
  useCache: boolean;
  requestedBy: string;
  requestedAt: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  retryCount: number;
  caseTitle: string;
  externalId: string;
}

export interface RulingReprocessListResult {
  items: RulingReprocessRequestItem[];
  total: number;
}

export interface EnqueueRulingReprocessResult {
  requestId: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class RulingReprocessService {
  private http = inject(HttpClient);

  list(status?: string, page = 1, pageSize = 50): Observable<RulingReprocessListResult> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (status) params = params.set('status', status);
    return this.http.get<RulingReprocessListResult>(BASE, { params });
  }

  enqueue(rulingId: string, useCache = false): Observable<EnqueueRulingReprocessResult> {
    return this.http.post<EnqueueRulingReprocessResult>(
      `${BASE}/rulings/${rulingId}`,
      { useCache }
    );
  }

  retry(requestId: string): Observable<EnqueueRulingReprocessResult> {
    return this.http.post<EnqueueRulingReprocessResult>(`${BASE}/${requestId}/retry`, {});
  }
}
