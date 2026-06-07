---
title: Layout
category: Architecture
categoryindex: 4
index: 3
description: The FS.Skia.UI.Layout subsystem — a Yoga-backed flexbox layout engine with a pure F# fallback, an Elmish workflow loop, and graph layout.
---

# Layout

`FS.Skia.UI.Layout` turns a declarative tree of layout *intents* into concrete
on-screen rectangles. You describe **what** you want — a column of children with
some padding, gaps, flex-grow weights and min/max sizes — and the engine computes
**where** everything lands. Internally it delegates the hard flexbox math to
[Facebook's Yoga](https://www.yarnpkg.com/package/yoga-layout) (via the `Yoga.Net`
package) but wraps it in a pure, immutable F# contract, carries a hand-written
pure fallback for when the native engine fails, and exposes an Elmish/MVU
workflow so layout can participate in the same message loop as the rest of an
FS.Skia.UI app. This page explains the data model, the evaluation pipeline, how
results feed the [scene](./scene.html) and [Elmish runtime](./elmish-mvu.html),
and the engine's honest trade-offs.

The package depends only on [`FS.Skia.UI.Scene`](./scene.html) and `Yoga.Net`; it
creates no windows and performs no rendering of its own. See the
[API reference](../reference/index.html) for every public member.

## The data model

Everything starts from a [`LayoutNode`](../reference/fs-skia-ui-layout-layoutnode.html)
tree. Each node carries an id, a `LayoutIntent`, a `LayoutVisibility`, an optional
content [`Scene`](./scene.html), an optional `ContentMeasure` function for
leaf-content measurement, and a list of child nodes:

```fsharp
type LayoutNode =
    { Id: LayoutNodeId            // string id, used for hit-testing and diagnostics
      Intent: LayoutIntent        // the flexbox styling for this node
      Visibility: LayoutVisibility // Visible | Hidden | Collapsed
      Measure: ContentMeasure option // leaf content sizing callback
      Content: Scene option       // what to draw at this node's bounds
      Children: LayoutNode list }
```

The [`LayoutIntent`](../reference/fs-skia-ui-layout-layoutintent.html) record is
the framework's flexbox vocabulary: `Direction` (`Row`/`Column`), `Wrap`,
`AlignItems`/`AlignSelf`/`JustifyContent` (the `LayoutAlign` cases include
`SpaceBetween`/`SpaceAround`/`SpaceEvenly`), `Padding`, `Margin`, `Gap`, explicit
`Size`/`MinSize`/`MaxSize`, and `FlexGrow`/`FlexShrink`/`FlexBasis`. These map
almost one-to-one onto Yoga style properties.

Evaluation against an
[`AvailableSpace`](../reference/fs-skia-ui-layout-availablespace.html) (a width and
height with `MeasureMode`s) produces a
[`LayoutResult`](../reference/fs-skia-ui-layout-layoutresult.html):

```fsharp
type LayoutResult =
    { Bounds: ComputedBounds list      // one ComputedBounds per node (id + LayoutBounds + visibility)
      Diagnostics: LayoutDiagnostic list
      Invalidated: LayoutNodeId list
      Revision: int64 }
```

The [`LayoutDiagnostic`](../reference/fs-skia-ui-layout-layoutdiagnostic.html) list
is a first-class output, not an exception channel. Bad inputs — a negative width,
a min larger than a max, an unmeasurable leaf, a duplicate node id, an empty id —
are *normalized* to safe values and reported as a diagnostic with a
`DiagnosticSeverity` and a `LayoutDiagnosticCode`, rather than throwing. The
[`Defaults`](../reference/fs-skia-ui-layout-defaults.html) module supplies
constructors (`layoutNode`, `availableSpace`, `layoutIntent`, `pixelSnapPolicy`,
`stackConfig`, …) so callers rarely write these records by hand.

## The evaluation pipeline

[`Layout.evaluate`](../reference/fs-skia-ui-layout-layout.html) is the core entry
point. A single call does several things in sequence:

1. **Normalize the available space.** Non-finite or negative width/height are
   clamped to `0` and recorded as an `InvalidAvailableSpace` error diagnostic.
2. **Run the pure validation pass.** `layoutNode` — a hand-written recursive
   flexbox approximation — runs *purely for its diagnostics* here, collecting
   padding/margin/gap/size normalization warnings and constraint conflicts.
3. **Try the Yoga layout.** `tryYogaLayout` builds a parallel tree of native Yoga
   nodes (`YGNodeNew`), applies each `LayoutIntent` via `applyYogaStyle`, wires a
   `YGMeasureFunc` for any leaf with a `Measure` callback, calls
   `YGNodeCalculateLayout`, then reads back absolute bounds and frees the native
   tree (`YGNodeFreeRecursive`).
4. **Fall back if Yoga throws.** If the native call fails for any reason the
   engine catches the exception, re-runs the pure `layoutNode` computation to
   produce real bounds, and appends a `FallbackBoundsApplied` warning naming the
   exception — the call still returns a usable `LayoutResult`.
5. **Validate the tree** for duplicate / empty ids and emit a summary
   `FallbackBoundsApplied` info diagnostic if any input needed bounded fallback
   geometry.

[`Layout.evaluateIncremental`](../reference/fs-skia-ui-layout-layout.html) is the
incremental door: it currently delegates to a full `evaluate`, bumps `Revision`,
and records the caller-supplied `changedNodeIds` in `Invalidated`. The interface
is incremental; the implementation re-evaluates the whole tree.

```text
LayoutNode tree + AvailableSpace
        │
        ▼
 normalizeAvailable ──► pure layoutNode (diagnostics only)
        │
        ▼
   tryYogaLayout ──(Ok)──► native bounds + measurement diagnostics
        │
      (Error ex)
        │
        ▼
 pure layoutNode (real bounds) + FallbackBoundsApplied
        │
        ▼
   LayoutResult { Bounds; Diagnostics; Invalidated; Revision }
```

## From bounds to pixels and scenes

A `LayoutResult` is geometry, not pixels. Three helpers connect it to rendering
and interaction:

- [`Layout.renderComputed`](../reference/fs-skia-ui-layout-layout.html) walks the
  result, looks up each node's `Content` scene by id, keeps only the `Visible`
  ones, and groups them into a single [`Scene`](./scene.html) via `Scene.group` —
  the value the [SkiaViewer host](./host-skiaviewer.html) ultimately draws.
- [`Layout.snapBounds`](../reference/fs-skia-ui-layout-layout.html) applies a
  [`PixelSnapPolicy`](../reference/fs-skia-ui-layout-pixelsnappolicy.html)
  (scale factor + `SnapMode` of `Floor`/`Round`/`Expand`) so fractional layout
  rectangles align to the device pixel grid, snapping the start and end edges
  independently to avoid sub-pixel seams.
- [`Layout.hitTestComputed`](../reference/fs-skia-ui-layout-layout.html) resolves
  a point to the front-most visible `LayoutNodeId` by scanning the bounds in
  reverse (last-drawn wins) after pixel-snapping. This is the same hit-test the
  pointer-interaction front door consumes (see [Input](./input.html)).

## The Elmish workflow loop

For apps that want layout to react to host resizes and node changes, the package
ships a small MVU loop mirroring the framework's
[Elmish/MVU](./elmish-mvu.html) conventions:

- [`Layout.initWorkflow`](../reference/fs-skia-ui-layout-layout.html) builds a
  [`LayoutWorkflowModel`](../reference/fs-skia-ui-layout-layoutworkflowmodel.html)
  (root, available space, last result, changed ids, snap policy) and emits an
  `EvaluateLayout` effect.
- [`Layout.updateWorkflow`](../reference/fs-skia-ui-layout-layout.html) is the
  pure reducer over
  [`LayoutWorkflowMsg`](../reference/fs-skia-ui-layout-layoutworkflowmsg.html)
  (`LayoutHostResized`, `LayoutVisibilityChanged`, `LayoutIntentChanged`,
  `LayoutMeasurementChanged`, `LayoutEvaluationCompleted`). Visibility/intent
  changes patch the node tree in place and request an incremental re-evaluation.
- [`Layout.interpretWorkflowEffect`](../reference/fs-skia-ui-layout-layout.html)
  is the impure edge: it runs `evaluate`/`evaluateIncremental` for a
  `LayoutWorkflowEffect` and feeds the result back as a
  `LayoutEvaluationCompleted` message.

The split keeps `updateWorkflow` pure and testable while the actual layout
computation (which calls into native Yoga) lives behind the effect interpreter.

## Convenience builders and graph layout

Beyond the node-tree engine the module exposes simpler, self-contained builders:
[`Layout.horizontalStack`](../reference/fs-skia-ui-layout-layout.html),
`verticalStack`, and `dock` take a `StackConfig`/`DockConfig` and a list of
[`LayoutChild`](../reference/fs-skia-ui-layout-layoutchild.html) values and return
a grouped `Scene`; `measureHorizontal`/`measureVertical` return the per-child
`LayoutBounds` for the even-split arithmetic they perform. (Note that the current
`horizontalStack`/`verticalStack`/`dock` bodies ignore their config and simply
group the children's content — the measurement functions, not the builders, carry
the geometry.)

The [`Graph`](../reference/fs-skia-ui-layout-graph.html) and
[`GraphValidation`](../reference/fs-skia-ui-layout-graphvalidation.html) modules
add node/edge graph layout on top: `Graph.layout`/`directed`/`undirected` return
`Result<_, GraphValidationIssue list>` (validating duplicate ids, missing
endpoints, self-loops, and cycles before placing), and `Graph.hitTest` maps a
point to a `GraphTarget` of `Node` or `Edge`.

## How it fits the framework

In a generated FS.Skia.UI app, layout sits between product state and the renderer:
the Elmish `view` produces a `LayoutNode` tree or stack/dock builders, `Layout`
computes `ComputedBounds`, `renderComputed` turns them into a `Scene`, and the
[host](./host-skiaviewer.html) presents it. The same `hitTestComputed` geometry is
what pointer input (see [Input](./input.html)) uses to address an interaction to
the correct node. The whole package is acyclic and host-independent — it knows
nothing about Vulkan, windows, or the event loop.

## Analysis

### Implementation strengths

- **Diagnostics-as-data, not exceptions.** Every invalid input —
  non-finite/negative dimensions, `min > max`, unmeasurable leaves, duplicate or
  empty ids — is normalized to a safe value and surfaced as a typed
  `LayoutDiagnostic` (`evaluate` returns a result even for garbage input), which
  makes the engine robust and easy to test.
- **A genuine native-failure fallback.** `tryYogaLayout` is wrapped in a
  try/catch that re-runs a real pure-F# layout (`layoutNode`) and frees the
  native tree on the error path, so a Yoga crash degrades to a usable result with
  a `FallbackBoundsApplied` warning rather than taking down the frame. There is
  even an `AppContext` switch (`ForceYogaFailure`) to exercise that path in tests.
- **Clean MVU separation.** `updateWorkflow` is a pure reducer and the impure
  layout computation is isolated in `interpretWorkflowEffect`, matching the rest
  of the framework's effect-at-the-edge discipline and keeping the reducer
  property-testable.

### Implementation weaknesses

- **"Incremental" is not incremental.** `evaluateIncremental` simply calls
  `evaluate` (a full re-layout of the whole tree) and only updates `Revision` and
  `Invalidated`; the `changedNodeIds` argument does not actually narrow the work,
  so the optimization the name promises is unimplemented.
- **Double work on every evaluate.** `evaluate` always runs the pure `layoutNode`
  pass for diagnostics *and* the full Yoga layout, so the hand-written flexbox
  approximation is computed on every call even when Yoga succeeds — wasted CPU on
  the hot path.
- **Stack/dock builders ignore their config.** `horizontalStack`/`verticalStack`/
  `dock` currently discard the `StackConfig`/`DockConfig` and just `Scene.group`
  the children's content; the geometry lives only in the separate
  `measureHorizontal`/`measureVertical` helpers, which is an easy footgun (the
  builder name implies it positions children, but it does not).

### Design pros

- **Borrowing a mature flexbox engine.** Delegating the actual layout math to
  Yoga means the framework inherits a battle-tested, widely-used flexbox
  implementation instead of maintaining a bespoke constraint solver, while still
  presenting an idiomatic immutable F# surface.
- **Layout is pure, declarative geometry.** The subsystem produces
  `ComputedBounds` values and never draws, which keeps it acyclic, host- and
  renderer-independent, and reusable for both rendering (`renderComputed`) and
  interaction (`hitTestComputed`) from one shared computation.
- **Pixel-snapping is a first-class, explicit policy.** Sub-pixel alignment is a
  caller-controlled `PixelSnapPolicy` (scale + mode, with independent start/end
  snapping) rather than hidden rounding, which gives crisp output across DPI
  scales without surprising the consumer.

### Design cons

- **A native dependency in an otherwise pure stack.** `Yoga.Net` pulls a native
  library into a framework that is otherwise managed F#, which complicates
  packaging/portability and is the very reason the fallback path has to exist;
  the pure and native layouts can also disagree subtly.
- **The diagnostic model trades safety for silence.** Because bad input is
  normalized to `0` and merely *reported*, a consumer that ignores the
  `Diagnostics` list can ship a visibly-broken layout with no hard failure — the
  honesty of the diagnostics depends on someone reading them.
- **String node ids and list-shaped results.** `LayoutNodeId` is a bare `string`
  and `LayoutResult.Bounds` is a flat list scanned linearly (e.g. reverse-scan
  hit-testing), which is simple but offers no compile-time id safety and scales
  poorly for very large trees.
