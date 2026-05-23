---
component: ratings
framework: angular
---

# Ratings Component

## Overview

AppKit Ratings component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Rating and review component:

- To allow users to rate and review products or services
- To reveal ratings and review information trousers

### Anatomy

**1. Star rating system**

**2. Sentiment rating system**

**3. Sentiment slider**

### Variants

#### Baseline star rating system:

Allows users to rate a product or service using a scale of one to five stars.

#### Baseline sentiment rating system:

Allows users to rate a product or service using a set of predefined sentiments such as "great," "good," "ok," "bad," and "awful”.

#### Baseline sentiment slider:

Allows users to rate a product or service by sliding a handle through a set of predefined sentiments such as "great," "good," "ok," "bad," and "awful”.

#### Customer ratings:

Provides a ratings summary.

#### Review comments:

The Review comments variant displays the review comments entered from the comment input modal, as well as the rating system and possible images associated with the comment.

#### Write a review form:

Allows users to provide a written review of a product or service.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-110472%26viewport%3D881%252C-28019%252C0.34%26t%3DImqWNnfXpzNgNsig-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use when something is reviewed or rated by others within the application.
- When using a rating and review component, ensure the rating standard is clear to the user.
- Star ratings should always have 5 available stars. This shouldn't be increased or decreased to fit various containers.
- Provide a ratings distribution summary at the top of the reviews section if possible. On Appkit this is included in the component under the “Customer ratings” variant.

### Behavior

- The rating can be cleared by clicking one more time on the “highest” star (e.g., the third star for a three-star rating).
- Tab places focus on the rating component. If focus is already on the rating component, the focus is moved to the next item in the page sequence.
- On keyboard focus, users can use the tab key to highlight a star rating and use the enter key to set a rating.
- Hitting enter one more time will clear the rating.

### Accessibility

- Section must be labelled with proper heading "rating and review".
- Use aria-label="" to identify "rating of 1 to 5 stars" to inform assistive technology users.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 9


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | baselineRating - default | `section: "example:1"` |
| 2 | baselineRating - read-only | `section: "example:2"` |
| 3 | baselineRating - sentiment | `section: "example:3"` |
| 4 | baselineRating - sentimentSlider | `section: "example:4"` |
| 5 | writeReview - default | `section: "example:5"` |
| 6 | customerRatings - default | `section: "example:6"` |
| 7 | reviewComments - default | `section: "example:7"` |
| 8 | reviewComments - multi | `section: "example:8"` |
| 9 | reviewComments - attachment | `section: "example:9"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### baselineRating - default


**Example #1** | **Variation**: baselineRating | **Modifier**: default | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<ap-ratings [total]="5" [active]="2" (onChange)="onRatingChange($event)"></ap-ratings>
```

#### TypeScript

```typescript
onRatingChange(event: any) {
	console.log(event);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### baselineRating - read-only


**Example #2** | **Variation**: baselineRating | **Modifier**: read-only | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<ap-ratings [type]="'readonly'" [total]="5" [active]="3" (onChange)="onRatingChange($event)"></ap-ratings>
```

#### TypeScript

```typescript
onRatingChange(event: any) {
	console.log(event);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### baselineRating - sentiment


**Example #3** | **Variation**: baselineRating | **Modifier**: sentiment | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<ap-ratings [type]="'sentiment'" [total]="5" [active]="2" (onChange)="onRatingChange($event)"></ap-ratings>
```

#### TypeScript

```typescript
onRatingChange(event: any) {
	console.log(event);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### baselineRating - sentimentSlider


**Example #4** | **Variation**: baselineRating | **Modifier**: sentimentSlider | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<ap-ratings [type]="'sentiment'" [slider]="true" [total]="5" [active]="3" (onChange)="onRatingChange($event)"></ap-ratings>
```

#### TypeScript

```typescript
onRatingChange(event: any) {
	console.log(event);
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### writeReview - default


**Example #5** | **Variation**: writeReview | **Modifier**: default | **State**: None

#### Module Import

```typescript
import { ReactiveFormsModule } from '@angular/forms';
import { RatingsModule } from '@appkit4/angular-components/ratings';
import { ButtonModule } from '@appkit4/angular-components/button';
import { FieldModule } from '@appkit4/angular-components/field';
import { FormsModule } from '@angular/forms'; 
```

#### HTML Template

```html
<div class="ap-ratings-demo-write-review-container">
	<form [formGroup]="reviewForm" (ngSubmit)="onSubmit()">
		<div class="ap-reviews">
			<div class="ap-reviews-label">
				<span>Write a review</span>
			</div>
			<span class="Appkit4-icon icon-close-outline" tabindex="0" role="button" type="button"
				aria-label="Close"></span>
			<div class="ap-reviews-fields">
				<ap-field [title]="'Name'">
					<input appkit-field formControlName="name" />
				</ap-field>
				<ap-field [title]="'Email address'">
					<input appkit-field formControlName="email" />
				</ap-field>
			</div>
		</div>
		<div class="ap-reviews">
			<div class="ap-reviews-label">
				<span>Select your rating</span>
			</div>
			<div class="ap-ratings-demo-container">
				<ap-ratings [total]="5" [active]="activeIndex" (onChange)="onRatingChange($event)">
				</ap-ratings>
			</div>
			<div class="ap-reviews-fields">
				<ap-field [title]="'Title'">
					<input appkit-field formControlName="title" />
				</ap-field>
				<ap-field [title]="'Details'">
					<textarea appkit-field formControlName="details" maxlength="420"></textarea>
					<ap-field-counter [value]="reviewForm.value.details" [max]="420"></ap-field-counter>
				</ap-field>
			</div>
		</div>
		<div class="ap-ratings-demo-footer">
			<button class="ap-ratings-demo-footer-attach" (click)="safariAgent?undefined:attach()" type="button" aria-label="attach link">
				<span aria-hidden="true" class="Appkit4-icon icon-paperclip-outline"></span>
				<input aria-hidden="true" #fileInput type="file" accept="image/*" (change)="onFileChange($event)" tabindex="-1" />
			</button>
			<ap-button [btnType]="'primary'" [label]="'Submit'" [style]="'width:99px;'" [disabled]="!reviewForm.valid"></ap-button>
		</div>
		<div *ngIf="fileImageUrls.length" class="uploaded-images">
			<div class="uploaded-images-placeholder">
				<div *ngFor="let ph of [1, 2, 3, 4];let k=index" class="uploaded-images-placeholder-item" [class.rimless]="k>=fileImageUrls.length"></div>
			</div>
			<div class="image-wrapper" *ngFor="let item of fileImageUrls;let i=index">
				<img [src]="item" [attr.alt]="'uploaded-image '+(i+1)">
				<span class="Appkit4-icon icon-circle-delete-outline" (click)="deleteImg(i)" (keydown.enter)="deleteImg(i)"
					tabindex="0" role="button" [attr.aria-label]="'Delete image '+(i+1)"></span>
			</div>
		</div>
	</form>
</div>
```

#### TypeScript

```typescript
import { ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';

constructor(private sanitizer: DomSanitizer, private fb: FormBuilder) { }

@ViewChild('fileInput') fileInput!: ElementRef;
safariAgent = navigator.userAgent.indexOf("Safari") > -1 && navigator.vendor.indexOf("Apple") > -1;
activeIndex: number = 2;
reviewForm: FormGroup = new FormGroup({});
target: any;
fileImageUrls: Array<any> = [];
uploadedFiles: Array<any> = [];

ngOnInit(): void{
	this.reviewForm = this.fb.group({
		name: [''],
		email: [''],
		title: [''],
		details: ['']
	});
}

onRatingChange(event: any) {
	this.activeIndex = event.activeIndex;
}

attach() {
	this.fileInput.nativeElement.click();
}

onFileChange(event: any) {
	this.target = event.target || event.srcElement;
	let imageFile = event.target.files[0];
	if (imageFile) {
		let imgUrl = window.URL.createObjectURL(imageFile);
		this.fileImageUrls.push(this.sanitizer.bypassSecurityTrustUrl(imgUrl));
		this.uploadedFiles.push(imageFile);
		this.target.value = '';
	}
}

deleteImg(index: number) {
	this.fileImageUrls.splice(index, 1);
	this.uploadedFiles.splice(index, 1);
}

onSubmit() {
	let formData: any = { ... this.reviewForm.value };
	formData.rating = this.activeIndex;
	formData.attachment = this.uploadedFiles;
	this.fileImageUrls = [];
	this.uploadedFiles = [];
	this.reviewForm.reset();
	console.log(formData);
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

.ap-ratings-demo-write-review-container {
	display: grid;
	position: relative;
	width: 540px;
	padding: $spacing-6;
	border-radius: $border-radius-3;
	box-shadow: $box-shadow-3;
	background-color: $color-background-container;

	.icon-close-outline {
		color: $color-text-heading;
		position: absolute;
		top: $spacing-4;
		right: $spacing-4;
		cursor: pointer;
		width: 40px;
		height: 40px;
		line-height: 40px;
		border-radius: $border-radius-2;
		padding: $spacing-3;

		&::before {
			font-weight: $font-weight-2;
		}

		&:hover {
			background-color: $color-background-hover;
		}
	}

	.ap-reviews {
		margin-bottom: $spacing-6;

		.ap-reviews-label {
			color: $color-text-heading;
			margin-bottom: $spacing-6;
			font: $font-weight-2 $typography-4 'PwC Helvetica Neue';
		}

		.ap-reviews-fields {
			display: grid;
			row-gap: $spacing-4;
		}

		.ap-ratings-demo-container {
			margin-bottom: $spacing-4;
		}
	}

	.ap-ratings-demo-footer {
		display: flex;
		justify-content: space-between;

		&-attach {
			width: 40px;
			height: 40px;
			padding: $spacing-3;
			border-radius: $border-radius-2;
			background-color: $color-background-container;
			cursor: pointer;

			&:hover {
				background-color: $color-background-hover;
			}

			.Appkit4-icon {
				color: $color-text-heading;
			}

			input[type="file"] {
				cursor: pointer;
				opacity: 0;
				visibility: visible;
				width: 0%;
				position: absolute;
				height: 0%;
				left: 0;
			}
		}
	}

	.uploaded-images {
		display: grid;
		gap: $spacing-3;
		grid-template-columns: repeat(4, 119px);
		margin-top: $spacing-6;
		position: relative;

		&-placeholder {
			display: flex;
			position: absolute;
			left: 0;
			top: 0;

			&-item {
				width: 119px;
				height: 120px;
				border-radius: $border-radius-2;

				&.rimless {
					border: dashed 1px $color-text-light;
				}
			}

			.uploaded-images-placeholder-item+.uploaded-images-placeholder-item {
				margin-left: $spacing-3;
			}
		}

		.image-wrapper {
			width: 119px;
			height: 120px;
			border-radius: $border-radius-2;
			position: relative;
			overflow: hidden;
			display: flex;
			justify-content: center;

			img {
				border-radius: $border-radius-2;
				max-height: 100%;
			}

			.Appkit4-icon {
				color: $neutral-01;
				cursor: pointer;
				position: absolute;
				border-radius: $border-radius-1;
				top: $spacing-1;
				right: $spacing-1;

				&:hover {
					background-color: rgba($neutral-20, $opacity-6);
				}
			}
		}
	}
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### customerRatings - default


**Example #6** | **Variation**: customerRatings | **Modifier**: default | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<div class="ap-ratings-demo-customer-ratings-container">
	<div class="customer-ratings-title">Customer reviews</div>
	<div class="customer-ratings-score-container">
		<span class="score">4.2</span>
		<ap-ratings [type]="'readonly'" [active]="3" [style]="'margin-top:20px;'"></ap-ratings>
	</div>
	<div class="customer-ratings-note">Based on 552 reviews</div>
	<div class="customer-ratings-bars-wrapper">
		<div *ngFor="let bar of customerRatingList" class="customer-ratings-bar">
			<div class="stars" [attr.aria-label]="bar.score+' star'">
				<span class="score">{{bar.score}}</span>
				<span class="Appkit4-icon icon-rating-fill" aria-hidden="true"></span>
			</div>
			<div class="bar">
				<div class="bar-bottom"></div>
				<div class="bar-top" [ngStyle]="{'width': bar.percentage}"></div>
			</div>
			<div class="percentage">{{bar.percentage}}</div>
		</div>
	</div>
</div>
```

#### TypeScript

```typescript
customerRatingList: any[] = [
	{ score: 5, percentage: '84%' },
	{ score: 4, percentage: '35%' },
	{ score: 3, percentage: '12%' },
	{ score: 2, percentage: '5%' },
	{ score: 1, percentage: '2%' }
];
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

.ap-ratings-demo-customer-ratings-container {
	background-color: $color-background-container;
	width: 288px;
	padding: 40px;
	box-shadow: $box-shadow-1;
	border-radius: $border-radius-3;

	.customer-ratings-title {
		font: $font-weight-2 $typography-4 'PwC Helvetica Neue';
		color: $color-text-heading;
		margin-bottom: $spacing-2;
	}

	.customer-ratings-score-container {
		display: flex;
		column-gap: $spacing-4;
		margin-bottom: $spacing-4;

		.score {
			width: 65px;
			font: $font-weight-2 $typography-6 'PwC Helvetica Neue';
			letter-spacing: -0.8px;
			color: $color-text-heading;
		}
	}

	.customer-ratings-note {
		font: $typography-1 'PwC Helvetica Neue';
		letter-spacing: -0.2px;
		color: $color-text-light;
		margin-bottom: $spacing-7;
	}

	.customer-ratings-bars-wrapper {
		display: grid;
		row-gap: $spacing-4;

		.customer-ratings-bar {
			display: flex;
			align-items: center;
			column-gap: $spacing-4;

			.stars {
				display: flex;
				align-items: center;
				color: $color-text-body;

				.score {
					font: $typography-3 'PwC Helvetica Neue';
					width: 13px;
					padding-left: $spacing-1;
					display: flex;
					justify-content: flex-end;
				}

				.Appkit4-icon {
					font-size: 12px;
					font-weight: $font-weight-2;
					width: 18px;
					padding-left: $spacing-1;
					letter-spacing: normal;
				}
			}

			.bar {
				position: relative;

				.bar-bottom {
					width: 120px;
					height: 24px;
					background-color: $color-background-hover-selected;
					border-radius: $border-radius-2;
				}

				.bar-top {
					position: absolute;
					top: 0;
					left: 0;
					width: 0;
					height: 24px;
					background-color: $color-background-primary;
					border-radius: $border-radius-2;
				}
			}

			.percentage {
				font: $typography-3 'PwC Helvetica Neue', sans-serif;
				color: $color-text-body;
			}
		}
	}
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### reviewComments - default


**Example #7** | **Variation**: reviewComments | **Modifier**: default | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<div class="ap-ratings-demo-review-comments-container">
	<ap-ratings type="readonly" [active]="defaultComments.stars - 1"></ap-ratings>
	<div class="review-comments-user-wrapper">
		<span class="review-comments-author">{{defaultComments.author}}</span>
		<span class="review-comments-timestamp">{{defaultComments.timestamp}}</span>
	</div>
	<div class="review-comments-title">{{defaultComments.title}}</div>
	<div class="review-comments-content">{{defaultComments.content}}</div>
</div>
```

#### TypeScript

```typescript
defaultComments: any = {
	stars: 4,
	author: 'Jamie Sutton',
	timestamp: '3 hours ago',
	title: 'Review title looks like this',
	content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.'
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

.ap-ratings-demo-review-comments-container {
	background-color: $color-background-container;
	width: 522px;
	padding: $spacing-6;
	box-shadow: $box-shadow-1;
	border-radius: $border-radius-3;
	scrollbar-width: none;

	&::-webkit-scrollbar {
		height: 0;
		width: 0;
	}

	.review-comments-user-wrapper {
		margin: $spacing-2 0 $spacing-4;
		font: $typography-3 'PwC Helvetica Neue';

		.review-comments-author {
			font-weight: $font-weight-2;
			color: $color-text-body;
			margin-right: $spacing-4;
		}

		.review-comments-timestamp {
			color: $color-text-light;
		}
	}

	.review-comments-title {
		font: $font-weight-2 $typography-5 'PwC Helvetica Neue';
		margin-bottom: $spacing-3;
		color: $color-text-heading;
	}

	.review-comments-content {
		font: $typography-3 'PwC Helvetica Neue';
		color: $color-text-body;
	}
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### reviewComments - multi


**Example #8** | **Variation**: reviewComments | **Modifier**: multi | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<div class="ap-ratings-demo-review-comments-container multi" aria-label="Review comments section" tabindex="0">
	<div *ngFor="let comment of multiComments">
		<ap-ratings type="readonly" [active]="comment.stars - 1"></ap-ratings>
		<div class="review-comments-user-wrapper">
			<span class="review-comments-author">{{comment.author}}</span>
			<span class="review-comments-timestamp">{{comment.timestamp}}</span>
		</div>
		<div class="review-comments-title">{{comment.title}}</div>
		<div class="review-comments-content">{{comment.content}}</div>
	</div>
</div>
```

#### TypeScript

```typescript
multiComments: any[] = [
	{
		stars: 2,
		author: 'Jamie Sutton',
		timestamp: '3 hours ago',
		title: 'Review title looks like this',
		content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.'
	},
	{
		stars: 3,
		author: 'Emmanuel Idehen',
		timestamp: '28 minutes ago',
		title: 'Review title looks like this',
		content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.'
	},
	{
		stars: 5,
		author: 'Victor Rouco',
		timestamp: '1 minute ago',
		title: 'Review title looks like this',
		content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.'
	}
];
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

.ap-ratings-demo-review-comments-container {
	background-color: $color-background-container;
	width: 522px;
	padding: $spacing-6;
	box-shadow: $box-shadow-1;
	border-radius: $border-radius-3;
	scrollbar-width: none;

	&::-webkit-scrollbar {
		height: 0;
		width: 0;
	}

	.review-comments-user-wrapper {
		margin: $spacing-2 0 $spacing-4;
		font: $typography-3 'PwC Helvetica Neue';

		.review-comments-author {
			font-weight: $font-weight-2;
			color: $color-text-body;
			margin-right: $spacing-4;
		}

		.review-comments-timestamp {
			color: $color-text-light;
		}
	}

	.review-comments-title {
		font: $font-weight-2 $typography-5 'PwC Helvetica Neue';
		margin-bottom: $spacing-3;
		color: $color-text-heading;
	}

	.review-comments-content {
		font: $typography-3 'PwC Helvetica Neue';
		color: $color-text-body;
	}

	&.multi {
		display: grid;
		row-gap: $spacing-6;
		height: 495px;
		overflow: scroll;
	}
}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### reviewComments - attachment


**Example #9** | **Variation**: reviewComments | **Modifier**: attachment | **State**: None

#### Module Import

```typescript
import { RatingsModule } from '@appkit4/angular-components/ratings';
```

#### HTML Template

```html
<div class="ap-ratings-demo-review-comments-container">
	<ap-ratings type="readonly" [active]="defaultComments.stars - 1"></ap-ratings>
	<div class="review-comments-user-wrapper">
		<span class="review-comments-author">{{defaultComments.author}}</span>
		<span class="review-comments-timestamp">{{defaultComments.timestamp}}</span>
	</div>
	<div class="review-comments-title">{{defaultComments.title}}</div>
	<div class="review-comments-content">{{defaultComments.content}}</div>
	<div class="review-comments-attachment" aria-label="Review comments attachment" tabindex="0">
		<img src="app/images/ratings/Media-1.png" alt="media-1" />
		<img src="app/images/ratings/Media-2.png" alt="media-2" />
		<img src="app/images/ratings/Media-3.svg" alt="media-3" />
	</div>
</div>
```

#### TypeScript

```typescript
defaultComments: any = {
	stars: 4,
	author: 'Jamie Sutton',
	timestamp: '3 hours ago',
	title: 'Review title looks like this',
	content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed mattis urna mauris, at laoreet nisl placerat eu. Aenean varius libero at enim finibus tristique.'
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

.ap-ratings-demo-review-comments-container {
	background-color: $color-background-container;
	width: 522px;
	padding: $spacing-6;
	box-shadow: $box-shadow-1;
	border-radius: $border-radius-3;
	scrollbar-width: none;

	&::-webkit-scrollbar {
		height: 0;
		width: 0;
	}

	.review-comments-user-wrapper {
		margin: $spacing-2 0 $spacing-4;
		font: $typography-3 'PwC Helvetica Neue';

		.review-comments-author {
			font-weight: $font-weight-2;
			color: $color-text-body;
			margin-right: $spacing-4;
		}

		.review-comments-timestamp {
			color: $color-text-light;
		}
	}

	.review-comments-title {
		font: $font-weight-2 $typography-5 'PwC Helvetica Neue';
		margin-bottom: $spacing-3;
		color: $color-text-heading;
	}

	.review-comments-content {
		font: $typography-3 'PwC Helvetica Neue';
		color: $color-text-body;
	}

	.review-comments-attachment {
		margin-top: $spacing-4;
		margin-left: calc(-1 * $spacing-6);
		display: flex;
		column-gap: $spacing-4;
		width: 522px;
		overflow-x: scroll;
		scrollbar-width: none;

		&::-webkit-scrollbar {
			height: 0;
			width: 0;
		}

		img {
			width: 200px;
			height: 120px;
			border-radius: $border-radius-2;

			&:first-child {
				margin-left: $spacing-6;
			}
			&:last-child {
				margin-right: $spacing-6;
			}
		}
	}
}
```

<!-- /EXAMPLE:9 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| type | string: 'baseline'\|'sentiment'\|'readonly' | The type of ratings. | 'baseline' | 4.0.0 |
| total | number | Total number of rating scales. Note: this property only works in baseline ratings and readonly ratings. For sentiment ratings, the total number is fixed at five. | 5 | 4.0.0 |
| active | number | The default active index of ratings | -1 | 4.0.0 |
| sentimentOptions | Array<{label:string,src:string}> | The custom labels and images of sentiment ratings. | - | 4.0.0 |
| slider | boolean | Enable the sentiment slider. Only works when type = 'sentiment'. | false | 4.0.0 |
| sliderLabel | string | The label of sentiment slider. Only works when slider = 'true'. | 'Rate your experience' | 4.0.0 |
| onChange | EventEmitter<any> | Callback to invoke when the rating star is clicked. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->