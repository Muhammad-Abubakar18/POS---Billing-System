# AGENTS.md — Rules for AI Coding Agents

These rules are **mandatory** for any AI agent (or developer) generating UI code for DrMusa. They are not suggestions. If a rule and a request conflict, the rule wins — flag the conflict instead of silently ignoring the rule.

DrMusa is a **WPF (.NET 8, MVVM)** healthcare desktop application. It must look like professional hospital/clinic software — not a website, not a mobile app, not a generic Bootstrap-style admin panel.

---

## 1. Tokens Are Law

- **Never invent colors.** Every color used in XAML must come from `DESIGN_TOKENS.json` → exposed as WPF `SolidColorBrush` resources in `Colors.xaml`.
- **Never invent font sizes.** Use only the sizes defined in `typography.fontSize`.
- **Never invent spacing.** Use only values from `spacing` (4, 8, 12, 16, 24, 32, 40, 48). No `Margin="7"` or `Padding="10,3,10,3"` guesswork.
- **Never invent corner radius, shadow, or border values.** Pull from `cornerRadius`, `shadow`, `borderWidth`.
- **Never invent control heights.** Buttons, inputs, table rows must match `controlHeight` tokens exactly.
- If a value is genuinely missing from tokens, **stop and add it to `DESIGN_TOKENS.json` first**, then use it. Do not hardcode a one-off.

## 2. Follow Microsoft Fluent 2 Principles

- Flat surfaces, subtle elevation (see `shadow` tokens), minimal chrome.
- Rounded corners are small and consistent (4px controls, 8px cards) — never pill-shaped buttons, never heavy skeuomorphism.
- Motion is subtle and functional, never decorative (see Rule 6).
- Iconography follows Fluent System Icons (see `ICON_GUIDELINES.md`).

## 3. Consistency Across Every Window

- Every screen uses the same top bar, sidebar, spacing rhythm, and typography scale (see `LAYOUT_RULES.md`, `SCREEN_RULES.md`).
- A button styled one way in Billing must look identical in Patient Registration. No per-screen reinvention.
- Reuse shared `UserControl`s / `Style`s instead of duplicating XAML. If you're about to copy-paste a block of XAML for a second screen, extract it into a reusable component instead.

## 4. Enterprise, Not Consumer

- Prefer dense, information-rich, clean layouts over large decorative whitespace-heavy consumer UI.
- No marketing-style hero sections, no illustrations, no playful copy.
- Data tables, forms, and status indicators are the primary UI language of this app.

## 5. No Unnecessary Animation

- Animations are allowed only for: dialog open/close fade, hover/pressed state transitions, loading indicators, toast entrance/exit.
- Duration must come from `animation.durationFast/Normal/Slow` tokens (100/150/250ms).
- No bouncing, no elastic easing, no page-transition slides, no attention-seeking pulsing (except a genuine "critical alert" badge, which must be justified in code comments).

## 6. Avoid Excessive Shadows

- Use the lowest `shadow` token that communicates the needed elevation. Most surfaces use `flat` or `card`. Reserve `dialog` for modals only.
- Never stack multiple shadows or use shadow to fake depth on flat elements like table rows.

## 7. Visual Hierarchy

- One primary action per screen (primary-colored button). All other actions are secondary/tertiary/subtle.
- Headings use `title`/`subtitle` tokens consistently — never bold body text as a substitute for a heading.
- Critical medical status (allergies, critical lab values, urgent flags) always uses `color.medical.critical` and must never be the only color-coded indicator — pair with an icon or label for accessibility.

## 8. Accessible Contrast

- All text/background pairs must meet WCAG AA (4.5:1 for body text, 3:1 for large text/UI components). The tokens in `DESIGN_TOKENS.json` are pre-vetted for this — do not lighten/darken them ad hoc.
- Never convey status by color alone (see Rule 7).
- All interactive controls must have a visible keyboard focus ring (`color.state.focusRing`, `borderWidth.focus`).

## 9. Reusable Components

- Every control type (button, input, card, dialog, badge, etc.) is defined **once** as a `Style` or `UserControl` in `/Themes/` or `/Controls/` and referenced everywhere via `DynamicResource`.
- Screen-specific XAML should only contain layout and data-binding — not new control styling.

## 10. Keep XAML Clean

- No inline `Style` overrides scattered across views. Styles live in ResourceDictionaries.
- No magic numbers in XAML — bind to StaticResource/DynamicResource tokens.
- Keep view files focused on layout; keep visual states in styles/templates.

## 11. Follow MVVM Strictly

- Views contain no business logic. Code-behind is limited to view-only concerns (focus management, animations triggered by visual state).
- All data and commands are exposed via ViewModels using `ICommand` and `INotifyPropertyChanged` (or a MVVM toolkit).
- No direct database or service calls from code-behind.

## 12. Never Hardcode UI Values When Tokens Exist

This is the umbrella rule for 1–11: before writing any numeric or color literal in XAML, check `DESIGN_TOKENS.json` first. If it exists there, reference it. If it doesn't, add it there before using it.

---

## Pre-flight Checklist for Any New Screen

Before generating a new screen or component, confirm:

1. Have I read `DESIGN_TOKENS.json`, `COMPONENT_RULES.md`, and `LAYOUT_RULES.md` for the relevant components?
2. Am I reusing existing styles/components instead of creating new ones?
3. Does this screen match the spacing/typography rhythm of existing screens?
4. Have I checked `DESIGN_CHECKLIST.md` before calling the work done?

If the answer to any of these is "no," stop and fix it before proceeding.
