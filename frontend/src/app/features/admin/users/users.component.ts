import { Component, signal, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { A11yModule } from '@angular/cdk/a11y';
import { NotificationService } from '@legal-ai-ar/shared-common/services/notification.service';
import { UserService, type User, type UserRole } from '../../../services/admin/user.service';
import { LoadingSpinnerComponent } from '@legal-ai-ar/shared-common/components/loading-spinner/loading-spinner.component';

const ROLE_LABELS: Record<UserRole, string> = {
  admin: 'Admin',
  lawyer: 'Abogado',
  viewer: 'Lector'
};

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [FormsModule, LoadingSpinnerComponent, A11yModule],
  template: `
    <div class="users">
      @if (state() === 'loading') {
        <div class="state-message" aria-live="polite">
          <app-loading-spinner message="Cargando usuarios..." />
        </div>
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <p>{{ error() }}</p>
          <button type="button" class="retry-btn" (click)="load()">Reintentar</button>
        </div>
      }

      @if (state() === 'loaded') {
        <div class="users-header">
          <h3 class="section-title">Usuarios</h3>
          <button type="button" class="create-btn" (click)="openCreateModal()">
            Crear usuario
          </button>
        </div>

        @if (users().length === 0) {
          <div class="state-message empty">
            <p>No hay usuarios. Crea el primero.</p>
          </div>
        }

        @if (users().length > 0) {
          <div class="table-wrap">
            <table class="users-table">
              <thead>
                <tr>
                  <th>Email</th>
                  <th>Nombre</th>
                  <th>Rol</th>
                  <th>Activo</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                @for (u of users(); track u.id) {
                  <tr [class.inactive]="!u.isActive">
                    <td>{{ u.email }}</td>
                    <td>{{ u.displayName || '—' }}</td>
                    <td>{{ roleLabel(u.role) }}</td>
                    <td>
                      <span class="status-badge" [class.active]="u.isActive">
                        {{ u.isActive ? 'Sí' : 'No' }}
                      </span>
                    </td>
                    <td>
                      <button
                        type="button"
                        class="action-btn edit"
                        [disabled]="!u.isActive || updatingId() === u.id"
                        (click)="openEditModal(u)"
                      >
                        Editar
                      </button>
                      @if (u.isActive) {
                        <button
                          type="button"
                          class="action-btn deactivate"
                          [disabled]="updatingId() === u.id"
                          (click)="openDeactivateConfirm(u)"
                        >
                          Desactivar
                        </button>
                      }
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      }
    </div>

    @if (showCreateModal()) {
      <div class="modal-overlay" (click)="closeCreateModal()" (keydown.escape)="closeCreateModal()">
        <div class="modal" role="dialog" aria-labelledby="create-title" aria-modal="true" cdkTrapFocus cdkTrapFocusAutoCapture (click)="$event.stopPropagation()">
          <h2 id="create-title">Crear usuario</h2>
          <div class="modal-body">
            <div class="form-group">
              <label for="create-email">Email <span class="required">*</span></label>
              <input
                id="create-email"
                type="email"
                [(ngModel)]="createEmail"
                placeholder="usuario@ejemplo.com"
              />
            </div>
            <div class="form-group">
              <label for="create-displayName">Nombre</label>
              <input
                id="create-displayName"
                type="text"
                [(ngModel)]="createDisplayName"
                placeholder="Opcional"
              />
            </div>
            <div class="form-group">
              <label for="create-role">Rol <span class="required">*</span></label>
              <select id="create-role" [(ngModel)]="createRole">
                <option value="admin">Admin</option>
                <option value="lawyer">Abogado</option>
                <option value="viewer">Lector</option>
              </select>
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" (click)="closeCreateModal()">Cancelar</button>
            <button type="button" class="btn-primary" [disabled]="createInProgress() || !createEmail.trim()" (click)="confirmCreate()">
              {{ createInProgress() ? 'Creando...' : 'Crear' }}
            </button>
          </div>
        </div>
      </div>
    }

    @if (showEditModal()) {
      <div class="modal-overlay" (click)="closeEditModal()" (keydown.escape)="closeEditModal()">
        <div class="modal" role="dialog" aria-labelledby="edit-title" aria-modal="true" cdkTrapFocus cdkTrapFocusAutoCapture (click)="$event.stopPropagation()">
          <h2 id="edit-title">Editar usuario</h2>
          <div class="modal-body">
            <div class="form-group">
              <label for="edit-email-display">Email</label>
              <input id="edit-email-display" type="text" [value]="editUser()?.email ?? ''" readonly class="readonly-field" />
            </div>
            <div class="form-group">
              <label for="edit-displayName">Nombre</label>
              <input
                id="edit-displayName"
                type="text"
                [(ngModel)]="editDisplayName"
                placeholder="Opcional"
              />
            </div>
            <div class="form-group">
              <label for="edit-role">Rol <span class="required">*</span></label>
              <select id="edit-role" [(ngModel)]="editRole">
                <option value="admin">Admin</option>
                <option value="lawyer">Abogado</option>
                <option value="viewer">Lector</option>
              </select>
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" (click)="closeEditModal()">Cancelar</button>
            <button type="button" class="btn-primary" [disabled]="editInProgress()" (click)="confirmEdit()">
              {{ editInProgress() ? 'Guardando...' : 'Guardar' }}
            </button>
          </div>
        </div>
      </div>
    }

    @if (showDeactivateConfirm()) {
      <div class="modal-overlay" (click)="cancelDeactivate()" (keydown.escape)="cancelDeactivate()">
        <div class="modal modal-sm" role="dialog" aria-modal="true" cdkTrapFocus cdkTrapFocusAutoCapture (click)="$event.stopPropagation()">
          <h2>¿Desactivar usuario {{ deactivateUser()?.email }}?</h2>
          <p>No podrá iniciar sesión.</p>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" (click)="cancelDeactivate()">Cancelar</button>
            <button type="button" class="btn-primary" [disabled]="updatingId() !== null" (click)="confirmDeactivate()">
              Desactivar
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .users { position: relative; }
    .state-message {
      text-align: center;
      padding: 3rem 2rem;
      color: var(--color-text-body);
    }
    .state-message.error { color: var(--color-primary); }
    .state-message.empty { color: var(--color-text-secondary); }
    .retry-btn {
      margin-top: 1rem;
      padding: 0.5rem 1rem;
      background: none;
      border: 1px solid var(--color-primary);
      color: var(--color-primary);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
    }
    .users-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }
    .section-title { margin: 0; font-size: 1.1rem; }
    .create-btn {
      padding: 0.5rem 1rem;
      font-size: 0.875rem;
      font-weight: 600;
      background: var(--color-primary);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .create-btn:hover { background: var(--color-primary-hover); }
    .table-wrap { overflow-x: auto; }
    .users-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }
    .users-table th,
    .users-table td {
      padding: 0.875rem 1rem;
      text-align: left;
      border-bottom: 1px solid var(--color-border);
    }
    .users-table th {
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
    }
    .users-table tr.inactive { opacity: 0.7; }
    .status-badge {
      font-size: 0.75rem;
      padding: 2px 8px;
      border-radius: var(--radius-pill);
      background: #f0f0f0;
      color: var(--color-text-secondary);
    }
    .status-badge.active { background: var(--color-success-bg); color: var(--color-success); }
    .action-btn {
      padding: 0.35rem 0.75rem;
      font-size: 0.8125rem;
      margin-right: 0.5rem;
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .action-btn.edit {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
    }
    .action-btn.edit:hover:not(:disabled) { background: var(--color-bg-hover); }
    .action-btn.deactivate {
      background: none;
      border: 1px solid var(--color-primary);
      color: var(--color-primary);
    }
    .action-btn.deactivate:hover:not(:disabled) { background: var(--color-primary-light); }
    .action-btn:disabled { opacity: 0.6; cursor: not-allowed; }
    .modal-overlay {
      position: fixed;
      inset: 0;
      background: rgba(0,0,0,0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 200;
    }
    .modal {
      background: var(--color-bg-surface);
      border-radius: var(--radius-md);
      padding: 2rem;
      max-width: 400px;
      width: 90%;
      box-shadow: var(--shadow-lg);
    }
    .modal h2 { font-size: 1.1rem; margin: 0 0 1rem 0; }
    .modal-body { margin-bottom: 1.5rem; }
    .form-group { margin-bottom: 1rem; }
    .form-group label {
      display: block;
      font-size: 0.85rem;
      margin-bottom: 0.25rem;
      color: var(--color-text-body);
    }
    .required { color: var(--color-primary); }
    .form-group input,
    .form-group select {
      width: 100%;
      height: 2.5rem;
      padding: 0 1rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      font-size: 0.875rem;
    }
    .form-group input.readonly-field {
      margin: 0;
      padding: 0 0.875rem;
      height: 2.5rem;
      line-height: 2.5rem;
      font-size: 0.875rem;
      color: var(--color-text-body);
      background: var(--color-bg-subtle);
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-border);
    }
    .modal.modal-sm { max-width: 360px; }
    .modal.modal-sm p { margin: 0 0 1rem 0; font-size: 0.875rem; color: var(--color-text-body); }
    .modal-actions {
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
    }
    .btn-secondary {
      padding: 0.5rem 1rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
      transition: all var(--transition-base);
    }
    .btn-primary {
      padding: 0.5rem 1rem;
      background: var(--color-primary);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
      transition: all var(--transition-base);
    }
    .btn-primary:hover:not(:disabled) { background: var(--color-primary-hover); }
    .btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
  `]
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private notify = inject(NotificationService);

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  users = signal<User[]>([]);
  error = signal<string>('');
  updatingId = signal<string | null>(null);

  showCreateModal = signal(false);
  createEmail = '';
  createDisplayName = '';
  createRole: UserRole = 'viewer';
  createInProgress = signal(false);

  showEditModal = signal(false);
  editUser = signal<User | null>(null);
  editDisplayName = '';
  editRole: UserRole = 'viewer';
  editInProgress = signal(false);

  showDeactivateConfirm = signal(false);
  deactivateUser = signal<User | null>(null);

  ngOnInit() {
    this.load();
  }

  load() {
    this.state.set('loading');
    this.error.set('');
    this.userService.getUsers().subscribe({
      next: (list) => {
        this.users.set(list);
        this.state.set('loaded');
      },
      error: (err) => {
        this.error.set(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al cargar.');
        this.state.set('error');
      }
    });
  }

  roleLabel(role: UserRole): string {
    return ROLE_LABELS[role] ?? role;
  }

  showToast(msg: string, isError = false) {
    if (isError) {
      this.notify.error(msg);
    } else {
      this.notify.success(msg);
    }
  }

  openCreateModal() {
    this.createEmail = '';
    this.createDisplayName = '';
    this.createRole = 'viewer';
    this.showCreateModal.set(true);
  }

  closeCreateModal() {
    this.showCreateModal.set(false);
  }

  confirmCreate() {
    const email = this.createEmail.trim();
    if (!email) return;

    this.createInProgress.set(true);
    this.userService.createUser({
      email,
      displayName: this.createDisplayName.trim() || undefined,
      role: this.createRole
    }).subscribe({
      next: () => {
        this.closeCreateModal();
        this.createInProgress.set(false);
        this.load();
        this.showToast('Usuario creado.');
      },
      error: (err) => {
        this.createInProgress.set(false);
        this.showToast(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al crear.', true);
      }
    });
  }

  openEditModal(u: User) {
    this.editUser.set(u);
    this.editDisplayName = u.displayName ?? '';
    this.editRole = u.role;
    this.showEditModal.set(true);
  }

  closeEditModal() {
    this.showEditModal.set(false);
    this.editUser.set(null);
  }

  confirmEdit() {
    const u = this.editUser();
    if (!u) return;

    this.editInProgress.set(true);
    this.userService.updateUser(u.id, {
      displayName: this.editDisplayName.trim() || undefined,
      role: this.editRole
    }).subscribe({
      next: () => {
        this.closeEditModal();
        this.editInProgress.set(false);
        this.load();
        this.showToast('Usuario actualizado.');
      },
      error: (err) => {
        this.editInProgress.set(false);
        this.showToast(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al actualizar.', true);
      }
    });
  }

  openDeactivateConfirm(u: User) {
    this.deactivateUser.set(u);
    this.showDeactivateConfirm.set(true);
  }

  cancelDeactivate() {
    this.showDeactivateConfirm.set(false);
    this.deactivateUser.set(null);
  }

  confirmDeactivate() {
    const u = this.deactivateUser();
    if (!u) return;

    this.updatingId.set(u.id);
    this.userService.deleteUser(u.id).subscribe({
      next: () => {
        this.cancelDeactivate();
        this.updatingId.set(null);
        this.load();
        this.showToast('Usuario desactivado.');
      },
      error: (err) => {
        this.updatingId.set(null);
        this.showToast(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al desactivar.', true);
      }
    });
  }
}
