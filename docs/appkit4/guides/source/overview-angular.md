---
guide: overview-angular
framework: angular
---

# Appkit Angular Design System

## Version Compatibility

**IMPORTANT:** Before installing Appkit, ensure your Angular version is compatible:

## Angular Version Compatibility

**Recommended:** Use the latest Appkit version (4.31.0) with Angular 19, 20 for new projects. For Angular 17, 18 projects, use 4.22.3.

**Note:** @angular/cdk (Component Dev Kit) is tightly coupled to the Angular framework version. If project uses @angular/core@19.x.x, use @angular/cdk@19.x.x. If project uses @angular/core@20.x.x, use @angular/cdk@20.x.x.

| Appkit Version | Angular Support | Ecosystems |
|----------------|-----------------|------------|
| **4.31.0** (Latest) | Angular 19, 20 | Appkit 4, Appkit New Era |
| 4.30.0 | Angular 19, 20 | Appkit 4 |
| 4.29.1 | Angular 19, 20 | Appkit 4 |
| 4.29.0 | Angular 19, 20 | Appkit 4 |
| 4.28.0 | Angular 19, 20 | Appkit 4 |
| 4.27.0 | Angular 19, 20 | Appkit 4 |
| 4.26.0 | Angular 19, 20 | Appkit 4 |
| 4.25.0 | Angular 19 | Appkit 4 |
| 4.24.0 | Angular 19 | Appkit 4 |
| 4.23.0 | Angular 19 | Appkit 4 |
| **4.22.3** (Latest for Angular 17, 18) | Angular 17, 18 | Appkit 4 |
| 4.22.1 | Angular 17, 18 | Appkit 4 |
| 4.22.0 | Angular 17, 18 | Appkit 4 |
| 4.21.0 | Angular 17, 18 | Appkit 4 |
| 4.20.0 | Angular 17, 18 | Appkit 4 |
| 4.19.0 | Angular 17, 18 | Appkit 4 |
| 4.18.0 | Angular 17, 18 | Appkit 4 |
| 4.17.0 | Angular 17, 18 | Appkit 4 |
| 4.16.0 | Angular 17, 18 | Appkit 4 |
| 4.15.0 | Angular 17, 18 | Appkit 4 |
| 4.14.0 | Angular 17 | Appkit 4 |
| 4.13.0 | Angular 17 | Appkit 4 |
| 4.12.0 | Angular 17 | Appkit 4 |
| 4.11.0 | Angular 16 | Appkit 4 |
| 4.10.0 | Angular 16 | Appkit 4 |
| 4.9.2 | Angular 16 | Appkit 4 |
| 4.9.1 | Angular 16 | Appkit 4 |
| 4.9.0 | Angular 16 | Appkit 4 |
| 4.8.1 | Angular 15 | Appkit 4 |
| 4.8.0 | Angular 15 | Appkit 4 |
| 4.7.0 | Angular 15 | Appkit 4 |
| 4.6.1 | Angular 13 | Appkit 4 |
| 4.6.0 | Angular 13 | Appkit 4 |
| 4.5.0 | Angular 13 | Appkit 4 |
| 4.4.0 | Angular 13 | Appkit 4 |
| 4.3.0 | Angular 13 | Appkit 4 |
| 4.2.0 | Angular 13 | Appkit 4 |
| 4.1.0 | Angular 13 | Appkit 4 |
| 4.0.0 | Angular 13 | Appkit 4 |

*Always check version compatibility before installing. Use `appkit_get_installation_guide` for version-specific installation instructions.*

## Design System Documentation

Available design tokens: accesibility, colors, colors_new_era, design-tokens, design-tokens_new_era, elevation, elevation_new_era, grid, icons, styles, styles_new_era, typography, typography_new_era, versions
*Use `get_design_tokens` tool with specific category for details.*

## Components (35)

| Component | Import |
|-----------|--------|
| accordion | `import { AccordionModule } from '@appkit4/angular-components/accordion';` |
| avatar | `import { AvatarModule } from '@appkit4/angular-components/avatar';` |
| badge | `import { BadgeModule } from '@appkit4/angular-components/badge';` |
| breadcrumb | `import { BreadcrumbModule } from '@appkit4/angular-components/breadcrumb';` |
| button | `import { ButtonModule } from '@appkit4/angular-components/button';` |
| checkbox | `import { CheckboxModule } from '@appkit4/angular-components/checkbox';` |
| datepicker | `import { FieldModule } from '@appkit4/angular-components/field';` |
| drawer | `import { DrawerModule } from '@appkit4/angular-components/drawer';` |
| dropdown | `import { DropdownModule } from '@appkit4/angular-components/dropdown';` |
| feeds-comments | `import { FeedModule } from '@appkit4/angular-components/feed';` |
| field | `import { FieldModule } from '@appkit4/angular-components/field';` |
| file-upload | `import { FileuploaderModule } from '@appkit4/angular-components/fileupload';` |
| filter | `import { CheckboxModule } from '@appkit4/angular-components/checkbox';` |
| footer | `import { FooterModule } from '@appkit4/angular-components/footer';` |
| header | `import { HeaderModule } from '@appkit4/angular-components/header';` |
| list | `import { ListModule } from '@appkit4/angular-components/list';` |
| loading | `import { LoadingModule } from '@appkit4/angular-components/loading';` |
| modal | `import { ModalModule } from '@appkit4/angular-components/modal';` |
| navigation | `import { NavigationModule } from '@appkit4/angular-components/navigation';` |
| notice | `import { NotificationModule } from '@appkit4/angular-components/notification';` |
| notification | `import { NotificationModule } from '@appkit4/angular-components/notification';` |
| pagination | `import { PaginationModule } from '@appkit4/angular-components/pagination';` |
| panel | `import { PanelModule } from '@appkit4/angular-components/panel';` |
| progress | `import { ProgressModule } from '@appkit4/angular-components/progress';` |
| radio | `import { RadioModule } from '@appkit4/angular-components/radio';` |
| ratings | `import { RatingsModule } from '@appkit4/angular-components/ratings';` |
| search | `import { SearchModule } from '@appkit4/angular-components/search';` |
| slider | `import { SliderModule } from '@appkit4/angular-components/slider';` |
| table | `import { TableModule } from '@appkit4/angular-components/table';` |
| tabs | `import { TabsModule } from '@appkit4/angular-components/tabs';` |
| tag | `import { TagModule } from '@appkit4/angular-components/tag';` |
| texteditor | `import { TextEditorModule } from '@appkit4/angular-text-editor/text-editor';` |
| toggle | `import { ToggleModule } from '@appkit4/angular-components/toggle';` |
| tooltip | `import { TooltipModule } from '@appkit4/angular-components/tooltip';` |
| tree | `import { TreeModule } from '@appkit4/angular-components/tree';` |

*Use `get_component` tool with component name for full documentation.*