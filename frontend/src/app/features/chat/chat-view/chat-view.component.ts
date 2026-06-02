import {
  Component,
  signal,
  computed,
  inject,
  ViewChild,
  ElementRef,
  AfterViewChecked,
  effect,
  OnInit
} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChatService, ValidationResult } from '../../../services/chat.service';
import { ToolStatusChipComponent, ToolChipState } from '../tool-status-chip/tool-status-chip.component';
import { ChatMarkdownPipe } from '@legal-ai-ar/shared-common/pipes/chat-markdown.pipe';
import { OnboardingService } from '../../../services/onboarding.service';
import { Subscription } from 'rxjs';

export type MessageRole = 'user' | 'assistant';

export interface ToolEvent {
  toolName: string;
  state: ToolChipState;
  resultCount?: number;
}

export interface ChatMessage {
  role: MessageRole;
  text: string;
  state: 'streaming' | 'complete' | 'error';
  error?: string;
  tools?: ToolEvent[];
  toolTextBreak?: number;
  validation?: ValidationResult;
  copied?: boolean;
}

interface SuggestedPrompt {
  icon: string;
  title: string;
  query: string;
}

export interface RulingContext {
  rulingId: string;
  rulingTitle: string;
}

export interface DocumentDetail {
  rulingId: string;
  title: string;
  tribunal?: string;
  date?: string;
  summary?: string;
}

export interface ContextItem {
  rulingId: string;
  title: string;
}

@Component({
  selector: 'app-chat-view',
  standalone: true,
  imports: [FormsModule, ToolStatusChipComponent, ChatMarkdownPipe],
  template: `
    <div class="chat-layout">
    <div class="chat-container">
      @if (messages().length === 0) {
        <div class="welcome">
          <div class="welcome-icon" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 2L2 7l10 5 10-5-10-5z"/><path d="M2 17l10 5 10-5"/><path d="M2 12l10 5 10-5"/>
            </svg>
          </div>
          <h1 class="welcome-title">Asistente Jurisprudencial</h1>
          <p class="welcome-sub">Consultas legales con citas verificables sobre jurisprudencia argentina</p>
          <div class="suggestions">
            @for (s of suggestions; track s.title) {
              <button class="suggestion-card" (click)="useSuggestion(s.query)">
                <span class="suggestion-icon" aria-hidden="true">{{ s.icon }}</span>
                <span class="suggestion-text">{{ s.title }}</span>
              </button>
            }
          </div>
        </div>
      } @else {
        <div class="messages" #messagesEl (click)="onMessagesClick($event)">
          @for (msg of messages(); track $index) {
            <div class="msg-row" [class.msg-user]="msg.role === 'user'" [class.msg-assistant]="msg.role === 'assistant'">
              <div class="msg-avatar" aria-hidden="true">
                @if (msg.role === 'assistant') {
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M12 2L2 7l10 5 10-5-10-5z"/><path d="M2 17l10 5 10-5"/><path d="M2 12l10 5 10-5"/>
                  </svg>
                } @else {
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>
                  </svg>
                }
              </div>
              <div class="msg-content">
                <span class="msg-author">{{ msg.role === 'user' ? 'Tú' : 'Asistente' }}</span>
                @if (msg.role === 'user') {
                  <div class="msg-text">{{ msg.text }}</div>
                } @else {
                  @if (msg.state === 'error') {
                    @if (msg.text) {
                      <div class="md-body" [innerHTML]="msg.text | chatMarkdown"></div>
                    }
                    <div class="error-block">
                      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
                      <span>{{ msg.error ?? 'Error al generar la respuesta.' }}</span>
                      <button type="button" class="retry-btn" (click)="retry()">
                        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg>
                        Reintentar
                      </button>
                    </div>
                  } @else {
                    @if (msg.tools && msg.tools.length > 0) {
                      <div class="tool-chips">
                        @for (tool of msg.tools; track $index) {
                          @if (tool.state === 'running') {
                            <app-tool-status-chip [toolName]="tool.toolName" [state]="tool.state" />
                          }
                        }
                        @if (doneToolCount(msg.tools) > 0) {
                          <span class="tool-summary"
                                [class.has-results]="toolResultTotal(msg.tools) > 0"
                                [class.no-results]="toolResultTotal(msg.tools) === 0">
                            @if (toolResultTotal(msg.tools) > 0) {
                              <span class="ts-icon" aria-hidden="true">&#10003;</span>
                            } @else {
                              <span class="ts-icon" aria-hidden="true">&#9675;</span>
                            }
                            {{ toolSummaryText(msg.tools) }}
                          </span>
                        }
                      </div>
                    }
                    @if (getThinkingText(msg); as thinking) {
                      <details class="thinking-section">
                        <summary>
                          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>
                          Razonamiento del asistente
                        </summary>
                        <div class="thinking-content" [innerHTML]="thinking | chatMarkdown"></div>
                      </details>
                    }
                    <div class="md-body" [innerHTML]="getAnswerText(msg) | chatMarkdown"></div>
                    @if (msg.state === 'streaming') {
                      <span class="typing-cursor" aria-hidden="true"></span>
                    }
                    @if (msg.state === 'complete' && msg.text) {
                      <div class="msg-actions">
                        <button type="button" class="action-btn" [class.copied]="msg.copied" (click)="copyMessage(msg, $index)" [title]="msg.copied ? 'Copiado' : 'Copiar respuesta'">
                          @if (msg.copied) {
                            <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"/></svg>
                          } @else {
                            <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="9" y="9" width="13" height="13" rx="2" ry="2"/><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/></svg>
                          }
                        </button>
                      </div>
                    }
                    @if (msg.validation) {
                      <div class="validation-status" [class.passed]="msg.validation.status === 'passed'" [class.warnings]="msg.validation.status === 'warnings'">
                        @if (msg.validation.status === 'passed') {
                          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
                          <span>{{ msg.validation.valid }} citas verificadas</span>
                        } @else {
                          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
                          <span>{{ msg.validation.valid }}/{{ msg.validation.citationsChecked }} citas verificadas</span>
                        }
                      </div>
                    }
                  }
                }
              </div>
            </div>
          }
        </div>
      }

      <div class="input-area">
        @if (rulingContext()) {
          <div class="context-card">
            <div class="context-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
            </div>
            <div class="context-info">
              <span class="context-label">Consultando sobre</span>
              <span class="context-title">{{ rulingContext()!.rulingTitle }}</span>
            </div>
            <button type="button" class="context-dismiss" (click)="dismissContext()" title="Quitar contexto">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
            </button>
          </div>
        }
        <div class="input-box" [class.focused]="inputFocused()" [class.has-context]="!!rulingContext()">
          <textarea
            #textareaEl
            class="chat-textarea"
            [placeholder]="rulingContext() ? 'Preguntá sobre este fallo...' : 'Escribe tu consulta legal...'"
            [value]="inputText()"
            (input)="onInput($event)"
            (keydown)="onKeydown($event)"
            (focus)="inputFocused.set(true)"
            (blur)="inputFocused.set(false)"
            [disabled]="isStreaming()"
            rows="1"
          ></textarea>
          <div class="input-actions">
            @if (isStreaming()) {
              <button type="button" class="stop-btn" (click)="stopStreaming()" title="Detener generación">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="currentColor"><rect x="6" y="6" width="12" height="12" rx="2"/></svg>
              </button>
            } @else {
              <button type="button" class="send-btn" [class.active]="canSend()" [disabled]="!canSend()" (click)="send()" title="Enviar consulta">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="19" x2="12" y2="5"/><polyline points="5 12 12 5 19 12"/></svg>
              </button>
            }
          </div>
        </div>
        <p class="input-hint">El asistente cita fallos verificables. Shift+Enter para nueva línea.</p>
      </div>
    </div>

    <aside class="side-panel" [class.expanded]="docPanelOpen() || ctxPanelOpen()">
      <div class="side-section doc-section" [class.section-open]="docPanelOpen()">
        @if (docPanelOpen() && selectedDocument()) {
          <section class="side-card">
            <div class="side-card-header" (click)="toggleDocPanel()">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/></svg>
              <h2>Detalle del documento</h2>
              <button type="button" class="side-card-dismiss" (click)="clearSelectedDocument(); $event.stopPropagation()" title="Quitar documento">
                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
              </button>
              <svg xmlns="http://www.w3.org/2000/svg" class="collapse-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="18 15 12 9 6 15"/></svg>
            </div>
            <div class="side-card-body">
              <h3 class="doc-title">{{ selectedDocument()!.title }}</h3>
              <div class="doc-meta-grid">
                <div class="doc-meta-item">
                  <span class="doc-meta-label">Tribunal</span>
                  <span class="doc-meta-value">{{ selectedDocument()!.tribunal || '—' }}</span>
                </div>
                <div class="doc-meta-item">
                  <span class="doc-meta-label">Fecha</span>
                  <span class="doc-meta-value">{{ selectedDocument()!.date || '—' }}</span>
                </div>
              </div>
              @if (selectedDocument()!.summary) {
                <p class="doc-summary-text">{{ selectedDocument()!.summary }}</p>
              }
              <button type="button" class="doc-action-link" (click)="navigateToRuling(selectedDocument()!.rulingId)">
                <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
                Ver fallo completo
                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
              </button>
            </div>
          </section>
        } @else {
          <button type="button" class="side-pill" [class.has-data]="!!selectedDocument()" (click)="toggleDocPanel()" title="Detalle del documento">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
            @if (selectedDocument()) { <span class="pill-dot"></span> }
          </button>
        }
      </div>
      <div class="side-section ctx-section" [class.section-open]="ctxPanelOpen()">
        @if (ctxPanelOpen() && contextItems().length > 0) {
          <section class="side-card">
            <div class="side-card-header" (click)="toggleCtxPanel()">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"/><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"/></svg>
              <h2>Contexto</h2>
              <span class="side-card-count">{{ contextItems().length }}</span>
              <svg xmlns="http://www.w3.org/2000/svg" class="collapse-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="18 15 12 9 6 15"/></svg>
            </div>
            <div class="side-card-body">
              @for (item of contextItems(); track item.rulingId) {
                <div class="ctx-item">
                  <div class="ctx-item-icon" aria-hidden="true">
                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
                  </div>
                  <span class="ctx-item-title" (click)="selectDocumentFromContext(item)" title="{{ item.title }}">{{ item.title }}</span>
                  <button type="button" class="ctx-item-remove" (click)="removeContextItem(item.rulingId)" title="Quitar">
                    <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
                  </button>
                </div>
              }
            </div>
          </section>
        } @else {
          <button type="button" class="side-pill" [class.has-data]="contextItems().length > 0" (click)="toggleCtxPanel()" title="Contexto documental">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"/><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"/></svg>
            @if (contextItems().length > 0) { <span class="pill-badge">{{ contextItems().length }}</span> }
          </button>
        }
      </div>
    </aside>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
      margin: -2rem -2.5rem;
    }

    .chat-layout {
      display: flex;
      height: 100%;
    }

    .chat-container {
      display: flex;
      flex-direction: column;
      height: 100%;
      flex: 1;
      min-width: 0;
      padding: 0 1.5rem;
    }

    /* ── Welcome ── */

    .welcome {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 0.75rem;
      padding-bottom: 4rem;
      text-align: center;
    }

    .welcome-icon {
      color: var(--color-primary);
      opacity: 0.85;
      margin-bottom: 0.5rem;
    }

    .welcome-title {
      font-family: var(--font-heading);
      font-size: 1.625rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0;
    }

    .welcome-sub {
      font-size: 0.9375rem;
      color: var(--color-text-secondary);
      margin: 0 0 1.25rem 0;
      max-width: 420px;
    }

    .suggestions {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 10px;
      width: 100%;
      max-width: 540px;
    }

    .suggestion-card {
      display: flex;
      align-items: flex-start;
      gap: 10px;
      padding: 14px 16px;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      cursor: pointer;
      text-align: left;
      font-size: 0.8125rem;
      color: var(--color-text-body);
      line-height: 1.45;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .suggestion-card:hover {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 1px rgba(208, 74, 2, 0.08);
    }

    .suggestion-icon {
      font-size: 1.125rem;
      flex-shrink: 0;
      margin-top: 1px;
    }

    /* ── Messages ── */

    .messages {
      flex: 1;
      overflow-y: auto;
      padding: 1.5rem 0 1rem 0;
      scroll-behavior: smooth;
    }

    .msg-row {
      display: flex;
      gap: 16px;
      padding: 1.25rem 0;
      align-items: flex-start;
    }

    .msg-row + .msg-row {
      border-top: 1px solid var(--color-border, #e8e8e8);
    }

    .msg-avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      flex-shrink: 0;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .msg-user .msg-avatar {
      background: var(--color-bg-subtle);
      color: var(--color-text-secondary);
      border: 1px solid var(--color-border);
    }

    .msg-assistant .msg-avatar {
      background: var(--color-primary);
      color: #fff;
    }

    .msg-content {
      flex: 1;
      min-width: 0;
    }

    .msg-author {
      display: block;
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-text);
      margin-bottom: 4px;
    }

    .msg-text {
      font-size: 0.9375rem;
      line-height: 1.6;
      color: var(--color-text-body);
      white-space: pre-wrap;
      word-break: break-word;
    }

    /* ── Markdown body (container - encapsulated) ── */

    .md-body {
      display: block;
      font-size: 0.9375rem;
      line-height: 1.7;
      color: var(--color-text-body);
    }

    /*
     * innerHTML content needs ::ng-deep to pierce Angular's
     * emulated ViewEncapsulation — dynamically inserted elements
     * don't receive the _ngcontent attribute.
     */

    :host ::ng-deep .md-body :first-child { margin-top: 0; }
    :host ::ng-deep .md-body :last-child { margin-bottom: 0; }

    :host ::ng-deep .md-body p {
      margin: 0 0 0.75rem 0;
      line-height: 1.7;
    }

    :host ::ng-deep .md-body ul,
    :host ::ng-deep .md-body ol {
      margin: 0 0 0.75rem 0;
      padding-left: 1.4rem;
    }

    :host ::ng-deep .md-body li {
      margin-bottom: 0.4rem;
      line-height: 1.65;
    }

    :host ::ng-deep .md-body h1,
    :host ::ng-deep .md-body h2,
    :host ::ng-deep .md-body h3 {
      font-size: 0.9375rem;
      font-weight: 600;
      margin: 1.25rem 0 0.5rem 0;
      color: var(--color-text);
    }

    :host ::ng-deep .md-body strong { font-weight: 600; }
    :host ::ng-deep .md-body em { font-style: italic; }

    /* ── Tables ── */

    :host ::ng-deep .md-body .table-wrap {
      overflow-x: auto;
      margin: 0.75rem 0;
      border-radius: 8px;
      border: 1px solid var(--color-border);
    }

    :host ::ng-deep .md-body table {
      border-collapse: collapse;
      width: 100%;
      font-size: 0.8125rem;
      line-height: 1.5;
    }

    :host ::ng-deep .md-body th,
    :host ::ng-deep .md-body td {
      padding: 8px 12px;
      text-align: left;
      border-bottom: 1px solid var(--color-border);
    }

    :host ::ng-deep .md-body th {
      font-weight: 600;
      background: var(--color-bg-subtle);
      color: var(--color-text);
      white-space: nowrap;
    }

    :host ::ng-deep .md-body tbody tr:last-child td {
      border-bottom: none;
    }

    :host ::ng-deep .md-body tbody tr:hover td {
      background: var(--color-bg-hover, rgba(0,0,0,0.02));
    }

    /* ── Horizontal rule ── */

    :host ::ng-deep .md-body hr {
      border: none;
      border-top: 1px solid var(--color-border);
      margin: 1rem 0;
    }

    /* ── Blockquote ── */

    :host ::ng-deep .md-body blockquote {
      margin: 0.75rem 0;
      padding: 0.5rem 1rem;
      border-left: 3px solid var(--color-primary);
      background: var(--color-bg-subtle);
      color: var(--color-text-secondary);
    }

    :host ::ng-deep .md-body blockquote p { margin: 0; }

    /* ── Code ── */

    :host ::ng-deep .md-body code {
      font-family: 'Consolas', 'Monaco', monospace;
      font-size: 0.85em;
      padding: 2px 5px;
      background: var(--color-bg-subtle);
      border-radius: 4px;
    }

    :host ::ng-deep .md-body pre {
      margin: 0.75rem 0;
      padding: 12px 16px;
      background: var(--color-bg-subtle);
      border-radius: 8px;
      overflow-x: auto;
    }

    :host ::ng-deep .md-body pre code {
      padding: 0;
      background: none;
    }

    /* ── Ruling cards ── */

    :host ::ng-deep .cite-ref {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      font-size: .6875rem;
      font-weight: 600;
      min-width: 18px;
      height: 18px;
      padding: 0 4px;
      border-radius: 4px;
      background: var(--color-primary, #d04a02);
      color: #fff;
      text-decoration: none;
      cursor: pointer;
      vertical-align: super;
      line-height: 1;
      margin: 0 1px;
      transition: transform .12s, box-shadow .12s;
    }
    :host ::ng-deep .cite-ref:hover {
      transform: scale(1.15);
      box-shadow: 0 2px 8px rgba(208,74,2,.3);
    }
    :host ::ng-deep .cite-sources {
      margin-top: 16px;
      padding-top: 12px;
      border-top: 1px solid var(--color-border, #e7e5e4);
    }
    :host ::ng-deep .cite-sources-header {
      display: flex;
      align-items: center;
      gap: 6px;
      font-size: .75rem;
      font-weight: 600;
      color: var(--color-text-secondary, #78716c);
      text-transform: uppercase;
      letter-spacing: .05em;
      margin-bottom: 8px;
    }
    :host ::ng-deep .cite-source {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 12px;
      border-radius: 8px;
      border: 1px solid var(--color-border, #e7e5e4);
      background: var(--color-bg-subtle, #fafaf9);
      margin-bottom: 4px;
      text-decoration: none;
      color: var(--color-text);
      cursor: pointer;
      transition: border-color .15s, background .15s;
    }
    :host ::ng-deep .cite-source:hover {
      border-color: var(--color-primary, #d04a02);
      background: var(--color-nav-active-bg, #fef3ee);
    }
    :host ::ng-deep .cite-source .cite-num {
      display: flex;
      align-items: center;
      justify-content: center;
      min-width: 22px;
      height: 22px;
      border-radius: 6px;
      background: var(--color-primary, #d04a02);
      color: #fff;
      font-size: .6875rem;
      font-weight: 700;
      flex-shrink: 0;
    }
    :host ::ng-deep .cite-source .cite-title {
      flex: 1;
      min-width: 0;
      font-size: .8125rem;
      font-weight: 500;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    :host ::ng-deep .cite-source svg {
      flex-shrink: 0;
      opacity: 0;
      transition: opacity .15s;
    }
    :host ::ng-deep .cite-source:hover svg { opacity: 1; }

    /* ── Tools ── */

    .tool-chips {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 6px;
      margin-bottom: 10px;
    }

    .tool-summary {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 12px;
      border-radius: 8px;
      font-size: 0.75rem;
      font-weight: 500;
      line-height: 1.4;
    }

    .tool-summary.has-results {
      background: #f0fdf4;
      color: #15803d;
      border: 1px solid #bbf7d0;
    }

    .tool-summary.no-results {
      background: #f5f5f4;
      color: #78716c;
      border: 1px solid #e7e5e4;
    }

    .tool-summary .ts-icon {
      font-size: 0.75rem;
      line-height: 1;
    }

    /* ── Thinking section ── */

    .thinking-section {
      margin-bottom: 12px;
      border-radius: 8px;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      font-size: 0.8125rem;
    }

    .thinking-section summary {
      padding: 8px 12px;
      cursor: pointer;
      color: var(--color-text-secondary);
      font-weight: 500;
      display: flex;
      align-items: center;
      gap: 6px;
      user-select: none;
      list-style: none;
    }

    .thinking-section summary::-webkit-details-marker { display: none; }

    .thinking-section summary::after {
      content: '\\203A';
      margin-left: auto;
      font-size: 1rem;
      transition: transform 0.2s;
    }

    .thinking-section[open] summary::after {
      transform: rotate(90deg);
    }

    .thinking-section summary:hover {
      color: var(--color-text);
    }

    .thinking-content {
      padding: 0 12px 12px;
      color: var(--color-text-secondary);
      line-height: 1.6;
    }

    :host ::ng-deep .thinking-content p { margin: 0 0 0.5rem; }
    :host ::ng-deep .thinking-content p:last-child { margin-bottom: 0; }

    /* ── Typing cursor ── */

    .typing-cursor {
      display: inline-block;
      width: 7px;
      height: 1.1em;
      background: var(--color-primary);
      border-radius: 1px;
      margin-left: 2px;
      vertical-align: text-bottom;
      animation: blink-cursor 1s step-end infinite;
    }

    @keyframes blink-cursor {
      50% { opacity: 0; }
    }

    /* ── Actions (copy) ── */

    .msg-actions {
      display: flex;
      gap: 4px;
      margin-top: 8px;
    }

    .action-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 30px;
      height: 30px;
      border-radius: 6px;
      color: var(--color-text-secondary);
      transition: background 0.15s, color 0.15s;
    }

    .action-btn:hover {
      background: var(--color-bg-hover);
      color: var(--color-text);
    }

    .action-btn.copied {
      color: var(--color-success);
    }

    /* ── Error ── */

    .error-block {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 14px;
      background: var(--color-error-bg);
      border: 1px solid rgba(208, 74, 2, 0.2);
      border-radius: 8px;
      font-size: 0.8125rem;
      color: var(--color-error);
      margin-top: 4px;
    }

    .retry-btn {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      margin-left: auto;
      padding: 5px 12px;
      background: var(--color-bg-surface);
      color: var(--color-primary);
      border: 1px solid var(--color-primary);
      border-radius: 6px;
      font-size: 0.8125rem;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.15s;
    }

    .retry-btn:hover {
      background: var(--color-nav-active-bg);
    }

    /* ── Validation ── */

    .validation-status {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      margin-top: 10px;
      padding: 5px 12px;
      border-radius: 8px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .validation-status.passed {
      color: #15803d;
      background: #f0fdf4;
    }

    .validation-status.warnings {
      color: #b45309;
      background: #fffbeb;
    }

    /* ── Input area ── */

    .input-area {
      padding: 0.75rem 0 1rem 0;
      flex-shrink: 0;
      max-width: 680px;
      width: 100%;
      align-self: center;
    }

    .context-card {
      display: flex;
      align-items: center;
      gap: 0.625rem;
      padding: 0.625rem 0.75rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-bottom: none;
      border-radius: 12px 12px 0 0;
    }

    .context-icon {
      flex-shrink: 0;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 8px;
      background: rgba(208, 74, 2, 0.08);
      color: var(--color-primary);
    }

    .context-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
    }

    .context-label {
      font-size: 0.6875rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      text-transform: uppercase;
      letter-spacing: 0.03em;
    }

    .context-title {
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-text);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .context-dismiss {
      flex-shrink: 0;
      width: 24px;
      height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 6px;
      color: var(--color-text-secondary);
      transition: background 0.15s, color 0.15s;
      cursor: pointer;
    }

    .context-dismiss:hover {
      background: var(--color-bg-hover);
      color: var(--color-text);
    }

    .input-box {
      display: flex;
      align-items: flex-end;
      gap: 0;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: 16px;
      padding: 6px 6px 6px 18px;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .input-box.has-context {
      border-top: 1px solid var(--color-border);
      border-radius: 0 0 16px 16px;
    }

    .input-box.focused {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.08);
    }

    .input-box.has-context.focused {
      border-color: var(--color-primary);
    }

    .chat-textarea {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-family: inherit;
      font-size: 0.9375rem;
      line-height: 1.5;
      color: var(--color-text);
      resize: none;
      padding: 8px 0;
      max-height: 180px;
      overflow-y: auto;
    }

    .chat-textarea::placeholder {
      color: var(--color-text-secondary);
    }

    .chat-textarea:disabled {
      opacity: 0.6;
    }

    .input-actions {
      display: flex;
      align-items: flex-end;
      padding-bottom: 2px;
    }

    .send-btn, .stop-btn {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: background 0.15s, opacity 0.15s;
      flex-shrink: 0;
    }

    .send-btn {
      background: var(--color-text-secondary);
      color: #fff;
      opacity: 0.4;
    }

    .send-btn.active {
      background: var(--color-primary);
      opacity: 1;
      cursor: pointer;
    }

    .send-btn.active:hover {
      background: var(--color-primary-hover);
    }

    .stop-btn {
      background: var(--color-text);
      color: #fff;
    }

    .stop-btn:hover {
      opacity: 0.85;
    }

    .input-hint {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      margin: 6px 0 0 0;
      text-align: center;
    }

    /* ── Scrollbar ── */

    .messages::-webkit-scrollbar {
      width: 6px;
    }

    .messages::-webkit-scrollbar-track {
      background: transparent;
    }

    .messages::-webkit-scrollbar-thumb {
      background: var(--color-border);
      border-radius: 3px;
    }

    .messages::-webkit-scrollbar-thumb:hover {
      background: var(--color-text-secondary);
    }

    .chat-textarea::-webkit-scrollbar {
      width: 4px;
    }

    .chat-textarea::-webkit-scrollbar-thumb {
      background: var(--color-border);
      border-radius: 2px;
    }

    /* ── Side panel ── */

    .side-panel {
      display: flex;
      flex-direction: column;
      width: 48px;
      min-width: 48px;
      padding: 1.25rem 6px;
      gap: 0.5rem;
      align-items: center;
      transition: width 0.2s ease, min-width 0.2s ease, padding 0.2s ease;
      overflow: hidden;
    }

    .side-panel.expanded {
      width: var(--nav-width);
      min-width: var(--nav-width);
      padding: 1.25rem 1rem 1rem 0;
      align-items: stretch;
    }

    .side-section {
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    .side-section.section-open {
      align-items: stretch;
    }

    .doc-section.section-open { flex: 3; min-height: 0; }
    .ctx-section.section-open { flex: 1; min-height: 0; }

    /* Collapsed pills */

    .side-pill {
      width: 36px;
      height: 36px;
      border-radius: var(--radius-sm, 8px);
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      color: var(--color-text-secondary);
      cursor: default;
      position: relative;
      transition: border-color 0.15s, color 0.15s, box-shadow 0.15s;
      flex-shrink: 0;
      opacity: 0.5;
    }

    .side-pill.has-data {
      border-color: rgba(208, 74, 2, 0.3);
      color: var(--color-primary);
      cursor: pointer;
      opacity: 1;
    }

    .side-pill.has-data:hover {
      border-color: var(--color-primary);
      box-shadow: var(--shadow-sm, 0 1px 3px rgba(0,0,0,0.08));
    }

    .pill-dot {
      position: absolute;
      top: 4px;
      right: 4px;
      width: 7px;
      height: 7px;
      border-radius: 50%;
      background: var(--color-primary);
    }

    .pill-badge {
      position: absolute;
      top: -4px;
      right: -4px;
      font-size: 0.5rem;
      font-weight: 700;
      background: var(--color-primary);
      color: #fff;
      border-radius: 50%;
      width: 16px;
      height: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      line-height: 1;
    }

    /* Expanded cards */

    .side-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md, 12px);
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: var(--shadow-sm, 0 1px 3px rgba(0,0,0,0.04));
      flex: 1;
      min-height: 0;
    }

    .side-card-header {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.5rem 0.75rem;
      border-bottom: 1px solid var(--color-border);
      flex-shrink: 0;
      cursor: pointer;
      transition: background 0.15s;
    }

    .side-card-header:hover {
      background: var(--color-bg-subtle, rgba(0,0,0,0.02));
    }

    .side-card-header > svg:first-child { color: var(--color-primary); flex-shrink: 0; }

    .side-card-header h2 {
      flex: 1;
      font-size: 0.625rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.6px;
      color: var(--color-primary);
      margin: 0;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .side-card-dismiss {
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: var(--radius-xs, 4px);
      color: var(--color-text-secondary);
      cursor: pointer;
      flex-shrink: 0;
      transition: background 0.15s, color 0.15s;
    }

    .side-card-dismiss:hover {
      background: var(--color-bg-hover);
      color: var(--color-error, #dc2626);
    }

    .collapse-icon {
      color: var(--color-text-secondary);
      flex-shrink: 0;
      opacity: 0.5;
    }

    .side-card-header:hover .collapse-icon { opacity: 1; }

    .side-card-count {
      font-size: 0.5625rem;
      font-weight: 600;
      color: #fff;
      background: var(--color-primary);
      border-radius: var(--radius-pill, 100px);
      padding: 1px 5px;
      min-width: 16px;
      text-align: center;
    }

    .side-card-body {
      flex: 1;
      overflow-y: auto;
      padding: 0.625rem 0.75rem;
    }

    .side-card-body::-webkit-scrollbar { width: 4px; }
    .side-card-body::-webkit-scrollbar-track { background: transparent; }
    .side-card-body::-webkit-scrollbar-thumb {
      background: var(--color-border);
      border-radius: 2px;
    }

    /* Document detail */

    .doc-title {
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0 0 0.5rem 0;
      line-height: 1.4;
      word-break: break-word;
    }

    .doc-meta-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0.375rem;
      margin-bottom: 0.5rem;
    }

    .doc-meta-item {
      padding: 0.375rem 0.5rem;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-sm, 6px);
    }

    .doc-meta-label {
      display: block;
      font-size: 0.5625rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
      margin-bottom: 1px;
    }

    .doc-meta-value {
      font-size: 0.6875rem;
      color: var(--color-text);
      font-weight: 500;
    }

    .doc-summary-text {
      font-size: 0.6875rem;
      color: var(--color-text-body);
      line-height: 1.5;
      margin: 0 0 0.5rem 0;
    }

    .doc-action-link {
      display: flex;
      align-items: center;
      gap: 5px;
      width: 100%;
      justify-content: center;
      padding: 0.375rem 0.5rem;
      background: var(--color-primary-light, rgba(208, 74, 2, 0.06));
      border: 1px solid rgba(208, 74, 2, 0.15);
      border-radius: var(--radius-sm, 8px);
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-primary);
      cursor: pointer;
      transition: box-shadow 0.15s, border-color 0.15s;
    }

    .doc-action-link:hover {
      box-shadow: var(--shadow-sm, 0 1px 3px rgba(0,0,0,0.04));
      border-color: rgba(208, 74, 2, 0.3);
    }

    /* Context items */

    .ctx-item {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 5px 0;
    }

    .ctx-item + .ctx-item {
      border-top: 1px solid var(--color-border);
    }

    .ctx-item-icon {
      flex-shrink: 0;
      color: var(--color-primary);
      opacity: 0.5;
      display: flex;
    }

    .ctx-item-title {
      flex: 1;
      font-size: 0.6875rem;
      color: var(--color-text);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      cursor: pointer;
    }

    .ctx-item-title:hover { color: var(--color-primary); }

    .ctx-item-remove {
      flex-shrink: 0;
      width: 18px;
      height: 18px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 4px;
      color: var(--color-text-secondary);
      cursor: pointer;
      opacity: 0;
      transition: opacity 0.15s, background 0.15s;
    }

    .ctx-item:hover .ctx-item-remove { opacity: 1; }

    .ctx-item-remove:hover {
      background: var(--color-bg-hover);
      color: var(--color-error, #dc2626);
    }
  `]
})
export class ChatViewComponent implements AfterViewChecked, OnInit {
  private chatService = inject(ChatService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  @ViewChild('messagesEl') messagesEl!: ElementRef<HTMLDivElement>;
  @ViewChild('textareaEl') textareaEl!: ElementRef<HTMLTextAreaElement>;

  inputText = signal('');
  inputFocused = signal(false);
  messages = signal<ChatMessage[]>([]);
  isStreaming = signal(false);
  rulingContext = signal<RulingContext | null>(null);
  private lastQueryForRetry = '';
  private lastContextForRetry: RulingContext | null = null;
  private shouldScroll = false;
  private activeSubscription: Subscription | null = null;
  private abortController: AbortController | null = null;

  selectedDocument = signal<DocumentDetail | null>(null);
  contextItems = signal<ContextItem[]>([]);
  docPanelOpen = signal(false);
  ctxPanelOpen = signal(false);

  canSend = computed(() => this.inputText().trim().length > 0 && !this.isStreaming());

  suggestions: SuggestedPrompt[] = [
    { icon: '\u2696', title: '\u00bfQu\u00e9 jurisprudencia existe sobre despido discriminatorio?', query: '\u00bfQu\u00e9 jurisprudencia existe sobre despido discriminatorio?' },
    { icon: '\uD83D\uDCDC', title: '\u00bfC\u00f3mo se aplica el art\u00edculo 245 LCT en fallos recientes?', query: '\u00bfC\u00f3mo se aplica el art\u00edculo 245 LCT en fallos recientes?' },
    { icon: '\uD83D\uDD0D', title: 'Buscar fallos de la CSJN sobre libertad de expresi\u00f3n', query: 'Buscar fallos de la CSJN sobre libertad de expresi\u00f3n' },
    { icon: '\uD83C\uDFDB', title: '\u00bfQu\u00e9 tribunales tratan da\u00f1os y perjuicios por mala praxis?', query: '\u00bfQu\u00e9 tribunales tratan da\u00f1os y perjuicios por mala praxis?' },
  ];

  constructor() {
    effect(() => {
      this.messages();
      this.shouldScroll = true;
    });
  }

  private onboarding = inject(OnboardingService);

  ngOnInit() {
    const params = this.route.snapshot.queryParams;
    if (params['rulingId'] && params['rulingTitle']) {
      this.rulingContext.set({
        rulingId: params['rulingId'],
        rulingTitle: params['rulingTitle']
      });
      setTimeout(() => this.textareaEl?.nativeElement?.focus());
    }
    this.onboarding.tryShow('chat-citations');
  }

  ngAfterViewChecked() {
    if (this.shouldScroll && this.messagesEl?.nativeElement) {
      this.messagesEl.nativeElement.scrollTop = this.messagesEl.nativeElement.scrollHeight;
      this.shouldScroll = false;
    }
  }

  onMessagesClick(event: MouseEvent) {
    const target = event.target as HTMLElement;

    const ctxBtn = target.closest('.ruling-ctx-btn');
    if (ctxBtn) {
      event.preventDefault();
      event.stopPropagation();
      const id = ctxBtn.getAttribute('data-ruling-id');
      const title = ctxBtn.getAttribute('data-ruling-title');
      if (id && title) {
        this.rulingContext.set({ rulingId: id, rulingTitle: title });
        this.addContextItem({ rulingId: id, title });
        setTimeout(() => this.textareaEl?.nativeElement?.focus());
      }
      return;
    }

    const link = target.closest('a.ruling-link');
    if (link) {
      event.preventDefault();
      const id = link.getAttribute('data-ruling-id');
      const title = link.getAttribute('title')
        || link.closest('.cite-source')?.querySelector('.cite-title')?.textContent?.trim()
        || 'Documento';
      if (id) {
        this.router.navigate(['/jurisprudencia', id]);
        this.addContextItem({ rulingId: id, title });
      }
    }
  }

  onInput(event: Event) {
    const textarea = event.target as HTMLTextAreaElement;
    this.inputText.set(textarea.value);
    this.autoGrow(textarea);
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.send();
    }
  }

  private autoGrow(el: HTMLTextAreaElement) {
    el.style.height = 'auto';
    el.style.height = Math.min(el.scrollHeight, 180) + 'px';
  }

  private resetTextarea() {
    if (this.textareaEl?.nativeElement) {
      this.textareaEl.nativeElement.style.height = 'auto';
    }
  }

  dismissContext() {
    this.rulingContext.set(null);
  }

  clearSelectedDocument() {
    this.selectedDocument.set(null);
    this.docPanelOpen.set(false);
  }

  removeContextItem(rulingId: string) {
    this.contextItems.update(items => items.filter(i => i.rulingId !== rulingId));
    if (this.contextItems().length === 0) this.ctxPanelOpen.set(false);
  }

  selectDocumentFromContext(item: ContextItem) {
    this.selectedDocument.set({ rulingId: item.rulingId, title: item.title });
    this.docPanelOpen.set(true);
  }

  navigateToRuling(rulingId: string) {
    this.router.navigate(['/jurisprudencia', rulingId]);
  }

  toggleDocPanel() {
    if (!this.selectedDocument()) return;
    this.docPanelOpen.update(v => !v);
  }

  toggleCtxPanel() {
    if (this.contextItems().length === 0) return;
    this.ctxPanelOpen.update(v => !v);
  }

  private addContextItem(item: ContextItem) {
    this.contextItems.update(items => {
      if (items.some(i => i.rulingId === item.rulingId)) return items;
      return [...items, item];
    });
    this.ctxPanelOpen.set(true);
  }

  useSuggestion(query: string) {
    this.inputText.set(query);
    setTimeout(() => this.send());
  }

  send() {
    const query = this.inputText().trim();
    if (!query || this.isStreaming()) return;

    this.inputText.set('');
    this.resetTextarea();
    this.lastQueryForRetry = query;

    const ctx = this.rulingContext();
    this.lastContextForRetry = ctx;

    const streamQuery = ctx
      ? `[Contexto: el usuario consulta sobre el fallo "${ctx.rulingTitle}" (ID: ${ctx.rulingId})]\n\n${query}`
      : query;

    this.messages.update((m) => [...m, { role: 'user', text: query, state: 'complete' }]);
    this.messages.update((m) => [...m, { role: 'assistant', text: '', state: 'streaming' }]);
    this.isStreaming.set(true);
    this.shouldScroll = true;

    this.rulingContext.set(null);

    this.abortController = new AbortController();
    this.activeSubscription = this.chatService.stream(streamQuery, { signal: this.abortController.signal }).subscribe({
      next: (ev) => this.handleStreamEvent(ev),
      error: () => this.isStreaming.set(false)
    });
  }

  retry() {
    const query = this.lastQueryForRetry;
    if (!query || this.isStreaming()) return;

    const ctx = this.lastContextForRetry;
    const streamQuery = ctx
      ? `[Contexto: el usuario consulta sobre el fallo "${ctx.rulingTitle}" (ID: ${ctx.rulingId})]\n\n${query}`
      : query;

    this.messages.update((m) => m.filter((msg) => !(msg.role === 'assistant' && msg.state === 'error')));
    this.messages.update((m) => [...m, { role: 'assistant', text: '', state: 'streaming' }]);
    this.isStreaming.set(true);
    this.shouldScroll = true;

    this.abortController = new AbortController();
    this.activeSubscription = this.chatService.stream(streamQuery, { signal: this.abortController.signal }).subscribe({
      next: (ev) => this.handleStreamEvent(ev),
      error: () => this.isStreaming.set(false)
    });
  }

  stopStreaming() {
    this.abortController?.abort();
    this.abortController = null;
    this.activeSubscription?.unsubscribe();
    this.activeSubscription = null;

    this.messages.update((m) => {
      const copy = [...m];
      const last = copy[copy.length - 1];
      if (last?.role === 'assistant' && last.state === 'streaming') {
        copy[copy.length - 1] = { ...last, state: 'complete' };
      }
      return copy;
    });
    this.isStreaming.set(false);
  }

  async copyMessage(msg: ChatMessage, index: number) {
    try {
      await navigator.clipboard.writeText(msg.text);
      this.messages.update((m) => {
        const copy = [...m];
        copy[index] = { ...copy[index], copied: true };
        return copy;
      });
      setTimeout(() => {
        this.messages.update((m) => {
          const copy = [...m];
          if (copy[index]) copy[index] = { ...copy[index], copied: false };
          return copy;
        });
      }, 2000);
    } catch { /* clipboard not available */ }
  }

  getThinkingText(msg: ChatMessage): string {
    if (msg.toolTextBreak && msg.toolTextBreak > 0 && msg.text.length > msg.toolTextBreak) {
      return msg.text.slice(0, msg.toolTextBreak).trim();
    }
    return '';
  }

  getAnswerText(msg: ChatMessage): string {
    if (msg.toolTextBreak && msg.toolTextBreak > 0 && msg.text.length > msg.toolTextBreak) {
      return msg.text.slice(msg.toolTextBreak);
    }
    return msg.text;
  }

  doneToolCount(tools: ToolEvent[]): number {
    return tools.reduce((n, t) => n + (t.state !== 'running' ? 1 : 0), 0);
  }

  toolResultTotal(tools: ToolEvent[]): number {
    return tools.reduce((sum, t) => sum + (t.resultCount ?? 0), 0);
  }

  toolSummaryText(tools: ToolEvent[]): string {
    const done = this.doneToolCount(tools);
    const results = this.toolResultTotal(tools);
    const running = tools.length - done;

    if (running > 0) return `${done} de ${tools.length} consultas completadas`;
    if (results > 0) return `${done} consultas · ${results} resultados encontrados`;
    return `${done} consultas realizadas · sin resultados`;
  }

  private handleStreamEvent(ev: import('../../../services/chat.service').ChatStreamEvent) {
    if (ev.type === 'chunk' && ev.text) {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant') {
          copy[copy.length - 1] = { ...last, text: last.text + ev.text };
        }
        return copy;
      });
      this.shouldScroll = true;
    } else if (ev.type === 'tool_start' && ev.toolName) {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant') {
          const tools = [...(last.tools ?? []), { toolName: ev.toolName!, state: 'running' as ToolChipState }];
          copy[copy.length - 1] = { ...last, tools, toolTextBreak: last.text.length };
        }
        return copy;
      });
      this.shouldScroll = true;
    } else if (ev.type === 'tool_end' && ev.toolName) {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant' && last.tools) {
          const tools = last.tools.map((t) =>
            t.toolName === ev.toolName ? { ...t, state: 'done' as ToolChipState, resultCount: ev.resultCount } : t
          );
          copy[copy.length - 1] = { ...last, tools };
        }
        return copy;
      });
      this.shouldScroll = true;
    } else if (ev.type === 'validation' && ev.validation) {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant') {
          copy[copy.length - 1] = { ...last, validation: ev.validation };
        }
        return copy;
      });
    } else if (ev.type === 'complete') {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant') {
          copy[copy.length - 1] = { ...last, state: 'complete' };
        }
        return copy;
      });
      this.isStreaming.set(false);
      this.activeSubscription = null;
      this.abortController = null;
    } else if (ev.type === 'error') {
      this.messages.update((m) => {
        const copy = [...m];
        const last = copy[copy.length - 1];
        if (last?.role === 'assistant') {
          copy[copy.length - 1] = { ...last, state: 'error', error: ev.error };
        }
        return copy;
      });
      this.isStreaming.set(false);
      this.activeSubscription = null;
      this.abortController = null;
    }
  }
}