# FS.Skia.UI.Controls.Elmish Source-Shaped API Reference

package-id: FS.Skia.UI.Controls.Elmish
package-version: local
generated-from: curated-fsi
assembly-reflection: false
repository-source-authoring-fallback: false
symbol-count: 88
xml-summary-count: 137
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

```
