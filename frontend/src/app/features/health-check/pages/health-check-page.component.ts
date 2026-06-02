import { Component } from '@angular/core';
import { UiSectionComponent } from '@legal-ai-ar/ui';

@Component({
  selector: 'app-health-check-page',
  standalone: true,
  imports: [UiSectionComponent],
  template: `
    <app-ui-section title="Estado del sistema">
      <p>Verificación de disponibilidad de la aplicación.</p>
    </app-ui-section>
  `
})
export class HealthCheckPageComponent {}
