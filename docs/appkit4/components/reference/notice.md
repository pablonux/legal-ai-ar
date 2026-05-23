---
component: notice
framework: angular
---

# Notice Component

## Overview

AppKit Notice component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use notice component:

- Whenever the user needs to be made aware of important information.
- It should be placed in a prominent location on the screen, such as above or below the main content area.
- Unlike notifications, the notice component should be used for non-actionable general announcements.
- When it is necessary to offer a peripheral message that requires immediate actions, the “Notification” component should be used.

### Anatomy

**1. Icon:** An optional icon that can be used to add visual context to the message.

**2. Title:** A short, descriptive title that summarizes the message.

**3. Body:** A more detailed description of the message or information.

**4. Link:** Option to embed link.

**5. Close button:** An optional button that allows the user to dismiss the notice.

**6. Container:** Defines the boundaries of the Notice component.

### Variants

None

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-95205%26viewport%3D1516%252C-29775%252C0.43%26t%3DqydgvtpkaIE8zEZ8-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- A notice should be used to communicate something official, but not specific to the user, such as a planned outage or event.
- The display of a notice should not be triggered by an action by the user. They are intended to inform the user, not alert them.
- Make lists scrollable.Notice components must be placed relative to the area of the page or application that the message relates to.

#### How not to use

- Do use a clear and concise title that accurately summarizes the message.
- Do include a body that provides more detailed information or instructions.
- Do use appropriate colors and icons to convey the type of message.
- Do not include too much information in the notice, as it can overwhelm the user.
- Do not use a notice to indicate system errors or change in status.
- Do not use a notice for things that require an action by the user.

### Behavior

- The notice should be dismissible by the user by clicking a close button.
- The notice should be persistent and remain visible until dismissed by the user.
- The notice should be accessible via keyboard and screen reader, with appropriate ARIA attributes.

### Accessibility

- Use appropriate ARIA roles and attributes to convey the type of message to screen readers.
- Use a clear and concise title that accurately summarizes the message.
- Provide an accessible way for the user to dismiss the notice.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 8


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |
| 2 | title | `section: "example:2"` |
| 3 | hyperlink | `section: "example:3"` |
| 4 | icon | `section: "example:4"` |
| 5 | title-hyperlink | `section: "example:5"` |
| 6 | title-hyperlink-icon | `section: "example:6"` |
| 7 | title-icon | `section: "example:7"` |
| 8 | hyperlink-icon | `section: "example:8"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = '';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = '';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: '',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### title


**Example #2** | **Variation**: None | **Modifier**: title | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = 'Lorem ipsum dolor sit';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = '';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: '',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### hyperlink


**Example #3** | **Variation**: None | **Modifier**: hyperlink | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = '';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = 'Lorem more';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: '',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### icon


**Example #4** | **Variation**: None | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = '';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = '';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: 'icon-placeholder-outline',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### title-hyperlink


**Example #5** | **Variation**: None | **Modifier**: title-hyperlink | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = 'Lorem ipsum dolor sit';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = 'Lorem more';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: '',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### title-hyperlink-icon


**Example #6** | **Variation**: None | **Modifier**: title-hyperlink-icon | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = 'Lorem ipsum dolor sit';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = 'Lorem more';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: 'icon-placeholder-outline',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### title-icon


**Example #7** | **Variation**: None | **Modifier**: title-icon | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = 'Lorem ipsum dolor sit';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = '';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: 'icon-placeholder-outline',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### hyperlink-icon


**Example #8** | **Variation**: None | **Modifier**: hyperlink-icon | **State**: None

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification [position]="position" [id]="id" [isNotice]="true" [closeable]="false" [animation]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';
constructor(protected _notificationSvc: NotificationService) {}
position: string = 'static';
id: string = 'notice';

ngAfterViewInit() {
  this.createNotification();
}
createNotification(): void {
  const title = '';
  const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
  const hyperLink = 'Lorem more';
  const hyperLinkHref = '';
  this._notificationSvc
    .show(title, message, hyperLink, hyperLinkHref,{
      duration: 0,
      id: this.id,
      clickToClose: false,
      showClose: true,
      icon: 'icon-placeholder-outline',
    })
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

 .nty-demo-wrapper {
   display: grid;
   grid-template-columns: repeat(4, auto);

   > * {
       margin: $spacing-7;
   }
 }
```

<!-- /EXAMPLE:8 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-notification

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| animation | boolean | When specified, the notification will has an animation. | true | 4.0.0 |
| id | string | The unique id which is required to identify each notification instance. | - | 4.0.0 |
| isNotice | boolean | When specified, applys the style of notice | 'false' | 4.0.0 |
| position | string: 'topLeft', 'bottomLeft', 'topRight', 'bottomRight', 'topCenter', 'bottomCenter', 'center', 'static' | Position of the component. | 'static' | 4.0.0 |
| closeable | boolean | When specified, hides the notice when clicking on the close icon. | true | 4.0.0 |
| onClose | EventEmitter<Event \| NotificationListComponent> | The callback function triggered when notification closes. | - | 4.0.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the notification is clicked. | - | 4.0.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |

### Notification Service Config

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| id | string | The unique id which is required to identify each notification instance. This should be the same as 'id' property of &lt;ap-notification&gt;&lt;/ap-notification&gt;. | - | 4.0.0 |
| title | string | Title of the notification. | - | 4.0.0 |
| message | string | Content of the notification. | - | 4.0.0 |
| hyperLink | string | Hyperlink of the notification. | - | 4.0.0 |
| duration | number | Duration (milliseconds), the notificaiton will not be hidden if it is 0. Duration will not be supported in static mode and static-header mode. | 0 | 4.0.0 |
| clickToClose | boolean | When specified, hides the notification on click. | false | 4.0.0 |
| showClose | boolean | When specified, shows the close icon. | false | 4.0.0 |
| icon | string | The class name of the icon | - | 4.0.0 |
| hyperLinkHref | string | Hyperlink href of the notification. | - | 4.6.0 |


<!-- /SECTION:properties -->