---
styles: styles
framework: angular
---

# Styles and Themes

AppKit provides 6 color themes with light and dark mode variants.

---

## 1. Available Themes

| Theme | Primary Color | SCSS File (Light) | SCSS File (Dark) |
|-------|---------------|-------------------|------------------|
| **Blue** (Default) | `#415385` | `index.scss` | `index.dark.scss` |
| Orange | `#D04A02` | `themes/index.orange.scss` | `themes/index.orange.dark.scss` |
| Teal | `#26776D` | `themes/index.teal.scss` | `themes/index.teal.dark.scss` |
| Pink | `#D93954` | `themes/index.pink.scss` | `themes/index.pink.dark.scss` |
| Red | `#E0301E` | `themes/index.red.scss` | `themes/index.red.dark.scss` |
| Black | `#2D2D2D` | `themes/index.black.scss` | `themes/index.black.dark.scss` |

---

## 2. Theme Import Methods

### SCSS Import (Recommended for Angular)

Best for full customization and access to design tokens.

```scss
// Required: Configure asset paths BEFORE importing theme
$ap-image-path: '/assets/appkit/images';
$ap-font-path: '/assets/appkit/font';
$ap-font-icon-path: '/assets/appkit/font-icon';

// Import theme (choose ONE)
@import '@appkit4/styles/scss/index.scss';                    // Blue Light (default)
@import '@appkit4/styles/scss/index.dark.scss';               // Blue Dark
@import '@appkit4/styles/scss/themes/index.orange.scss';      // Orange Light
@import '@appkit4/styles/scss/themes/index.orange.dark.scss'; // Orange Dark
// Same pattern for: teal, pink, red, black

// Optional: Import for custom styling
@import '@appkit4/styles/scss/variables';  // Design tokens
@import '@appkit4/styles/scss/mixin';      // Mixins like setElevation()
```


### CDN Import

<!-- VERSION:cdn-styles -->
```html
<link rel="stylesheet" href="https://appkitcdn.pwc.com/appkit4/cdn/styles/4.11.0/appkit.min.css">
<link rel="stylesheet" href="https://appkitcdn.pwc.com/appkit4/cdn/styles/4.11.0/themes/appkit.orange.min.css">
```
<!-- /VERSION:cdn-styles -->

---

## 3. Design Tokens

### Color Tokens

| Category | Token | Usage |
|----------|-------|-------|
| **Background** | `$color-background-default` | App background |
| | `$color-background-container` | Card/panel background |
| | `$color-background-primary` | Theme primary color |
| | `$color-background-hover` | Hover state |
| | `$color-background-selected` | Selected state |
| | `$color-background-secondary` | Lighter backgrounds |
| **Text** | `$color-text-heading` | Headings |
| | `$color-text-body` | Body text |
| | `$color-text-primary` | Theme primary text |
| | `$color-text-link-primary` | Links |
| | `$color-text-white` | White text |
| **Status** | `$color-background-danger` | Error (#C52A1A) |
| | `$color-background-success` | Success (#21812D) |
| | `$color-background-warning` | Warning (#FFBF1F) |

### Spacing Tokens (8px base)

| Token | Value | Token | Value |
|-------|-------|-------|-------|
| `$spacing-1` | 2px | `$spacing-5` | 16px (default) |
| `$spacing-2` | 4px | `$spacing-6` | 20px |
| `$spacing-3` | 8px | `$spacing-7` | 24px |
| `$spacing-4` | 12px | `$spacing-8` | 48px |

### Typography Tokens

| Token | Font-size | Line-height | Usage |
|-------|-----------|-------------|-------|
| `$typography-body-xs` | 12px | 14px | Helper text, captions |
| `$typography-body-s` | 14px | 20px | Tooltips, secondary text |
| `$typography-body` | 16px | 24px | Default body text |
| `$typography-heading-s` | 20px | 24px | Sub-section headings |
| `$typography-heading-m` | 24px | 32px | Screen titles |
| `$typography-heading-l` | 36px | 42px | Top level headings |
| `$typography-data` | 48px | 48px | Data visualization, metrics |

| Token | Value |
|-------|-------|
| `$font-weight-1` | 400 (Regular) |
| `$font-weight-2` | 500 (Medium) |
| `$font-weight-3` | 700 (Bold) |

### Border & Elevation Tokens

| Token | Value |
|-------|-------|
| `$border-radius-1` | 2px |
| `$border-radius-2` | 4px |
| `$border-radius-3` | 8px |
| `$elevation-shadow-flat` | None |
| `$elevation-shadow-low` | Cards, inputs |
| `$elevation-shadow-medium` | Dropdowns, panels |
| `$elevation-shadow-high` | Modals, drawers |

---

## 4. Grid System

12-column responsive grid with 4 breakpoints:

| Breakpoint | Width Range | Columns |
|------------|-------------|---------|
| Large Desktop | 1440px+ | 12 |
| Small Desktop | 1240px - 1439px | 12 |
| Tablet | 600px - 1239px | 8 |
| Mobile | 0px - 599px | 4 |

```html
<div class="ap-container">
  <div class="row">
    <!-- Full width on mobile, half on tablet, third on desktop -->
    <div class="col-12 col-md-4 col-lg-4">Column 1</div>
    <div class="col-12 col-md-4 col-lg-4">Column 2</div>
    <div class="col-12 col-md-4 col-lg-4">Column 3</div>
  </div>
</div>
```

---

## 5. Elevation Mixin

```scss
@import '@appkit4/styles/scss/mixin';

.my-card {
  @include setElevation('low');     // Cards, buttons
  @include setElevation('medium');  // Dropdowns, tooltips
  @include setElevation('high');    // Modals, drawers
}
```

---

## 6. Quick Reference

| Use Case | Token/Class |
|----------|-------------|
| Primary button background | `$color-background-primary` |
| Body text | `$color-text-body` |
| Card padding | `$spacing-5` (16px) |
| Card shadow | `@include setElevation('low')` |
| Heading font | `$font-weight-2` (500) |
| Error state | `$color-background-danger` |
| Border radius | `$border-radius-2` (4px) |
| Large metrics | `$typography-data` |
| Page titles | `$typography-heading-m` |

---

## 7. Rules Summary

### ✅ ALWAYS

- Use SCSS variables instead of hardcoded hex/px values
- Use `setElevation()` mixin for shadows
- Configure asset paths before importing theme
- Use pre-defined state tokens (`$color-background-hover`, `$color-background-selected`)
- Use AppKit grid system (`ap-container`, `row`, `col-*`) for layouts
- Use typography tokens for font sizes
- Import `@appkit4/styles/scss/variables` in each SCSS file that uses tokens

### ❌ NEVER

- Override fonts - AppKit includes PwC-approved fonts
- Use SCSS color functions (`lighten()`, `darken()`, `rgba()`) on tokens
- Mix multiple themes in the same app
- Hardcode colors (`#415385`, `#FFFFFF`, `white`)
- Hardcode font-size values (`14px`, `32px`)
- Use custom CSS Grid - use AppKit grid system
- Add custom hover transforms
- Hardcode widths/heights

---

## 8. Common Mistakes

### 8.1 Font Override

```scss
// ❌ WRONG - Remove these from default templates
body {
  font-family: system-ui, -apple-system, sans-serif;
}

// ✅ CORRECT - No font-family needed, AppKit applies fonts automatically
```


**Correct Angular `styles.scss`:**

```scss
$ap-image-path: '/assets/appkit/images';
$ap-font-path: '/assets/appkit/font';
$ap-font-icon-path: '/assets/appkit/font-icon';

@import '@appkit4/styles/scss/index.scss';

body { margin: 0; }
```

### 8.2 Hardcoded Values

```scss
// ❌ WRONG
body { background-color: #FFFFFF; color: #333333; }
.badge { color: white; padding: 8px 16px; }

// ✅ CORRECT
@import '@appkit4/styles/scss/variables';
body { background-color: $color-background-default; color: $color-text-body; }
.badge { color: $color-text-white; padding: $spacing-2 $spacing-4; }
```

### 8.3 Hardcoded Font Sizes

```scss
// ❌ WRONG
.metric-value { font-size: 32px; }
.page-title { font-size: 24px; }

// ✅ CORRECT
.metric-value { font-size: $typography-data; }
.page-title { font-size: $typography-heading-m; }
```

### 8.4 Missing Variable Import

```scss
// ❌ ERROR: "Undefined variable $spacing-8"
.my-component { padding: $spacing-8; }

// ✅ CORRECT: Import at top of EACH file
@import '@appkit4/styles/scss/variables';
.my-component { padding: $spacing-8; }
```

### 8.5 SCSS Color Functions on Tokens

AppKit tokens are CSS custom properties. SCSS functions run at compile time and cannot process them.

```scss
// ❌ COMPILATION ERROR
background: lighten($color-background-primary, 10%);
background: rgba($color-background-primary, 0.5);

// ✅ USE PRE-DEFINED STATE TOKENS
background: $color-background-hover;
background: $color-background-secondary;
opacity: $opacity-3;
```

### 8.6 Custom CSS Grid

```scss
// ❌ WRONG
.dashboard-grid { display: grid; grid-template-columns: repeat(3, 1fr); }

// ✅ CORRECT - Use AppKit grid classes
```

```html
<div class="ap-container">
  <div class="row">
    <div class="col-6">Half</div>
    <div class="col-6">Half</div>
  </div>
</div>
```

### 8.7 Custom Hover Transforms

```scss
// ❌ WRONG
.card:hover { transform: translateY(-4px); transition: all 0.3s ease; }

// ✅ CORRECT - Use elevation only
.card {
  box-shadow: $elevation-shadow-low;
  &:hover { box-shadow: $elevation-shadow-medium; }
}
```

### 8.8 Hardcoded Dimensions

```scss
// ❌ WRONG
.search-box { width: 400px; }
.card { min-height: 280px; }

// ✅ CORRECT
.search-box { width: 100%; max-width: 25rem; }
// Or use grid columns for sizing
```

### 8.9 Custom Icon Sizes

```scss
// ❌ WRONG
.icon { font-size: 56px; }

// ✅ CORRECT - Use AppKit icon size classes
// 16px: ap-font-16 ap-container-16
// 24px: ap-font-24 ap-container-24 (default)
// 32px: ap-font-32 ap-container-32
// 48px: ap-font-40 ap-container-40
```

### 8.10 Fixed Widths and Custom Breakpoints

```scss
// ❌ WRONG
.container { width: 1200px; }
@media (max-width: 768px) { .sidebar { display: none; } }

// ✅ CORRECT - Use AppKit breakpoints
@import '@appkit4/styles/scss/variables';
.my-component {
  padding: $spacing-5;
  @media (max-width: 1239px) { padding: $spacing-4; }  // Tablet
  @media (max-width: 599px) { padding: $spacing-3; }   // Mobile
}
```