# Phase 1 Data Model — 085

This feature is mostly **additive public surface** over existing types. No new
persisted entities; the "entities" are the new/changed public contract shapes and
the input/effect data that flows through the MVU boundary.

## Existing types reused (unchanged)

| Type | Source | Role here |
|------|--------|-----------|
| `Control<'msg>` | `src/Controls/Types` | Input to `renderTree`. |
| `Theme` | `src/Controls` | Active theme for layout/paint. |
| `Size = { Width: int; Height: int }` | `src/Scene/Scene.fsi` | Output extent for `renderTree` / size-aware `View`. |
| `SceneNode` / `Scene` | `src/Scene/Scene.fsi` | Painted output. |
| `ControlRenderResult<'msg> = { Scene; Layout: LayoutNode; Diagnostics; EventBindings: ControlEventBinding<'msg> list; NodeCount }` | `src/Controls/Types.fsi` | Return of `renderTree`; `Layout` + `EventBindings` drive hit-testing. |
| `ControlEventBinding<'msg> = { ControlId; EventKind: string; Dispatch: ControlEvent -> 'msg }` | `src/Controls/Types.fsi` | Correlated with `LayoutNode` bounds by `ControlId` for hit-testing. **Unchanged** — no bounds field added. |
| `PointerInteraction` | `src/Controls.Elmish` | Data form of a pointer event after hit-test + 4px fold. |
| `ControlsElmish.interpretPointerOutcome` / `interpretPointerEffect` | `src/Controls.Elmish` | Already-public routing the host reuses. |
| `ViewerKey` (union) | `src/KeyboardInput/KeyboardInput.fsi` | **Unchanged**; `normalize` behavior only. |
| `ViewerEffect`, `ViewerOptions`, `ViewerLaunchOutcome`, `ViewerRunFailure`, `ViewerDiagnosticsOptions` | `src/SkiaViewer/SkiaViewer.fsi` | Reused by the new host variant. |
| `GeneratedAppHost<'model,'msg>` + `Viewer.runApp` | `src/SkiaViewer/SkiaViewer.fsi` | **Untouched** — preserves the durable `GovernanceTests` literal (FR-006). |

## New public surface (additive)

### `Control.renderTree` (FR-001, FR-002, FR-003)
```
val renderTree:
    theme: Theme -> size: Size -> control: Control<'msg> -> ControlRenderResult<'msg>
```
- Real recursive Yoga layout at `size`; paints nested containers **and** children.
- Invariant: two structurally different trees ⇒ `Scene` differs (SC-001).
- Invariant: `Control.render` / `Widget.render` behavior + goldens unchanged (FR-003).

### `InteractiveAppHost<'model,'msg>` (FR-004, FR-006, FR-009)
```
type InteractiveAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> SceneNode          // size-aware (FR-009)
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option   // pointer seam (FR-004)
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }
```
- Mirrors `GeneratedAppHost` field-for-field **plus** `MapPointer` and a
  size-carrying `View`. Distinct record ⇒ no construction-site break.

### `Viewer.runInteractiveApp` (FR-004, FR-005)
```
val runInteractiveApp:
    options: ViewerOptions ->
    host: InteractiveAppHost<'model,'msg> ->
        Result<ViewerLaunchOutcome, ViewerRunFailure>
```

## MVU / effect boundary (Constitution IV)

| Element | Where |
|---------|-------|
| `Model` | consumer-owned (`'model`). |
| `Msg` | consumer-owned (`'msg`); produced by `MapKey` / `MapPointer` / `Tick`. |
| Pointer event → data | `ViewerEvent.Pointer*` → hit-test (`Layout`×`EventBindings` by `ControlId`) → `PointerInteraction` (with 4px fold) — **pure data**. |
| `update` | `host.Update` — **pure** (`'msg -> 'model -> 'model * ViewerEffect list`). |
| Effect interpreter (edge) | `runInteractiveApp` loop executes `ViewerEffect` + drives the render/dispatch cycle. |

## Behavior change (no new type)

- `KeyboardInput.normalize` (FR-007, FR-008): new recognized name families map to existing
  `Digit n` / `Letter X`; totality preserved (`Unknown raw` terminal arm).
