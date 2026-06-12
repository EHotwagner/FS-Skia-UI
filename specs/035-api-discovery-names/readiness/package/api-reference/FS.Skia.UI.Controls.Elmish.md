# FS.Skia.UI.Controls.Elmish Source-Shaped API Reference

package-id: FS.Skia.UI.Controls.Elmish
package-version: local
generated-from: curated-fsi
assembly-reflection: false
repository-source-authoring-fallback: false
symbol-count: 136
xml-summary-count: 240
source-fsi-paths:
- src/Controls.Elmish/ControlsElmish.fsi
sampled-symbols:
omitted-symbol-reasons:
- none
unsupported-symbols:
- none
diagnostics:
- none

## Common Samples

## Curated Signatures
```fsharp
namespace FS.Skia.UI.Controls.Elmish

open System
open FS.Skia.UI.Controls
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open Elmish

/// Public contract type exposed by this FS.Skia.UI package.
type AdapterDiagnostic =
    { Code: string
      Message: string
      Source: string }

/// Public contract type exposed by this FS.Skia.UI package.
type AdapterEffect<'msg> =
    | DispatchProductMessage of 'msg
    | DispatchControlRuntimeMessage of ControlRuntimeMsg
    | DispatchKeyboardMessage of KeyboardMsg
    | DispatchHostCommand of string
    | ReportAdapterDiagnostic of AdapterDiagnostic

/// Public contract type exposed by this FS.Skia.UI package.
type AdapterCommand<'msg> = AdapterEffect<'msg> list

/// Public contract type exposed by this FS.Skia.UI package.
type AdapterSubscription<'msg> =
    { Id: string
      Subscribe: unit -> AdapterCommand<'msg> }

/// Public contract type exposed by this FS.Skia.UI package.
type AdapterProgram<'model, 'msg> =
    { Init: unit -> 'model * AdapterCommand<'msg>
      Update: 'msg -> 'model -> 'model * AdapterCommand<'msg>
      View: 'model -> Control<'msg>
      Subscriptions: 'model -> AdapterSubscription<'msg> list }

[<RequireQualifiedAccess>]
/// Feature 111 (US1, FR-001): the closed TRIGGER taxonomy naming WHY a frame ran. The scheduler
/// classifies each produced frame from the input that caused it and runs only the phases that cause
/// requires (`FrameMetrics.ViewCalled`/`DiffRan`/`LayoutRan`/`PaintRan`). `RequireQualifiedAccess` —
/// the case names `Key`/`Tick`/`Idle` would otherwise shadow a consumer's own `Msg` cases when it
/// `open`s this namespace, so they must be qualified (`FrameCause.Tick` etc.), exactly as `FrameInput`
/// requires. `Resize`/`Theme` are live-scheduler causes (a window resize / theme switch between
/// paints); the deterministic `Perf.runScript` corpus produces only `Idle`/`PointerMove`/
/// `PointerDiscrete`/`Key`/`Tick` (a model-driven theme change is a `Key` frame with the theme changed
/// as an effect, not a `Theme` cause).
type FrameCause =
    | Idle
    | PointerMove
    | PointerDiscrete
    | Key
    | Tick
    | Resize
    | Theme

/// Feature 108/109/110/111 (US1, FR-001/002): the per-frame structured work/timing signal the host
/// loop and the deterministic `Perf.runScript` driver both produce. The count/bool fields are the
/// byte-stable determinism surface (FR-007/SC-005); `FrameDuration` is reported for real perf
/// observation but EXCLUDED from golden assertions (it varies run to run, FR-012). Feature 109
/// replaced the conflating `ViewRebuilt` with the two precise booleans `ProductModelChanged` +
/// `ViewCalled` and added the integer `FullRenderCount`. Feature 110 added `FullRenderFallbackCount`
/// and narrowed `FullRenderCount`/`ViewCalled` so retained routing increments NEITHER. Feature 111
/// added `FrameCause` + the per-phase booleans `DiffRan`/`LayoutRan`/`PaintRan` (the VIEW phase is
/// `ViewCalled`) and narrowed `ViewCalled`/`FullRenderCount` to `false`/`0` on a model-unchanged frame
/// (the scheduler reuses the already-produced view tree, FR-003/FR-011).
type FrameMetrics =
    { /// A product message actually changed the model this frame (the reference identity of the folded
      /// model changed across `host.Update`). `false` for a no-message frame, a pure hover/focus
      /// frame, and an animation-only tick (FR-001/003/005).
      ProductModelChanged: bool
      /// THE VIEW PHASE: `host.View size model` actually ran this frame to (re)produce a tree. Feature
      /// 111 narrows this — it is `false` on a model-unchanged frame (including an animation-only tick,
      /// which formerly reported `true`) because the scheduler reuses the already-produced view tree and
      /// skips `host.View` (FR-003/FR-011); the overlay/paint fact moves to `PaintRan`. Still equals
      /// `FullRenderCount > 0`. Feature 110: retained pointer routing does not set it true either.
      ViewCalled: bool
      /// Number of full `host.View` + `Control.renderTree` materializations this frame performed — the
      /// retained-step render where it occurs, plus any oracle fallback render. Feature 110 narrowed
      /// this: routing a pointer event via the retained path increments NEITHER this nor `ViewCalled`
      /// (the per-sample routing full render is removed from the hot path, FR-008); a model-driven
      /// re-render after a dispatched message still counts.
      FullRenderCount: int
      /// Nodes re-measured this frame (from `WorkReductionRecord.RemeasuredNodeCount`); 0 on an idle
      /// frame, bounded (overlay-assembly, not whole-tree) on an animation-only frame.
      RemeasuredNodeCount: int
      /// Raw pointer samples that arrived this frame, including deferred/queued moves carried from a
      /// prior boundary (K before coalescing) (FR-008).
      PointerSamplesReceived: int
      /// Pointer MOVES actually applied after coalescing — at most one per frame (FR-009/SC-002).
      PointerMovesProcessed: int
      /// Feature 110 (FR-009): how many times retained pointer routing fell back to a full render to
      /// route an event this frame. `0` for every normal scripted pointer scenario (SC-005); non-zero
      /// only when the retained frame could not resolve a bindable hit and the preserved full-render
      /// oracle had to run (a counted correctness escape hatch, never the normal path). Deterministic,
      /// golden-asserted.
      FullRenderFallbackCount: int
      /// Feature 111 (FR-001): the trigger that caused this frame (idle / pointer-move / pointer-discrete
      /// / key / tick / resize / theme). Deterministic, golden-asserted. Names the trigger, not the
      /// effect — a key that changes the model is `FrameCause.Key` with `ProductModelChanged = true`.
      FrameCause: FrameCause
      /// Feature 111 (FR-002): the DIFF/reconcile phase ran — a newly-produced view tree was reconciled
      /// against the retained tree this frame (the retained step ran on a fresh `host.View`). An
      /// animation-only tick re-samples the overlay WITHOUT producing a new tree, so it reports `false`.
      DiffRan: bool
      /// Feature 111 (FR-002): the LAYOUT phase ran — at least one node was re-measured this frame
      /// (equivalent to `RemeasuredNodeCount > 0`, but set explicitly as part of the phase record).
      LayoutRan: bool
      /// Feature 111 (FR-002): the PAINT phase ran — the painted scene (a model render) or the animation
      /// overlay was (re)assembled this frame. `true` on model frames AND animation-only ticks; `false`
      /// on idle and pure routing frames. (Hit-test is intentionally NOT a phase field — clarified
      /// 2026-06-12: the deterministic path does not hit-test coalesced moves; routing work stays in
      /// `PointerSamplesReceived`/`PointerMovesProcessed`/`FullRenderFallbackCount`.)
      PaintRan: bool
      /// Wall-clock duration of the frame's work — reported, EXCLUDED from the golden/determinism
      /// surface (FR-012).
      FrameDuration: TimeSpan }

[<RequireQualifiedAccess>]
/// Feature 108 (US3, FR-009): one ordered step of the deterministic perf driver. `Key` carries the
/// parsed base key + held modifiers; `Pointer` carries an already-resolved `PointerInteraction`;
/// `Tick` advances animation clocks by an injected delta; `Idle` is a no-input frame.
/// `RequireQualifiedAccess` — the generic case names (`Key`/`Pointer`/`Tick`/`Idle`) would otherwise
/// shadow a consumer's own `Msg` cases when it `open`s this namespace, so they must be qualified
/// (`FrameInput.Tick` etc.).
type FrameInput<'msg> =
    | Key of ViewerKey * KeyModifiers
    | Pointer of PointerInteraction
    | Tick of TimeSpan
    | Idle

/// Pointer-routing, size-aware durable host (feature 085, research D3-AMEND). Mirrors
/// `GeneratedAppHost` field-for-field PLUS a `MapPointer` seam over `PointerInteraction` and a
/// size-carrying `View` that returns a `Control<'msg>` tree (so `Control.renderTree` yields the
/// `Scene` + `Layout` + `EventBindings` the host routes). Lives in Controls.Elmish — not SkiaViewer —
/// because `PointerInteraction`/`interpretPointerOutcome` are Controls surface and the viewer is
/// host-independent. `Theme` drives `renderTree`. Feature 090: a hit control's authored
/// `EventBindings` (`onClick`/`onChanged`) are dispatched in the live window; `MapKey` gains a
/// focus-aware text-routing seam for the focused text control (see `routeInteractivePointer`,
/// `routeFocusedText`, and `runInteractiveApp`). Feature 108: the additive `MapKeyChord` /
/// `OnFrameMetrics` fields carry inert defaults (at-rest byte-identical).
type InteractiveAppHost<'model, 'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> Control<'msg>
      Theme: Theme
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option
      Tick: TimeSpan -> 'msg option
      /// Feature 108 (US5, FR-016): an additive modifier-aware key seam consulted BEFORE `MapKey`.
      /// The default (`fun _ _ -> None`) ignores modifiers and defers to `MapKey`, so unmodified
      /// keys route exactly as today (at-rest byte-identical, SC-012).
      MapKeyChord: ViewerKey -> KeyModifiers -> 'msg option
      /// Feature 108 (US2, FR-006): an additive opt-in observability sink called once per frame with
      /// that frame's `FrameMetrics`. The default (`ignore`) is inert, so a host that does not
      /// observe metrics is byte-identical to its pre-108 behaviour (SC-012).
      OnFrameMetrics: FrameMetrics -> unit
      Diagnostics: ViewerDiagnosticsOptions }

/// Verdict of a responds-proof (feature 090, FR-006): `Responsive` when a real input applied to the
/// running host produced a visible change in the rendered output (`Before` ≠ `After`), `Inert` when
/// it did not. An inert host (renders but does not respond) can only yield `Inert`.
type RespondsVerdict =
    | Responsive
    | Inert

/// A captured input→visible-change responds-proof (feature 090, FR-006/FR-007): the `Before` frame,
/// the `After` frame produced by applying a real dispatched interaction (route → `host.Update` fold →
/// re-render, exactly as the live repaint loop), and the `Verdict`. A distinct evidence class from a
/// render-only screenshot (one frame, no interaction) and from the offscreen `runInteractivePointerOnce`
/// route probe (model layer only): an app that renders but does not respond yields identical frames and
/// an `Inert` verdict, so "renders" cannot be passed off as "responds".
type RespondsProof =
    { Before: Scene
      After: Scene
      Verdict: RespondsVerdict }

/// Pure, total bridge between the adapter's effect-list command model
/// (`AdapterCommand<'msg>`) and Elmish `Cmd<'msg>` (068, additive).
module AdapterCmd =
    /// The Elmish no-op command (= `Cmd.none`). Law: `toCmd route [] = none`.
    val none: Cmd<'msg>
    /// Lift a single product message into an `AdapterCommand`
    /// (= `[ DispatchProductMessage msg ]`). Law: `productMessages (ofMessage m) = [ m ]`.
    val ofMessage: msg: 'msg -> AdapterCommand<'msg>
    /// The ordered `DispatchProductMessage` payloads carried by the command
    /// (the round-trip oracle); no other effect case contributes.
    val productMessages: command: AdapterCommand<'msg> -> 'msg list
    /// Total conversion to an Elmish `Cmd<'msg>`: `route` maps EVERY `AdapterEffect`
    /// case (product and non-product) to a `'msg`, preserving list order; `[]` ->
    /// `Cmd.none`. Pure to construct; never throws. FR-003/FR-008.
    val toCmd: route: (AdapterEffect<'msg> -> 'msg) -> command: AdapterCommand<'msg> -> Cmd<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ControlsElmish =
    /// Public contract function exposed by this FS.Skia.UI package.
    val interpretKeyboardEffect: mapCommand: (CommandId -> 'msg) -> effect: KeyboardEffect -> AdapterCommand<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val interpretControlEffect: mapRuntime: (ControlRuntimeMsg -> 'msg) -> effect: ControlRuntimeEffect -> AdapterCommand<'msg>
    /// Lower a single pointer interaction (075) into adapter commands. Diagnostics
    /// lower to `ReportAdapterDiagnostic`; every other interaction is offered to the
    /// consumer router `mapInteraction` (a `None` result is a no-op `[]`). Mirrors
    /// `interpretKeyboardEffect`/`interpretControlEffect`; no new `AdapterEffect`
    /// case is required. FR-001/FR-010/FR-011.
    val interpretPointerEffect:
        mapInteraction: (PointerInteraction -> 'msg option) -> interaction: PointerInteraction -> AdapterCommand<'msg>
    /// Convenience: lower the `(PointerInteraction list, ControlRuntimeMsg list)`
    /// produced by `Pointer.update` in one call — runtime messages through
    /// `DispatchControlRuntimeMessage` (applied first to keep `ControlRuntime`
    /// state consistent), then interactions through `interpretPointerEffect`.
    val interpretPointerOutcome:
        mapInteraction: (PointerInteraction -> 'msg option) ->
        interactions: PointerInteraction list ->
        runtimeMessages: ControlRuntimeMsg list ->
            AdapterCommand<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val subscriptions: keyboard: AdapterSubscription<'msg> list -> controls: AdapterSubscription<'msg> list -> AdapterSubscription<'msg> list
    /// Public contract function exposed by this FS.Skia.UI package.
    val program:
        init: (unit -> 'model * AdapterCommand<'msg>) ->
        update: ('msg -> 'model -> 'model * AdapterCommand<'msg>) ->
        view: ('model -> Control<'msg>) ->
        subscriptions: ('model -> AdapterSubscription<'msg> list) ->
            AdapterProgram<'model, 'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostic: source: string -> code: string -> message: string -> AdapterDiagnostic
    /// Adapt a typed (`Widget<'msg>`-returning) view to the `Control<'msg>` view the
    /// program record expects (= `view >> Widget.toControl`). Lets typed authoring
    /// compose through the adapter with no boundary shim in product code. FR-001/FR-004.
    val widgetView: view: ('model -> Widget<'msg>) -> ('model -> Control<'msg>)
    /// Build a program whose view is authored with the typed front door (returns
    /// `Widget<'msg>`); the adapter lowers internally via `Widget.toControl`. Equivalent
    /// to `program init update (widgetView view) subscriptions`. FR-001/FR-004.
    val programOfWidget:
        init: (unit -> 'model * AdapterCommand<'msg>) ->
        update: ('msg -> 'model -> 'model * AdapterCommand<'msg>) ->
        view: ('model -> Widget<'msg>) ->
        subscriptions: ('model -> AdapterSubscription<'msg> list) ->
            AdapterProgram<'model, 'msg>

    /// The single pointer-routing step the interactive host performs per native pointer sample:
    /// renders `host.View size model` via `Control.renderTree host.Theme size`, hit-tests the
    /// laid-out bounds through the shipped 075 pipeline (`Pointer.update`, incl. the 4px click/drag
    /// fold), then routes each emitted interaction (feature 090, FR-001/FR-003): a hit control's
    /// authored `EventBindings` (`onClick`/`onChanged`) are dispatched — the authored control id is
    /// recovered via `Control.nearestAuthored` (so a click inside a container-keyed composite resolves
    /// to the authored container) and joined with `rendered.EventBindings` by `(ControlId, EventKind)`.
    /// An authored binding wins and consumes the interaction; `host.MapPointer` is the fallback,
    /// consulted ONLY for interactions no authored binding matched (no double-dispatch). A control with
    /// no authored binding behaves exactly as before (additive). Returns the advanced `PointerState`
    /// (threaded across samples) plus the product messages. `runInteractiveApp` wires exactly this;
    /// exposed so a headless test exercises the real adapter path without opening a window (research D6).
    val routeInteractivePointer:
        host: InteractiveAppHost<'model, 'msg> ->
        state: PointerState ->
        size: Size ->
        model: 'model ->
        input: ViewerPointerInput ->
            PointerState * 'msg list

    /// Feature 110 (FR-001/FR-002/FR-003): resolve a single already-resolved `PointerInteraction` from
    /// the RETAINED frame, performing NO `host.View` + `Control.renderTree` for routing. A binding-
    /// eligible `Click` hit-tests via `RetainedRender.retainedHitTest` over the retained frame's cached
    /// boxes, bridges that `RetainedId` to the authored `ControlId` via
    /// `RetainedRender.authoredControlIds`, and dispatches the retained frame's matching `EventBindings`
    /// (the same authored binding the full-render path fires, including a composite whose binding is
    /// authored above the hit node); every other interaction, and a `Click` with no matching binding,
    /// falls back to `host.MapPointer` exactly as the oracle does. Returns the product messages and a
    /// FALLBACK COUNT: when the retained frame cannot resolve a bindable hit (`retainedHitTest` `None`
    /// over a point a `Click` named) it falls back to one preserved full-render oracle resolution
    /// (`Control.renderTree` + `nearestAuthored`) and returns `1` (FR-007/FR-009); the normal path
    /// returns `0`. `internal` because it consumes the internal `RetainedRender`; the adapter tests
    /// reach it via `InternalsVisibleTo` (it IS the production routing path, SC-001/SC-002/SC-005).
    val internal routeRetainedInteraction:
        host: InteractiveAppHost<'model, 'msg> ->
        size: Size ->
        model: 'model ->
        retained: RetainedRender<'msg> ->
        render: ControlRenderResult<'msg> ->
        interaction: PointerInteraction ->
            'msg list * int

    /// Feature 110 (FR-001/FR-004/FR-006): the live retained pointer route. Maps the native sample with
    /// `Pointer.toMsg` and runs `Pointer.update` over the retained frame's already-evaluated CACHED
    /// `LayoutResult` (`retained.Layout`) — NOT a freshly evaluated layout — then resolves every emitted
    /// interaction through `routeRetainedInteraction`, summing the per-interaction fallback counts.
    /// Returns the advanced `PointerState` (threaded across samples), the product messages, and the
    /// frame's `FullRenderFallbackCount` (0 on every normal scenario). Dispatch-identical to the
    /// preserved `routeInteractivePointer` oracle by construction — same gesture fold over the same
    /// `LayoutResult`, same authored binding resolution, same `MapPointer` fallback (FR-006/FR-011).
    /// `internal` because it consumes the internal `RetainedRender`; tests reach it via
    /// `InternalsVisibleTo` and compare it directly against the oracle (SC-003).
    val internal routeRetainedPointer:
        host: InteractiveAppHost<'model, 'msg> ->
        retained: RetainedRender<'msg> ->
        render: ControlRenderResult<'msg> ->
        state: PointerState ->
        size: Size ->
        model: 'model ->
        input: ViewerPointerInput ->
            PointerState * 'msg list * int

    /// 092 (FR-004): resolve a point to the stable `RetainedId` of the control under it, via the
    /// retained tree's per-node boxes — replacing the 090 `ControlId` `hitTest |> nearestAuthored`
    /// path (which collapses unkeyed same-kind siblings onto one id). `None` for a true gap / outside
    /// the root. `internal` because it takes the internal `RetainedRender` structure; the adapter
    /// tests reach it via InternalsVisibleTo (it IS the production focus-resolution path, SC-002).
    val internal resolveFocus: retained: RetainedRender<'msg> -> x: float -> y: float -> RetainedId option

    /// 092 focus-aware text routing on the RETAINED structure (FR-005/FR-006), replacing the 090
    /// `ControlId`-keyed seam: deliver `msg` to the focused control's `RetainedId`-keyed `TextInput`
    /// state held in `retained.StateByIdentity[id].Text`, seeding from the control's current value +
    /// kind-derived line mode on FIRST focus (so the first keystroke appends to a pre-filled value),
    /// and return the next retained structure (whose carried text state survives a positional shift
    /// via `step`) plus ALL of the focused control's matched `onChanged` product messages — every
    /// binding, not just the first. When `focused` is `None`/names no live node, the structure is
    /// returned unchanged and no message is produced. `internal` because it takes the internal
    /// `RetainedRender`; the adapter tests drive it through InternalsVisibleTo (the real seam SC-001
    /// exercises, with no hand-seeded identity map). The 090 `ControlId`-keyed `routeFocusedText` is
    /// REPLACED (breaking within this package surface; covered by the recaptured baseline + migration
    /// note). Scope: routing seam only — caret/selection/IME-UX/undo and general focus/tab-traversal
    /// are trajectory item E4.
    val internal routeFocusedText:
        retained: RetainedRender<'msg> ->
        focused: RetainedId option ->
        msg: TextInputMsg ->
            RetainedRender<'msg> * 'msg list

    /// E4 (FR-003/FR-006/FR-007): route a delivered key to the current FocusedControl over the
    /// RETAINED tree, generalizing the 092 `routeFocusedText` text seam to all interactive kinds.
    /// Resolves the focused control via its stable `RetainedId` (E2 identity), reads its
    /// `KeyboardOperation`, and applies `Focus.route`:
    ///   - Activate  -> the focused control's authored activation `EventBindings` (the same message a
    ///                  pointer activation dispatches), matched by (ControlId, click-equivalent kind),
    ///                  fired ONCE (no double-dispatch);
    ///   - Navigate  -> the focused control's authored value-change/selection bindings (a slider/
    ///                  numeric control steps its `value` by the arrow direction and dispatches its
    ///                  `onChanged` bindings);
    ///   - Traverse  -> `Focus.traverse order (focused control's id) move`, emitting
    ///                  `ControlRuntimeMsg.FocusControl next`;
    ///   - Fallthrough -> no message (the host then consults `host.MapKey`).
    /// A focused TEXT control's printable keys are handled by the unchanged E1 `routeFocusedText`
    /// path BEFORE this is consulted (so text delivery is not regressed, SC-003). Returns the
    /// (unchanged) retained structure, the focus-update `ControlRuntime` messages, and the focused
    /// control's authored product messages. Total; never throws (an unmatched key -> no msgs).
    /// `internal` because it takes the internal `RetainedRender` structure; the adapter tests reach
    /// it via `InternalsVisibleTo` (it IS the production key-routing path, SC-002/SC-004, with no
    /// hand-seeded identity map).
    val internal routeFocusedKey:
        retained: RetainedRender<'msg> ->
        focused: RetainedId option ->
        order: TabOrder ->
        key: ViewerKey ->
        shift: bool ->
            RetainedRender<'msg> * ControlRuntimeMsg list * 'msg list

    /// Build a responds-proof verdict from a before/after frame pair (feature 090, FR-006):
    /// `Responsive` when the frames differ, `Inert` when identical. The reusable core the pointer and
    /// text responds-proof captures share.
    val respondsProofOf: before: Scene -> after: Scene -> RespondsProof

    /// Capture an input→visible-change responds-proof for a pointer interaction on the running host
    /// (feature 090, FR-006/FR-007): render the BEFORE frame, route the interaction through the real
    /// `routeInteractivePointer` adapter path, fold the produced messages with `host.Update`, render
    /// the AFTER frame, and emit both frames + a verdict. A host whose live window is inert (an
    /// authored binding dropped) yields identical frames and an `Inert` verdict — it cannot be passed
    /// off as a responds-proof. Reuses the production render path; no live Vulkan window required.
    val captureRespondsProof:
        host: InteractiveAppHost<'model, 'msg> ->
        state: PointerState ->
        size: Size ->
        model: 'model ->
        input: ViewerPointerInput ->
            RespondsProof

    /// Launch `host` as a durable, pointer-routing, size-aware window (feature 085). Each frame
    /// renders `host.View size model` through `Control.renderTree host.Theme size`; native pointer
    /// samples are hit-tested through `Pointer.update` (incl. the shipped 4px click/drag fold) and
    /// routed by `routeInteractivePointer` — a hit control's authored `EventBindings` are dispatched
    /// (authored binding wins; `host.MapPointer` is the fallback for unconsumed interactions, feature
    /// 090 FR-001/FR-003), and keystrokes are routed focus-first (feature 094 / E4): each native key
    /// is offered to the E1 `routeFocusedText` seam (a focused TEXT control's printable keys), then
    /// to `routeFocusedKey` (the general activation / navigation / Tab-traversal seam over the
    /// focused control's `KeyboardOperation` and the `Focus.order` tab order), and finally falls
    /// through to `host.MapKey` for any key no focused control and no traversal consumed. A pointer
    /// press sets focus to the focusable control under it (FR-006), so a later key reaches it; a
    /// press on a non-focusable region leaves focus unchanged. Reuses `Viewer.runInteractiveViewer`;
    /// the durable `Viewer.runApp` literal is untouched.
    ///
    /// Feature 091 (E2, behavioral note — signature unchanged): the host no longer rebuilds the
    /// whole tree every frame. It holds a retained previous tree (`module internal RetainedRender`,
    /// the wired 067 reconciler) and produces each frame by `Reconcile.diff`-ing the next tree
    /// against it and reusing the unchanged subtrees' cached render fragments — O(changed-subtree),
    /// byte-for-byte identical to a full rebuild (FR-004/FR-005). Per-control state re-keys to the
    /// stable diff-conferred identity so it survives an unrelated re-render (FR-003); diff
    /// diagnostics (e.g. `KeyCollision`) surface through the host diagnostics channel, never
    /// dropped (FR-007). The consumer `Init`/`Update`/`View`/`MapKey`/`MapPointer`/`Tick`/`Theme`/
    /// `Diagnostics` contract is unchanged — an existing consumer needs zero changes to benefit
    /// (FR-008).
    val runInteractiveApp:
        options: ViewerOptions -> host: InteractiveAppHost<'model, 'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>

    /// Feature 108 (US3, FR-009/010): the pure, headless, deterministic frame driver. Folds an
    /// ordered `FrameInput` script over the host's pure `Update` + `RetainedRender.step`, advancing
    /// one frame per step (consecutive pointer-MOVE inputs coalesce into a single frame) and
    /// accumulating the per-frame `FrameMetrics`. Shares the message→update→retained-step +
    /// clock-advance + coalescing code path with `runInteractiveApp` (no parallel logic), so a
    /// regression that un-coalesces moves or reintroduces a per-hover full rebuild fails the
    /// byte-stable count golden (SC-003/004/005) rather than shipping. The four count/bool fields are
    /// identical across repeated runs of the same script; `FrameDuration` is not asserted.
    module Perf =
        /// Fold an ordered `FrameInput` script over the host's pure `Update` + `RetainedRender.step`,
        /// returning the per-frame `FrameMetrics` (consecutive pointer-MOVE inputs coalesce into one
        /// frame). Pure, headless, byte-stable in its count/bool fields (SC-003/004/005).
        val runScript:
            host: InteractiveAppHost<'model, 'msg> ->
            size: Size ->
            script: FrameInput<'msg> list ->
                FrameMetrics list

```
