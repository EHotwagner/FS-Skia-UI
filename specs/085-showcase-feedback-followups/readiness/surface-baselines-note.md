# Surface-area baselines (T008)

Pre-implementation snapshot of which public surfaces move so post-implementation
drift is reviewable. The authoritative regeneration runs in **T034**
(`./fake.sh build -t RefreshSurfaceBaselines`) + per-package `.fsi.txt` via
`PerPackageSurface.captureCurrent`; this note records the intended additive delta.

## FS.Skia.UI.Controls (`readiness/surface-baselines/FS.Skia.UI.Controls.txt`)

- **+ `Control.renderTree: theme -> size -> control -> ControlRenderResult<'msg>`**
  (additive `val` in `module Control`). `Control.render` and every other existing
  `val` unchanged (FR-003).

## FS.Skia.UI.SkiaViewer (`readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`)

- **+ `type ViewerPointerButtonKind`** (RequireQualifiedAccess: Primary/Secondary/Middle)
- **+ `type ViewerPointerPhaseKind`** (RequireQualifiedAccess: Moved/Pressed/Released/Wheel/Exited)
- **+ `type ViewerPointerInput`** (`{ Phase; X; Y; Button; DeltaX; DeltaY }`)
- **+ `type InteractiveViewerHost<'model,'msg>`** (pointer/size-aware generic runner host)
- **+ `Viewer.runInteractiveViewer`** + **`Viewer.runInteractiveViewerWithWindowBehavior`**
- `GeneratedAppHost`, `Viewer.runApp`, `runAppWithWindowBehavior` unchanged (FR-006).

## FS.Skia.UI.Controls.Elmish (`readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`)

- **+ `type InteractiveAppHost<'model,'msg>`** (Control/PointerInteraction-aware host)
- **+ `ControlsElmish.runInteractiveApp: options -> host -> Result<ViewerLaunchOutcome, ViewerRunFailure>`**
- New ProjectReference `Controls.Elmish -> SkiaViewer` (research D3-AMEND);
  `DependencyReport` moves accordingly.

## FS.Skia.UI.KeyboardInput

- No `.fsi`/surface change — `ViewerKey` union unchanged; `normalize` is a behavior-only
  fix (US3, FR-007/FR-008).
