---
component: progress
framework: angular
---

# Progress Component

## Overview

AppKit Progress component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Progress component:

- To visualize the progress of a multi-step process.
- Use a progress stepper for steps that are clearly defined.
- To display the current step and the total number of steps, as well as the status of each step.

### Anatomy

**1. Track:** Represent the total progress.

**2. Active step dot:** Represent the current step.

**3. Step labels:** Provide a description of each step in the process.

### Variants

1. **Track:** Represent the total progress.

2. **Active step dot:** Represent the current step.

3. **Step labels:** Provide a description of each step in the process.

### Variants

#### Progress bar:

Recommended for displaying status of processes with less number of steps

#### Progress stepper:

Used to display a multi-step process. Examples of use cases include checkout flows, onboarding flows, and form submissions.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-105871%26viewport%3D1513%252C-36739%252C0.48%26t%3DDdFeAZ64qClqOPmD-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use short descriptive labels for each step.
- If applicable to the use case, make each step clickable so users can navigate between steps.

#### How not to use

- Do not use a progress stepper as the primary navigation for your application.
- Do not use long labels for steps.
- Do not stack progress steppers or use multiple progress steppers within the same workflow.

### Behavior

- The active step dot should always be visible and clearly differentiated from the other steps.
- The step labels should provide clear and concise descriptions of each step.

### Accessibility

- Ensure each step is labeled descriptively.
- Ensure status is provided for each step.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 19


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | progressBar | `section: "example:1"` |
| 2 | progressBar - vertical | `section: "example:2"` |
| 3 | progressBar - readOnly | `section: "example:3"` |
| 4 | progressBar - vertical-readOnly | `section: "example:4"` |
| 5 | progressStepper | `section: "example:5"` |
| 6 | progressStepper - vertical | `section: "example:6"` |
| 7 | progressStepper - readOnly | `section: "example:7"` |
| 8 | progressStepper - label | `section: "example:8"` |
| 9 | progressStepper - error | `section: "example:9"` |
| 10 | progressStepper - vertical-readOnly | `section: "example:10"` |
| 11 | progressStepper - vertical-label | `section: "example:11"` |
| 12 | progressStepper - vertical-error | `section: "example:12"` |
| 13 | progressStepper - readOnly-label | `section: "example:13"` |
| 14 | progressStepper - readOnly-error | `section: "example:14"` |
| 15 | progressStepper - label-error | `section: "example:15"` |
| 16 | progressStepper - vertical-readOnly-label | `section: "example:16"` |
| 17 | progressStepper - vertical-readOnly-error | `section: "example:17"` |
| 18 | progressStepper - readOnly-label-error | `section: "example:18"` |
| 19 | progressStepper - vertical-readOnly-label-error | `section: "example:19"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### progressBar


**Example #1** | **Variation**: progressBar | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ProgressModule } from '@appkit4/angular-components/progress';
```

#### HTML Template

```html
<ap-progress [readonly]="false" 
  [orientation]="'horizontal'" 
  [space]="120" 
  [(activeIndex)]="activeIndex" 
  [steps]="steps" 
  (activeIndexChange)="onActiveIndexChange($event)">
</ap-progress>
```

#### TypeScript

```typescript
activeIndex = 0;
steps:any[] = [
  { label: "Start", tooltipText: "Start" },
  { label: "Mid", tooltipText: "Mid" },
  { label: "End" }
];

onActiveIndexChange(index: number): void {
  console.log('current active index is ' + index);
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

:host ::ng-deep {
  .ap-progress {
      margin: $spacing-5;
      margin-bottom: rem(64px);
  }
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### progressBar - vertical


**Example #2** | **Variation**: progressBar | **Modifier**: vertical | **State**: None

#### Module Import

```typescript
import { ProgressModule } from '@appkit4/angular-components/progress';
```

#### HTML Template

```html
<ap-progress [readonly]="false" 
  [orientation]="'vertical'" 
  [space]="120" 
  [(activeIndex)]="activeIndex" 
  [steps]="steps" 
  (activeIndexChange)="onActiveIndexChange($event)">
</ap-progress>
```

#### TypeScript

```typescript
activeIndex = 0;
steps:any[] = [
  { label: "Start", tooltipText: "Start" },
  { label: "Mid", tooltipText: "Mid" },
  { label: "End" }
];

onActiveIndexChange(index: number): void {
  console.log('current active index is ' + index);
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

:host ::ng-deep {
  .ap-progress {
      margin: $spacing-5;
      margin-bottom: rem(64px);
  }
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### progressBar - readOnly


**Example #3** | **Variation**: progressBar | **Modifier**: readOnly | **State**: None

#### Module Import

```typescript
import { ProgressModule } from '@appkit4/angular-components/progress';
```

#### HTML Template

```html
<ap-progress [readonly]="true" 
  [orientation]="'horizontal'" 
  [space]="120" 
  [(activeIndex)]="activeIndex" 
  [steps]="steps" 
  (activeIndexChange)="onActiveIndexChange($event)">
</ap-progress>
```

#### TypeScript

```typescript
activeIndex = 0;
steps:any[] = [
  { label: "Start", tooltipText: "Start" },
  { label: "Mid", tooltipText: "Mid" },
  { label: "End" }
];

onActiveIndexChange(index: number): void {
  console.log('current active index is ' + index);
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

:host ::ng-deep {
  .ap-progress {
      margin: $spacing-5;
      margin-bottom: rem(64px);
  }
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### progressBar - vertical-readOnly


**Example #4** | **Variation**: progressBar | **Modifier**: vertical-readOnly | **State**: None

#### Module Import

```typescript
import { ProgressModule } from '@appkit4/angular-components/progress';
```

#### HTML Template

```html
<ap-progress [readonly]="true" 
  [orientation]="'vertical'" 
  [space]="120" 
  [(activeIndex)]="activeIndex" 
  [steps]="steps" 
  (activeIndexChange)="onActiveIndexChange($event)">
</ap-progress>
```

#### TypeScript

```typescript
activeIndex = 0;
steps:any[] = [
  { label: "Start", tooltipText: "Start" },
  { label: "Mid", tooltipText: "Mid" },
  { label: "End" }
];

onActiveIndexChange(index: number): void {
  console.log('current active index is ' + index);
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

:host ::ng-deep {
  .ap-progress {
      margin: $spacing-5;
      margin-bottom: rem(64px);
  }
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### progressStepper


**Example #5** | **Variation**: progressStepper | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = false;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'normal', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### progressStepper - vertical


**Example #6** | **Variation**: progressStepper | **Modifier**: vertical | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = false;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'normal', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### progressStepper - readOnly


**Example #7** | **Variation**: progressStepper | **Modifier**: readOnly | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'normal', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### progressStepper - label


**Example #8** | **Variation**: progressStepper | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = false;
activeIndex = 3;
space = 148;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here'  },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here'  },
  { label: 'Step 3 name displays here', status: 'normal', tooltipText: 'Step 3 name displays here'  },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here'  }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### progressStepper - error


**Example #9** | **Variation**: progressStepper | **Modifier**: error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = false;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'warning', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### progressStepper - vertical-readOnly


**Example #10** | **Variation**: progressStepper | **Modifier**: vertical-readOnly | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'normal', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### progressStepper - vertical-label


**Example #11** | **Variation**: progressStepper | **Modifier**: vertical-label | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = false;
activeIndex = 3;
space = 84;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here'  },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here'  },
  { label: 'Step 3 name displays here', status: 'normal', tooltipText: 'Step 3 name displays here'  },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here'  }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### progressStepper - vertical-error


**Example #12** | **Variation**: progressStepper | **Modifier**: vertical-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = false;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'warning', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### progressStepper - readOnly-label


**Example #13** | **Variation**: progressStepper | **Modifier**: readOnly-label | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = true;
activeIndex = 3;
space = 148;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here'  },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here'  },
  { label: 'Step 3 name displays here', status: 'normal', tooltipText: 'Step 3 name displays here'  },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here'  }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### progressStepper - readOnly-error


**Example #14** | **Variation**: progressStepper | **Modifier**: readOnly-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'warning', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### progressStepper - label-error


**Example #15** | **Variation**: progressStepper | **Modifier**: label-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = false;
activeIndex = 3;
space = 148;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here' },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here' },
  { label: 'Step 3 name displays here', status: 'warning', tooltipText: 'Step 3 name displays here' },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### progressStepper - vertical-readOnly-label


**Example #16** | **Variation**: progressStepper | **Modifier**: vertical-readOnly-label | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here'  },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here'  },
  { label: 'Step 3 name displays here', status: 'normal', tooltipText: 'Step 3 name displays here'  },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here'  }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### progressStepper - vertical-readOnly-error


**Example #17** | **Variation**: progressStepper | **Modifier**: vertical-readOnly-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { status: 'normal', tooltipText: 'Step 1 name displays here' },
  { status: 'normal', tooltipText: 'Step 2 name displays here' },
  { status: 'warning', tooltipText: 'Step 3 name displays here' },
  { status: 'normal' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### progressStepper - readOnly-label-error


**Example #18** | **Variation**: progressStepper | **Modifier**: readOnly-label-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'horizontal';
readonly = true;
activeIndex = 3;
space = 148;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here' },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here' },
  { label: 'Step 3 name displays here', status: 'warning', tooltipText: 'Step 3 name displays here' },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### progressStepper - vertical-readOnly-label-error


**Example #19** | **Variation**: progressStepper | **Modifier**: vertical-readOnly-label-error | **State**: None

#### Module Import

```typescript
import { ProgressStepperModule } from '@appkit4/angular-components/progress-stepper';
```

#### HTML Template

```html
<ap-progress-stepper-set [orientation]="orientation" 
  [(activeIndex)]="activeIndex" 
  [space]="space" 
  [readonly]="readonly"
  (activeIndexChange)="onActiveIndexChange($event)">
    <ap-progress-stepper *ngFor="let step of steps; let i = index" 
      [label]="step.label" 
      [status]="step.status"
      [tooltipText]="step.tooltipText">
      <!-- Set the content of every stepper panel here -->
    </ap-progress-stepper>
</ap-progress-stepper-set> 
```

#### TypeScript

```typescript
  orientation = 'vertical';
readonly = true;
activeIndex = 3;
space = 84;
steps:any[] = [
  { label: 'Step 1 name displays here', status: 'normal', tooltipText: 'Step 1 name displays here' },
  { label: 'Step 2 name displays here', status: 'normal', tooltipText: 'Step 2 name displays here' },
  { label: 'Step 3 name displays here', status: 'warning', tooltipText: 'Step 3 name displays here' },
  { label: 'Step 4 name displays here', status: 'normal', tooltipText: 'Step 4 name displays here' }
];

onActiveIndexChange(index: number) {
  console.log('The index of current selected step is ' + index);
}
```

<!-- /EXAMPLE:19 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-progress

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| space | number | The spacing(px) of each step. if not specified, inherits from the parent element. | 0 | 4.0.0 |
| activeIndex | number | Index of the active item. | 0 | 4.0.0 |
| steps | array: \[{label:"start", tooltipText:"start tooltip"}\] | List of the progress items. The "tooltipText" property is available from v4.9.1. | \[\] | 4.0.0 |
| orientation | string: 'horizontal' \| 'vertical' | The orientation of the step bar, horizontal or vertical. | 'horizontal' | 4.0.0 |
| readonly | boolean | Whether the items are clickable or not. | true | 4.0.0 |
| activeIndexChange | EventEmitter<number> | Callback to invoke when the new step is selected. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| trigger | string: 'hover'\|'click' | The trigger of Tooltip. Set trigger to "" not to display the tooltip. | 'hover' | 4.9.1 |

### ap-progress-stepper-set

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| space | number | The spacing(px) of each step. | 84 | 4.0.0 |
| activeIndex | number | Index of the current active item. | 0 | 4.0.0 |
| orientation | string: 'horizontal' \| 'vertical' | To specify the orientation of the stepper, horizontal or vertical. | 'horizontal' | 4.0.0 |
| readonly | boolean | Whether the items are clickable or not. | false | 4.0.0 |
| distance | number | The distance between stepper button and its tooltip content, the unit is px. | 4 | 4.0.0 |
| activeIndexChange | EventEmitter<number> | Callback to invoke when the new step is selected. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |

### ap-progress-stepper

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| label | string | The label name of the stepper. | '' | 4.0.0 |
| status | string: 'normal'\|'warning' | The status of the stepper. | 'normal' | 4.0.0 |
| trigger | string: 'hover'\|'click' | The trigger of Tooltip. Set trigger to "" not to display the tooltip. | 'hover' | 4.0.0 |
| hideTooltipOnBlur | boolean | If true, hide the tooltip after the stepper button loses focus. | true | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| tooltipText | string | The tooltipContent of the stepper. | '' | 4.9.1 |


<!-- /SECTION:properties -->