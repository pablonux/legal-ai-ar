/** Domain models for health-check (thin slice). */

export interface HealthCheckSummary {
  readonly id: string;
  readonly status: string;
}
