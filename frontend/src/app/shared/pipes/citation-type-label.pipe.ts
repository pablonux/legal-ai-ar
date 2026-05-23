import { Pipe, PipeTransform } from '@angular/core';

const LABELS: Record<string, string> = {
  CITES: 'Cita',
  UPHOLDS: 'Confirma',
  OVERRULES: 'Revoca',
  DISTINGUISHES: 'Distingue'
};

/**
 * Translates CitationType (CITES, UPHOLDS, OVERRULES, DISTINGUISHES) to Spanish label.
 */
@Pipe({ name: 'citationTypeLabel', standalone: true })
export class CitationTypeLabelPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    return LABELS[value.toUpperCase()] ?? value;
  }
}
