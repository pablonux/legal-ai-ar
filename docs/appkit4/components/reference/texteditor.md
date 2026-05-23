---
component: texteditor
framework: angular
---

# Texteditor Component

## Overview

AppKit Texteditor component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use the Rich text editor component:

- For allowing users to input or edit text within a larger form or application.
- To enable users to enter, format, and edit text content.

### Anatomy

1. **Toolbar:** A horizontal bar that contains various text formatting options such as bold, italic, underline, font size, font family, etc.

2. **Input area:** The space where users can type and edit the text content.

3. **Container:** Defines the area of the tex editor.

### Variants

#### Simple:

Provides only the most commonly used editing tools to reduce visual clutter and complexity.

#### Expanded:

Provides access to more advanced editing tools and features.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-111270%26viewport%3D1195%252C-36721%252C0.42%26t%3Dq75OTpoSsv1lfO79-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Consider using a Rich Text Editor when the text input is intended to be viewed by others, and it is necessary to have control over the appearance of the content to effectively convey the intended message.

#### How not to use

- Consider limiting the formatting options to what is required.
- Don't use a Rich text editor component when the user doesn't need control over the look and feel of the input text:
- If the input is short, consider using an Input field.
- If input is longer, consider using a Text area field.

### Behavior

- Character formatting is applied to any selected text/characters.
- Paragraph formatting is applied to the entire paragraph surrounding the selection or cursor.
- Adding a link: Highlight the text, click the link icon and add a URL to the dialog.

### Accessibility

- Inform user of character limits.
- Provide instructions for completing field.
- Use aria-required="true" for required fields.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 2


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | simple | `section: "example:1"` |
| 2 | expanded | `section: "example:2"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### simple


**Example #1** | **Variation**: simple | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { TextEditorModule } from '@appkit4/angular-text-editor/text-editor';
```

#### HTML Template

```html
                      <div class="simple-editor-container">
  <ap-text-editor [(data)]="mockData1" [config]="simpleTextEditorConfig"></ap-text-editor>
</div>
```

#### TypeScript

```typescript
mockData1: string  =   "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.";
public simpleTextEditorConfig = {
  toolbar: [
    'fontFamily',
    'fontSize',
    'bold',
    'italic',
    'alignment:left',
    'alignment:center',
    'alignment:right',
    'alignment:justify',
  ]
};
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

 .simple-editor-container{
   width: rem(460px);
   padding: $spacing-7;
 }

 :host ::ng-deep .simple-editor-container {
   .ck-editor__editable_inline {
     max-height: rem(136px);
   }
 }
```

#### Dependency

<!-- VERSION:ckeditor-dep-1 -->
```text
"@appkit4/ckeditor5-appkit-build": "4.5.0",
"lodash-es": "^4.17.21"
```
<!-- /VERSION:ckeditor-dep-1 -->

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### expanded


**Example #2** | **Variation**: expanded | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { TextEditorModule } from '@appkit4/angular-text-editor/text-editor';
```

#### HTML Template

```html
                      <div class="doc-editor-container">
  <ap-text-editor [(data)]="mockData2" (onReady)="onDocEditorReady($event)" [config]="docTextEditorConfig"></ap-text-editor>
</div>
```

#### TypeScript

```typescript
 // The adapters below are upload adapter used for text-editor's image upload function, 
 // there are two kind of adapter, one is Base64Adapter, 
 // which not require a backend service, 
 // the other is normal upload, which requires config of backend service url,token,etc. 
 // Samples of these two adapter provided in this sample code.
 // All the samples can be modified for custom use.

 class Base64UploadAdapter {
  public loader: any;
  public reader:any;
  constructor(loader:any) {
    // The file loader instance to use during the upload.
    this.loader = loader;
    this.reader= new window.FileReader();
  }

  // Starts the upload process.
  upload() {
    return new Promise( ( resolve, reject ) => {
      const reader = this.reader;

      reader.addEventListener( 'load', () => {
        resolve( { default: reader.result } );
      } );

      reader.addEventListener( 'error', (err:Error) => {
        reject( err );
      } );

      reader.addEventListener( 'abort', () => {
        reject();
      } );

      this.loader.file.then( (file:any) => {
        reader.readAsDataURL( file );
      } );
    } );
  }

  // Aborts the upload process.
  abort() {
    this.reader.abort();
  }

}

class UploadAdapter {
  public loader: any;
  public url: string;
  public xhr: any;
  public token: string;
  public id:number=1;
  constructor(loader:any,url:string,token:string) {
    // The file loader instance to use during the upload.
    this.loader = loader;
    this.url=url;
    this.token=token;
  }

  // Starts the upload process.
  upload() {
    return this.loader.file.then(
      (file:any) =>
        new Promise((resolve, reject) => {
          this._initRequest();
          this._initListeners(resolve, reject, file);
          this._sendRequest(file);
        })
    );
  }

  // Aborts the upload process.
  abort() {
    if (this.xhr) {
      this.xhr.abort();
    }
  }

  _initRequest() {
    const xhr = (this.xhr = new XMLHttpRequest());
    xhr.open("POST", this.url+"?id="+this.id, true);
    xhr.responseType = "json";
    this.id++;
  }

  _initListeners(resolve:any, reject:any, file:any) {
    const xhr = this.xhr;
    const loader = this.loader;
    const genericErrorText = "Couldn't upload file:";
    xhr.addEventListener("error", () => reject(genericErrorText));
    xhr.addEventListener("abort", () => reject());
    xhr.addEventListener("load", () => {
      const response = xhr.response;
      if(xhr.status===200){
        return resolve({
          default: "http://localhost:4200/appkit4-service/api/file/download?id="+(this.id-1),
        });
      }
      if (!response || response.error) {
        console.error(response && response.error ? response.error.message : genericErrorText)
        return reject();
      }
      resolve({
        default: "http://localhost:4200/appkit4-service/api/file/download?id="+(this.id-1),
      });
    });

    if (xhr.upload) {
      xhr.upload.addEventListener("progress", (evt:any) => {
        if (evt.lengthComputable) {
          loader.uploadTotal = evt.total;
          loader.uploaded = evt.loaded;
        }
      });
    }
  }

  _sendRequest(file:any) {
    const data = new FormData();
    data.append("fileUpload", file);
    this.xhr.send(data);
  }
}

/**
 * Texteditor component script start
 * */
mockData2 = '<div>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec bibendum neque eget diam sodales, '+
'at gravida nisl tincidunt. Mauris tempus nibh vel nibh pharetra, ac interdum ante feugiat. Mauris ac dui lacus. '+
'Nullam id cursus odio, sit amet viverra ipsum. Pellentesque habitant morbi tristique senectus et netus et malesuada'+
' fames ac turpis egestas. Nulla feugiat sodales hendrerit. Suspendisse condimentum nunc nec lacus commodo ultricies '+
'ut egestas odio. Nullam aliquam viverra lorem, vitae blandit augue viverra ac. Nunc imperdiet purus felis, nec blandit'+
' turpis ultricies quis. Mauris ultrices nisi nec lorem maximus, sit amet aliquet arcu sagittis. Nullam malesuada tristique'+
' massa, vulputate lacinia urna tempus eget.</div><div> Duis porttitor eu urna in feugiat. Phasellus dignissim orci nibh,'+
' vehicula luctus arcu pulvinar ac. Donec et nunc nec nisi hendrerit sollicitudin eu et eros. Aliquam accumsan sed sapien'+
' id pellentesque. Curabitur fermentum lectus dui, ut lobortis elit malesuada euismod. Curabitur gravida cursus pellentesque.'+
' Cras at tempor ex. Vivamus arcu orci, iaculis vitae luctus id, dapibus vitae sapien.</div>';

public docTextEditorConfig = {
  toolbar: [
    'fontFamily',
    'fontSize',
    'bold',
    'italic',
    'strikethrough',
    'underline',
    'bulletedList',
    'numberedList',
    'indent',
    'outdent',
    'alignment:left',
    'alignment:center',
    'alignment:right',
    'alignment:justify',
    'link',
    'uploadImage',
    'undo',
    'redo',
  ],
  image: {
    toolbar: [],
  },
};
onDocEditorReady(editor : any) {
  editor.plugins.get("FileRepository").createUploadAdapter = (loader : any) => {
    return new Base64UploadAdapter(loader);//in this line, Base64UploadApapter can be changed to UploadAdapter
  };
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

.doc-editor-container{
  width: rem(980px);
  padding: $spacing-7;
}
:host ::ng-deep .doc-editor-container {
  .ck-editor__editable_inline {
    max-height: rem(248px);
  }
}
```

#### Dependency

<!-- VERSION:ckeditor-dep-2 -->
```text
"@appkit4/ckeditor5-appkit-build": "4.5.0",
"lodash-es": "^4.17.21"
```
<!-- /VERSION:ckeditor-dep-2 -->

<!-- /EXAMPLE:2 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-text-editor

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| disabled | boolean | Set text editor disabled. | false | 4.0.0 |
| data | string | The text content of text editor. Two-way binding is supported. | '' | 4.1.0 |
| dataChange | EventEmitter<string> | The callback function triggered when the text content changes. | - | 4.2.0 |
| config | object | The config object of text editor.Introduced by config table below. | {} | 4.0.0 |
| onReady | EventEmitter<Object> | The callback function triggered when the editor is ready with the editor instance. | - | 4.0.0 |
| onChange | EventEmitter<Object> | The callback function triggered when the content of the editor has changed with the content string and the editor instance. | - | 4.0.0 |
| onFocus | EventEmitter<Object> | The callback function triggered when the editing view of the editor is focused with the editor instance. | - | 4.0.0 |
| onBlur | EventEmitter<Object> | The callback function triggered when the editing view of the editor is blurred with the editor instance. | - | 4.0.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component | '' | 4.0.0 |

### Config

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| fontFamily | object | The configuration of the font family feature. It is introduced by the FontFamilyConfig table. | {} | 4.0.0 |
| fontSize | object | The configuration of the font size feature. It is introduced by the FontSizeConfig feature. | {} | 4.0.0 |
| toolbar | array | The editor toolbar configuration. | \["fontFamily","fontSize","bold","italic","alignment:left","alignment:center","alignment:right","alignment:justify"\] | 4.0.0 |

### Font Family Config

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| options | array<string> | Available font family options defined as an array of strings. | \["default","Arial, Helvetica, sans-serif","Courier New, Courier, monospace","Georgia, serif","Lucida Sans Unicode, Lucida Grande, sans-serif","Tahoma, Geneva, sans-serif","Times New Roman, Times, serif","Trebuchet MS, Helvetica, sans-serif","Verdana, Geneva, sans-serif"\] | 4.0.0 |
| supportAllValues | boolean | By default the plugin removes any font-family value that does not match the plugin's configuration. It means that if you paste content with font families that the editor does not understand, the font-family attribute will be removed and the content will be displayed with the default font.You can preserve pasted font family values by switching the supportAllValues option to true | false | 4.0.0 |

### Font Size Config

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| options | array<Number> | Available font size options. Expressed as predefined presets, numerical 'pixel' values. | \["tiny","small","default","big","huge"\] | 4.0.0 |


<!-- /SECTION:properties -->
