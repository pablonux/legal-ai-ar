import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

const BASE = `${environment.apiUrl}/api/admin/dlq`;

export type DlqQueueName = 'discoverer' | 'fetcher' | 'parser' | 'enricher' | 'persister' | 'indexer' | 'crawler' | 'enrichment';

export interface DlqMessageError {
  message: string;
  type: string;
}

export interface DlqMessage {
  id: string;
  insertedOn: string;
  dequeueCount: number;
  bodyPreview: string;
  error?: DlqMessageError | null;
}

export interface DlqPeekResult {
  queue: string;
  messageCount: number;
  messages: DlqMessage[];
}

export interface RequeueResult {
  success: boolean;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class DlqService {
  readonly validQueues: DlqQueueName[] = ['discoverer', 'fetcher', 'parser', 'enricher', 'persister', 'indexer'];

  constructor(private http: HttpClient) {}

  getMessages(queue: DlqQueueName, maxMessages = 32): Observable<DlqPeekResult> {
    const params = new HttpParams()
      .set('queue', queue)
      .set('maxMessages', maxMessages.toString());
    return this.http.get<DlqPeekResult>(BASE, { params });
  }

  requeue(queue: DlqQueueName, messageId: string): Observable<RequeueResult> {
    return this.http.post<RequeueResult>(`${BASE}/${queue}/${encodeURIComponent(messageId)}/requeue`, {});
  }
}
