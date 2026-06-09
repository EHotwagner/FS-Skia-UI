---
title: Controls Gallery — Pointer Playground & Custom Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Pointer Playground & Custom Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Give the full mouse/pointer event surface a dedicated home and demonstrate the
`custom-control` wrapper hosting bespoke Skia content driven directly by pointer
samples — every `PointerInteraction` case made visible and measurable.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| custom-control | CustomControl | — | `onCustom` | Product-owned Skia surface fed raw pointer input |

This page is also the reference exercise for the entire pointer pipeline
(`src/Controls/Pointer.fsi`): `HoverEnter`/`HoverLeave`, `PressedDown`/`ReleasedUp`,
`Click`, `DragBegin`/`DragMove`/`DragEnd`, `DragCancelled`, `Scroll`,
`FocusMovedByPointer`, and the three `PointerButton`s.

## User Experience

A large interactive canvas (a `CustomControl`) fills the page. The user moves the
pointer to leave a live crosshair and trail, presses and drags to draw or move
draggable tokens, wheel-scrolls to zoom, secondary-clicks to open a context menu,
and middle-clicks to drop a marker. An event inspector lists the most recent raw
pointer interactions with their coordinates, button, and deltas, proving the full
event surface end-to-end.

## Layout

- A heading `Pointer Playground & Custom` and description.
- A large `CustomControl` canvas (fills most of the page) hosting:
  - A live crosshair at the pointer position and a fading move-trail.
  - Several draggable circular tokens in categorical palette colors.
  - A zoom/pan transform driven by the wheel.
- A right-hand "Event inspector" `ListView` of the last ~16 `PointerInteraction`s,
  each shown as `kind · button · (x, y) · (dx, dy)`.
- A small readout of live pointer state: hover target, pressed buttons, drag
  active, zoom level.

## Mouse & Pointer Interactions

This page must surface every pointer case explicitly:

- `HoverEnter` / `HoverLeave`: entering and leaving tokens highlights them and logs
  the transition.
- Pointer move: updates the crosshair and trail; the inspector shows `Moved`
  samples with coordinates.
- `PressedDown` (Primary): begins a candidate press on the token (or canvas) under
  the pointer.
- `ReleasedUp` within the 4.0 px threshold without movement: resolves to `Click`
  (drops a dot / selects a token).
- `DragBegin` once movement exceeds the 4.0 px `DragThreshold`: starts dragging the
  token; `DragMove` repositions it live; `DragEnd` commits its new position.
- `DragCancelled`: pressing `Esc` mid-drag (or losing capture) restores the token's
  pre-drag position.
- `Scroll` (wheel): zooms the canvas transform about the pointer; horizontal wheel
  pans.
- `PressedDown` (Secondary): opens a `ContextMenu` at the pointer with canvas
  actions (`Clear trail`, `Reset zoom`, `Remove token`).
- `PressedDown` (Middle): drops a persistent marker at the pointer.
- `FocusMovedByPointer`: clicking the canvas takes keyboard focus so arrow keys nudge
  the selected token.

Every interaction the canvas receives is appended to the event inspector and
mirrored in the shell status strip.

## Keyboard

- `Arrow` keys nudge the selected token by one unit; `Shift+Arrow` by ten.
- `Esc` cancels an in-progress drag or closes the context menu.
- `Delete` removes the selected token; `R` resets zoom and pan.

## Core Behaviors

- The custom control receives `PointerSample`s and emits domain events through
  `onCustom`; the page interprets them into draw/drag/zoom state.
- The drag threshold cleanly separates `Click` from `DragBegin`; a press-and-release
  in place is never a drag.
- A cancelled drag fully restores pre-drag state.
- Zoom/pan is a single affine transform applied to canvas content; pointer
  coordinates are mapped through its inverse for hit-testing.
- The event inspector is bounded (most-recent-first) and never grows without limit.

## Data Model

- Token list: id, position, color, selected flag, and pre-drag position.
- Canvas transform: zoom level and pan offset.
- Move-trail points with fade ages; dropped dots and middle-click markers.
- Pointer snapshot: hover target, pressed button set, drag-active flag.
- Bounded interaction log of raw `PointerInteraction`s.
- Context-menu open flag and anchor.

## Visual / Palette Requirements

- The canvas uses the surface background with a faint border-divider grid.
- Tokens use the categorical palette; the selected token shows an accent ring.
- The crosshair and trail use accent at decreasing opacity; markers use the
  secondary role.
- The event inspector rows alternate surface / surface-raised for legibility.

## App State

Track: tokens and their positions/selection; canvas transform; trail and markers;
pointer snapshot; interaction log; context-menu state; seed for initial token
placement.

## Determinism and Evidence

- Accept a seed for initial token positions and colors.
- Evidence mode injects a deterministic pointer script: hover a token, click it,
  drag it past the threshold and release, attempt a sub-threshold press (verifying
  it is a click, not a drag), wheel-zoom in then reset, secondary-click to open and
  select a context action, and middle-click to drop a marker.
- Evidence outcome: counts of each `PointerInteraction` kind, the final token
  positions, the zoom level, marker count, the click-vs-drag threshold verification,
  and close reason.
- Screenshot evidence shows the crosshair and trail, a token mid-drag with its
  accent ring, the zoomed canvas, and the context menu open at a pointer anchor.

## Acceptance Criteria

- Every `PointerInteraction` kind is exercised, logged in the inspector, and
  narrated in the status strip.
- The 4.0 px threshold reliably distinguishes `Click` from `DragBegin`.
- A cancelled drag restores the token's pre-drag position exactly.
- Wheel zoom/pan transforms the canvas and hit-testing stays correct under the
  transform.
- Secondary-click opens a context menu at the pointer; middle-click drops a marker.
- Clicking the canvas moves keyboard focus so arrow keys nudge the selected token.
- The interaction log stays bounded.

## Out of Scope

- Multi-touch or gesture recognition beyond single-pointer drag and wheel.
- Pressure / tilt / stylus input.
- Saving the canvas content to an image or file.
