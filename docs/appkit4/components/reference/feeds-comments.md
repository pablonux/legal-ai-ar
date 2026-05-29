---
component: feeds-comments
framework: angular
---

# Feeds-comments Component

## Overview

AppKit Feeds-comments component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use feed and comment components to:

- Present dynamic and constantly changing information.
- Encourage comments and conversations.

### Anatomy

1.** Avatar:** Appkit’s avatar.

2.** Comment:** Body area in which comments are displayed.

3.** Timestamp:** Displays time entry was posted.

4.** Social options:** Social interactions represented with icons.

5. Add comment modal is based on Appkit’s modal structure.

6.** Attachment:** Displays the media attached to comments.

### Variants

#### Activity feed:

Displays comments and content users input into the system in chronological order.

#### Add comment modal:

Modal to enter new comments, based on Appkit's modal.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-12466%26viewport%3D849%252C-8951%252C0.32%26t%3DjPPoQ5KpS5LflHNw-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use the feed and comment component to enable users to author and post content shared within a feed.
- The feed should have a strong visual hierarchy that makes it easy for users to distinguish between different types of content and prioritize what they want to read or engage with.
- Display comments in chronological order. This helps to support the flow of the conversation, with newer comments appearing below older ones. This makes it easier for users to follow the conversation and respond to specific comments.

#### How not to use

- Do not use a feed and comment component to edit site content outside of the feed.

### Behavior

- Users should be able to click on each content item to view the full content and leave comments.
- Users should be able to scroll through the feed and load more content items as they reach the end of the current list.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 4


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | feed | `section: "example:1"` |
| 2 | feed - attachment | `section: "example:2"` |
| 3 | addComment | `section: "example:3"` |
| 4 | addComment - attachment | `section: "example:4"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### feed


**Example #1** | **Variation**: feed | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FeedModule } from '@appkit4/angular-components/feed';
```

#### HTML Template

```html
<div class="feeds-comments-wrapper">
  <ap-feed>
    <ng-container *ngFor="let comment of commentList">
        <ap-feed-item 
            [comment]="comment"
            (onLikeStatusChange)="onLikeStatusChange($event)"
            (onCommentClick)="onCommentClick($event)">
        </ap-feed-item>
    </ng-container>
  </ap-feed>
</div>
```

#### TypeScript

```typescript
commentList = [
  {
      shortName: 'JS',
      shortNameBgColor: '#415385',
      shortNameFontColor: '#ffffff',
      fullName: 'Jamie Sutton',
      commentsTime: '3 hours ago',
      commentsContent: `Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, 
          at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.`
  },
  {
      shortName: 'EL',
      shortNameBgColor: '#415385',
      shortNameFontColor: '#ffffff',
      fullName: 'Ernesto Laborda',
      commentsTime: '28 min ago',
      likesCount: 24,
      liked: true,
      commentsCount: 8,
      commentsContent: `Nunc feugiat vitae leo at molestie. Donec feugiat nunc a aliquet dignissim. 
          Proin ut euismod urna, id pulvinar erat.`
  }
];

onLikeStatusChange(event: any) {
  console.log('previous comment: ', event);
  let currentItem = this.commentList.find((item: any) => item === event);
  if (currentItem) {
      currentItem.liked = !currentItem.liked;
      currentItem.likesCount && (currentItem.liked ? currentItem.likesCount++ : currentItem.likesCount--);
  }
}

onCommentClick(event: any) {
  console.log('current comment: ', event);
}
```

#### SCSS Styles

```scss
.feeds-comments-wrapper {
  width: 540px;
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### feed - attachment


**Example #2** | **Variation**: feed | **Modifier**: attachment | **State**: None

#### Module Import

```typescript
import { FeedModule } from '@appkit4/angular-components/feed';
```

#### HTML Template

```html
<div class="feeds-comments-wrapper">
  <ap-feed>
    <ng-container *ngFor="let comment of commentList">
        <ap-feed-item 
            [comment]="comment"
            (onLikeStatusChange)="onLikeStatusChange($event)"
            (onCommentClick)="onCommentClick($event)"
            (onPreviewImage)="onPreviewImage($event)">
        </ap-feed-item>
    </ng-container>
  </ap-feed>
</div>
```

#### TypeScript

```typescript
commentList = [
  {
      shortName: 'JS',
      shortNameBgColor: '#415385',
      shortNameFontColor: '#ffffff',
      fullName: 'Jamie Sutton',
      commentsTime: '3 hours ago',
      commentsContent: `Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, 
          at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.`
  },
  {
      shortName: 'EL',
      shortNameBgColor: '#415385',
      shortNameFontColor: '#ffffff',
      fullName: 'Ernesto Laborda',
      commentsTime: '28 min ago',
      likesCount: 24,
      liked: true,
      commentsCount: 8,
      commentsContent: `Nunc feugiat vitae leo at molestie. Donec feugiat nunc a aliquet dignissim. 
          Proin ut euismod urna, id pulvinar erat.`
  },
  {
    shortName: 'VR',
    shortNameBgColor: '#415385',
    shortNameFontColor: '#ffffff',
    fullName: 'Victor Rouco',
    commentsTime: 'Just now',
    images: ['app/images/feedcomment/Image 2.svg', 'app/images/feedcomment/Image.svg'],
    likesCount: 3,
    liked: false,
    commentsCount: 1,
    commentsContent: `Aenean non mi porta, dignissim tortor sed, fringilla magna.`
  }
];

onLikeStatusChange(event: any) {
  console.log('previous comment: ', event);
  let currentItem = this.commentList.find((item: any) => item === event);
  if (currentItem) {
      currentItem.liked = !currentItem.liked;
      currentItem.likesCount && (currentItem.liked ? currentItem.likesCount++ : currentItem.likesCount--);
  }
}

onCommentClick(event: any) {
  console.log('current comment: ', event);
}

onPreviewImage(event: boolean) {
  console.log('Is picture previewing: ', event);
}
```

#### SCSS Styles

```scss
.feeds-comments-wrapper {
  width: 540px;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### addComment


**Example #3** | **Variation**: addComment | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { CommentModule } from '@appkit4/angular-components/comment';
import { ButtonModule } from '@appkit4/angular-components/button';
import { FieldModule } from '@appkit4/angular-components/field';
import { ModalModule } from '@appkit4/angular-components/modal';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open comment panel'"></ap-button>
<ap-modal #addCommentPanel appModalId="addCommentPanel" [closable]="false" [initialFocusIndex]="0" [styleClass]="'add-comment-panel-modal'">
  <ap-comment
      [showCounter]="true" 
      (onCloseClick)="close()" 
      (onAddClick)="addComments($event)">
  </ap-comment>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';


@ViewChild('addCommentPanel', { static: false }) addCommentPanel: any;

showModal() {
  this.addCommentPanel.showModal('addCommentPanel');
}

close() {
  // console.log('cancle this operation');
  this.addCommentPanel.closeModal('addCommentPanel');
}

addComments(event: any) {
  console.log('comments added: ', event); 
}
```

#### SCSS Styles

```scss
.feeds-comments-wrapper {
  width: 540px;
}

::ng-deep {
  .add-comment-panel-modal {
      width: 540px;

      .ap-modal-header {
          display: none;
      }

      .ap-modal-body {
          padding: 0;
      }
  }
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### addComment - attachment


**Example #4** | **Variation**: addComment | **Modifier**: attachment | **State**: None

#### Module Import

```typescript
import { CommentModule } from '@appkit4/angular-components/comment';
import { ButtonModule } from '@appkit4/angular-components/button';
import { FieldModule } from '@appkit4/angular-components/field';
import { ModalModule } from '@appkit4/angular-components/modal';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" (onClick)="showModal()" [label]="'Open comment panel'"></ap-button>
<ap-modal #addCommentPanel appModalId="addCommentPanel" [closable]="false" [initialFocusIndex]="0" [styleClass]="'add-comment-panel-modal'">
  <ap-comment
      [showCounter]="true" 
      (onCloseClick)="close()" 
      (onAddClick)="addComments($event)"
      [showAttachment]="showAttachment">
  </ap-comment>
</ap-modal>
```

#### TypeScript

```typescript
import { ViewChild } from '@angular/core';

showAttachment = true;
@ViewChild('addCommentPanel', { static: false }) addCommentPanel: any;

showModal() {
  this.addCommentPanel.showModal('addCommentPanel');
}

close() {
  // console.log('cancle this operation');
  this.addCommentPanel.closeModal('addCommentPanel');
}

addComments(event: any) {
  console.log('comments added: ', event); 
}
```

#### SCSS Styles

```scss
.feeds-comments-wrapper {
  width: 540px;
}

::ng-deep {
  .add-comment-panel-modal {
      width: 540px;

      .ap-modal-header {
          display: none;
      }

      .ap-modal-body {
          padding: 0;
      }
  }
}
```

<!-- /EXAMPLE:4 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-feed

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| style | string | The inline style of the component. | '' | 4.2.0 |
| styleClass | string | The style class names of the component. | '' | 4.2.0 |

### ap-feed-item

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| labelForLike | string | The label for the 'likes' button. | 'likes' | 4.2.0 |
| labelForComment | string | The label for the 'comments' button. | 'comments' | 4.2.0 |
| onLikeStatusChange | EventEmitter<Comment> | Callback to invoke when the like button is clicked. Event properties: • shortName: Label of the avatar. • shortNameBgColor: Background color of the avatar. • shortNameFontColor: Font color of the avatar. • fullName: Full name of commenter. • commentsTime: Comment time. • images: • likesCount: Number of likes. • liked: Whether the comment is liked or not. • commentsCount: Number of sub comments. • commentsContent: Content of the comment. | - | 4.0.0 |
| onCommentClick | EventEmitter<Comment> | Callback to invoke when the comment button is clicked. Event properties: • shortName: Label of the avatar. • shortNameBgColor: Background color of the avatar. • shortNameFontColor: Font color of the avatar. • fullName: Full name of commenter. • commentsTime: Comment time. • images: • likesCount: Number of likes. • liked: Whether the comment is liked or not. • commentsCount: Number of sub comments. • commentsContent: Content of the comment. | - | 4.0.0 |
| onPreviewImage | EventEmitter<boolean> | Callback to invole when the image is clicked to preview. | - | 4.0.0 |
| comment | Array<Comment> | The comment which displays in comments panel. Event properties: • shortName: Label of the avatar. • shortNameBgColor: Background color of the avatar. • shortNameFontColor: Font color of the avatar. • fullName: Full name of commenter. • commentsTime: Comment time. • images: • likesCount: Number of likes. • liked: Whether the comment is liked or not. • commentsCount: Number of sub comments. • commentsContent: Content of the comment. | - | 4.2.0 |
| shortNameTemplate | TemplateRef<any> | Additional content which will be added to the shortName. | - | 4.2.0 |
| headerTemplate | TemplateRef<any> | Additional content which will be added to the header. | - | 4.2.0 |
| bodyTemplate | TemplateRef<any> | Additional content which will be added to the body. | - | 4.2.0 |
| footerTemplate | TemplateRef<any> | Additional content which will be added to the footer. | - | 4.2.0 |
| style | string | The inline style of the component. | '' | 4.2.0 |
| styleClass | string | The style class names of the component. | '' | 4.2.0 |

### ap-comment

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| title | string | The title of the comment. | 'Add a new comment' | 4.0.0 |
| placeholderWithoutContent | string | Default text to display when there is no value in the comment text area. | 'Write a comment...' | 4.0.0 |
| placeholderWithContent | string | Default text to display when there is value in the comment text area. | 'Your comment' | 4.0.0 |
| commentsContent | string | The content of the comment. | '' | 4.0.0 |
| maxLength | number | The max length that user can input in the comment text area. | 420 | 4.0.0 |
| showCounter | boolean | Whether to display the counter in the comment text area. | false | 4.0.0 |
| disabled | boolean | Whether the comment text area is disabled. | false | 4.0.0 |
| onMoreClick | EventEmitter<Event> | Callback to invoke when the more button is clicked. | - | 4.0.0 |
| onCloseClick | EventEmitter<Event> | Callback to invoke when the close or cancel button is clicked. | - | 4.0.0 |
| onAddClick | EventEmitter<{ originalEvent: Event, commentsContent: string, uploadedFiles: File\[\] }> | Callback to invoke when the add button is clicked. Event properties: • originalEvent: Event. • commentsContent: Comment text. • uploadedFiles: Array of uploaded files. | - | 4.0.0 |
| addBtnName | string | The name of the add comment button. | 'Add comment' | 4.0.0 |
| showAttachment | boolean | When specified, shows the attachment icon. | false | 4.0.0 |
| showClose | boolean | When specified, shows the close icon. | true | 4.2.0 |
| showMore | boolean | When specified, shows the more icon. | true | 4.2.0 |
| headerActionTemplate | TemplateRef<any> | Additional content which will be added to the header of the comment. | - | 4.2.0 |
| footerTemplate | TemplateRef<any> | Additional content which will be added to the footer of the comment. | - | 4.2.0 |
| commentStyle | string | The inline style of the component. | '' | 4.0.0 |
| commentStyleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->