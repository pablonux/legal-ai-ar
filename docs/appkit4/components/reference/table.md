---
component: table
framework: angular
---

# Table Component

## Overview

AppKit Table component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Tables:

- To display data in a structured format.
- To show large sets of data that need to be sorted and filtered.

### Anatomy

1. **Table header:** The row that contains column headers.

2. **Table body:** The area where the data is displayed.

3. **Table row:** The individual row that contains data.

4. **Table cell:** The individual cell that contains data.

### Variants

#### Default:

The Default variant has standard row height and padding.

#### Condensed:

The Condensed variant has a reduced row height and padding.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2F%2Fwww.figma.com%2Fproto%2FBmrAOae85hXrSMa1rpqGkQ%2FAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D194903-99853%26viewport%3D1222%252C-29250%252C0.36%26t%3Dt9mibRaAl7v9xMgY-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Tables should be used to organize and display data that is too detailed to describe with text or a visualization.
- Take the time to understand what your users actually need to see within a table and how the columns should be organized within the table.

#### How not to use

- Do not use a table if the data can be more easily represented by text or a visualization.
- Do not use tables to recreate Microsoft Excel within your application.

### Behavior

- Sorting: Tables columns are sortable, allowing users to sort ascending or descending by clicking on the column header.
- Hovering: Hovering over a row highlights it.

### Accessibility

- Table label must be clear and concise.
- Use <caption> element to give the table a name.
- Header cells must be marked up with <th>, and data cells with <td>.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 16


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | default | `section: "example:1"` |
| 2 | default - title | `section: "example:2"` |
| 3 | default - striped | `section: "example:3"` |
| 4 | default - checkbox | `section: "example:4"` |
| 5 | default - title-striped | `section: "example:5"` |
| 6 | default - title-checkbox | `section: "example:6"` |
| 7 | default - striped-checkbox | `section: "example:7"` |
| 8 | default - title-striped-checkbox | `section: "example:8"` |
| 9 | condensed | `section: "example:9"` |
| 10 | condensed - title | `section: "example:10"` |
| 11 | condensed - striped | `section: "example:11"` |
| 12 | condensed - checkbox | `section: "example:12"` |
| 13 | condensed - title-striped | `section: "example:13"` |
| 14 | condensed - title-checkbox | `section: "example:14"` |
| 15 | condensed - striped-checkbox | `section: "example:15"` |
| 16 | condensed - title-striped-checkbox | `section: "example:16"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### default


**Example #1** | **Variation**: default | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" class="ap-table-demo">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" class="ap-table-demo">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### default - title


**Example #2** | **Variation**: default | **Modifier**: title | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" (onSort)="onSort($event)" class="ap-table-demo">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" (onSort)="onSort($event)" class="ap-table-demo">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### default - striped


**Example #3** | **Variation**: default | **Modifier**: striped | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" class="ap-table-demo" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" class="ap-table-demo" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### default - checkbox


**Example #4** | **Variation**: default | **Modifier**: checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### default - title-striped


**Example #5** | **Variation**: default | **Modifier**: title-striped | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" (onSort)="onSort($event)" class="ap-table-demo" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" (onSort)="onSort($event)" class="ap-table-demo" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### default - title-checkbox


**Example #6** | **Variation**: default | **Modifier**: title-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### default - striped-checkbox


**Example #7** | **Variation**: default | **Modifier**: striped-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### default - title-striped-checkbox


**Example #8** | **Variation**: default | **Modifier**: title-striped-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### condensed


**Example #9** | **Variation**: condensed | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" class="ap-table-demo" [condensed]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" class="ap-table-demo" [condensed]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### condensed - title


**Example #10** | **Variation**: condensed | **Modifier**: title | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" (onSort)="onSort($event)" class="ap-table-demo" [condensed]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" (onSort)="onSort($event)" class="ap-table-demo" [condensed]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### condensed - striped


**Example #11** | **Variation**: condensed | **Modifier**: striped | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" class="ap-table-demo" [condensed]="true" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" class="ap-table-demo" [condensed]="true" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### condensed - checkbox


**Example #12** | **Variation**: condensed | **Modifier**: checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### condensed - title-striped


**Example #13** | **Variation**: condensed | **Modifier**: title-striped | **State**: None

#### Module Import

```typescript
import { TableModule } from '@appkit4/angular-components/table';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" (onSort)="onSort($event)" class="ap-table-demo" [condensed]="true" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" (onSort)="onSort($event)" class="ap-table-demo" [condensed]="true" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId">
        <td>
          <span>{{data.order}}</span>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### condensed - title-checkbox


**Example #14** | **Variation**: condensed | **Modifier**: title-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### condensed - striped-checkbox


**Example #15** | **Variation**: condensed | **Modifier**: striped-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true" [striped]="true">
  <table>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### condensed - title-striped-checkbox


**Example #16** | **Variation**: condensed | **Modifier**: title-striped-checkbox | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<!-- The common way to use the table component -->
<ap-table #table [(originalData)]="tdata" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of table.data; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### HTML Extra

```html
<!-- The second way to use the table component -->
<!-- Usually applicable to the scenario which rendering multiple tables using *ngFor -->
<ap-table [(originalData)]="tdata" [(data)]="tableData" [showSelectAll]="true" (onSelectAllChange)="onSelectAllChange($event)" (onSort)="onSort($event)" class="ap-table-demo ap-table-checkbox" [condensed]="true" [striped]="true">
  <table>
    <thead>
      <tr>
        <th ap-sort [sortKey]="'order'">
          <span>Order</span>
        </th>
        <th ap-sort [sortKey]="'city'">
          <span>City</span>
        </th>
        <th ap-sort [sortKey]="'fulfilled'">
          <span>Fulfilled</span>
        </th>
        <th ap-sort [sortKey]="'description'" [sortFunc1]="compareFunc">
          <span>Description</span>
        </th>
        <th ap-sort [slot]="'start'" [sortKey]="'total'">
          <span>Total</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let data of tableData; trackBy: trackId" [class.selected]="data.selected">
        <td>
          <ap-checkbox [(ngModel)]="data.selected" class="ap-table-checkbox-container">
            <span>{{data.order}}</span>
          </ap-checkbox>
        </td>
        <td>
          <span>{{data.city}}</span>
        </td>
        <td>
          <span>{{data.fulfilled}}</span>
        </td>
        <td>
          <span>{{data.description}}</span>
        </td>
        <td>
          <span>{{data.total | currency: 'USD'}}</span>
        </td>
      </tr>
    </tbody>
  </table>
</ap-table>
```

#### TypeScript

```typescript
// tdata is the original data used for the data initialization
tdata: any[] = [
  {
    order: '92330',
    city: 'Hong Kong',
    fulfilled: 'Yes',
    description: 'Lorem ipsum dolor sit amet consectetur adipiscing elit',
    total: 94.24
  },
  {
    order: '23638',
    city: 'Stockholm',
    fulfilled: 'No',
    description: 'Sed do eiusmod tempor incididunt ut labore',
    total: 373.08
  },
  {
    order: '82070',
    city: 'São Paulo',
    fulfilled: 'Yes',
    description: 'Et dolore magna aliqua consectetur',
    total: 837.56
  },
  {
    order: '36301',
    city: 'Saint Petersburg',
    fulfilled: 'Yes',
    description: 'Ut enim ad minim veniam quis nostrud exercitation',
    total: 128.86
  },
  {
    order: '45230',
    city: 'Toronto',
    fulfilled: 'Yes',
    description: 'Ullamco laboris nisi ut aliquip ex ea commodo consequat',
    total: 238.01
  }
];
// tableData is used for sorting and rendering the table
// only used in the second way of rendering the table component (see 'HTML-extra' tab)
tableData = this.tdata;

// track unique identifier in data, which is 'order' in this case, to optimize performance
trackId(index: number, item: any) {
  return item.order;
}

onSelectAllChange(event: any) {
  console.log('onSelectAllChange', event);
}

compareFunc(a: any, b: any): number {
  return a.description.toUpperCase() > b.description.toUpperCase() ? 1 : -1;
}

onSort(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
 /*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.ap-table-demo {
  th {
      background-color: $color-background-container;
  }

  td {
    &:last-child {
      text-align: right;
    }
  }
}
```

#### Dependency

```text
"lodash-es": "^4.17.21"
```

<!-- /EXAMPLE:16 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-table

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| originalData | array | Data of the table.Caution: '\_initialSortIndex', '\_previousSortIndex' and '\_nextSortIndex' are reserved keys of data for sorting purpose. | \[\] | 4.0.0 |
| data | array | Data for sorting and rendering the table. After sorting, it generates three extra keys other than the originalData, including '\_initialSortIndex', '\_previousSortIndex' and '\_nextSortIndex'.Used in the second way for rendering the table component. (See 'HTML-extra' tab) | originalData | 4.0.0 |
| condensed | boolean | If true, table displays in condensed style. | false | 4.0.0 |
| striped | boolean | If true, table displays in striped style. | false | 4.0.0 |
| onSort | EventEmitter<{ tableId: string, sortKey: string, sortingPhase: number }> | Callback to invoke when sorting. | - | 4.0.0 |
| sortActive | string | The sortKey of the active header. Only takes effect when having sortPhase property. Note that changing sortActive and sortPhase at the same time may cause sorting conflicts. | - | 4.0.0 |
| sortPhase | number: 0 \| 1 \| 2 | The sorting phase of the active header. Only takes effect when having sortActive property. Note that changing sortActive and sortPhase at the same time may cause sorting conflicts. | - | 4.0.0 |
| animatedSorting | boolean | Enable animated table sorting. | true | 4.0.0 |
| disableDefaultSort | boolean | Disable the data sorting. If true, you can set the 'originalData' according to the sortingPhase from 'onSort' callback to implement server-side sorting. | false | 4.3.0 |
| showSelectAll | boolean | Show the checkbox to select all data. | false | 4.3.0 |
| onSelectAllChange | EventEmitter<{ originEvent: Event, checkboxState: boolean }> | Callback to invoke when changing the state of the select all checkbox. | - | 4.3.0 |
| config | Object | The configuration of the table paginator. When using the table with pagination component, assign the pagination reference to config\['pagination'\] and change page size by setting config\['pageSize'\]. The default pageSize is 50. | {} | 4.4.0 |
| tableId | string | Id string of the table | Random string of 14 characters in length. | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |

### th

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| sortKey | string | The sort key of data. | '' | 4.0.0 |
| slot | string: 'start'\|'end' | The position of sorter in header. | 'end' | 4.0.0 |
| sortFunc1 | compareFunction: (a: any, b: any) => number | The first phase customized compare function of sorter. Using the default ASCII ascending order if not provided. | - | 4.0.0 |
| sortFunc2 | compareFunction: (a: any, b: any) => number | The second phase customized compare function of sorter. Using the default ASCII descending order if not provided. | - | 4.0.0 |


<!-- /SECTION:properties -->