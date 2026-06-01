import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface ValidationResult {
  status: 'passed' | 'warnings';
  citationsChecked: number;
  valid: number;
  warnings: number;
  details: string[];
}

export interface ChatStreamEvent {
  type: 'chunk' | 'tool_start' | 'tool_end' | 'validation' | 'complete' | 'error';
  text?: string;
  toolName?: string;
  resultCount?: number;
  validation?: ValidationResult;
  error?: string;
}

/**
 * ChatService consumes POST /api/chat SSE stream.
 * Uses fetch() + ReadableStream since EventSource only supports GET.
 * Supports typed SSE events: tool_start, tool_end (via `event:` field) and text chunks.
 */
@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly baseUrl = `${environment.apiUrl}/api/chat`;
  private readonly auth = inject(AuthService);

  stream(query: string, options?: { signal?: AbortSignal }): Observable<ChatStreamEvent> {
    return new Observable<ChatStreamEvent>((subscriber) => {
      const controller = new AbortController();
      const signal = options?.signal ?? controller.signal;

      const run = async () => {
        const trimmed = query.trim();
        if (!trimmed) {
          subscriber.next({ type: 'error', error: 'La consulta no puede estar vacía.' });
          subscriber.complete();
          return;
        }

        const truncated = trimmed.length > 1000 ? trimmed.slice(0, 1000) : trimmed;

        try {
          const headers: Record<string, string> = { 'Content-Type': 'application/json' };
          const fetchOptions: RequestInit = {
            method: 'POST',
            headers,
            body: JSON.stringify({ query: truncated }),
            signal,
            credentials: this.auth.usePlatformCredentials ? 'include' : 'same-origin'
          };
          const response = await fetch(this.baseUrl, fetchOptions);

          if (!response.ok) {
            await response.text();
            let errMsg = 'Error al conectar con el servidor.';
            if (response.status === 401) errMsg = 'Debe iniciar sesión para usar el chat.';
            else if (response.status === 429) errMsg = 'Demasiadas solicitudes. Espera un momento.';
            else if (response.status >= 500) errMsg = 'Error del servidor. Intenta más tarde.';
            subscriber.next({ type: 'error', error: errMsg });
            subscriber.complete();
            return;
          }

          const reader = response.body?.getReader();
          if (!reader) {
            subscriber.next({ type: 'error', error: 'No se pudo leer la respuesta.' });
            subscriber.complete();
            return;
          }

          const decoder = new TextDecoder();
          let buffer = '';
          let currentChunk: string[] = [];
          let pendingEventType: string | null = null;

          const flushChunk = () => {
            if (currentChunk.length > 0) {
              const text = currentChunk.join('\n');
              if (text) subscriber.next({ type: 'chunk', text });
              currentChunk = [];
            }
          };

          while (true) {
            const { done, value } = await reader.read();
            if (done) break;

            buffer += decoder.decode(value, { stream: true });
            const lines = buffer.split('\n');
            buffer = lines.pop() ?? '';

            for (const line of lines) {
              if (line.startsWith('event: ')) {
                flushChunk();
                pendingEventType = line.slice(7).trim();
              } else if (line.startsWith('data: ')) {
                const content = line.slice(6);

                if (pendingEventType === 'tool_start' || pendingEventType === 'tool_end') {
                  try {
                    const parsed = JSON.parse(content);
                    if (pendingEventType === 'tool_start') {
                      subscriber.next({ type: 'tool_start', toolName: parsed.tool });
                    } else {
                      subscriber.next({ type: 'tool_end', toolName: parsed.tool, resultCount: parsed.resultCount });
                    }
                  } catch { /* ignore malformed tool event */ }
                  pendingEventType = null;
                } else if (pendingEventType === 'validation') {
                  try {
                    const parsed = JSON.parse(content) as ValidationResult;
                    subscriber.next({ type: 'validation', validation: parsed });
                  } catch { /* ignore malformed validation event */ }
                  pendingEventType = null;
                } else if (content === '[DONE]') {
                  flushChunk();
                } else {
                  currentChunk.push(content);
                }
              } else if (line === '') {
                flushChunk();
                pendingEventType = null;
              }
            }
          }

          flushChunk();

          subscriber.next({ type: 'complete' });
        } catch (err) {
          if (err instanceof Error) {
            if (err.name === 'AbortError') {
              subscriber.next({ type: 'error', error: 'La conexión se interrumpió. Puedes reintentar.' });
            } else {
              subscriber.next({ type: 'error', error: err.message || 'Error de conexión.' });
            }
          } else {
            subscriber.next({ type: 'error', error: 'Error desconocido.' });
          }
        } finally {
          subscriber.complete();
        }
      };

      run();
      return () => controller.abort();
    });
  }
}
