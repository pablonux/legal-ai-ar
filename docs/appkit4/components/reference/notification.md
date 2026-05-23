**Note:** This component documentation is large (114KB). Consider using section parameter (usage, examples, properties) for specific content.

---

---
component: notification
framework: angular
---

# Notification Component

## Overview

AppKit Notification component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Notification:

- Any time important information needs to be displayed to the user.

### Anatomy

**1. Icon:** Represents the type of notification (e.g. error, success, info)

**2. Message:** Conveys the information to the user.

**3. Close button:** Allows the user to dismiss the notification.

**4. Container:** Defines the boundaries of the Notification component.

### Variants

#### Alert:

Use to display alerts triggered based on specific events, such as a successful action or an error message.

#### Global notification:

Use for high-attention level notifications.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-95782%26viewport%3D1511%252C-35572%252C0.5%26t%3Dfbeu0fltv4aMf4is-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Notifications should be used any time important information needs to be displayed to the user.
- Notifications are triggered by something such as a system process. user, not alert them.
- Make the Notification component dismissible.
- Limit the amount of text in the Message to keep it concise.

#### How not to use

- Don't overload the user with too many notifications.

### Behavior

- The Notification component should appear and disappear smoothly, without interrupting the user's workflow.
- The Close button should be prominently displayed and easily accessible.
- The Notification component should be dismissible by clicking the close button.
- Global notification component should always be placed at the top of the screen and at the topmost level.

### Accessibility

- Use aria-live="polite" for messages to be queued/announced when current task is complete.
- Use aria-live="assertive" for notifications to be announced immediately without receiving focus unless user needs to interact with modal.
- If interaction is required within the component, it must receive keyboard and screenreader focus.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 36


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | alert - default | `section: "example:1"` |
| 2 | alert - title - default | `section: "example:2"` |
| 3 | alert - hyperlink - default | `section: "example:3"` |
| 4 | alert - icon - default | `section: "example:4"` |
| 5 | alert - title-hyperlink - default | `section: "example:5"` |
| 6 | alert - title-icon - default | `section: "example:6"` |
| 7 | alert - hyperlink-icon - default | `section: "example:7"` |
| 8 | alert - title-hyperlink-icon - default | `section: "example:8"` |
| 9 | alert - icon - error | `section: "example:9"` |
| 10 | alert - title-icon - error | `section: "example:10"` |
| 11 | alert - hyperlink-icon - error | `section: "example:11"` |
| 12 | alert - title-hyperlink-icon - error | `section: "example:12"` |
| 13 | alert - icon - warning | `section: "example:13"` |
| 14 | alert - title-icon - warning | `section: "example:14"` |
| 15 | alert - hyperlink-icon - warning | `section: "example:15"` |
| 16 | alert - title-hyperlink-icon - warning | `section: "example:16"` |
| 17 | alert - icon - success | `section: "example:17"` |
| 18 | alert - title-icon - success | `section: "example:18"` |
| 19 | alert - hyperlink-icon - success | `section: "example:19"` |
| 20 | alert - title-hyperlink-icon - success | `section: "example:20"` |
| 21 | global-notification - icon - default | `section: "example:21"` |
| 22 | global-notification - icon-timed - default | `section: "example:22"` |
| 23 | global-notification - icon-expandable - default | `section: "example:23"` |
| 24 | global-notification - icon-timed-expandable - default | `section: "example:24"` |
| 25 | global-notification - icon - error | `section: "example:25"` |
| 26 | global-notification - icon-timed - error | `section: "example:26"` |
| 27 | global-notification - icon-expandable - error | `section: "example:27"` |
| 28 | global-notification - icon-timed-expandable - error | `section: "example:28"` |
| 29 | global-notification - icon - warning | `section: "example:29"` |
| 30 | global-notification - icon-timed - warning | `section: "example:30"` |
| 31 | global-notification - icon-expandable - warning | `section: "example:31"` |
| 32 | global-notification - icon-timed-expandable - warning | `section: "example:32"` |
| 33 | global-notification - icon - success | `section: "example:33"` |
| 34 | global-notification - icon-timed - success | `section: "example:34"` |
| 35 | global-notification - icon-expandable - success | `section: "example:35"` |
| 36 | global-notification - icon-timed-expandable - success | `section: "example:36"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### alert - default


**Example #1** | **Variation**: alert | **Modifier**: None | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'You have been invited to a meeting.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### alert - title - default


**Example #2** | **Variation**: alert | **Modifier**: title | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Google alert';
    const message = 'You have been invited to a meeting: Review design for notifications.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### alert - hyperlink - default


**Example #3** | **Variation**: alert | **Modifier**: hyperlink | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'You have been invited to a meeting.';
    const hyperLink = 'Add to calendar';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### alert - icon - default


**Example #4** | **Variation**: alert | **Modifier**: icon | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'You have been invited to a meeting.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-calendar-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### alert - title-hyperlink - default


**Example #5** | **Variation**: alert | **Modifier**: title-hyperlink | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Google alert';
    const message = 'You have been invited to a meeting: Review design for notifications.';
    const hyperLink = 'Add to calendar';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### alert - title-icon - default


**Example #6** | **Variation**: alert | **Modifier**: title-icon | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Google alert';
    const message = 'You have been invited to a meeting: Review design for notifications.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-calendar-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### alert - hyperlink-icon - default


**Example #7** | **Variation**: alert | **Modifier**: hyperlink-icon | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'You have been invited to a meeting.';
    const hyperLink = 'Add to calendar';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-calendar-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### alert - title-hyperlink-icon - default


**Example #8** | **Variation**: alert | **Modifier**: title-hyperlink-icon | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Google alert';
    const message = 'You have been invited to a meeting: Review design for notifications.';
    const hyperLink = 'Add to calendar';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-calendar-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### alert - icon - error


**Example #9** | **Variation**: alert | **Modifier**: icon | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### alert - title-icon - error


**Example #10** | **Variation**: alert | **Modifier**: title-icon | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### alert - hyperlink-icon - error


**Example #11** | **Variation**: alert | **Modifier**: hyperlink-icon | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### alert - title-hyperlink-icon - error


**Example #12** | **Variation**: alert | **Modifier**: title-hyperlink-icon | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### alert - icon - warning


**Example #13** | **Variation**: alert | **Modifier**: icon | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### alert - title-icon - warning


**Example #14** | **Variation**: alert | **Modifier**: title-icon | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### alert - hyperlink-icon - warning


**Example #15** | **Variation**: alert | **Modifier**: hyperlink-icon | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### alert - title-hyperlink-icon - warning


**Example #16** | **Variation**: alert | **Modifier**: title-hyperlink-icon | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:16 -->

<!-- EXAMPLE:17 -->
### alert - icon - success


**Example #17** | **Variation**: alert | **Modifier**: icon | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:17 -->

<!-- EXAMPLE:18 -->
### alert - title-icon - success


**Example #18** | **Variation**: alert | **Modifier**: title-icon | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:18 -->

<!-- EXAMPLE:19 -->
### alert - hyperlink-icon - success


**Example #19** | **Variation**: alert | **Modifier**: hyperlink-icon | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:19 -->

<!-- EXAMPLE:20 -->
### alert - title-hyperlink-icon - success


**Example #20** | **Variation**: alert | **Modifier**: title-hyperlink-icon | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false"></ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'static';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = 'Lorem ipsum dolor sit';
    const message = 'Lorem ipsum dolor sit amet, tuso consectetur adipiscing elit.';
    const hyperLink = 'Lorem more';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:20 -->

<!-- EXAMPLE:21 -->
### global-notification - icon - default


**Example #21** | **Variation**: global-notification | **Modifier**: icon | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-circle-warning-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:21 -->

<!-- EXAMPLE:22 -->
### global-notification - icon-timed - default


**Example #22** | **Variation**: global-notification | **Modifier**: icon-timed | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-circle-warning-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:22 -->

<!-- EXAMPLE:23 -->
### global-notification - icon-expandable - default


**Example #23** | **Variation**: global-notification | **Modifier**: icon-expandable | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-circle-warning-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:23 -->

<!-- EXAMPLE:24 -->
### global-notification - icon-timed-expandable - default


**Example #24** | **Variation**: global-notification | **Modifier**: icon-timed-expandable | **State**: default

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'default';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-circle-warning-outline",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:24 -->

<!-- EXAMPLE:25 -->
### global-notification - icon - error


**Example #25** | **Variation**: global-notification | **Modifier**: icon | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:25 -->

<!-- EXAMPLE:26 -->
### global-notification - icon-timed - error


**Example #26** | **Variation**: global-notification | **Modifier**: icon-timed | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:26 -->

<!-- EXAMPLE:27 -->
### global-notification - icon-expandable - error


**Example #27** | **Variation**: global-notification | **Modifier**: icon-expandable | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:27 -->

<!-- EXAMPLE:28 -->
### global-notification - icon-timed-expandable - error


**Example #28** | **Variation**: global-notification | **Modifier**: icon-timed-expandable | **State**: error

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'error';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-error-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:28 -->

<!-- EXAMPLE:29 -->
### global-notification - icon - warning


**Example #29** | **Variation**: global-notification | **Modifier**: icon | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:29 -->

<!-- EXAMPLE:30 -->
### global-notification - icon-timed - warning


**Example #30** | **Variation**: global-notification | **Modifier**: icon-timed | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:30 -->

<!-- EXAMPLE:31 -->
### global-notification - icon-expandable - warning


**Example #31** | **Variation**: global-notification | **Modifier**: icon-expandable | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:31 -->

<!-- EXAMPLE:32 -->
### global-notification - icon-timed-expandable - warning


**Example #32** | **Variation**: global-notification | **Modifier**: icon-timed-expandable | **State**: warning

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'warning';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-warning-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:32 -->

<!-- EXAMPLE:33 -->
### global-notification - icon - success


**Example #33** | **Variation**: global-notification | **Modifier**: icon | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:33 -->

<!-- EXAMPLE:34 -->
### global-notification - icon-timed - success


**Example #34** | **Variation**: global-notification | **Modifier**: icon-timed | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =false;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:34 -->

<!-- EXAMPLE:35 -->
### global-notification - icon-expandable - success


**Example #35** | **Variation**: global-notification | **Modifier**: icon-expandable | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = false;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 0,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:35 -->

<!-- EXAMPLE:36 -->
### global-notification - icon-timed-expandable - success


**Example #36** | **Variation**: global-notification | **Modifier**: icon-timed-expandable | **State**: success

#### Module Import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notification';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
```

#### HTML Template

```html
<ap-notification  [showTimed]="showTimed" [showExpandedIcon]="showExpandedIcon" [status]="status" [animation]="false" [position]="position" [id]="id" [closeable]="false">
    <div class="ap-notification-image-box">
        <span>
          <img src="app/assets/global-notification.svg" alt="content-1">
        </span>
    </div>
    <div class="ap-notification-text-box">
      <span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Praesent placerat ullamcorper metus, eu eleifend dui eleifend.</span>
    </div>
    <div class="ap-notification-button">
      <ap-button [btnType]="'secondary'" [styleClass]="'leftbtn'" [label]="'Label'" ></ap-button>
      <ap-button [btnType]="'primary'" [label]="'Label'"></ap-button> 
    </div>
</ap-notification>
```

#### TypeScript

```typescript
import { NotificationService } from '@appkit4/angular-components/notification';

  constructor(protected _notificationSvc: NotificationService) {}
  position: string = 'topHeader';
  id: string = 'notification1';
  showTimed: boolean = true;
  showExpandedIcon: boolean =true;
  status: 'default' | 'success' | 'warning' | 'error' = 'success';

  ngAfterViewInit() {
    this.createNotification();
  }

  createNotification(): void {
    const title = '';
    const message = 'Lorem ipsun alert with <a class="ap-notification-link" href="#" tabindex="0">link</a>.';
    const hyperLink = '';
    const hyperLinkHref = '';
    this._notificationSvc
      .show(
        title,
        message,
        hyperLink,
        hyperLinkHref,
        {
            duration: 5000,
            id: this.id,
            clickToClose: false,
            showClose: true,
            icon: "icon-success-fill",
        },
        this.showTimed,
        this.showExpandedIcon
      )
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

 ::ng-deep{
   .ap-notification-image-box{
     width: rem(284px);
     height: rem(191px);
     border-radius: $border-2;
     background-color: $neutral-02;
   }

   .ap-notification-text-box{
     margin:$spacing-6 0;
     width: rem(417px);
     height: $height-7;
     font-size: $typography-text-size-3;
     line-height: $typography-line-height-3;
     letter-spacing: $letter-spacing-1;
     text-align: center;
     color: $neutral-14;
   }

   .ap-notification-button{
     display: inline-flex;
     align-items: center;
     justify-content: center;

     .ap-button-secondary.leftbtn{
       margin-right: $spacing-3;
     } 
   }
 }
```

<!-- /EXAMPLE:36 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-notification

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| animation | boolean | When specified, the notification will has an animation. | true | 4.0.0 |
| id | string | The unique id which is required to identify each notification instance. | - | 4.0.0 |
| position | string: 'topLeft', 'bottomLeft', 'topRight', 'bottomRight', 'topCenter', 'bottomCenter', 'center', 'static', 'topHeader' | Position of the component. Display a global notification when specify position "topHeader". | 'static' | 4.0.0 |
| onClose | EventEmitter<Event \| NotificationListComponent> | The callback function triggered when notification closes. | - | 4.0.0 |
| onClick | EventEmitter<Event> | The callback function triggered when clicking the notification. | - | 4.0.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component | '' | 4.0.0 |
| maxNotificationsCount | number | The max amount of notification instances can be created,no limit if value is 0 | 0 | 4.0.0 |
| closeable | boolean | When specified, hides the notice when clicking on the close icon. | true | 4.0.0 |
| showTimed | boolean | Whether to show countdown circle timer, ONLY used for Global notification (position = topHeader). When showTimer set to true, duration is mandatory. | false | 4.0.0 |
| showExpandedIcon | boolean | Whether to show expanded icon, ONLY used for Global notification (position = topHeader). | false | 4.0.0 |
| status | string: 'default', 'success', 'warning', 'error' | The status for notifications. | 'default' | 4.0.0 |
| close | (notification: Notification) => void | Close one notification. | - | 4.6.0 |
| closeAllNotifications | () => void | Close all notifications. | - | 4.6.0 |

### Notification Service Config

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| id | string | The unique id which is required to identify each notification instance. This should be the same as 'id' property of &lt;ap-notification&gt;&lt;/ap-notification&gt;. | - | 4.0.0 |
| title | string | Title of the notification. | - | 4.0.0 |
| message | string | Content of the notification. | - | 4.0.0 |
| hyperLink | string | Hyperlink of the notification. | - | 4.0.0 |
| duration | number | Duration (milliseconds) before auto-dismiss, does not dismiss if set to 0. Duration will not be supported in static mode and static-header mode. | 0 | 4.0.0 |
| clickToClose | boolean | When specified, hides the notification on click. | false | 4.0.0 |
| showClose | boolean | When specified, shows the close icon. | false | 4.0.0 |
| showTimed | boolean | Whether to show countdown circle timer, ONLY used for Global notification (position = topHeader). When showTimer set to true, duration is mandatory. This should be the same as 'showTimed' property of &lt;ap-notification&gt;&lt;/ap-notification&gt;. | false | 4.0.0 |
| showExpandedIcon | boolean | Whether to show expanded icon, ONLY used for Global notification (position = topHeader). This should be the same as 'showExpandedIcon' property of &lt;ap-notification&gt;&lt;/ap-notification&gt;. | false | 4.0.0 |
| icon | string | The class name of the icon | - | 4.0.0 |
| hyperLinkHref | string | Hyperlink href of the notification. | - | 4.6.0 |


<!-- /SECTION:properties -->