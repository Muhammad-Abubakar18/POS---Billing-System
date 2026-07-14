# Typography — DrMusa Design System

Font family: **Segoe UI Variable** (fallback: Segoe UI). This is the native Windows 11 / Fluent 2 typeface and keeps DrMusa feeling like real desktop software.

All values below correspond exactly to `DESIGN_TOKENS.json → typography`. Never deviate from this table.

## Type Scale

| Role | Token | Size (px) | Weight | Line Height | Usage |
|---|---|---|---|---|---|
| Display Large | `displayLarge` | 40 | SemiBold | 52 | Login screen app name only |
| Display Small | `displaySmall` | 32 | SemiBold | 40 | Splash / large empty-state headline |
| Title Large | `titleLarge` | 28 | SemiBold | 36 | Rarely used — major report titles |
| Page Title | `title` | 24 | SemiBold | 32 | Top of every screen (e.g. "Patient Registration") |
| Subtitle Large | `subtitleLarge` | 20 | SemiBold | 28 | Dashboard section headers |
| Subtitle | `subtitle` | 18 | SemiBold | 24 | Card headers, dialog titles |
| Body Large | `bodyLarge` | 16 | Regular | 22 | Emphasized body text, key values (totals, prices) |
| Body | `body` | 14 | Regular | 20 | Default UI text — labels, table cells, form fields |
| Body Strong | `bodyStrong` | 14 | SemiBold | 20 | Emphasized labels, active nav item, table header |
| Caption | `caption` | 12 | Regular | 16 | Helper text, timestamps, secondary metadata |
| Caption Small | `captionSmall` | 11 | Regular | 14 | Badge text, footnotes |

## Font Weight Usage

| Weight | Value | When to use |
|---|---|---|
| Regular | 400 | Default for all body copy, table data, placeholder text |
| Medium | 500 | Rarely; slight emphasis without full SemiBold weight (e.g. selected tab label) |
| SemiBold | 600 | Headings, section titles, primary button labels, active nav items, key numeric values (totals, patient ID) |
| Bold | 700 | Reserved for critical alerts / medical warnings only (e.g. "ALLERGY" badge text). Never used for general emphasis. |

**Rule of thumb:** SemiBold is DrMusa's default emphasis weight. True Bold (700) is reserved exclusively for safety-critical medical flags so it retains urgency.

## Line Height & Letter Spacing

- Line heights are fixed per size (see table) — do not use "auto" line height, to keep vertical rhythm predictable across screens.
- Letter spacing: `normal` (0) for all UI text. `tight` (-0.2) may be used only on `displayLarge`/`displaySmall` for large headline text. `wide` (0.2) is used only for all-caps labels (e.g. section eyebrow labels, status badges in caps).

## Hierarchy Examples

**Screen header block:**
```
Patient Registration                    ← title (24/SemiBold)
Add a new patient record                ← caption (12/Regular, text.secondary)
```

**Card:**
```
Today's Appointments                    ← subtitle (18/SemiBold)
12 scheduled · 3 completed              ← caption (12/Regular, text.secondary)
```

**Data table:**
```
PATIENT NAME   |   AGE   |   STATUS      ← bodyStrong (14/SemiBold), letter-spacing wide, uppercase, text.secondary
John Doe       |   34    |   Stable      ← body (14/Regular), text.primary
```

**Form field:**
```
Full Name *                             ← body (14/SemiBold) label
[ input field ]                         ← body (14/Regular) input text
Enter patient's legal full name         ← caption (12/Regular, text.tertiary) helper text
```

**KPI / stat card:**
```
248                                      ← displaySmall or title, SemiBold, text.primary
Total Patients Today                    ← caption, text.secondary
```

## Rules

- Never use more than 3 type sizes on a single screen's primary content area (e.g. `title` + `body` + `caption`).
- Never fake a heading with bold body text — always step up to the correct type token.
- Do not use italics anywhere in the product; it reads as inconsistent with Fluent 2 and reduces legibility in dense medical tables.
- All-caps text is reserved for table column headers and small status badges only — never for buttons or body copy.
