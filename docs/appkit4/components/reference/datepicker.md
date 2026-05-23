---
component: datepicker
framework: angular
---

# Datepicker Component

## Overview

AppKit Datepicker component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Datepicker component:

- To allow users to select past, present or future date or range of dates.

### Anatomy

**1. Label:** Text in input field that describes to the user the required date. 

**2. Date field:** Allows users to type the desired date. Displays selected date from calendar overlay. 

**3. Icon:** Calendar icon that triggers the calendar overlay.

**4. Calendar:** An overlay grid that displays the days of the month.

**5. Navigation icons:** Allow users to navigate through months and years. 

**6. Date Selection:** A visual cue that indicates the selected date(s).

### Variants

Time picker:

#### Single:

For selecting single date.

#### Double:

Set of two two date pickers for range selection across multiple months.

#### Custom:

For range selection presets.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-122461%26viewport%3D1177%252C-8209%252C0.4%26t%3D1jhygcH6OxsJkn4h-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use the user local settings to show and input the date in correct format
- Date picker calendar popup is not always necessary - especially for dates that are far from current day or the ones which don't require the user to see the context of days.
- Indicate current date
- Keep the length of date picker input fixed instead of stretching full width, so the calendar popup will not be too far away from the actual date.

#### How not to use

- Don't use date picker if users only need day, year or month. Use separate inputs or dropdown instead.
- Don't allow users to select dates which are not relevant to the context.
- Stretch the calendar to the full length of input.

### Behavior

- By default date-date picker is working as an input with Label, after click it will show placeholder with date format.
- By clicking on the calendar icon, user will open calendar with ability to select date or range with mouse or by selecting the day using keyboard.
- Keyboard shortcuts: TAB, arrows, PgUp PgDown, Esc?
- Custom date-picker will set the range according to pre-defined intervals
- Calendar will automatically close after the user makes a selection.

### Accessibility

- Date picker labels should clearly identify what is expected to be inputted by the user.
- Note* Screenreader users usually prefer separate combobox selections for Month, Day, Year.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 10


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | single - singleSelection | `section: "example:1"` |
| 2 | single - dateRange | `section: "example:2"` |
| 3 | double - singleSelection | `section: "example:3"` |
| 4 | double - dateRange | `section: "example:4"` |
| 5 | custom - singleSelection | `section: "example:5"` |
| 6 | custom - dateRange | `section: "example:6"` |
| 7 | timepicker - 12h-inline | `section: "example:7"` |
| 8 | timepicker - 24h-inline | `section: "example:8"` |
| 9 | timepicker - 12h | `section: "example:9"` |
| 10 | timepicker - 24h | `section: "example:10"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### single - singleSelection


**Example #1** | **Variation**: single | **Modifier**: singleSelection | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpType]="'single'" [dpRange]="false" [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### single - dateRange


**Example #2** | **Variation**: single | **Modifier**: dateRange | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date range'" [datepicker]="true" [dpType]="'single'" [dpRange]="true" [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### double - singleSelection


**Example #3** | **Variation**: double | **Modifier**: singleSelection | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpType]="'double'" [dpRange]="false" [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### double - dateRange


**Example #4** | **Variation**: double | **Modifier**: dateRange | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date range'" [datepicker]="true" [dpType]="'double'" [dpRange]="true" [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### custom - singleSelection


**Example #5** | **Variation**: custom | **Modifier**: singleSelection | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date of birth'" [datepicker]="true" [dpType]="'single'" [dpCustom]="true" [dpRange]="false"
  [dpOptions]="datepickerOptions" [dpMinDate]="minDate" [dpMaxDate]="maxDate" [dpDisabledDates]="disabledDates"
  [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
  <div class="ap-datepicker-options">
    <span *ngFor="let option of datepickerOptions" tabindex="0" role="button" type="button">{{option.name}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
datepickerOptions = [
  { name: 'Today', dates: [new Date(), new Date()] },
  { name: 'Yesterday', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 1), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 1)] },
  { name: 'Last 7 days', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 6), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())] },
  { name: 'Last 30 days', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 29), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())] },
  { name: 'This month', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), 1), new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0)] },
  { name: 'Last month', dates: [new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1), new Date(new Date().getFullYear(), new Date().getMonth(), 0)] },
];
minDate = new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate());
maxDate = new Date(new Date().getFullYear() + 1, new Date().getMonth(), new Date().getDate());
disabledDates = [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 2)];

onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.ap-datepicker-options {
    width: 154px;
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### custom - dateRange


**Example #6** | **Variation**: custom | **Modifier**: dateRange | **State**: None

#### Module Import

```typescript
import { FieldModule } from '@appkit4/angular-components/field';
```

#### HTML Template

```html
<ap-field [title]="'Date range'" [datepicker]="true" [dpType]="'single'" [dpCustom]="true" [dpRange]="true"
  [dpOptions]="datepickerOptions" [dpMinDate]="minDate" [dpMaxDate]="maxDate" [dpDisabledDates]="disabledDates"
  [(dpSelectedDates)]="dates" (dpSelectedDatesChange)="onSelectedDatesChange($event)">
  <input appkit-field />
  <div class="ap-datepicker-options">
    <span *ngFor="let option of datepickerOptions" tabindex="0" role="button" type="button">{{option.name}}</span>
  </div>
</ap-field>
```

#### TypeScript

```typescript
dates: Date[] = [];
datepickerOptions = [
  { name: 'Today', dates: [new Date(), new Date()] },
  { name: 'Yesterday', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 1), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 1)] },
  { name: 'Last 7 days', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 6), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())] },
  { name: 'Last 30 days', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 29), new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())] },
  { name: 'This month', dates: [new Date(new Date().getFullYear(), new Date().getMonth(), 1), new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0)] },
  { name: 'Last month', dates: [new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1), new Date(new Date().getFullYear(), new Date().getMonth(), 0)] },
];
minDate = new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate());
maxDate = new Date(new Date().getFullYear() + 1, new Date().getMonth(), new Date().getDate());
disabledDates = [new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 2)];

onSelectedDatesChange(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.ap-datepicker-options {
    width: 154px;
}
```

#### Dependency

```text
"focus-trap": "^5.1.0",
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### timepicker - 12h-inline


**Example #7** | **Variation**: timepicker | **Modifier**: 12h-inline | **State**: None

#### Module Import

```typescript
import { TimepickerModule } from '@appkit4/angular-components/timepicker';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-timepicker [(ngModel)]="time" [inline]="true" [hourTime]="12" (onValueChange)="onValueChange($event)"></ap-timepicker>
```

#### TypeScript

```typescript
time!: Date;
onValueChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### timepicker - 24h-inline


**Example #8** | **Variation**: timepicker | **Modifier**: 24h-inline | **State**: None

#### Module Import

```typescript
import { TimepickerModule } from '@appkit4/angular-components/timepicker';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-timepicker [(ngModel)]="time" [inline]="true" [hourTime]="24" (onValueChange)="onValueChange($event)"></ap-timepicker>
```

#### TypeScript

```typescript
time!: Date;
onValueChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### timepicker - 12h


**Example #9** | **Variation**: timepicker | **Modifier**: 12h | **State**: None

#### Module Import

```typescript
import { TimepickerModule } from '@appkit4/angular-components/timepicker';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-timepicker [(ngModel)]="time" [inline]="false" [hourTime]="12" (onValueChange)="onValueChange($event)"></ap-timepicker>
```

#### TypeScript

```typescript
time!: Date;
onValueChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### timepicker - 24h


**Example #10** | **Variation**: timepicker | **Modifier**: 24h | **State**: None

#### Module Import

```typescript
import { TimepickerModule } from '@appkit4/angular-components/timepicker';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-timepicker [(ngModel)]="time" [inline]="false" [hourTime]="24" (onValueChange)="onValueChange($event)"></ap-timepicker>
```

#### TypeScript

```typescript
time!: Date;
onValueChange(event: any) {
  console.log(event);
}
```

#### Dependency

```text
"ng-click-outside2": "^12.0.0"
```

<!-- /EXAMPLE:10 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-field

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| datepicker | boolean | Enable the Datepicker mode | false | 4.0.0 |
| title | string | The label of the field | '' | 4.0.0 |
| fieldId | string | The id string of input field | Random string of 14 characters in length | 4.0.0 |
| hideTitleOnInput | boolean | Hide title and disable floating animation for the field which has value. | false | 4.5.0 |
| attachedDOM | string | The query selectors for the custom position of datepicker dialog. If the attachedDOM is set, the dialog will not be closed when clicking outside. By default, it is located inside the &lt;ap-field&gt; element. | - | 4.0.0 |
| adaptivePosition | boolean | Enable the adaptive position of the datepicker dialog | false | 4.0.0 |
| disableOutsideClose | boolean | If true, the calendar panel will not be closed when clicking outside. | false | 4.4.0 |
| editable | boolean | Enable manual input of the datepicker.By default, the editable is set to false in the following scenarios:1. dpView='date', dpDateformat is not within the range 'MM/dd/yyyy', 'dd/MM/yyyy', 'yyyy/MM/dd', 'yyyy/dd/MM' and 'dd MMM yyyy'.2. dpView='month', dpDateformat is not within the range 'MM/yyyy', 'yyyy/MM' and 'MMM yyyy'.3. dpView='year', dpDateformat is not 'yyyy'. | true | 4.2.0 |
| error | boolean | Enable the error state of the input. | false | 4.6.0 |
| useCustomValidation | boolean | If true, the built-in validation logic is ignored, which means that there is no error message or error styles by default. You can set the property 'error' and use the callback event 'dpOnDateInvalid' to customize error state of datepicker. For example, HTML: &lt;ap-field \[title\]="'Date'" \[datepicker\]="true" \[useCustomValidation\]="true" \[error\]="error" (dpOnDateInvalid)="onValidate($event)"&gt; &lt;input appkit-field aria-errormessage="errormessage" /&gt; &lt;div \*ngIf="error" class="ap-field-datepicker-error-message-container"&gt; &lt;span class="ap-field-datepicker-error-message" id="errormessage" aria-live="assertive"&gt;Please enter a valid date&lt;/span&gt; &lt;/div&gt; &lt;/ap-field&gt; TypeScript: error = false; onValidate(event: any) { this.error = event.invalid; } | false | 4.6.0 |
| dpView | 'date' \| 'month' \| 'year' | Enable date picker, month picker or year picker | 'date' | 4.4.0 |
| dpRange | boolean | Enable the range dates | true | 4.0.0 |
| dpType | string: 'single'\|'double' | The type of datepicker | 'single' | 4.0.0 |
| dpCustom | boolean | Enable custom mode, used with either single or double datepicker | false | 4.1.0 |
| dpAutoClose | boolean | Whether to hide the dialog on date selection. | true | 4.0.0 |
| dpOptions | Array<{name:string,dates:\[Date,Date\]}> | Options of selectable ranges of dates, apply to customzied datepicker only | - | 4.0.0 |
| dpSelectedDates | Array<Date> | The default selected dates of datepicker and two-way binding is supported. Note that this property cannot be used with angular forms (reactive or template-driven forms). There will be conflicts in values if you use them at the same time. | \[\] | 4.0.0 |
| dpSelectedDatesChange | EventEmitter<Array\[Date\]> | Callback to invoke when the dpSelectedDates changes | - | 4.0.0 |
| dpOnDateInvalid | EventEmitter<{ value: string, invalid: boolean }> | Callback to invoke every time the date changes. Event properties: • value: Current value of the input field. • invalid: State of the value as determined using built-in validation logic. | - | 4.1.0 |
| dpMinDate | Date | The minimum date of datepicker. Day is ignored when dpView='month'. Month and day are ignored when dpView='year'. | - | 4.0.0 |
| dpMaxDate | Date | The maximum date of datepicker. Day is ignored when dpView='month'. Month and day are ignored when dpView='year'. | - | 4.0.0 |
| dpShowOtherMonths | boolean | Whether to display dates in other months (non-selectable) at the start or end of the current month. Not applicable to month year picker. | true | 4.0.0 |
| dpDisabledDates | Array<Date> | Array with dates that should be disabled (not selectable), such as \[new Date(), new Date(2021, 9, 1)\]. Day is ignored when dpView='month'. Month and day are ignored when dpView='year'. | - | 4.0.0 |
| dpDisabledRanges | Array<\[Date, Date\]> | Array with ranges of dates that should be disabled (not selectable), such as \[\[new Date(), new Date(2022, 1, 1)\]\]. Day is ignored when dpView='month'. Month and day are ignored when dpView='year'. | - | 4.0.0 |
| dpDisabledDays | Array<number> | Array with weekday numbers that should be disabled (not selectable), such as \[0, 3, 6\]. Not applicable to month year picker. | - | 4.0.0 |
| dpDateformat | string | The display date format of selected date(s). Refer to Angular DatePipe.Note that, input field is not editable in the following scenarios:1. dpView='date', dpDateformat is not within the range 'MM/dd/yyyy', 'dd/MM/yyyy', 'yyyy/MM/dd', 'yyyy/dd/MM' and 'dd MMM yyyy'.2. dpView='month', dpDateformat is not within the range 'MM/yyyy', 'yyyy/MM' and 'MMM yyyy'.3. dpView='year', dpDateformat is not 'yyyy'. | 'MM/dd/yyyy' | 4.5.0 |
| dpDateSeparator | string | The custom character that separates the month from the day and the day from the year.Note: A space character as a separator is applicable only for the formats 'dd MMM yyyy' and 'MMM yyyy'. This property works in the following scenarios only:1. dpView='date', dpDateformat falls within the range 'MM/dd/yyyy', 'dd/MM/yyyy', 'yyyy/MM/dd' and 'yyyy/dd/MM'.2. dpView='month', dpDateformat falls within the range 'MM/yyyy' and 'yyyy/MM'. | '/' | 4.3.0 |
| dpRangeConnectorFormat | string | The string between two dates in the input field, apply to range mode only. | 'to' | 4.3.0 |
| dpLanguage | string: 'cs' \| 'da' \| 'de' \| 'en' \| 'es' \| 'fi' \| 'fr' \| 'hu' \| 'nl' \| 'pl' \| 'pt-BR' \| 'pt' \| 'ro' \| 'sk' \| 'zh' \| 'ja' \| 'zh-Hant' \| 'ko' | The display language of datepicker. 'ja', 'zh-Hant', 'ko' are available from v4.10.0. Localization support for the datepicker input field (String only) is available from 4.27.0 | - | 4.0.0 |
| dpFirstDayOfWeek | number | The first day of the week. Not applicable to month year picker. | 0 | 4.0.0 |
| dpCalendarPosition | 'top' \| 'top-left' \| 'top-right' \| 'bottom' \| 'bottom-left' \| 'bottom-right' \| 'left' \| 'left-top' \| 'left-bottom' \| 'right' \| 'right-top' \| 'right-bottom' | The default position of the calendar. It doesn't work when 'adaptivePosition' = true. | 'bottom-right' | 4.4.0 |
| dpOnClickNavigators | EventEmitter<{ originalEvent: Event, direction: 'next'\|'prev', view: 'date'\|'month'\|'year', month: number, year: number, yearRangeStart: number \| null }> | Callback to invoke when changing current month and year by the arrow navigators. Note that the parameter 'month' is started at 0. Event properties: • originalEvent: Event. • direction: Whether the next or previous navigator is clicked. • view: Current view when clicking on the navigators. • month: Current month when clicking on the navigators. • year: Current year when clicking on the navigators. • yearRangeStart: Current start year of the 'year' view. | - | 4.4.0 |
| onClose | EventEmitter<Event> | Callback funtion of closing the dialog. | - | 4.4.0 |
| onShow | EventEmitter<Event> | Callback to invoke when opening the dialog. | - | 4.4.0 |
| onClickOutside | EventEmitter<Event> | Callback to invoke when click outside of the field. It won't be triggered if 'attachedDOM' is set for the datepicker. | - | 4.4.0 |
| toggleDatepicker | (event: Event, focusAfterClose?: boolean) => void | Toggles the visibility of the datepicker dialog. Method parameters: • event: Event. • focusAfterClose: Optional. Whether to focus on the input field after calendar closed. | - | 4.4.0 |
| showDatepicker | (event: Event) => void | Open the datepicker dialog. | - | 4.4.0 |
| closeDatepicker | (event: Event, focusAfterClose?: boolean) => void | Close the datepicker dialog. Method parameters: • event: Event. • focusAfterClose: Optional. Whether to focus on the input field after calendar closed. | - | 4.4.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component | '' | 4.0.0 |

### ap-timepicker

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| inline | boolean | Enable inline or default time picker. | true | 4.6.1 |
| title | string | For default time picker only. Title of the input field. | 'Time' | 4.6.1 |
| hourTitle | string | Title of the hour selector. | 'Hours' | 4.6.0 |
| minuteTitle | string | Title of the minute selector. | 'Minutes' | 4.6.0 |
| hourTime | 12 \| 24 | Set 12 hour time or 24 hour time | 12 | 4.6.0 |
| defaultMeridiem | 'AM' \| 'PM' | Default value for the meridiem selector. Applies to 12 hour time only. | 'AM' | 4.6.0 |
| hourIncrements | number | Custom hour increments for the hour selector. | 1 | 4.6.0 |
| minuteIncrements | number | Custom minute increments for the minute selector. | 15 | 4.6.0 |
| minHour | number | The minimum hour to select from. | - | 4.6.0 |
| maxHour | number | The maximum hour to select from. | - | 4.6.0 |
| minMinute | number | The minimum minute to select from. | - | 4.6.0 |
| maxMinute | number | The maximum minute to select from. | - | 4.6.0 |
| error | boolean | The error state for time picker. | false | 4.6.0 |
| readonly | boolean | The readonly state for time picker. | false | 4.6.1 |
| disabled | boolean | The disabled state for time picker. | false | 4.6.1 |
| hideTitleOnInput | boolean | Hide title and disable floating animation for the time picker with value. | false | 4.7.0 |
| hourId | string | For inline time picker only. Id of the hour selector. | 'ap-timepicker-' + Random string of 15 characters in length. | 4.6.0 |
| minuteId | string | For inline time picker only. Id of the minute selector. | 'ap-timepicker-' + Random string of 15 characters in length. | 4.6.0 |
| meridiemId | string | For inline time picker only. Id of the meridiem selector. | 'ap-timepicker-' + Random string of 15 characters in length. | 4.6.0 |
| ngModel | Date | The selected time in Date format. | - | 4.6.0 |
| onValueChange | EventEmitter<Date> | Callback to invoke when the time value changes. | - | 4.6.0 |
| onInput | EventEmitter<{originEvent: Event, inputValue: string, filterResult: Array<{label: string, value: number\|string, disabled?: boolean}>, type: 'hours'\|'minutes'\|'meridiem'}> | For inline time picker only. Callback to invoke on input. Event properties: • originEvent: Event. • inputValue: Current input value. • filterResult: Filtered list of the selections. • type: Selector type. | - | 4.6.0 |
| onClose | EventEmitter<{originEvent: Event, type?: 'hours'\|'minutes'\|'meridiem'}> | Callback to invoke when closing the dropdown. Event properties: • originEvent: Event. • type: For inline time picker only. Selector type. | - | 4.6.0 |
| style | string | The inline style of the component | '' | 4.6.0 |
| styleClass | string | The style class names of the component | '' | 4.6.0 |
| attachedDOM | string | The query selectors for the custom position of the time picker dialog. | - | 4.9.1 |
| adaptivePosition | boolean | Enable the adaptive position of the time picker dialog. | false | 4.9.1 |
| closeTimepicker | EventEmitter<Event> | Since 'attachedDOM' is used and when clicking outside, the dropdown can't be closed. Please use ViewChild to call this function. | - | 4.9.1 |


<!-- /SECTION:properties -->