# Phase 1 Data Model: Interactive Non-Game Consumer Fitness

**Feature**: `086-interactive-consumer-fitness` | **Date**: 2026-06-09

This feature is mostly contract/behavior change over existing types. The entities
below are the new or modified data shapes; each names the file, the validation rule,
and the requirement it serves.

## 1. `SceneNode.Translate` — offset wrapper (FR-013, D1)

| Field | Type | Meaning |
|-------|------|---------|
| offset | `float * float` | `(dx, dy)` applied to the wrapped sub-scene |
| child | `Scene` | the sub-scene shifted as a whole |

- **File**: `src/Scene/Scene.fsi` / `src/Scene/Scene.fs`; descriptor `TranslateElement`.
- **Rule**: rendering pushes a canvas translation around `child`, so **every**
  descendant kind (`Path`/`Points`/`Vertices`/`Chart`) shifts uniformly. Nesting
  composes (`Translate (a, Translate (b, s))` = sum of offsets).
- **Back-compat**: purely additive case; existing scenes never contain it.

## 2. `SceneNode.SizedText` — chrome text with explicit size (FR-014, D2)

| Field | Type | Meaning |
|-------|------|---------|
| position | `float * float` | baseline origin |
| text | `string` | the string |
| size | `float` | explicit font size (points) |
| color | `Color` | fill |

- **File**: `src/Scene/Scene.fsi` / `Scene.fs`; descriptor `SizedTextElement`;
  constructor `Scene.sizedText`.
- **Rule**: routes through the same glyph layout as `TextRun.Font.Size`. The existing
  `Text` case is **unchanged** — a `Text` with no explicit size keeps default-font
  rendering (FR-014 back-compat / Edge Case).

## 3. `Stack` orientation attribute (FR-007, D3)

| Attribute key | Values | Default |
|---------------|--------|---------|
| `orientation` | `"horizontal"` \| `"vertical"` | `"vertical"` |

- **File**: builder in `src/Controls/Control.fs` (`module Stack`); read by `directionOf`.
- **Rule**: `directionOf` returns `Row` when the kind is a documented horizontal
  container (`toolbar`/`split-view`/`wrap`/`grid`/`dock`) **or** the control carries
  `orientation = horizontal`; otherwise `Column`. No `.fsi` type change (additive
  builder + behavior).

## 4. Internal structural layout id (FR-008/FR-009, D4)

- **File**: internal to `src/Controls/Control.fs` (`renderTree`'s `toLayout`/`paint`).
- **Rule**: each node receives a **collision-free** `LayoutNodeId` derived
  deterministically from its tree path (sibling index, preferring an explicit `Key`),
  threaded identically into layout and paint. Two unkeyed same-kind siblings get
  **distinct** ids → distinct, non-overlapping bounds. No randomness/clock
  (resume-safe / deterministic).
- **Consequence**: an explicit container width/height (already read at
  `Control.fs:1038-1047`) is now reflected because the collision no longer masks it.

## 5. `ControlRenderResult<'msg>` — add computed bounds (FR-011/FR-012, D5)

Current (`src/Controls/Types.fsi:285-290`): `Scene · Layout · Diagnostics · EventBindings · NodeCount`.

**Add** one field:

| Field | Type | Meaning |
|-------|------|---------|
| Bounds | `(ControlId * Rect) list` | evaluated absolute bounds of every rendered control, keyed by `ControlId` |

- **File**: `src/Controls/Types.fsi`; populated in `Control.fs:renderTree` from
  `result.Bounds` (the `LayoutResult` currently computed at `:1067` and discarded).
- **Rule**: every control that has a `ControlId` and was laid out appears exactly once;
  the value is the **evaluated** box (not the input tree). `Layout = root` stays for
  back-compat.
- **Companion**: `Control.hitTest : ControlRenderResult<'msg> -> float -> float -> ControlId option`
  resolves a point to the containing control (or `None` in a gap), layered over
  `Layout.hitTestComputed` (`Layout.fsi:22`). FR-012.

## 6. Scaffold product model — neutral (FR-001/FR-002, SCAFFOLD-1)

Replaces the game `Model` in `template/base/src/Product/Model.fs` (the replaceable
scaffold model per `scaffold-map.md`).

| Game token (removed) | Neutral replacement |
|----------------------|---------------------|
| `Screen = Initial\|Options\|Main\|Paused\|Ended` | `Page` (e.g. `Home\|Settings\|Content\|...`) — app navigation states |
| `ActiveColumn` / `ActiveRow` | content-region cursor/selection fields (neutral names) |
| `Tally` / `Stage` / `NextToken` | generic status fields (e.g. `ItemCount` / `Step` / `NextLabel`) |
| `playfield` / `gameplayRegion` (`View.fs`/`LayoutEvidence.fs`) | `contentRegion` / `contentArea` |
| "circular entities" (`View.fs`) | removed; default view renders controls (FR-003) |

- **Rule (FR-002)**: the **durable governance/evidence/window tokens**
  (`--scene-evidence`, `SceneEvidence.render`, `RendererMode = "deterministic-scene"`,
  the visual-evidence honesty vocabulary, the window-diagnostics classes) MUST stay
  present and pass — only their **framing** becomes "app/content region", never "game".
  Renaming for neutrality MUST NOT drop a governance-scanned token.

## 7. Scaffold default `view` — real control tree (FR-003, VIEW-1)

- **File**: `template/base/src/Product/View.fs`.
- **Rule**: the durable `view : Model -> SceneNode` renders the real
  `controlsExampleView` through `Control.renderTree` (the production tree-render path),
  **not** the hand-built `Group([...])` of rectangles/grid/text. The unmodified default
  app shows actual styled controls (SC-002).

## 8. Product-family marker (FR-004/005/006, D6)

- **Mechanism**: a template generation profile value (`controls` vs `game`) riding the
  existing `//#if (profile == ...)` machinery in the scaffold.
- **Rule**: the **controls** family default launch = `ControlsElmish.runInteractiveApp`
  (pointer host); the **game** family default = `Viewer.runApp ... generatedHost`.
  Governance asserts *presence of the per-family persistent interactive host*, not the
  single literal. Neither family loses its persistent-launch guarantee (SC-003, SC-008).

## 9. Viewer key warm-up queue (FR-015/FR-016, D7)

- **File**: `src/SkiaViewer/SkiaViewer.fs` host input path (`~1480-1511`).
- **Rule**: a **bounded** pre-ready FIFO buffers key events captured before the pipeline
  signals ready and flushes them in order once ready; past the cap it drops-oldest with
  a diagnostic (Principle VII). After ready, dispatch is direct. Documented in
  `fs-skia-viewer-host` (FR-016).
