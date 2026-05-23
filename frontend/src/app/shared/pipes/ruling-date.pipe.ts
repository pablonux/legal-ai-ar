import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats a date string (ISO 8601 or similar) as dd/MM/yyyy (es-AR).
 */
@Pipe({ name: 'rulingDate', standalone: true })
export class RulingDatePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleDateString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }
}
