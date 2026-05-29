---
styles: typography
framework: angular
---

# Typography

Typography is a fundamental element of Appkit's design system, ensuring consistent and readable text across all interfaces. Helvetica Neue is used as the primary typeface throughout the system, providing a versatile and professional appearance for both headers and paragraph text.

## Typeface

Helvetica Neue is a versatile font with a wide array of weights available. We use the same font family across all elements of our content, for both headers and paragraph text. We constrain our font weights to 3 choices in our product design to create simplicity.

**Font Family:** Helvetica Neue

> **Note:** Font files are available for download. [Download Helvetica Neue fonts](https://appkit.pwc.com/appkit4/fonts/fonts.zip)

## Scale

Use the type scale to establish visual hierarchy through size to support your communications and experiences. Appkit font scale ensures that different sizes of text can work together harmoniously.

| Style | Font-size | Line-height | Font-weight | Letter-spacing | Usage |
|-------|-----------|-------------|-------------|----------------|-------|
| Body XS | 12px | 14px | 400 | -0.2px | Used in some component-specific instances. Helper text. |
| Body S | 14px | 20px | 400 | -0.2px | Tooltips. |
| Body | 16px | 24px | 400 | - | Default body style. Blocks of texts. Components. |
| Heading S | 20px | 24px | 500 | - | Sub-section headings. Lower level headings. |
| Heading M | 24px | 32px | 500 | - | Screen titles. Large headings. Headings that identify key functionality. |
| Heading L | 36px | 42px | 500 | - | Screen titles. Top level headings. *Mobile headings. |
| Data | 48px | 48px | 500 | -0.8px | Data visualization. Oversized screen titles. *Use in moderation. |

## Weight

We offer 3 different text weights to add visual clarity and adjust voice or meaning.

| Weight | Value | Usage |
|--------|-------|-------|
| Regular | 400 | Default weight for body text and most content |
| Medium | 500 | Used for headings and emphasis |
| Bold | 700 | Used for strong emphasis and important callouts |

## Font color

We have three primary tokens to assign text colors. These color values provide enough contrast to our suggested background color (`#F3F3F3`) to meet WCAG's contrast standards.

| Token | Hex | Description |
|-------|-----|-------------|
| `$font-color-1` | `#696969` | Lighter font color |
| `$font-color-2` | `#474747` | Medium font color |
| `$font-color-3` | `#252525` | Darker font color |

## Pairing styles

The following are likely combinations you will use in your layouts. Follow this guide to spacing and combining styles. These guidelines go hand in hand with the Appkit spacing styleguide.

### Heading, subheading and body

When combining headings, subheadings, and body text:

- **Heading to body:** Use 20px spacing between heading and body text
- **Subheading to body:** Use 8px spacing between subheading and body text
- **Section breaks:** Use 48px spacing between major sections

These spacing guidelines ensure proper visual hierarchy and readability while maintaining consistency with the Appkit spacing system.

## Line length

Wide lines of text are difficult to read and make it harder for people to focus. While there is no right way to measure the perfect width for text, a good goal is to aim for between 60 and 100 characters per line including spacing.

### Guidelines

- **Ideal range:** 60-100 characters per line (including spacing)
- **Acceptable range:** 40-84 characters per line
- **Too long:** Lines over 120 characters should be avoided

Maintaining appropriate line length improves readability and helps users maintain focus while reading content.