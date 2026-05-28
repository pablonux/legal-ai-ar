---
component: badge
framework: angular
---

# Badge Component

## Overview

AppKit Badge component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use badge component: 

- To describe a status within an application.
- To quantify, categorize or organize elements.
- To highlight numeric value.

### Anatomy

1.** Container:** Defines the shape of the component.

2.** Label:** Label that usually indicates status, categories or descriptor.

### Variants

#### Default

Use when more prominence is needed.

#### Compact

Smaller size to accommodate for tighter spaces like tables.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-107474%26viewport%3D-143%252C-852%252C0.55%26t%3DA5ZR5MO35ivBeNLQ-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

-  Use a mix of Inline, Outlined or Neutral badges in instances where they may have too much emphasis on the screen like long tables with lots of badges. 
-  Use default badges for a general status or state. 
-  Use status badges to represent positive status or state. 
-  The placement of badges depends on the use case but we recommend that they be placed near the element they modify or inform about. 
-  When placing text near each other, the ideal distance is between 8-12px. 
-  The separation between badges within a group or badges of the same type must be 4px. 
-  The primary color of the default badges changes according to the theme. 
-  Use tags instead when the goal is to group or categorize content. 
-  Use tags instead when the goal is to display filter results. 
-  Use tags instead when the attributes can be dismissed or closed. 
-  Be aware of correct status color usage. 
-  Use short labels.

### Behavior

When a label is very long, it should be truncated and include a tooltip to expose the full label on hover.

### Accessibility

- Badges need to have a simple, short, and unique label
- Do not use color alone to differentiate statuses
- Check for 4.5:1 color contrast for text on badge color

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 48


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | inline - filled - default | `section: "example:1"` |
| 2 | inline - filled - neutral | `section: "example:2"` |
| 3 | inline - filled - error | `section: "example:3"` |
| 4 | inline - filled - warning | `section: "example:4"` |
| 5 | inline - filled - success | `section: "example:5"` |
| 6 | inline - outlined - default | `section: "example:6"` |
| 7 | inline - outlined - neutral | `section: "example:7"` |
| 8 | inline - outlined - error | `section: "example:8"` |
| 9 | inline - outlined - warning | `section: "example:9"` |
| 10 | inline - outlined - success | `section: "example:10"` |
| 11 | inline - filled-group | `section: "example:11"` |
| 12 | inline - outlined-group | `section: "example:12"` |
| 13 | contained - filled - default | `section: "example:13"` |
| 14 | contained - filled - neutral | `section: "example:14"` |
| 15 | contained - filled - error | `section: "example:15"` |
| 16 | contained - filled - warning | `section: "example:16"` |
| 17 | contained - filled - success | `section: "example:17"` |
| 18 | contained - outlined - default | `section: "example:18"` |
| 19 | contained - outlined - neutral | `section: "example:19"` |
| 20 | contained - outlined - error | `section: "example:20"` |
| 21 | contained - outlined - warning | `section: "example:21"` |
| 22 | contained - outlined - success | `section: "example:22"` |
| 23 | contained - filled-group | `section: "example:23"` |
| 24 | contained - outlined-group | `section: "example:24"` |
| 25 | inline - outlined-icon - default | `section: "example:25"` |
| 26 | inline - outlined-icon - neutral | `section: "example:26"` |
| 27 | inline - outlined-icon - error | `section: "example:27"` |
| 28 | inline - outlined-icon - warning | `section: "example:28"` |
| 29 | inline - outlined-icon - success | `section: "example:29"` |
| 30 | inline - filled-icon - default | `section: "example:30"` |
| 31 | inline - filled-icon - neutral | `section: "example:31"` |
| 32 | inline - filled-icon - error | `section: "example:32"` |
| 33 | inline - filled-icon - warning | `section: "example:33"` |
| 34 | inline - filled-icon - success | `section: "example:34"` |
| 35 | inline - outlined-icon-group | `section: "example:35"` |
| 36 | inline - filled-icon-group | `section: "example:36"` |
| 37 | contained - outlined-icon - default | `section: "example:37"` |
| 38 | contained - outlined-icon - neutral | `section: "example:38"` |
| 39 | contained - outlined-icon - error | `section: "example:39"` |
| 40 | contained - outlined-icon - warning | `section: "example:40"` |
| 41 | contained - outlined-icon - success | `section: "example:41"` |
| 42 | contained - filled-icon - default | `section: "example:42"` |
| 43 | contained - filled-icon - neutral | `section: "example:43"` |
| 44 | contained - filled-icon - error | `section: "example:44"` |
| 45 | contained - filled-icon - warning | `section: "example:45"` |
| 46 | contained - filled-icon - success | `section: "example:46"` |
| 47 | contained - outlined-icon-group | `section: "example:47"` |
| 48 | contained - filled-icon-group | `section: "example:48"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### inline - filled - default


**Example #1** | **Variation**: inline | **Modifier**: filled | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### inline - filled - neutral


**Example #2** | **Variation**: inline | **Modifier**: filled | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### inline - filled - error


**Example #3** | **Variation**: inline | **Modifier**: filled | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### inline - filled - warning


**Example #4** | **Variation**: inline | **Modifier**: filled | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### inline - filled - success


**Example #5** | **Variation**: inline | **Modifier**: filled | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### inline - outlined - default


**Example #6** | **Variation**: inline | **Modifier**: outlined | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### inline - outlined - neutral


**Example #7** | **Variation**: inline | **Modifier**: outlined | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### inline - outlined - error


**Example #8** | **Variation**: inline | **Modifier**: outlined | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### inline - outlined - warning


**Example #9** | **Variation**: inline | **Modifier**: outlined | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### inline - outlined - success


**Example #10** | **Variation**: inline | **Modifier**: outlined | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### inline - filled-group


**Example #11** | **Variation**: inline | **Modifier**: filled-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info" size="undefined" icon=""></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### inline - outlined-group


**Example #12** | **Variation**: inline | **Modifier**: outlined-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info-outlined" size="undefined" icon=""></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### contained - filled - default


**Example #13** | **Variation**: contained | **Modifier**: filled | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### contained - filled - neutral


**Example #14** | **Variation**: contained | **Modifier**: filled | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### contained - filled - error


**Example #15** | **Variation**: contained | **Modifier**: filled | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### contained - filled - warning


**Example #16** | **Variation**: contained | **Modifier**: filled | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### contained - filled - success


**Example #17** | **Variation**: contained | **Modifier**: filled | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### contained - outlined - default


**Example #18** | **Variation**: contained | **Modifier**: outlined | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### contained - outlined - neutral


**Example #19** | **Variation**: contained | **Modifier**: outlined | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### contained - outlined - error


**Example #20** | **Variation**: contained | **Modifier**: outlined | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### contained - outlined - warning


**Example #21** | **Variation**: contained | **Modifier**: outlined | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### contained - outlined - success


**Example #22** | **Variation**: contained | **Modifier**: outlined | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success-outlined" size="undefined" icon=""></ap-badge>
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### contained - filled-group


**Example #23** | **Variation**: contained | **Modifier**: filled-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info" size="undefined" icon=""></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### contained - outlined-group


**Example #24** | **Variation**: contained | **Modifier**: outlined-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success-outlined" size="undefined" icon=""></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info-outlined" size="undefined" icon=""></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### inline - outlined-icon - default


**Example #25** | **Variation**: inline | **Modifier**: outlined-icon | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### inline - outlined-icon - neutral


**Example #26** | **Variation**: inline | **Modifier**: outlined-icon | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### inline - outlined-icon - error


**Example #27** | **Variation**: inline | **Modifier**: outlined-icon | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### inline - outlined-icon - warning


**Example #28** | **Variation**: inline | **Modifier**: outlined-icon | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### inline - outlined-icon - success


**Example #29** | **Variation**: inline | **Modifier**: outlined-icon | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### inline - filled-icon - default


**Example #30** | **Variation**: inline | **Modifier**: filled-icon | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### inline - filled-icon - neutral


**Example #31** | **Variation**: inline | **Modifier**: filled-icon | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### inline - filled-icon - error


**Example #32** | **Variation**: inline | **Modifier**: filled-icon | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### inline - filled-icon - warning


**Example #33** | **Variation**: inline | **Modifier**: filled-icon | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### inline - filled-icon - success


**Example #34** | **Variation**: inline | **Modifier**: filled-icon | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### inline - outlined-icon-group


**Example #35** | **Variation**: inline | **Modifier**: outlined-icon-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### inline - filled-icon-group


**Example #36** | **Variation**: inline | **Modifier**: filled-icon-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info" size="undefined" icon="art-themes-outline"></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:36 -->

<!-- EXAMPLE:37 -->
### contained - outlined-icon - default


**Example #37** | **Variation**: contained | **Modifier**: outlined-icon | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:37 -->

<!-- EXAMPLE:38 -->
### contained - outlined-icon - neutral


**Example #38** | **Variation**: contained | **Modifier**: outlined-icon | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:38 -->

<!-- EXAMPLE:39 -->
### contained - outlined-icon - error


**Example #39** | **Variation**: contained | **Modifier**: outlined-icon | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:39 -->

<!-- EXAMPLE:40 -->
### contained - outlined-icon - warning


**Example #40** | **Variation**: contained | **Modifier**: outlined-icon | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:40 -->

<!-- EXAMPLE:41 -->
### contained - outlined-icon - success


**Example #41** | **Variation**: contained | **Modifier**: outlined-icon | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:41 -->

<!-- EXAMPLE:42 -->
### contained - filled-icon - default


**Example #42** | **Variation**: contained | **Modifier**: filled-icon | **State**: default

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Miami" type="primary" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:42 -->

<!-- EXAMPLE:43 -->
### contained - filled-icon - neutral


**Example #43** | **Variation**: contained | **Modifier**: filled-icon | **State**: neutral

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Dallas" type="info" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:43 -->

<!-- EXAMPLE:44 -->
### contained - filled-icon - error


**Example #44** | **Variation**: contained | **Modifier**: filled-icon | **State**: error

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="24" type="danger" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:44 -->

<!-- EXAMPLE:45 -->
### contained - filled-icon - warning


**Example #45** | **Variation**: contained | **Modifier**: filled-icon | **State**: warning

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Denver" type="warning" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:45 -->

<!-- EXAMPLE:46 -->
### contained - filled-icon - success


**Example #46** | **Variation**: contained | **Modifier**: filled-icon | **State**: success

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<ap-badge value="Milwaukee" type="success" size="undefined" icon="art-themes-outline"></ap-badge>
```

<!-- /EXAMPLE:46 -->

<!-- EXAMPLE:47 -->
### contained - outlined-icon-group


**Example #47** | **Variation**: contained | **Modifier**: outlined-icon-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info-outlined" size="undefined" icon="art-themes-outline"></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:47 -->

<!-- EXAMPLE:48 -->
### contained - filled-icon-group


**Example #48** | **Variation**: contained | **Modifier**: filled-icon-group | **State**: None

#### Module Import

```typescript
import { BadgeModule } from '@appkit4/angular-components/badge';
```

#### HTML Template

```html
<div>
  <ap-badge class="badge-item" value="Miami" type="primary" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="24" type="danger" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Denver" type="warning" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Milwaukee" type="success" size="undefined" icon="art-themes-outline"></ap-badge>
  <ap-badge class="badge-item" value="Dallas" type="info" size="undefined" icon="art-themes-outline"></ap-badge>
</div>
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

.badge-item + .badge-item {
  margin-left: $spacing-2;
}
```

<!-- /EXAMPLE:48 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| value | string | Value to display in the badge. | '' | 4.0.0 |
| backgroundColor | string | Background color of the badge. | Default value is the primary color. | 4.0.0 |
| fontColor | string | Font color of the badge. | '\#ffffff' | 4.0.0 |
| size | string:'small'\|'large' | Size of the badge. | 'small' | 4.0.0 |
| marginLeft | string | Margin left of the badge. | '0' | 4.0.0 |
| type | string:'primary'\|'primary-outlined'\|'danger'\|'danger-outlined'\|'warning'\|'warning-outlined'\|'success'\|'success-outlined'\|'info'\|'info-outlined' | Type of the badge. | 'primary' | 4.0.0 |
| icon | string | Name of the icon. | '' | 4.14.0 |
| style | string | Inline style of the component. | '' | 4.5.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| badgeRole | string: 'button'\|'link' | The role of the badge. | - | 4.0.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the badge is clicked, only works when the role is 'button' or 'link'. | - | 4.0.0 |


<!-- /SECTION:properties -->