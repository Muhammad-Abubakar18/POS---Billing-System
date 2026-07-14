# Icon Guidelines — DrMusa Design System

## Icon Set

Use **Microsoft Fluent System Icons** (the same family used in Windows 11, Microsoft 365, and Fluent 2 apps) exclusively. Do not mix icon sets — this is one of the fastest ways to make an app look inconsistent.

- Prefer the **Regular** style for default UI icons.
- Use the **Filled** style only for the active/selected state (e.g. active sidebar nav item switches Regular → Filled) — a standard Fluent 2 convention.
- Source: `Segoe Fluent Icons` font (built into Windows 11) or the Fluent UI System Icons NuGet/SVG package for WPF projects not relying on the font glyph approach.

## Sizes

From `DESIGN_TOKENS.json → iconSize`:

| Token | Size | Usage |
|---|---|---|
| `xs` | 12px | Inline badge icons, tiny status dots |
| `sm` | 16px | Inside text boxes (search, calendar), table row inline actions, tooltips |
| `md` | 20px | Sidebar navigation, toolbar buttons, form field icons |
| `lg` | 24px | Top bar icons (notifications, profile), card header icons |
| `xl` | 32px | Empty-state illustrations, dashboard KPI card icons |
| `xxl` | 48px | Large empty states, onboarding/login screen |

**Never use an icon size outside this list.** If a spot looks like it needs 18px, use `sm` (16px) — do not scale arbitrarily.

## Placement & Spacing

- Icon + label (button, nav item): icon on the left, `spacing.xxs` (4px) gap to text, vertically centered.
- Icon-only buttons: icon centered in a minimum 32×32px hit area, even if the icon itself is 16–20px.
- Leading icons in text boxes: positioned at `spacing.sm` (12px) from the left edge, vertically centered.
- Trailing icons (dropdown chevron, clear/×): `spacing.sm` from the right edge.

## Color

- Default icon color: `text.secondary` for neutral/inactive icons.
- Active/selected icon color: `brand.primary`.
- Semantic icons (success/warning/danger/info) use the matching `semantic.*` color — never a neutral gray on a colored badge/toast.
- Icons on a solid colored background (e.g. inside a primary button) use `onPrimary` (white).
- Disabled icons: `text.disabled` at `opacity.disabled`.

## Usage Rules

- **Do** use icons to reinforce meaning that's also conveyed in text or color — never as the sole indicator of a critical state (accessibility).
- **Do** keep icon strokes/weight consistent — Fluent icons use a uniform stroke width; don't mix in a heavier or thinner custom icon.
- **Do** use module-representative icons consistently across the app: e.g. the same "patient" icon glyph is used in the sidebar, on patient cards, and in the patient table — never swapped for a different glyph mid-app.
- **Avoid** decorative icons that don't map to a real action or status — every icon in DrMusa should be functional.
- **Avoid** colorful multi-color icon sets (emoji-style icons) — this breaks the clinical, professional Fluent 2 tone. Icons are monochrome, tinted via the color rules above.
- **Avoid** animating icons except for legitimate loading spinners.

## Examples

| Context | Icon | Size | Color |
|---|---|---|---|
| Sidebar "Patients" nav (inactive) | Person (Regular) | `md` (20px) | `text.secondary` |
| Sidebar "Patients" nav (active) | Person (Filled) | `md` (20px) | `brand.primary` |
| "Print Receipt" button | Printer (Regular) | `sm` (16px) | `onPrimary` (white, inside primary button) |
| Critical status badge | Warning triangle (Filled) | `xs`/`sm` | `medical.critical` |
| Search box | Magnifying glass (Regular) | `sm` (16px) | `text.tertiary` |
| Empty state ("No appointments today") | Calendar (Regular) | `xl` (32px) | `text.tertiary` |
| Top bar notifications | Bell (Regular) | `lg` (24px) | `text.secondary`, with a `semantic.danger` dot badge if unread |
