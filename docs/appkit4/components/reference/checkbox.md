---
component: checkbox
framework: angular
---

# Checkbox Component

## Overview

AppKit Checkbox component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use checkboxes to:
- Select one or more options from a number of choices.
- Allow the user to activate a setting or selection.

### Anatomy

- **Checkbox:** selection control.
- **Label:** Text associated with choice item.

### Variants

Usage of one or the other depends on the user and the product.

#### Single

Represents one user

#### List (Horizontal)

Represents one user

#### List (Vertical)

Represents one user

### State

#### Selected

The selected state of checkboxes is used to indicate the selection or completion of an action or item.

#### Deselected

The deselected state of checkboxes is used to indicate that an item or action has not been selected or is not in a completed state.

#### Indeterminate

An indeterminate checkbox is a visual representation of an "intermediate" state between "checked" and "unchecked". Here are a few scenarios when designers and coders should use indeterminate checkboxes in design systems documentation:

### Indeterminate

Parent-Child Relationships: In a parent-child relationship, where a group of checkboxes are related, the parent checkbox can be indeterminate if some, but not all, of its children are checked.

Group Selection: When a group of related checkboxes are selected, the group checkbox can be indeterminate to show that some, but not all, options are selected.

Mixed Selection: When a selection of checkboxes has a mix of checked and unchecked options, the parent checkbox can be indeterminate to show this mixed state.

### Alignment and Placement

- Only use the label provided with the checkbox component – do not add custom text and pair with a standalone checkbox icon
- Contextual grouping: Group checkboxes based on their context or related functionality. For example, grouping all checkboxes related to notifications or related preferences.
- User-centered: Consider the needs and expectations of the end-user when grouping checkboxes and ensure that the groupings make sense and are easy to understand.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-120300%26viewport%3D527%252C-4439%252C0.4%26t%3D8NzmLdtmnv2gd0ZG-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Checkboxes should be used in combination with a description or label.
- A single checkbox can be used to acknowledge a statement, a list of 2 or more checkboxes can be used for displaying a set of choices
- When providing many checkboxes to the user, consider also providing a way to "reset" or "select all"
- Whenever possible, arrange checkboxes vertically to offer better scanning.
- When placing multiple groups of checkboxes horizontally, the spacing should be 20px.
- Checkboxes are usually used in forms of modals or directly on the page. Checkboxes within a form or modal must be placed at least 20px below or before the text.

#### How not to use

- Do not use a checkbox when only one option is allowed to be selected from a list. Consider using a radio button instead.

### Behavior

- Consider using alternative input methods, such as dropdown menus or toggle switches, for lengthy lists of checkboxes
- Consider truncating long lists of checkboxes with an option to expand and view all.
- Provide filtering or search functionality: For long lists of checkboxes, it can be helpful to provide filtering or search functionality that allows users to quickly find the options they need.
- Nesting logic is not currently supported
- In general, it's best to follow the visual order of the checkboxes, from left to right and top to bottom. This can help users navigate the checkboxes efficiently using the keyboard.

### Accessibility

- Use native HTML, <input type="checkbox">.
- Associate the label with the input.
- Use <label> tag to define a label to the right of the checkbox.
- Use aria-required="true" if checkbox is required and indicate with an asterisk * next to label.
- List of checkboxes should be grouped in a <fieldset>

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 30


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | selected | `section: "example:1"` |
| 2 | selected - label | `section: "example:2"` |
| 3 | selected - list | `section: "example:3"` |
| 4 | selected - disabled | `section: "example:4"` |
| 5 | selected - label-list | `section: "example:5"` |
| 6 | selected - label - disabled | `section: "example:6"` |
| 7 | selected - list - disabled | `section: "example:7"` |
| 8 | selected - label-list - disabled | `section: "example:8"` |
| 9 | deselected | `section: "example:9"` |
| 10 | deselected - label | `section: "example:10"` |
| 11 | deselected - list | `section: "example:11"` |
| 12 | deselected - disabled | `section: "example:12"` |
| 13 | deselected - label-list | `section: "example:13"` |
| 14 | deselected - label - disabled | `section: "example:14"` |
| 15 | deselected - list - disabled | `section: "example:15"` |
| 16 | deselected - label-list - disabled | `section: "example:16"` |
| 17 | indeterminate | `section: "example:17"` |
| 18 | indeterminate - label | `section: "example:18"` |
| 19 | indeterminate - disabled | `section: "example:19"` |
| 20 | indeterminate - label - disabled | `section: "example:20"` |
| 21 | selected - label-required | `section: "example:21"` |
| 22 | selected - label-required - disabled | `section: "example:22"` |
| 23 | deselected - label-required | `section: "example:23"` |
| 24 | deselected - label-required - disabled | `section: "example:24"` |
| 25 | indeterminate - label-required | `section: "example:25"` |
| 26 | indeterminate - label-required - disabled | `section: "example:26"` |
| 27 | selected - label-list-required | `section: "example:27"` |
| 28 | selected - label-list-required - disabled | `section: "example:28"` |
| 29 | deselected - label-list-required | `section: "example:29"` |
| 30 | deselected - label-list-required - disabled | `section: "example:30"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### selected


**Example #1** | **Variation**: selected | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = false
showCheckboxLabel = false;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### selected - label


**Example #2** | **Variation**: selected | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = false
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### selected - list


**Example #3** | **Variation**: selected | **Modifier**: list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = false;
showCheckboxLabel = false;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### selected - disabled


**Example #4** | **Variation**: selected | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = true
showCheckboxLabel = false;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### selected - label-list


**Example #5** | **Variation**: selected | **Modifier**: label-list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = false;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### selected - label - disabled


**Example #6** | **Variation**: selected | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = true
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### selected - list - disabled


**Example #7** | **Variation**: selected | **Modifier**: list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = true;
showCheckboxLabel = false;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### selected - label-list - disabled


**Example #8** | **Variation**: selected | **Modifier**: label-list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = true;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### deselected


**Example #9** | **Variation**: deselected | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = false;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### deselected - label


**Example #10** | **Variation**: deselected | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### deselected - list


**Example #11** | **Variation**: deselected | **Modifier**: list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = false;
showCheckboxLabel = false;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### deselected - disabled


**Example #12** | **Variation**: deselected | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = false;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### deselected - label-list


**Example #13** | **Variation**: deselected | **Modifier**: label-list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = false;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### deselected - label - disabled


**Example #14** | **Variation**: deselected | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### deselected - list - disabled


**Example #15** | **Variation**: deselected | **Modifier**: list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = true;
showCheckboxLabel = false;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### deselected - label-list - disabled


**Example #16** | **Variation**: deselected | **Modifier**: label-list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="false">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = true;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### indeterminate


**Example #17** | **Variation**: indeterminate | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = false;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### indeterminate - label


**Example #18** | **Variation**: indeterminate | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = true;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### indeterminate - disabled


**Example #19** | **Variation**: indeterminate | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = false;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### indeterminate - label - disabled


**Example #20** | **Variation**: indeterminate | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="false">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = true;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### selected - label-required


**Example #21** | **Variation**: selected | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = false
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### selected - label-required - disabled


**Example #22** | **Variation**: selected | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = true
checkboxDisabled = true
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### deselected - label-required


**Example #23** | **Variation**: deselected | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### deselected - label-required - disabled


**Example #24** | **Variation**: deselected | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = true;
tempState = false

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### indeterminate - label-required


**Example #25** | **Variation**: indeterminate | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = false
showCheckboxLabel = true;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### indeterminate - label-required - disabled


**Example #26** | **Variation**: indeterminate | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ap-checkbox [(ngModel)]="checkboxSelected" [disabled]="checkboxDisabled"
    [indeterminate]="tempState" (onChange)="!checkboxDisabled && onClick($event)" [required]="true">
  <span *ngIf="showCheckboxLabel">Label</span>
  <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-checkbox>
```

#### TypeScript

```typescript
checkboxSelected = false
checkboxDisabled = true
showCheckboxLabel = true;
tempState = true

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### selected - label-list-required


**Example #27** | **Variation**: selected | **Modifier**: label-list-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="true">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = false;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### selected - label-list-required - disabled


**Example #28** | **Variation**: selected | **Modifier**: label-list-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="true">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = true;
checkboxDisabled = true;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: true},
  {name: 'Stockholm', key: 'M', selected: true},
  {name: 'São Paulo', key: 'P', selected: true}, 
  {name: 'Saint Petersburg', key: 'R', selected: true}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### deselected - label-list-required


**Example #29** | **Variation**: deselected | **Modifier**: label-list-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="true">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = false;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### deselected - label-list-required - disabled


**Example #30** | **Variation**: deselected | **Modifier**: label-list-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
```

#### HTML Template

```html
<ul>
  <li *ngFor="let item of list"  (onChange)="checkboxDisabled &&  onClick($event)" [ngStyle]="{'list-style-type': 'none'}">
        <ap-checkbox [(ngModel)]="item.selected" [disabled]="checkboxDisabled" [indeterminate]="tempState" [required]="true">   
            <span *ngIf="showCheckboxLabel ">{{item.name }}</span>
            <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
            <!-- <ng-template ngTemplate="checkedLabel">{{item.name }} is checked</ng-template>
            <ng-template ngTemplate="uncheckedLabel">{{item.name }} is unchecked</ng-template>  -->
        </ap-checkbox>
  </li>
</ul>
```

#### TypeScript

```typescript
checkboxSelected = false;
checkboxDisabled = true;
showCheckboxLabel = true;
tempState = false

list = [
  {name: 'Hong Kong', key: 'A', selected: false},
  {name: 'Stockholm', key: 'M', selected: false},
  {name: 'São Paulo', key: 'P', selected: false}, 
  {name: 'Saint Petersburg', key: 'R', selected: false}
];

onClick(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:30 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| inputId | string | Identifier of the focus input to match a label defined for the component. | - | 4.0.0 |
| ngModel | boolean | Default state of the checkbox, two-way binding is supported. | false | 4.0.0 |
| disabled | boolean | If it is true, it specifies that the checkbox is disabled. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| onChange | EventEmitter<{ originEvent: Event, checkboxState: boolean }> | Callback to invoke when the state of the checkbox changes. Event properties: • originEvent: Event. • checkboxState: Whether the checkbox is checked. | - | 4.0.0 |
| readonly | boolean | If set to true, it specifies that the checkbox cannot be edited. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.0.0 |
| required | boolean | If set to true, it specifies that the checkbox need to be checked before submittting the form. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.0.0 |
| indeterminate | boolean | When being a checkbox, setting styles after the child part is selected.. | false | 4.0.0 |
| tabindex | number | The tabindex of the checkbox. | 0 | 4.0.0 |
| ariaLabel | string | The value of aria-label of the checkbox, but if the ariaLabelLedby has value, this property will be ignored. | - | 4.3.0 |
| ariaLabelledBy | string | The value of aria-labelledby of the checkbox. | - | 4.3.0 |
| ariaDescribedby | string | The value of aria-describedby of the checkbox. | - | 4.3.0 |
| labelPosition | string: 'before'\|'after' | It specifies the position where label should be placed, before or after the checkbox. | 'after' | 4.3.0 |
| onIndeterminateChange | EventEmitter<boolean> | Callback to invoke when the indeterminate value of the checkbox changes. | - | 4.3.0 |
| focus | () => void | Apply focus to the checkbox. Note that the outline focus indicator only appears when triggered by keyboard. | - | 4.3.0 |
| toggle | (event: Event) => void | Toggle the checked state of the checkbox. | - | 4.3.0 |
| setCheckboxState | (event: { originEvent: Event, checked: boolean, indeterminate?: boolean }) => void | Set checked or indeterminate state of the checkbox. Method parameters: • originEvent: Event. • checked: Set checkbox checked or not. • indeterminate: Set checkbox indeterminate or not. | - | 4.3.0 |


<!-- /SECTION:properties -->