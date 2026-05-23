**Note:** This component documentation is large (121KB). Consider using section parameter (usage, examples, properties) for specific content.

---

---
component: toggle
framework: angular
---

# Toggle Component

## Overview

AppKit Toggle component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Toggle component:

- When the user needs to switch between two states, such as turning a feature on or off.

### Anatomy

1. **Track:** The background of the toggle that shows the available states and the current state of the toggle.

2. **Handle:** The movable part of the toggle that indicates its current state.

3. **Label:** The text that describes the purpose of the toggle component.

4. **Indicator:** Checkmark icon that appears on the on position.

### Variants

#### Inline:

A standalone toggle. It is appropriate to use standalone switches only when their connection to other components is evident, and they provide enough context, such as in application panels.

#### Contained:

An emphasized toggle.

#### With label:

Labels are recommended for toggles at all times. In the absence of a label, toggles become standalone.

#### Disabled:

A disabled toggle component cannot be clicked or interacted with and is usually displayed with a different color to indicate its state.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-130356%26viewport%3D1261%252C-36716%252C0.36%26t%3D21bM3sgWbpviwh7a-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Ensure the toggle component is placed in a location that is easy to find and understand by the user.
- Use clear and concise language in the toggle component's label to convey its purpose and state.
- Toggles should be used whenever the user is presented with the ability to turn options on or off.
- In situations where the context is unambiguous without a text label, standalone switches are appropriate to use.
- When it comes to communicating activation (e.g., on/off states), switches are preferable over checkboxes. On the other hand, checkboxes are more suitable for communicating selection (e.g., multiple table rows). Unlike checkboxes, switches cannot display an error state.
- Switches have a binary state of either being on or off. There are no indeterminate switches in accessibility APIs, and therefore, they cannot be made accessible. If you want to display a partial state, it is recommended to use a checkbox instead of a switch.
- When a toggle is switched between the active or inactive state, the selection is applied immediately.
- Unlike a radio button, a toggle should be used to make a single option active or inactive.

#### How not to use

- Do not use a toggle to switch between two unrelated options.
- Do not use a toggle for a primary action.

### Behavior

- Clicking the Toggle component toggles its state between "on" and "off".
- Hovering over the toggle component shows a selection cursor to indicate toggle interactivity.
- The focus state should be visible when the toggle component is selected using the keyboard or other input devices.
- The toggle component has an animation to indicate its state change.
- Optionally, there's an indicator that appears when the toggle is in the on state.

### Accessibility

- ARIA label should be updated to reflect the "on/off" states in your apps usage.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 72


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | inline | `section: "example:1"` |
| 2 | inline - disabled | `section: "example:2"` |
| 3 | inline - indicator | `section: "example:3"` |
| 4 | inline - indicator - disabled | `section: "example:4"` |
| 5 | inline - label | `section: "example:5"` |
| 6 | inline - label - disabled | `section: "example:6"` |
| 7 | inline - listHorizontal | `section: "example:7"` |
| 8 | inline - listHorizontal - disabled | `section: "example:8"` |
| 9 | inline - listVertical | `section: "example:9"` |
| 10 | inline - listVertical - disabled | `section: "example:10"` |
| 11 | inline - label-indicator | `section: "example:11"` |
| 12 | inline - label-indicator - disabled | `section: "example:12"` |
| 13 | inline - label-listHorizontal | `section: "example:13"` |
| 14 | inline - label-listHorizontal - disabled | `section: "example:14"` |
| 15 | inline - label-listVertical | `section: "example:15"` |
| 16 | inline - label-listVertical - disabled | `section: "example:16"` |
| 17 | inline - label-required | `section: "example:17"` |
| 18 | inline - label-required - disabled | `section: "example:18"` |
| 19 | inline - indicator-listHorizontal | `section: "example:19"` |
| 20 | inline - indicator-listHorizontal - disabled | `section: "example:20"` |
| 21 | inline - indicator-listVertical | `section: "example:21"` |
| 22 | inline - indicator-listVertical - disabled | `section: "example:22"` |
| 23 | inline - label-indicator-listHorizontal | `section: "example:23"` |
| 24 | inline - label-indicator-listHorizontal - disabled | `section: "example:24"` |
| 25 | inline - label-indicator-listVertical | `section: "example:25"` |
| 26 | inline - label-indicator-listVertical - disabled | `section: "example:26"` |
| 27 | inline - label-indicator-required | `section: "example:27"` |
| 28 | inline - label-indicator-required - disabled | `section: "example:28"` |
| 29 | inline - label-listHorizontal-required | `section: "example:29"` |
| 30 | inline - label-listHorizontal-required - disabled | `section: "example:30"` |
| 31 | inline - label-listVertical-required | `section: "example:31"` |
| 32 | inline - label-listVertical-required - disabled | `section: "example:32"` |
| 33 | inline - label-indicator-listHorizontal-required | `section: "example:33"` |
| 34 | inline - label-indicator-listHorizontal-required - disabled | `section: "example:34"` |
| 35 | inline - label-indicator-listVertical-required | `section: "example:35"` |
| 36 | inline - label-indicator-listVertical-required - disabled | `section: "example:36"` |
| 37 | contained - indicator - disabled | `section: "example:37"` |
| 38 | contained - indicator | `section: "example:38"` |
| 39 | contained - disabled | `section: "example:39"` |
| 40 | contained | `section: "example:40"` |
| 41 | contained - label | `section: "example:41"` |
| 42 | contained - label - disabled | `section: "example:42"` |
| 43 | contained - listHorizontal | `section: "example:43"` |
| 44 | contained - listHorizontal - disabled | `section: "example:44"` |
| 45 | contained - listVertical | `section: "example:45"` |
| 46 | contained - listVertical - disabled | `section: "example:46"` |
| 47 | contained - label-indicator | `section: "example:47"` |
| 48 | contained - label-indicator - disabled | `section: "example:48"` |
| 49 | contained - label-listHorizontal | `section: "example:49"` |
| 50 | contained - label-listHorizontal - disabled | `section: "example:50"` |
| 51 | contained - label-listVertical | `section: "example:51"` |
| 52 | contained - label-listVertical - disabled | `section: "example:52"` |
| 53 | contained - label-required | `section: "example:53"` |
| 54 | contained - label-required - disabled | `section: "example:54"` |
| 55 | contained - indicator-listHorizontal | `section: "example:55"` |
| 56 | contained - indicator-listHorizontal - disabled | `section: "example:56"` |
| 57 | contained - indicator-listVertical | `section: "example:57"` |
| 58 | contained - indicator-listVertical - disabled | `section: "example:58"` |
| 59 | contained - label-indicator-listHorizontal | `section: "example:59"` |
| 60 | contained - label-indicator-listHorizontal - disabled | `section: "example:60"` |
| 61 | contained - label-indicator-listVertical | `section: "example:61"` |
| 62 | contained - label-indicator-listVertical - disabled | `section: "example:62"` |
| 63 | contained - label-indicator-required | `section: "example:63"` |
| 64 | contained - label-indicator-required - disabled | `section: "example:64"` |
| 65 | contained - label-listHorizontal-required | `section: "example:65"` |
| 66 | contained - label-listHorizontal-required - disabled | `section: "example:66"` |
| 67 | contained - label-listVertical-required | `section: "example:67"` |
| 68 | contained - label-listVertical-required - disabled | `section: "example:68"` |
| 69 | contained - label-indicator-listHorizontal-required | `section: "example:69"` |
| 70 | contained - label-indicator-listHorizontal-required - disabled | `section: "example:70"` |
| 71 | contained - label-indicator-listVertical-required | `section: "example:71"` |
| 72 | contained - label-indicator-listVertical-required - disabled | `section: "example:72"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### inline


**Example #1** | **Variation**: inline | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = false
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### inline - disabled


**Example #2** | **Variation**: inline | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = false
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### inline - indicator


**Example #3** | **Variation**: inline | **Modifier**: indicator | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = false
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### inline - indicator - disabled


**Example #4** | **Variation**: inline | **Modifier**: indicator | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = false
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### inline - label


**Example #5** | **Variation**: inline | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### inline - label - disabled


**Example #6** | **Variation**: inline | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### inline - listHorizontal


**Example #7** | **Variation**: inline | **Modifier**: listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### inline - listHorizontal - disabled


**Example #8** | **Variation**: inline | **Modifier**: listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### inline - listVertical


**Example #9** | **Variation**: inline | **Modifier**: listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### inline - listVertical - disabled


**Example #10** | **Variation**: inline | **Modifier**: listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### inline - label-indicator


**Example #11** | **Variation**: inline | **Modifier**: label-indicator | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### inline - label-indicator - disabled


**Example #12** | **Variation**: inline | **Modifier**: label-indicator | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### inline - label-listHorizontal


**Example #13** | **Variation**: inline | **Modifier**: label-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### inline - label-listHorizontal - disabled


**Example #14** | **Variation**: inline | **Modifier**: label-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### inline - label-listVertical


**Example #15** | **Variation**: inline | **Modifier**: label-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### inline - label-listVertical - disabled


**Example #16** | **Variation**: inline | **Modifier**: label-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### inline - label-required


**Example #17** | **Variation**: inline | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### inline - label-required - disabled


**Example #18** | **Variation**: inline | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### inline - indicator-listHorizontal


**Example #19** | **Variation**: inline | **Modifier**: indicator-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### inline - indicator-listHorizontal - disabled


**Example #20** | **Variation**: inline | **Modifier**: indicator-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### inline - indicator-listVertical


**Example #21** | **Variation**: inline | **Modifier**: indicator-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### inline - indicator-listVertical - disabled


**Example #22** | **Variation**: inline | **Modifier**: indicator-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### inline - label-indicator-listHorizontal


**Example #23** | **Variation**: inline | **Modifier**: label-indicator-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### inline - label-indicator-listHorizontal - disabled


**Example #24** | **Variation**: inline | **Modifier**: label-indicator-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### inline - label-indicator-listVertical


**Example #25** | **Variation**: inline | **Modifier**: label-indicator-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### inline - label-indicator-listVertical - disabled


**Example #26** | **Variation**: inline | **Modifier**: label-indicator-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### inline - label-indicator-required


**Example #27** | **Variation**: inline | **Modifier**: label-indicator-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### inline - label-indicator-required - disabled


**Example #28** | **Variation**: inline | **Modifier**: label-indicator-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### inline - label-listHorizontal-required


**Example #29** | **Variation**: inline | **Modifier**: label-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### inline - label-listHorizontal-required - disabled


**Example #30** | **Variation**: inline | **Modifier**: label-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### inline - label-listVertical-required


**Example #31** | **Variation**: inline | **Modifier**: label-listVertical-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### inline - label-listVertical-required - disabled


**Example #32** | **Variation**: inline | **Modifier**: label-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### inline - label-indicator-listHorizontal-required


**Example #33** | **Variation**: inline | **Modifier**: label-indicator-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### inline - label-indicator-listHorizontal-required - disabled


**Example #34** | **Variation**: inline | **Modifier**: label-indicator-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### inline - label-indicator-listVertical-required


**Example #35** | **Variation**: inline | **Modifier**: label-indicator-listVertical-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### inline - label-indicator-listVertical-required - disabled


**Example #36** | **Variation**: inline | **Modifier**: label-indicator-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Inline';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### contained - indicator - disabled


**Example #37** | **Variation**: contained | **Modifier**: indicator | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = false;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### contained - indicator


**Example #38** | **Variation**: contained | **Modifier**: indicator | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = false;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### contained - disabled


**Example #39** | **Variation**: contained | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = false;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### contained


**Example #40** | **Variation**: contained | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = false;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### contained - label


**Example #41** | **Variation**: contained | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### contained - label - disabled


**Example #42** | **Variation**: contained | **Modifier**: label | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### contained - listHorizontal


**Example #43** | **Variation**: contained | **Modifier**: listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### contained - listHorizontal - disabled


**Example #44** | **Variation**: contained | **Modifier**: listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:44 -->

<!-- EXAMPLE:45 -->
### contained - listVertical


**Example #45** | **Variation**: contained | **Modifier**: listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:45 -->

<!-- EXAMPLE:46 -->
### contained - listVertical - disabled


**Example #46** | **Variation**: contained | **Modifier**: listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:46 -->

<!-- EXAMPLE:47 -->
### contained - label-indicator


**Example #47** | **Variation**: contained | **Modifier**: label-indicator | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:47 -->

<!-- EXAMPLE:48 -->
### contained - label-indicator - disabled


**Example #48** | **Variation**: contained | **Modifier**: label-indicator | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="false">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:48 -->

<!-- EXAMPLE:49 -->
### contained - label-listHorizontal


**Example #49** | **Variation**: contained | **Modifier**: label-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:49 -->

<!-- EXAMPLE:50 -->
### contained - label-listHorizontal - disabled


**Example #50** | **Variation**: contained | **Modifier**: label-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:50 -->

<!-- EXAMPLE:51 -->
### contained - label-listVertical


**Example #51** | **Variation**: contained | **Modifier**: label-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:51 -->

<!-- EXAMPLE:52 -->
### contained - label-listVertical - disabled


**Example #52** | **Variation**: contained | **Modifier**: label-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:52 -->

<!-- EXAMPLE:53 -->
### contained - label-required


**Example #53** | **Variation**: contained | **Modifier**: label-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:53 -->

<!-- EXAMPLE:54 -->
### contained - label-required - disabled


**Example #54** | **Variation**: contained | **Modifier**: label-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:54 -->

<!-- EXAMPLE:55 -->
### contained - indicator-listHorizontal


**Example #55** | **Variation**: contained | **Modifier**: indicator-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:55 -->

<!-- EXAMPLE:56 -->
### contained - indicator-listHorizontal - disabled


**Example #56** | **Variation**: contained | **Modifier**: indicator-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:56 -->

<!-- EXAMPLE:57 -->
### contained - indicator-listVertical


**Example #57** | **Variation**: contained | **Modifier**: indicator-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:57 -->

<!-- EXAMPLE:58 -->
### contained - indicator-listVertical - disabled


**Example #58** | **Variation**: contained | **Modifier**: indicator-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">

        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:58 -->

<!-- EXAMPLE:59 -->
### contained - label-indicator-listHorizontal


**Example #59** | **Variation**: contained | **Modifier**: label-indicator-listHorizontal | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:59 -->

<!-- EXAMPLE:60 -->
### contained - label-indicator-listHorizontal - disabled


**Example #60** | **Variation**: contained | **Modifier**: label-indicator-listHorizontal | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:60 -->

<!-- EXAMPLE:61 -->
### contained - label-indicator-listVertical


**Example #61** | **Variation**: contained | **Modifier**: label-indicator-listVertical | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:61 -->

<!-- EXAMPLE:62 -->
### contained - label-indicator-listVertical - disabled


**Example #62** | **Variation**: contained | **Modifier**: label-indicator-listVertical | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label">Toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="false" [attr.aria-label]="'Toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = false;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:62 -->

<!-- EXAMPLE:63 -->
### contained - label-indicator-required


**Example #63** | **Variation**: contained | **Modifier**: label-indicator-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = false;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:63 -->

<!-- EXAMPLE:64 -->
### contained - label-indicator-required - disabled


**Example #64** | **Variation**: contained | **Modifier**: label-indicator-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<ap-toggle (onChange)="onToggleChange($event)" [large]="true" [disabled]="switchDisabled" [showIndicator]="true"  [checked]="false" [required]="true">
   <span *ngIf="showSwitchLabel">Label</span>
</ap-toggle>      
```

#### TypeScript

```typescript
switchDisabled = true;
showSwitchLabel = true;
onToggleChange(event: any): void{
  console.log(event);
}
```

<!-- /EXAMPLE:64 -->

<!-- EXAMPLE:65 -->
### contained - label-listHorizontal-required


**Example #65** | **Variation**: contained | **Modifier**: label-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:65 -->

<!-- EXAMPLE:66 -->
### contained - label-listHorizontal-required - disabled


**Example #66** | **Variation**: contained | **Modifier**: label-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:66 -->

<!-- EXAMPLE:67 -->
### contained - label-listVertical-required


**Example #67** | **Variation**: contained | **Modifier**: label-listVertical-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:67 -->

<!-- EXAMPLE:68 -->
### contained - label-listVertical-required - disabled


**Example #68** | **Variation**: contained | **Modifier**: label-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = false;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:68 -->

<!-- EXAMPLE:69 -->
### contained - label-indicator-listHorizontal-required


**Example #69** | **Variation**: contained | **Modifier**: label-indicator-listHorizontal-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:69 -->

<!-- EXAMPLE:70 -->
### contained - label-indicator-listHorizontal-required - disabled


**Example #70** | **Variation**: contained | **Modifier**: label-indicator-listHorizontal-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="horizontalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list horizontal" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.horizontal {
      display: flex;
      flex-direction: row;
      gap: rem(24px);
    }
  }
}
```

<!-- /EXAMPLE:70 -->

<!-- EXAMPLE:71 -->
### contained - label-indicator-listVertical-required


**Example #71** | **Variation**: contained | **Modifier**: label-indicator-listVertical-required | **State**: None

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = false;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:71 -->

<!-- EXAMPLE:72 -->
### contained - label-indicator-listVertical-required - disabled


**Example #72** | **Variation**: contained | **Modifier**: label-indicator-listVertical-required | **State**: disabled

#### Module Import

```typescript
import { ToggleModule} from '@appkit4/angular-components/toggle';
```

#### HTML Template

```html
<div class="ap-switch-demo-wrapper">
 <label for="verticalSwitchList" class="ap-switch-list-label required">Required toggle list</label>

<div class="ap-switch-list vertical" [attr.aria-required]="true" [attr.aria-label]="'Required toggle list'">
      <ng-container *ngFor="let item of list2, let i = index">
        <ap-toggle (onChange)="onToggleChange($event)" [disabled]="switchDisabled" [showIndicator]="showSwitchIndicator"
        [large]="switchVariation === 'Contained'" [checked]="false">
          <span>{{ item.name }}</span>
        </ap-toggle>
      </ng-container>
</div>
</div>
```

#### TypeScript

```typescript
switchDisabled = true;
switchRequired = true;
showSwitchIndicator = true;
switchVariation = 'Contained';

list2 = [
  {name: 'Hong Kong', key: 'A'},
  {name: 'Stockholm', key: 'M'},
  {name: 'São Paulo', key: 'P'}, 
  {name: 'Saint Petersburg', key: 'R'}
];

onToggleChange(event: any): void{
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

  .ap-switch-demo-wrapper{
      display: flex;
      flex-direction: column;
      gap: $spacing-3;

   .ap-switch-list-label{
      font-weight: $font-weight-2;
      line-height: rem(24px);
      letter-spacing: -0.4px;
      color: $color-text-heading;
    }


  .ap-switch-list-label.required {
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

  .ap-switch-list{

    &.vertical {
      display: flex;
      flex-direction: column;
      gap: rem(8px);
     }
  }
}
```

<!-- /EXAMPLE:72 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| disabled | boolean | If it is true, it specifies that the toggle should be disabled. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| inputId | string | Identifier of the focus input to match a label defined for the component. | - | 4.0.0 |
| large | boolean | When specified, displays in the large size, only works for the switch icon. | false | 4.0.0 |
| ngModel | boolean | Default state of the toggle, two-way binding is supported.FormsModule needs to be imported from @angular/forms. | false | 4.0.0 |
| onChange | EventEmitter<any> | Callback to invoke when the state of the toggle changes. | - | 4.0.0 |
| readonly | boolean | If set to true, it specifies that the toggle cannot be edited. | false | 4.0.0 |
| required | boolean | Specifies whether the Toggle is required. | false | 4.21.0 |
| tabindex | number | The tabindex of the toggle. | 0 | 4.0.0 |
| ariaLabel | string | The value of aria-label of the toggle, but if the ariaLabelLedby has value, this property will be ignored. | - | 4.0.0 |
| ariaLabelledBy | string | The value of aria-labelledby of the toggle. | - | 4.0.0 |
| labelPosition | string: 'before'\|'after' | It specifies the position where label should be placed, before or after the toggle. | 'after' | 4.3.0 |
| focus | () => void | Apply focus to the toggle. Note that the outline focus indicator only appears when triggered by keyboard. | - | 4.3.0 |
| toggle | (event: Event) => void | Toggle the selected state of the toggle. | - | 4.3.0 |
| showIndicator | boolean | Trigger to control whether a toggle will show a checkmark indicator or not. | false | 4.5.0 |
| type | string: 'submit', 'button', 'reset' | Type of toggle component. | button | 4.6.1 |


<!-- /SECTION:properties -->