---
title: Controls Gallery — Layout & Containers Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Layout & Containers Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every layout and container control composing child content, including
a pointer-draggable split divider and pointer-driven scroll, so the structural
behavior of each container is directly observable.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| stack | Stack | `children` | — | Vertical/horizontal ordered composition |
| grid | Grid | `children` | — | Row/column structured composition |
| dock | Dock | `children` | — | Edge-docked regions + fill |
| wrap | Wrap | `children` | — | Reflowing wrap layout |
| border | Border | `child` | — | Single child with border + padding |
| panel | Panel | `children` | — | General child surface |
| scroll-viewer | Collections | `child` | `onChanged` | Scrollable viewport |
| split-view | Collections | `children` | `onChanged` | Resizable two-region layout |

## User Experience

The page is a tour of layout primitives, each rendered with labeled, tinted child
boxes so the container's arrangement is unmistakable. The user drags the split
view's divider to repartition two regions and wheel-scrolls an overflowing
scroll-viewer. The wrap container visibly reflows as the window width changes.

## Layout

- A heading `Layout & Containers` and description.
- A labeled demo card per container, each holding 3–6 numbered child tiles in
  distinct categorical palette tints:
  - `Stack` shown both vertical and horizontal with visible spacing.
  - `Grid` shown as a 3×3 arrangement of tiles.
  - `Dock` shown with top/left/right/bottom regions around a fill region.
  - `Wrap` shown with enough tiles to reflow across rows.
  - `Border` shown wrapping a single tile with visible border and padding.
  - `Panel` shown as a plain surface holding tiles.
  - `ScrollViewer` shown with content taller than its viewport.
  - `SplitView` shown with two labeled regions and a draggable divider.

## Mouse & Pointer Interactions

- `DragBegin`/`DragMove`/`DragEnd` on the split-view divider repartition the two
  regions continuously, honoring the 4.0 px threshold; `onChanged` reports the new
  ratio.
- Wheel `Scroll` over the scroll-viewer moves its content and fires `onChanged`
  with the new offset; the scroll thumb is also draggable.
- `HoverEnter` on a child tile highlights it and narrates its container and index
  to the status strip.
- Resizing the window reflows the `Wrap` and re-measures every container live.

## Keyboard

- `Tab` moves focus across focusable demo regions.
- `Arrow` keys nudge the split divider and scroll the viewport by a step when those
  regions are focused.

## Core Behaviors

- Stack preserves child order and applies its orientation and spacing.
- Grid places children into rows and columns with aligned cells.
- Dock pins edge regions and gives remaining space to the fill region.
- Wrap flows children left-to-right and wraps to a new row when width is exceeded.
- Border applies a uniform border and padding around exactly one child.
- ScrollViewer clips overflow and exposes a scroll offset via `onChanged`.
- SplitView keeps both regions visible, clamps the divider to a min size per side,
  and reports the split ratio via `onChanged`.

## Data Model

- Per-container child tile lists (id, index, tint).
- Split-view ratio and divider drag state.
- Scroll-viewer content size and current offset.
- Window size used for wrap reflow.

## Visual / Palette Requirements

- Child tiles use the categorical series palette tints with foreground-legible
  labels.
- Container chrome (borders, dividers, padding) uses border-divider; the split
  divider highlights to accent on hover/drag.
- Spacing, padding, and docked widths are visibly non-zero and stable.

## App State

Track: per-container child lists; split ratio and drag flag; scroll offset; window
size; hovered tile id.

## Determinism and Evidence

- Child tile contents are fixed (seed-independent).
- Evidence mode drags the split divider to a set ratio, scrolls the scroll-viewer
  by a page, and reflows the wrap at two window widths.
- Evidence outcome: final split ratio, final scroll offset, wrap row counts at each
  width, drag/scroll counts, and close reason.
- Screenshot evidence shows the dock layout, the wrap reflowed across rows, and the
  split view at a non-default ratio.

## Acceptance Criteria

- Each container arranges its children per its documented rule.
- Dragging the split divider repartitions both regions and fires `onChanged`,
  clamped to per-side minimums.
- Scrolling the viewport moves content and fires `onChanged`; the thumb is
  draggable.
- The wrap reflows when the window width changes.
- Border padding and stack spacing are visibly applied and stable.

## Out of Scope

- Nested split views or more than two split regions.
- Drag-and-drop of tiles between containers.
- Absolute / canvas positioning beyond the listed containers.
