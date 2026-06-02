import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';
import type { KbStats } from '../models/stats.models';

@Injectable({ providedIn: 'root' })
export class StatsService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/stats`;

  getKbStats(): Observable<KbStats> {
    return this.http.get<KbStats>(`${this.base}/kb`);
  }
}
