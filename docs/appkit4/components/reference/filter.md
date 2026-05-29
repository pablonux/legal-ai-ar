---
component: filter
framework: angular
---

# Filter Component

## Overview

AppKit Filter component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use filters:

- To filter and display different subsets of items in a list, table or UI.
- To allow users to search, refine, and filter the data displayed.

### Anatomy

1. **Header:** Describes the filter sub criteria list.

2. **Expand icon “chevron”:** Expands or collapses criteria list.

3. **Criteria list:** List of choices available to users. It can be composed of two types of selectors: radio buttons or checkmarks.

4. **Criteria count:** Represents the amount of items available for each filter option.

### Variants

#### Single selection:

The user can select only one item at a time.

#### Multiple selection:

The user can select multiple items without using a modifier.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-79588%26viewport%3D1317%252C-15243%252C0.43%26t%3DlF2l7SnrQIYO0SBJ-1%26scaling%3Dmin-zoom%26mode%3Ddesignhttps://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-79588%26viewport%3D1317%252C-15243%252C0.43%26t%3DlF2l7SnrQIYO0SBJ-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use simple language for filter choices and filter categories.
- The options presented within the filter should be short, descriptive, and relevant to the data or content being filtered.
- Take the time to understand what criteria your users need to filter by in the given context.
- Consider ordering the list of filter options alpha-numerically or by most used to least used.
- Within the filter use radio buttons if only one option can be selected. Use checkboxes if multiple can be selected.
- The filter component should present clear choices that allow users to quickly understand what they are browsing.
- For lengthy filter groups, avoid placing filters inside small scrollable panels.
- Users should, where possible, view all filter options at the same time.
- Consider small screen sizes when designing the interface for each filter and the total number of filters to include.
- If the layout is not large enough to keep the applied filters open all the time, or if the filters appear in an overlay pane, display applied filters above the results. They can be represented in the form of tags.

#### How not to use

- Do not add unnecessary filter options. Take the time to understand what options should be made available to your users. Adding unnecessary UI decreases the ease of use.
- Don't use long descriptions for filter option labels.

### Behavior

- Never auto-scroll users on a single input and never collapse a group of filters automatically.
- Always update filters and display results asynchronously, so that on each filter entry, matching results can be updated asynchronously, while filters always remain accessible and in the same place.

### Accessibility

- Each radio button should have a <label>. Associate the two by matching the <label>’s for attribute to the <input>’s id attribute.
- Group related radio buttons together with <fieldset> and describe the group with <legend>.
- Radio buttons should provide current state to screen reader (selected or unselected).
- ARIA controls of the button should match the ID of the container.
- Trigger should have ARIA-expanded true or false.
- Component should announce changes to the results page using aria-live="assertive".

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 6


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | singleselection - normal | `section: "example:1"` |
| 2 | singleselection - disabled | `section: "example:2"` |
| 3 | singleselection - status | `section: "example:3"` |
| 4 | multipleselection - normal | `section: "example:4"` |
| 5 | multipleselection - disabled | `section: "example:5"` |
| 6 | multipleselection - status | `section: "example:6"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### singleselection - normal


**Example #1** | **Variation**: singleselection | **Modifier**: normal | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="false" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item (onClick)="onEnterForRadio(item)" [item]="item" [role]="'radio'" [isRadioSelected]="item.name===filterItemName">
                    <ap-radio [value]="item.name" [(ngModel)]="filterItemName" 
                        (onChange)="onRadioSelected($event, item)" [disabled]="item.disabled" [tabindex]="-1"></ap-radio>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
filterItemName = 'Blue';
headerName = 'Single selection';
filterData = [
    {
        name: 'Blue',
        comment: '128', 
        disabled: false
    },
    {
        name: 'Green',
        comment: '48',
        disabled: false
    },
    {
        name: 'Indigo',
        comment: '256',
        disabled: false
    },
    {
        name: 'Orange',
        comment: '10',
        disabled: false
    },
    {
        name: 'Pink',
        comment: '32',
        disabled: false
    }, 
    {
        name: 'Purple',
        comment: '8',
        disabled: false
    }
];

onRadioSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current selected:' + JSON.stringify(item));
}

onEnterForRadio(item: any) {
    if (!item.disabled && this.filterItemName !== item.name) {
        this.filterItemName = item.name;
        this.onRadioSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### singleselection - disabled


**Example #2** | **Variation**: singleselection | **Modifier**: disabled | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="false" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item (onClick)="onEnterForRadio(item)" [item]="item" [role]="'radio'" [isRadioSelected]="item.name===filterItemName">
                    <ap-radio [value]="item.name" [(ngModel)]="filterItemName" 
                        (onChange)="onRadioSelected($event, item)" [disabled]="item.disabled" [tabindex]="-1"></ap-radio>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
filterItemName = 'Blue';
headerName = 'Single selection Disabled';
filterData = [
    {
        name: 'Blue',
        comment: '128', 
        disabled: true
    },
    {
        name: 'Green',
        comment: '48',
        disabled: false
    },
    {
        name: 'Indigo',
        comment: '256',
        disabled: true
    },
    {
        name: 'Orange',
        comment: '10',
        disabled: false
    },
    {
        name: 'Pink',
        comment: '32',
        disabled: false
    }, 
    {
        name: 'Purple',
        comment: '8',
        disabled: false
    }
];

onRadioSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current selected:' + JSON.stringify(item));
}

onEnterForRadio(item: any) {
    if (!item.disabled && this.filterItemName !== item.name) {
        this.filterItemName = item.name;
        this.onRadioSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### singleselection - status


**Example #3** | **Variation**: singleselection | **Modifier**: status | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="false" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item (onClick)="onEnterForRadio(item)" [item]="item" [role]="'radio'" [isRadioSelected]="item.name===filterItemName">
                    <ap-radio [value]="item.name" [(ngModel)]="filterItemName" 
                        (onChange)="onRadioSelected($event, item)" [disabled]="item.disabled" [tabindex]="-1"></ap-radio>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
filterItemName = 'Completed';
headerName = 'Single selection Status';
filterData = [
    {
        name: 'Completed',
        comment: '128', 
        class: 'complete',
        disabled: false
    },
    {
        name: 'In-progress',
        comment: '48',
        class: 'inprogress',
        disabled: false
    },
    {
        name: 'Error',
        comment: '256',
        class: 'error',
        disabled: false
    },
    {
        name: 'Draft',
        comment: '16',
        class: 'draft',
        disabled: false
    }
];

onRadioSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current selected:' + JSON.stringify(item));
}

onEnterForRadio(item: any) {
    if (!item.disabled && this.filterItemName !== item.name) {
        this.filterItemName = item.name;
        this.onRadioSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### multipleselection - normal


**Example #4** | **Variation**: multipleselection | **Modifier**: normal | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="true" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item [item]="item" (onClick)="onEnterForCheckbox(item)" [role]="'checkbox'">
                    <ap-checkbox [(ngModel)]="item.checked" [disabled]="item.disabled" 
                        (onChange)="onCheckboxSelected($event, item)" [tabindex]="-1"></ap-checkbox>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
headerName = 'Multiple selection';
filterData = [
    {
        checked: false,
        name: 'Blue',
        comment: '128', 
        disabled: false
    },
    {
        checked: false,
        name: 'Green',
        comment: '48',
        disabled: false
    },
    {
        checked: false,
        name: 'Indigo',
        comment: '256',
        disabled: false
    },
    {
        checked: false,
        name: 'Orange',
        comment: '10',
        disabled: false
    },
    {
        checked: false,
        name: 'Pink',
        comment: '32',
        disabled: false
    }, 
    {
        checked: false,
        name: 'Purple',
        comment: '8',
        disabled: false
    }
];

onCheckboxSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current operation:' + JSON.stringify(item));
    const allSelected = this.filterData.filter(element => {return element.checked});
    // console.log('all selected:' + JSON.stringify(allSelected));
}

onEnterForCheckbox(item: any) {
    if (!item.disabled) {
        item.checked = !item.checked;
        this.onCheckboxSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### multipleselection - disabled


**Example #5** | **Variation**: multipleselection | **Modifier**: disabled | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="true" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item [item]="item" (onClick)="onEnterForCheckbox(item)" [role]="'checkbox'">
                    <ap-checkbox [(ngModel)]="item.checked" [disabled]="item.disabled" 
                        (onChange)="onCheckboxSelected($event, item)" [tabindex]="-1"></ap-checkbox>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
headerName = 'Multiple selection Disabled';
filterData = [
    {
        checked: false,
        name: 'Blue',
        comment: '128', 
        disabled: false
    },
    {
        checked: false,
        name: 'Green',
        comment: '48',
        disabled: false
    },
    {
        checked: false,
        name: 'Indigo',
        comment: '256',
        disabled: true
    },
    {
        checked: false,
        name: 'Orange',
        comment: '10',
        disabled: false
    },
    {
        checked: false,
        name: 'Pink',
        comment: '32',
        disabled: false
    }, 
    {
        checked: false,
        name: 'Purple',
        comment: '8',
        disabled: false
    }
];

onCheckboxSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current operation:' + JSON.stringify(item));
    const allSelected = this.filterData.filter(element => {return element.checked});
    // console.log('all selected:' + JSON.stringify(allSelected));
}

onEnterForCheckbox(item: any) {
    if (!item.disabled) {
        item.checked = !item.checked;
        this.onCheckboxSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### multipleselection - status


**Example #6** | **Variation**: multipleselection | **Modifier**: status | **State**: None

#### Module Import

```typescript
import { CheckboxModule} from '@appkit4/angular-components/checkbox';
import { FilterModule } from '@appkit4/angular-components/filter';
import { RadioModule } from '@appkit4/angular-components/radio';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class="filter-demo-wrapper">
    <ap-filter [isMultiSelectable]="true" [data]="filterData" [headerName]="headerName" [expand]="expand">
            <ng-container *ngFor="let item of filterData; let i = index">
                <ap-filter-item [item]="item" (onClick)="onEnterForCheckbox(item)" [role]="'checkbox'">
                    <ap-checkbox [(ngModel)]="item.checked" [disabled]="item.disabled" 
                        (onChange)="onCheckboxSelected($event, item)" [tabindex]="-1"></ap-checkbox>
                </ap-filter-item>
            </ng-container>
    </ap-filter>
</div>
```

#### TypeScript

```typescript
expand = false;
headerName = 'Multiple selection Status';
filterData = [
    {
        checked: false,
        name: 'Completed',
        comment: '128', 
        class: 'complete',
        disabled: false
    },
    {
        checked: false,
        name: 'In-progress',
        comment: '48',
        class: 'inprogress',
        disabled: false
    },
    {
        checked: false,
        name: 'Error',
        comment: '256',
        class: 'error',
        disabled: false
    },
    {
        checked: false,
        name: 'Draft',
        comment: '16',
        class: 'draft',
        disabled: false
    }
];

onCheckboxSelected(event: any, item: any) {
    if (event)
        event.originEvent.stopPropagation();
    // console.log('current operation:' + JSON.stringify(item));
    const allSelected = this.filterData.filter(element => {return element.checked});
    // console.log('all selected:' + JSON.stringify(allSelected));
}

onEnterForCheckbox(item: any) {
    if (!item.disabled) {
        item.checked = !item.checked;
        this.onCheckboxSelected(false, item);
    }
}
```

#### SCSS Styles

```scss
.filter-demo-wrapper {
    width: 320px;
}
```

<!-- /EXAMPLE:6 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-filter

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| expand | boolean | Whether to expand the filter. | false | 4.0.0 |
| data | Array<any> | Detail info of filter: \[{checked: boolean(if the item selected), name: string(item name), comment: string(comment of item), class: string(status icon class, can ignore if not need show the status icon, value is complete/inprogress/draft), disabled: boolean(if the item is disabled or enabled)}\]. | \[\] | 4.0.0 |
| headerName | string | Text of the filter header. | '' | 4.0.0 |
| isMultiSelectable | boolean | The value of the HTML aria attribute of multiselectable of filter. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |

### ap-filter-item

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| item | object | Detail info of item: {checked: boolean(if the item selected), name: string(item name), comment: string(comment of item), class: string(status icon class, can ignore if not need show the status icon, value is complete/inprogress/draft), disabled: boolean(if the item is disabled or enabled)}. | {} | 4.0.0 |
| onClick | EventEmitter<Event> | If every filter item is clicked or tab to enter, then will trigger this function. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| role | string: 'radio'\|'checkbox' | The role of filter item. | 'radio' | 4.0.0 |
| isRadioSelected | boolean | If filter item is selected, it is only applicable when role is radio. It will impact the accessibility of the filter item and its value should be a dynamic variable or an expression which will return a dynamic value. | false | 4.0.0 |


<!-- /SECTION:properties -->