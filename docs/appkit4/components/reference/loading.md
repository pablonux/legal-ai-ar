---
component: loading
framework: angular
---

# Loading Component

## Overview

AppKit Loading component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use the Loading component:

- To indicate to the user that something is currently being loaded or processed.
- When loading a page.
- When data is being fetched from an API.
- Expose progress of an action.

### Anatomy

1. **Track:** Total progress indicator.

2. **Progress:** The current progress is shown on a linear or circular track.

### Variants

#### Linear:

Linear progression variant.

#### Circular:

Spinner variant that indicates loading.

#### *Indeterminate:

Use to indicate loading state when the expected progression time is unknown.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-92914%26viewport%3D211%252C-23621%252C0.4%26t%3DoA0hCMJyNYuLCgK7-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- A loading bar/spinner should be displayed anytime processing time or load time within an application exceeds 3 seconds.
- Try to keep the loading state brief, to prevent users from getting frustrated.
- Make sure the loading state is clearly visible and distinguishable from other content on the page.
- Expose the linear loading indicator to the top of the content area where the progress is taking place.
- Expose the circular loading indicator at the topmost level of the application. An overlay can be used to guarantee proper visibility.

### Behavior

- Show loading state as soon as possible after a user action has triggered it.

### Accessibility

- role="alert" and aria-live="assertive" to ensure screen reader users are provided with information.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 8


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | linear | `section: "example:1"` |
| 2 | linear - indeterminate | `section: "example:2"` |
| 3 | linear - compact | `section: "example:3"` |
| 4 | linear - indeterminate-compact | `section: "example:4"` |
| 5 | circular | `section: "example:5"` |
| 6 | circular - indeterminate | `section: "example:6"` |
| 7 | circular - compact | `section: "example:7"` |
| 8 | circular - indeterminate-compact | `section: "example:8"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### linear


**Example #1** | **Variation**: linear | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = false;
compact = false;
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### linear - indeterminate


**Example #2** | **Variation**: linear | **Modifier**: indeterminate | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = true;
compact = false;
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### linear - compact


**Example #3** | **Variation**: linear | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = false;
compact = true;
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### linear - indeterminate-compact


**Example #4** | **Variation**: linear | **Modifier**: indeterminate-compact | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = true;
compact = true;
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### circular


**Example #5** | **Variation**: circular | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [loadingType]="'circular'" [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = false;
compact = false;
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### circular - indeterminate


**Example #6** | **Variation**: circular | **Modifier**: indeterminate | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [loadingType]="'circular'" [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = true;
compact = false;
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### circular - compact


**Example #7** | **Variation**: circular | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [loadingType]="'circular'" [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = false;
compact = true;
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### circular - indeterminate-compact


**Example #8** | **Variation**: circular | **Modifier**: indeterminate-compact | **State**: None

#### Module Import

```typescript
import { LoadingModule} from '@appkit4/angular-components/loading';
```

#### HTML Template

```html
<ap-loading [loadingType]="'circular'" [indeterminate]="indeterminate" [compact]="compact"></ap-loading>
```

#### TypeScript

```typescript
indeterminate = true;
compact = true;
```

<!-- /EXAMPLE:8 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| loadingType | string: 'linear' \| 'circular' | Type of the loading. | 'linear' | 4.0.0 |
| linearWidth | string | Width of the linear loading. | '148px' | 4.0.0 |
| circularWidth | string | Width of the circular loading. | '24px' | 4.0.0 |
| indeterminate | boolean | Animation of the loading. | false | 4.0.0 |
| compact | boolean | Size of the loading. | false | 4.0.0 |
| stopPercent | number | Set circular loading progress value. For linear loading, it is supported from 4.6.0. | 75 | 4.0.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component | '' | 4.0.0 |


<!-- /SECTION:properties -->
