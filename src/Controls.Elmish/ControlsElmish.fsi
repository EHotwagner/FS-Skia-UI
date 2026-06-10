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

/// Pointer-routing, size-aware durable host (feature 085, research D3-AMEND). Mirrors
/// `GeneratedAppHost` field-for-field PLUS a `MapPointer` seam over `PointerInteraction` and a
/// size-carrying `View` that returns a `Control<'msg>` tree (so `Control.renderTree` yields the
/// `Scene` + `Layout` + `EventBindings` the host routes). Lives in Controls.Elmish — not SkiaViewer —
/// because `PointerInteraction`/`interpretPointerOutcome` are Controls surface and the viewer is
/// host-independent. `Theme` drives `renderTree`. Feature 090: a hit control's authored
/// `EventBindings` (`onClick`/`onChanged`) are dispatched in the live window; `MapKey` gains a
/// focus-aware text-routing seam for the focused text control (see `routeInteractivePointer`,
/// `routeFocusedText`, and `runInteractiveApp`).
type InteractiveAppHost<'model, 'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> Control<'msg>
      Theme: Theme
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option
      Tick: TimeSpan -> 'msg option
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

    /// Focus-aware text-routing seam (feature 090, FR-008): deliver a `TextInputMsg` (a keystroke /
    /// committed or composed text) to the currently `focused` text control's existing `TextInput`
    /// model and fold that control's authored `onChanged` binding into product messages — so
    /// TextBox/TextArea/NumericInput are typeable in `runInteractiveApp`. Only the focused control's
    /// model advances (`models` holds one `TextInputModel` per text control, keyed by `ControlId`); an
    /// unfocused control's model is returned unchanged. Reuses `ControlRuntime.FocusedControl` +
    /// `TextInput.update` — no parallel text model. When `focused` is `None`/names no model, the models
    /// are returned unchanged and no product message is produced (the host's unchanged `MapKey` path
    /// handles the key). Scope: routing seam only — caret/selection/IME-UX/undo and general
    /// focus/tab-traversal across all control kinds are trajectory item E4 (FR-008a).
    val routeFocusedText:
        rendered: ControlRenderResult<'msg> ->
        focused: ControlId option ->
        models: Map<ControlId, TextInputModel> ->
        msg: TextInputMsg ->
            Map<ControlId, TextInputModel> * 'msg list

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
    /// 090 FR-001/FR-003), and keystrokes to a focused text control are delivered through the
    /// focus-aware text seam (`routeFocusedText`, FR-008) before falling through to `host.MapKey`.
    /// Reuses `Viewer.runInteractiveViewer`; the durable `Viewer.runApp` literal is untouched.
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
