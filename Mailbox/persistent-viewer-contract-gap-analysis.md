# Persistent Viewer Contract Gap Analysis

Date: 2026-05-26

Source context:
- Consumer project: `/home/developer/projects/tetrisdemo`
- FS.Skia.UI checkout: `/home/developer/projects/FS-Skia-UI`
- Consumer package versions observed: `0.1.13-preview.1`
- Affected package: `FS.Skia.UI.SkiaViewer`
- Related generated app profile: default generated `app` profile with Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls, and Controls.Elmish.

## Executive Summary

The generated Tetris demo could satisfy its Spec Kit implementation and evidence gates without providing a persistent interactive graphical desktop window. The demo implemented Tetris state, menus, scoring, levels, visual events, tests, deterministic scene evidence, and bounded viewer smoke evidence, but the default executable did not open a playable graphical app. It built the control tree and exited.

This happened because the currently consumed `FS.Skia.UI.SkiaViewer` package exposes bounded evidence APIs such as `Viewer.runBounded`, `Viewer.runUntilFirstFrame`, and `Viewer.runForFrames`, but does not expose a first-class persistent interactive app-host API in the package surface used by the generated app. The evidence workflow then treated bounded scene/viewer proof as sufficient for graphical readiness.

The durable fix is to add an explicit persistent viewer contract to `FS.Skia.UI.SkiaViewer`, make generated graphical apps use it by default, and add governance checks that fail when a generated app only prints, renders metadata, or runs bounded smoke from its default executable path.

## Problem Statement

The user expectation for an "elaborate Tetris demo" was a runnable graphical desktop app:

```text
dotnet run --project src/tetrisdemo/tetrisdemo.fsproj
```

should open a persistent window, accept keyboard input, render the game, and keep running until the user exits.

Instead, the implemented default path printed:

```text
Elaborate Tetris Demo controls: 33
```

and exited. The closest available graphical command was bounded smoke:

```bash
FS_SKIA_ENABLE_BOUNDED_VIEWER_SIMULATION=1 \
  dotnet run --project src/tetrisdemo/tetrisdemo.fsproj -- \
  --bounded-smoke specs/001-elaborate-tetris-demo/evidence/bounded-viewer-smoke-simulated.txt
```

which produced:

```text
status=ok smoke=bounded-viewer frames-rendered=1 renderer-mode=vulkan
```

That is useful evidence, but it is not a playable graphical application.

## Why This Slipped Through

### 1. Package Surface Lacks A Persistent App Host Contract

The consumed `FS.Skia.UI.SkiaViewer` package surface includes:

- `Viewer.runBounded`
- `Viewer.runUntilFirstFrame`
- `Viewer.runForFrames`
- `GeneratedAppHost.smoke`

These APIs are framed around bounded evidence collection. They can prove that a scene can be described, diagnostics can be produced, and a bounded run model can complete. They do not provide an obvious default contract for:

- opening a persistent desktop window
- running an app/event loop until close
- dispatching real keyboard events into app messages
- rendering model-derived frames over time
- processing ticks/subscriptions
- handling exit/close requests

The absence of this contract makes it easy for a generated app to implement only tests and evidence commands while still appearing graphically governed.

### 2. The Generated Template Default Path Is Too Weak

The generated app template pattern initializes the adapter program, builds a view/control tree, prints a count, and exits. This is useful as a compile/smoke check, but it is not a graphical app launch.

For a generated app profile that includes `FS.Skia.UI.SkiaViewer`, the default executable should be the real graphical host path. Bounded smoke and scene metadata should be opt-in CLI flags.

### 3. Evidence Gates Do Not Distinguish "Visual Evidence" From "Playable Window"

The Spec Kit task and audit flow accepted:

- deterministic scene evidence
- bounded viewer smoke evidence
- unsupported renderer diagnostics
- semantic tests for model and view behavior

Those are necessary, but not sufficient for an interactive desktop app. The governance model needs a separate requirement/evidence category for "persistent interactive viewer host path exists and is wired from the default executable."

### 4. Unsupported Renderer Handling Became Too Permissive

The current bounded smoke path reports live viewer smoke as unsupported unless `FS_SKIA_ENABLE_BOUNDED_VIEWER_SIMULATION=1` is set. That is honest evidence for bounded smoke, but in the generated app workflow it can mask a stronger product requirement: the package must expose a real persistent window API before an interactive app feature can be considered complete.

Unsupported host diagnostics should explain runtime limitations. They should not substitute for a missing package capability.

## Impact

The current contract shape creates false confidence:

- A generated app can pass tests and evidence audit while not opening a playable window.
- User-facing requirements such as "menus", "gameplay", "keyboard controls", and "visual effects" can be validated through model tests and bounded evidence without being reachable in a real graphical host.
- Consumers may blame their app implementation when the real gap is package/API capability.
- Future generated games/tools may repeat the same failure because the template and governance do not force persistent viewer wiring.

## Recommended Package Fixes

### 1. Add A Persistent Scene Run API

Add a real desktop window entry point for simple scene apps:

```fsharp
module Viewer =
    val run :
        options: ViewerOptions ->
        scene: SceneNode ->
            Result<unit, ViewerRunFailure>
```

This should open a window, render the scene, and keep running until the user closes the window or the host fails.

### 2. Add A Persistent Generated App Host API

Add a first-class app host contract for model-driven generated apps:

```fsharp
type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

module Viewer =
    val runApp :
        options: ViewerOptions ->
        host: GeneratedAppHost<'model,'msg> ->
            Result<unit, ViewerRunFailure>
```

`runApp` should:

- call `host.Init`
- open the native window
- render `host.View model`
- dispatch keyboard events through `host.MapKey`
- call `host.Update` for user messages
- process `host.Tick` on frame/time cadence
- interpret `ViewerEffect` values at the viewer edge
- close on host close or `CloseWindow`
- surface `ViewerRunFailure` with blocked stage, category, classification, and message

This makes the app/event-loop contract impossible to confuse with bounded evidence.

### 3. Preserve Bounded Evidence APIs As Evidence APIs

Keep the existing bounded APIs, but document them explicitly as evidence helpers:

```fsharp
module Viewer =
    val runBounded : ...
    val runUntilFirstFrame : ...
    val runForFrames : ...
```

Their documentation should state:

- bounded APIs do not replace `run` or `runApp`
- bounded APIs may be used in CI/readiness evidence
- bounded simulation is not proof that the persistent viewer works

### 4. Add Runtime Capability Detection

Generated apps need to distinguish missing package capability from unsupported host environment:

```fsharp
type ViewerRuntimeCapability =
    { SupportsPersistentWindow: bool
      SupportsBoundedSmoke: bool
      SupportsKeyboardInput: bool
      RendererMode: string
      UnsupportedReason: string option }

module Viewer =
    val runtimeCapability : unit -> ViewerRuntimeCapability
```

The default app can then fail early with actionable diagnostics:

```text
status=failed graphical-app
blocked-stage=App
classification=ProductDefect
message=FS.Skia.UI.SkiaViewer does not expose a persistent app host in this package version.
```

or:

```text
status=unsupported graphical-app
blocked-stage=Window
classification=UnsupportedEnvironment
message=DISPLAY or WAYLAND_DISPLAY is required on Linux.
```

### 5. Provide A Control/Elmish Bridge To Scene

Generated apps using `FS.Skia.UI.Controls` and `FS.Skia.UI.Controls.Elmish` also need a clear route from `Control<'msg>` to `SceneNode` for the viewer. If that route exists internally, expose and document it as the supported generated-app path. If it does not exist, add one.

The generated app should not have to choose between:

- a Controls view that can be counted/tested but not launched
- a Scene view that can be bounded-smoked but not reuse Controls widgets

## Recommended Template Fixes

### 1. Make Default `main` Launch The Persistent App

For generated app profiles that include SkiaViewer, the default executable should call the persistent app host:

```fsharp
[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--bounded-smoke" :: path :: _ -> boundedSmoke false path
    | "--scene-evidence" :: path :: _ -> sceneEvidence path
    | _ ->
        match Viewer.runApp viewerOptions generatedHost with
        | Ok () -> 0
        | Error failure ->
            eprintfn "status=failed graphical-app blocked-stage=%A classification=%A category=%A message=%s"
                failure.BlockedStage
                failure.Classification
                failure.DiagnosticCategory
                failure.Message
            1
```

Bounded smoke should remain available, but only behind explicit flags.

### 2. Add A Generated App Host Skeleton

Generated app source should include:

- `Model`
- `Msg`
- pure `init`
- pure `update`
- `view`
- `mapKey`
- `tick`
- `viewerOptions`
- `generatedHost`

This gives every generated interactive app a standard path from viewer events to application state.

### 3. Fail Generated Guidance Checks When Default Main Is Not Graphical

Add a generated guidance check that fails when a SkiaViewer app:

- defaults to `printfn` and exits
- only calls `Control.count`
- only exposes `--bounded-smoke`
- lacks `Viewer.runApp` or equivalent persistent host invocation
- lacks a `MapKey` path for keyboard-capable profiles

This would have caught the Tetris demo failure immediately.

## Recommended Governance / Spec Kit Fixes

### 1. Add A Viewer Capability Gate To Planning

Generated graphical features should require a planning check:

```md
Viewer host capability gate: confirm the installed FS.Skia.UI.SkiaViewer
version exposes a persistent interactive run API, not only bounded evidence APIs.
If absent, implementation is blocked or must explicitly expand package/framework scope.
```

### 2. Add A Required Task For Default Graphical Launch

Task generation should include a task like:

```md
- [ ] Txxx [US*] [skillist: fs-skia-project, fs-skia-skiaviewer]
  Wire the default executable path to a persistent interactive graphical viewer
  window and verify it remains running until user exit on supported hosts.
```

This task should not be markable `[X]` using only `runBounded`.

### 3. Add A Distinct Evidence Artifact

Require:

```text
specs/<feature>/evidence/graphical-app-launch.txt
```

with fields:

```text
status=ok|unsupported|failed
mode=persistent-window
command=dotnet run --project ...
window-opened=true|false
input-dispatch=true|false
exit-path=true|false
blocked-stage=
classification=
message=
```

### 4. Audit For Bounded-Only Substitution

The evidence audit should flag generated graphical apps where:

- a SkiaViewer package is referenced
- tasks claim user-facing graphical completion
- only bounded smoke/scene evidence exists
- no persistent graphical app launch evidence exists

This should be a readiness contract hit, not a diff-scan advisory.

## Acceptance Criteria For The Fix

The package/template/governance fix should be considered complete when:

1. `FS.Skia.UI.SkiaViewer` exposes a persistent `Viewer.run` and/or `Viewer.runApp` API.
2. The generated app template uses the persistent API by default.
3. Bounded smoke remains available under explicit CLI flags.
4. Generated guidance checks fail a SkiaViewer app whose default path only prints or runs bounded evidence.
5. Spec Kit task generation includes a default graphical launch task for graphical features.
6. Evidence audit can distinguish persistent graphical launch evidence from bounded smoke evidence.
7. A generated Tetris-style app can be launched with:

   ```bash
   dotnet run --project src/<product>/<product>.fsproj
   ```

   and opens a persistent window on a supported host.

## Migration Notes

Existing generated apps that only expose bounded smoke should not silently pass as graphical apps after this change. They should either:

- adopt `Viewer.runApp`
- mark themselves as non-interactive/headless demos
- or document the missing persistent viewer API as a blocking runtime/package capability gap

For compatibility, bounded APIs can remain unchanged. The key change is that generated graphical app readiness must require a persistent app-host path in addition to bounded evidence.

## Suggested First Implementation Slice

1. Add `Viewer.runApp` to `src/SkiaViewer/SkiaViewer.fsi` and `src/SkiaViewer/SkiaViewer.fs`.
2. Add SkiaViewer tests that assert:
   - init opens a window effect
   - key events route through `GeneratedAppHost.MapKey`
   - `CloseWindow` exits the run loop
   - unsupported host conditions return `UnsupportedEnvironment`
3. Update `template/base/src/Product/Program.fs` so default `main` uses `Viewer.runApp`.
4. Add generated guidance checks for persistent graphical launch.
5. Regenerate or update a sample generated app and verify the default path is no longer a print-and-exit smoke.

This should prevent future generated apps from satisfying visual/governance evidence while missing the actual playable graphical host.
