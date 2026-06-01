import { Component, signal, inject, DestroyRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { RulingService } from '../../../services/ruling.service';
import { AuthService } from '../../../services/auth.service';
import { RulingReprocessService } from '../../../services/admin/ruling-reprocess.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { ProceedingService } from '../../../services/proceeding.service';
import { RulingCardComponent } from '../ruling-card/ruling-card.component';
import { ProceedingTimelineComponent } from '../proceeding-timeline/proceeding-timeline.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { SkeletonDetailComponent } from '../../../shared/components/skeletons/skeleton-detail.component';
import { RulingDatePipe } from '../../../shared/pipes/ruling-date.pipe';
import { CitationTypeLabelPipe } from '../../../shared/pipes/citation-type-label.pipe';
import type { RulingDetail, RelatedRuling, PersonParticipation, Court } from '../../../models/ruling.models';
import type { ProceedingResponse, ProsecutorOpinionResponse } from '../../../models/proceeding.models';
import { GraphExplorerComponent } from '../../graph-explorer/graph-explorer/graph-explorer.component';
import { BreadcrumbComponent } from '../../../shared/components/breadcrumb/breadcrumb.component';
import { EntityPreviewDirective } from '../../../shared/directives/entity-preview.directive';

type DetailState = 'loading' | 'loaded' | 'notFound' | 'error';
type DetailTab = 'info' | 'doctrine' | 'votes' | 'relations' | 'proceeding' | 'graph';

@Component({
  selector: 'app-ruling-detail',
  standalone: true,
  imports: [RouterLink, RulingCardComponent, ProceedingTimelineComponent, LoadingSpinnerComponent, SkeletonDetailComponent, RulingDatePipe, CitationTypeLabelPipe, GraphExplorerComponent, BreadcrumbComponent, EntityPreviewDirective],
  template: `
    <div class="ruling-detail">
      @if (state() === 'loading') {
        <app-skeleton-detail />
      }

      @if (state() === 'notFound') {
        <div class="state-message">
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" class="state-icon"><circle cx="12" cy="12" r="10"/><path d="M16 16s-1.5-2-4-2-4 2-4 2"/><line x1="9" y1="9" x2="9.01" y2="9"/><line x1="15" y1="9" x2="15.01" y2="9"/></svg>
          <h2>Fallo no encontrado</h2>
          <p>El fallo solicitado no existe o fue eliminado.</p>
          <a routerLink="/jurisprudencia" class="link-button">Volver a buscar</a>
        </div>
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" class="state-icon"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
          <h2>Error al cargar</h2>
          <p>No se pudo cargar el fallo. Intente de nuevo.</p>
          <a routerLink="/jurisprudencia" class="link-button">Volver a buscar</a>
        </div>
      }

      @if (state() === 'loaded' && ruling(); as r) {
        <app-breadcrumb [items]="[{ label: 'Consulta' }, { label: 'Jurisprudencia', route: '/jurisprudencia' }, { label: r.caseTitle }]" />

        <header class="ruling-header">
          <div class="header-top">
            <div class="header-badges">
              @if (r.jurisdictionArea) {
                <span class="header-badge">{{ r.jurisdictionArea }}</span>
              }
              @if (r.instance) {
                <span class="header-badge">{{ r.instance }}</span>
              }
              @if (r.isUnconstitutional) {
                <span class="header-badge badge-alert">Inconstitucional</span>
              }
              @if (r.legalBranch) {
                <span class="header-badge">{{ r.legalBranch }}</span>
              }
              @if (r.precedentWeight) {
                <span class="header-badge">{{ r.precedentWeight }}</span>
              }
              @if (r.isPlenario) {
                <span class="header-badge">Plenario</span>
              }
              @if (r.isLeadingCase) {
                <span class="header-badge">Leading case</span>
              }
              @if (r.status === 'Reprocessing') {
                <span class="header-badge badge-reprocess">En reprocesamiento</span>
              }
            </div>
            <span class="header-date">{{ r.rulingDate | rulingDate }}</span>
          </div>
          <h1>{{ r.caseTitle }}</h1>
          <div class="header-sub">
            <div class="court-block">
              <a class="court-name" [routerLink]="['/organismos', r.court.id]" [entityPreview]="{ type: 'court', id: r.court.id }">
                <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/><path d="M9 9h.01"/><path d="M9 13h.01"/><path d="M9 17h.01"/></svg>
                {{ r.court.name }}
              </a>
              @if (courtOntologySummary(r.court); as courtOnt) {
                <span class="court-ontology">{{ courtOnt }}</span>
              }
            </div>
            @if (r.caseNumber) {
              <span class="case-num">{{ r.caseNumber }}</span>
            }
          </div>
        </header>

        <div class="content-split">
          <div class="doc-panel">
            <div class="doc-panel-header">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
              <span class="doc-panel-title">Documento original</span>
            </div>
            @if (documentState() === 'loading') {
              <div class="doc-placeholder">
                <app-loading-spinner message="Cargando documento..." />
              </div>
            }
            @if (documentState() === 'unavailable') {
              <div class="doc-placeholder doc-unavailable">
                <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2">
                  <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                  <polyline points="14 2 14 8 20 8"/>
                  <line x1="9" y1="12" x2="15" y2="12"/>
                  <line x1="9" y1="16" x2="13" y2="16"/>
                </svg>
                <p>Documento no disponible</p>
              </div>
            }
            @if (documentState() === 'loaded' && documentUrl()) {
              <iframe
                class="doc-iframe"
                [src]="documentUrl()"
                title="Documento del fallo"
                aria-label="Vista previa del documento"
              ></iframe>
            }
          </div>

          <div class="detail-panel">
            @if (r.summary) {
              <section class="content-card summary-card">
                <div class="card-icon-row">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/><polyline points="10 9 9 9 8 9"/></svg>
                  <h2>Resumen</h2>
                </div>
                <p class="card-text">{{ r.summary }}</p>
              </section>
            }

            @if (r.holding) {
              <section class="content-card holding-card">
                <div class="card-icon-row">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
                  <h2>Considerando</h2>
                </div>
                <p class="card-text">{{ r.holding }}</p>
              </section>
            }

            @if (r.prosecutorOpinion; as po) {
              <section class="content-card prosecutor-card">
                <div class="card-icon-row">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                  <h2>Dictamen del Procurador</h2>
                  <span class="prosecutor-badge" [class.agreed]="po.agreedWithCourt" [class.disagreed]="!po.agreedWithCourt">
                    {{ po.agreedWithCourt ? 'Coincidió con la Corte' : 'Disintió con la Corte' }}
                  </span>
                </div>
                <div class="prosecutor-name">{{ po.prosecutorName }}</div>
                @if (po.summary) {
                  <p class="card-text">{{ po.summary }}</p>
                }
                @if (po.recommendedDirection) {
                  <div class="prosecutor-direction">
                    <span class="direction-label">Recomendación:</span> {{ po.recommendedDirection }}
                  </div>
                }
              </section>
            }

            <div class="tab-bar">
              <button type="button" class="tab-btn" [class.active]="activeTab() === 'info'" (click)="activeTab.set('info')">
                <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>
                Información
              </button>
              @if (ruling()?.ratioDecidendi || ruling()?.doctrinaLegal || ruling()?.doctrines?.length) {
                <button type="button" class="tab-btn" [class.active]="activeTab() === 'doctrine'" (click)="activeTab.set('doctrine')">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
                  Doctrina
                </button>
              }
              @if (ruling()?.votes?.length) {
                <button type="button" class="tab-btn" [class.active]="activeTab() === 'votes'" (click)="activeTab.set('votes')">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                  Votos
                  <span class="tab-count">{{ ruling()!.votes.length }}</span>
                </button>
              }
              <button type="button" class="tab-btn" [class.active]="activeTab() === 'relations'" (click)="activeTab.set('relations')">
                <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"/><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"/></svg>
                Relaciones
                @if (relationCount() > 0) {
                  <span class="tab-count">{{ relationCount() }}</span>
                }
              </button>
              @if (proceeding()) {
                <button type="button" class="tab-btn" [class.active]="activeTab() === 'proceeding'" (click)="activeTab.set('proceeding')">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
                  Expediente
                  <span class="tab-count">{{ proceeding()!.rulings.length }}</span>
                </button>
              }
              <button type="button" class="tab-btn" [class.active]="activeTab() === 'graph'" (click)="activeTab.set('graph')">
                <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/><line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/></svg>
                Grafo
              </button>
            </div>

            @if (activeTab() === 'info') {
              <div class="tab-content">
                <div class="meta-grid">
                  @if (r.jurisdiction) {
                    <div class="meta-item">
                      <span class="meta-label">Jurisdicción</span>
                      <span class="meta-value">{{ r.jurisdiction }}</span>
                    </div>
                  }
                  @if (r.resourceType) {
                    <div class="meta-item">
                      <span class="meta-label">Tipo de recurso</span>
                      <span class="meta-value">{{ r.resourceType }}</span>
                    </div>
                  }
                  @if (r.rulingDirection) {
                    <div class="meta-item">
                      <span class="meta-label">Sentencia</span>
                      <span class="meta-value">{{ r.rulingDirection }}</span>
                    </div>
                  }
                  @if (r.subjectArea) {
                    <div class="meta-item">
                      <span class="meta-label">Área temática</span>
                      <span class="meta-value">{{ r.subjectArea }}</span>
                    </div>
                  }
                </div>

                @if (r.persons.length) {
                  <div class="info-section" id="intervinientes">
                    <h3>Intervinientes</h3>
                    @if (participationTribunal(r).length) {
                      <h4 class="subh">Tribunal (integración y votos)</h4>
                      <div class="persons-wrap">
                        @for (person of participationTribunal(r); track person.personId + person.rulingRole) {
                          <a class="person-pill" [routerLink]="['/sujetos', person.personId]" [entityPreview]="{ type: 'person', id: person.personId }">
                            <span class="person-avatar">{{ person.displayName[0] }}</span>
                            <div class="person-info">
                              <span class="person-name">{{ person.displayName }}</span>
                              <span class="person-role">{{ roleLabel(person.rulingRole) }}</span>
                            </div>
                          </a>
                        }
                      </div>
                    }
                    @if (participationFiscalia(r).length) {
                      <h4 class="subh">Ministerio Público y defensa</h4>
                      <div class="persons-wrap">
                        @for (person of participationFiscalia(r); track person.personId + person.rulingRole) {
                          <a class="person-pill" [routerLink]="['/sujetos', person.personId]" [entityPreview]="{ type: 'person', id: person.personId }">
                            <span class="person-avatar">{{ person.displayName[0] }}</span>
                            <div class="person-info">
                              <span class="person-name">{{ person.displayName }}</span>
                              <span class="person-role">{{ roleLabel(person.rulingRole) }}</span>
                            </div>
                          </a>
                        }
                      </div>
                    }
                    @if (participationOtros(r).length) {
                      <h4 class="subh">Otras intervenciones</h4>
                      <div class="persons-wrap">
                        @for (person of participationOtros(r); track person.personId + person.rulingRole) {
                          <a class="person-pill" [routerLink]="['/sujetos', person.personId]" [entityPreview]="{ type: 'person', id: person.personId }">
                            <span class="person-avatar">{{ person.displayName[0] }}</span>
                            <div class="person-info">
                              <span class="person-name">{{ person.displayName }}</span>
                              <span class="person-role">{{ roleLabel(person.rulingRole) }}</span>
                            </div>
                          </a>
                        }
                      </div>
                    }
                  </div>
                }

                @if (r.keywords.length) {
                  <div class="info-section">
                    <h3>Palabras clave</h3>
                    <div class="keywords">
                      @for (kw of r.keywords; track kw.id) {
                        @if (kw.thesaurusTermId) {
                          <a class="keyword-tag keyword-linked" [routerLink]="['/jurisprudencia/resultados']" [queryParams]="{ query: '', keywords: kw.description }" title="Buscar fallos con este descriptor">
                            <svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/></svg>
                            {{ kw.description }}
                          </a>
                        } @else {
                          <span class="keyword-tag">{{ kw.description }}</span>
                        }
                      }
                    </div>
                  </div>
                }
              </div>
            }

            @if (activeTab() === 'doctrine') {
              <div class="tab-content">
                @if (r.ratioDecidendi) {
                  <section class="content-card">
                    <div class="card-icon-row">
                      <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
                      <h2>Ratio Decidendi</h2>
                    </div>
                    <p class="card-text doctrine-text">{{ r.ratioDecidendi }}</p>
                  </section>
                }
                @if (r.doctrinaLegal) {
                  <section class="content-card">
                    <div class="card-icon-row">
                      <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
                      <h2>Doctrina Legal</h2>
                    </div>
                    <p class="card-text doctrine-text">{{ r.doctrinaLegal }}</p>
                  </section>
                }
                @if (r.doctrines?.length) {
                  <section class="content-card">
                    <div class="card-icon-row">
                      <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                      <h2>Doctrinas estructuradas</h2>
                    </div>
                    @for (d of r.doctrines; track d.id) {
                      <div class="doctrine-entry" [class.overruled]="d.isOverruled">
                        @if (d.topic) { <span class="doctrine-topic">{{ d.topic }}</span> }
                        <p class="card-text">{{ d.statement }}</p>
                        @if (d.isOverruled && d.overruledByRulingTitle) {
                          <span class="doctrine-overruled">Superada por: <a [routerLink]="['/jurisprudencia', d.overruledByRulingId]">{{ d.overruledByRulingTitle }}</a></span>
                        }
                      </div>
                    }
                  </section>
                }
              </div>
            }

            @if (activeTab() === 'votes') {
              <div class="tab-content">
                @for (vote of r.votes; track vote.id) {
                  <div class="vote-card" [attr.data-vote-type]="vote.voteType">
                    <div class="vote-header">
                      <span class="vote-type-badge" [attr.data-type]="vote.voteType">{{ voteTypeLabel(vote.voteType) }}</span>
                      @if (vote.pages) {
                        <span class="vote-pages">pp. {{ vote.pages }}</span>
                      }
                    </div>
                    @if (vote.judges.length) {
                      <div class="vote-judges">
                        @for (judge of vote.judges; track judge) {
                          <span class="vote-judge-pill">{{ judge }}</span>
                        }
                      </div>
                    }
                    @if (vote.summary) {
                      <p class="vote-summary">{{ vote.summary }}</p>
                    }
                  </div>
                }
              </div>
            }

            @if (activeTab() === 'relations') {
              <div class="tab-content">
                @if (r.statutes.length) {
                  <div class="info-section">
                    <h3>Normas citadas</h3>
                    <div class="statute-list">
                      @for (s of r.statutes; track s.number + s.name) {
                        <div class="statute-item">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/></svg>
                          <div class="statute-text">
                            <span class="statute-name">Ley {{ s.number }}</span>
                            <span class="statute-desc">{{ s.name }}@if (s.articles) { — art. {{ s.articles }} }</span>
                            @if (s.normType || s.normativeLevel) {
                              <span class="statute-ontology">
                                @if (s.normType) { <span>{{ s.normType }}</span> }
                                @if (s.normType && s.normativeLevel) { <span class="statute-sep">·</span> }
                                @if (s.normativeLevel) { <span>{{ s.normativeLevel }}</span> }
                              </span>
                            }
                            @if (s.effectiveFrom || s.effectiveTo) {
                              <span class="statute-validity">
                                @if (s.effectiveFrom && s.effectiveTo) {
                                  Vigencia: {{ s.effectiveFrom | rulingDate }} – {{ s.effectiveTo | rulingDate }}
                                } @else if (s.effectiveFrom) {
                                  Vigencia desde: {{ s.effectiveFrom | rulingDate }}
                                } @else if (s.effectiveTo) {
                                  Vigencia hasta: {{ s.effectiveTo | rulingDate }}
                                }
                              </span>
                            }
                          </div>
                          @if (s.url) {
                            <a [href]="s.url" target="_blank" rel="noopener" class="statute-link" title="Ver norma">
                              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><polyline points="15 3 21 3 21 9"/><line x1="10" y1="14" x2="21" y2="3"/></svg>
                            </a>
                          }
                        </div>
                      }
                    </div>
                  </div>
                }

                @if (r.citations.length) {
                  <div class="info-section">
                    <h3>Citas jurisprudenciales</h3>
                    <div class="citation-list">
                      @for (c of r.citations; track c.externalAlias + c.citationType) {
                        <div class="citation-item">
                          <span class="citation-type">{{ c.citationType | citationTypeLabel }}</span>
                          <span class="citation-alias">{{ c.externalAlias }}</span>
                          @if (c.targetRulingId) {
                            <a [routerLink]="['/jurisprudencia', c.targetRulingId]" [entityPreview]="{ type: 'ruling', id: c.targetRulingId }" class="citation-go">
                              {{ c.targetCaseTitle ?? 'Ver fallo' }}
                              <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
                            </a>
                          }
                        </div>
                      }
                    </div>
                  </div>
                }

                @if (!r.statutes.length && !r.citations.length) {
                  <div class="empty-tab">
                    <p>No hay normas citadas ni citas registradas para este fallo.</p>
                  </div>
                }
              </div>
            }

            @if (activeTab() === 'proceeding' && proceeding()) {
              <div class="tab-content">
                <app-proceeding-timeline [proceeding]="proceeding()" />
              </div>
            }

            @if (activeTab() === 'graph') {
              <div class="tab-content graph-tab-content">
                @defer (when activeTab() === 'graph') {
                  <app-graph-explorer
                    [initialEntityType]="'ruling'"
                    [initialEntityId]="r.id"
                    (entitySelected)="onGraphNodeClick($event)" />
                } @loading {
                  <app-loading-spinner message="Cargando visor de grafo..." />
                }
              </div>
            }

            @if (isAdmin() && r.status !== 'Reprocessing') {
              <section class="admin-reprocess-card">
                <h3>Administración</h3>
                <p>Reprocesamiento completo desde la fuente (CSJN). El fallo dejará de aparecer en búsqueda hasta finalizar.</p>
                <label class="reprocess-cache">
                  <input type="checkbox" [checked]="reprocessUseCache()" (change)="reprocessUseCache.set($any($event.target).checked)" />
                  Usar caché de APIs
                </label>
                <div class="admin-reprocess-actions">
                  <button type="button" class="btn-reprocess" [disabled]="reprocessBusy()" (click)="enqueueReprocess(r.id)">
                    {{ reprocessBusy() ? 'Encolando…' : 'Reprocesar desde fuente' }}
                  </button>
                  <a routerLink="/admin/reproceso" class="link-queue">Ver cola</a>
                </div>
              </section>
            }

            <div class="assistant-cta" (click)="askAssistant(r.id, r.caseTitle)">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/></svg>
              <div class="cta-text">
                <span class="cta-title">Consultar al Asistente sobre este fallo</span>
                <span class="cta-sub">Pregunta sobre doctrina, citas o análisis legal</span>
              </div>
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="cta-arrow"><polyline points="9 18 15 12 9 6"/></svg>
            </div>
          </div>
        </div>

        @if (relatedRulings().length) {
          <section class="related-section">
            <h2 class="related-title">Fallos relacionados</h2>
            <div class="related-scroll">
              @for (rel of relatedRulings(); track rel.id) {
                <app-ruling-card [ruling]="rel" />
              }
            </div>
          </section>
        }
      }
    </div>
  `,
  styles: [`
    .ruling-detail {
      max-width: 1400px;
      margin: 0 auto;
    }

    /* ── Header ── */
    .ruling-header {
      margin-bottom: 1.75rem;
      padding-bottom: 1.5rem;
      border-bottom: 1px solid var(--color-border);
    }
    .header-top {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 0.75rem;
      gap: 1rem;
    }
    .header-badges {
      display: flex;
      gap: 6px;
      flex-wrap: wrap;
    }
    .header-badge {
      display: inline-block;
      padding: 3px 10px;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
    }
    .header-badge.badge-alert {
      background: var(--color-error-bg);
      border-color: rgba(208, 74, 2, 0.2);
      color: var(--color-primary);
    }
    .header-date {
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      white-space: nowrap;
    }
    .ruling-header h1 {
      font-size: 1.375rem;
      margin: 0 0 0.625rem;
      color: var(--color-text);
      line-height: 1.35;
    }
    .header-sub {
      display: flex;
      align-items: flex-start;
      gap: 1.25rem;
      font-size: 0.8125rem;
      color: var(--color-text-body);
    }
    .court-block {
      display: flex;
      flex-direction: column;
      gap: 4px;
      min-width: 0;
    }
    .court-name {
      display: flex;
      align-items: center;
      gap: 5px;
      font-weight: 500;
    }
    .court-name svg { color: var(--color-text-secondary); flex-shrink: 0; }
    .court-ontology {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      line-height: 1.35;
    }
    .case-num {
      color: var(--color-text-secondary);
      font-family: var(--font-mono);
      font-size: 0.75rem;
    }

    /* ── Split layout ── */
    .content-split {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 2rem;
      align-items: start;
      min-height: 600px;
    }

    /* ── Document panel ── */
    .doc-panel {
      position: sticky;
      top: 1rem;
      display: flex;
      flex-direction: column;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-sm);
      overflow: hidden;
      background: var(--color-bg-surface);
      height: calc(100vh - 8rem);
      min-height: 500px;
    }
    .doc-panel-header {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.625rem 1rem;
      border-bottom: 1px solid var(--color-border);
      background: var(--color-bg-subtle);
    }
    .doc-panel-header svg { color: var(--color-primary); }
    .doc-panel-title {
      font-size: 0.6875rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-primary);
    }
    .doc-iframe {
      flex: 1;
      width: 100%;
      border: none;
      height: 100%;
    }
    .doc-placeholder {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 1rem;
      color: var(--color-text-secondary);
      font-size: 0.875rem;
    }
    .doc-placeholder p { margin: 0; }
    .doc-unavailable svg { color: var(--color-border-input); }

    /* ── Detail panel ── */
    .detail-panel { overflow-y: auto; }

    /* ── Content cards (Summary / Holding) ── */
    .content-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem 1.5rem;
      margin-bottom: 1rem;
    }
    .card-icon-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 0.75rem;
    }
    .card-icon-row svg { color: var(--color-primary); flex-shrink: 0; }
    .card-icon-row h2 {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-primary);
      margin: 0;
    }
    .card-text {
      margin: 0;
      font-size: 0.875rem;
      line-height: 1.7;
      color: var(--color-text-body);
      white-space: pre-wrap;
    }
    .summary-card { border-left: 3px solid var(--color-primary); }
    .holding-card { border-left: 3px solid var(--color-primary-amber); }
    .prosecutor-card { border-left: 3px solid #7c3aed; }
    .prosecutor-card .card-icon-row { gap: 0.5rem; flex-wrap: wrap; }
    .prosecutor-badge {
      font-size: 0.625rem;
      font-weight: 600;
      padding: 0.125rem 0.5rem;
      border-radius: var(--radius-pill, 9999px);
    }
    .prosecutor-badge.agreed { background: #dcfce7; color: #166534; }
    .prosecutor-badge.disagreed { background: #fee2e2; color: #991b1b; }
    .prosecutor-name {
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-text-body);
      margin-bottom: 0.25rem;
    }
    .prosecutor-direction {
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      margin-top: 0.25rem;
    }
    .direction-label { font-weight: 600; }

    /* ── Tab bar ── */
    .tab-bar {
      display: flex;
      gap: 0;
      border-bottom: 1px solid var(--color-border);
      margin-bottom: 0;
    }
    .tab-btn {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.75rem 1.25rem;
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      border-bottom: 2px solid transparent;
      margin-bottom: -1px;
      transition: color var(--transition-fast), border-color var(--transition-fast);
    }
    .tab-btn:hover { color: var(--color-text); }
    .tab-btn.active {
      color: var(--color-primary);
      border-bottom-color: var(--color-primary);
      font-weight: 600;
    }
    .tab-btn svg { opacity: 0.7; }
    .tab-btn.active svg { opacity: 1; }
    .tab-count {
      font-size: 0.6875rem;
      background: var(--color-primary);
      color: #fff;
      padding: 1px 6px;
      border-radius: var(--radius-pill);
      font-weight: 600;
    }

    /* ── Tab content ── */
    .tab-content {
      padding: 1.25rem 0;
    }

    /* ── Meta grid ── */
    .meta-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0.75rem;
      margin-bottom: 1.5rem;
    }
    .meta-item {
      padding: 0.75rem 1rem;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-sm);
    }
    .meta-label {
      display: block;
      font-size: 0.6875rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
      margin-bottom: 2px;
    }
    .meta-value {
      font-size: 0.875rem;
      color: var(--color-text);
      font-weight: 500;
    }

    /* ── Info sections ── */
    .info-section {
      margin-bottom: 1.5rem;
    }
    .info-section h3 {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-text-secondary);
      margin: 0 0 0.75rem;
    }
    .subh {
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-body);
      margin: 0.75rem 0 0.5rem;
      text-transform: none;
      letter-spacing: 0;
    }
    .info-section .subh:first-of-type { margin-top: 0; }

    /* ── Persons ── */
    .persons-wrap {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .person-pill {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 8px 12px;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
    }
    a.person-pill {
      text-decoration: none;
      color: inherit;
      transition: border-color 0.15s, background 0.15s;
    }
    a.person-pill:hover {
      border-color: var(--color-primary);
      background: var(--color-bg-subtle);
    }
    .person-avatar {
      width: 30px;
      height: 30px;
      border-radius: 50%;
      background: var(--color-bg-subtle);
      color: var(--color-text-secondary);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.75rem;
      font-weight: 600;
      flex-shrink: 0;
    }
    .person-info { display: flex; flex-direction: column; }
    .person-name { font-size: 0.8125rem; font-weight: 500; color: var(--color-text); }
    .person-role { font-size: 0.6875rem; color: var(--color-text-secondary); }

    /* ── Keywords ── */
    .keywords { display: flex; flex-wrap: wrap; gap: 6px; }
    .keyword-tag {
      background: var(--color-primary-light);
      border: 1px solid rgba(208, 74, 2, 0.15);
      color: var(--color-primary);
      padding: 3px 12px;
      border-radius: var(--radius-pill);
      font-size: 0.75rem;
      font-weight: 500;
    }
    a.keyword-linked {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      text-decoration: none;
      cursor: pointer;
      transition: border-color 0.15s, box-shadow 0.15s;
    }
    a.keyword-linked:hover {
      border-color: rgba(208, 74, 2, 0.4);
      box-shadow: 0 1px 3px rgba(208, 74, 2, 0.1);
      text-decoration: none;
    }
    a.keyword-linked svg { flex-shrink: 0; opacity: 0.6; }

    /* ── Statutes ── */
    .statute-list {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .statute-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
    }
    .statute-item svg { color: var(--color-text-secondary); flex-shrink: 0; }
    .statute-text { flex: 1; min-width: 0; }
    .statute-name { font-size: 0.8125rem; font-weight: 600; color: var(--color-text); display: block; }
    .statute-desc { font-size: 0.75rem; color: var(--color-text-secondary); display: block; }
    .statute-ontology {
      display: block;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      margin-top: 4px;
    }
    .statute-sep { margin: 0 0.25rem; opacity: 0.7; }
    .statute-validity {
      display: block;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      margin-top: 2px;
    }
    .statute-link {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 28px;
      height: 28px;
      border-radius: var(--radius-sm);
      color: var(--color-primary);
      transition: background var(--transition-fast);
      flex-shrink: 0;
    }
    .statute-link:hover { background: var(--color-nav-active-bg); text-decoration: none; }

    /* ── Citations ── */
    .citation-list {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .citation-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      font-size: 0.8125rem;
    }
    .citation-type {
      flex-shrink: 0;
      padding: 2px 8px;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-xs);
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
      text-transform: uppercase;
    }
    .citation-alias {
      flex: 1;
      min-width: 0;
      color: var(--color-text-body);
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    .citation-go {
      display: flex;
      align-items: center;
      gap: 4px;
      color: var(--color-primary);
      font-size: 0.75rem;
      font-weight: 500;
      text-decoration: none;
      white-space: nowrap;
      flex-shrink: 0;
    }
    .citation-go:hover { text-decoration: underline; }

    .empty-tab {
      text-align: center;
      padding: 3rem 1rem;
      color: var(--color-text-secondary);
      font-size: 0.875rem;
    }

    /* ── Doctrine tab ── */
    .doctrine-text { white-space: pre-wrap; line-height: 1.7; }
    .doctrine-entry { padding: .75rem 0; border-bottom: 1px solid var(--color-border); }
    .doctrine-entry:last-child { border-bottom: none; }
    .doctrine-entry.overruled { opacity: .6; }
    .doctrine-topic { display: inline-block; font-size: .6875rem; font-weight: 600; text-transform: uppercase; letter-spacing: .04em; color: var(--color-primary); margin-bottom: .25rem; }
    .doctrine-overruled { display: block; font-size: .75rem; color: #dc2626; margin-top: .25rem; }
    .doctrine-overruled a { color: #dc2626; text-decoration: underline; }

    /* ── Votes tab ── */
    .vote-card {
      padding: 1.25rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-bg-surface);
      margin-bottom: 0.75rem;
    }
    .vote-card[data-vote-type="Majority"] { border-left: 3px solid #16a34a; }
    .vote-card[data-vote-type="Dissent"] { border-left: 3px solid #dc2626; }
    .vote-card[data-vote-type="Concurrence"] { border-left: 3px solid #2563eb; }
    .vote-card[data-vote-type="Individual"] { border-left: 3px solid #7c3aed; }
    .vote-header { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.625rem; }
    .vote-type-badge {
      display: inline-block; padding: 2px 8px; border-radius: var(--radius-xs);
      font-size: 0.6875rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.03em;
      border: 1px solid var(--color-border); background: var(--color-bg-subtle);
    }
    .vote-type-badge[data-type="Majority"] { background: rgba(22,163,74,.08); border-color: rgba(22,163,74,.2); color: #16a34a; }
    .vote-type-badge[data-type="Dissent"] { background: rgba(220,38,38,.08); border-color: rgba(220,38,38,.2); color: #dc2626; }
    .vote-type-badge[data-type="Concurrence"] { background: rgba(37,99,235,.08); border-color: rgba(37,99,235,.2); color: #2563eb; }
    .vote-type-badge[data-type="Individual"] { background: rgba(124,58,237,.08); border-color: rgba(124,58,237,.2); color: #7c3aed; }
    .vote-pages { font-size: 0.6875rem; color: var(--color-text-secondary); }
    .vote-judges { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 0.5rem; }
    .vote-judge-pill {
      display: inline-block; padding: 2px 10px; border-radius: var(--radius-pill);
      font-size: 0.75rem; background: var(--color-bg-subtle); border: 1px solid var(--color-border);
      color: var(--color-text);
    }
    .vote-summary { font-size: 0.8125rem; color: var(--color-text); line-height: 1.6; margin: 0; white-space: pre-wrap; }

    .graph-tab-content { height: 500px; }

    /* ── Assistant CTA ── */
    .badge-reprocess { background: #fff3cd; color: #856404; }
    .admin-reprocess-card {
      margin-bottom: 1rem;
      padding: 1rem;
      border: 1px solid var(--color-border-input);
      border-radius: 8px;
      background: var(--color-surface-alt);
    }
    .admin-reprocess-card h3 { margin: 0 0 0.5rem; font-size: 0.9rem; }
    .admin-reprocess-card p { margin: 0 0 0.75rem; font-size: 0.8rem; color: var(--color-text-body); }
    .reprocess-cache { display: flex; align-items: center; gap: 0.5rem; font-size: 0.8rem; margin-bottom: 0.75rem; }
    .admin-reprocess-actions { display: flex; align-items: center; gap: 1rem; flex-wrap: wrap; }
    .btn-reprocess {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 6px;
      background: var(--color-primary);
      color: #fff;
      font-size: 0.85rem;
      cursor: pointer;
    }
    .btn-reprocess:disabled { opacity: 0.6; cursor: not-allowed; }
    .link-queue { font-size: 0.85rem; color: var(--color-primary); }

    .assistant-cta {
      display: flex;
      align-items: center;
      gap: 14px;
      padding: 1rem 1.25rem;
      background: var(--color-primary-light);
      border: 1px solid rgba(208, 74, 2, 0.15);
      border-radius: var(--radius-md);
      cursor: pointer;
      transition: box-shadow var(--transition-fast), border-color var(--transition-fast);
      margin-top: 0.5rem;
    }
    .assistant-cta:hover {
      box-shadow: var(--shadow-sm);
      border-color: rgba(208, 74, 2, 0.3);
    }
    .assistant-cta > svg:first-child { color: var(--color-primary); flex-shrink: 0; }
    .cta-text { flex: 1; }
    .cta-title { display: block; font-size: 0.875rem; font-weight: 600; color: var(--color-primary); }
    .cta-sub { display: block; font-size: 0.75rem; color: var(--color-text-secondary); margin-top: 1px; }
    .cta-arrow { color: var(--color-primary); flex-shrink: 0; opacity: 0.6; }

    /* ── Related ── */
    .related-section {
      margin-top: 2.5rem;
      padding-top: 2rem;
      border-top: 1px solid var(--color-border);
    }
    .related-title {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-text-secondary);
      margin: 0 0 1rem;
    }
    .related-scroll {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 0.75rem;
    }

    /* ── State messages ── */
    .state-message {
      text-align: center;
      padding: 5rem 2rem;
      color: var(--color-text-body);
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
    }
    .state-message h2 { margin: 0.5rem 0 0; }
    .state-message p { margin: 0; }
    .state-message.error { color: var(--color-primary); }
    .state-icon { color: var(--color-border-input); }
    .link-button {
      display: inline-block;
      margin-top: 1rem;
      padding: 0.625rem 1.5rem;
      background: var(--color-primary);
      color: #fff;
      text-decoration: none;
      border-radius: var(--radius-sm);
      font-size: 0.875rem;
      font-weight: 600;
      transition: background var(--transition-fast);
    }
    .link-button:hover { background: var(--color-primary-hover); }

    @media (max-width: 900px) {
      .content-split { grid-template-columns: 1fr; }
      .doc-panel { position: static; height: 60vh; }
      .meta-grid { grid-template-columns: 1fr; }
      .related-scroll { grid-template-columns: 1fr; }
    }
  `]
})
export class RulingDetailComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private rulingService = inject(RulingService);
  private proceedingService = inject(ProceedingService);
  private auth = inject(AuthService);
  private reprocessService = inject(RulingReprocessService);
  private notify = inject(NotificationService);
  private destroyRef = inject(DestroyRef);
  private sanitizer = inject(DomSanitizer);

  isAdmin = () => this.auth.user()?.role === 'admin';
  reprocessUseCache = signal(false);
  reprocessBusy = signal(false);

  ruling = signal<RulingDetail | null>(null);
  relatedRulings = signal<RelatedRuling[]>([]);
  state = signal<DetailState>('loading');
  activeTab = signal<DetailTab>('info');
  documentUrl = signal<SafeResourceUrl | null>(null);
  documentState = signal<'loading' | 'loaded' | 'unavailable'>('loading');
  proceeding = signal<ProceedingResponse | null>(null);

  relationCount = signal(0);

  constructor() {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = params.get('id');
        if (!id) {
          this.state.set('notFound');
          return;
        }
        this.loadRuling(id);
      });
  }

  private loadRuling(id: string) {
    this.state.set('loading');
    this.ruling.set(null);
    this.relatedRulings.set([]);
    this.documentUrl.set(null);
    this.documentState.set('loading');
    this.proceeding.set(null);
    this.activeTab.set('info');

    this.rulingService.getById(id).subscribe({
      next: (r) => {
        this.ruling.set(r);
        this.state.set('loaded');
        this.relationCount.set(r.statutes.length + r.citations.length);
        this.loadProceeding(id);
        this.loadRelated(id);
        if (r.blobPath) {
          this.loadDocument(id);
        } else {
          this.documentState.set('unavailable');
        }
      },
      error: (err: { status?: number }) => {
        const status = err?.status ?? 0;
        this.state.set(status === 404 ? 'notFound' : 'error');
      }
    });
  }

  private loadDocument(id: string) {
    this.rulingService.getDocumentBlobUrl(id).subscribe({
      next: (url) => {
        this.documentUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(url));
        this.documentState.set('loaded');
      },
      error: () => this.documentState.set('unavailable')
    });
  }

  private loadRelated(id: string) {
    this.rulingService.getRelated(id, 10).subscribe({
      next: (list) => this.relatedRulings.set(list),
      error: () => {}
    });
  }

  private loadProceeding(id: string) {
    this.proceedingService.getByRuling(id).subscribe({
      next: (p) => this.proceeding.set(p),
    });
  }

  private readonly formationRoles = new Set(['SIGNATORY', 'DISSENT', 'CONCURRENCE', 'MAJORITY_AUTHOR', 'MAJORITY']);

  participationTribunal(r: RulingDetail): PersonParticipation[] {
    return r.persons.filter(p => this.formationRoles.has(p.rulingRole));
  }

  participationFiscalia(r: RulingDetail): PersonParticipation[] {
    return r.persons.filter(p => p.rulingRole === 'PROSECUTOR' || p.rulingRole === 'PUBLIC_DEFENDER');
  }

  participationOtros(r: RulingDetail): PersonParticipation[] {
    const ids = new Set([
      ...this.participationTribunal(r).map(p => p.personId),
      ...this.participationFiscalia(r).map(p => p.personId),
    ]);
    return r.persons.filter(p => !ids.has(p.personId));
  }

  formatPerson(person: PersonParticipation): string {
    return `${person.displayName} (${this.roleLabel(person.rulingRole)})`;
  }

  roleLabel(type: string): string {
    const labels: Record<string, string> = {
      SIGNATORY: 'Firmante',
      DISSENT: 'Disidente',
      MAJORITY: 'Mayoría',
      MAJORITY_AUTHOR: 'Autor del voto',
      CONCURRENCE: 'Concurrencia',
      PROSECUTOR: 'Fiscal',
      PUBLIC_DEFENDER: 'Defensor'
    };
    return labels[type] ?? type;
  }

  voteTypeLabel(type: string): string {
    const labels: Record<string, string> = {
      Majority: 'Voto de mayoría',
      Dissent: 'Disidencia',
      Concurrence: 'Concurrencia',
      Individual: 'Voto individual',
    };
    return labels[type] ?? type;
  }

  formatStatute(s: { number: string; name: string; articles?: string }): string {
    let text = `Ley ${s.number} — ${s.name}`;
    if (s.articles) text += ` art. ${s.articles}`;
    return text;
  }

  askAssistant(rulingId: string, caseTitle: string) {
    this.router.navigate(['/asistente'], {
      queryParams: { rulingId, rulingTitle: caseTitle }
    });
  }

  enqueueReprocess(rulingId: string) {
    if (!confirm('¿Enviar este fallo a reprocesamiento completo? Dejará de estar en búsqueda hasta que termine.')) {
      return;
    }
    this.reprocessBusy.set(true);
    this.reprocessService.enqueue(rulingId, this.reprocessUseCache()).subscribe({
      next: res => {
        this.notify.success(res.message);
        this.reprocessBusy.set(false);
        this.loadRuling(rulingId);
      },
      error: err => {
        this.reprocessBusy.set(false);
        this.notify.error(err?.error?.message ?? err?.error?.error ?? 'No se pudo encolar el reprocesamiento.');
      }
    });
  }

  onGraphNodeClick(entity: { id: string; entityType: string }) {
    if (entity.entityType === 'ruling') {
      const id = entity.id.replace('ruling:', '');
      this.router.navigate(['/jurisprudencia', id]);
    } else if (entity.entityType === 'court') {
      const id = entity.id.replace('court:', '');
      this.router.navigate(['/organismos', id]);
    } else if (entity.entityType === 'person') {
      const id = entity.id.replace('person:', '');
      this.router.navigate(['/sujetos', id]);
    }
  }

  courtOntologySummary(court: Court): string | null {
    const parts: string[] = [];
    if (court.courtCategory) parts.push(court.courtCategory);
    if (court.fuero) parts.push(court.fuero);
    if (court.instanceLevel != null) parts.push(String(court.instanceLevel));
    if (court.governmentLevel) parts.push(court.governmentLevel);
    return parts.length ? parts.join(' · ') : null;
  }
}
