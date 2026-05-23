**Note:** This component documentation is large (258KB). Consider using section parameter (usage, examples, properties) for specific content.

---

---
component: dropdown
framework: angular
---

# Dropdown Component

## Overview

AppKit Dropdown component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use dropdowns as follows:

- Use dropdowns in forms, pages, modals or panels.
- To filter or sort related content.
- As a stylized selection component.

### Anatomy

**1. Field:** Container area.

**2. Placeholder:** Prompt text.

**3. Dropdown arrow:** Opens the menu.

**4. Menu:** List of options presented to the user.

**5. Menu item/option:** Individual selectable choice for the user.

### Variants

#### Single select:

Only one item can be selected from the menu. 

#### Multi-select:

While most dropdowns are an extension of radio buttons (in that you can only choose one item) Multiple selection dropdown menus lets the user select multiple items in one input field.

#### Combo box:

Combines a text field with a pull-down button in a single control.

#### Multi-select with tags

Allows users to select multiple options from a dropdown list, with each selected item represented as a tag inside the input field.

### Combobox error state handling

Combo boxes support various error states to help users recover quickly and understand what went wrong. Follow these guidelines when implementing and styling error states:

#### General error

Scenario: A required field was left empty or other basic validation failed. Visual indicator:

- Red border around the field
- Red helper message shown below the field
- Selected tags (if any) remain unchanged

#### Min/max selection not met

Scenario: The number of selections made is below the minimum or above the maximum allowed. Visual indicator:

- Red border around the field
- Red helper message indicating the required range
- Selected tags or badges are not visually marked as errors

#### Tag level error

Scenario: A specific selected tag causes a custom validation issue (e.g. deprecated item, disallowed choice). Visual indicator:

- Red border around the field
- Problematic tags styled with a red error color or outline
- Optional helper message with clarification

#### Multiple errors

Scenario: Multiple validation issues appear simultaneously — for example, a required tag is missing, a deprecated tag is selected, and the user has exceeded the selection limit. Visual indicator:

- Red border around the field
- Multiple error tags styled in red
- A general error message shown below the field (may reference one or more of the issues)

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-128701%26viewport%3D1307%252C-9634%252C0.39%26t%3Dg32jPXI6hs0ehttV-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Dropdown components can support the single selection or multiple sections of options within the list.
- When a single selection is made the selected option should appear as the value within the field. If multiple options are selected, the number of options selected can appear as the value within the field. For example: "12 items selected".
- Add a label in dropdown and combo boxes. Not having a label is ambiguous and not accessible.
- Keep menu items concise. Keep menu items short and concise. Long menu items that cause text to wrap to multiple lines are discouraged.
- Use help text to show hints, formatting, and requirements.
- Use dropdown menus with a maximum of 20 items is recommended.
- Use combo boxes instead of dropdowns to allow users to filter longer lists to only the selections matching a query.
- The minimum width of dropdowns should be 96px which is 2x the height of the input field.

#### How not to use

- Do not use a dropdown if there is only one or two options to choose from. It would be better to use a radio button for choosing between two options. If the user should be able to select up to 3 options consider using checkboxes.
- Do not align field input text to the grid and hang the container.

### Behavior

- The combo box's text input functionality is intended to make large lists easier to search. If you have less than 6 items, use the radio buttons. If you have more than 6 items, consider filters if your selection list is complex enough to search and filter. If it's not complex enough for a combo box, you can use a selector.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 70


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | singleSelect | `section: "example:1"` |
| 2 | singleSelect - required | `section: "example:2"` |
| 3 | singleSelect - customItemValue | `section: "example:3"` |
| 4 | singleSelect - customItemValue-required | `section: "example:4"` |
| 5 | multiSelect | `section: "example:5"` |
| 6 | multiSelect - required | `section: "example:6"` |
| 7 | multiSelect - selectAll | `section: "example:7"` |
| 8 | multiSelect - selectAll-required | `section: "example:8"` |
| 9 | singleSelect - combobox | `section: "example:9"` |
| 10 | multiSelect - combobox | `section: "example:10"` |
| 11 | multiSelect - combobox-selectAll | `section: "example:11"` |
| 12 | multiSelectWithTags - combobox | `section: "example:12"` |
| 13 | multiSelectWithTags - combobox-selectAll | `section: "example:13"` |
| 14 | singleSelect - combobox-label | `section: "example:14"` |
| 15 | singleSelect - combobox-label-required | `section: "example:15"` |
| 16 | multiSelect - combobox-label | `section: "example:16"` |
| 17 | multiSelect - combobox-label-required | `section: "example:17"` |
| 18 | multiSelect - combobox-label-selectAll | `section: "example:18"` |
| 19 | multiSelect - combobox-label-selectAll-required | `section: "example:19"` |
| 20 | multiSelectWithTags - combobox-label | `section: "example:20"` |
| 21 | multiSelectWithTags - combobox-label-required | `section: "example:21"` |
| 22 | multiSelectWithTags - combobox-label-selectAll | `section: "example:22"` |
| 23 | multiSelectWithTags - combobox-label-selectAll-required | `section: "example:23"` |
| 24 | multiSelect - combobox - error | `section: "example:24"` |
| 25 | multiSelect - combobox - errorMinMax | `section: "example:25"` |
| 26 | multiSelect - combobox - errorTagLevel | `section: "example:26"` |
| 27 | multiSelect - combobox - errorMultiple | `section: "example:27"` |
| 28 | multiSelect - combobox-selectAll - error | `section: "example:28"` |
| 29 | multiSelect - combobox-selectAll - errorMinMax | `section: "example:29"` |
| 30 | multiSelect - combobox-selectAll - errorTagLevel | `section: "example:30"` |
| 31 | multiSelect - combobox-selectAll - errorMultiple | `section: "example:31"` |
| 32 | multiSelect - combobox-label-selectAll - error | `section: "example:32"` |
| 33 | multiSelect - combobox-label-selectAll - errorMinMax | `section: "example:33"` |
| 34 | multiSelect - combobox-label-selectAll - errorTagLevel | `section: "example:34"` |
| 35 | multiSelect - combobox-label-selectAll - errorMultiple | `section: "example:35"` |
| 36 | multiSelect - combobox-label-selectAll-required - error | `section: "example:36"` |
| 37 | multiSelect - combobox-label-selectAll-required - errorMinMax | `section: "example:37"` |
| 38 | multiSelect - combobox-label-selectAll-required - errorTagLevel | `section: "example:38"` |
| 39 | multiSelect - combobox-label-selectAll-required - errorMultiple | `section: "example:39"` |
| 40 | multiSelect - combobox-label-required - error | `section: "example:40"` |
| 41 | multiSelect - combobox-label-required - errorMinMax | `section: "example:41"` |
| 42 | multiSelect - combobox-label-required - errorTagLevel | `section: "example:42"` |
| 43 | multiSelect - combobox-label-required - errorMultiple | `section: "example:43"` |
| 44 | multiSelect - combobox-label - error | `section: "example:44"` |
| 45 | multiSelect - combobox-label - errorMinMax | `section: "example:45"` |
| 46 | multiSelect - combobox-label - errorTagLevel | `section: "example:46"` |
| 47 | multiSelect - combobox-label - errorMultiple | `section: "example:47"` |
| 48 | multiSelectWithTags - combobox-label - error | `section: "example:48"` |
| 49 | multiSelectWithTags - combobox-label - errorMinMax | `section: "example:49"` |
| 50 | multiSelectWithTags - combobox-label - errorTagLevel | `section: "example:50"` |
| 51 | multiSelectWithTags - combobox-label - errorMultiple | `section: "example:51"` |
| 52 | multiSelectWithTags - combobox-selectAll - error | `section: "example:52"` |
| 53 | multiSelectWithTags - combobox-selectAll - errorMinMax | `section: "example:53"` |
| 54 | multiSelectWithTags - combobox-selectAll - errorTagLevel | `section: "example:54"` |
| 55 | multiSelectWithTags - combobox-selectAll - errorMultiple | `section: "example:55"` |
| 56 | multiSelectWithTags - combobox-label-selectAll - error | `section: "example:56"` |
| 57 | multiSelectWithTags - combobox-label-selectAll - errorMinMax | `section: "example:57"` |
| 58 | multiSelectWithTags - combobox-label-selectAll - errorTagLevel | `section: "example:58"` |
| 59 | multiSelectWithTags - combobox-label-selectAll - errorMultiple | `section: "example:59"` |
| 60 | multiSelectWithTags - combobox-label-selectAll-required - error | `section: "example:60"` |
| 61 | multiSelectWithTags - combobox-label-selectAll-required - errorMinMax | `section: "example:61"` |
| 62 | multiSelectWithTags - combobox-label-selectAll-required - errorTagLevel | `section: "example:62"` |
| 63 | multiSelectWithTags - combobox-label-selectAll-required - errorMultiple | `section: "example:63"` |
| 64 | multiSelectWithTags - combobox-label-required - error | `section: "example:64"` |
| 65 | multiSelectWithTags - combobox-label-required - errorMinMax | `section: "example:65"` |
| 66 | multiSelectWithTags - combobox-label-required - errorTagLevel | `section: "example:66"` |
| 67 | multiSelectWithTags - combobox-label-required - errorMultiple | `section: "example:67"` |
| 68 | singleSelection - search | `section: "example:68"` |
| 69 | multiple - search | `section: "example:69"` |
| 70 | multiple - search-selectAll | `section: "example:70"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### singleSelect


**Example #1** | **Variation**: singleSelect | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required" [enableNgContent]="false"
  [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required"  [enableNgContent]="false"
    [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
title: string= "Dropdown";
selectedItem1 =  { };
required: boolean = false;
list1: any[] =[
  { value: 'item1', label: 'Default' },
  { value: 'item2', label: 'Disabled', disabled: true },
  { value: 'item3', label: 'Icon' , iconName: 'thumb-up-outline'},
  { value: 'item4', label: 'Badge', badgeValue: 'New' },
  { value: 'item5', label: 'Description', descValue: 'Lorem ipsum' },
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}

setAriaLabel(item: any): string {
  const prefixLabel = item.iconName ? `Icon ${item.iconName}, ` : '';
  const label = item.label || '';
  const badgeLabel = item.badgeValue ? `, Badge ${item.badgeValue}` : '';
  const suffixLabel = item.descValue ? `, ${item.descValue}` : '';
  return prefixLabel + label + badgeLabel + suffixLabel;
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### singleSelect - required


**Example #2** | **Variation**: singleSelect | **Modifier**: required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required" [enableNgContent]="false"
  [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required"  [enableNgContent]="false"
    [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
title: string= "Dropdown";
selectedItem1 =  { };
required: boolean = true;
list1: any[] =[
  { value: 'item1', label: 'Default' },
  { value: 'item2', label: 'Disabled', disabled: true },
  { value: 'item3', label: 'Icon' , iconName: 'thumb-up-outline'},
  { value: 'item4', label: 'Badge', badgeValue: 'New' },
  { value: 'item5', label: 'Description', descValue: 'Lorem ipsum' },
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}

setAriaLabel(item: any): string {
  const prefixLabel = item.iconName ? `Icon ${item.iconName}, ` : '';
  const label = item.label || '';
  const badgeLabel = item.badgeValue ? `, Badge ${item.badgeValue}` : '';
  const suffixLabel = item.descValue ? `, ${item.descValue}` : '';
  return prefixLabel + label + badgeLabel + suffixLabel;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### singleSelect - customItemValue


**Example #3** | **Variation**: singleSelect | **Modifier**: customItemValue | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required" [enableNgContent]="false"
    [showTag]="true" [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### TypeScript

```typescript
title: string= "Dropdown";
required: boolean = false;
selectedItem1 =  { };
list1: any[] =[
  { value: 'item1', label: 'Blue',  tagBackgroundColor: '#415385', tagFontColor: "#ffffff"},
  { value: 'item2', label: 'Orange', tagBackgroundColor: '#D04A02', tagFontColor: "#ffffff"},
  { value: 'item3', label: 'Pink', tagBackgroundColor: '#D93954', tagFontColor: "#ffffff" },
  { value: 'item4', label: 'Red', tagBackgroundColor: '#E0301E', tagFontColor: "#ffffff"},
  { value: 'item5', label: 'Green', tagBackgroundColor: '#4EB523', tagFontColor: "#ffffff" },
  { value: 'item6', label: 'Purple', tagBackgroundColor: '#8E34F4', tagFontColor: "#ffffff"}
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}  
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### singleSelect - customItemValue-required


**Example #4** | **Variation**: singleSelect | **Modifier**: customItemValue-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list1' [selectType]="'single'" [title]="title" [required]="required" [enableNgContent]="false"
    [showTag]="true" [(ngModel)]="selectedItem1" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### TypeScript

```typescript
title: string= "Dropdown";
required: boolean = true;
selectedItem1 =  { };
list1: any[] =[
  { value: 'item1', label: 'Blue',  tagBackgroundColor: '#415385', tagFontColor: "#ffffff"},
  { value: 'item2', label: 'Orange', tagBackgroundColor: '#D04A02', tagFontColor: "#ffffff"},
  { value: 'item3', label: 'Pink', tagBackgroundColor: '#D93954', tagFontColor: "#ffffff" },
  { value: 'item4', label: 'Red', tagBackgroundColor: '#E0301E', tagFontColor: "#ffffff"},
  { value: 'item5', label: 'Green', tagBackgroundColor: '#4EB523', tagFontColor: "#ffffff" },
  { value: 'item6', label: 'Purple', tagBackgroundColor: '#8E34F4', tagFontColor: "#ffffff"}
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}  
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### multiSelect


**Example #5** | **Variation**: multiSelect | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [showSelectAll]="withSelectAll" [required]="required" [enableNgContent]="false"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [required]="required" [enableNgContent]="false" [showSelectAll]="withSelectAll"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}

  title: string= "Dropdown"
  required : boolean = false;
  list2: settingData[] = [
    { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'White', showCheckbox: true, groupName: 'Color' },
      { value: 'item2', label: 'Black', showCheckbox: true, groupName: 'Color' },
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' }
    ] },
    { label: 'Size', type: 'group', items: [
      { value: 'item4', label: 'Small', showCheckbox: true, groupName: 'Size' },
      { value: 'item5', label: 'Medium', showCheckbox: true,groupName: 'Size' },
      { value: 'item6', label: 'Large',  showCheckbox: true, groupName: 'Size' },
      { value: 'item7', label: 'Extra large', showCheckbox: true, groupName: 'Size' }
    ]} 
  ]; 
  selectedItem2 = {};
  withSelectAll = false;

  onSelectItem(event:any): void{
    console.log(event);
  }

  onSelect(event: any): void {
    console.log(event.selected);
  }
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### multiSelect - required


**Example #6** | **Variation**: multiSelect | **Modifier**: required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [showSelectAll]="withSelectAll" [required]="required" [enableNgContent]="false"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [required]="required" [enableNgContent]="false" [showSelectAll]="withSelectAll"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}

  title: string= "Dropdown"
  required : boolean = true;
  list2: settingData[] = [
    { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'White', showCheckbox: true, groupName: 'Color' },
      { value: 'item2', label: 'Black', showCheckbox: true, groupName: 'Color' },
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' }
    ] },
    { label: 'Size', type: 'group', items: [
      { value: 'item4', label: 'Small', showCheckbox: true, groupName: 'Size' },
      { value: 'item5', label: 'Medium', showCheckbox: true,groupName: 'Size' },
      { value: 'item6', label: 'Large',  showCheckbox: true, groupName: 'Size' },
      { value: 'item7', label: 'Extra large', showCheckbox: true, groupName: 'Size' }
    ]} 
  ]; 
  selectedItem2 = {};
  withSelectAll = false;

  onSelectItem(event:any): void{
    console.log(event);
  }

  onSelect(event: any): void {
    console.log(event.selected);
  }
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### multiSelect - selectAll


**Example #7** | **Variation**: multiSelect | **Modifier**: selectAll | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [showSelectAll]="withSelectAll" [required]="required" [enableNgContent]="false"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [required]="required" [enableNgContent]="false" [showSelectAll]="withSelectAll"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}

  title: string= "Dropdown"
  required : boolean = false;
  list2: settingData[] = [
    { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'White', showCheckbox: true, groupName: 'Color' },
      { value: 'item2', label: 'Black', showCheckbox: true, groupName: 'Color' },
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' }
    ] },
    { label: 'Size', type: 'group', items: [
      { value: 'item4', label: 'Small', showCheckbox: true, groupName: 'Size' },
      { value: 'item5', label: 'Medium', showCheckbox: true,groupName: 'Size' },
      { value: 'item6', label: 'Large',  showCheckbox: true, groupName: 'Size' },
      { value: 'item7', label: 'Extra large', showCheckbox: true, groupName: 'Size' }
    ]} 
  ]; 
  selectedItem2 = {};
  withSelectAll = true;

  onSelectItem(event:any): void{
    console.log(event);
  }

  onSelect(event: any): void {
    console.log(event.selected);
  }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### multiSelect - selectAll-required


**Example #8** | **Variation**: multiSelect | **Modifier**: selectAll-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [showSelectAll]="withSelectAll" [required]="required" [enableNgContent]="false"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
</ap-dropdown>
```

#### HTML Extra

```html
<!-- Use below template to add customized content to each dropdown item -->
<ap-dropdown [list]='list2' [selectType]="'multiple'" [title]="title" [required]="required" [enableNgContent]="false" [showSelectAll]="withSelectAll"
    [(ngModel)]="selectedItem2" (onSelect)="onSelect($event)">
    <ng-template let-item ngTemplate="itemTemplate">
      <!--Customized content here -->
    </ng-template>
</ap-dropdown>
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}

  title: string= "Dropdown"
  required : boolean = true;
  list2: settingData[] = [
    { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'White', showCheckbox: true, groupName: 'Color' },
      { value: 'item2', label: 'Black', showCheckbox: true, groupName: 'Color' },
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' }
    ] },
    { label: 'Size', type: 'group', items: [
      { value: 'item4', label: 'Small', showCheckbox: true, groupName: 'Size' },
      { value: 'item5', label: 'Medium', showCheckbox: true,groupName: 'Size' },
      { value: 'item6', label: 'Large',  showCheckbox: true, groupName: 'Size' },
      { value: 'item7', label: 'Extra large', showCheckbox: true, groupName: 'Size' }
    ]} 
  ]; 
  selectedItem2 = {};
  withSelectAll = true;

  onSelectItem(event:any): void{
    console.log(event);
  }

  onSelect(event: any): void {
    console.log(event.selected);
  }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### singleSelect - combobox


**Example #9** | **Variation**: singleSelect | **Modifier**: combobox | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox  [list]="list3" [selectType]="'single'" [(ngModel)]="selectedItem3" [required]="required"  (onFilterResult)="onFilterResult($event)"
      (onSelect)="onSelect($event)" [ariaLabelledBy]="'comboboxTitle'">
       <ng-container *ngFor="let item of list3; let i = index;">
          <ap-dropdown-list-item   [item]="item" [ariaLabel]="setAriaLabel(item)"
              [highlightText]="highlightText">
          </ap-dropdown-list-item>
      </ng-container>
  </ap-combobox>
</div>  
```

#### TypeScript

```typescript
highlightText: string =" ";
ariaLabelledBy: string = "";
selectedItem3 =  { };
required:boolean = false;
list3: any =[
  { value: 'item1', label: 'Blue' },
  { value: 'item2', label: 'Orange' },
  { value: 'item3', label: 'Pink' },
  { value: 'item4', label: 'Red', },
  { value: 'item5', label: 'Purple'},
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}

onFilterResult(event: any): void {
  this.highlightText = event.searchStr;
  if(event.result && event.result.length > 0) {
    this.list3 = event.result;
  }else {
    this.list3 = [];
  }
}

setAriaLabel(item: any): string {
  const prefixLabel = item.iconName ? `Icon ${item.iconName}, ` : '';
  const label = item.label || '';
  const badgeLabel = item.badgeValue ? `, Badge ${item.badgeValue}` : '';
  const suffixLabel = item.descValue ? `, ${item.descValue}` : '';
  return prefixLabel + label + badgeLabel + suffixLabel;
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### multiSelect - combobox


**Example #10** | **Variation**: multiSelect | **Modifier**: combobox | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### multiSelect - combobox-selectAll


**Example #11** | **Variation**: multiSelect | **Modifier**: combobox-selectAll | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### multiSelectWithTags - combobox


**Example #12** | **Variation**: multiSelectWithTags | **Modifier**: combobox | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = true;
required: boolean = false;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### multiSelectWithTags - combobox-selectAll


**Example #13** | **Variation**: multiSelectWithTags | **Modifier**: combobox-selectAll | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = true;
required: boolean = false;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### singleSelect - combobox-label


**Example #14** | **Variation**: singleSelect | **Modifier**: combobox-label | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [list]="list3" [selectType]="'single'" [(ngModel)]="selectedItem3" [required]="required"  (onFilterResult)="onFilterResult($event)"
      (onSelect)="onSelect($event)" [ariaLabelledBy]="'comboboxTitle'">
       <ng-container *ngFor="let item of list3; let i = index;">
          <ap-dropdown-list-item   [item]="item" [ariaLabel]="setAriaLabel(item)"
              [highlightText]="highlightText">
          </ap-dropdown-list-item>
      </ng-container>
  </ap-combobox>
</div>  
```

#### TypeScript

```typescript
highlightText: string =" ";
ariaLabelledBy: string = "";
selectedItem3 =  { };
required:boolean = false;
list3: any =[
  { value: 'item1', label: 'Blue' },
  { value: 'item2', label: 'Orange' },
  { value: 'item3', label: 'Pink' },
  { value: 'item4', label: 'Red', },
  { value: 'item5', label: 'Purple'},
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}

onFilterResult(event: any): void {
  this.highlightText = event.searchStr;
  if(event.result && event.result.length > 0) {
    this.list3 = event.result;
  }else {
    this.list3 = [];
  }
}

setAriaLabel(item: any): string {
  const prefixLabel = item.iconName ? `Icon ${item.iconName}, ` : '';
  const label = item.label || '';
  const badgeLabel = item.badgeValue ? `, Badge ${item.badgeValue}` : '';
  const suffixLabel = item.descValue ? `, ${item.descValue}` : '';
  return prefixLabel + label + badgeLabel + suffixLabel;
}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### singleSelect - combobox-label-required


**Example #15** | **Variation**: singleSelect | **Modifier**: combobox-label-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [list]="list3" [selectType]="'single'" [(ngModel)]="selectedItem3" [required]="required"  (onFilterResult)="onFilterResult($event)"
      (onSelect)="onSelect($event)" [ariaLabelledBy]="'comboboxTitle'">
       <ng-container *ngFor="let item of list3; let i = index;">
          <ap-dropdown-list-item   [item]="item" [ariaLabel]="setAriaLabel(item)"
              [highlightText]="highlightText">
          </ap-dropdown-list-item>
      </ng-container>
  </ap-combobox>
</div>  
```

#### TypeScript

```typescript
highlightText: string =" ";
ariaLabelledBy: string = "";
selectedItem3 =  { };
required:boolean = true;
list3: any =[
  { value: 'item1', label: 'Blue' },
  { value: 'item2', label: 'Orange' },
  { value: 'item3', label: 'Pink' },
  { value: 'item4', label: 'Red', },
  { value: 'item5', label: 'Purple'},
];

onSelectItem(event:any): void{
  console.log(event);
}

onSelect(event: any): void {
  console.log(event);
}

onFilterResult(event: any): void {
  this.highlightText = event.searchStr;
  if(event.result && event.result.length > 0) {
    this.list3 = event.result;
  }else {
    this.list3 = [];
  }
}

setAriaLabel(item: any): string {
  const prefixLabel = item.iconName ? `Icon ${item.iconName}, ` : '';
  const label = item.label || '';
  const badgeLabel = item.badgeValue ? `, Badge ${item.badgeValue}` : '';
  const suffixLabel = item.descValue ? `, ${item.descValue}` : '';
  return prefixLabel + label + badgeLabel + suffixLabel;
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### multiSelect - combobox-label


**Example #16** | **Variation**: multiSelect | **Modifier**: combobox-label | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### multiSelect - combobox-label-required


**Example #17** | **Variation**: multiSelect | **Modifier**: combobox-label-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = true;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### multiSelect - combobox-label-selectAll


**Example #18** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### multiSelect - combobox-label-selectAll-required


**Example #19** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = true;
comboboxError = false;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### multiSelectWithTags - combobox-label


**Example #20** | **Variation**: multiSelectWithTags | **Modifier**: combobox-label | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = true;
required: boolean = false;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### multiSelectWithTags - combobox-label-required


**Example #21** | **Variation**: multiSelectWithTags | **Modifier**: combobox-label-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = true;
required: boolean = true;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### multiSelectWithTags - combobox-label-selectAll


**Example #22** | **Variation**: multiSelectWithTags | **Modifier**: combobox-label-selectAll | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = true;
required: boolean = false;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### multiSelectWithTags - combobox-label-selectAll-required


**Example #23** | **Variation**: multiSelectWithTags | **Modifier**: combobox-label-selectAll-required | **State**: None

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
  <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
  [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError" 
  (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'">
      <ng-container *ngFor="let group of list4">
        <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
        <ng-container *ngFor="let item of group.items">
            <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                [highlightText]="highlightText"
                [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
            </ap-dropdown-list-item>
        </ng-container>
      </ng-container>
  </ap-combobox>

</div>  
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = true;
required: boolean = true;
comboboxError = false;


  list4: settingData[]= [ 
      { label: 'Color', type: 'group', items: [
          { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
          { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
          { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
          { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
          { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
          { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
        ] 
      }
    ];

  selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);

}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### multiSelect - combobox - error


**Example #24** | **Variation**: multiSelect | **Modifier**: combobox | **State**: error

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">

     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>Please select something else</div>
       </div>


   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### multiSelect - combobox - errorMinMax


**Example #25** | **Variation**: multiSelect | **Modifier**: combobox | **State**: errorMinMax

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="'error-minMax'">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">


     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">Please remove some items to continue</div>
       </div>

   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount;


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### multiSelect - combobox - errorTagLevel


**Example #26** | **Variation**: multiSelect | **Modifier**: combobox | **State**: errorTagLevel

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">



     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>"Blue" is invalid and cannot be used</div>
       </div>
   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### multiSelect - combobox - errorMultiple


**Example #27** | **Variation**: multiSelect | **Modifier**: combobox | **State**: errorMultiple

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="getErrorMinMaxStatus() ? 'error-minMax' : ''">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">
     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="getSelectedItem()">The tag "Blue" is invalid and cannot be used</div>
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">You can select up to three items</div>
        </div>



   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = this.list4[0].items.slice(0, 4);


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}

getErrorMinMaxStatus(): boolean{
  return  !this.getSelectedItem() && (this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount);
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() || this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### multiSelect - combobox-selectAll - error


**Example #28** | **Variation**: multiSelect | **Modifier**: combobox-selectAll | **State**: error

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">

     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>Please select something else</div>
       </div>


   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### multiSelect - combobox-selectAll - errorMinMax


**Example #29** | **Variation**: multiSelect | **Modifier**: combobox-selectAll | **State**: errorMinMax

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="'error-minMax'">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">


     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">Please remove some items to continue</div>
       </div>

   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount;


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### multiSelect - combobox-selectAll - errorTagLevel


**Example #30** | **Variation**: multiSelect | **Modifier**: combobox-selectAll | **State**: errorTagLevel

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">



     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>"Blue" is invalid and cannot be used</div>
       </div>
   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### multiSelect - combobox-selectAll - errorMultiple


**Example #31** | **Variation**: multiSelect | **Modifier**: combobox-selectAll | **State**: errorMultiple

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox  [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="getErrorMinMaxStatus() ? 'error-minMax' : ''">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">
     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="getSelectedItem()">The tag "Blue" is invalid and cannot be used</div>
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">You can select up to three items</div>
        </div>



   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = this.list4[0].items.slice(0, 4);


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}

getErrorMinMaxStatus(): boolean{
  return  !this.getSelectedItem() && (this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount);
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() || this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### multiSelect - combobox-label-selectAll - error


**Example #32** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll | **State**: error

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">

     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>Please select something else</div>
       </div>


   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### multiSelect - combobox-label-selectAll - errorMinMax


**Example #33** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll | **State**: errorMinMax

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="'error-minMax'">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">


     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">Please remove some items to continue</div>
       </div>

   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount;


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### multiSelect - combobox-label-selectAll - errorTagLevel


**Example #34** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll | **State**: errorTagLevel

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">



     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>"Blue" is invalid and cannot be used</div>
       </div>
   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### multiSelect - combobox-label-selectAll - errorMultiple


**Example #35** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll | **State**: errorMultiple

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="getErrorMinMaxStatus() ? 'error-minMax' : ''">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">
     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="getSelectedItem()">The tag "Blue" is invalid and cannot be used</div>
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">You can select up to three items</div>
        </div>



   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = false;
comboboxError = true;

minCount = 1;
maxCount = 3;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = this.list4[0].items.slice(0, 4);


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}

getErrorMinMaxStatus(): boolean{
  return  !this.getSelectedItem() && (this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount);
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() || this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### multiSelect - combobox-label-selectAll-required - error


**Example #36** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll-required | **State**: error

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">

     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>Please select something else</div>
       </div>


   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### multiSelect - combobox-label-selectAll-required - errorMinMax


**Example #37** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll-required | **State**: errorMinMax

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="'error-minMax'">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">


     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">Please remove some items to continue</div>
       </div>

   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;

minCount = 1;
maxCount = 3;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount;


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### multiSelect - combobox-label-selectAll-required - errorTagLevel


**Example #38** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll-required | **State**: errorTagLevel

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">



     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>"Blue" is invalid and cannot be used</div>
       </div>
   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### multiSelect - combobox-label-selectAll-required - errorMultiple


**Example #39** | **Variation**: multiSelect | **Modifier**: combobox-label-selectAll-required | **State**: errorMultiple

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="getErrorMinMaxStatus() ? 'error-minMax' : ''">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">
     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="getSelectedItem()">The tag "Blue" is invalid and cannot be used</div>
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">You can select up to three items</div>
        </div>



   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = true;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;

minCount = 1;
maxCount = 3;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = this.list4[0].items.slice(0, 4);


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}

getErrorMinMaxStatus(): boolean{
  return  !this.getSelectedItem() && (this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount);
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() || this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### multiSelect - combobox-label-required - error


**Example #40** | **Variation**: multiSelect | **Modifier**: combobox-label-required | **State**: error

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">

     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>Please select something else</div>
       </div>


   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### multiSelect - combobox-label-required - errorMinMax


**Example #41** | **Variation**: multiSelect | **Modifier**: combobox-label-required | **State**: errorMinMax

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="'error-minMax'">
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">


     <div class="ap-combobox-validation-error" aria-live="polite">
         <div *ngIf="selectedItem4.length < minCount">Please select something</div>
         <div *ngIf="selectedItem4.length > maxCount">Please remove some items to continue</div>
       </div>

   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;

minCount = 1;
maxCount = 3;



list4: settingData[]= [ 
    { label: 'Color', type: 'group', items: [
        { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color'},
        { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color'},
        { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color' },
        { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color' },
        { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color' },
        { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color'},
      ] 
    }
  ];

selectedItem4 = [];


onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.selectedItem4.length < this.minCount || this.selectedItem4.length > this.maxCount;


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }

 ::ng-deep .ap-combobox.error-minMax .ap-combobox-input-box .ap-tag.ap-tag-default.ap-tag-sm.combobox-tag {
   background-color: $color-background-hover-selected !important;
   color: $color-text-body !important;
 }
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### multiSelect - combobox-label-required - errorTagLevel


**Example #42** | **Variation**: multiSelect | **Modifier**: combobox-label-required | **State**: errorTagLevel

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" >
     <ng-container *ngFor="let group of list4">
         <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
         <ng-container *ngFor="let item of group.items">
             <ap-dropdown-list-item  [item]="item" [role]="'checkbox'"
                 [highlightText]="highlightText"
                 [showCheckbox]="item.showCheckbox" (onSelectItem)="onSelectItem($event)">
             </ap-dropdown-list-item>
         </ng-container>
     </ng-container>
 </ap-combobox>

  <div role="alert" *ngIf="comboboxError">



     <div class="ap-combobox-validation-error" aria-live="polite">
         <div>"Blue" is invalid and cannot be used</div>
       </div>
   </div>
</div> 
```

#### TypeScript

```typescript
interface settingData {
  label: string;
  value?: string;
  type?: string;
  groupName?: string;
  checked?: boolean;
  disabled?: boolean;
  showCheckbox?: boolean;
  items?: any;
}


msg: string= "Nothing matches your results";
highlightText: string = '';
withSelectAll = false;
withMultipleTags = false;
required: boolean = true;
comboboxError = true;


list4: settingData[]= [ 
  { label: 'Color', type: 'group', items: [
      { value: 'item1', label: 'Blue', showCheckbox: true, groupName: 'Color', checked: true},
      { value: 'item2', label: 'Orange', showCheckbox: true, groupName: 'Color', checked: false},
      { value: 'item3', label: 'Pink', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item4', label: 'Red', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item5', label: 'Teal', showCheckbox: true, groupName: 'Color', checked: false },
      { value: 'item6', label: 'Purple', showCheckbox: true, groupName: 'Color', checked: false},
    ] 
  }
];

selectedItem4 = [{
    "value": "item1",
    "label": "Blue",
    "showCheckbox": true,
    "groupName": "Color",
    "checked": true
}];


getSelectedItem(){
  let hasBlueItem = this.selectedItem4.some((item: any) =>
      item.label && item.label.toLowerCase().includes('blue')
    );
  return hasBlueItem;  
}




onSelect(event: any): void {
  console.log(event);


  this.comboboxError = this.getSelectedItem() ; 


}

onFilterResult2(event:any): void{
  this.highlightText = event.searchStr;
  if(event.result && event.result.length> 0) {
    this.list4 = event.result;
  }else {
    this.list4= [];
  }
}

onSelectItem(event:any): void{
  console.log(event);
}
```

#### SCSS Styles

```scss
@import './node_modules/@appkit4/styles/scss/_variables.scss';
 .ap-combobox-validation-error {
   margin: $spacing-3 0 0 $spacing-3; // 8px 0 0 8px;
   height: 12px;
   line-height: 12px;
   font-size: 12px;
   color: $color-text-error;

   &>div {
     margin-bottom: 0.25rem;
   }
 }
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### multiSelect - combobox-label-required - errorMultiple


**Example #43** | **Variation**: multiSelect | **Modifier**: combobox-label-required | **State**: errorMultiple

#### Module Import

```typescript
import { DropdownModule} from '@appkit4/angular-components/dropdown';
import { BadgeModule } from "@appkit4/angular-components/badge";
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { ComboboxModule } from "@appkit4/angular-components/combobox";
import { TagModule } from "@appkit4/angular-components/tag";
import { FormsModule } from "@angular/forms"; 
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<div class="ap-combobox-demo-wrapper">
 <ap-combobox [comboboxTitle]="'Combobox title'" [selectType]="'multiple'" [list]="list4" [showSelectAll]="withSelectAll" [required]="required"
 [showMultipleTags]="withMultipleTags" [(ngModel)]="selectedItem4" (onSelect)="onSelect($event)" [error]="comboboxError"
 (onFilterResult)="onFilterResult2($event)" [ariaLabelledBy]="'comboboxTitle'" [styleClass]="getErrorMinMaxStatus() ? 'error-minMax' : ''">
   

---
**WARNING:** Response truncated at 150000 characters. Use section parameter for specific content to avoid truncation.