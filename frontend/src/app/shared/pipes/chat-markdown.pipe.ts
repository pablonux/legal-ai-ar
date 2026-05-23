import { Pipe, PipeTransform, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked, Renderer } from 'marked';

const CITATION_REGEX = /\{caso:\s*"([^"]+)"\s*,\s*id:\s*"([a-f0-9-]+)"\}/gi;

interface ParsedCitation {
  title: string;
  id: string;
  index: number;
}

function preprocessCitations(text: string): string {
  const citations: ParsedCitation[] = [];
  const seenIds = new Map<string, number>();

  let processed = text.replace(CITATION_REGEX, (_, title: string, id: string) => {
    let idx: number;
    if (seenIds.has(id)) {
      idx = seenIds.get(id)!;
    } else {
      idx = citations.length + 1;
      seenIds.set(id, idx);
      citations.push({ title, id, index: idx });
    }
    const safeTitle = title.replace(/"/g, '&quot;');
    return `<a class="cite-ref ruling-link" data-ruling-id="${id}" title="${safeTitle}">[${idx}]</a>`;
  });

  if (citations.length > 0) {
    processed += '\n\n<div class="cite-sources">'
      + '<div class="cite-sources-header">'
      + '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>'
      + `<span>Fuentes (${citations.length})</span>`
      + '</div>';
    for (const c of citations) {
      const safeTitle = c.title.replace(/"/g, '&quot;');
      processed += `<a class="cite-source ruling-link" data-ruling-id="${c.id}" title="${safeTitle}">`
        + `<span class="cite-num">${c.index}</span>`
        + `<span class="cite-title">${safeTitle}</span>`
        + `<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><polyline points="15 3 21 3 21 9"/><line x1="10" y1="14" x2="21" y2="3"/></svg>`
        + `</a>`;
    }
    processed += '</div>';
  }

  return processed;
}

function wrapTables(html: string): string {
  return html.replace(/<table>/g, '<div class="table-wrap"><table>')
             .replace(/<\/table>/g, '</table></div>');
}

function buildRenderer(): Partial<Renderer> {
  return {
    paragraph({ tokens }): string {
      const body = this.parser!.parseInline(tokens);
      return `<p>${body}</p>`;
    },
  };
}

marked.use({ renderer: buildRenderer() as Renderer });

@Pipe({
  name: 'chatMarkdown',
  standalone: true,
})
export class ChatMarkdownPipe implements PipeTransform {
  private sanitizer = inject(DomSanitizer);

  transform(text: string): SafeHtml {
    if (!text) return '';

    const withCitations = preprocessCitations(text);
    const raw = marked.parse(withCitations, { async: false }) as string;
    const html = wrapTables(raw);

    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}
