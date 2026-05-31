// US3 (FR-008, SC-003) consumer name-collision fixture.
//
// Realistic authoring order: the consumer declares its own domain — including a
// `Normal` case and its own `update`/`init` — and THEN opens the viewer
// namespace. Before `[<RequireQualifiedAccess>]`, the framework's bare
// `ViewerWindowStartupState.Normal` is imported unqualified and SHADOWS the
// consumer's `AppState.Normal`, so `init`/`update` fail to type-check.
// After the hardening, the framework's `Normal` requires qualification, the
// consumer's `Normal` stays in scope, and this compiles unchanged.
//
// `update`/`init` are plain consumer let-bindings here on purpose: the
// framework's `Viewer.update` / `ElmishAdapter.update` / `Keyboard.update` are
// module-qualified (no `[<AutoOpen>]`), so they never shadow these — proving the
// enumeration in ../collision-name-enumeration.md.

#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

// Consumer's own domain, defined before opening the framework.
type AppState =
    | Normal
    | Busy

open FS.Skia.UI.Scene
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.SkiaViewer

// Consumer's own init/update — must resolve to the consumer's `Normal`.
let init () : AppState = Normal

let update (state: AppState) : AppState =
    match state with
    | Normal -> Busy
    | Busy -> Normal

// Touch the framework's qualified surface to prove the open is real and used.
let viewerOptions: ViewerOptions =
    { Title = "collision-fixture"
      InitialSize = { Width = 320; Height = 240 } }

let _firstState = init ()
let _nextState = update _firstState
printfn "us3 fixture compiled: %A -> %A (%s)" _firstState _nextState viewerOptions.Title
