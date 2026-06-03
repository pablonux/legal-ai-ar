# Mockup → AppKit 4 Component Map

Correspondence between wireframe CSS classes and real Angular AppKit 4 components.
Use this as reference when implementing each feature in the Angular SPA.

> **Ecosystem:** AppKit 4 Re-Branded (orange theme)  
> **Package:** `@appkit4/angular-components@4.31.x`  
> **Updated:** 2026-06-03

---

## Shell

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.topnav` | `<ap-header>` | `HeaderModule` | Solid variant (orange bg). Logo lockup: `pwc` wordmark + product name in header, not sidebar. |
| `.sidebar` + `.sidebar-item` | `<ap-navigation>` | `NavigationModule` | Use expanded variant. Collapse option available. Do not nest inside modals. |
| `.app-shell` layout | Layout shell pattern | — | Header + sidebar composition. See `docs/appkit4/layout-shell/`. |

---

## Navigation & Content

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.tabs` / `.tab` | `<ap-tabs>` | `TabModule` | Active tab uses orange border-bottom in Re-Branded theme. |
| `.breadcrumb-*` | `<ap-breadcrumb>` | `BreadcrumbModule` | Not yet in mockups — add when detail views are implemented. |
| `.pagination-*` | `<ap-pagination>` | `PaginationModule` | Not yet in mockups — needed for search results. |

---

## Inputs & Search

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.search-bar` | `<ap-search>` | `SearchModule` | Or `<ap-field>` with search icon. Border-radius 8px matches `$border-radius-3`. |
| `.filter-panel` + `.filter-option input[checkbox]` | `<ap-filter>` + `<ap-checkbox>` | `FilterModule`, `CheckboxModule` | AppKit filter panel wraps checkboxes and date pickers. |
| `.chat-input` | `<ap-field>` (textarea variant) | `FieldModule` | Use `[type]="'textarea'"` for multi-line chat input. |
| `input[type=checkbox]` inline | `<ap-checkbox>` | `CheckboxModule` | `accent-color` override not needed — AppKit checkbox uses theme color natively. |
| date inputs | `<ap-datepicker>` | `DatepickerModule` | Used in task/deadline screens (f2.4). |

---

## Containers & Cards

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.card` | `<ap-panel>` | `PanelModule` | Pass `[title]` for header. Elevation via `[bordered]` prop. |
| `.kpi-card` | `<ap-panel>` (no title, flat) | `PanelModule` | KPI value uses Heading L (36px/500) — apply inline or via `.heading-l` utility. |
| `.result-item` | `<ap-list>` item | `ListModule` | Use list items with title, description, and trailing actions. |
| `.kanban-card` | `<ap-panel>` (compact) | `PanelModule` | Kanban columns are custom layout; cards inside are panels. |
| `.modal-*` (when present) | `<ap-modal>` | `ModalModule` | Not currently in wireframes — use for confirm dialogs and detail drawers. |
| `.drawer-*` (when present) | `<ap-drawer>` | `DrawerModule` | Use for slide-over panels (e.g. filter config, norm preview). |

---

## Buttons & Actions

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.btn.btn-primary` | `<ap-button [kind]="'primary'">` | `ButtonModule` | Text left-aligned, 24px horizontal padding, 8px margin between buttons. |
| `.btn.btn-secondary` | `<ap-button [kind]="'secondary'">` | `ButtonModule` | — |
| `.btn.btn-outline` | `<ap-button [kind]="'tertiary'">` | `ButtonModule` | AppKit calls this "tertiary", not "outline". |
| `.btn.btn-icon` | `<ap-button [kind]="'icon'">` | `ButtonModule` | 32×32px. Pass icon class via `[prefixIcon]`. |
| `.topnav-btn` (bell, search) | `<ap-button [kind]="'icon'">` inside `<ap-header>` | `ButtonModule` | Header icon buttons via the header's `actions` slot. |

---

## Status & Feedback

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.chip` / `.badge` | `<ap-badge>` | `BadgeModule` | Use `[type]` for semantic color variants. Border-radius is 4px (`$border-radius-2`). |
| `.notif-dot` (red dot on bell) | `<ap-notification>` or badge overlay | `NotificationModule` | Pass count via `[count]` on the notification button. |
| Status notice bars | `<ap-notice>` | `NoticeModule` | For inline success/error/warning notices (e.g. ingestion status in f1.8). |
| Toast alerts | `<ap-notification>` (toast variant) | `NotificationModule` | For transient alerts after actions. |
| `.empty-state` | `<ap-notice>` or custom with AppKit typography | — | No dedicated AppKit empty-state component; use notice or plain panel. |

---

## Data Display

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.data-table` | `<ap-table>` | `TableModule` | Supports sorting, selection, pagination, fullscreen. See `docs/appkit4/components/reference/`. |
| `.progress-*` / loading bars | `<ap-progress>` | `ProgressModule` | For indeterminate loading states during AI queries. |
| `.loading-*` / spinners | `<ap-loading>` | `LoadingModule` | Use for skeleton states during data fetch. |
| `.risk-bar-track` / `.risk-bar-fill` | Custom + AppKit `<ap-progress>` | `ProgressModule` | Risk gauge can use `<ap-progress>` with `$data-*` color overrides. |
| `.avatar` / `.avatar-group` | `<ap-avatar>` | `AvatarModule` | Supports initials, images, group stacking. |
| Ratings / feedback stars | `<ap-ratings>` | `RatingsModule` | Used in f4.4 feedback screen. |

---

## Chat & AI

| Wireframe element | AppKit component | Angular import | Notes |
|---|---|---|---|
| `.chat-bubble` | Custom Angular component | — | No AppKit chat bubble. Implement as a custom component following wireframe styles (8px radius, `$primary-orange-01` for user, white for AI). |
| `.citation` inline badge | Custom span | — | Inline citation badges are domain-specific. Use AppKit typography tokens. |
| `.chat-input-bar` | `<ap-field>` (textarea) + `<ap-button>` | `FieldModule`, `ButtonModule` | Wire to SSE streaming endpoint. |

---

## Icons

| Wireframe usage | AppKit class | How to apply |
|---|---|---|
| `<span class="Appkit4-icon icon-*-outline">` | As-is | AppKit icon font loaded via `@appkit4/styles`. Classes on `<span>` elements. |
| Icon inputs on AppKit components | `[prefixIcon]="'icon-search-outline'"` | Many AppKit components accept icon class names directly as inputs — no wrapper `<span>` needed. |

> **Icon font in Angular:** loaded via `$ap-font-icon-path` SCSS variable pointing to `/assets/appkit/font-icon/`. See `docs/appkit4/getting-started/installation.md`.

---

## Mapping gaps (not covered by AppKit)

These wireframe patterns have no direct AppKit equivalent and will require custom Angular components:

| Wireframe element | Recommendation |
|---|---|
| `.graph-area` / `.graph-node` / `.graph-edge` | Cytoscape.js component (already in tech stack). Style nodes/edges using `--data-*` color tokens. |
| `.chat-bubble` | Custom `ChatBubbleComponent` in `shared/`. Apply AppKit tokens via SCSS variables. |
| `.kanban` (column layout) | Custom `KanbanBoardComponent` in `features/projects/`. Use AppKit panels for cards. |
| `.citation` | Custom `CitationBadgeComponent` in `shared/`. Use AppKit blue tokens for styling. |
| `.kpi-grid` | Custom layout using AppKit grid classes (`.ap-grid`) + AppKit panels. |
