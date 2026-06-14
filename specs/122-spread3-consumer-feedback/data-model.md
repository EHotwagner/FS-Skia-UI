# Data Model: Spread3 Consumer Feedback Remediation (Feature 122)

No persisted/MVU data entities change. The "model" here is the present-path decision state and the
additive public launch surface.

## 1. PresentAction (new, internal-with-test-seam) — `src/SkiaViewer/Host/OpenGl.fs(i)`

```fsharp
[<RequireQualifiedAccess>]
/// Feature 122 (FR-001/002): what the DirectToSwapchain host does this frame. Replaces the
/// binary "skip-or-present" so multi-buffer swapchains never rotate an undrawn (black) buffer.
type PresentAction =
    | PaintAndPresent     // scene changed / first frame / size changed: full paint + SwapBuffers
    | RepresentLastGood   // unchanged scene, buffers not yet all filled: blit cached frame + SwapBuffers (no scene walk)
    | SkipPresent         // unchanged scene, buffers known-filled: full idle (byte-identical to 120/121)
```

### `planPresent` (pure)
```fsharp
/// Pure present decision. PaintAndPresent iff `shouldPresent`; else RepresentLastGood while
/// `idleRepresentsRemaining > 0`; else SkipPresent. Exposed for the idle-transition test (mirrors
/// `shouldPresent`/`shouldAdvanceFrame`).
val planPresent:
    prev: Scene option -> next: Scene -> sizeChanged: bool -> idleRepresentsRemaining: int -> PresentAction
```
Truth table (with `shouldPresent` reused unchanged):

| `shouldPresent` | `idleRepresentsRemaining` | result |
|---|---|---|
| true | (any) | `PaintAndPresent` |
| false | `> 0` | `RepresentLastGood` |
| false | `<= 0` | `SkipPresent` |

### Host loop state (internal, in `GlHost.run`)
| field | type | transition |
|---|---|---|
| `lastGoodFrame` | `SKImage option` | set to `surface.Snapshot()` after each `PaintAndPresent` (dispose previous) |
| `idleRepresentsRemaining` | `int` | `PaintAndPresent` → `bufferFillDepth - 1`; `RepresentLastGood` → `n-1`; `SkipPresent` → unchanged |
| `representedCount` | `int` | incremented on `RepresentLastGood` (observability; complements `skippedPresentCount`) |
| `bufferFillDepth` | `int` (const) | `3` — covers typical triple-buffering; not public (FR-004 deferred) |

**Invariant (FR-001):** after any `PaintAndPresent`, the next `bufferFillDepth-1` unchanged frames
`RepresentLastGood` (each buffer gets the frame), so no buffer in the rotation is ever undrawn.
**Invariant (FR-002):** `RepresentLastGood` does a single cached-image blit (no measure/paint/scene
walk); steady-state unchanged frames are `SkipPresent` (full idle). The `OffscreenReadback` path is
untouched → screenshot goldens byte-identical.

## 2. Additive controls launch surface (FR-005) — `src/Controls.Elmish/ControlsElmish.fsi`

```fsharp
/// Feature 122 (FR-003/005): as `runInteractiveApp` with an explicit window behavior threaded
/// into the live launch (startup-state / resize / maximize / position / backend), so a generated
/// app's `--window-startup normal` actually applies to the controls window instead of only the
/// options report. Delegates to `Viewer.runInteractiveViewerWithWindowBehavior`. `runInteractiveApp`
/// is unchanged (default windowed-fullscreen); existing consumers need no change.
val runInteractiveAppWithWindowBehavior:
    options: ViewerOptions ->
    behavior: ViewerWindowBehaviorRequest ->
    host: InteractiveAppHost<'model, 'msg> ->
        Result<ViewerLaunchOutcome, ViewerRunFailure>
```
Implementation: identical to `runInteractiveApp` (ControlsElmish.fs:825) but the terminal call
becomes `Viewer.runInteractiveViewerWithWindowBehavior options behavior viewerHost`
(vs `Viewer.runInteractiveViewer options viewerHost` at line 1245). No other field changes.

**Reused existing surface (already shipped — FR-003 satisfied through it):**
`ViewerWindowBehaviorRequest { ResizePolicy; MaximizePolicy; StartupState; StartupPosition;
BackendPreference }`, `ViewerWindowStartupState.{Normal|Maximized|Minimized|Fullscreen|WindowedFullscreen}`,
`Viewer.defaultWindowBehavior`, `Viewer.runInteractiveViewerWithWindowBehavior`.

## 3. Generated `Program.fs` launch wiring (FR-005) — `template/base/src/Product/Program.fs`

App-profile branch (Program.fs:156) changes from:
```fsharp
let launchResult = ControlsElmish.runInteractiveApp viewerOptions interactiveHost
```
to the windowed-flag-aware form mirroring the game branch (Program.fs:161-165):
```fsharp
let launchResult =
    if Product.WindowOptions.windowFlagSupplied args then
        ControlsElmish.runInteractiveAppWithWindowBehavior viewerOptions windowBehaviorRequest interactiveHost
    else
        ControlsElmish.runInteractiveApp viewerOptions interactiveHost
```
`windowBehaviorRequest` is already computed at Program.fs:87. No-flag default → unchanged.

## 4. CustomControl guard (FR-006) — `src/Controls/CustomControl.fs`

No type change. `validate`/`create` replace `s.Trim() = ""` with `String.IsNullOrWhiteSpace s` and
null-guard the `Accessibility.defaultFor … definition.Id` argument and each `Effects` string, so a
null `Id`/effect yields a validation diagnostic instead of an NRE. `CustomControlDefinition` `.fsi`
unchanged.

## 5. Catalog/doc string deltas (FR-007/008/009/010/011)

Pure text — no type/contract change beyond the regenerated `docs/controls-catalog.md`. See
[contracts/public-surface-delta.md](./contracts/public-surface-delta.md).
