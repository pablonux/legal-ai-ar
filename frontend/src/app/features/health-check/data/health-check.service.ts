import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';
import type { HealthCheckSummary } from './health-check.models';

@Injectable({ providedIn: 'root' })
export class HealthCheckService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/health-check`;

  list(): Observable<HealthCheckSummary[]> {
    return this.http.get<HealthCheckSummary[]>(this.baseUrl);
  }
}
