---
title: Controls Gallery Demo Spec — Overview & Shell
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery Demo Spec — Overview & Shell

## Goal

Build a complete, multi-page demonstration application — the **Controls Gallery** —
that exercises every control in the FS.Skia.UI catalog, drives them with real
pointer and keyboard input, and renders them under a single, cohesive, attractive
color palette. This overview defines the shared application shell, the navigation
model, the palette, and the pointer interaction contract that every page spec in
this folder depends on. Each page spec (`01-…` through `10-…`) covers one category
of controls and inherits everything defined here.

## Scope

The gallery must demonstrate all 52 supported controls from
`src/Controls/Catalog.fs`, distributed across ten content pages:

| Page | Spec | Controls demonstrated |
|------|------|-----------------------|
| Display & Typography | `01-display-and-typography.md` | text-block, rich-text, label, image, icon, separator, badge |
| Buttons & Commands | `02-buttons-and-commands.md` | button, icon-button, toggle-button, split-button |
| Text & Numeric Input | `03-text-and-numeric-input.md` | text-box, text-area, numeric-input, slider, date-picker, time-picker |
| Selection & Toggles | `04-selection-and-toggles.md` | check-box, radio-group, switch, list-box, multi-select-list, combo-box, color-picker |
| Data & Collections | `05-data-and-collections.md` | list-view, tree-view, data-grid |
| Layout & Containers | `06-layout-and-containers.md` | stack, grid, dock, wrap, border, panel, scroll-viewer, split-view |
| Navigation & Menus | `07-navigation-and-menus.md` | tabs, menu, context-menu, toolbar |
| Overlays & Feedback | `08-overlays-and-feedback.md` | tooltip, dialog, overlay, toast, progress-bar, spinner, validation-message |
| Charts & Graphs | `09-charts-and-graphs.md` | line-chart, bar-chart, pie-chart, scatter-plot, graph-view |
| Pointer Playground & Custom | `10-pointer-playground-and-custom.md` | custom-control plus the full pointer-event surface |

Every catalog control must appear on exactly one page and be reachable by
navigation from the shell.

## User Experience

The app opens to the **Display & Typography** page with the navigation rail
selected on that entry and the Light palette active. The user moves between pages
by clicking entries in a persistent left navigation rail (or via keyboard), and
every page presents its controls live and interactive — clickable, hoverable,
editable — rather than as static screenshots. A top app bar carries the gallery
title, a theme toggle, and an accent-palette selector. A bottom status strip
narrates the most recent pointer and focus activity so behavior is legible in
both interactive use and captured evidence. The experience must be readable, fast
to understand, and deterministic under a seed.

## Application Shell

The shell is a `Dock` (or equivalent) composition with four fixed regions that
persist across every page:

- **Top app bar** (docked top): the title `FS.Skia.UI — Controls Gallery`, a
  `Switch` or `ToggleButton` that flips Light/Dark, and a `ComboBox` that selects
  the accent palette (Indigo, Teal, Rose). The bar keeps a stable height.
- **Navigation rail** (docked left): a vertical, single-selection list — modeled
  as a `ListBox` or `Menu` — with one entry per content page. The selected entry
  is visually distinct; hovering an entry previews the hover state; clicking an
  entry navigates. The rail keeps a stable width.
- **Content region** (fill): hosts the active page inside a `ScrollViewer` so long
  pages scroll without resizing the shell.
- **Status strip** (docked bottom): shows the active page, the last pointer
  interaction (kind, target control id, and coordinates), the focused control id,
  and the last dispatched message. It keeps a stable height.

Navigation is driven by a single `ActivePage` field in the model; the content
region renders `match model.ActivePage with …`. No page may alter the dimensions
of the bar, rail, or status strip.

## Color Palette

The gallery ships one cohesive palette — **Indigo & Teal on Slate** — expressed
through the `Theme` record (`Foreground`, `Background`, `Accent`, `Danger`,
`Muted`) and the DTCG design-token source (`src/Controls/design-tokens.tokens.json`,
typed surface `DesignTokens.Light` / `DesignTokens.Dark`). Accent overrides apply
through `Theme.withAccent`; density through `Theme.withDensity`.

### Light theme

| Role | Hex | Usage |
|------|-----|-------|
| Canvas / background | `#F6F7F9` | Window and content backdrop |
| Surface | `#FFFFFF` | Cards, panels, input fields |
| Surface raised | `#EEF1F5` | App bar, nav rail, headers |
| Foreground | `#1B2330` | Primary text |
| Foreground muted | `#5B6675` | Secondary text, captions |
| Border / divider | `#DCE1E8` | Separators, field borders |
| Accent / primary | `#4F46E5` | Primary actions, selection |
| Accent hover | `#4338CA` | Hovered primary actions |
| Accent soft | `#EEF0FE` | Selected-row and tint fills |
| Secondary | `#0EA5E9` | Secondary emphasis |
| Success | `#15A34A` | Valid / positive state |
| Warning | `#E8930C` | Caution state |
| Danger | `#DC2626` | Destructive actions, errors |
| Focus ring | `#6366F1` | 2px keyboard-focus outline |

### Dark theme

| Role | Hex |
|------|-----|
| Canvas / background | `#0E131A` |
| Surface | `#19212C` |
| Surface raised | `#222C39` |
| Foreground | `#E7ECF3` |
| Foreground muted | `#9AA7B6` |
| Border / divider | `#2C3744` |
| Accent / primary | `#818CF8` |
| Accent hover | `#A5B4FC` |
| Secondary | `#38BDF8` |
| Success | `#4ADE80` |
| Warning | `#FBBF24` |
| Danger | `#F87171` |

### Accent-palette variants

The accent selector swaps only the accent triad, keeping neutrals constant:

- **Indigo** (default): primary `#4F46E5`, hover `#4338CA`, soft `#EEF0FE`.
- **Teal**: primary `#14B8A6`, hover `#0F9488`, soft `#E6FAF6`.
- **Rose**: primary `#F43F5E`, hover `#E11D48`, soft `#FEECEF`.

### Categorical series palette (charts & multi-value controls)

An ordered, colorblind-aware sequence reused by every chart, the color picker
swatches, and any multi-series control:

1. Indigo `#4F46E5`
2. Teal `#14B8A6`
3. Amber `#F59E0B`
4. Rose `#F43F5E`
5. Sky `#0EA5E9`
6. Violet `#8B5CF6`

### Pointer-state visual mapping

Every interactive control must express these pointer states using palette roles,
consistently across all pages:

- **Normal**: surface fill, border divider.
- **Hover**: surface-raised fill, accent border.
- **Pressed**: accent fill (hover shade), foreground-on-accent text.
- **Focused**: 2px focus-ring outline, retained through hover and press.
- **Selected**: accent-soft fill, accent-colored text or marker.
- **Disabled**: muted foreground at reduced emphasis, no hover response.

All color choices must satisfy the theme's `ContrastRequiredRatio` (WCAG AA) for
text and essential UI against their background.

## Pointer & Mouse Interaction Contract

Every page consumes the framework pointer pipeline (`src/Controls/Pointer.fsi`),
which raises `PointerInteraction` values the gallery folds into messages:

- `HoverEnter` / `HoverLeave` → drive the hover visual state and update the status
  strip's "hover" field.
- `PressedDown` / `ReleasedUp` → drive the pressed visual state.
- `Click` → primary activation for buttons, list rows, nav entries, tabs, swatches.
- `DragBegin` / `DragMove` / `DragEnd` → drive sliders, the split-view divider,
  scroll thumbs, and the pointer-playground drag surface; honor the **4.0 px drag
  threshold** (`PointerState.DragThreshold`) so a click is never misread as a drag.
- `DragCancelled` → restore pre-drag state cleanly.
- `Scroll` (wheel) → scroll the content region, list views, and the data grid.
- `PressedDown` with `PointerButton.Secondary` → open a `ContextMenu` at the
  pointer location.
- `PointerButton.Middle` → reserved; demonstrated only on the pointer playground.
- `FocusMovedByPointer` → reconcile pointer focus with keyboard focus.

Each `PointerInteraction` the gallery handles must be reflected in the status strip
as `kind · controlId · (x, y)` so pointer behavior is observable without a
debugger. Pointer coordinates honor the active `PixelSnapPolicy` and `LayoutResult`.

## Keyboard Contract

Pointer and keyboard must stay reconciled on every page:

- `Tab` / `Shift+Tab`: move focus in catalog focus order.
- `Enter` / `Space`: activate the focused control.
- `Arrow keys`: move within the focused composite (list, radio group, tabs, grid).
- `Ctrl+1` … `Ctrl+0`: jump directly to pages 1–10.
- `Ctrl+L`: toggle Light/Dark.
- `Esc`: dismiss the active overlay (dialog, context menu, tooltip).

## Data Model

The shell model tracks at minimum:

- `ActivePage`: the selected content page.
- `ThemeMode`: Light or Dark; `Accent`: Indigo, Teal, or Rose.
- `Pointer`: the framework `PointerState`.
- `FocusedControl`: the currently focused control id.
- `LastInteraction`: the most recent `PointerInteraction` for the status strip.
- `LastMessage`: a short description of the last dispatched message.
- `Seed`: the deterministic seed for any generated sample data.
- Per-page sub-models, owned by their page specs.

## Visual Requirements

- The app bar, nav rail, content region, and status strip are always visible and
  keep stable dimensions while content scrolls.
- The active nav entry is unmistakably distinct from hovered and idle entries.
- Theme and accent changes recolor every visible control immediately and
  consistently, with no element left on a stale palette.
- Focus, hover, pressed, and selected states are visually distinguishable from one
  another and from the normal state on every interactive control.
- Text and essential UI meet the contrast ratio in both themes.

## App State

Track at minimum: active page; theme mode and accent; pointer state; focused
control id; last pointer interaction; last message; random seed; and each page's
own sub-model as defined in its spec.

## Determinism and Evidence

- Accept an optional seed governing all generated sample data across pages.
- Evidence mode injects a deterministic script that visits **every** page in
  order, performs at least one pointer interaction per page (hover, then click or
  drag), toggles the theme once, and switches the accent once.
- The evidence outcome must include: frame count, the ordered list of pages
  visited, the count of pointer interactions by kind, the final theme and accent,
  the final focused control, and the close reason.
- Screenshot evidence must include one capture per page plus one capture of the
  Dark theme, each showing the shell with the active page populated.
- The captured palette in evidence must match the hex values in this spec.

## Acceptance Criteria

- Every one of the 52 catalog controls is reachable and rendered live on exactly
  one page.
- Clicking a nav entry navigates to that page and updates the selection and status
  strip; the shell regions never resize.
- Hover, press, click, drag, scroll, and secondary-click are each observable on at
  least one control and narrated in the status strip.
- The 4.0 px drag threshold prevents a click from registering as a drag.
- Toggling the theme and switching the accent recolor all visible controls
  immediately and consistently.
- Keyboard focus order, activation, and the page/theme shortcuts all work and stay
  reconciled with pointer focus.
- The palette in both themes meets the required contrast ratio.
- Interactive mode remains open until explicitly closed; evidence mode closes
  itself deterministically after the scripted tour.

## Out of Scope

- Persisting navigation, theme, or page state to disk.
- Authoring new controls outside the existing catalog.
- Animation beyond the spinner and progress indicators and simple state
  transitions.
- Localization, networking, or external assets and audio.
