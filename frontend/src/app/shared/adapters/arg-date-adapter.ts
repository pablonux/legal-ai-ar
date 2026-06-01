import { Injectable } from '@angular/core';
import { NativeDateAdapter } from '@angular/material/core';

/**
 * Custom DateAdapter for es-AR locale.
 * NativeDateAdapter.parse() delegates to Date.parse() which cannot handle
 * dd/mm/yyyy — the standard Argentine date format. This override splits the
 * string on "/" and constructs the Date manually so that manually-typed dates
 * work correctly in mat-datepicker inputs.
 */
@Injectable()
export class ArgDateAdapter extends NativeDateAdapter {
  override parse(value: string): Date | null {
    if (typeof value !== 'string' || !value.trim()) {
      return null;
    }

    const parts = value.trim().split('/');
    if (parts.length === 3) {
      const day = parseInt(parts[0], 10);
      const month = parseInt(parts[1], 10) - 1;
      const year = parseInt(parts[2], 10);

      if (!isNaN(day) && !isNaN(month) && !isNaN(year) && year > 0) {
        const date = new Date(year, month, day);
        if (
          date.getFullYear() === year &&
          date.getMonth() === month &&
          date.getDate() === day
        ) {
          return date;
        }
      }
    }

    return super.parse(value);
  }
}
