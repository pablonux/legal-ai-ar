**Note:** This component documentation is large (390KB). Consider using section parameter (usage, examples, properties) for specific content.

---

---
component: modal
framework: angular
---

# Modal Component

## Overview

AppKit Modal component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Modal component:

- To display content that is separate from the main page.
- To convey information, provide a warning, force acknowledgment of an action, or display an error.

### Anatomy

**1. Headline:** Contains the title of the modal.

**2. Icon:** Icon to trigger additional actions.

**3. Close icon:** Closes the modal.

**4. Body:** The body of the modal contains the content of the modal.

**5. Buttons:** The footer of the modal contains any additional actions or buttons, such as a "close" or "submit" button.

### Variants

None

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-94522%26viewport%3D1232%252C-31914%252C0.48%26t%3Da2CkdLzXTHT8DrXK-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Modal should have a clear call to action to save or submit and/or close the modal.
- Modals should be used sparingly. Consider the following when using them: necessity of interrupting the user's flow, the data that will be blocked while displaying the modal and the importance of the message within the modal.
- Use a close button for dismissing the modal.

#### How not to use

- Do not use a modal within a modal.
- Do not use a modal for tasks that are complex or take significant time to complete.
- Do not overuse modals, they can be disruptive to the user's flow.
- Do not use modals for navigation.
- To contain information that is relevant to the rest of the page content, the Panel component must be used, not modals. Refer to the Panel component for more information.

### Behavior

- The modal component should be triggered by a button or link.
- When the modal is open, the main content of the page should be dimmed to indicate that the user is in a different context. Appkit uses an overlay with background blur style to help users focus on the modal contents.
- When the modal is closed, the main content of the page should be restored to its original state.

### Accessibility

- Ensure main window is not focusable when modal is open, use WAI ARIA role="dialog" and aria-modal="true" for screenreader to identify that the window behind modal can not be interacted with.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 117


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |
| 2 | headline | `section: "example:2"` |
| 3 | headline-body | `section: "example:3"` |
| 4 | headline-icon | `section: "example:4"` |
| 5 | headline-badge | `section: "example:5"` |
| 6 | headline-button | `section: "example:6"` |
| 7 | headline-body-icon | `section: "example:7"` |
| 8 | headline-body-badge | `section: "example:8"` |
| 9 | headline-body-button | `section: "example:9"` |
| 10 | headline-button-secondaryButton | `section: "example:10"` |
| 11 | headline-body-icon-badge | `section: "example:11"` |
| 12 | headline-body-icon-button | `section: "example:12"` |
| 13 | headline-body-button-secondaryButton | `section: "example:13"` |
| 14 | headline-icon-badge-button | `section: "example:14"` |
| 15 | headline-body-icon-badge-button | `section: "example:15"` |
| 16 | headline-body-icon-button-secondaryButton | `section: "example:16"` |
| 17 | headline-body-icon-badge-button-secondaryButton | `section: "example:17"` |
| 18 | icon | `section: "example:18"` |
| 19 | icon-badge | `section: "example:19"` |
| 20 | icon-button | `section: "example:20"` |
| 21 | icon-button-secondaryButton | `section: "example:21"` |
| 22 | icon-badge-button | `section: "example:22"` |
| 23 | icon-badge-button-secondaryButton | `section: "example:23"` |
| 24 | badge | `section: "example:24"` |
| 25 | badge-button | `section: "example:25"` |
| 26 | badge-button-secondaryButton | `section: "example:26"` |
| 27 | button | `section: "example:27"` |
| 28 | button-secondaryButton | `section: "example:28"` |
| 29 | dynamicModal | `section: "example:29"` |
| 30 | headline-dynamicModal | `section: "example:30"` |
| 31 | headline-body-dynamicModal | `section: "example:31"` |
| 32 | headline-icon-dynamicModal | `section: "example:32"` |
| 33 | headline-badge-dynamicModal | `section: "example:33"` |
| 34 | headline-button-dynamicModal | `section: "example:34"` |
| 35 | headline-body-icon-dynamicModal | `section: "example:35"` |
| 36 | headline-body-badge-dynamicModal | `section: "example:36"` |
| 37 | headline-body-button-dynamicModal | `section: "example:37"` |
| 38 | headline-button-secondaryButton-dynamicModal | `section: "example:38"` |
| 39 | headline-body-icon-badge-dynamicModal | `section: "example:39"` |
| 40 | headline-body-icon-button-dynamicModal | `section: "example:40"` |
| 41 | headline-body-button-secondaryButton-dynamicModal | `section: "example:41"` |
| 42 | headline-icon-badge-button-dynamicModal | `section: "example:42"` |
| 43 | headline-body-icon-badge-button-dynamicModal | `section: "example:43"` |
| 44 | headline-body-icon-button-secondaryButton-dynamicModal | `section: "example:44"` |
| 45 | headline-body-icon-badge-button-secondaryButton-dynamicModal | `section: "example:45"` |
| 46 | footnote | `section: "example:46"` |
| 47 | headline-footnote | `section: "example:47"` |
| 48 | headline-body-footnote | `section: "example:48"` |
| 49 | headline-icon-footnote | `section: "example:49"` |
| 50 | headline-badge-footnote | `section: "example:50"` |
| 51 | headline-button-footnote | `section: "example:51"` |
| 52 | headline-body-icon-footnote | `section: "example:52"` |
| 53 | headline-body-badge-footnote | `section: "example:53"` |
| 54 | headline-body-button-footnote | `section: "example:54"` |
| 55 | headline-icon-badge-footnote | `section: "example:55"` |
| 56 | headline-icon-button-footnote | `section: "example:56"` |
| 57 | headline-badge-button-footnote | `section: "example:57"` |
| 58 | headline-button-secondaryButton-footnote | `section: "example:58"` |
| 59 | headline-body-icon-badge-footnote | `section: "example:59"` |
| 60 | headline-body-icon-button-footnote | `section: "example:60"` |
| 61 | headline-body-badge-button-footnote | `section: "example:61"` |
| 62 | headline-body-button-secondaryButton-footnote | `section: "example:62"` |
| 63 | headline-icon-badge-button-footnote | `section: "example:63"` |
| 64 | headline-icon-button-secondaryButton-footnote | `section: "example:64"` |
| 65 | headline-badge-button-secondaryButton-footnote | `section: "example:65"` |
| 66 | headline-body-icon-badge-button-footnote | `section: "example:66"` |
| 67 | headline-icon-badge-button-secondaryButton-footnote | `section: "example:67"` |
| 68 | headline-body-badge-button-secondaryButton-footnote | `section: "example:68"` |
| 69 | headline-body-icon-button-secondaryButton-footnote | `section: "example:69"` |
| 70 | headline-body-icon-badge-button-secondaryButton-footnote | `section: "example:70"` |
| 71 | icon-footnote | `section: "example:71"` |
| 72 | icon-badge-footnote | `section: "example:72"` |
| 73 | icon-button-footnote | `section: "example:73"` |
| 74 | icon-button-secondaryButton-footnote | `section: "example:74"` |
| 75 | icon-badge-button-footnote | `section: "example:75"` |
| 76 | icon-badge-button-secondaryButton-footnote | `section: "example:76"` |
| 77 | badge-footnote | `section: "example:77"` |
| 78 | badge-button-footnote | `section: "example:78"` |
| 79 | badge-button-secondaryButton-footnote | `section: "example:79"` |
| 80 | button-footnote | `section: "example:80"` |
| 81 | button-secondaryButton-footnote | `section: "example:81"` |
| 82 | dynamicModal-footnote | `section: "example:82"` |
| 83 | headline-dynamicModal-footnote | `section: "example:83"` |
| 84 | headline-body-dynamicModal-footnote | `section: "example:84"` |
| 85 | headline-icon-dynamicModal-footnote | `section: "example:85"` |
| 86 | headline-badge-dynamicModal-footnote | `section: "example:86"` |
| 87 | headline-button-dynamicModal-footnote | `section: "example:87"` |
| 88 | headline-body-icon-dynamicModal-footnote | `section: "example:88"` |
| 89 | headline-body-badge-dynamicModal-footnote | `section: "example:89"` |
| 90 | headline-body-button-dynamicModal-footnote | `section: "example:90"` |
| 91 | headline-icon-badge-dynamicModal-footnote | `section: "example:91"` |
| 92 | headline-icon-button-dynamicModal-footnote | `section: "example:92"` |
| 93 | headline-badge-button-dynamicModal-footnote | `section: "example:93"` |
| 94 | headline-button-secondaryButton-dynamicModal-footnote | `section: "example:94"` |
| 95 | headline-body-icon-badge-dynamicModal-footnote | `section: "example:95"` |
| 96 | headline-body-icon-button-dynamicModal-footnote | `section: "example:96"` |
| 97 | headline-body-badge-button-dynamicModal-footnote | `section: "example:97"` |
| 98 | headline-body-button-secondaryButton-dynamicModal-footnote | `section: "example:98"` |
| 99 | headline-icon-badge-button-dynamicModal-footnote | `section: "example:99"` |
| 100 | headline-icon-button-secondaryButton-dynamicModal-footnote | `section: "example:100"` |
| 101 | headline-badge-button-secondaryButton-dynamicModal-footnote | `section: "example:101"` |
| 102 | headline-body-icon-badge-button-dynamicModal-footnote | `section: "example:102"` |
| 103 | headline-icon-badge-button-secondaryButton-dynamicModal-footnote | `section: "example:103"` |
| 104 | headline-body-badge-button-secondaryButton-dynamicModal-footnote | `section: "example:104"` |
| 105 | headline-body-icon-button-secondaryButton-dynamicModal-footnote | `section: "example:105"` |
| 106 | headline-body-icon-badge-button-secondaryButton-dynamicModal-footnote | `section: "example:106"` |
| 107 | icon-dynamicModal-footnote | `section: "example:107"` |
| 108 | icon-badge-dynamicModal-footnote | `section: "example:108"` |
| 109 | icon-button-dynamicModal-footnote | `section: "example:109"` |
| 110 | icon-button-secondaryButton-dynamicModal-footnote | `section: "example:110"` |
| 111 | icon-badge-button-dynamicModal-footnote | `section: "example:111"` |
| 112 | icon-badge-button-secondaryButton-dynamicModal-footnote | `section: "example:112"` |
| 113 | badge-dynamicModal-footnote | `section: "example:113"` |
| 114 | badge-button-dynamicModal-footnote | `section: "example:114"` |
| 115 | badge-button-secondaryButton-dynamicModal-footnote | `section: "example:115"` |
| 116 | button-dynamicModal-footnote | `section: "example:116"` |
| 117 | button-secondaryButton-dynamicModal-footnote | `section: "example:117"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle"> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### headline


**Example #2** | **Variation**: None | **Modifier**: headline | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle"> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### headline-body


**Example #3** | **Variation**: None | **Modifier**: headline-body | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 540px;';
contentStyle: string = 'min-height: 92px;';
footerStyle: string = 'display: none;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### headline-icon


**Example #4** | **Variation**: None | **Modifier**: headline-icon | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### headline-badge


**Example #5** | **Variation**: None | **Modifier**: headline-badge | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### headline-button


**Example #6** | **Variation**: None | **Modifier**: headline-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### headline-body-icon


**Example #7** | **Variation**: None | **Modifier**: headline-body-icon | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 540px;';
contentStyle: string = 'min-height: 92px;';
footerStyle: string = 'display: none;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### headline-body-badge


**Example #8** | **Variation**: None | **Modifier**: headline-body-badge | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 540px;';
contentStyle: string = 'min-height: 92px;';
footerStyle: string = 'display: none;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### headline-body-button


**Example #9** | **Variation**: None | **Modifier**: headline-body-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### headline-button-secondaryButton


**Example #10** | **Variation**: None | **Modifier**: headline-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### headline-body-icon-badge


**Example #11** | **Variation**: None | **Modifier**: headline-body-icon-badge | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 540px;';
contentStyle: string = 'min-height: 92px;';
footerStyle: string = 'display: none;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### headline-body-icon-button


**Example #12** | **Variation**: None | **Modifier**: headline-body-icon-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### headline-body-button-secondaryButton


**Example #13** | **Variation**: None | **Modifier**: headline-body-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### headline-icon-badge-button


**Example #14** | **Variation**: None | **Modifier**: headline-icon-badge-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### headline-body-icon-badge-button


**Example #15** | **Variation**: None | **Modifier**: headline-body-icon-badge-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### headline-body-icon-button-secondaryButton


**Example #16** | **Variation**: None | **Modifier**: headline-body-icon-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### headline-body-icon-badge-button-secondaryButton


**Example #17** | **Variation**: None | **Modifier**: headline-body-icon-badge-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### icon


**Example #18** | **Variation**: None | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### icon-badge


**Example #19** | **Variation**: None | **Modifier**: icon-badge | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### icon-button


**Example #20** | **Variation**: None | **Modifier**: icon-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### icon-button-secondaryButton


**Example #21** | **Variation**: None | **Modifier**: icon-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### icon-badge-button


**Example #22** | **Variation**: None | **Modifier**: icon-badge-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### icon-badge-button-secondaryButton


**Example #23** | **Variation**: None | **Modifier**: icon-badge-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### badge


**Example #24** | **Variation**: None | **Modifier**: badge | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template> 
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### badge-button


**Example #25** | **Variation**: None | **Modifier**: badge-button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### badge-button-secondaryButton


**Example #26** | **Variation**: None | **Modifier**: badge-button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### button


**Example #27** | **Variation**: None | **Modifier**: button | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### button-secondaryButton


**Example #28** | **Variation**: None | **Modifier**: button-secondaryButton | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'" (onClick)="handleOk()" [label]="'Ipsum'"></ap-button>  
      <ap-button [btnType]="'primary'" (onClick)="handleCancel()" [label]="'Lorem'"></ap-button></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### dynamicModal


**Example #29** | **Variation**: None | **Modifier**: dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">

        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;


  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### headline-dynamicModal


**Example #30** | **Variation**: None | **Modifier**: headline-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### headline-body-dynamicModal


**Example #31** | **Variation**: None | **Modifier**: headline-body-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### headline-icon-dynamicModal


**Example #32** | **Variation**: None | **Modifier**: headline-icon-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### headline-badge-dynamicModal


**Example #33** | **Variation**: None | **Modifier**: headline-badge-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### headline-button-dynamicModal


**Example #34** | **Variation**: None | **Modifier**: headline-button-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">


           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### headline-body-icon-dynamicModal


**Example #35** | **Variation**: None | **Modifier**: headline-body-icon-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### headline-body-badge-dynamicModal


**Example #36** | **Variation**: None | **Modifier**: headline-body-badge-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### headline-body-button-dynamicModal


**Example #37** | **Variation**: None | **Modifier**: headline-body-button-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">


           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### headline-button-secondaryButton-dynamicModal


**Example #38** | **Variation**: None | **Modifier**: headline-button-secondaryButton-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">

           <ap-button [btnType]="'secondary'" [label]="'Ipsum'"></ap-button>
           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
      &-customize {
        display: flex;
      }

    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### headline-body-icon-badge-dynamicModal


**Example #39** | **Variation**: None | **Modifier**: headline-body-icon-badge-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### headline-body-icon-button-dynamicModal


**Example #40** | **Variation**: None | **Modifier**: headline-body-icon-button-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">


           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### headline-body-button-secondaryButton-dynamicModal


**Example #41** | **Variation**: None | **Modifier**: headline-body-button-secondaryButton-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">

           <ap-button [btnType]="'secondary'" [label]="'Ipsum'"></ap-button>
           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container"> 

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
      &-customize {
        display: flex;
      }

    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### headline-icon-badge-button-dynamicModal


**Example #42** | **Variation**: None | **Modifier**: headline-icon-badge-button-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body"> 
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">


           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }


  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### headline-body-icon-badge-button-dynamicModal


**Example #43** | **Variation**: None | **Modifier**: headline-body-icon-badge-button-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">


           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### headline-body-icon-button-secondaryButton-dynamicModal


**Example #44** | **Variation**: None | **Modifier**: headline-body-icon-button-secondaryButton-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">

           <ap-button [btnType]="'secondary'" [label]="'Ipsum'"></ap-button>
           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
      &-customize {
        display: flex;
      }

    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:44 -->

<!-- EXAMPLE:45 -->
### headline-body-icon-badge-button-secondaryButton-dynamicModal


**Example #45** | **Variation**: None | **Modifier**: headline-body-icon-badge-button-secondaryButton-dynamicModal | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { DynamicModalDemoModule } from './app.component'; // Import the custom module
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="openModal('showDynamicModal')" [label]="'Open Modal'"></ap-button>
```

#### TypeScript

```typescript
import { DynamicDialogService } from "@appkit4/angular-components/modal";
import { ButtonModule } from "@appkit4/angular-components/button";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { Component, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Your own component in which you define the method 'openModal'
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: false
})
export class AppComponent {
  constructor(public dynamicDialogService: DynamicDialogService) {}

  openModal(modalId: string) {
    let modal = this.dynamicDialogService.openModal(
      DynamicModalDemoComponent,
      {
        appModalId: modalId,
        styleClass: 'dynamic-modal-wrapper'
      },
      {
        title: 'Lorem ipsum dolor sit'
      }
    );
    // Get value from the modal
    modal.instance.afterClosed().subscribe((value: any) => {
      console.log(value);
    });
  }
}

// Customized component which displayed in modal dynamically, user can define it with any name, need to declare it in module
@Component({
  selector: 'dynamic-modal-demo',
  standalone: false,
  styleUrls: ['./app.component.scss'],
  template: `
    <div class="ap-modal-live-demo" role="dialog" aria-modal="true"
      aria-label="the modal of Lorem ipsum dolor sit">
      <div class="ap-modal-header">
        <ap-badge value="Success" type="success">
        </ap-badge>
        <div class="ap-modal-title">
          {{title}}
        </div>
      </div>
      <div class="ap-modal-body">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit,
          sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
          veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
        </p>
      </div>

      <div class="ap-modal-footer">
        <div class="ap-modal-footer-customize">

           <ap-button [btnType]="'secondary'" [label]="'Ipsum'"></ap-button>
           <ap-button [btnType]="'primary'" (onClick)="handleCancel()" (keyup.enter)="handleCancel()" [label]="'Lorem'"></ap-button>
        </div>
      </div>
      <div class="ap-modal-header-icons-container">
        <button type="button" class="ap-modal-header-icon ap-modal-header-more"
          aria-label="more">
          <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
        </button>

        <button type="button" aria-label="Close" (click)="handleCancel()"
          (keydown)="onKeydownButton($event)"
          class="ap-modal-header-icon ap-modal-header-close">
          <span class="Appkit4-icon icon-close-outline"></span>
        </button>
      </div>
    </div>
  `
})
export class DynamicModalDemoComponent {
  title: string = '';
  constructor(public dynamicDialogService: DynamicDialogService) {}

  handleCancel() {
    this.dynamicDialogService.closeModal('showDynamicModal', {
      message: 'Hey, this is a message from the dynamic modal!'
    });
  }
  onKeydownButton(event: any) {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      this.handleCancel();
    }
  }
}

@NgModule({
  imports: [CommonModule, ButtonModule, BadgeModule], 
  exports: [DynamicModalDemoComponent], 
  declarations: [DynamicModalDemoComponent]
})
export class DynamicModalDemoModule {}
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

::ng-deep .dynamic-modal-wrapper {
  width: rem(540px);
}

// Css for customize component
.ap-modal-live-demo {
  position: relative;
  background-color: $color-background-container;
  box-shadow: 0 rem(8px) rem(16px) rem(-2px) rgba(71, 71, 71, .24), 0 0 1px 0 rgba(71, 71, 71, .08);
  border-radius: $border-radius-3;
  font-size: rem(16px);
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;

  .ap-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      text-align: left;
      padding: $spacing-6;

      .ap-badge {
        margin-right: $spacing-3;
      }

      .ap-modal-title {
        font: $typography-heading-s;
        letter-spacing: rem(-0.4px);
        color: $color-text-heading;
        margin-right: auto;
      }

  }
  .ap-modal-body {
    min-height: rem(92px);
  }

  .ap-modal-body {
    padding: 0px $spacing-6 $spacing-6 $spacing-6;
    min-height: rem(92px);

    p {
        margin: 0;
        font: $typography-body;
        letter-spacing: rem(-0.4px);
        color: $color-text-body;
    }
  }

  .ap-modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-3 $spacing-6 $spacing-6 $spacing-6;
    overflow: visible;
    min-height: rem(64px);
      &-customize {
        display: flex;
      }

    }

  .ap-modal-header-icons-container {
    position: absolute;
    top: $spacing-4;
    right: $spacing-4;
    display: flex;
    gap: $spacing-2;

    .ap-modal-header-icon {
      position: relative;
      width: rem(40px);
      height: rem(40px);
      padding: $spacing-3;
      background-color: transparent;

      &:hover {
          background-color: $color-background-hover;
          border-radius: $border-radius-2;
          cursor: pointer;
      }
    }
  }
}
```

<!-- /EXAMPLE:45 -->

<!-- EXAMPLE:46 -->
### footnote


**Example #46** | **Variation**: None | **Modifier**: footnote | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal [ariaLabel]="'modal demo'" appModalId="modal-footer" [title]="' '" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">
      <div class="ap-modal-footnote">Required fields</div></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:46 -->

<!-- EXAMPLE:47 -->
### headline-footnote


**Example #47** | **Variation**: None | **Modifier**: headline-footnote | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
    <ng-template ngTemplate="footer">
      <div class="ap-modal-footnote">Required fields</div></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:47 -->

<!-- EXAMPLE:48 -->
### headline-body-footnote


**Example #48** | **Variation**: None | **Modifier**: headline-body-footnote | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <p>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
    sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
  </p>
    <ng-template ngTemplate="footer">
      <div class="ap-modal-footnote">Required fields</div></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:48 -->

<!-- EXAMPLE:49 -->
### headline-icon-footnote


**Example #49** | **Variation**: None | **Modifier**: headline-icon-footnote | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="icons">
    <button type="button" class="ap-modal-header-icon ap-modal-header-more" aria-label="more">
      <span class="Appkit4-icon icon-horizontal-more-outline" aria-hidden="true"></span>
    </button>
  </ng-template>
    <ng-template ngTemplate="footer">
      <div class="ap-modal-footnote">Required fields</div></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; margin-top: -0.5rem;';
@ViewChild('modal', { static: false }) modal: any;

showModal() {
  this.modal.showModal('modal-footer');
}

handleOk() {

}

handleCancel() {
  this.modal.closeModal('modal-footer');
}     
```

<!-- /EXAMPLE:49 -->

<!-- EXAMPLE:50 -->
### headline-badge-footnote


**Example #50** | **Variation**: None | **Modifier**: headline-badge-footnote | **State**: None

#### Module Import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from "@appkit4/angular-components/badge";
```

#### HTML Template

```html
                         <ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open Modal'"></ap-button>
<ap-modal #modal appModalId="modal-footer" [title]="'Lorem ipsum dolor sit'" [style]="style" [footerStyle]="footerStyle" [contentStyle]="contentStyle">
  <ng-template ngTemplate="header">
    <ap-badge value="Success" type="success"></ap-badge>
  </ng-template>
    <ng-template ngTemplate="footer">
      <div class="ap-modal-footnote">Required fields</div></ng-template>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

style: string = 'width: 33.75rem; // 540px';
contentStyle: string = 'min-height: 5.75rem; // 92px';
footerStyle: string = 'padding-top: 0.5rem; m

---
**WARNING:** Response truncated at 150000 characters. Use section parameter for specific content to avoid truncation.