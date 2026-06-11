---
name: fs-skia-controls-host
description: Maintainer-facing guide to the FS.Skia.UI.Controls.Elmish interactive-host seam — how runInteractiveApp drives the retained render structure each frame (RetainedRender.step over the keyed diff), advances per-identity animation clocks from an injected Tick delta, stamps runtime visual state pre-reconcile, routes keys focus-first through routeFocusedKey, and resolves pointer hits to a stable identity via retainedHitTest. Use when reading or extending the live controls host loop, the per-frame retained-state/clock/visual-state wiring, or the key/pointer routing seam.
compatibility: F# net10.0; FS.Skia.UI.Controls.Elmish package. Public entry `ControlsElmish.runInteractiveApp` (feature 085); the per-frame retained wiring is `module internal RetainedRender` (Controls) + internal host seams (`routeFocusedKey`, `resolveFocus`) reached via InternalsVisibleTo. Zero public-surface delta from the 096–103 roadmap — `runInteractiveApp`'s signature is unchanged.
metadata:
  author: fs-skia-ui
  source: specs/104-refresh-live-path-skills/plan.md
---

# fs-skia-controls-host

The **interactive-host seam** for `FS.Skia.UI.Controls.Elmish`: the live loop that drives a
`Control<'msg>` tree to a window, frame by frame, over the wired keyed reconciler. This is the
**maintainer-facing** counterpart to the consumer-facing [[fs-skia-viewer-host]] — it documents what
`runInteractiveApp` does *internally* each frame (the retained structure it holds, the animation
clocks it advances, the visual state it stamps, the keys and pointers it routes), not just the
host record a consumer fills in.

It exists because the R1–R6 live-path roadmap (096–103) layered real per-frame behavior onto the
091 wiring — incremental layout, an injected-delta animation clock, a visual-state cross-fade — and
that behavior lives at the `Controls.Elmish` host edge, not in any single control. A maintainer
touching the host loop needs one place that maps it.

## Scope / when to use

Use this skill when:

- You are reading or extending `src/Controls.Elmish/ControlsElmish.fs(i)` — the `runInteractiveApp`
  loop, `routeFocusedKey`, `resolveFocus`, or the per-frame `RetainedRender.step` call.
- You need to know **where** per-control state (focus, text, animation clock) lives across frames and
  how it survives a positional shift (the interpreter-edge ref state keyed by stable `RetainedId`).
- You are wiring a new per-frame concern (a clock, a visual-state source, a key/pointer route) and
  need the order: assemble visual state → stamp pre-reconcile → diff → advance clocks → sample on
  paint.
- You are tracing why a key reaches (or doesn't reach) a focused control vs. `host.MapKey`, or why a
  pointer click resolves to a particular control identity.

Do **not** use this for *consumer* host authoring (filling in `InteractiveAppHost`, the
preview-vs-tree decision, the windowed-fullscreen blur caveat) — that is [[fs-skia-viewer-host]].
Do **not** use it for the diff invariants themselves — that is [[fs-skia-reconciliation]]. This skill
is the **host that drives** the retained structure those two describe.

## Driven-library API (the host seam, `ControlsElmish.fsi`)

The live entry is **public**; the per-frame retained wiring is **internal** (reached by the adapter
and its tests via `InternalsVisibleTo`). Read `src/Controls.Elmish/ControlsElmish.fsi` and
`src/Controls/RetainedRender.fsi` for the authoritative signatures.

- **`runInteractiveApp : ViewerOptions -> InteractiveAppHost<'model,'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>`**
  (feature 085) — the live host entry. The `InteractiveAppHost` record carries
  `Init / Update / View / Theme / MapKey / MapPointer / Tick / Diagnostics`: `Init : unit ->
  'model * ViewerEffect list`, `Update : 'msg -> 'model -> 'model * ViewerEffect list`,
  `View : Size -> 'model -> Control<'msg>` (size-aware), `Theme`, `MapKey : ViewerKey -> bool ->
  'msg option`, `MapPointer : PointerInteraction -> 'msg option`, `Tick : TimeSpan -> 'msg option`.
- **`RetainedRender.step : theme -> size -> prev:RetainedRender<'msg> -> next:Control<'msg> -> RetainedRenderStep<'msg>`**
  / **`RetainedRender.init`** (Controls, `module internal`) — produce each frame from the retained
  `prev` and the next lowered tree by `Reconcile.diff`-ing and reusing fragments. The
  `RetainedRender<'msg>` the host holds carries `Root`, `NextId`, `StateByIdentity : Map<RetainedId,
  RetainedUiState>`, `Theme` (the fragment-reuse key), and `Layout : LayoutResult` (the previous
  frame's bounds cache).
- **`RetainedRender.advance` / `updateClockForState` / `sampleOnPaint`** — advance a per-identity
  `AnimationClock` by an injected delta, decide its start/retarget/advance/drop on a visual-state
  change, and composite the cross-fade on paint.
- **`ControlRuntime.applyRuntimeVisualState : model -> control -> control`** (internal) — stamp each
  control's derived `VisualState` onto the lowered tree **pre-reconcile**.
- **`routeFocusedKey` (internal)** — route a delivered key to the focused control over the retained
  tree; **`resolveFocus` / `RetainedRender.retainedHitTest` (internal)** — resolve a point to a
  stable `RetainedId`.

### What the host does each frame

1. **Assemble + stamp visual state.** Build a `ControlRuntimeModel` from the live pointer / focus
   state and stamp it onto the lowered `host.View size model` tree with
   `ControlRuntime.applyRuntimeVisualState` — **before** the diff runs (byte-identical at `Normal`,
   feature 096 / R1).
2. **Diff + reuse.** Hand the stamped tree to `RetainedRender.step`, which `Reconcile.diff`-s it
   against the retained `prev`, reuses unchanged subtrees' cached fragments and the previous
   `Layout` bounds (incremental re-measure, 097/101 / R2), and re-keys per-control state to the
   stable `RetainedId` so it survives a positional shift (091 / E2).
3. **Advance clocks + sample.** On `host.Tick delta`, advance each identity's `AnimationClock` by the
   **injected** delta (`RetainedRender.advance`; no wall-clock), then paint composites the active
   clock's cross-fade with `RetainedRender.sampleOnPaint` (099/103 / R4/R6). A settled/absent clock
   paints byte-identically to the static render.
4. **Route input.** Keys go focus-first (text seam → `routeFocusedKey` → `host.MapKey`); pointer
   samples hit-test to a stable identity and dispatch the hit control's authored bindings (or
   `host.MapPointer`).

## Runnable example — fill the host, run it

The consumer surface is just the `InteractiveAppHost` record; the retained wiring is automatic.

```fsharp
open System
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

let host : InteractiveAppHost<Model, Msg> =
    { Init = fun () -> initModel, []
      Update = fun msg model -> update msg model              // pure: 'model -> 'model * ViewerEffect list
      View = fun (size: Size) model -> view size model        // size-aware: returns Control<'msg>
      Theme = Theme.light
      MapKey = fun key isDown -> mapKey key isDown            // last-resort: only keys no focused control consumed
      MapPointer = fun interaction -> mapPointer interaction  // fallback for unconsumed hits
      Tick = fun (delta: TimeSpan) -> None                    // delta drives the per-identity AnimationClock
      Diagnostics = Viewer.defaultDiagnostics }

// runInteractiveApp holds the RetainedRender in interpreter-edge ref state and produces every
// frame via RetainedRender.step — the consumer record above is unchanged from 085 (096–103 added
// no field; the live behavior rides the internal seam).
ControlsElmish.runInteractiveApp { Title = "App"; InitialSize = { Width = 1024; Height = 768 } } host
```

```fsharp
// Maintainer view of the per-frame seam (internal symbols, reached via InternalsVisibleTo):
// 1. stamp runtime visual state pre-reconcile (096):
let stamped = ControlRuntime.applyRuntimeVisualState runtimeModel (host.View size model)
// 2. diff + reuse fragments/bounds, carrying StateByIdentity/Layout/Theme (091/097/101):
let frame   = RetainedRender.step host.Theme size prev stamped
// 3. on Tick, advance each identity's clock by the INJECTED delta (099 — no wall-clock):
let clock'  = RetainedRender.advance delta clock
// 4. route a key focus-first; an unconsumed key falls through to host.MapKey:
let retained', runtimeMsgs, productMsgs = ControlsElmish.routeFocusedKey retained focused order key shift
// 5. resolve a pointer to a stable identity (no unkeyed-sibling collision, 092):
let hit     = RetainedRender.retainedHitTest x y prev
```

## Key & pointer routing (the seam's two edges)

- **Keys are routed focus-first** (feature 094 / E4). Each native key is offered to the E1
  `routeFocusedText` text seam (a focused TEXT control's printable keys), then to internal
  `routeFocusedKey retained focused order key shift`, which reads the focused control's
  `KeyboardOperation` and applies `Focus.route`: `Activate` fires the focused control's authored
  activation bindings once; `Navigate` steps its value/selection (a slider's arrow); `Traverse`
  moves focus via `Focus.traverse`; `Fallthrough` yields no message — and **only then** the host
  consults `host.MapKey`. So `MapKey` is the last resort for keys no focused control and no traversal
  consumed.
- **Pointers resolve to a stable identity** (feature 092 / FR-004). `RetainedRender.retainedHitTest`
  (via the adapter's `resolveFocus`) returns the deepest retained node whose cached `Fragment.Box`
  contains the point — a **per-node** identity with no unkeyed-same-kind-sibling collision (unlike
  the `ControlId` `hitTest` path). A press sets focus to the focusable control under it (so a later
  key reaches it); the hit control's authored `EventBindings` win, with `host.MapPointer` the
  fallback for unconsumed interactions.

## Persistent problems

When the host loop, a clock, or a route will not behave after reasonable in-repo attempts,
**official online docs first** (the F#/.NET docs and the driven library's own documentation — Elmish
MVU for the update/effect boundary, React reconciliation/keys as the keyed-diff prior art), then
community sources (forums, issue trackers, changelogs). Record findings and resolving links in
`specs/<feature>/feedback/` and, for durable lessons, in this skill's **Sources** line. Offline,
record "research blocked — <why>" rather than hard-failing the phase.

## Related

- [[fs-skia-reconciliation]] — the keyed VDOM diff and the retained `RetainedRender` structure this
  host drives each frame (`step`, the bounds cache, the animation clock, `sampleOnPaint`).
- [[fs-skia-viewer-host]] — the **consumer-facing** counterpart: filling in `InteractiveAppHost`,
  the preview-vs-tree (`Control.render` vs `Control.renderTree`) distinction, the windowed-fullscreen
  blur caveat, and keyboard warm-up.
- [[fs-skia-ui-widgets]] — building the `Control<'msg>` tree (and its E1–E5 capabilities) this host
  renders and routes input to.
- [[fs-skia-elmish]] — the MVU `init`/`update`/`Effect` wiring the host's interpreter edge folds.

## Sources / links

- Feature 085 (interactive host) / 091–103 (live retained path): `specs/085-*/plan.md`,
  `specs/104-refresh-live-path-skills/plan.md`; signatures in `src/Controls.Elmish/ControlsElmish.fsi`
  and `src/Controls/RetainedRender.fsi`.
- Elmish (the MVU model the host loop follows): https://elmish.github.io/elmish/
- React reconciliation & keys (keyed-VDOM-diff prior art the retained path builds on):
  https://react.dev/learn/preserving-and-resetting-state
- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
