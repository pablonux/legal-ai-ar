**Note:** This component documentation is large (212KB). Consider using section parameter (usage, examples, properties) for specific content.

---

---
component: field
framework: angular
---

# Field Component

## Overview

AppKit Field component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use input fields:

- When you want to collect information from the user, such as text, numbers, or dates.
- When the user needs to fill or edit information in a form.
- To allow users to search for specific information.

### Anatomy

**1. Placeholder:** Text that appears in the input field as a hint or suggestion for what information to enter in the field. 

**2. Label:** Text description that describes the purpose of the input field.

**3. Input:** The area where the user can enter or edit information.

**4. Icon:** An optional icon that can be used to provide additional context for the input field.

**5. Helper text:** Displays helper text (Optional)

**6. Input container:** Delimits the input field area.

**7. Required indicator:** Indicates required fields.

### Variants

#### Default:

A simple text input field.

#### Date Picker:

An input field that allows the user to select a date from a calendar.

#### Dropdown:

An input field that allows the user to select an option from a dropdown menu.

#### Password:

An input field for collecting sensitive information from the user, such as passwords.

#### Phone Number:

An input field for collecting phone numbers, typically with the format of (XXX) XXX-XXXX

#### Prefix and Suffix:

An input field that can have a prefix or suffix text, such as a currency symbol or unit of measurement.

#### Search:

An input field that allows the user to search for specific information.

#### Text Area:

An input field that allows the user to enter multiple lines of text.

#### Tooltip:

An input field with a tooltip that can appear when the user interacts with it.

#### Number Input:

An input field for collecting numerical information from the user.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-89406%26viewport%3D723%252C-9708%252C0.22%26t%3DqmmWWB9ShR2LLFKY-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Input fields should support the type and amount of data a user is expected to enter into it. For example, if the data is to be currency, then limit accepted values to only numerical values and consider auto formatting to maintain consistency.
- Use short but descriptive hint text to help users understand what data format or type is expected to be entered.
- Use placeholders as a secondary label, but do not rely on placeholders as the primary way to label the field.
- The text field should have enough space around it for easy tapping or clicking. Appkit recommends spacings of 12-16px between input fields of the same group and use a minimum of 8 pixels of margin or padding between input fields.
- Appkit recommends using a consistent vertical rhythm for the spacing and alignment of input fields.
- Place input fields in a logical order: The order of input fields should be based on how the user will think about the information. For example, if the form is for a shipping address, the input fields for the name, address, city, state, and zip code should be grouped together and in that order.
- Group related input fields together: Group related input fields together to make it clear to the user what information is being gathered.

#### How not to use

- Do not overload the view with too many fields. Too many fields at once will overwhelm the user.
- Do not use a single line field if the expected amount of text will exceed the provided text area.

### Behavior

Specs of interactions and responsive behavior both embedded in components and these needed to be defined by the designer ? Figma interactions / how to use (tutorial video coming soon)

### Accessibility

- The text field should be labeled properly for screen readers.
- The text field should have a clear focus indicator when selected.
- The text field should be clearly distinguishable from the surrounding content.
- Label and format should explain how information should be entered into form field. Ex: First, Middle, Last Name; Birthdate (MM/DD/YYYY).
- Provide an error message and how to fix it and when the user enters wrong info.
- Any required fields should be labeled as such.
- Provide a legend before each group of input fields indicating that the required field indicator () is a required field. Ex: " required".

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 100


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | default - default | `section: "example:1"` |
| 2 | default - disabled | `section: "example:2"` |
| 3 | default - error | `section: "example:3"` |
| 4 | default - readOnly | `section: "example:4"` |
| 5 | default - required - default | `section: "example:5"` |
| 6 | default - suggestion - default | `section: "example:6"` |
| 7 | default - required-suggestion - default | `section: "example:7"` |
| 8 | default - required - disabled | `section: "example:8"` |
| 9 | default - suggestion - disabled | `section: "example:9"` |
| 10 | default - required-suggestion - disabled | `section: "example:10"` |
| 11 | default - required - error | `section: "example:11"` |
| 12 | default - suggestion - error | `section: "example:12"` |
| 13 | default - required-suggestion - error | `section: "example:13"` |
| 14 | default - required - readOnly | `section: "example:14"` |
| 15 | default - suggestion - readOnly | `section: "example:15"` |
| 16 | default - required-suggestion - readOnly | `section: "example:16"` |
| 17 | clearText - default | `section: "example:17"` |
| 18 | clearText - disabled | `section: "example:18"` |
| 19 | clearText - error | `section: "example:19"` |
| 20 | clearText - readOnly | `section: "example:20"` |
| 21 | clearText - required - default | `section: "example:21"` |
| 22 | clearText - suggestion - default | `section: "example:22"` |
| 23 | clearText - required-suggestion - default | `section: "example:23"` |
| 24 | clearText - required - disabled | `section: "example:24"` |
| 25 | clearText - suggestion - disabled | `section: "example:25"` |
| 26 | clearText - required-suggestion - disabled | `section: "example:26"` |
| 27 | clearText - required - error | `section: "example:27"` |
| 28 | clearText - suggestion - error | `section: "example:28"` |
| 29 | clearText - required-suggestion - error | `section: "example:29"` |
| 30 | clearText - required - readOnly | `section: "example:30"` |
| 31 | clearText - suggestion - readOnly | `section: "example:31"` |
| 32 | clearText - required-suggestion - readOnly | `section: "example:32"` |
| 33 | datePicker - default | `section: "example:33"` |
| 34 | datePicker - disabled | `section: "example:34"` |
| 35 | datePicker - readOnly | `section: "example:35"` |
| 36 | datePicker - required - default | `section: "example:36"` |
| 37 | datePicker - required - disabled | `section: "example:37"` |
| 38 | datePicker - required - readOnly | `section: "example:38"` |
| 39 | dropdown - default | `section: "example:39"` |
| 40 | dropdown - disabled | `section: "example:40"` |
| 41 | dropdown - required - default | `section: "example:41"` |
| 42 | dropdown - required - disabled | `section: "example:42"` |
| 43 | passwordCreate - default | `section: "example:43"` |
| 44 | passwordCreate - disabled | `section: "example:44"` |
| 45 | passwordCreate - readOnly | `section: "example:45"` |
| 46 | passwordCreate - required - default | `section: "example:46"` |
| 47 | passwordCreate - required - disabled | `section: "example:47"` |
| 48 | passwordCreate - required - readOnly | `section: "example:48"` |
| 49 | passwordLogin - default | `section: "example:49"` |
| 50 | passwordLogin - disabled | `section: "example:50"` |
| 51 | passwordLogin - readOnly | `section: "example:51"` |
| 52 | passwordLogin - required - default | `section: "example:52"` |
| 53 | passwordLogin - required - disabled | `section: "example:53"` |
| 54 | passwordLogin - required - readOnly | `section: "example:54"` |
| 55 | phoneNumber - default | `section: "example:55"` |
| 56 | phoneNumber - disabled | `section: "example:56"` |
| 57 | phoneNumber - readOnly | `section: "example:57"` |
| 58 | phoneNumber - required - default | `section: "example:58"` |
| 59 | phoneNumber - required - disabled | `section: "example:59"` |
| 60 | phoneNumber - required - readOnly | `section: "example:60"` |
| 61 | prefixAndSuffix - default | `section: "example:61"` |
| 62 | prefixAndSuffix - disabled | `section: "example:62"` |
| 63 | prefixAndSuffix - readOnly | `section: "example:63"` |
| 64 | prefixAndSuffix - required - default | `section: "example:64"` |
| 65 | prefixAndSuffix - required - disabled | `section: "example:65"` |
| 66 | prefixAndSuffix - required - readOnly | `section: "example:66"` |
| 67 | search - default | `section: "example:67"` |
| 68 | search - disabled | `section: "example:68"` |
| 69 | textArea - default | `section: "example:69"` |
| 70 | textArea - disabled | `section: "example:70"` |
| 71 | textArea - readOnly | `section: "example:71"` |
| 72 | textArea - error | `section: "example:72"` |
| 73 | textArea - required - default | `section: "example:73"` |
| 74 | textArea - required - disabled | `section: "example:74"` |
| 75 | textArea - required - readOnly | `section: "example:75"` |
| 76 | textArea - required - error | `section: "example:76"` |
| 77 | tooltip - default | `section: "example:77"` |
| 78 | tooltip - disabled | `section: "example:78"` |
| 79 | tooltip - error | `section: "example:79"` |
| 80 | tooltip - readOnly | `section: "example:80"` |
| 81 | tooltip - required - default | `section: "example:81"` |
| 82 | tooltip - suggestion - default | `section: "example:82"` |
| 83 | tooltip - required-suggestion - default | `section: "example:83"` |
| 84 | tooltip - required - disabled | `section: "example:84"` |
| 85 | tooltip - suggestion - disabled | `section: "example:85"` |
| 86 | tooltip - required-suggestion - disabled | `section: "example:86"` |
| 87 | tooltip - required - error | `section: "example:87"` |
| 88 | tooltip - suggestion - error | `section: "example:88"` |
| 89 | tooltip - required-suggestion - error | `section: "example:89"` |
| 90 | tooltip - required - readOnly | `section: "example:90"` |
| 91 | tooltip - suggestion - readOnly | `section: "example:91"` |
| 92 | tooltip - required-suggestion - readOnly | `section: "example:92"` |
| 93 | number - default | `section: "example:93"` |
| 94 | number - disabled | `section: "example:94"` |
| 95 | number - error | `section: "example:95"` |
| 96 | number - readOnly | `section: "example:96"` |
| 97 | number - required - default | `section: "example:97"` |
| 98 | number - required - disabled | `section: "example:98"` |
| 99 | number - required - error | `section: "example:99"` |
| 100 | number - required - readOnly | `section: "example:100"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### default - default


**Example #1** | **Variation**: default | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### default - disabled


**Example #2** | **Variation**: default | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### default - error


**Example #3** | **Variation**: default | **Modifier**: None | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <div role="alert">
    <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
  </div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### default - readOnly


**Example #4** | **Variation**: default | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### default - required - default


**Example #5** | **Variation**: default | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### default - suggestion - default


**Example #6** | **Variation**: default | **Modifier**: suggestion | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### default - required-suggestion - default


**Example #7** | **Variation**: default | **Modifier**: required-suggestion | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### default - required - disabled


**Example #8** | **Variation**: default | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### default - suggestion - disabled


**Example #9** | **Variation**: default | **Modifier**: suggestion | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### default - required-suggestion - disabled


**Example #10** | **Variation**: default | **Modifier**: required-suggestion | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### default - required - error


**Example #11** | **Variation**: default | **Modifier**: required | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <div role="alert">
    <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
  </div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### default - suggestion - error


**Example #12** | **Variation**: default | **Modifier**: suggestion | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
 <div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
  <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
</div> 

</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
  this.emailError = !!this.emailDemo && this.emailDemo.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }

    &:has(+ .ap-field-email-validation-error) {
      margin-bottom: $spacing-3;
    }
  }
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### default - required-suggestion - error


**Example #13** | **Variation**: default | **Modifier**: required-suggestion | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
 <div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
  <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
</div> 

</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
  this.emailError = !!this.emailDemo && this.emailDemo.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }

    &:has(+ .ap-field-email-validation-error) {
      margin-bottom: $spacing-3;
    }
  }
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### default - required - readOnly


**Example #14** | **Variation**: default | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### default - suggestion - readOnly


**Example #15** | **Variation**: default | **Modifier**: suggestion | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### default - required-suggestion - readOnly


**Example #16** | **Variation**: default | **Modifier**: required-suggestion | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### clearText - default


**Example #17** | **Variation**: clearText | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### clearText - disabled


**Example #18** | **Variation**: clearText | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### clearText - error


**Example #19** | **Variation**: clearText | **Modifier**: None | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
  <div role="alert">
    <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
  </div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### clearText - readOnly


**Example #20** | **Variation**: clearText | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### clearText - required - default


**Example #21** | **Variation**: clearText | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### clearText - suggestion - default


**Example #22** | **Variation**: clearText | **Modifier**: suggestion | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### clearText - required-suggestion - default


**Example #23** | **Variation**: clearText | **Modifier**: required-suggestion | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### clearText - required - disabled


**Example #24** | **Variation**: clearText | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### clearText - suggestion - disabled


**Example #25** | **Variation**: clearText | **Modifier**: suggestion | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### clearText - required-suggestion - disabled


**Example #26** | **Variation**: clearText | **Modifier**: required-suggestion | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### clearText - required - error


**Example #27** | **Variation**: clearText | **Modifier**: required | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
  <div role="alert">
    <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
  </div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### clearText - suggestion - error


**Example #28** | **Variation**: clearText | **Modifier**: suggestion | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
 <div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
  <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
</div> 

</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
  this.emailError = !!this.emailDemo && this.emailDemo.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }

    &:has(+ .ap-field-email-validation-error) {
      margin-bottom: $spacing-3;
    }
  }
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### clearText - required-suggestion - error


**Example #29** | **Variation**: clearText | **Modifier**: required-suggestion | **State**: error

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [error]="emailError">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-errormessage="errormessage" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
 <div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span (click)="fillEmail()" #email>&#64;pwc.com</span>?
  </div> 
  <div *ngIf="emailError" id="errormessage" class="ap-field-email-validation-error">Please enter a valid email address</div>
</div> 

</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
emailError: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
  this.emailError = value && value.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
  this.emailError = !!this.emailDemo && this.emailDemo.match(/^[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_-]+(\.com)+$/) === null;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }

    &:has(+ .ap-field-email-validation-error) {
      margin-bottom: $spacing-3;
    }
  }
  .ap-field-email-validation-error {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-error;
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### clearText - required - readOnly


**Example #30** | **Variation**: clearText | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### clearText - suggestion - readOnly


**Example #31** | **Variation**: clearText | **Modifier**: suggestion | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### clearText - required-suggestion - readOnly


**Example #32** | **Variation**: clearText | **Modifier**: required-suggestion | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
  <ap-field [title]="'Email address'" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="emailDemo" (change)="onEmailChange($event)" aria-label="Email address" />
  <ap-field-cleaner [(ngModel)]="emailDemo"></ap-field-cleaner>
<div role="alert">
  <div *ngIf="emailSuggestion" class="ap-field-email-validation-suggestion">
    Did you mean <span>&#64;pwc.com</span>?
  </div> 
</div>
</ap-field>
```

#### TypeScript

```typescript
emailDemo: string = 'ernestolaborda@pwc.';
emailSuggestion: boolean = true;
onEmailChange(event: any) {
  let value = event.target.value;
  this.emailSuggestion = value && !value.match(/@pwc\.com$/);
}
fillEmail() {
  this.emailDemo = this.emailDemo.replace(/^([\s\S]+)@[\s\S]*/, '$1@pwc.com');
  this.emailSuggestion = false;
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

::ng-deep {
  .ap-field-email-validation-suggestion {
    margin: $spacing-3 0 0 $spacing-3;
    height: 12px;
    line-height: 12px;
    font-size: 12px;
    color: $color-text-body;

    span {
      color: $color-text-primary;
      font-weight: $font-weight-2;
      cursor: pointer;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### datePicker - default


**Example #33** | **Variation**: datePicker | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### datePicker - disabled


**Example #34** | **Variation**: datePicker | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth" [disabled]="true">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### datePicker - readOnly


**Example #35** | **Variation**: datePicker | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth" [readonly]="true">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### datePicker - required - default


**Example #36** | **Variation**: datePicker | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth" [required]="true">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### datePicker - required - disabled


**Example #37** | **Variation**: datePicker | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth" [required]="true" [disabled]="true">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### datePicker - required - readOnly


**Example #38** | **Variation**: datePicker | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpRange]="false"
  [(dpSelectedDates)]="dateBirth" [required]="true" [readonly]="true">
  <input appkit-field aria-label="Date of birth"/>
</ap-field>
```

#### TypeScript

```typescript
dateBirth: Date[] = [];
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### dropdown - default


**Example #39** | **Variation**: dropdown | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]="dropdownGender" [selectType]="'single'" [title]="'Gender'" [(ngModel)]="selectedItem" [enableNgContent]="false"
  (onSelect)="onSelectDropdown($event)">
</ap-dropdown>
```

#### TypeScript

```typescript
dropdownGender: any[] = [
  { label: 'Male', value: 'Male' },
  { label: 'Female', value: 'Female' },
  { label: 'Other', value: 'Other' }
];
selectedItem = {};
onSelectDropdown(event: any) {
  console.log(event);
}
onSelectItem(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### dropdown - disabled


**Example #40** | **Variation**: dropdown | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]="dropdownGender" [selectType]="'single'" [title]="'Gender'" [(ngModel)]="selectedItem" [enableNgContent]="false"
  (onSelect)="onSelectDropdown($event)" [disabled]="true">
</ap-dropdown>
```

#### TypeScript

```typescript
dropdownGender: any[] = [
  { label: 'Male', value: 'Male' },
  { label: 'Female', value: 'Female' },
  { label: 'Other', value: 'Other' }
];
selectedItem = {};
onSelectDropdown(event: any) {
  console.log(event);
}
onSelectItem(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### dropdown - required - default


**Example #41** | **Variation**: dropdown | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]="dropdownGender" [selectType]="'single'" [title]="'Gender'" [(ngModel)]="selectedItem" [enableNgContent]="false"
  (onSelect)="onSelectDropdown($event)" [required]="true">
</ap-dropdown>
```

#### TypeScript

```typescript
dropdownGender: any[] = [
  { label: 'Male', value: 'Male' },
  { label: 'Female', value: 'Female' },
  { label: 'Other', value: 'Other' }
];
selectedItem = {};
onSelectDropdown(event: any) {
  console.log(event);
}
onSelectItem(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### dropdown - required - disabled


**Example #42** | **Variation**: dropdown | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { DropdownModule } from '@appkit4/angular-components/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
```

#### HTML Template

```html
<ap-dropdown [list]="dropdownGender" [selectType]="'single'" [title]="'Gender'" [(ngModel)]="selectedItem" [enableNgContent]="false"
  (onSelect)="onSelectDropdown($event)" [required]="true" [disabled]="true">
</ap-dropdown>
```

#### TypeScript

```typescript
dropdownGender: any[] = [
  { label: 'Male', value: 'Male' },
  { label: 'Female', value: 'Female' },
  { label: 'Other', value: 'Other' }
];
selectedItem = {};
onSelectDropdown(event: any) {
  console.log(event);
}
onSelectItem(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### passwordCreate - default


**Example #43** | **Variation**: passwordCreate | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### passwordCreate - disabled


**Example #44** | **Variation**: passwordCreate | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'" [disabled]="true">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:44 -->

<!-- EXAMPLE:45 -->
### passwordCreate - readOnly


**Example #45** | **Variation**: passwordCreate | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'" [readonly]="true">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:45 -->

<!-- EXAMPLE:46 -->
### passwordCreate - required - default


**Example #46** | **Variation**: passwordCreate | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'" [required]="true">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:46 -->

<!-- EXAMPLE:47 -->
### passwordCreate - required - disabled


**Example #47** | **Variation**: passwordCreate | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:47 -->

<!-- EXAMPLE:48 -->
### passwordCreate - required - readOnly


**Example #48** | **Variation**: passwordCreate | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-password" [title]="'Password'" [type]="'password'" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="passwordCreateDemo" aria-label="Password"/>
  <div class="ap-field-password-creator" aria-hidden="true">
    <span [class.highlight]="passwordConditionLetter">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Letter</span>
    </span>
    <span [class.highlight]="passwordConditionNumber">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Number</span>
    </span>
    <span [class.highlight]="passwordConditionSpecChar">
      <span class="Appkit4-icon icon-circle-checkmark-fill"></span>
      <span class="ap-field-password-condition">Special character</span>
    </span>
  </div>
  <div class="ap-field-password-condition-sr-only" aria-live="polite">
    <span>{{passwordConditionLetter?'Contains letter':'Not contains letter'}}</span>
    <span>{{passwordConditionNumber?'Contains number':'Not contains number'}}</span>
    <span>{{passwordConditionSpecChar?'Contains special character':'Not contains special character'}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
passwordCreateDemo = '';
get passwordConditionLetter() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[A-Za-z]/g);
}
get passwordConditionNumber() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[0-9]/g);
}
get passwordConditionSpecChar() {
  return this.passwordCreateDemo && this.passwordCreateDemo.match(/[\.\@\$\!\%\*\#\_\~\?\&\^]/g);
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

::ng-deep {
  .ap-field-password {
    .ap-field:not(.has-length),
    .ap-field:not(:focus-within) {
      .ap-field-password-creator {
        visibility: hidden;
        opacity: 0;
        transform: translateY(-10px);
      }
    }

    .ap-field-password-creator {
      position: absolute;
      left: 0;
      top: $spacing-8;
      display: grid;
      grid-auto-flow: column;
      grid-column-gap: $spacing-4;
      margin: $spacing-3 0;
      color: $color-text-body;
      transition: visibility $transition-0, opacity $transition-0, transform $transition-0;

      &>span {
        display: grid;
        grid-auto-flow: column;
        grid-column-gap: $spacing-1;

        .icon-circle-checkmark-fill {
          width: 12px;
          height: 12px;
          line-height: 12px;
          font-size: 8px;

          &::before {
            font-weight: $font-weight-3;
          }
        }

        .ap-field-password-condition {
          line-height: 12px;
          font-size: 12px;
        }

        &.highlight {
          color: $color-text-primary;
        }

        &:not(.highlight) {
          opacity: $opacity-6;
        } 
      }
    }

    .ap-field-password-condition-sr-only {
      position: absolute;
      left: 0;
      top: $spacing-8;
      clip: rect(0 0 0 0);
      height: 0;
      width: 0;
      margin: 0;
      padding: 0;
      overflow: hidden;
      white-space: no-wrap;
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:48 -->

<!-- EXAMPLE:49 -->
### passwordLogin - default


**Example #49** | **Variation**: passwordLogin | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:49 -->

<!-- EXAMPLE:50 -->
### passwordLogin - disabled


**Example #50** | **Variation**: passwordLogin | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:50 -->

<!-- EXAMPLE:51 -->
### passwordLogin - readOnly


**Example #51** | **Variation**: passwordLogin | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:51 -->

<!-- EXAMPLE:52 -->
### passwordLogin - required - default


**Example #52** | **Variation**: passwordLogin | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true" [required]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:52 -->

<!-- EXAMPLE:53 -->
### passwordLogin - required - disabled


**Example #53** | **Variation**: passwordLogin | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:53 -->

<!-- EXAMPLE:54 -->
### passwordLogin - required - readOnly


**Example #54** | **Variation**: passwordLogin | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field [title]="'Password'" [type]="'password'"
  [revealer]="true" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="passwordLoginDemo" aria-label="Password"/>
</ap-field>
```

#### TypeScript

```typescript
passwordLoginDemo = '';
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:54 -->

<!-- EXAMPLE:55 -->
### phoneNumber - default


**Example #55** | **Variation**: phoneNumber | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:55 -->

<!-- EXAMPLE:56 -->
### phoneNumber - disabled


**Example #56** | **Variation**: phoneNumber | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone" [disabled]="true">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:56 -->

<!-- EXAMPLE:57 -->
### phoneNumber - readOnly


**Example #57** | **Variation**: phoneNumber | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone" [readonly]="true">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:57 -->

<!-- EXAMPLE:58 -->
### phoneNumber - required - default


**Example #58** | **Variation**: phoneNumber | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone" [required]="true">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:58 -->

<!-- EXAMPLE:59 -->
### phoneNumber - required - disabled


**Example #59** | **Variation**: phoneNumber | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone" [required]="true" [disabled]="true">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:59 -->

<!-- EXAMPLE:60 -->
### phoneNumber - required - readOnly


**Example #60** | **Variation**: phoneNumber | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-phone" [title]="'Phone number'" [dropdown]="dropdownPhone" [img]="true"
  [selectedItem]="selectedPhone" [required]="true" [readonly]="true">
  <input appkit-field #phoneNumberField (input)="onPhoneChange($event)" [(ngModel)]="phone" aria-label="Phone number"/>
  <div class="ap-field-dropdown-phone-tag" #phoneTag>{{selectedPhone.descValue}}</div>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownPhone" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onPhoneSelect($event)">
    <ng-template ngTemplate="prefixTemp">
      <img class="ap-field-dropdown-icon" *ngIf="item.url" [src]="item.url" [alt]="item.label" />
    </ng-template>
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

@ViewChild('phoneTag') phoneTag!: ElementRef;
@ViewChild('phoneNumberField') phoneNumberField!: ElementRef;
phone = '';
dropdownPhone: any = [
  {
    label: 'China',
    value: 'China',
    url: 'assets/images/icon/china.png',
    descValue: '+86'
  },
  {
    label: 'Cuba',
    value: 'Cuba',
    url: 'assets/images/icon/cuba.png',
    descValue: '+53'
  },
  {
    label: 'India',
    value: 'India',
    url: 'assets/images/icon/india.png',
    descValue: '+91'
  },
  {
    label: 'Jamaica',
    value: 'Jamaica',
    url: 'assets/images/icon/jamaica.png',
    descValue: '+876'
  },
  {
    label: 'United States',
    value: 'United States',
    url: 'assets/images/icon/us.png',
    descValue: '+1'
  }
];

selectedPhone = {
  label: 'United States',
  value: 'United States',
  url: 'assets/images/icon/us.png',
  descValue: '+1'
};
onPhoneSelect(event: any) {
  this.selectedPhone = event.selected;
  setTimeout(() => {
    this.onPhoneChange({
      target: this.phoneNumberField.nativeElement
    }); // trigger formatter
    const tagWidth = this.phoneTag.nativeElement.getBoundingClientRect().width;
    (<HTMLElement>this.phoneNumberField.nativeElement).style.paddingLeft = tagWidth + 2 + 'px';
  }, 0);
}
onPhoneChange(event: any) {
  let value = event.target.value;
  if (this.selectedPhone.descValue === '+1') {
    value = value.replace(/[^\d]/g, '');
    if (value.length <= 3) {
      value = value.replace(/([\d]{1,3})/, '($1');
    } else if (value.length <= 6) {
      value = value.replace(/([\d]{3})([\d]{1,3})/, '($1) $2');
    } else {
      value = value.replace(/([\d]{3})([\d]{3})([\d]{1,4})[\d]*/, '($1) $2-$3');
    }
  } else {
    value = value.replace(/[^\d]/g, '');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
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

::ng-deep {
  .ap-field-dropdown-phone {
    .ap-field-img {
      padding: $spacing-1;
    }

    .ap-field-dropdown-item {

      .ap-option-item {
        padding: $spacing-4;

        .ap-option-left {
          column-gap: $spacing-3;
        }

        .ap-checkbox-label {
          margin: 0;

          .ap-option-prefix {
            display: flex;
            margin: 0;
          }
        }
      }

      .ap-field-dropdown-icon {
        padding: $spacing-1;
        width: 24px;
        height: 24px;
        user-select: none;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:60 -->

<!-- EXAMPLE:61 -->
### prefixAndSuffix - default


**Example #61** | **Variation**: prefixAndSuffix | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:61 -->

<!-- EXAMPLE:62 -->
### prefixAndSuffix - disabled


**Example #62** | **Variation**: prefixAndSuffix | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight" [disabled]="true">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency" [disabled]="true">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume" [disabled]="true">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:62 -->

<!-- EXAMPLE:63 -->
### prefixAndSuffix - readOnly


**Example #63** | **Variation**: prefixAndSuffix | **Modifier**: None | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight" [readonly]="true">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency" [readonly]="true">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume" [readonly]="true">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:63 -->

<!-- EXAMPLE:64 -->
### prefixAndSuffix - required - default


**Example #64** | **Variation**: prefixAndSuffix | **Modifier**: required | **State**: default

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight" [required]="true">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency" [required]="true">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume" [required]="true">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:64 -->

<!-- EXAMPLE:65 -->
### prefixAndSuffix - required - disabled


**Example #65** | **Variation**: prefixAndSuffix | **Modifier**: required | **State**: disabled

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight" [required]="true" [disabled]="true">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume" [required]="true" [disabled]="true">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:65 -->

<!-- EXAMPLE:66 -->
### prefixAndSuffix - required - readOnly


**Example #66** | **Variation**: prefixAndSuffix | **Modifier**: required | **State**: readOnly

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Height'" [dropdown]="dropdownHeight" [unit]="true"
  [selectedItem]="tagHeight" [required]="true" [readonly]="true">
  <input appkit-field #heightField (input)="onHeightChange($event)" [(ngModel)]="height" aria-label="Height"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownHeight" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onHeightSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Currency'" [dropdown]="dropdownCurrency" [unit]="true"
  [selectedItem]="tagCurrency" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="currency" aria-label="Currency"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownCurrency" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onCurrencySelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
<ap-field class="ap-field-dropdown-prefixsuffix" [title]="'Volume'" [dropdown]="dropdownVolume" [unit]="true"
  [selectedItem]="tagVolume" [required]="true" [readonly]="true">
  <input appkit-field [(ngModel)]="volume" aria-label="Volume"/>
  <ap-dropdown-list-item #dropdownListItem class="ap-field-dropdown-item" *ngFor="let item of dropdownVolume" [item]="item"
    [ariaLabel]="item.label+', '+ item.descValue" (onSelectItem)="onVolumeSelect($event)">
    <ng-template ngTemplate="suffixTemp">
      <span>{{item.descValue}}</span>
    </ng-template>
  </ap-dropdown-list-item>
</ap-field>
```

#### TypeScript

```typescript
import { ViewChild, ElementRef } from '@angular/core';

/* Height */
@ViewChild('heightField') heightField!: ElementRef;
height = "";
tagHeight = {
  label: 'Feet and inches',
  value: 'Feet and inches',
  descValue: 'ft/in',
  unit: 'ft/in'
};
dropdownHeight: any = [
  {
    label: 'Centimeters',
    value: 'Centimeters',
    descValue: 'cm',
    unit: 'cm'
  },
  {
    label: 'Feet and inches',
    value: 'Feet and inches',
    descValue: 'ft/in',
    unit: 'ft/in'
  }
];
onHeightSelect(event: any) {
  this.tagHeight = event.selected;
  this.onHeightChange({
    target: this.heightField.nativeElement,
    originEvent: event.originEvent
  });
}
onHeightChange(event: any) {
  let value = event.target.value;
  if (this.tagHeight.unit === 'ft/in') {
    value = value.replace(/[^\d]/g, '');
    if (event.inputType === 'deleteContentBackward' && !(event.target.value.endsWith('”') || event.target.value.endsWith('’'))) {
      value = value.substring(0, value.length - 1);
    }
    if (value.length === 1) {
      value = value.replace(/([\d])/, '$1’');
    } else if (value.length === 2) {
      value = value.replace(/([\d])([\d])/, '$1’ $2”');
    } else {
      let value1 = value.substring(0, 1);
      let value2 = value.substring(1);
      value2 = value2.startsWith('0') ? value2.replace('0', '') : value2;
      if (parseInt(value2) > 11) {
        if (event.data) {
          value2 = value2.replace(event.data, '');
        } else {
          value2 = '11';
        }
      }
      value = value ? `${value1}’ ${value2}”` : '';
    }
  } else if (this.tagHeight.unit === 'cm') {
    value = value.replace(/[^\d\.]/g, '').replace(/\.(\d*)\./g, '.$1').replace(/^0+/, '0').replace(/^0+([1-9])/, '$1');
  }
  event.target.value = value;
  event.target.dispatchEvent(new Event('change'));
}
/* Currency */
currency = "";
dropdownCurrency: any = [
  {
    label: 'EUR',
    value: 'EUR',
    descValue: '€',
    unit: '€'
  },
  {
    label: 'USD',
    value: 'USD',
    descValue: '$',
    unit: '$'
  },
  {
    label: 'CNY',
    value: 'CNY',
    descValue: '¥',
    unit: '¥'
  }
];
tagCurrency = {
  label: 'EUR',
  value: 'EUR',
  descValue: '€',
  unit: '€'
};
onCurrencySelect(event: any) {
  this.tagCurrency = event.selected;
}
/* Volume */
volume = "";
dropdownVolume: any = [
  {
    label: 'Ounce',
    value: 'Ounce',
    descValue: 'oz',
    unit: 'oz'
  }
];
tagVolume = {
  label: 'Ounce',
  value: 'Ounce',
  descValue: 'oz',
  unit: 'oz'
};
onVolumeSelect(event: any) {
  this.tagVolume = event.selected;
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

::ng-deep {
  .ap-field-dropdown-prefixsuffix {
    .ap-field-dropdown-item {
      .ap-option-item {
        padding: $spacing-4;
      }
    }
  }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0",
```

<!-- /EXAMPLE:66 -->

<!-- EXAMPLE:67 -->
### search - default


**Example #67** | **Variation**: search | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { SearchModule} fr

---
**WARNING:** Response truncated at 150000 characters. Use section parameter for specific content to avoid truncation.