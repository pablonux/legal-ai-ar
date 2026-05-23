---
component: radio
framework: angular
---

# Radio Component

## Overview

AppKit Radio component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Radio button:

- When users need to select one option from a group of mutually exclusive options.

### Anatomy

**1. Icon:** Represent the selectable option.

**2. Label:** Describes the option.

### Variants

#### Selected:

Selected

#### Deselected:

Deselected

#### List modifier:

A set of radio buttons that function as a group, allowing only one option to be selected at a time.

#### List modifier:

A set of radio buttons that function as a group, allowing only one option to be selected at a time.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-108365%26viewport%3D1737%252C-47368%252C0.59%26t%3D6s0zWc6cVMGlIUwD-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use clear and concise labels.
- Group related options together.
- Provide clear visual indicators for selected options.
- A radio button should always include a label for each possible selection.
- Only one option can be selected using a radio button. There must be at least two options available to choose from.

#### How not to use

- Do not use a radio button to support the selection of multiple options.
- Do not use a radio button if there is only one option.
- Do not use a radio button without a label.
- Do not use a radio button as a form of navigation or to trigger an action.

### Behavior

- Specs of interactions and responsive behavior both embedded in components and these needed to be defined by the designer ? Figma interactions / how to use (tutorial video coming soon)

### Accessibility

- Each radio button should have a <label>. Associate the two by matching the <label>’s for attribute to the <input>’s id attribute.
- Group related radio buttons together with <fieldset> and describe the group with <legend>.
- Radio buttons should provide current state to screen reader (selected or unselected).

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 44


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
| 13 | deselectedd - label-list | `section: "example:13"` |
| 14 | deselected - label - disabled | `section: "example:14"` |
| 15 | deselected - list - disabled | `section: "example:15"` |
| 16 | deselected - label-list - disabled | `section: "example:16"` |
| 17 | selected - listHorizontal | `section: "example:17"` |
| 18 | selected - listHorizontal - disabled | `section: "example:18"` |
| 19 | selected - listVertical | `section: "example:19"` |
| 20 | selected - listVertical - disabled | `section: "example:20"` |
| 21 | selected - label-listHorizontal | `section: "example:21"` |
| 22 | selected - label-listHorizontal - disabled | `section: "example:22"` |
| 23 | selected - label-listVertical | `section: "example:23"` |
| 24 | selected - label-listVertical - disabled | `section: "example:24"` |
| 25 | deselected - listHorizontal | `section: "example:25"` |
| 26 | deselected - listHorizontal - disabled | `section: "example:26"` |
| 27 | deselected - listVertical | `section: "example:27"` |
| 28 | deselected - listVertical - disabled | `section: "example:28"` |
| 29 | deselected - label-listHorizontal | `section: "example:29"` |
| 30 | deselected - label-listHorizontal - disabled | `section: "example:30"` |
| 31 | deselected - label-listVertical | `section: "example:31"` |
| 32 | deselected - label-listVertical - disabled | `section: "example:32"` |
| 33 | selected - label-required | `section: "example:33"` |
| 34 | selected - label-required - disabled | `section: "example:34"` |
| 35 | selected - label-listHorizontal-required | `section: "example:35"` |
| 36 | selected - label-listHorizontal-required - disabled | `section: "example:36"` |
| 37 | selected - label-listVertical-required | `section: "example:37"` |
| 38 | selected - label-listVertical-required - disabled | `section: "example:38"` |
| 39 | deselected - label-required | `section: "example:39"` |
| 40 | deselected - label-required - disabled | `section: "example:40"` |
| 41 | deselected - label-listHorizontal-required | `section: "example:41"` |
| 42 | deselected - label-listHorizontal-required - disabled | `section: "example:42"` |
| 43 | deselected - label-listVertical-required | `section: "example:43"` |
| 44 | deselected - label-listVertical-required - disabled | `section: "example:44"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### selected


**Example #1** | **Variation**: selected | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="false">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = false;
singleSelect:string= "test";
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### selected - label


**Example #2** | **Variation**: selected | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="false">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = true;
singleSelect:string= "test";
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### selected - list


**Example #3** | **Variation**: selected | **Modifier**: list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### selected - disabled


**Example #4** | **Variation**: selected | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="false">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = false;
singleSelect:string= "test";
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### selected - label-list


**Example #5** | **Variation**: selected | **Modifier**: label-list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### selected - label - disabled


**Example #6** | **Variation**: selected | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="false">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = true;
singleSelect:string= "test";
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### selected - list - disabled


**Example #7** | **Variation**: selected | **Modifier**: list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### selected - label-list - disabled


**Example #8** | **Variation**: selected | **Modifier**: label-list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### deselected


**Example #9** | **Variation**: deselected | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="false">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = false;

onRadioStateChange(event:any): void {
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
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="false">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = true;

onRadioStateChange(event:any): void {
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
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### deselected - disabled


**Example #12** | **Variation**: deselected | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="false">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = false;

onRadioStateChange(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### deselectedd - label-list


**Example #13** | **Variation**: deselectedd | **Modifier**: label-list | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### deselected - label - disabled


**Example #14** | **Variation**: deselected | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="false">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = true;

onRadioStateChange(event:any): void {
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
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### deselected - label-list - disabled


**Example #16** | **Variation**: deselected | **Modifier**: label-list | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### selected - listHorizontal


**Example #17** | **Variation**: selected | **Modifier**: listHorizontal | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### selected - listHorizontal - disabled


**Example #18** | **Variation**: selected | **Modifier**: listHorizontal | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### selected - listVertical


**Example #19** | **Variation**: selected | **Modifier**: listVertical | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### selected - listVertical - disabled


**Example #20** | **Variation**: selected | **Modifier**: listVertical | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### selected - label-listHorizontal


**Example #21** | **Variation**: selected | **Modifier**: label-listHorizontal | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### selected - label-listHorizontal - disabled


**Example #22** | **Variation**: selected | **Modifier**: label-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### selected - label-listVertical


**Example #23** | **Variation**: selected | **Modifier**: label-listVertical | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### selected - label-listVertical - disabled


**Example #24** | **Variation**: selected | **Modifier**: label-listVertical | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### deselected - listHorizontal


**Example #25** | **Variation**: deselected | **Modifier**: listHorizontal | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### deselected - listHorizontal - disabled


**Example #26** | **Variation**: deselected | **Modifier**: listHorizontal | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### deselected - listVertical


**Example #27** | **Variation**: deselected | **Modifier**: listVertical | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### deselected - listVertical - disabled


**Example #28** | **Variation**: deselected | **Modifier**: listVertical | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">

       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### deselected - label-listHorizontal


**Example #29** | **Variation**: deselected | **Modifier**: label-listHorizontal | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### deselected - label-listHorizontal - disabled


**Example #30** | **Variation**: deselected | **Modifier**: label-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### deselected - label-listVertical


**Example #31** | **Variation**: deselected | **Modifier**: label-listVertical | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### deselected - label-listVertical - disabled


**Example #32** | **Variation**: deselected | **Modifier**: label-listVertical | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label">Radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Radio List'" [ariaRequired]="false">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = false;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### selected - label-required


**Example #33** | **Variation**: selected | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="true">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = true;
singleSelect:string= "test";
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### selected - label-required - disabled


**Example #34** | **Variation**: selected | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio [(ngModel)]="singleSelect" [value]="singleSelect"  [disabled]="radioDisabled" [required]="true">
    <span *ngIf="showRadioLabel" >Label</span>
    <!-- Here is the label template which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
    <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
    <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = true;
singleSelect:string= "test";
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### selected - label-listHorizontal-required


**Example #35** | **Variation**: selected | **Modifier**: label-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### selected - label-listHorizontal-required - disabled


**Example #36** | **Variation**: selected | **Modifier**: label-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### selected - label-listVertical-required


**Example #37** | **Variation**: selected | **Modifier**: label-listVertical-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### selected - label-listVertical-required - disabled


**Example #38** | **Variation**: selected | **Modifier**: label-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### deselected - label-required


**Example #39** | **Variation**: deselected | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="true">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = false;
showRadioLabel = true;

onRadioStateChange(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### deselected - label-required - disabled


**Example #40** | **Variation**: deselected | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<ap-radio ngModel [disabled]="radioDisabled"  (onChange)="onRadioStateChange($event)" [required]="true">
  <span *ngIf="showRadioLabel" >Label</span>
  <!-- Here is the template for label, which is available from v4.9.2. Please notice that the template below and ng-content above is mutually-exclusive. -->
  <!-- <ng-template ngTemplate="checkedLabel">checked</ng-template>
  <ng-template ngTemplate="uncheckedLabel">unchecked</ng-template>  -->
</ap-radio>
```

#### TypeScript

```typescript
radioDisabled = true;
showRadioLabel = true;

onRadioStateChange(event:any): void {
  console.log(event);
}
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### deselected - label-listHorizontal-required


**Example #41** | **Variation**: deselected | **Modifier**: label-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### deselected - label-listHorizontal-required - disabled


**Example #42** | **Variation**: deselected | **Modifier**: label-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="horizontalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'horizontal-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### deselected - label-listVertical-required


**Example #43** | **Variation**: deselected | **Modifier**: label-listVertical-required | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = false;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### deselected - label-listVertical-required - disabled


**Example #44** | **Variation**: deselected | **Modifier**: label-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { RadioModule } from '@appkit4/angular-components/radio'; 
```

#### HTML Template

```html
<div class="ap-radio-demo-wrapper">
 <label for="verticalRadioList" class="ap-radio-list-label required">Required radio list</label>

 <ap-radio-group [ariaRequired]="radioRequired" [styleClass]="'vertical-radio-list'" [ariaLabel]="'Required Radio List'" [ariaRequired]="true">
     <ng-container *ngFor="let item of list2, let i = index">
       <ap-radio name="city" [value]="item.name"
       [styleClass]="'demo-item'" [(ngModel)]="groupRadioSelect"
       [disabled]="radioDisabled" (onChange)="onSelected($event)">
        <span>{{ item.name }}</span>
       </ap-radio>
     </ng-container>
 </ap-radio-group>
</div>
```

#### TypeScript

```typescript
radioDisabled = true;
radioRequired = true;
groupRadioSelect: string="Hong Kong";

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onSelected(event:any): void {
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

  .ap-radio-demo-wrapper{
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  .ap-radio-list-label{
   font-weight: $font-weight-2;
   line-height: rem(24px);
   letter-spacing: -0.4px;
   margin: 0 0 0 0.25rem;
   color: $color-text-heading;
  }


  .ap-radio-list-label.required {
    display: flex;

    &::after {
     content: '';
     display: block;
     width: rem(4px);
     height: rem(4px);
     background: $color-text-error;
     border-radius: 50%;
     margin-left: $spacing-2;
    }
  }           
```

<!-- /EXAMPLE:44 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-radio-group

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| ariaRequired | boolean | The value of aria-required of radio group. | false | 4.3.0 |
| style | string | The inline style of the component. | '' | 4.3.0 |
| styleClass | string | The style class names of the component. | '' | 4.3.0 |
| ariaLabelledBy | string | The value of aria-labelledby of the radio group. | - | 4.6.0 |
| ariaLabel | string | The value of aria-label of the radio group, but if the ariaLabelLedby has value, this property will be ignored. | - | 4.6.0 |

### ap-radio

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| name | string | The name of radio group. | - | 4.0.0 |
| value | string | The value of the radio. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| inputId | string | Identifier of the focus input to match a label defined for the component. | - | 4.0.0 |
| disabled | boolean | If it is true, it specifies that the radio should be disabled. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.0.0 |
| ngModel | string | Default state of the radio, two-way binding is supported. | - | 4.0.0 |
| onChange | EventerEmitter<any> | Callback to invoke when the radio button receives click. | - | 4.0.0 |
| ariaLabelledBy | string | The value of aria-labelledby of the radio. | - | 4.0.0 |
| readonly | boolean | If set to true, it specifies that the radio cannot be edited. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.0.0 |
| required | boolean | If set to true, it specifies that the radio need to be selected before submittting the form. Please notice that the property will be only applied to styles. If use formControl please use the validation for formControl instead. | false | 4.21.0 |
| tabindex | number | The tabindex of the radio. | 0 | 4.0.0 |
| ariaLabel | string | The value of aria-label of the radio, but if the ariaLabelLedby has value, this property will be ignored. | - | 4.3.0 |
| labelPosition | string: 'before'\|'after' | It specifies the position where label should be placed, before or after the radio. | 'after' | 4.3.0 |
| focus | () => void | Apply focus to the radio. Note that the outline focus indicator only appears when triggered by keyboard. | - | 4.3.0 |


<!-- /SECTION:properties -->