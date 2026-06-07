---
title: Elmish / MVU Runtime
category: Architecture
categoryindex: 3
index: 5
description: The Elmish/MVU runtime — how a user model, messages, update, effects, and the animation tick drive the rendered scene.
---

# Elmish / MVU Runtime

The `FS.Skia.UI.Elmish` package is the thin bridge between a standard
[Elmish](https://elmish.github.io/elmish/) Model-View-Update program and the
Skia/Vulkan viewer host. It does **not** reimplement the Elmish loop; it wraps
your own `model`/`msg` together with the viewer's state so a single `update`
folds both your application logic and the rendered scene forward, and it adds one
interpreter-edge helper — the animation tick — for time-driven redraws. This page
explains the runtime for a newcomer: the public types, how a message flows from
dispatch through `update` to effects and back to a re-rendered scene, and where
the boundaries between pure logic and host side effects sit.

This is a small package by design. The two source files are
[`Elmish.fs`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/src/Elmish/Elmish.fs)
(the adapter) and
[`AnimationTick.fs`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/src/Elmish/AnimationTick.fs)
(the tick subscription). The adapter contract is published in
[`Elmish.fsi`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/src/Elmish/Elmish.fsi).

## Where this sits

The Elmish runtime is one part of a layered framework. Below it the
[scene](./scene.html) layer supplies the immutable drawing vocabulary, the
[layout](./layout.html) engine resolves geometry, and [input](./input.html)
turns host events into messages. The [controls suite](./controls.html) composes
over all three. The Elmish package is the seam that lets a pure MVU program drive
the host without the program ever touching Vulkan, Skia, or window events
directly.

## The adapter contract

The whole adapter is three types and two functions. The model and message types
wrap your own:

```fsharp
type ElmishAdapterModel<'model> =
    { UserModel: 'model
      Scene: SceneNode
      Viewer: ViewerModel }

type ElmishAdapterMsg<'msg> =
    | UserMsg of 'msg
    | ViewerMsg of ViewerMsg

type ElmishAdapterEffect<'msg> =
    | DispatchUser of 'msg
    | DispatchViewer of ViewerEffect
```

- [`ElmishAdapterModel<'model>`](../reference/fs-skia-ui-elmish-elmishadaptermodel-1.html)
  is the bridged state: your `UserModel`, the current `Scene` (a `SceneNode` from
  the [Scene](./scene.html) layer), and the host's `ViewerModel`.
- [`ElmishAdapterMsg<'msg>`](../reference/fs-skia-ui-elmish-elmishadaptermsg-1.html)
  is a message envelope. `UserMsg` carries your own message; `ViewerMsg` carries a
  viewer message such as a window or frame event.
- [`ElmishAdapterEffect<'msg>`](../reference/fs-skia-ui-elmish-elmishadaptereffect-1.html)
  is the matching effect envelope. `DispatchUser` re-routes a message back into
  your program; `DispatchViewer` carries a `ViewerEffect` for the host
  interpreter to perform.

The module [`ElmishAdapter`](../reference/fs-skia-ui-elmish-elmishadapter.html)
holds the two functions. `init` builds the combined model from `ViewerOptions`,
your initial user model, and an initial scene, returning the model plus the
viewer's startup effects:

```fsharp
val init:
    viewerOptions: ViewerOptions ->
    userModel: 'model ->
    scene: SceneNode ->
        ElmishAdapterModel<'model> * ElmishAdapterEffect<'msg> list
```

`update` folds one envelope into the model, using a supplied `render: 'model ->
SceneNode` to refresh the scene:

```fsharp
val update:
    render: ('model -> SceneNode) ->
    msg: ElmishAdapterMsg<'msg> ->
    model: ElmishAdapterModel<'model> ->
        ElmishAdapterModel<'model> * ElmishAdapterEffect<'msg> list
```

## How a message flows

The control flow is deliberately literal. The entire `update` body is:

```fsharp
let update render msg model =
    match msg with
    | UserMsg userMsg -> model, [ DispatchUser userMsg ]
    | ViewerMsg viewerMsg ->
        let viewer, effects = Viewer.update viewerMsg model.Viewer
        let next = { model with Viewer = viewer; Scene = render model.UserModel }
        next, (effects |> List.map DispatchViewer)
```

Two distinct paths follow from this:

1. **A user message** (`UserMsg`) is **not interpreted by the adapter**. The
   adapter leaves the model untouched and emits a single `DispatchUser userMsg`
   effect. That effect re-enters the host's dispatch loop, where your own
   `update` is responsible for advancing `UserModel`. In other words the adapter
   does not own your reducer — it forwards your message and stays out of the way.
2. **A viewer message** (`ViewerMsg`) is delegated to
   [`Viewer.update`](../reference/fs-skia-ui-skiaviewer-viewer.html), which
   advances the `ViewerModel` and returns `ViewerEffect`s. Crucially, the
   adapter **re-renders here**: it rebuilds `Scene` by calling `render
   model.UserModel`, so the displayed scene tracks the latest user model on every
   viewer turn. The viewer effects are wrapped as `DispatchViewer` and handed
   back for the host to interpret.

So the round trip is: a host event becomes a `ViewerMsg`; `update` advances the
viewer and rebuilds the scene from the user model; the new scene lives in the
adapter model; the viewer effects flow to the host interpreter, which draws the
frame. This mirrors the broader viewer program contract documented in
[Runtime Design](../reports/runtime-design.html), where applications own
`Model`/`Msg`/`init`/`update`/`view` and the viewer owns the interpreter for
`ViewerEffect`. The adapter is the glue that keeps your render function in lockstep
with viewer turns without your code calling the renderer.

## Effects and the boundary

The adapter never performs a side effect itself. It only emits descriptions —
`DispatchUser` and `DispatchViewer` — that the host loop interprets. This keeps
the package pure and testable: `init` and `update` are ordinary functions over
immutable records, with no GPU surface, window, or timer involved. The actual
Vulkan/Skia work, screenshot capture, and shutdown live behind `ViewerEffect` in
the host (see [Runtime Design](../reports/runtime-design.html) for the
event→effect→interpreter pipeline). The configuration of the governance tooling
that decides which gates validate such changes is itself compiled F# rather than
runtime-parsed data, per
[ADR 0005](../adr/0005-configuration-representation.html); the same
"describe, don't perform" discipline shows up in the adapter's effect lists.

## The animation tick

The one moving part beyond the adapter is the animation tick in
[`AnimationTick.fs`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/src/Elmish/AnimationTick.fs)
(feature 073). It is the only interpreter-edge component of the animation slice:
it advances time by emitting frame-delta messages, and it gates redraws so that
the host stops requesting frames once the UI settles.

```fsharp
type AnimationTick = AnimationTick of TimeSpan

module Animation =
    val tickSubscription:
        isAnimating: ('model -> bool) ->
        toMsg: (TimeSpan -> 'msg) ->
        interval: TimeSpan ->
        model: 'model ->
            Sub<'msg>
```

[`Animation.tickSubscription`](../reference/fs-skia-ui-elmish-animation.html) is
shaped to plug directly into Elmish's `Program.withSubscription`. Its behaviour:

- While `isAnimating model` holds, it returns a `Sub<'msg>` with one entry keyed
  `["fs-skia-ui"; "animation-tick"]`. On start it dispatches an **immediate**
  first frame (`toMsg interval`) so the first advance does not wait a full
  interval, then a `System.Threading.Timer` dispatches `toMsg interval` once per
  `interval` thereafter.
- The delta carried is always the **nominal** `interval`, not a measured
  wall-clock delta. This is a deliberate fixed-step time model — deterministic and
  matching the fixed-step game-loop convention — rather than a variable-step
  clock.
- Once `isAnimating model` returns `false`, the subscription returns `Sub.none`.
  With no subscription entry the host stops requesting frames, so an idle UI does
  no redraw (the FR-006 redraw-gating goal, enforced at the framework-request
  level). The author embeds the `TimeSpan` in their own message via `toMsg`, or
  pattern-matches the bundled `AnimationTick` case directly.

The tick is additive: it is a subscription the author opts into, and the carried
delta flows through the *same* `update`/effect path as any other message. Nothing
about animation special-cases the adapter.

## Putting it together

A minimal usage (from the package README) shows the shape end to end:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Elmish

type Model = { Count: int }
type Msg = Increment

let render (model: Model) : SceneNode =
    Text((20.0, 40.0), $"Count: {model.Count}", Colors.black)

let options = { Title = "Counter"; InitialSize = { Width = 640; Height = 480 } }

let initial, effects =
    ElmishAdapter.init options { Count = 0 } (render { Count = 0 })

let next, _ =
    ElmishAdapter.update render (UserMsg Increment) initial
```

For controls-aware authoring there is a parallel, richer adapter in
`FS.Skia.UI.Controls.Elmish` that lowers control runtime, keyboard, and pointer
effects into Elmish commands — see the [controls suite page](./controls.html).

## Related pages

- [Namespace reference: `FS.Skia.UI.Elmish`](../reference/fs-skia-ui-elmish.html)
- [API reference index](../reference/index.html)
- [Scene](./scene.html) · [Layout](./layout.html) · [Input](./input.html) ·
  [Controls suite](./controls.html)
- [Typed control front door & Penpot flow](../controls-design/typed-front-door.html)
- [Runtime Design report](../reports/runtime-design.html) ·
  [ADR 0005 — configuration representation](../adr/0005-configuration-representation.html)

## Analysis

### Implementation strengths

- The adapter is genuinely tiny and total: the entire `update` is a two-case
  match over immutable records with no exceptions, no mutable state, and no host
  dependency, so it can be unit-tested without a GPU surface — exactly the
  testability the [Runtime Design report](../reports/runtime-design.html) calls
  for.
- Re-rendering is centralised in one place — the `ViewerMsg` branch rebuilds
  `Scene = render model.UserModel` — so the displayed scene cannot silently drift
  out of sync with the user model on a viewer turn.
- The animation tick is the only component touching `System.Threading.Timer`, and
  it disposes the timer through the returned `IDisposable`, keeping the
  side-effecting surface small and contained.
- Redraw gating is implemented honestly: returning `Sub.none` when settled means
  the host literally stops being asked for frames, rather than rendering
  identical frames and discarding them.

### Implementation weaknesses

- The `UserMsg` branch does not run the user's `update`; it only forwards a
  `DispatchUser` effect. Correct behaviour therefore depends on the host loop
  wiring that effect back to the user reducer, which is implicit here and not
  enforced by the adapter's own types.
- The scene is **only** rebuilt on `ViewerMsg`; on the `UserMsg` path the
  adapter returns `model` unchanged, so anyone reasoning solely about the adapter
  must understand that the user-model change and the consequent re-render happen
  on a *later* viewer turn, not in the same step.
- The tick carries a fixed nominal `interval` delta, so a slow or stalled host
  produces animation time that diverges from wall-clock time; there is no
  catch-up or measured-delta path.
- The timer period is clamped to `max 1.0` millisecond and truncated to an
  `int64`; sub-millisecond intervals collapse to 1 ms, which is invisible to a
  caller passing, e.g., a 0.5 ms `TimeSpan`.

### Design pros

- Wrapping rather than reimplementing Elmish means the package inherits the whole
  Elmish ecosystem (`Program`, `Cmd`, `Sub`, `withSubscription`) instead of
  forking it, and the contract stays small enough to read in one sitting.
- The effect-envelope design (`DispatchUser` / `DispatchViewer`) keeps the pure
  reducer strictly separated from host side effects, consistent with the
  describe-don't-perform discipline that also motivates compiled-F# configuration
  in [ADR 0005](../adr/0005-configuration-representation.html).
- The fixed-step tick is deterministic-friendly, which suits screenshot and
  contract-smoke evidence where reproducible frames matter more than wall-clock
  fidelity.
- Generic over `'model`/`'msg`, the adapter imposes no shape on application state
  beyond "give me a `render`", so it composes with any Elmish program.

### Design cons

- The wrap-not-reimplement choice exposes Elmish concepts (`Sub`, `SubId`,
  `Dispatch`) directly to consumers, so the package is only approachable to
  someone already comfortable with Elmish; it is not a self-contained UI runtime.
- Forcing a full `render model.UserModel` on every viewer turn is simple but
  coarse: there is no diffing or memoisation at this layer, so render cost scales
  with how expensive the user's `render` is, every frame the viewer ticks.
- The fixed-step time model is a usability trade-off — correct for deterministic
  loops, but surprising for authors who expect a real elapsed delta and must
  instead measure time themselves if they need it.
- Splitting animation into a separate opt-in subscription keeps the core small
  but means time-driven UI is not "on" by default; authors must know to wire
  `tickSubscription` and supply an `isAnimating` predicate, or nothing animates.
