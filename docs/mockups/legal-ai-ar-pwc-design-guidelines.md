# Web Design Guidelines — Legal AI AR (PwC)

> Version 3.0 — March 2026
> This document defines the visual and interface standards for the **Legal AI AR** platform, aligned with PwC's **AppKit4** design system. **Design ganador**: propuesta Feedback Exchange light (proposal-final-01-07).

---

## 1. Design Principles

PwC's visual identity is based on four fundamental principles:

1. **Human and approachable** — Use of readable and warm typography, natural imagery, and accessible language.
2. **Distinctive** — Warm palette of oranges and neutrals that differentiates PwC from competitors using blue or green.
3. **Consistent** — A single visual system (AppKit4) applied globally without brand variations by product or service.
4. **Professional and trustworthy** — Generous spacing, clear hierarchy, elements without unnecessary ornamentation.

---

## 2. Application Shell — General Structure

### 2.1 Main Layout

The entire application uses the **three-zone shell** pattern (identical to PwC's Feedback Exchange):

```
┌──────────────────────────────────────────────────────────┐
│  HEADER  (height: 4rem / 64px)  — position: fixed top   │
├────────────┬─────────────────────────────────────────────┤
│            │                                             │
│  SIDEBAR   │         MAIN CONTENT AREA                   │
│  (280px)   │         (flex: 1, overflow-y: auto)         │
│            │                                             │
├────────────┴─────────────────────────────────────────────┤
│  FOOTER  (padding: 1rem, border-top)                    │
└──────────────────────────────────────────────────────────┘
```

**Shell rules:**
- The header is `position: fixed; top: 0` with `z-index: 200`.
- The content wrapper has `padding-top: 4rem` to compensate for the fixed header.
- The sidebar **never** collapses on desktop; on mobile it is hidden.
- Each page **does not include** its own `<app-header>`; this lives in the shell's `app.component`.

### 2.2 Layout Tokens

| Token            | Value  | Description                          |
|------------------|--------|--------------------------------------|
| `$header-height` | `64px` | Fixed header height                  |
| `$nav-width`     | `280px`| Fixed sidebar width                  |
| `$z-header`      | `200`  | Header z-index                       |
| `$z-overlay`     | `300`  | Overlay/modal z-index                |

---

## 3. Header (app-header)

### 3.1 Visual Specifications (Design ganador v3.0)

```
Background:      #ffffff
Height:          4rem (64px)
Padding:         0 1rem 0 1.5rem
Box-shadow:      0 0 1px 0 #e8e8e8
```

> **Cambio v3.0**: Header **blanco** (no oscuro). Sin glassmorphism.

### 3.2 Header Content

| Left zone | Right zone |
|-----------|------------|
| Logo `pwc.svg` + texto "Legal AI AR" | Nav (Bienvenida, Búsqueda, Chat) · Iniciar recorrido · Español (Argentina) · Soporte · Avatar |

**Logo PwC:**
```
Asset:          assets/pwc.svg
Height:         1.5rem (24px)
Width:          auto (mantener proporción)
```

**App name:**
```
Font-size:      1rem
Font-weight:    500
Color:          #252525
```

### 3.3 Header Options

- **Nav**: Bienvenida, Búsqueda, Chat — color `#474747`, hover `#D04A02`
- **Iniciar recorrido**: icono + texto, color `#474747`
- **Español (Argentina)**: selector de idioma
- **Soporte**: icono de ayuda
- **Avatar**: `2.5rem` (40px), círculo, fondo `#D04A02`, iniciales en blanco

---

## 4. Sidebar / Navigation (app-sidebar)

### 4.1 Visual Specifications (Design ganador v3.0)

```
Width:      280px (fixed, does not collapse on desktop)
Background: #ffffff
Padding:    1.75rem 1rem 4.5rem
```

> **Cambio v3.0**: Sidebar **blanco** (no `#191919`). Sin secciones etiquetadas.

### 4.2 Navigation Items

Items principales (siempre visibles):

```
■ Bienvenida   (icon: home-outline)
■ Búsqueda     (icon: search-outline)
■ Coach        (icon: chat-outline)
■ Panel        (icon: bar-chart-outline)
■ Admin        (icon: settings-outline) — solo en vistas de administración
```

### 4.3 Navigation Item Styles

```
/* Inactive item */
Display:        flex; align-items: center
Padding:        1rem 1rem 1rem 0.75rem
Height:         3rem
Border-radius:  0.25rem
Color:          #252525
Gap:            1.5rem (icon + label)

/* Item hover */
Background:     rgba(0,0,0,0.04)

/* Active item */
Background:     rgba(208,74,2,0.12)
Color:          #D04A02
Font-weight:    600
```

### 4.4 Iconography (Sidebar)

- **SVG outline**: `stroke`, `fill: none`, `stroke-width: 1.5`
- Tamaño: `24×24px`
- Inactivo: `#474747`
- Activo: `#D04A02`

---

## 5. Color Palette

### 5.1 Primary Colors

| Token                     | Hex       | Main use                                   |
|---------------------------|-----------|---------------------------------------------|
| `$pwc-orange` / `--color-orange` | `#D04A02` | Primary CTAs, active nav border, accents |
| `$pwc-amber`              | `#EB8C00` | Primary button hover, accent gradient       |
| `$pwc-black`              | `#000000` | Maximum weight text                         |
| `$pwc-white`              | `#FFFFFF` | Card and panel backgrounds                  |

### 5.2 Application Background Colors (v3.0)

| Surface           | Token              | Hex       |
|-------------------|--------------------|-----------|
| Page background   | `$pwc-background`  | `#f3f3f3` |
| Cards / Panels    | `$pwc-surface`     | `#FFFFFF` |
| Sidebar           | `$nav-bg`          | `#ffffff` |
| Header            | `$header-bg`       | `#ffffff` |
| Hero gradient     | `—`                | `linear-gradient(135deg, #fff 0%, #fff8f5 100%)` |
| Alternate background | `$pwc-gray-50`  | `#F8F8F8` |

> **Cambio v3.0**: Header y sidebar **blancos**. Hero con gradiente suave hacia tono cálido.

### 5.3 Neutral Colors

| Token            | Hex       | Use                                          |
|------------------|-----------|----------------------------------------------|
| `$pwc-gray-900`  | `#252525` | Heading text                                 |
| `$pwc-gray-800`  | `#474747` | Body text                                    |
| `$pwc-gray-500`  | `#696969` | Secondary text / metadata                    |
| `$pwc-gray-300`  | `#d1d1d1` | Card and input borders                       |
| `$pwc-gray-100`  | `#f3f3f3` | General page background                      |

### 5.4 State Colors

| State    | Color     | Background |
|----------|-----------|------------|
| Success  | `#21812d` | `#e9f5eb`  |
| Warning  | `#7a5a00` | `#fff8d6`  |
| Error    | `#D04A02` | `#fff0eb`  |

---

## 6. Typography

### 6.1 Font Families

| Role        | Font                | Stack                                    |
|-------------|---------------------|------------------------------------------|
| Headings    | **Georgia**         | `Georgia, 'Times New Roman', serif`      |
| UI / Body   | **Arial / Helvetica**| `Arial, Helvetica, sans-serif`          |

### 6.2 Type Scale

| Element       | Font   | Size | Weight | Use                              |
|---------------|--------|------|--------|----------------------------------|
| Page Eyebrow  | Arial  | 11px | 700    | Uppercase label above title      |
| H1 / Page Title| Georgia| 28px | 300    | Main page title                  |
| H2 / Panel Title| Arial | 14px | 600    | Panel/card titles                |
| Body          | Arial  | 14px | 400    | General text                     |
| Body Small    | Arial  | 13px | 400    | Secondary text, excerpts         |
| Meta / Caption| Arial  | 11–12px| 400–600| Metadata, labels, breadcrumbs   |
| Stat / Number | Georgia| 1.875rem| 300  | Highlighted KPIs and statistics   |

### 6.3 Eyebrow Labels (new in v2.0)

```css
font-size: 11px;
font-weight: 700;
text-transform: uppercase;
letter-spacing: 1.5px;
color: #D04A02;
margin-bottom: 0.5rem;
```

Use: above each page title to provide section context ("PwC Legal AI AR Platform", "Tools").

---

## 7. Interface Components

### 7.1 Panels / Cards (ap-panel)

```css
background:    #ffffff;
border:        1px solid #d1d1d1;
border-radius: 0.25rem;          /* new in v2.0: small radius */
box-shadow:    0 2px 8px rgba(0,0,0,0.08);
padding:       1.5rem;
transition:    box-shadow 0.2s;
```

**Panel with side accent:**
```css
position: relative;
overflow: hidden;
/* ::before */
content: '';
position: absolute;
top: 0; left: 0;
width: 3px; height: 100%;
background: #D04A02;
```

**Panel Title Icon:**
```css
width: 28px; height: 28px;
background: #fff0eb;
border-left: 3px solid #D04A02;
border-radius: 0.25rem;
```

> **Change from v1.0**: `border-radius: 0.25rem` (4px) is allowed to follow the AppKit4 guide. Buttons keep `border-radius: 0.25rem` instead of `0px`.

### 7.2 Buttons

#### Primary (CTA)
```css
background:    #D04A02;
color:         #ffffff;
border:        none;
padding:       0.625rem 1.25rem;
height:        2.5rem;
font-size:     14px;
font-weight:   500;
border-radius: 0.25rem;
transition:    background 0.2s;
/* hover: */ background: #EB8C00;
```

#### Secondary (Outline)
```css
background:    transparent;
color:         #252525;
border:        1px solid #d1d1d1;
padding:       0.625rem 1.25rem;
height:        2.5rem;
font-size:     14px;
font-weight:   400;
border-radius: 0.25rem;
/* hover: */ background: #f3f3f3; border-color: #aaa;
```

#### Ghost / Text
```css
background:    transparent;
color:         #696969;
border:        none;
font-size:     13px;
/* hover: */ color: #D04A02;
```

> **Change from v1.0**: Buttons use `border-radius: 0.25rem` (AppKit4). The secondary is "neutral" (gray, not orange outline) to reduce visual saturation.

### 7.3 Inputs

```css
height:        2.5rem;
border:        1px solid #d1d1d1;
border-radius: 0.25rem;
padding:       0 1rem 0 2.5rem;   /* with internal icon */
font-size:     14px;
background:    #f3f3f3;
/* focus: */
border-color:  #D04A02;
box-shadow:    0 0 0 2px rgba(208,74,2,0.12);
background:    #ffffff;
```

### 7.4 Breadcrumb

```css
font-size:  12px;
color:      #696969;
gap:        6px;
/* link: */  color: #D04A02;
/* sep (›): */ color: #aaaaaa;
```

### 7.5 Badges / Tags

```
Primary style (orange):
  background: #fff0eb; border: 1px solid #f5c4b0; color: #D04A02
  padding: 2px 10px; border-radius: 100px; font-size: 11px; font-weight: 600

Neutral style (gray):
  background: #f0f0f0; border: 1px solid #ddd; color: #555
  (same padding and typography)

Success style (green):
  background: #e9f5eb; border: 1px solid #b5ddb9; color: #21812d
```

> **Change from v1.0**: Badges are **pill** (`border-radius: 100px`), not rectangular.

### 7.6 Progress Bar / Loading

```css
height:     3px;
background: linear-gradient(90deg, #D04A02, #EB8C00);
track:      #e8e8e8;
border-radius: 100px;
```

### 7.7 Analysis Sections

Subsection headers within analysis:
```css
font-size:      11px;
font-weight:    700;
text-transform: uppercase;
letter-spacing: 1px;
color:          #D04A02;
display:        flex; align-items: center; gap: 0.5rem;
margin-bottom:  0.75rem;
```

14×14px SVG icons accompany these titles.

### 7.8 Stat Cards (KPI)

```css
.stat-card {
  background:    #ffffff;
  border:        1px solid #d1d1d1;
  border-radius: 0.25rem;
  padding:       1.25rem 1.5rem;
  position:      relative;
  /* left accent */
  &::before { content:''; position:absolute; top:0; left:0;
              width:3px; height:100%; background:#D04A02; }
}
.stat-label  { font-size:11px; font-weight:600; text-transform:uppercase; color:#696969; }
.stat-value  { font-size:1.875rem; font-weight:300; font-family:Georgia,serif; }
.stat-delta  { font-size:11px; color:#21812d; }
```

---

## 8. Spacing and Page Layout

### 8.1 Spacing System

| Token       | Value | Use                                     |
|-------------|-------|-----------------------------------------|
| `$spacing-xs` | 4px | Minimum inline separation               |
| `$spacing-sm` | 8px | Icon padding, small gap                 |
| `$spacing-md` | 16px | Internal padding, input gap             |
| `$spacing-lg` | 24px | Separation between components           |
| `$spacing-xl` | 32px | Section padding, panel padding          |
| `$spacing-xxl` | 48px | Separation between sections            |

### 8.2 Main Content Padding

```css
.main-content {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem 2rem;   /* 24px 32px */
  background: #f3f3f3;
}
```

### 8.3 Typical Grids

| View          | Grid                               |
|---------------|------------------------------------|
| KPI Stats     | `repeat(3, 1fr)`, gap 1rem         |
| Welcome panels| `repeat(2, 1fr)`, gap 1rem         |
| Feature cards | `repeat(3, 1fr)`, gap 1.25rem     |
| Case detail   | `1fr 1fr`, gap 1.25rem             |

---

## 9. Iconography (v3.0)

- Use **outline** SVG (`stroke`, `fill: none`, `stroke-width: 1.5`).
- Sidebar icons: `24×24px`.
- Header icons: `20×20px`.
- Color: inherited from `currentColor`; active in orange `#D04A02`.
- Decorative icons: `aria-hidden="true"`.

### 9.1 Assets (docs/mockups/assets/)

| Asset | Uso |
|-------|-----|
| `pwc.svg` | Logo PwC en header |
| `welcome-legal-ai.svg` | Ilustración hero en Bienvenida |
| `chat-illus.svg` | Ilustración en Chat |

---

## 10. Backgrounds and Surfaces (v3.0)

| Surface          | Color                    | Use                                    |
|------------------|--------------------------|----------------------------------------|
| Page background  | `#f3f3f3`                | General background of content area     |
| Cards / Panels   | `#ffffff`                | Background of all panels               |
| Header           | `#ffffff`                | Top bar (light)                        |
| Sidebar          | `#ffffff`                | Lateral navigation (light)             |
| Alternate panel  | `#fafafa`                | Internal panel header                  |
| Orange tint      | `#fff0eb`                | Primary badge/tag background, icon bg  |
| Hero gradient    | `#fff` → `#fff8f5`       | Hero sections (135deg)                 |
| Footer           | `#f3f3f3`                | Footer, same as page background       |

---

## 11. Footer

```css
padding:       1rem 2rem;
background:    #f3f3f3;
border-top:    1px solid #d1d1d1;
display:       flex; justify-content: space-between; align-items: center;
font-size:     11px;
color:         #696969;
```

Content: copyright on left + links "Terms of use / Privacy / Support" on right.

---

## 12. Locale e idioma

- **Locale**: `es-AR` (Español Argentina)
- **HTML**: `lang="es-AR"`
- Selector de idioma en header: "Español (Argentina)"

---

## 13. Accessibility

- Minimum text/background contrast: **4.5:1** (WCAG AA).
- Minimum interactive text size: **14px**.
- All interactive elements must have `:focus-visible` with `outline: 2px solid #D04A02; outline-offset: 2px`.
- Decorative icons: `aria-hidden="true"`.
- Breadcrumb with `aria-label="Navigation"`.

---

## 14. CSS Variables and SCSS Tokens

```scss
// _pwc-tokens.scss — tokens updated v2.0
$pwc-orange:         #D04A02;
$pwc-amber:          #EB8C00;
$pwc-yellow:         #FFB600;
$pwc-black:          #000000;
$pwc-white:          #FFFFFF;

// AppKit4 grays (aligned with Feedback Exchange)
$pwc-gray-900:       #252525;   // text-heading
$pwc-gray-800:       #474747;   // text-body
$pwc-gray-500:       #696969;   // text-secondary
$pwc-gray-300:       #d1d1d1;   // borders
$pwc-gray-100:       #f3f3f3;   // bg-default
$pwc-gray-50:        #f8f8f8;   // alternate surface

// Nav (v3.0 — light shell)
$nav-bg:             #ffffff;
$nav-text:           #252525;
$nav-text-active:    #D04A02;
$nav-hover-bg:       rgba(0,0,0,0.04);
$nav-active-bg:      rgba(208,74,2,0.12);
$nav-width:          280px;

// Layout
$header-height:      64px;
$border-radius-sm:   0.25rem;  // 4px — AppKit4 standard
$border-radius-pill: 100px;    // badges/tags

// Shadows
$shadow-sm:  0 2px 8px rgba(0,0,0,0.08);
$shadow-md:  0 4px 16px rgba(0,0,0,0.12);

// Fonts
$font-heading: Georgia, 'Times New Roman', serif;
$font-body:    Arial, Helvetica, sans-serif;
```
