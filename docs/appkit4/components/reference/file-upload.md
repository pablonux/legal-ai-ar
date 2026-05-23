---
component: file-upload
framework: angular
---

# File-upload Component

## Overview

AppKit File-upload component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Upload form component:

- When users need to upload one or more files.
- When users need the option to upload files by dragging and dropping.

### Anatomy

1. **Title:** Provides context to user.

2. **Icon:** Slot for extra icon.

3. **Description:** Text to help the user make an informed selection.

4. **Button and drop zone label:** Represents the drop area and the action to select a file to upload.

5. **Uploaded file:** A file that has successfully been uploaded.

6. **Progress bar:** A visual element to show progress.

7. **Error:** Inline error handling with option to try again.

8. **Close icon:** Closes the upload modal.

9. **Delete icon:** The delete x icon will remove the uploaded file.

10. **Progress Indicator:** A visual percentage indicator that shows the progress of the upload.

11. **Success indicator:** A visual element to indicate files have been uploaded successfully.

12. **Delete all button:** The delete all button will delete all files.

13. **Upload button:** The button that initiates the upload process.

### Variants

#### Inline:

Upload component is integrated into the page content, such as in a form or within a text block. This variant is often used when the upload component is a secondary action on the page and needs to be integrated with other content.

#### Modal:

The upload component is displayed in a separate container on the page.

#### Multi-upload:

Allows users to upload multiple files.

#### Triggered:

Upload process won't initiate until button is clicked.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-134464%26viewport%3D1155%252C-49559%252C0.45%26t%3Dy7f0Sg3kNZFdipK2-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- An upload form element should be used anytime the user needs to upload an external form into an application.
- Make it clear to users if the data can't be retracted after it is uploaded or will overwrite existing data.
- Ensure that the upload form component is easy to use, clear, and concise.
- The label for the drop zone area should convey upload limitations.
- The label for the drop zone area should convey that users have the option to either drag and drop a file into the zone or click the button to upload a file.
- Use an ellipsis (…) if the filename extends beyond the width of its parent element.

### Behavior

- Clicking the file input opens a file browser or prompts the user to select a file.
- Clicking the upload button initiates the upload process.
- The progress indicator updates as the upload progresses.
- The error message should display if there is an error during the upload process.

### Accessibility

- After upload form closes, focus should return to the element that initiated upload form.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 8


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | inline | `section: "example:1"` |
| 2 | inline - multiUpload | `section: "example:2"` |
| 3 | inline - trigger | `section: "example:3"` |
| 4 | inline - multiUpload-trigger | `section: "example:4"` |
| 5 | modal | `section: "example:5"` |
| 6 | modal - multiUpload | `section: "example:6"` |
| 7 | modal - trigger | `section: "example:7"` |
| 8 | modal - multiUpload-trigger | `section: "example:8"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### inline


**Example #1** | **Variation**: inline | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload  uploadTitle="Upload your file" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: false,
  type: 'inline'
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: true,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-1 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-1 -->

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### inline - multiUpload


**Example #2** | **Variation**: inline | **Modifier**: multiUpload | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload  uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: true,
  type: 'inline'
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: true,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-2 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-2 -->

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### inline - trigger


**Example #3** | **Variation**: inline | **Modifier**: trigger | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload  uploadTitle="Upload your file" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: false,
  type: 'inline',
  trigger: true
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: false,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-3 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-3 -->

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### inline - multiUpload-trigger


**Example #4** | **Variation**: inline | **Modifier**: multiUpload-trigger | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload  uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: true,
  type: 'inline',
  trigger: true
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: false,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-4 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-4 -->

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### modal


**Example #5** | **Variation**: modal | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    (onClose)="closeContent($event)"
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: false,
  type: 'modal'
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: true,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
}

closeContent(data: any){
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-5 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-5 -->

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### modal - multiUpload


**Example #6** | **Variation**: modal | **Modifier**: multiUpload | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    (onClose)="closeContent($event)"
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: true,
  type: 'modal'
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: true,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
}

closeContent(data: any){
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-6 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-6 -->

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### modal - trigger


**Example #7** | **Variation**: modal | **Modifier**: trigger | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    (onClose)="closeContent($event)"
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: false,
  type: 'modal',
  trigger: true
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: false,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
}

closeContent(data: any){
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-7 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-7 -->

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### modal - multiUpload-trigger


**Example #8** | **Variation**: modal | **Modifier**: multiUpload-trigger | **State**: None

#### Module Import

```typescript
import { FileuploaderModule } from '@appkit4/angular-components/fileupload';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class="fileupload-demo-wrapper">
  <ap-fileupload uploadTitle="Upload your files" 
    [config]="fileuploadConfig" 
    (onUpload)="uploadFiles($event)" 
    (onClose)="closeContent($event)"
    [uploadInstruction]="'You can upload JPG, PNG or PDF files. The max file size is 10mb.'"
    [fileType]="['JPG', 'PNG', 'PDF']"
    [customUploader]="customUploader">
  </ap-fileupload>
</div>
```

#### TypeScript

```typescript
import { FileUploader } from '@appkit4/ng2-file-upload';

fileuploadConfig = {
  multiple: true,
  type: 'modal',
  trigger: true
}

customUploader:FileUploader = new FileUploader({
  maxFileSize: 10*1024*1024,
  autoUpload: false,
  url: 'testUrl' // User need to customize the value of this property, 'testUrl' is just a fake value.
});

uploadFiles(data: any) {
  console.log(data)
}

closeContent(data: any){
  console.log(data)
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

.fileupload-demo-wrapper {
  width: rem(540px);
}
```

#### Dependency

<!-- VERSION:ng2-file-upload-dep-8 -->
```text
"@appkit4/ng2-file-upload": "^1.0.0"
```
<!-- /VERSION:ng2-file-upload-dep-8 -->

<!-- /EXAMPLE:8 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| config | object | Config of the component: { multiple: boolean, type: string: 'inline'\|'modal', trigger: boolean } | { multiple: false, type: 'inline', trigger: false } | 4.0.0 |
| uploadTitle | string | The title of the upload form. | 'Upload your files' | 4.0.0 |
| uploadButtonName | string | The name of the upload button. | 'Upload' | 4.0.0 |
| onClose | EventEmitter<any> | Callback to invoke when the close button is clicked. | N/A | 4.0.0 |
| uploadInstruction | string | The instruction text that displays under the title. | '' | 4.0.0 |
| errorMessage | string | The error message that displays when file upload fails. | 'We have encountered unexpected problems. The selected file won't be uploaded.' | 4.0.0 |
| fileType | Array<string> | The list of file types that can be uploaded and any type of file is able to be uploaded if no value is specified. | \[\] | 4.0.0 |
| customUploader | FileUploader of 'ng2-file-upload' | The customized uploader parameters. Reference: \`new FileUploader({}) (import { FileUploader } from '@appkit4/ng2-file-upload')\` | N/A | 4.0.0 |
| withCredential | boolean | If false, will not sending credentials in the request. | true | 4.6.0 |
| onAddFile | EventEmitter<any> | Callback to invoke after adding file, user can use this method to get the file which need to be uploaded. | N/A | 4.2.0 |
| onDeleteFile | EventEmitter<any> | Callback to invoke after deleting file, user can use this method to get the file which deleted. | N/A | 4.2.0 |
| onUpload | EventEmitter<any> | Callback to invoke when the upload button is clicked, user can use this method to get the files which need to be uploaded and upload them manually. | N/A | 4.0.0 |
| onUploadSuccess | EventEmitter<any> | Callback to invoke when file upload is successful, the parameter is the success response and status. \*Note: this method is available only when property 'customUploader' includes 'url' property and has value. | N/A | 4.0.0 |
| onUploadFail | EventEmitter<any> | Callback to invoke if file upload fails, the parameter is the fail response and status. \*Note: this method is available only when property 'customUploader' includes 'url' property and has value. | N/A | 4.0.0 |
| onValidatedFail | EventEmitter<any> | Callback to invoke when validated fail. | N/A | 4.0.0 |
| onReUpload | EventEmitter<any> | Callback to invoke when the re-upload button is clicked, user can use this method to get the file which to be re-uploaded. | N/A | 4.14.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->