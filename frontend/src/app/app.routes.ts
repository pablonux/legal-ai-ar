import { Routes } from '@angular/router';
import { authGuard } from '@legal-ai-ar/core';
import { ShellComponent } from '@legal-ai-ar/shell';
import { SearchHomeComponent } from './features/search/search-home/search-home.component';
import { SearchResultsComponent } from './features/search/search-results/search-results.component';
import { RulingDetailComponent } from './features/search/ruling-detail/ruling-detail.component';
import { ChatViewComponent } from './features/chat/chat-view/chat-view.component';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { DeadLetterQueueComponent } from './features/admin/dead-letter-queue/dead-letter-queue.component';
import { UsersComponent } from './features/admin/users/users.component';
import { WelcomeComponent } from './features/welcome/welcome.component';
import { KbDashboardComponent } from './features/kb-dashboard/kb-dashboard.component';
import { CourtsListComponent } from './features/catalogs/courts-list/courts-list.component';
import { CourtDetailComponent } from './features/catalogs/court-detail/court-detail.component';
import { PersonsListComponent } from './features/catalogs/persons-list/persons-list.component';
import { PersonDetailComponent } from './features/catalogs/person-detail/person-detail.component';
import { ThesaurusListComponent } from './features/catalogs/thesaurus-list/thesaurus-list.component';
import { ThesaurusDetailComponent } from './features/catalogs/thesaurus-detail/thesaurus-detail.component';

export const routes: Routes = [
  { path: 'login', redirectTo: '/bienvenida', pathMatch: 'full' },
  {
    path: 'sesion-requerida',
    loadComponent: () =>
      import('./features/auth/session-required/session-required.component').then(
        (m) => m.SessionRequiredComponent
      )
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: '/bienvenida', pathMatch: 'full' },
      { path: 'bienvenida', component: WelcomeComponent },

      // --- Ontological spaces ---
      { path: 'jurisprudencia', component: SearchHomeComponent },
      { path: 'jurisprudencia/resultados', component: SearchResultsComponent },
      { path: 'jurisprudencia/:id', component: RulingDetailComponent },
      { path: 'organismos', component: CourtsListComponent },
      { path: 'organismos/:id', component: CourtDetailComponent },
      { path: 'sujetos', component: PersonsListComponent },
      { path: 'sujetos/:id', component: PersonDetailComponent },
      { path: 'vocabulario', component: ThesaurusListComponent },
      { path: 'vocabulario/:id', component: ThesaurusDetailComponent },
      {
        path: 'ordenamiento',
        loadComponent: () =>
          import('./features/statutes/statute-list/statute-list.component').then(
            (m) => m.StatuteListComponent
          )
      },
      {
        path: 'ordenamiento/piramide',
        loadComponent: () =>
          import('./features/statutes/normative-pyramid/normative-pyramid.component').then(
            (m) => m.NormativePyramidComponent
          )
      },
      {
        path: 'ordenamiento/:id',
        loadComponent: () =>
          import('./features/statutes/statute-detail/statute-detail.component').then(
            (m) => m.StatuteDetailComponent
          )
      },
      {
        path: 'procesos',
        loadComponent: () =>
          import('./features/proceedings/proceeding-list/proceeding-list.component').then(
            (m) => m.ProceedingListComponent
          )
      },
      {
        path: 'procesos/:id',
        loadComponent: () =>
          import('./features/proceedings/proceeding-detail/proceeding-detail.component').then(
            (m) => m.ProceedingDetailComponent
          )
      },
      { path: 'causas', redirectTo: '/procesos', pathMatch: 'full' },

      // --- Tools ---
      { path: 'asistente', component: ChatViewComponent },
      { path: 'estadisticas', component: KbDashboardComponent },
      {
        path: 'ontologia',
        loadComponent: () =>
          import('./features/ontology/ontology-page/ontology-page.component').then(
            (m) => m.OntologyPageComponent
          )
      },
      {
        path: 'explorador',
        loadComponent: () =>
          import('./features/graph-explorer/graph-explorer-page/graph-explorer-page.component').then(
            (m) => m.GraphExplorerPageComponent
          )
      },

      // --- Admin ---
      {
        path: 'admin',
        component: AdminLayoutComponent,
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/admin/ingestion/ingestion.component').then(
                (m) => m.IngestionComponent
              )
          },
          { path: 'dlq', component: DeadLetterQueueComponent },
          {
            path: 'reproceso',
            loadComponent: () =>
              import('./features/admin/reprocess-queue/reprocess-queue.component').then(
                (m) => m.ReprocessQueueComponent
              )
          },
          { path: 'users', component: UsersComponent },
          {
            path: 'jobs/:jobId',
            loadComponent: () =>
              import('./features/admin/jobs/job-detail/job-detail.component').then(
                (m) => m.JobDetailComponent
              )
          },
          {
            path: ':entityType',
            loadComponent: () =>
              import('./features/admin/entity-sources/entity-sources.component').then(
                (m) => m.EntitySourcesComponent
              )
          },
          {
            path: ':entityType/sources/:sourceId/jobs',
            loadComponent: () =>
              import('./features/admin/source-jobs/source-jobs.component').then(
                (m) => m.SourceJobsComponent
              )
          },
          {
            path: ':entityType/sources/:sourceId/jobs/:jobId',
            loadComponent: () =>
              import('./features/admin/jobs/job-detail/job-detail.component').then(
                (m) => m.JobDetailComponent
              )
          }
        ]
      },

      {
        path: 'salud',
        loadChildren: () =>
          import('./features/health-check/health-check.routes').then((m) => m.healthCheckRoutes)
      },

      // --- Backward compatibility redirects (F2.2) ---
      { path: 'buscar', redirectTo: '/jurisprudencia', pathMatch: 'full' },
      { path: 'buscar/resultados', redirectTo: '/jurisprudencia/resultados', pathMatch: 'full' },
      { path: 'fallos/:id', redirectTo: '/jurisprudencia/:id' },
      { path: 'chat', redirectTo: '/asistente', pathMatch: 'full' },
      { path: 'catalogos', redirectTo: '/organismos', pathMatch: 'full' },
      { path: 'catalogos/tribunales', redirectTo: '/organismos', pathMatch: 'full' },
      { path: 'catalogos/tribunales/:id', redirectTo: '/organismos/:id' },
      { path: 'catalogos/personas', redirectTo: '/sujetos', pathMatch: 'full' },
      { path: 'catalogos/personas/:id', redirectTo: '/sujetos/:id' },
      { path: 'catalogos/tesauro', redirectTo: '/vocabulario', pathMatch: 'full' },
      { path: 'catalogos/tesauro/:id', redirectTo: '/vocabulario/:id' },
      { path: 'dashboard', redirectTo: '/estadisticas', pathMatch: 'full' },

      { path: '**', redirectTo: '/bienvenida' }
    ]
  }
];
