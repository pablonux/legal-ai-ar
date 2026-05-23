---
component: slider
framework: angular
---

# Slider Component

## Overview

AppKit Slider component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Sidebar component:

- When users need to select a value within a specific range.
- When the range of values is large and requires a more precise selection.
- When users need to visually compare values or see the relationship between values.

When not to use:

- When users need to input an exact value.
- When the range of values is small and a traditional input control would suffice.
- When the slider would add unnecessary complexity to the user interface.

### Anatomy

1. **Container:** The main container for the slider.

2. **Description:** Text that describes the purpose of the slider.

3. **Track:** The track of the slider.

4. **Handle:** The handle of the slider.

5. **Value:** Value indicators.

6. **Input:** Numeric input field option.

### Variants

#### Single Slider:

A single slider allows the user to select a single value from a continuous range of values. This type of slider is often used for simple scenarios where a user needs to select a single value, such as choosing a temperature or a volume level.

#### Range Slider:

A range slider allows the user to select a range of values within a continuous range. This type of slider is often used when users need to specify a range, such as selecting a price range or a date range.

#### Single Slider with Input:

A single slider with an input field allows the user to select a single value by either dragging the slider or entering the value directly into the input field. This type of slider is useful when users need to specify an exact value, such as selecting a specific percentage or dollar amount.

#### Range Slider with Input:

A range slider with input fields allows the user to select a range of values by either dragging the slider handles or entering the values directly into the input fields. This type of slider is useful when users need to specify an exact range of values, such as selecting a specific time frame.

#### Intervals Slider:

An intervals slider allows the user to select discrete values within a range of values. This type of slider is often used when users need to select specific values from a predefined list, such as selecting a font size or a color from a palette. Intervals sliders often have a set of predefined steps that the user can choose from.

#### Color range:

Allows the user to select a range of colors using a slider control that displays the selected color range.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-120681%26viewport%3D1266%252C-34643%252C0.36%26t%3Dulv08TFhaTlC3XvD-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use appropriate values for min, max, and step to ensure the slider is easy to use.
- Consider the placement and size of the slider to ensure it is easily discoverable and usable.
- Sliders can be used to allow the user to select a value.
- They are best used when precision is not of importance as they can be difficult for users.

#### How not to use

- Do not use sliders for making very precise selections.

### Behavior

- The handle moves smoothly along the track when dragged and snaps to tick marks.
- The value of the slider should update in real-time as the handle is moved.

### Accessibility

- Use aria-label to identify component.
- Slider label should be clear and concise.
- Use slider with numeric input for optimal screen reader usability.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 6


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | single | `section: "example:1"` |
| 2 | range | `section: "example:2"` |
| 3 | singleInput | `section: "example:3"` |
| 4 | rangeInput | `section: "example:4"` |
| 5 | intervals | `section: "example:5"` |
| 6 | colors | `section: "example:6"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### single


**Example #1** | **Variation**: single | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
```

#### HTML Template

```html
<div class="ap-slider-wrapper">
  <div class="ap-slider-header">
    <label id="singleSlider" class="ap-slider-label">Single slider</label>
    <div class="ap-slider-text">
      {{value | number}}
    </div>
  </div>
  <ap-slider [(ngModel)]="value" [sliderId]="'singleSlider'" [max]="max" [step]="step"></ap-slider>
</div>
```

#### TypeScript

```typescript
value = 50;
max = 256;
step = 10;
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);
  }
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### range


**Example #2** | **Variation**: range | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
```

#### HTML Template

```html
<div class="ap-slider-wrapper">
  <div class="ap-slider-header">
    <label id="rangeSlider" class="ap-slider-label">Range slider</label>
    <div class="ap-slider-text">
      {{value[0]}} to {{value[1]}}
    </div>
  </div>
  <ap-slider [(ngModel)]="value" [sliderId]="'rangeSlider'" [max]="max" [step]="step" [range]="true"></ap-slider>
</div>
```

#### TypeScript

```typescript
value: number[] = [256, 512];
max = 768;
step= 2;
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);
  }
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### singleInput


**Example #3** | **Variation**: singleInput | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<div class="ap-slider-wrapper with-input">
  <div class="ap-slider-label-wrapper">
    <label id="singleInputSlider" class="ap-slider-label">Single input slider</label>
  </div>
  <ap-slider [(ngModel)]="val3" [sliderId]="'singleInputSlider'" [min]="minVal3" [max]="maxVal3" [step]="step3" [showRange]="true"></ap-slider>
  <div class="ap-slider-header">
    <div class="ap-slider-text">
      <ap-field [hideTitleOnInput]="true">
        <input aria-label="singleInputSlider" appkit-field [(ngModel)]="val3" (blur)="onChangeVal3($event)" />
      </ap-field>
    </div>
  </div>
</div>
```

#### TypeScript

```typescript
val3: number = 2;
maxVal3 = 20;
minVal3 = 2;
step3 = 2;

onChangeVal3(e: any) {
  let value = e.target.value;
  value = this.ensureNumberInRange(value, this.minVal3, this.maxVal3, this.step3);
  this.val3 = value;
}

ensureNumberInRange(num: number, min: number, max: number, step: number): number {
  const digits = this.countDecimals(step);
  if (digits) {
    if (((num - min) * 10 * digits) % (step * 10 * digits)) {
      num = Number((Math.round(((num - min) * 10 * digits) / (step * 10 * digits)) * step + min).toFixed(digits));
    }
  } else {
    if ((num - min) % step) {
      num = Math.round((num - min) / step) * step + min;
    }
  }
  if (isNaN(num) || num < min) {
    return min;
  } else if (num > max) {
    return max;
  } else {
    return num;
  }
}

countDecimals(num: any) {
  if(Math.floor(num) === num) return 0;
  return num.toString().split(".")[1].length || 0; 
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);

    &.with-input {
      position: relative;

      .ap-slider-label-wrapper {
        height: 32px;
        line-height: 32px;
      }

      .ap-slider-header {
        position: absolute;
        right: 12px;
        top: 12px;
      }
    }
  }
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### rangeInput


**Example #4** | **Variation**: rangeInput | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<div class="ap-slider-wrapper with-input">
  <div class="ap-slider-label-wrapper">
    <label id="rangeInputSlider" class="ap-slider-label">Range input slider</label>
  </div>
  <ap-slider [(ngModel)]="rangeValues4" [sliderId]="'rangeInputSlider'" [min]="minVal4" [max]="maxVal4" [step]="step4" [range]="true" [showRange]="true"></ap-slider>
  <div class="ap-slider-header">
    <div class="ap-slider-text">
      <ap-field [hideTitleOnInput]="true">
        <input aria-label="minimum of rangeInputSlider" (input)="onStartChange($event)" (blur)="onBlur()" appkit-field [(ngModel)]="rangeValues4[0]" />
      </ap-field>
      <span class="ap-slider-input-text">to</span>
      <ap-field [hideTitleOnInput]="true">
        <input aria-label="maximum of rangeInputSlider" (input)="onEndChange($event)" (blur)="onBlur()" appkit-field [(ngModel)]="rangeValues4[1]" />
      </ap-field>
    </div>
  </div>
</div>
```

#### TypeScript

```typescript
rangeValues4: number[] = [256, 512];
minVal4 = 0;
maxVal4 = 768;
step4 = 2;

onStartChange(e: any) {
  let value = e.target.value;
  let rangeValues4 = [];
  rangeValues4 = [value, this.rangeValues4[1]];
  this.rangeValues4 = rangeValues4;
}

onEndChange(e: any) {
  let value = e.target.value;
  let rangeValues4 = [];
  rangeValues4 = [this.rangeValues4[0], value];
  this.rangeValues4 = rangeValues4;
}


onBlur() {
  let rangeValues4 = [...this.rangeValues4].map(val => 
    this.ensureNumberInRange(val, this.minVal4, this.maxVal4, this.step4)).sort((a, b) => a - b);
  this.rangeValues4 = rangeValues4;
}

ensureNumberInRange(num: number, min: number, max: number, step: number): number {
  const digits = this.countDecimals(step);
  if (digits) {
    if (((num - min) * 10 * digits) % (step * 10 * digits)) {
      num = Number((Math.round(((num - min) * 10 * digits) / (step * 10 * digits)) * step + min).toFixed(digits));
    }
  } else {
    if ((num - min) % step) {
      num = Math.round((num - min) / step) * step + min;
    }
  }
  if (isNaN(num) || num < min) {
    return min;
  } else if (num > max) {
    return max;
  } else {
    return num;
  }
}

countDecimals(num: any) {
  if(Math.floor(num) === num) return 0;
  return num.toString().split(".")[1].length || 0; 
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);

    &.with-input {
      position: relative;

      .ap-slider-label-wrapper {
        height: 32px;
        line-height: 32px;
      }

      .ap-slider-header {
        position: absolute;
        right: 12px;
        top: 12px;
      }
    }
  }
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### intervals


**Example #5** | **Variation**: intervals | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
```

#### HTML Template

```html
<div class="slider-demo-wrapper">
  <div class="ap-slider-wrapper">
    <div class="ap-slider-header">
      <div class="mr-auto ap-label-wrapper">
        <label id="intervalsSlider" class="ap-slider-label">Intervals slider</label>
      </div>
    </div>
    <ap-slider [(ngModel)]="value" [sliderId]="'intervalsSlider'" [min]="min" [max]="max" [step]="step" [hasInterval]="true"></ap-slider>
  </div>
</div>
```

#### TypeScript

```typescript
value = 256;
min = 0;
max = 768;
step = 256;
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);
  }
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### colors


**Example #6** | **Variation**: colors | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SliderModule } from '@appkit4/angular-components/slider';
```

#### HTML Template

```html
<div class="ap-slider-wrapper">
  <div class="ap-slider-header">
    <label id="colorsSlider" class="ap-slider-label">Colors slider</label>
    <div class="ap-slider-text">
      {{value[0] | number}} to {{value[1] | number}}
    </div>
  </div>
  <ap-slider class="gradient" [(ngModel)]="value" [sliderId]="'colorsSlider'" [max]="max" [step]="step" [range]="true"></ap-slider>
</div>
```

#### TypeScript

```typescript
value: number[] = [205000, 500000];
max = 800000;
step = 100;
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

:host ::ng-deep {
  .ap-slider-wrapper {
    width: rem(336px);
  }

  .gradient .ap-slider-handle-first {
    background: #30d158;
  }

  .gradient .ap-slider-handle-last {
    background: #ff2d55;
  }

  .gradient .ap-slider-track {
    background-image: linear-gradient(to right, #30d158 0%, #ff2d55 100%);
  }
}
```

<!-- /EXAMPLE:6 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| sliderId | string | The unique id of the slider. | - | 4.0.0 |
| disabled | boolean | If it is true, it specifies the slider will be disabled. | false | 4.0.0 |
| min | number | The minimum value the slider can slide to. | 0 | 4.0.0 |
| max | number | The maximum value the slider can slide to. | 100 | 4.0.0 |
| step | number | The granularity the slider can step through values. It should be greater than 0. | 0 | 4.0.0 |
| range | boolean | Dual thumb mode. | false | 4.0.0 |
| showRange | boolean | Display min value and max value of range. | false | 4.4.0 |
| hasInterval | boolean | Whether the slider has intervals. | false | 4.0.0 |
| ngModel | number \| number\[\] | The value of slider. Use number if 'range' is false. Otherwise, use \[number, number\] format.FormsModule needs to be imported from @angular/forms. | - | 4.0.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| onChange | EventEmitter<number\[\] \| number> | Callback to invoke when the value of the slider changes. | - | 4.0.0 |
| onSlideEnd | EventEmitter<number\[\] \| number> | Callback to invoke when onmouseup is fired. | - | 4.0.0 |


<!-- /SECTION:properties -->