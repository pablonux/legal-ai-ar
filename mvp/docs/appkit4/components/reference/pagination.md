---
component: pagination
framework: angular
---

# Pagination Component

## Overview

AppKit Pagination component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use pagination:

-  To navigate between a set of pages. 
-  To allow users to move forwards and backwards through the pages and to select a specific page.

### Anatomy

1. Items Per Page Selector 
-  Label ("Show") - Text indicating that the user can choose the number of items displayed per page. 
-  Dropdown Selector (25 ▼) - A dropdown menu allowing users to select how many items are displayed per page. 
-  Unit Label ("Items / Page") - A descriptor indicating that the selected number applies to items displayed per page. 

 2. Navigation Controls 
-  Previous Button ("← Previous") - A button for navigating to the previous page. It appears disabled (grayed out), likely because the user is on the first page. 
-  Page Indicator ("1 of 250") - Displays the current page number and the total number of pages available. 
-  Current Page Input Field (1) - An input box that allows users to manually enter a page number and jump to that page. 
-  "of" Text - Provides context that the number on the right represents the total number of pages. 
-  Total Pages Indicator (250) - Displays the total number of pages available. 
-  Next Button ("Next →") - A button for navigating to the next page. 

 3. Container & Layout 
-  Component Container (Outer Box) - Defines the overall area of the pagination component.

### Variants

The pagination component comes in different variants to adapt to different layouts and use cases while maintaining usability and accessibility. 

#### Default

 The Default pagination variant includes labels for page controls and the "Show" label in the items-per-page selector. The "Previous" and "Next" buttons are clearly labeled with text, providing a clear and accessible navigation option. 

#### Compact

 The Compact variant removes the labels for navigation buttons, replacing them with only icons, making the pagination component visually lighter. It also removes the "Show" label before the items-per-page selector, simplifying the UI further.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fmPbaAkC4MrfoAXD64RDByG%2F%25F0%259F%25A7%25B0-AK4-Blue-%2526-Orange-%25E2%2580%2593-Nov-2023%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-111270%26viewport%3D1730%252C-52774%252C0.6%26t%3DBofxSD5rTFh0eRqi-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Keep the number of pages displayed small and manageable.
- Make it easy for the user to understand their current page and the total number of pages.
- Pagination should be placed at the bottom of the panel or page that it controls.

#### How not to use

- Do not overload the user with too many options or navigation choices.
- Do not use the component to navigate to pages with vastly different content.
- Do not use the pagination component as the primary navigation.

### Behavior

-  The component should update to reflect the current page and total number of pages.  
-  The Previous and Next buttons should be disabled when the user is on the first or last page, respectively.  
-  The component should respond to user interactions (e.g. clicking a page number or button) smoothly and without reloading the page.

### Accessibility

-  Dim and disable left/prev button when first page. 
-  Dim and disable right/last button when at last page.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 4


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |
| 2 | label | `section: "example:2"` |
| 3 | itemsPage | `section: "example:3"` |
| 4 | label-itemsPage | `section: "example:4"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { PaginationModule } from '@appkit4/angular-components/pagination';
```

#### HTML Template

```html
<ap-pagination [max]="10"></ap-pagination>
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### label


**Example #2** | **Variation**: None | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { PaginationModule } from '@appkit4/angular-components/pagination';
```

#### HTML Template

```html
<ap-pagination [label]="['Previous','Next']" [max]="10"></ap-pagination>
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### itemsPage


**Example #3** | **Variation**: None | **Modifier**: itemsPage | **State**: None

#### Module Import

```typescript
import { PaginationModule } from '@appkit4/angular-components/pagination';
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <div class="pagination-demo-wrapper">
  <div class="items-per-page">
    <label for="itemsPerPage" *ngIf="showLabel" class="ap-pagination-options-show">Show</label>
    <div class="items-per-page-dropdown">
      <ap-dropdown class="page-count-dropdown" [styleClass]="'page-count-dropdown'" [list]="pageCountOptions" [selectType]="'single'"
      [hideTitleOnInput]="true" [title]="''" [required]="false" [enableNgContent]="false" [(ngModel)]="selectedPageCount"
      (onSelect)="onCountChange($event)">
      </ap-dropdown>
     </div>
    <span class="ap-pagination-items-per-page" >Items / Page</span>
  </div>

  <ap-pagination [(ngModel)]="pageIndex" [max]="max"></ap-pagination>
</div>
```

#### TypeScript

```typescript
showLabel: boolean = false;
itemsPerPage: boolean = true;
totalItems: number = 6250;
max: number = 6250;
selectedPageCount: any = { label: '25', value: 25 };
pageIndex: number = 1;
pageCountOptions: Array<any> = [
   { label: '10', value: '10' },
   { label: '25', value: '25' },
   { label: '100', value: '100'},
   { label: '250', value: '250'},
  ];

  onCountChange(event: any): void {
    this.max = Math.ceil(this.totalItems / this.selectedPageCount.value);
    this.pageIndex = 1;
  }
```

#### SCSS Styles

```scss
.pagination-demo-wrapper {
  width: 100%;
  display: flex;
  justify-content: space-around;
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### label-itemsPage


**Example #4** | **Variation**: None | **Modifier**: label-itemsPage | **State**: None

#### Module Import

```typescript
import { PaginationModule } from '@appkit4/angular-components/pagination';
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="pagination-demo-wrapper">
  <div class="items-per-page">
    <label for="itemsPerPage" *ngIf="showLabel" class="ap-pagination-options-show">Show</label>
    <div class="items-per-page-dropdown">
      <ap-dropdown class="page-count-dropdown" [styleClass]="'page-count-dropdown'" [list]="pageCountOptions" [selectType]="'single'"
      [hideTitleOnInput]="true" [title]="''" [required]="false" [enableNgContent]="false" [(ngModel)]="selectedPageCount"
      (onSelect)="onCountChange($event)">
      </ap-dropdown>
     </div>
    <span class="ap-pagination-items-per-page" >Items / Page</span>
  </div>

  <ap-pagination [(ngModel)]="pageIndex" [label]="['Previous', 'Next']" [max]="max"></ap-pagination>
</div>
```

#### TypeScript

```typescript
showLabel: boolean = true;
itemsPerPage: boolean = true;
totalItems: number = 6250;
max: number = 6250
selectedPageCount: any = { label: '25', value: 25 };
pageIndex: number = 1;
pageCountOptions: Array<any> = [
   { label: '10', value: '10' },
   { label: '25', value: '25' },
   { label: '100', value: '100'},
   { label: '250', value: '250'},
  ];

  onCountChange(event: any): void {
    this.max = Math.ceil(this.totalItems / this.selectedPageCount.value);
    this.pageIndex = 1;
  }
```

#### SCSS Styles

```scss
.pagination-demo-wrapper {
  width: 100%;
  display: flex;
  justify-content: space-around;
}
```

<!-- /EXAMPLE:4 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| min | number | The minimum boundary of the pagination. | 1 | 4.0.0 |
| max | number | The maximum boundary of the pagination. | 100 | 4.0.0 |
| step | number | The step length of the "prev" and "next" buttons. | 1 | 4.0.0 |
| label | \[string , string\] | Define the labels of pagination previous and next buttons. | null | 4.0.0 |
| ngModel | number | Default value of the pagination, two-way binding is supported. | 0 | 4.0.0 |
| onChange | EventEmitter<number> | Callback to invoke when current page number changes. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->
