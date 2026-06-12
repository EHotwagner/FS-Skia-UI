# Phase 1 Data Model — feature 108

Closed type surface, placement, and the identity/parity invariants. All types are
plain F# records/DUs (Principle III). Visibility lives in `.fsi` (Principle II).

## New / changed types by package

### `FS.Skia.UI.Controls` — `Focus` (`src/Controls/Focus.fsi`)

No new type. One new function:

```fsharp
/// Stamp VisualState.Focused on the control whose identity (Key ?? structural path)
/// equals `focused`, leaving every other control untouched. `None` returns the tree
/// byte-identical. Preserves a consumer-set non-Normal state (e.g. Disabled wins).
val markFocused: focused: ControlId option -> control: Control<'msg> -> Control<'msg>
```

- **Identity**: `Key ?? structural path`, the feature-098 unification. The same id
  `Focus.order`/`traverse` enumerate, so reachable-by-traversal == paintable-by-ring.
- **Invariant (FR-003)**: at most one control carries `Focused` after a stamp.
- **Invariant (SC-012)**: `markFocused None tree ≡ tree` (structural-Scene identity);
  a stamp on a control already `Normal` adds only the `Focused` state.

### `FS.Skia.UI.Controls` — `Control` / `Widget` (`Control.fsi`, `Widget.fsi`)

```fsharp
module Control =
    /// Change only the message type. Structure, Key, Accessibility, and Children
    /// identity are preserved; every Attr's msg-bearing AttrValue handler is mapped
    /// through `f`. Lowers structurally equal to authoring directly in 'b.
    val map: f: ('a -> 'b) -> control: Control<'a> -> Control<'b>

module Widget =
    val map: f: ('a -> 'b) -> widget: Widget<'a> -> Widget<'b>
```

- **Structure preservation**: `Kind`, `Key`, `Content`, `Accessibility`, and the
  `Children` shape are unchanged; only `Attr<'a>.Value : AttrValue<'a>` handlers are
  rewritten to `'b`. `Widget.map = ofControl ∘ Control.map f ∘ toControl`.
- **Invariant (FR-014/SC-007)**: `Control.map f c` lowers structurally equal to a
  control authored directly in `'b` (proven by `%A` projection equality).
- **Identity (edge case)**: keys/focus identity survive the map (only the msg type
  changes).

### `FS.Skia.UI.Controls` — `DataGrid` (`DataGrid.fsi`)

No new type required. Behaviour change on the existing surface:

```fsharp
// DataGridSort stays { ColumnKey: string; Direction: DataGridSortDirection }
// The state carried is `DataGridSort option`; `None` = unsorted.
// SortBy column transition becomes a three-state cycle on the SAME column:
//   None              -> Some { col; Ascending }
//   Some Ascending    -> Some { col; Descending }
//   Some Descending   -> None            // <- new: third toggle clears
//   (SortBy a DIFFERENT column always starts at Ascending)
```

- **Invariant (FR-015/SC-008)**: three `SortBy` presses on one column return to
  `None` with no product-side special-casing. `DataGridSortChanged` effect now also
  fires `DataGridSortChanged None` on the clearing transition.

### `FS.Skia.UI.Controls` — `Theming` (`Theming.fsi`, new — placement decision D8)

```fsharp
module Theming =
    /// theme mode + accent -> a role palette (the live-theming primitive).
    val resolve: mode: Palettes.RampVariant -> accent: Color -> RolePalette
    /// Project a role palette onto the framework Theme (paint theme for the render path).
    val toTheme: palette: RolePalette -> Theme
// WCAG contrast is reused from FS.Skia.UI.Color: `Contrast.ratio : Color -> Color -> float`
// (re-documented as the contrast helper; NOT re-implemented here).
```

- `RolePalette` is a small record of the role colours `toTheme` consumes (background,
  foreground, accent, danger, muted, focus-ring). Exact field set finalised in
  contracts/Theming.fsi.
- **Invariant (FR-018)**: `toTheme` yields the exact model-derived palette for the
  render path while the consumer keeps a static `host.Theme` for the fragment-reuse
  key (documented split — never reuses a stale fragment when only the palette changed).

### `FS.Skia.UI.KeyboardInput` (`KeyboardInput.fsi`)

```fsharp
type KeyModifiers =
    { Ctrl: bool; Alt: bool; Shift: bool; Meta: bool }

module ViewerKeyboard =
    val noModifiers: KeyModifiers
    /// Parse Ctrl+/Alt+/Shift+/Meta+ prefixes (case-insensitive, any order) off the
    /// raw key, returning the base ViewerKey, the down/up flag, and the modifiers.
    val normalizeEventWithModifiers:
        event: ViewerKeyEvent -> ViewerKey * bool * KeyModifiers
    // existing `normalize` / `normalizeEvent` unchanged for back-compat.
```

- **Invariant (FR-016/SC-009)**: an unmodified key parses to `noModifiers` and the
  same `ViewerKey` as today (byte-identical routing); a chord recovers all held
  modifiers — zero silent loss.

### `FS.Skia.UI.Controls.Elmish` (`ControlsElmish.fsi`)

```fsharp
type FrameMetrics =
    { RemeasuredNodeCount: int        // from WorkReductionRecord
      PointerSamplesReceived: int     // raw samples that arrived this frame
      PointerMovesProcessed: int      // <= 1 after coalescing
      ViewRebuilt: bool               // host.View called this frame?
      FrameDuration: System.TimeSpan } // reported, EXCLUDED from golden/determinism

type FrameInput<'msg> =
    | Key of ViewerKey * KeyModifiers
    | Pointer of PointerInteraction
    | Tick of System.TimeSpan
    | Idle

// additive host fields (inert defaults preserve at-rest behaviour):
//   MapKeyChord: ViewerKey -> KeyModifiers -> 'msg option   (consulted before MapKey)
//   OnFrameMetrics: FrameMetrics -> unit                    (opt-in observability sink)
type InteractiveAppHost<'model,'msg> =
    { // …existing fields…
      MapKeyChord: ViewerKey -> KeyModifiers -> 'msg option
      OnFrameMetrics: FrameMetrics -> unit }

module Perf =
    /// Pure, headless: fold an ordered input script over the host's pure update +
    /// RetainedRender.step, one frame per step, accumulating byte-stable FrameMetrics
    /// (counts) per frame. Shares the coalescing/step code path with runInteractiveApp.
    val runScript:
        host: InteractiveAppHost<'model,'msg> ->
        size: FS.Skia.UI.Scene.Size ->
        script: FrameInput<'msg> list ->
            FrameMetrics list
```

- **Invariant (FR-007/SC-003)**: the four count/bool fields of `runScript` output are
  identical across repeated runs of the same script; `FrameDuration` is not asserted.
- **Invariant (FR-008/SC-005)**: an `Idle` frame → `RemeasuredNodeCount = 0`,
  `ViewRebuilt = false`; a pure-`Pointer` hover frame → `ViewRebuilt = false`.
- **Invariant (FR-011/SC-004)**: K `Pointer` moves in one frame →
  `PointerMovesProcessed ≤ 1`, `PointerSamplesReceived = K`.

### `FS.Skia.UI.SkillSupport` — `EvidenceTour` (`EvidenceTour.fsi`, new)

```fsharp
module EvidenceTour =
    /// Generic, dependency-free: fold an ordered Msg script over a pure update,
    /// one step per Msg, accumulating a structured outcome. (No framework metrics.)
    val run:
        script: 'msg list ->
        seed: 'model ->
        update: ('msg -> 'model -> 'model) ->
        project: ('model -> 'acc -> 'acc) ->
        initial: 'acc ->
            'acc
```

- Sits beside the shipped `SkillSupport.Random` (splitmix64) so a consumer's tour is
  byte-stable across runtimes.

## Placement summary

| Type / fn | Package | File | New? |
|---|---|---|---|
| `Focus.markFocused` | Controls | Focus.fsi | new fn |
| `Control.map` / `Widget.map` | Controls | Control.fsi / Widget.fsi | new fn |
| DataGrid tri-state cycle | Controls | DataGrid.fs(.fsi unchanged) | behaviour |
| `Theming.resolve` / `toTheme`, `RolePalette` | Controls | Theming.fsi (new) | new |
| (contrast) `Contrast.ratio` | Color | Contrast.fsi | reuse |
| `KeyModifiers`, `normalizeEventWithModifiers` | KeyboardInput | KeyboardInput.fsi | new |
| `FrameMetrics`, `FrameInput`, `Perf.runScript` | Controls.Elmish | ControlsElmish.fsi | new |
| `MapKeyChord`, `OnFrameMetrics` host fields | Controls.Elmish | ControlsElmish.fsi | new fields |
| `EvidenceTour.run` | SkillSupport | EvidenceTour.fsi (new) | new |

## Cross-cutting invariants

- **At-rest byte-identity (SC-012)**: with no focus, `VisualState.Normal`, and no
  pending input, every render path is structurally identical to the pre-108 output.
  `markFocused None`, `MapKeyChord`/`OnFrameMetrics` defaults, and the coalescing
  accumulator at zero samples are all no-ops.
- **Construction-site sweep**: adding `MapKeyChord`/`OnFrameMetrics` to
  `InteractiveAppHost` requires updating every framework record-construction site
  (samples `ControlsGallery`/`DemoReel`, FSI preludes `scripts/*-prelude.fsx`,
  generated host) in the same change — caught by `RefreshSurfaceBaselines` build and
  `FsiTranscripts` (memory `feature-100`).
- **No equality on `Control<'msg>`**: compare via `sprintf "%A"` projections in tests
  (memory `feature-096`/`101`).
- **`val internal` discipline**: cross-assembly-internal helpers (e.g. the coalescing
  accumulator, the shared step) stay omitted from `.fsi` or declared `val internal`
  where a test/host in another assembly needs them (memory `feature-096`).
