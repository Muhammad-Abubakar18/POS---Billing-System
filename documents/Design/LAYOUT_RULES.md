# Layout Rules — DrMusa Design System

## 8-Point Spacing System

All spacing in DrMusa is built on a 4px base unit, scaling in the following steps (from `DESIGN_TOKENS.json → spacing`):

| Token | Value | Typical use |
|---|---|---|
| `xxs` | 4px | Icon-to-text gap, tight internal padding |
| `xs` | 8px | Gap between related inline elements (badge + label) |
| `sm` | 12px | Input padding, gap between form fields in a row |
| `md` | 16px | Standard component padding, gap between cards in a grid |
| `lg` | 24px | Card internal padding, section spacing |
| `xl` | 32px | Gap between major page sections |
| `xxl` | 40px | Page-level top/bottom margins |
| `xxxl` | 48px | Rare — large dashboard section breaks |

**Rule:** Never use a spacing value not on this list. If something looks like it needs "10px," use `sm` (12px) — consistency beats pixel-perfection to an arbitrary number.

## Window & Page Structure

- Minimum application window size: 1280×720 (`window.minWidth/minHeight`). Standard target: 1366×768.
- Page structure, top to bottom: **Top Bar** (56px, fixed) → **Body** (Sidebar + Content, fills remaining height).
- Content area padding: `spacing.xl` (32px) on all sides for standard screens; `spacing.lg` (24px) for dense data screens (billing, POS cart).

## Grid Usage

- Use a 12-column responsive grid within the content area for dashboards and form layouts.
- Gutter between columns: `spacing.md` (16px).
- Cards/panels snap to column boundaries — never float at arbitrary widths.
- Common patterns: 4 KPI cards = 3 columns each; 2-column form = 6 columns each; primary content + side panel = 8/4 split.

## Margins & Padding

- Outer page margin: `spacing.xl` (32px) from window edge to content.
- Card-to-card gap: `spacing.md` (16px).
- Section-to-section vertical gap: `spacing.xl` (32px).
- Form field-to-field gap: `spacing.md` (16px) vertical, `spacing.lg` (24px) horizontal between columns.

## Alignment

- Left-align all text and form labels (no centered body content — this isn't a marketing site).
- Right-align numeric table columns (currency, quantities, IDs where sortable).
- Vertically center content within fixed-height rows (table rows, toolbar items, list items).

## Containers, Cards, Sections

- **Container** = the scrollable content region beneath the top bar, bounded by sidebar.
- **Section** = a logical grouping within a container, introduced by a `subtitleLarge`/`subtitle` heading, `spacing.xl` from adjacent sections.
- **Card** = a bounded, elevated (`shadow.card`) unit within a section — see `COMPONENT_RULES.md`.

## Page Structure Patterns

### Dashboard Layout
```
[Top Bar]
[Sidebar] | [Page Title + date/context]
          | [KPI card row — 3-4 cards, equal width, spacing.md gap]
          | [Two-column: chart/list (8 col) + activity feed (4 col)]
          | [Recent records table, full width]
```

### Form Layout (e.g. Patient Registration)
```
[Top Bar]
[Sidebar] | [Page Title + breadcrumb]
          | [Card: form section 1 — 2-column field grid]
          | [Card: form section 2 — 2-column field grid]
          | [Sticky footer bar: Cancel (secondary) + Save (primary), right-aligned]
```

### Table/List Layout (e.g. Patient List, Product List)
```
[Top Bar]
[Sidebar] | [Page Title + primary action button, right-aligned]
          | [Toolbar: search + filters, left | export/actions, right]
          | [DataGrid, full width, sticky header]
          | [Pagination footer]
```

### Split Panel Layout (e.g. POS Billing screen)
```
[Top Bar]
[Sidebar] | [Left panel 65%: product grid/search]  [Right panel 35%: cart + totals + Print button]
```
Split panels use a fixed divider (no arbitrary drag-resize) unless a screen explicitly requires it; ratio defined per screen in `SCREEN_RULES.md`.

### Navigation Layout
Sidebar is persistent and fixed-width (`window.sidebarWidth`) on all screens except full-screen modes (e.g. printable receipt preview). Collapse to icon-only (`sidebarCollapsedWidth`) is a user preference, not a per-screen decision — must behave identically everywhere.

## Responsive Behavior (Desktop)

DrMusa targets fixed desktop/POS terminal use, not arbitrary window resizing like a website. Still:
- Below `window.minWidth` (1280px), the app should not allow further shrinking (set `MinWidth` on the main window).
- Between min and standard width, content reflows: 12-column grid compresses gutters proportionally; card grids drop from 4→3→2 columns rather than shrinking card content illegibly.
- Sidebar auto-collapses to icon-only below 1366px width to preserve content space.
