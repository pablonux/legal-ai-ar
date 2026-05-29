---
component: button
framework: angular
---

# Button Component

## Overview

AppKit Button component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use buttons as event triggers or actions. Commonly placed within other areas of the UI. Each type of button has peculiarities of use.

### Anatomy

A button’s text label is the most important element on a button, as it communicates the action that will be performed when the user interacts with it. The text is always left-aligned, not centered.

If a text label is not used, an icon should be present to signify what the button does.

- Container
- Label
- Chevron
- Icon

### Hierarchy

| Variation | Description |
|---|---|
| Primary | Heavy visual weight and identifies the primary action in a set of buttons |
| Secondary | Medium visual weight for any task that is not a primary action |
| Tertiary | Lightest visual weight for independent or less prominent actions |
| Negative | Bold visual weight to signify destructive actions |
| Text | Least visual weight for less important actions |

### Best Practices

Regardless of the button variant you are using, these guidelines should be followed

#### How to use

- Use sentence case on all instances. Don’t use capitalization to emphasize a button.
- Set a button’s width to be the size of the text label, with 24px padding on both sides or alternatively, buttons can expand to the size of the parent element.
- Keep 8 pixels margin between buttons.
- Buttons padding in relation to other interface elements should be 20px.
- Buttons can be adapted to take the full width of the modal with a 20px padding.

#### How not to use

- Don’t override or apply custom colors. The colors have been designed to be accessible and consistent across products.
- Avoid long labels on text buttons.
- Text and Secondary outlined buttons should be placed carefully to guarantee legibility.
- When using icons in buttons, don’t change the padding or align the icon in a different position.
- Don’t add punctuation or decoration to button labels.
- Don’t place secondary buttons over images or intricate backgrounds.

### Primary

A primary button should be used for a required or principal action within a view. Commonly used for actions such as upload, submit, confirm or delete.

#### Usage

#### Default

A primary button should be used for a required or principal action within a view. Commonly used for actions such as upload, submit, confirm or delete.

#### Split

A split action button is used for instances where a primary call to action has many options.

#### Menu

Similar to a group button, the menu button is used for instances where a call to action has many options, however it does have some differences concerning the interaction.

#### Add

The Add or FAB Button typically appears in front of all screen content and is used to represent a primary action.

### Secondary

A secondary button is used for any task that is not a primary action.

#### Usage

#### Secondary

A secondary button is used for any task that is not a primary action.

#### Group

A group action button is used for instances where a secondary call to action has many options.

### Tertiary

A tertiary button is used for independent or less prominent actions.

#### Usage

#### Default

A tertiary button is used for independent or less prominent actions.

#### Group

A group action button is used for instances where a tertiary call to action has many options.

### Negative

A negative button warns the user that corresponding action will have negative or destructive impact, such as permanent deletion of data.

#### Usage

#### Default

A negative button warns the user that corresponding action will have negative or destructive impact, such as permanent deletion of data.

#### Group

A group action button is used for instances where a negative call to action has many options.

### Text

A text button is used for less important actions.

#### Usage

#### Default

A text button is used for less important actions.

#### Text

A group action button is used for instances where a less prominent call to action has many options.

### Group

A button group is used for buttons whose actions are related to each other.

#### Usage

#### Horizontal

Use by default.

#### Vertical

Use when horizontal space is limited.

#### Left-aligned

In general, used for buttons that sit on a page without a container.

#### Right-aligned

In general, used for buttons that sit within a container (modals, panels, tables). It may also be used center-aligned when following center-aligned text.

#### Fill

In general, used for buttons that sit within a narrow container.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-108507%26viewport%3D-50%252C-2734%252C0.52%26t%3DTMTEdkRmOcKxYsdh-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Behavior

If a single button is used in a container, the size of the button can also change as a parent container scales to adapt.

Label and icons must remain centered when the container's measurements are scaled.
The placement and size of the buttons can change to suit the shapes of the container.

### Accessibility

- Buttons should display a visible focus when users tab to them. Don't suppress focus outline.
- When in focus, SPACE and ENTER can activate the button.
- Use standard markup to create buttons, such as a <button> to perform an action or a <a> to go to another page as screen readers handle buttons and links differently.
- When in focus, SPACE and ENTER can activate the button.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 94


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | primary | `section: "example:1"` |
| 2 | primary - icon | `section: "example:2"` |
| 3 | primary - compact | `section: "example:3"` |
| 4 | primary - disabled | `section: "example:4"` |
| 5 | primary - loading | `section: "example:5"` |
| 6 | primary - icon-compact | `section: "example:6"` |
| 7 | primary - icon - disabled | `section: "example:7"` |
| 8 | primary - icon - loading | `section: "example:8"` |
| 9 | primary - compact - disabled | `section: "example:9"` |
| 10 | primary - compact - loading | `section: "example:10"` |
| 11 | primary - icon-compact - loading | `section: "example:11"` |
| 12 | primary - icon-compact - disabled | `section: "example:12"` |
| 13 | secondary | `section: "example:13"` |
| 14 | secondary - icon | `section: "example:14"` |
| 15 | secondary - compact | `section: "example:15"` |
| 16 | secondary - disabled | `section: "example:16"` |
| 17 | secondary - loading | `section: "example:17"` |
| 18 | secondary - icon-compact | `section: "example:18"` |
| 19 | secondary - icon - disabled | `section: "example:19"` |
| 20 | secondary - icon - loading | `section: "example:20"` |
| 21 | secondary - compact - disabled | `section: "example:21"` |
| 22 | secondary - compact - loading | `section: "example:22"` |
| 23 | secondary - icon-compact - loading | `section: "example:23"` |
| 24 | secondary - icon-compact - disabled | `section: "example:24"` |
| 25 | tertiary | `section: "example:25"` |
| 26 | tertiary - icon | `section: "example:26"` |
| 27 | tertiary - compact | `section: "example:27"` |
| 28 | tertiary - disabled | `section: "example:28"` |
| 29 | tertiary - loading | `section: "example:29"` |
| 30 | tertiary - icon-compact | `section: "example:30"` |
| 31 | tertiary - icon - disabled | `section: "example:31"` |
| 32 | tertiary - icon - loading | `section: "example:32"` |
| 33 | tertiary - compact - disabled | `section: "example:33"` |
| 34 | tertiary - compact - loading | `section: "example:34"` |
| 35 | tertiary - icon-compact - loading | `section: "example:35"` |
| 36 | tertiary - icon-compact - disabled | `section: "example:36"` |
| 37 | text | `section: "example:37"` |
| 38 | text - icon | `section: "example:38"` |
| 39 | text - compact | `section: "example:39"` |
| 40 | text - disabled | `section: "example:40"` |
| 41 | text - loading | `section: "example:41"` |
| 42 | text - icon-compact | `section: "example:42"` |
| 43 | text - icon - disabled | `section: "example:43"` |
| 44 | text - icon - loading | `section: "example:44"` |
| 45 | text - compact - disabled | `section: "example:45"` |
| 46 | text - compact - loading | `section: "example:46"` |
| 47 | text - icon-compact - loading | `section: "example:47"` |
| 48 | text - icon-compact - disabled | `section: "example:48"` |
| 49 | negative | `section: "example:49"` |
| 50 | negative - icon | `section: "example:50"` |
| 51 | negative - compact | `section: "example:51"` |
| 52 | negative - disabled | `section: "example:52"` |
| 53 | negative - loading | `section: "example:53"` |
| 54 | negative - icon-compact | `section: "example:54"` |
| 55 | negative - icon - disabled | `section: "example:55"` |
| 56 | negative - icon - loading | `section: "example:56"` |
| 57 | negative - compact - disabled | `section: "example:57"` |
| 58 | negative - compact - loading | `section: "example:58"` |
| 59 | negative - icon-compact - loading | `section: "example:59"` |
| 60 | negative - icon-compact - disabled | `section: "example:60"` |
| 61 | group - leftAlignment | `section: "example:61"` |
| 62 | group - compact-leftAlignment | `section: "example:62"` |
| 63 | group - leftAlignment-vertical | `section: "example:63"` |
| 64 | group - compact-leftAlignment-vertical | `section: "example:64"` |
| 65 | group - leftAlignment - disabled | `section: "example:65"` |
| 66 | group - compact-leftAlignment - disabled | `section: "example:66"` |
| 67 | group - leftAlignment-vertical - disabled | `section: "example:67"` |
| 68 | group - compact-leftAlignment-vertical - disabled | `section: "example:68"` |
| 69 | group - rightAlignment | `section: "example:69"` |
| 70 | group - compact-rightAlignment | `section: "example:70"` |
| 71 | group - rightAlignment-vertical | `section: "example:71"` |
| 72 | group - compact-rightAlignment-vertical | `section: "example:72"` |
| 73 | group - rightAlignment - disabled | `section: "example:73"` |
| 74 | group - compact-rightAlignment - disabled | `section: "example:74"` |
| 75 | group - rightAlignment-vertical - disabled | `section: "example:75"` |
| 76 | group - compact-rightAlignment-vertical - disabled | `section: "example:76"` |
| 77 | split | `section: "example:77"` |
| 78 | split - disabled | `section: "example:78"` |
| 79 | split - compact | `section: "example:79"` |
| 80 | split - compact - disabled | `section: "example:80"` |
| 81 | menu | `section: "example:81"` |
| 82 | menu - disabled | `section: "example:82"` |
| 83 | menu - compact | `section: "example:83"` |
| 84 | menu - complex | `section: "example:84"` |
| 85 | menu - compact - disabled | `section: "example:85"` |
| 86 | menu - complex - disabled | `section: "example:86"` |
| 87 | menu - compact-complex - disabled | `section: "example:87"` |
| 88 | menu - compact-complex | `section: "example:88"` |
| 89 | icon-only-menu | `section: "example:89"` |
| 90 | icon-only-menu - disabled | `section: "example:90"` |
| 91 | icon-only-menu - compact | `section: "example:91"` |
| 92 | icon-only-menu - compact - disabled | `section: "example:92"` |
| 93 | add | `section: "example:93"` |
| 94 | add - compact | `section: "example:94"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### primary


**Example #1** | **Variation**: primary | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### primary - icon


**Example #2** | **Variation**: primary | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### primary - compact


**Example #3** | **Variation**: primary | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### primary - disabled


**Example #4** | **Variation**: primary | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [disabled]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### primary - loading


**Example #5** | **Variation**: primary | **Modifier**: None | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### primary - icon-compact


**Example #6** | **Variation**: primary | **Modifier**: icon-compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### primary - icon - disabled


**Example #7** | **Variation**: primary | **Modifier**: icon | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [disabled]="true" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### primary - icon - loading


**Example #8** | **Variation**: primary | **Modifier**: icon | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### primary - compact - disabled


**Example #9** | **Variation**: primary | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [disabled]="true" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### primary - compact - loading


**Example #10** | **Variation**: primary | **Modifier**: compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### primary - icon-compact - loading


**Example #11** | **Variation**: primary | **Modifier**: icon-compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### primary - icon-compact - disabled


**Example #12** | **Variation**: primary | **Modifier**: icon-compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [disabled]="true" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### secondary


**Example #13** | **Variation**: secondary | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### secondary - icon


**Example #14** | **Variation**: secondary | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### secondary - compact


**Example #15** | **Variation**: secondary | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### secondary - disabled


**Example #16** | **Variation**: secondary | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [disabled]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### secondary - loading


**Example #17** | **Variation**: secondary | **Modifier**: None | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### secondary - icon-compact


**Example #18** | **Variation**: secondary | **Modifier**: icon-compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### secondary - icon - disabled


**Example #19** | **Variation**: secondary | **Modifier**: icon | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [disabled]="true" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### secondary - icon - loading


**Example #20** | **Variation**: secondary | **Modifier**: icon | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### secondary - compact - disabled


**Example #21** | **Variation**: secondary | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [disabled]="true" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### secondary - compact - loading


**Example #22** | **Variation**: secondary | **Modifier**: compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### secondary - icon-compact - loading


**Example #23** | **Variation**: secondary | **Modifier**: icon-compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### secondary - icon-compact - disabled


**Example #24** | **Variation**: secondary | **Modifier**: icon-compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'secondary'" [disabled]="true" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### tertiary


**Example #25** | **Variation**: tertiary | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### tertiary - icon


**Example #26** | **Variation**: tertiary | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### tertiary - compact


**Example #27** | **Variation**: tertiary | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### tertiary - disabled


**Example #28** | **Variation**: tertiary | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [disabled]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### tertiary - loading


**Example #29** | **Variation**: tertiary | **Modifier**: None | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### tertiary - icon-compact


**Example #30** | **Variation**: tertiary | **Modifier**: icon-compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### tertiary - icon - disabled


**Example #31** | **Variation**: tertiary | **Modifier**: icon | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [disabled]="true" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### tertiary - icon - loading


**Example #32** | **Variation**: tertiary | **Modifier**: icon | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### tertiary - compact - disabled


**Example #33** | **Variation**: tertiary | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [disabled]="true" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### tertiary - compact - loading


**Example #34** | **Variation**: tertiary | **Modifier**: compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### tertiary - icon-compact - loading


**Example #35** | **Variation**: tertiary | **Modifier**: icon-compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### tertiary - icon-compact - disabled


**Example #36** | **Variation**: tertiary | **Modifier**: icon-compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'tertiary'" [disabled]="true" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### text


**Example #37** | **Variation**: text | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### text - icon


**Example #38** | **Variation**: text | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### text - compact


**Example #39** | **Variation**: text | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### text - disabled


**Example #40** | **Variation**: text | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [disabled]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### text - loading


**Example #41** | **Variation**: text | **Modifier**: None | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### text - icon-compact


**Example #42** | **Variation**: text | **Modifier**: icon-compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### text - icon - disabled


**Example #43** | **Variation**: text | **Modifier**: icon | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [disabled]="true" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### text - icon - loading


**Example #44** | **Variation**: text | **Modifier**: icon | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:44 -->

<!-- EXAMPLE:45 -->
### text - compact - disabled


**Example #45** | **Variation**: text | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [disabled]="true" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:45 -->

<!-- EXAMPLE:46 -->
### text - compact - loading


**Example #46** | **Variation**: text | **Modifier**: compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:46 -->

<!-- EXAMPLE:47 -->
### text - icon-compact - loading


**Example #47** | **Variation**: text | **Modifier**: icon-compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:47 -->

<!-- EXAMPLE:48 -->
### text - icon-compact - disabled


**Example #48** | **Variation**: text | **Modifier**: icon-compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'text'" [disabled]="true" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:48 -->

<!-- EXAMPLE:49 -->
### negative


**Example #49** | **Variation**: negative | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:49 -->

<!-- EXAMPLE:50 -->
### negative - icon


**Example #50** | **Variation**: negative | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:50 -->

<!-- EXAMPLE:51 -->
### negative - compact


**Example #51** | **Variation**: negative | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:51 -->

<!-- EXAMPLE:52 -->
### negative - disabled


**Example #52** | **Variation**: negative | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [disabled]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:52 -->

<!-- EXAMPLE:53 -->
### negative - loading


**Example #53** | **Variation**: negative | **Modifier**: None | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:53 -->

<!-- EXAMPLE:54 -->
### negative - icon-compact


**Example #54** | **Variation**: negative | **Modifier**: icon-compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:54 -->

<!-- EXAMPLE:55 -->
### negative - icon - disabled


**Example #55** | **Variation**: negative | **Modifier**: icon | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [disabled]="true" [icon]="'time-outline'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:55 -->

<!-- EXAMPLE:56 -->
### negative - icon - loading


**Example #56** | **Variation**: negative | **Modifier**: icon | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:56 -->

<!-- EXAMPLE:57 -->
### negative - compact - disabled


**Example #57** | **Variation**: negative | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [disabled]="true" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:57 -->

<!-- EXAMPLE:58 -->
### negative - compact - loading


**Example #58** | **Variation**: negative | **Modifier**: compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:58 -->

<!-- EXAMPLE:59 -->
### negative - icon-compact - loading


**Example #59** | **Variation**: negative | **Modifier**: icon-compact | **State**: loading

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [icon]="'time-outline'" [loading]="true" [isLoading]="isLoading" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
isLoading = false;
onClickButton(event: any) {
  this.isLoading = true;
  setTimeout(() => {
    this.isLoading = false;
  }, 1000);
}
```

<!-- /EXAMPLE:59 -->

<!-- EXAMPLE:60 -->
### negative - icon-compact - disabled


**Example #60** | **Variation**: negative | **Modifier**: icon-compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'negative'" [disabled]="true" [icon]="'time-outline'" [compact]="true" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:60 -->

<!-- EXAMPLE:61 -->
### group - leftAlignment


**Example #61** | **Variation**: group | **Modifier**: leftAlignment | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'primary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:61 -->

<!-- EXAMPLE:62 -->
### group - compact-leftAlignment


**Example #62** | **Variation**: group | **Modifier**: compact-leftAlignment | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'primary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:62 -->

<!-- EXAMPLE:63 -->
### group - leftAlignment-vertical


**Example #63** | **Variation**: group | **Modifier**: leftAlignment-vertical | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'primary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:63 -->

<!-- EXAMPLE:64 -->
### group - compact-leftAlignment-vertical


**Example #64** | **Variation**: group | **Modifier**: compact-leftAlignment-vertical | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'primary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:64 -->

<!-- EXAMPLE:65 -->
### group - leftAlignment - disabled


**Example #65** | **Variation**: group | **Modifier**: leftAlignment | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:65 -->

<!-- EXAMPLE:66 -->
### group - compact-leftAlignment - disabled


**Example #66** | **Variation**: group | **Modifier**: compact-leftAlignment | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:66 -->

<!-- EXAMPLE:67 -->
### group - leftAlignment-vertical - disabled


**Example #67** | **Variation**: group | **Modifier**: leftAlignment-vertical | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:67 -->

<!-- EXAMPLE:68 -->
### group - compact-leftAlignment-vertical - disabled


**Example #68** | **Variation**: group | **Modifier**: compact-leftAlignment-vertical | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:68 -->

<!-- EXAMPLE:69 -->
### group - rightAlignment


**Example #69** | **Variation**: group | **Modifier**: rightAlignment | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:69 -->

<!-- EXAMPLE:70 -->
### group - compact-rightAlignment


**Example #70** | **Variation**: group | **Modifier**: compact-rightAlignment | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:70 -->

<!-- EXAMPLE:71 -->
### group - rightAlignment-vertical


**Example #71** | **Variation**: group | **Modifier**: rightAlignment-vertical | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:71 -->

<!-- EXAMPLE:72 -->
### group - compact-rightAlignment-vertical


**Example #72** | **Variation**: group | **Modifier**: compact-rightAlignment-vertical | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:72 -->

<!-- EXAMPLE:73 -->
### group - rightAlignment - disabled


**Example #73** | **Variation**: group | **Modifier**: rightAlignment | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:73 -->

<!-- EXAMPLE:74 -->
### group - compact-rightAlignment - disabled


**Example #74** | **Variation**: group | **Modifier**: compact-rightAlignment | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-horizontal">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
.button-demo-wrapper-horizontal {
  display: flex;
  width: rem(360px);
  justify-content: space-between;
}
```

<!-- /EXAMPLE:74 -->

<!-- EXAMPLE:75 -->
### group - rightAlignment-vertical - disabled


**Example #75** | **Variation**: group | **Modifier**: rightAlignment-vertical | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:75 -->

<!-- EXAMPLE:76 -->
### group - compact-rightAlignment-vertical - disabled


**Example #76** | **Variation**: group | **Modifier**: compact-rightAlignment-vertical | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="button-demo-wrapper-vertical">
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'secondary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
  <ap-button [btnType]="'primary'" [label]="'Label'" [disabled]="true" [compact]="true" (onClick)="onClickButton($event)"></ap-button>
</div>
```

#### TypeScript

```typescript
onClickButton(event: any) {
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.button-demo-wrapper-vertical {
  display: grid;
  ap-button{
      &:not(:first-child) {
          margin-top: $spacing-4;
      }
  }
}
```

<!-- /EXAMPLE:76 -->

<!-- EXAMPLE:77 -->
### split


**Example #77** | **Variation**: split | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-group-button [buttonData]="buttonData" (onClick)="onClickButton($event)" 
    [expandArialabel]="'choose button action'">
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-group-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:77 -->

<!-- EXAMPLE:78 -->
### split - disabled


**Example #78** | **Variation**: split | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-group-button [disabled]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
    [expandArialabel]="'choose button action'">
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-group-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:78 -->

<!-- EXAMPLE:79 -->
### split - compact


**Example #79** | **Variation**: split | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-group-button [compact]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
    [expandArialabel]="'choose button action'">
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-group-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:79 -->

<!-- EXAMPLE:80 -->
### split - compact - disabled


**Example #80** | **Variation**: split | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-group-button [disabled]="true" [compact]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
    [expandArialabel]="'choose button action'">
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-group-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:80 -->

<!-- EXAMPLE:81 -->
### menu


**Example #81** | **Variation**: menu | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-menu-button [buttonData]="buttonData" (onClick)="onClickButton($event)" 
   >
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:81 -->

<!-- EXAMPLE:82 -->
### menu - disabled


**Example #82** | **Variation**: menu | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-menu-button [disabled]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
   >
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:82 -->

<!-- EXAMPLE:83 -->
### menu - compact


**Example #83** | **Variation**: menu | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-menu-button [compact]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
   >
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:83 -->

<!-- EXAMPLE:84 -->
### menu - complex


**Example #84** | **Variation**: menu | **Modifier**: complex | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
 <div class="button-demo-wrapper">
  <ap-menu-button [buttonData]="buttonData" [complex]= "true" 
  (onClick)="onClickButton($event)">
      <ap-complex-panel>
          <ng-template ngTemplate="header">
              <span class="ap-panel-header-title">Title</span>
              <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
          </ng-template>
          <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
          <ng-template ngTemplate="footer">
              <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
              <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
          </ng-template>
      </ap-complex-panel>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
buttonData = {
  main: {  
    value: "Label", 
  }
};

onClickButton(event: any) {
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:84 -->

<!-- EXAMPLE:85 -->
### menu - compact - disabled


**Example #85** | **Variation**: menu | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { DropdownListItemModule } from '@appkit4/angular-components/dropdown-list-item';
import { ButtonModule } from '@appkit4/angular-components/button'; 
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-menu-button [disabled]="true" [compact]="true" [buttonData]="buttonData" (onClick)="onClickButton($event)" 
   >
    <ap-dropdown-list-item #dropdownListItem *ngFor="let item of buttonData.options" [item]="item" [role]="'menuitem'" [ariaLabel]="item.label" [addAriaCurrent]="false"></ap-dropdown-list-item>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
buttonData = {
  main: {  
    value: "Label", 
  },
  options: [
    { label: "Default", disabled: false },
    { label: "Hover", disabled: false },
    { label: "Selected", disabled: false },
    { label: "Disabled", disabled: true }
  ]
};

onClickButton(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:85 -->

<!-- EXAMPLE:86 -->
### menu - complex - disabled


**Example #86** | **Variation**: menu | **Modifier**: complex | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
 <div class="button-demo-wrapper">
  <ap-menu-button [buttonData]="buttonData" [disabled]="true" [complex]= "true" 
  (onClick)="onClickButton($event)">
      <ap-complex-panel>
          <ng-template ngTemplate="header">
              <span class="ap-panel-header-title">Title</span>
              <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
          </ng-template>
          <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
          <ng-template ngTemplate="footer">
              <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
              <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
          </ng-template>
      </ap-complex-panel>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
buttonData = {
  main: {  
    value: "Label", 
  }
};

onClickButton(event: any) {
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:86 -->

<!-- EXAMPLE:87 -->
### menu - compact-complex - disabled


**Example #87** | **Variation**: menu | **Modifier**: compact-complex | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
 <div class="button-demo-wrapper">
  <ap-menu-button [buttonData]="buttonData" [disabled]="true" [compact]="true" [complex]= "true" 
  (onClick)="onClickButton($event)">
      <ap-complex-panel>
          <ng-template ngTemplate="header">
              <span class="ap-panel-header-title">Title</span>
              <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
          </ng-template>
          <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
          <ng-template ngTemplate="footer">
              <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
              <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
          </ng-template>
      </ap-complex-panel>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
buttonData = {
  main: {  
    value: "Label", 
  }
};

onClickButton(event: any) {
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:87 -->

<!-- EXAMPLE:88 -->
### menu - compact-complex


**Example #88** | **Variation**: menu | **Modifier**: compact-complex | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
 <div class="button-demo-wrapper">
  <ap-menu-button [buttonData]="buttonData" [compact]="true" [complex]= "true" 
  (onClick)="onClickButton($event)">
      <ap-complex-panel>
          <ng-template ngTemplate="header">
              <span class="ap-panel-header-title">Title</span>
              <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
          </ng-template>
          <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
          <ng-template ngTemplate="footer">
              <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
              <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
          </ng-template>
      </ap-complex-panel>
  </ap-menu-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
buttonData = {
  main: {  
    value: "Label", 
  }
};

onClickButton(event: any) {
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:88 -->

<!-- EXAMPLE:89 -->
### icon-only-menu


**Example #89** | **Variation**: icon-only-menu | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-panel-button [icon]="'circle-more-outline'" [btnType]="'primary'">
      <ap-complex-panel>
        <ng-template ngTemplate="header">
            <span class="ap-panel-header-title">Title</span>
            <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
        </ng-template>
        <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
        <ng-template ngTemplate="footer">
            <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
            <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
        </ng-template>
    </ap-complex-panel>
  </ap-panel-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:89 -->

<!-- EXAMPLE:90 -->
### icon-only-menu - disabled


**Example #90** | **Variation**: icon-only-menu | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-panel-button [icon]="'circle-more-outline'" [btnType]="'primary'" [disabled]="true">
      <ap-complex-panel>
        <ng-template ngTemplate="header">
            <span class="ap-panel-header-title">Title</span>
            <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
        </ng-template>
        <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
        <ng-template ngTemplate="footer">
            <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
            <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
        </ng-template>
    </ap-complex-panel>
  </ap-panel-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:90 -->

<!-- EXAMPLE:91 -->
### icon-only-menu - compact


**Example #91** | **Variation**: icon-only-menu | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-panel-button [icon]="'circle-more-outline'" [btnType]="'primary'" [compact]="true">
      <ap-complex-panel>
        <ng-template ngTemplate="header">
            <span class="ap-panel-header-title">Title</span>
            <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
        </ng-template>
        <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
        <ng-template ngTemplate="footer">
            <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
            <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
        </ng-template>
    </ap-complex-panel>
  </ap-panel-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:91 -->

<!-- EXAMPLE:92 -->
### icon-only-menu - compact - disabled


**Example #92** | **Variation**: icon-only-menu | **Modifier**: compact | **State**: disabled

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { ComplexPanelModule } from '@appkit4/angular-components/complex-panel';
```

#### HTML Template

```html
<div class="button-demo-wrapper">
  <ap-panel-button [icon]="'circle-more-outline'" [btnType]="'primary'" [disabled]="true" [compact]="true">
      <ap-complex-panel>
        <ng-template ngTemplate="header">
            <span class="ap-panel-header-title">Title</span>
            <a class="ap-link" [attr.href]="linkContent.href" [target]="linkContent.target ? linkContent.target :'_blank'">{{linkContent.name}}</a>
        </ng-template>
        <div>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>
        <ng-template ngTemplate="footer">
            <ap-button [btnType]="'secondary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
            <ap-button [btnType]="'primary'" [label]="'Action'" [styleClass]="'panel-button'"></ap-button>
        </ng-template>
    </ap-complex-panel>
  </ap-panel-button>
</div>
```

#### TypeScript

```typescript
linkContent = { name: 'Clear All', href: '#', target: '_blank'}
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

.ap-panel-header-title{
  font-weight: $font-weight-2;
  font-size: $typography-text-size-5;
  line-height: $typography-line-height-3;
  letter-spacing: $letter-spacing-1;
  color: $color-text-heading;
}
:host ::ng-deep{
  .ap-button.panel-button:not([disabled]){
      padding: $spacing-3 $spacing-5;

      &:last-child{
          margin-left: $spacing-3;
      }
  }
}

.button-demo-wrapper {
  display: flex;
  justify-content: center;
}
```

<!-- /EXAMPLE:92 -->

<!-- EXAMPLE:93 -->
### add


**Example #93** | **Variation**: add | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [icon]="'plus-outline'" [add]="true" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:93 -->

<!-- EXAMPLE:94 -->
### add - compact


**Example #94** | **Variation**: add | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [icon]="'plus-outline'" [compact]="true" [add]="true" (onClick)="onClickButton($event)"></ap-button>
```

#### TypeScript

```typescript
onClickButton(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:94 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Single

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| btnType | string: 'primary'\|'secondary'\|'tertiary'\|'negative'\|'text' | Type of the button. | 'primary' | 4.0.0 |
| label | string | Text of the button. | '' | 4.0.0 |
| disabled | boolean | If it is true, it specifies that the button should be disabled. | false | 4.0.0 |
| icon | string | Name of the icon. | '' | 4.0.0 |
| loading | boolean | Whether the button can be triggered with loading state. | false | 4.0.0 |
| isLoading | boolean | Whether the button will be in the loading state by default, only works when 'loading' is 'true'. | false | 4.0.0 |
| add | boolean | Whether the button is an add button. | false | 4.0.0 |
| type | string:'submit'\|'reset'\|'button' | The type aria attribute of HTML button element semantics, applies to the main button. | 'submit' | 4.0.0 |
| role | string:'button'\|'link' | The role aria attribute of HTML semantics, applies to the main button. | 'button' | 4.0.0 |
| compact | boolean | When specified, the button displays in the compact size. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| ariaDescribedby | string | The aria-describedby of the button element. | '' | 4.4.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the button is clicked. | - | 4.0.0 |
| onFocus | EventEmitter<Event> | Callback to invoke when the button is focused. | - | 4.0.0 |
| onBlur | EventEmitter<Event> | Callback to invoke when the button is blurred. | - | 4.0.0 |
| ariaLabel | string | The value of aria-label of the button. | - | 4.6.1 |

### Group

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| disabled | boolean | If it is true, it specifies that the dropdown of the group button should be disabled. | false | 4.0.0 |
| buttonId | string | The id string of group button dialog or menu button dialog. | 'group'+ 'dropdown'/'menu'+'dropdown' | 4.0.0 |
| buttonData | array | The button items of group button. The value of 'main' property will be set to the label of main button. | - | 4.0.0 |
| type | string:'submit'\|'reset'\|'button' | The type aria attribute of HTML button element semantics, apply to the main button. | 'submit' | 4.0.0 |
| role | string:'button'\|'link' | The role aria attribute of HTML semantics, only apply to the main button of group button. | 'button' | 4.0.0 |
| compact | boolean | When specified, the button displays in the compact size. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| ariaDescribedby | string | The aria-describedby of the button element. | '' | 4.4.0 |
| expandAriaLabel | string | Defines a string that label the group type button expand icon for accessibility. | '' | 4.17.0 |
| complex | boolean | The type of menu button dropdown box. | false | 4.4.0 |
| onClick | EventEmitter<{ item: any, event: Event }> | Callback to invoke when the main button or a button of dropdown is clicked. Event properties: • item: Item could be main or one of options in buttonData, depending on the item which is clicked. • event: Event. | - | 4.0.0 |
| onFocus | EventEmitter<{ item: any, event: Event }> | Callback to invoke when the main button or a button of dropdown is focused. Event properties: • item: Item could be main or one of options in buttonData, depending on the item which is focused. • event: Event. | - | 4.0.0 |
| onBlur | EventEmitter<{ item: any, event: Event }> | Callback to invoke when the main button or a button of dropdown is blurred. Event properties: • item: Item could be main or one of options in buttonData, depending on the item which is blurred. • event: Event. | - | 4.0.0 |
| onClose | EventEmitter<{ item: any, event: Event }> | Callback to invoke when the dialog been closed. | - | 4.0.0 |
| ariaLabel | string | The value of aria-label of the button. | - | 4.6.1 |

### PanelButton

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| disabled | boolean | If it is true, it specifies that the dropdown of the group button should be disabled. | false | 4.7.0 |
| buttonId | string | The id string of icon only menu button dialog. | 'panel'+ 'dropdown' | 4.7.0 |
| type | string:'submit'\|'reset'\|'button' | The type aria attribute of HTML button element semantics, apply to the main button. | 'submit' | 4.7.0 |
| btnType | string: 'primary'\|'secondary'\|'tertiary'\|'negative'\|'text' | Type of the button. | 'primary' | 4.7.0 |
| role | string:'button'\|'link' | The role aria attribute of HTML semantics, only apply to the main button. | 'button' | 4.7.0 |
| compact | boolean | When specified, the button displays in the compact size. | false | 4.7.0 |
| style | string | The inline style of the component. | '' | 4.7.0 |
| styleClass | string | The style class names of the component. | '' | 4.7.0 |
| ariaDescribedby | string | The aria-describedby of the button element. | '' | 4.7.0 |
| ariaLabel | string | The value of aria-label of the button. | - | 4.7.0 |
| onClick | EventEmitter<{ event: Event }> | Callback to invoke when the main button is clicked. Event properties: • event: Event. | - | 4.7.0 |
| onFocus | EventEmitter<{ event: Event }> | Callback to invoke when the main button is focused. Event properties: • event: Event. | - | 4.7.0 |
| onBlur | EventEmitter<{ event: Event }> | Callback to invoke when the main button is blurred. Event properties: • event: Event. | - | 4.7.0 |
| onClose | EventEmitter<{ event: Event }> | Callback to invoke when the dialog been closed. | - | 4.16.0 |


<!-- /SECTION:properties -->