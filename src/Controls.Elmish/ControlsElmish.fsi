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
/// `Scene` + `Layout` + `EventBindings` the host hit-tests by `ControlId`). Lives in
/// Controls.Elmish — not SkiaViewer — because `PointerInteraction`/`interpretPointerOutcome` are
/// Controls surface and the viewer is host-independent. `Theme` drives `renderTree`.
type InteractiveAppHost<'model, 'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> Control<'msg>
      Theme: Theme
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

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
    /// fold), and routes the emitted interactions through `interpretPointerOutcome host.MapPointer`
    /// to product messages. Returns the advanced `PointerState` (threaded across samples) plus the
    /// product messages. `runInteractiveApp` wires exactly this; exposed so a headless test
    /// exercises the real adapter path without opening a window (research D6). FR-004/FR-005.
    val routeInteractivePointer:
        host: InteractiveAppHost<'model, 'msg> ->
        state: PointerState ->
        size: Size ->
        model: 'model ->
        input: ViewerPointerInput ->
            PointerState * 'msg list

    /// Launch `host` as a durable, pointer-routing, size-aware window (feature 085). Each frame
    /// renders `host.View size model` through `Control.renderTree host.Theme size`; native pointer
    /// samples are hit-tested (`Layout.hitTestComputed` × `EventBindings` by `ControlId`, with the
    /// shipped 4px click/drag fold via `Pointer.update`) and routed through
    /// `interpretPointerOutcome host.MapPointer` to product messages folded by `host.Update`.
    /// Reuses `Viewer.runInteractiveViewer`; the durable `Viewer.runApp` literal is untouched
    /// (FR-004/FR-005/FR-006/FR-009).
    val runInteractiveApp:
        options: ViewerOptions -> host: InteractiveAppHost<'model, 'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>
