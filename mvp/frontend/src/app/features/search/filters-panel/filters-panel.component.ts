import { Component, input, output, effect, computed, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';
import { RulingService } from '../../../services/ruling.service';
import { ThesaurusService } from '../../../services/thesaurus.service';
import type { SearchFiltersRequest, SearchFacets } from '../../../models/ruling.models';
import type { ThesaurusTerm } from '../../../models/thesaurus.models';

@Component({
  selector: 'app-filters-panel',
  standalone: true,
  imports: [FormsModule, MatDatepickerModule, MatFormFieldModule, MatInputModule],
  template: `
    @if (inline()) {
      <!-- Inline mode: grid layout for search page -->
      <div class="inline-filters">
        <div class="inline-header">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
          </svg>
          <h2>Filtros</h2>
          @if (activeFilterCount() > 0) {
            <span class="inline-count">{{ activeFilterCount() }} activo{{ activeFilterCount() > 1 ? 's' : '' }}</span>
            <button type="button" class="inline-clear" (click)="clearFilters()">
              <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg>
              Limpiar
            </button>
          }
        </div>
        @if (facetsLoading()) {
          <p class="facets-status">Cargando filtros...</p>
        }
        @if (facetsError()) {
          <p class="facets-status facets-error">
            No se pudieron cargar los valores de filtro.
            <button type="button" class="retry-link" (click)="loadFacets()">Reintentar</button>
          </p>
        }
        <div class="inline-grid">
          <div class="filter-field">
            <label for="i-jurisdictionArea">Área jurisdiccional</label>
            <select id="i-jurisdictionArea" [(ngModel)]="localFilters.jurisdictionArea" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todas</option>
              @for (f of facets()?.jurisdictionAreas ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-instance">Instancia</label>
            <select id="i-instance" [(ngModel)]="localFilters.instance" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todas</option>
              @for (f of facets()?.instances ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-court">Tribunal</label>
            <select id="i-court" [(ngModel)]="localFilters.court" (ngModelChange)="onCourtChange()">
              <option [ngValue]="undefined">Todos</option>
              @for (f of facets()?.courts ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-courtType">Tipo de tribunal</label>
            <select id="i-courtType" [(ngModel)]="localFilters.courtType" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todos</option>
              @for (f of facets()?.courtTypes ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-fuero">Fuero</label>
            <select id="i-fuero" [(ngModel)]="localFilters.fuero" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todos</option>
              @for (f of facets()?.fueros ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-legalBranch">Rama del derecho</label>
            <select id="i-legalBranch" [(ngModel)]="localFilters.legalBranch" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todas</option>
              @for (f of facets()?.legalBranches ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-precedentWeight">Peso precedencial</label>
            <select id="i-precedentWeight" [(ngModel)]="localFilters.precedentWeight" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todos</option>
              @for (f of facets()?.precedentWeights ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label for="i-subjectArea">Materia</label>
            <select id="i-subjectArea" [(ngModel)]="localFilters.subjectArea" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todas</option>
              @for (f of facets()?.subjectAreas ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field">
            <label>Desde fecha</label>
            <mat-form-field appearance="outline" class="date-field" [floatLabel]="'always'">
              <input matInput [matDatepicker]="iPickerFrom" placeholder="dd/mm/aaaa" [value]="toDate(localFilters.dateFrom)" (dateChange)="onDateChange('dateFrom', $event.value)" />
              <mat-datepicker-toggle matIconSuffix [for]="iPickerFrom" />
              <mat-datepicker #iPickerFrom />
            </mat-form-field>
          </div>
          <div class="filter-field">
            <label>Hasta fecha</label>
            <mat-form-field appearance="outline" class="date-field" [floatLabel]="'always'">
              <input matInput [matDatepicker]="iPickerTo" placeholder="dd/mm/aaaa" [value]="toDate(localFilters.dateTo)" (dateChange)="onDateChange('dateTo', $event.value)" />
              <mat-datepicker-toggle matIconSuffix [for]="iPickerTo" />
              <mat-datepicker #iPickerTo />
            </mat-form-field>
          </div>
          <div class="filter-field">
            <label for="i-resourceType">Tipo de recurso</label>
            <select id="i-resourceType" [(ngModel)]="localFilters.resourceType" (ngModelChange)="emitFilters()">
              <option [ngValue]="undefined">Todos</option>
              @for (f of facets()?.resourceTypes ?? []; track f.value) {
                <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
              }
            </select>
          </div>
          <div class="filter-field filter-checkbox">
            <label>
              <input type="checkbox" [ngModel]="localFilters.isUnconstitutional ?? false" (ngModelChange)="onUnconstitutionalChange($event)" />
              <span>Solo inconstitucionalidad</span>
            </label>
          </div>
        </div>
        <div class="inline-keywords">
          <div class="filter-field kw-autocomplete">
            <label for="i-keywords">Palabras clave (tesauro SAIJ)</label>
            @if (selectedKeywords().length) {
              <div class="kw-chips">
                @for (kw of selectedKeywords(); track kw) {
                  <span class="kw-chip">
                    {{ kw }}
                    <button type="button" class="kw-chip-x" (click)="removeKeyword(kw)" [attr.aria-label]="'Quitar ' + kw">&times;</button>
                  </span>
                }
              </div>
            }
            <input
              id="i-keywords"
              type="text"
              [(ngModel)]="keywordsText"
              (ngModelChange)="onKeywordInput($event)"
              (keydown.enter)="addFreeKeyword($event)"
              (focus)="showSuggestions = true"
              (blur)="onBlur()"
              placeholder="Buscar descriptor..."
              autocomplete="off"
            />
            @if (showSuggestions && suggestions().length) {
              <ul class="kw-suggestions">
                @for (s of suggestions(); track s.id) {
                  <li (mousedown)="selectSuggestion(s)">
                    <span class="sug-label">{{ s.label }}</span>
                    @if (s.branch) {
                      <span class="sug-branch">{{ s.branch }}</span>
                    }
                  </li>
                }
              </ul>
            }
          </div>
        </div>
        @if (dateError) {
          <p class="filter-error" role="alert">
            <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
            {{ dateError }}
          </p>
        }
      </div>
    } @else {
      <!-- Sidebar mode: collapsed pill / expanded card -->
      @if (!isExpanded()) {
        <button
          type="button"
          class="side-pill"
          [class.has-data]="activeFilterCount() > 0"
          (click)="toggleExpanded()"
          title="Filtros avanzados"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
          </svg>
          @if (activeFilterCount() > 0) {
            <span class="pill-badge">{{ activeFilterCount() }}</span>
          }
        </button>
      }

      @if (isExpanded()) {
        <section class="side-card">

          <div class="side-card-header" (click)="toggleExpanded()">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
              <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
            </svg>
            <h2>Filtros</h2>
            @if (activeFilterCount() > 0) {
              <span class="side-card-count">{{ activeFilterCount() }}</span>
            }
            <svg xmlns="http://www.w3.org/2000/svg" class="collapse-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <polyline points="18 15 12 9 6 15"/>
            </svg>
          </div>

          <div class="side-card-body">
            <div class="filter-field">
              <label for="jurisdictionArea">Área jurisdiccional</label>
              <select id="jurisdictionArea" [(ngModel)]="localFilters.jurisdictionArea" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todas</option>
                @for (f of facets()?.jurisdictionAreas ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="instance">Instancia</label>
              <select id="instance" [(ngModel)]="localFilters.instance" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todas</option>
                @for (f of facets()?.instances ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="court">Tribunal</label>
              <select id="court" [(ngModel)]="localFilters.court" (ngModelChange)="onCourtChange()">
                <option [ngValue]="undefined">Todos</option>
                @for (f of facets()?.courts ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="s-courtType">Tipo de tribunal</label>
              <select id="s-courtType" [(ngModel)]="localFilters.courtType" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todos</option>
                @for (f of facets()?.courtTypes ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="s-fuero">Fuero</label>
              <select id="s-fuero" [(ngModel)]="localFilters.fuero" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todos</option>
                @for (f of facets()?.fueros ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="s-legalBranch">Rama del derecho</label>
              <select id="s-legalBranch" [(ngModel)]="localFilters.legalBranch" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todas</option>
                @for (f of facets()?.legalBranches ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="s-precedentWeight">Peso precedencial</label>
              <select id="s-precedentWeight" [(ngModel)]="localFilters.precedentWeight" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todos</option>
                @for (f of facets()?.precedentWeights ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label>Desde fecha</label>
              <mat-form-field appearance="outline" class="date-field" [floatLabel]="'always'">
                <input matInput [matDatepicker]="pickerFrom" placeholder="dd/mm/aaaa" [value]="toDate(localFilters.dateFrom)" (dateChange)="onDateChange('dateFrom', $event.value)" />
                <mat-datepicker-toggle matIconSuffix [for]="pickerFrom" />
                <mat-datepicker #pickerFrom />
              </mat-form-field>
            </div>
            <div class="filter-field">
              <label>Hasta fecha</label>
              <mat-form-field appearance="outline" class="date-field" [floatLabel]="'always'">
                <input matInput [matDatepicker]="pickerTo" placeholder="dd/mm/aaaa" [value]="toDate(localFilters.dateTo)" (dateChange)="onDateChange('dateTo', $event.value)" />
                <mat-datepicker-toggle matIconSuffix [for]="pickerTo" />
                <mat-datepicker #pickerTo />
              </mat-form-field>
            </div>
            <div class="filter-field">
              <label for="subjectArea">Materia</label>
              <select id="subjectArea" [(ngModel)]="localFilters.subjectArea" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todas</option>
                @for (f of facets()?.subjectAreas ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field">
              <label for="resourceType">Tipo de recurso</label>
              <select id="resourceType" [(ngModel)]="localFilters.resourceType" (ngModelChange)="emitFilters()">
                <option [ngValue]="undefined">Todos</option>
                @for (f of facets()?.resourceTypes ?? []; track f.value) {
                  <option [value]="f.value">{{ f.value }} ({{ f.count }})</option>
                }
              </select>
            </div>
            <div class="filter-field filter-checkbox">
              <label>
                <input type="checkbox" [ngModel]="localFilters.isUnconstitutional ?? false" (ngModelChange)="onUnconstitutionalChange($event)" />
                <span>Solo inconstitucionalidad</span>
              </label>
            </div>
            <div class="filter-field kw-autocomplete">
              <label for="keywords">Palabras clave</label>
              @if (selectedKeywords().length) {
                <div class="kw-chips">
                  @for (kw of selectedKeywords(); track kw) {
                    <span class="kw-chip">
                      {{ kw }}
                      <button type="button" class="kw-chip-x" (click)="removeKeyword(kw)" [attr.aria-label]="'Quitar ' + kw">&times;</button>
                    </span>
                  }
                </div>
              }
              <input
                id="keywords"
                type="text"
                [(ngModel)]="keywordsText"
                (ngModelChange)="onKeywordInput($event)"
                (keydown.enter)="addFreeKeyword($event)"
                (focus)="showSuggestions = true"
                (blur)="onBlur()"
                placeholder="Buscar descriptor..."
                autocomplete="off"
              />
              @if (showSuggestions && suggestions().length) {
                <ul class="kw-suggestions">
                  @for (s of suggestions(); track s.id) {
                    <li (mousedown)="selectSuggestion(s)">
                      <span class="sug-label">{{ s.label }}</span>
                      @if (s.branch) {
                        <span class="sug-branch">{{ s.branch }}</span>
                      }
                    </li>
                  }
                </ul>
              }
            </div>

            @if (dateError) {
              <p class="filter-error" role="alert">
                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
                {{ dateError }}
              </p>
            }

            <button type="button" class="clear-btn" (click)="clearFilters()" [disabled]="activeFilterCount() === 0">
              <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg>
              Limpiar filtros
            </button>
          </div>

        </section>
      }
    }
  `,
  styles: [`
    :host {
      display: contents;
    }

    /* ── Inline mode ── */

    .inline-filters {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      padding: 1rem 1.25rem;
    }

    .inline-header {
      display: flex;
      align-items: center;
      gap: 6px;
      margin-bottom: 0.75rem;
      color: var(--color-primary);
    }

    .inline-header h2 {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-primary);
      margin: 0;
      flex: 1;
    }

    .inline-count {
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-primary);
      background: rgba(208, 74, 2, 0.08);
      padding: 2px 8px;
      border-radius: 100px;
    }

    .inline-clear {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      background: none;
      border: none;
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-primary);
      cursor: pointer;
      padding: 2px 6px;
      border-radius: 4px;
      transition: background 0.15s;
    }

    .inline-clear:hover {
      background: rgba(208, 74, 2, 0.06);
    }

    .inline-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 0.625rem;
    }

    .inline-keywords {
      margin-top: 0.625rem;
    }

    .facets-status {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin: 0 0 0.5rem;
    }

    .facets-error {
      color: var(--color-primary);
    }

    .retry-link {
      background: none;
      border: none;
      color: var(--color-primary);
      text-decoration: underline;
      font-size: 0.75rem;
      cursor: pointer;
      padding: 0;
      margin-left: 4px;
    }

    /* ── Collapsed pill (identical to chat side-pill) ── */

    .side-pill {
      width: 36px;
      height: 36px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      color: var(--color-text-secondary);
      cursor: pointer;
      position: relative;
      transition: border-color 0.15s, color 0.15s, box-shadow 0.15s;
      flex-shrink: 0;
    }

    .side-pill:hover {
      border-color: rgba(208, 74, 2, 0.3);
      color: var(--color-primary);
      box-shadow: 0 1px 3px rgba(0,0,0,0.08);
    }

    .side-pill.has-data {
      border-color: rgba(208, 74, 2, 0.35);
      color: var(--color-primary);
    }

    .side-pill.has-data:hover {
      border-color: var(--color-primary);
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

    /* ── Expanded card (identical to chat side-card) ── */

    .side-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: 0 1px 3px rgba(0,0,0,0.04);
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
    }

    .side-card-count {
      font-size: 0.5625rem;
      font-weight: 600;
      color: #fff;
      background: var(--color-primary);
      border-radius: 100px;
      padding: 1px 5px;
      min-width: 16px;
      text-align: center;
    }

    .collapse-icon {
      color: var(--color-text-secondary);
      flex-shrink: 0;
      opacity: 0.5;
    }

    .side-card-header:hover .collapse-icon { opacity: 1; }

    .side-card-body {
      flex: 1;
      overflow-y: auto;
      padding: 0.625rem 0.75rem;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .side-card-body::-webkit-scrollbar { width: 4px; }
    .side-card-body::-webkit-scrollbar-track { background: transparent; }
    .side-card-body::-webkit-scrollbar-thumb {
      background: var(--color-border);
      border-radius: 2px;
    }

    /* ── Filter fields ── */

    .filter-field label {
      display: block;
      font-size: 0.5625rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
      margin-bottom: 3px;
    }

    .filter-field input,
    .filter-field select {
      width: 100%;
      height: 2rem;
      padding: 0 0.625rem;
      border: 1px solid var(--color-border-input);
      border-radius: 6px;
      font-size: 0.75rem;
      font-family: inherit;
      color: var(--color-text);
      background: var(--color-bg-subtle, #fafafa);
      box-sizing: border-box;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .filter-field input:focus,
    .filter-field select:focus {
      outline: none;
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.08);
      background: var(--color-bg-surface);
    }

    .filter-checkbox {
      align-self: end;
      padding-bottom: 0.25rem;
    }

    .filter-checkbox label {
      display: flex;
      align-items: center;
      gap: 6px;
      cursor: pointer;
      font-size: 0.75rem;
      color: var(--color-text);
    }

    .filter-checkbox input[type="checkbox"] {
      width: auto;
      height: auto;
      accent-color: var(--color-primary);
      cursor: pointer;
    }

    .date-field {
      width: 100%;
    }

    /* ── Error ── */

    .filter-error {
      display: flex;
      align-items: center;
      gap: 4px;
      color: var(--color-primary);
      font-size: 0.75rem;
      margin: 0;
    }

    /* ── Clear button ── */

    .clear-btn {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      width: 100%;
      justify-content: center;
      margin-top: 0.25rem;
      padding: 0.375rem 0.5rem;
      background: var(--color-primary-light, rgba(208, 74, 2, 0.06));
      border: 1px solid rgba(208, 74, 2, 0.15);
      border-radius: 8px;
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-primary);
      cursor: pointer;
      transition: box-shadow 0.15s, border-color 0.15s;
    }

    .clear-btn:hover:not(:disabled) {
      box-shadow: 0 1px 3px rgba(0,0,0,0.04);
      border-color: rgba(208, 74, 2, 0.3);
    }

    .clear-btn:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }

    /* ── Keyword autocomplete ── */
    .kw-autocomplete { position: relative; }

    .kw-chips {
      display: flex;
      flex-wrap: wrap;
      gap: 4px;
      margin-bottom: 4px;
    }

    .kw-chip {
      display: inline-flex;
      align-items: center;
      gap: 3px;
      background: var(--color-primary-light, rgba(208, 74, 2, 0.08));
      border: 1px solid rgba(208, 74, 2, 0.2);
      color: var(--color-primary);
      padding: 1px 6px 1px 8px;
      border-radius: var(--radius-pill, 100px);
      font-size: 0.625rem;
      font-weight: 500;
      max-width: 100%;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .kw-chip-x {
      background: none;
      border: none;
      color: var(--color-primary);
      font-size: 0.8125rem;
      line-height: 1;
      padding: 0 1px;
      cursor: pointer;
      opacity: 0.6;
    }
    .kw-chip-x:hover { opacity: 1; }

    .kw-suggestions {
      position: absolute;
      left: 0;
      right: 0;
      top: 100%;
      z-index: 50;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-top: none;
      border-radius: 0 0 6px 6px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
      max-height: 180px;
      overflow-y: auto;
      list-style: none;
      margin: 0;
      padding: 2px 0;
    }

    .kw-suggestions li {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 5px 10px;
      font-size: 0.75rem;
      cursor: pointer;
      transition: background 0.1s;
    }

    .kw-suggestions li:hover {
      background: var(--color-primary-light, rgba(208, 74, 2, 0.06));
    }

    .sug-label { color: var(--color-text); font-weight: 500; }
    .sug-branch {
      font-size: 0.625rem;
      color: var(--color-text-secondary);
      margin-left: 6px;
      flex-shrink: 0;
    }
  `]
})
export class FiltersPanelComponent implements OnInit {
  private rulingService = inject(RulingService);
  private thesaurusService = inject(ThesaurusService);

  inline = input(false);
  isExpanded = input(false);
  filters = input<SearchFiltersRequest>({});

  expandedChange = output<boolean>();
  filtersChange = output<SearchFiltersRequest>();

  facets = signal<SearchFacets | null>(null);
  facetsLoading = signal(true);
  facetsError = signal(false);
  suggestions = signal<ThesaurusTerm[]>([]);
  selectedKeywords = signal<string[]>([]);
  protected localFilters: SearchFiltersRequest = {};
  protected keywordsText = '';
  protected showSuggestions = false;
  protected dateError: string | null = null;

  private searchSubject = new Subject<string>();

  activeFilterCount = computed(() => {
    const f = this.filters();
    let count = 0;
    if (f.jurisdictionArea) count++;
    if (f.instance) count++;
    if (f.court) count++;
    if (f.courtType) count++;
    if (f.fuero) count++;
    if (f.legalBranch) count++;
    if (f.precedentWeight) count++;
    if (f.dateFrom) count++;
    if (f.dateTo) count++;
    if (f.keywords?.length) count++;
    if (f.subjectArea) count++;
    if (f.resourceType) count++;
    if (f.isUnconstitutional) count++;
    return count;
  });

  constructor() {
    effect(() => {
      const f = this.filters();
      this.localFilters = { ...f };
      this.selectedKeywords.set(f.keywords?.slice() ?? []);
      this.keywordsText = '';
    }, { allowSignalWrites: true });

    this.searchSubject
      .pipe(
        debounceTime(250),
        distinctUntilChanged(),
        switchMap(q => q.length >= 2 ? this.thesaurusService.search(q, 8) : of([]))
      )
      .subscribe(results => {
        const current = new Set(this.selectedKeywords());
        this.suggestions.set(results.filter(r => !current.has(r.label)));
      });
  }

  ngOnInit() {
    this.loadFacets();
  }

  loadFacets() {
    this.facetsLoading.set(true);
    this.facetsError.set(false);
    this.rulingService.getFacets().subscribe({
      next: (f) => {
        this.facets.set(f);
        this.facetsLoading.set(false);
      },
      error: () => {
        this.facets.set(null);
        this.facetsLoading.set(false);
        this.facetsError.set(true);
      }
    });
  }

  toggleExpanded() {
    this.expandedChange.emit(!this.isExpanded());
  }

  protected onCourtChange() {
    this.localFilters.courtId = undefined;
    this.emitFilters();
  }

  protected onUnconstitutionalChange(checked: boolean) {
    this.localFilters.isUnconstitutional = checked || undefined;
    this.emitFilters();
  }

  protected onKeywordInput(value: string) {
    this.searchSubject.next(value.trim());
  }

  protected selectSuggestion(term: ThesaurusTerm) {
    const kws = [...this.selectedKeywords(), term.label];
    this.selectedKeywords.set(kws);
    this.keywordsText = '';
    this.suggestions.set([]);
    this.showSuggestions = false;
    this.syncKeywordsToFilter(kws);
  }

  protected addFreeKeyword(event: Event) {
    event.preventDefault();
    const text = this.keywordsText.trim();
    if (!text) return;
    const kws = [...this.selectedKeywords(), text];
    this.selectedKeywords.set(kws);
    this.keywordsText = '';
    this.suggestions.set([]);
    this.syncKeywordsToFilter(kws);
  }

  protected removeKeyword(keyword: string) {
    const kws = this.selectedKeywords().filter(k => k !== keyword);
    this.selectedKeywords.set(kws);
    this.syncKeywordsToFilter(kws);
  }

  protected onBlur() {
    setTimeout(() => { this.showSuggestions = false; }, 200);
  }

  private syncKeywordsToFilter(kws: string[]) {
    this.localFilters.keywords = kws.length ? kws : undefined;
    this.emitFilters();
  }

  protected toDate(dateStr?: string): Date | null {
    return dateStr ? new Date(dateStr + 'T00:00:00') : null;
  }

  protected onDateChange(field: 'dateFrom' | 'dateTo', value: Date | null) {
    if (value && !isNaN(value.getTime())) {
      const y = value.getFullYear();
      const m = String(value.getMonth() + 1).padStart(2, '0');
      const d = String(value.getDate()).padStart(2, '0');
      this.localFilters[field] = `${y}-${m}-${d}`;
    } else {
      this.localFilters[field] = undefined;
    }
    this.emitFilters();
  }

  protected emitFilters() {
    this.validateDates();
    this.filtersChange.emit({ ...this.localFilters });
  }

  private validateDates() {
    const from = this.localFilters.dateFrom;
    const to = this.localFilters.dateTo;
    if (from && to && from > to) {
      this.dateError = 'La fecha desde debe ser menor o igual a la fecha hasta.';
    } else {
      this.dateError = null;
    }
  }

  clearFilters() {
    this.localFilters = {};
    this.keywordsText = '';
    this.selectedKeywords.set([]);
    this.suggestions.set([]);
    this.dateError = null;
    this.filtersChange.emit({});
  }
}
