# Contract: Animation public surface (`FS.Skia.UI.Scene.Animation`)

Tier 1, additive-only. This contract pins the shape the new public `.fsi` must expose. Exact
signatures are validated by FSI transcripts, `PackageSurfaceCheck`, and `PerPackageSurfaceDiff`.

## Value types

```fsharp
type Easing =
    | Linear
    | EaseIn
    | EaseOut
    | EaseInOut

type Transform =
    { TranslateX: float
      TranslateY: float
      ScaleX: float
      ScaleY: float
      RotationDegrees: float }

type Tween<'a> =
    { Start: 'a
      End: 'a
      Duration: System.TimeSpan
      Easing: Easing }

type Animation =
    { Opacity: Tween<float> option
      Transform: Tween<Transform> option
      Color: Tween<Scene.Color> option }

type AnimationState<'a> =
    { Current: 'a
      Start: 'a
      Target: 'a
      Elapsed: System.TimeSpan
      Duration: System.TimeSpan
      Easing: Easing }
```

## Module surface (signature shape)

```fsharp
module Easing =
    val apply: easing: Easing -> t: float -> float          // endpoints pinned, input clamped
    val [<Literal>] Default: Easing                         // = EaseInOut (FR-003)

module Transform =
    val identity: Transform
    val isIdentity: transform: Transform -> bool
    val lerp: a: Transform -> b: Transform -> t: float -> Transform
    val toPerspectiveTransform: transform: Transform -> Scene.PerspectiveTransform

module Color =                                              // extends interpolation, additive
    val lerp: a: Scene.Color -> b: Scene.Color -> t: float -> Scene.Color

val lerpFloat: a: float -> b: float -> t: float -> float

module Tween =
    val progress: elapsed: System.TimeSpan -> tween: Tween<'a> -> float
    val sample: interp: ('a -> 'a -> float -> 'a) -> elapsed: System.TimeSpan -> tween: Tween<'a> -> 'a

module Animation =
    val empty: Animation
    val applyAt: elapsed: System.TimeSpan -> animation: Animation -> target: Scene.Scene -> Scene.SceneNode
    val sampleFrames: times: System.TimeSpan list -> animation: Animation -> target: Scene.Scene -> Scene.Scene list
    val isSettled: elapsed: System.TimeSpan -> animation: Animation -> bool

module AnimationState =
    val create: interp: ('a -> 'a -> float -> 'a) -> initial: 'a -> duration: System.TimeSpan -> easing: Easing -> AnimationState<'a>
    val advance: delta: System.TimeSpan -> state: AnimationState<'a> -> AnimationState<'a>
    val retarget: newTarget: 'a -> state: AnimationState<'a> -> AnimationState<'a>
    val value: state: AnimationState<'a> -> 'a
    val isActive: state: AnimationState<'a> -> bool
```

> The exact `Default`/`empty` spelling and whether `Color.lerp` is a sub-module vs. a flat
> binding are tasks-phase details; the *contract guarantees* below are binding regardless.

## Contract guarantees

1. **Additive only.** Every symbol above is new. No existing `FS.Skia.UI.Scene` (or Elmish)
   signature changes shape; regenerated baselines show additions only
   (`PackageSurfaceCheck` / `PerPackageSurfaceDiff`).
2. **No `obj`, no stringly-typed values.** Properties, easing, and durations are strongly typed
   (`Tween<'a>`, `Easing`, `TimeSpan`).
3. **Endpoints pinned.** `Easing.apply e 0.0 = 0.0` and `Easing.apply e 1.0 = 1.0` for every
   `e` *(AnimationTests easing-endpoints)*.
4. **Clamped domain.** `Tween.progress` / `Tween.sample` clamp `elapsed` to `[0, Duration]`;
   `Duration ≤ 0` ⇒ progress `1.0` *(AnimationTests clamp / zero-duration)*.
5. **Identity-at-rest pass-through.** `Animation.applyAt` returns the target scene unwrapped
   when sampled opacity `= 1.0` and transform is identity *(AnimationOutputTests settled≡static,
   un-animated-unchanged)*.
6. **Retarget without snap-back.** `AnimationState.retarget` sets `Start = Current`
   *(AnimationTests retarget-no-snapback)*.
7. **Pure.** No function above performs I/O, reads wall-clock, or mutates shared state; the only
   interpreter-edge component is the Elmish tick subscription.
