---
title: Controls Gallery — Display & Typography Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Display & Typography Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every read-only display and typography control rendering faithful
geometry under the gallery palette, with pointer hover surfacing tooltips and
selection where applicable.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| text-block | TextBlock | `text` | — | Static multi-line body text |
| rich-text | RichText | `runs` | — | Styled runs: weight, color, size, clipping |
| label | Label | `text` | — | Short field labels |
| image | Image | `value` | — | Image placeholder / drawing-surface reference |
| icon | Icon | `text` | — | Named glyph at several sizes |
| separator | Separator | — | — | Horizontal and vertical dividers |
| badge | Badge | `text` | — | Compact status counts in accent/success/danger |

## User Experience

The page presents a vertical, scrollable sequence of titled sections — one per
control — each pairing a short label with a live instance. Nothing here is
editable, but hovering any element raises its tooltip and updates the status strip,
proving the pointer pipeline reaches even non-interactive controls.

## Layout

- A page heading `Display & Typography` over a one-line description.
- A `Stack` of bordered sections, one per control, each a `Border` wrapping a
  `Label` caption and the control instance.
- A `Separator` between sections demonstrating the divider control itself.
- The rich-text section shows at least three runs differing in weight, color
  (accent, foreground, muted), and size within one line, plus a clipped run.
- The badge section shows badges in accent, success, warning, and danger roles.

## Mouse & Pointer Interactions

- `HoverEnter` on any section raises that control's `Tooltip` and writes
  `hover · <id> · (x,y)` to the status strip.
- `HoverLeave` dismisses the tooltip.
- Clicking a badge selects it (accent-soft fill) and echoes the badge text to the
  status strip; a second click deselects.
- Wheel `Scroll` moves the page within the content `ScrollViewer`.

## Keyboard

- `Tab` moves focus across the badges (the only focusable items here).
- `Enter` / `Space` toggles the focused badge's selection.

## Core Behaviors

- Text, label, icon, and image content is model-owned and never editable here.
- Rich-text runs render with their per-run weight, color, and size; an overlong
  run clips rather than overflowing its box.
- Separators keep a fixed thickness and never collapse.
- A badge reflects its role color and may be selected/deselected by pointer.

## Data Model

- A list of display sections, each with a caption and a control descriptor.
- For rich-text: an ordered list of runs (text, weight, color role, size).
- For badges: text, role color, and a selected flag.

## Visual / Palette Requirements

- Body text uses foreground; captions use foreground-muted.
- Rich-text run colors map to accent, foreground, and muted roles.
- Badge roles map to accent, success, warning, and danger; each badge's text meets
  contrast against its fill.
- The image placeholder shows a labeled surface (not a blank box) at a fixed
  aspect ratio.

## App State

Track: section list; rich-text run definitions; per-badge selected flags; hovered
control id; scroll offset.

## Determinism and Evidence

- Sample text and run content are fixed (seed-independent).
- Evidence mode hovers each section in order and toggles one badge selection.
- Evidence outcome: section count, runs rendered, badges toggled, hovered ids, and
  close reason.
- Screenshot evidence shows the rich-text multi-run line, the icon row, and the
  badge row with one badge selected.

## Acceptance Criteria

- All seven controls render with faithful geometry and palette roles.
- Hover raises and dismisses tooltips and narrates to the status strip.
- Rich-text runs show distinct weight, color, and size; the clipped run clips.
- Badges select and deselect by pointer and keyboard.
- Separators stay fixed; the image placeholder shows a labeled surface.
- Theme/accent switches recolor every element on the page.

## Out of Scope

- Editing any text or image content.
- Loading real image files from disk or network.
- Animated or marquee text.
