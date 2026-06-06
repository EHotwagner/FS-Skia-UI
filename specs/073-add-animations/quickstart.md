# Quickstart: Add Animations

## For a product author — fade + slide a panel into view (Story 1)

Declare the motion as data against the widget you already author. No clock, no per-frame
interpolation.

```fsharp
open System
open FS.Skia.UI.Scene
open FS.Skia.UI.Scene.Animation

// The panel you already render, as a Scene:
let panel : Scene = myPanelScene model

// Declare an entrance: opacity 0 -> 1 and slide up 24px -> 0 over 300ms, ease-out.
let entrance : Animation =
    { Animation.empty with
        Opacity =
            Some { Start = 0.0; End = 1.0
                   Duration = TimeSpan.FromMilliseconds 300.0; Easing = EaseOut }
        Transform =
            Some { Start = { Transform.identity with TranslateY = 24.0 }
                   End   = Transform.identity
                   Duration = TimeSpan.FromMilliseconds 300.0; Easing = EaseOut } }

// In your view, lower the animation at the model's current elapsed time:
let view (model: Model) : SceneNode =
    Animation.applyAt model.Elapsed entrance (myPanelScene model)
// At elapsed >= 300ms the transform is identity and opacity is 1.0, so applyAt returns the
// panel UNWRAPPED — byte-identical to the static render (FR-006). No redraw is requested once
// settled.
```

Drive elapsed time with the animation tick subscription so frames advance only while active:

```fsharp
open FS.Skia.UI.Elmish

let subscriptions (model: Model) =
    [ Animation.tickSubscription (fun m -> not (Animation.isSettled m.Elapsed entrance)) ]
    // emits `AnimationTick delta` only while the entrance is running; silent once it settles.

let update msg model =
    match msg with
    | AnimationTick delta -> { model with Elapsed = model.Elapsed + delta }, Cmd.none
    | ...                  -> ...
```

## For a product author — glide a value to a new target (Story 2)

```fsharp
// Hold an AnimationState in your model for the value that should glide:
type Model = { Bar: AnimationState<float>; ... }

let init () =
    { Bar = AnimationState.create lerpFloat 0.0 (TimeSpan.FromMilliseconds 200.0) EaseInOut },
    Cmd.none

let update msg model =
    match msg with
    | SetTarget v        -> { model with Bar = AnimationState.retarget v model.Bar }, Cmd.none
    | AnimationTick delta -> { model with Bar = AnimationState.advance delta model.Bar }, Cmd.none

let view model =
    let shown = AnimationState.value model.Bar   // continues from where it was on retarget
    renderBar shown
// A mid-flight SetTarget retargets from the displayed value — no snap back to 0 (FR-005/SC-006).
```

## For a framework maintainer — add/verify the slice

1. **Sketch the surface in FSI first** (Principle I): load the packed `FS.Skia.UI.Scene` and
   exercise `Easing.apply`, `Tween.sample`, `Animation.applyAt`, `AnimationState.retarget`
   before writing `Animation.fs`. Capture the transcript under `readiness/fsi/`.
2. **Write failing tests** (`tests/Scene.Tests/AnimationTests.fs`): easing endpoints +
   monotonicity (FsCheck), clamp + zero-duration, identity-at-rest, retarget-no-snapback.
3. **Implement** `src/Scene/Animation.fs` against the now-stable `Animation.fsi`; add the
   compile entry to `Scene.fsproj` (after `Scene.fs`).
4. **Add the tick helper** in `src/Elmish/AnimationTick.{fsi,fs}` and its `Elmish.Tests`
   gating test (active emits, settled silent).
5. **Capture parity goldens**: `FS_SKIA_CAPTURE_GOLDEN=1 dotnet test tests/Parity.Tests` to
   write `fixtures/v3-host-golden/scene-output/animation-*.txt`; re-run without the env var to
   prove byte-identical re-capture.
6. **Regenerate baselines**: `./fake.sh build -t RefreshSurfaceBaselines` (Scene + Elmish
   surface), and per-package baselines via `PerPackageSurface.captureCurrent`.

## Validate

```bash
# 1. Route FIRST — run only the gates it prints for the actual diff:
./fake.sh build -t Route

# 2. New public .fsi escalates to the package-surface rule (FocusedAuthority):
#    PackageSurfaceCheck, FsiTranscripts, PerPackageSurfaceDiff — plus Dev for the tests.
#    FAKE-backed targets share .fake state: run sequentially, never concurrently.
./fake.sh build -t Dev
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit     # after_implement hook; must be PASS, no [S]/[S*]
```
