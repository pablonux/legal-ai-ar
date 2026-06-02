/*
 * Public API Surface of shared-common
 */

export { ArgDateAdapter } from './lib/shared/adapters/arg-date-adapter';
export { NotificationService } from './lib/shared/services/notification.service';

export { BreadcrumbComponent } from './lib/shared/components/breadcrumb/breadcrumb.component';
export { CommandPaletteComponent } from './lib/shared/components/command-palette/command-palette.component';
export {
  EmptyStateComponent,
  type EmptyStateAction
} from './lib/shared/components/empty-state/empty-state.component';
export { EntityPreviewPopupComponent } from './lib/shared/components/entity-preview-popup/entity-preview-popup.component';
export { LoadingSpinnerComponent } from './lib/shared/components/loading-spinner/loading-spinner.component';
export { OnboardingTipComponent } from './lib/shared/components/onboarding-tip/onboarding-tip.component';
export { SkeletonLoaderComponent } from './lib/shared/components/skeleton-loader/skeleton-loader.component';
export { ToastContainerComponent } from './lib/shared/components/toast-container/toast-container.component';

export { SkeletonCardComponent } from './lib/shared/components/skeletons/skeleton-card.component';
export { SkeletonChatMessageComponent } from './lib/shared/components/skeletons/skeleton-chat-message.component';
export { SkeletonDetailComponent } from './lib/shared/components/skeletons/skeleton-detail.component';
export { SkeletonStatComponent } from './lib/shared/components/skeletons/skeleton-stat.component';
export { SkeletonTableRowComponent } from './lib/shared/components/skeletons/skeleton-table-row.component';

export { EntityPreviewDirective } from './lib/shared/directives/entity-preview.directive';
export type { PreviewEntityType } from './lib/shared/directives/entity-preview.directive';

export { ChatMarkdownPipe } from './lib/shared/pipes/chat-markdown.pipe';
export { CitationTypeLabelPipe } from './lib/shared/pipes/citation-type-label.pipe';
export { RulingDatePipe } from './lib/shared/pipes/ruling-date.pipe';
