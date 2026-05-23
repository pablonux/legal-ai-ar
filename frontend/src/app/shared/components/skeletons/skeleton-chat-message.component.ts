import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-chat-message',
  standalone: true,
  template: `
    @for (_ of items; track $index; let odd = $odd) {
      <div class="sk-msg" [class.sk-msg-user]="odd">
        <div class="sk-avatar skeleton"></div>
        <div class="sk-bubble">
          <div class="skeleton sk-line"></div>
          <div class="skeleton sk-line sk-line-mid"></div>
          @if (!odd) {
            <div class="skeleton sk-line sk-line-short"></div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      padding: 1rem 0;
    }

    .sk-msg {
      display: flex;
      gap: 0.75rem;
      align-items: flex-start;
    }

    .sk-msg-user {
      flex-direction: row-reverse;
    }

    .sk-avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      flex-shrink: 0;
    }

    .sk-bubble {
      display: flex;
      flex-direction: column;
      gap: 0.375rem;
      max-width: 70%;
    }

    .sk-line {
      height: 12px;
      width: 280px;
      max-width: 100%;
    }

    .sk-line-mid { width: 200px; }
    .sk-line-short { width: 140px; }
  `]
})
export class SkeletonChatMessageComponent {
  count = input(3);
  get items() { return Array(this.count()); }
}
