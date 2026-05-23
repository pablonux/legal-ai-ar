import {
  Directive,
  ElementRef,
  inject,
  input,
  OnDestroy,
  Renderer2,
  ComponentRef,
  ViewContainerRef,
} from '@angular/core';
import { EntityPreviewPopupComponent } from '../components/entity-preview-popup/entity-preview-popup.component';

export type PreviewEntityType = 'ruling' | 'court' | 'person' | 'statute' | 'proceeding';

@Directive({
  selector: '[entityPreview]',
  standalone: true,
})
export class EntityPreviewDirective implements OnDestroy {
  entityPreview = input.required<{ type: PreviewEntityType; id: string | number }>();

  private el = inject(ElementRef);
  private renderer = inject(Renderer2);
  private vcr = inject(ViewContainerRef);

  private popupRef: ComponentRef<EntityPreviewPopupComponent> | null = null;
  private showTimer: ReturnType<typeof setTimeout> | null = null;
  private hideTimer: ReturnType<typeof setTimeout> | null = null;
  private listeners: (() => void)[] = [];

  constructor() {
    const native = this.el.nativeElement as HTMLElement;
    this.listeners.push(
      this.renderer.listen(native, 'mouseenter', () => this.onEnter()),
      this.renderer.listen(native, 'mouseleave', () => this.onLeave()),
      this.renderer.listen(native, 'focus', () => this.onEnter()),
      this.renderer.listen(native, 'blur', () => this.onLeave()),
    );
  }

  private onEnter() {
    this.clearHide();
    if (this.popupRef) return;
    this.showTimer = setTimeout(() => this.show(), 300);
  }

  private onLeave() {
    this.clearShow();
    this.hideTimer = setTimeout(() => this.hide(), 200);
  }

  private show() {
    if (this.popupRef) return;
    const { type, id } = this.entityPreview();
    this.popupRef = this.vcr.createComponent(EntityPreviewPopupComponent);
    this.popupRef.setInput('entityType', type);
    this.popupRef.setInput('entityId', id);

    const popup = this.popupRef.location.nativeElement as HTMLElement;
    popup.addEventListener('mouseenter', () => this.clearHide());
    popup.addEventListener('mouseleave', () => this.onLeave());

    requestAnimationFrame(() => this.position(popup));
  }

  private position(popup: HTMLElement) {
    const anchor = (this.el.nativeElement as HTMLElement).getBoundingClientRect();
    const vw = window.innerWidth;
    const vh = window.innerHeight;
    const pw = 320;
    const spaceBelow = vh - anchor.bottom;
    const showAbove = spaceBelow < 220 && anchor.top > 220;

    popup.style.position = 'fixed';
    popup.style.zIndex = '9999';
    popup.style.width = pw + 'px';

    let left = anchor.left + anchor.width / 2 - pw / 2;
    if (left < 8) left = 8;
    if (left + pw > vw - 8) left = vw - pw - 8;
    popup.style.left = left + 'px';

    if (showAbove) {
      popup.style.bottom = (vh - anchor.top + 6) + 'px';
      popup.style.top = 'auto';
    } else {
      popup.style.top = (anchor.bottom + 6) + 'px';
      popup.style.bottom = 'auto';
    }
  }

  private hide() {
    this.popupRef?.destroy();
    this.popupRef = null;
  }

  private clearShow() {
    if (this.showTimer) { clearTimeout(this.showTimer); this.showTimer = null; }
  }

  private clearHide() {
    if (this.hideTimer) { clearTimeout(this.hideTimer); this.hideTimer = null; }
  }

  ngOnDestroy() {
    this.clearShow();
    this.clearHide();
    this.hide();
    this.listeners.forEach(fn => fn());
  }
}
