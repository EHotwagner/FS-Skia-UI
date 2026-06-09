# Contract sketch — new/changed public surface (085)

These are the **`.fsi` deltas** to draft and exercise in FSI (Constitution I) before
any `.fs` body. They are additive; no existing signature is removed or changed.

## `src/Controls/Control.fsi` — add to `module Control`
```fsharp
/// Faithfully rasterize a NESTED control tree to a Scene using real Yoga layout and
/// paint at the given output size (distinct from `render`, the Feature-080 single-
/// control PREVIEW). Lays out and paints nested containers AND their children, so two
/// structurally different trees produce visibly different scenes. Additive: `render`
/// and `Widget.render` are unchanged (FR-003).
val renderTree:
    theme: Theme -> size: Size -> control: Control<'msg> -> ControlRenderResult<'msg>
```
- `Size` is `FS.Skia.UI.Scene.Size` (already in scope via the Scene dependency).
- `ControlRenderResult<'msg>` is the existing type (carries `Scene`, `Layout`,
  `EventBindings`, `Diagnostics`, `NodeCount`).

## `src/SkiaViewer/SkiaViewer.fsi` — add (leave `GeneratedAppHost` / `runApp` intact)
```fsharp
/// Pointer-aware, size-aware host variant. Mirrors GeneratedAppHost field-for-field
/// PLUS a pointer seam (MapPointer) and a size-carrying View. Separate record so the
/// existing GeneratedAppHost construction sites and the durable
/// `Viewer.runApp viewerOptions generatedHost` GovernanceTests literal are unbroken
/// (FR-006).
type InteractiveAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

/// Durable host launch path that routes raised ViewerEvent.Pointer* events by hit-
/// testing the rendered layout (ControlRenderResult.Layout × EventBindings, by
/// ControlId) and dispatching through ControlsElmish.interpretPointerOutcome
/// host.MapPointer (incl. the 4px click/drag fold). Additive to runApp (FR-004/005/006).
val runInteractiveApp:
    options: ViewerOptions ->
    host: InteractiveAppHost<'model,'msg> ->
        Result<ViewerLaunchOutcome, ViewerRunFailure>
```
- `PointerInteraction` is referenced from `FS.Skia.UI.Controls.Elmish`; confirm the
  SkiaViewer project already references Controls.Elmish (or route through an
  intermediate the project depends on) during FSI drafting.

## `src/KeyboardInput/KeyboardInput.fs` — `normalize` (no `.fsi` change)
Behavior contract (the `.fsi` `val normalize: raw: string -> ViewerKey` and the
`ViewerKey` union are **unchanged**):
```
"Number5" | "Digit5" | "Keypad5" | "Key5"  -> Digit 5      (case-insensitive)
"KeyL"                                      -> Letter 'L'   (case-insensitive)
<unrecognized>                             -> Unknown raw  (totality preserved)
existing arrows/named/function/bare-char   -> unchanged
```

## FSI exercise checklist (Constitution I, step 2)
1. `Control.renderTree theme {Width=640;Height=480} tree` returns a `ControlRenderResult`
   whose `Scene` differs for two structurally different `tree` values.
2. Construct an `InteractiveAppHost` record and pass it to `Viewer.runInteractiveApp`
   in a headless/bounded run; observe a synthetic pointer press dispatch a `msg`.
3. `ViewerKeyboard.normalize "Number5"` = `Digit 5`; `normalize "KeyL"` = `Letter 'L'`;
   `normalize "Totally-Unknown"` = `Unknown "Totally-Unknown"`.
