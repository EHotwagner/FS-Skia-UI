// Contract sketch for feature 068 — additive public surface of FS.Skia.UI.Controls.Elmish.
// This is the signature the failing-first tests pin and the implementation must satisfy.
// EXISTING declarations are shown for context and are UNCHANGED (FR-002); only the items
// marked `// NEW (068)` are added. Final naming is fixed here; the real edit lands in
// src/Controls.Elmish/ControlsElmish.fsi.

namespace FS.Skia.UI.Controls.Elmish

open FS.Skia.UI.Controls          // Control<'msg>, Widget<'msg>, Widget.toControl
open FS.Skia.UI.KeyboardInput
open Elmish                        // NEW (068) — names Cmd<'msg> from the already-referenced Fable.Elmish

// ---- EXISTING (unchanged) ---------------------------------------------------------------
type AdapterDiagnostic =
    { Code: string; Message: string; Source: string }

type AdapterEffect<'msg> =
    | DispatchProductMessage of 'msg
    | DispatchControlRuntimeMessage of ControlRuntimeMsg
    | DispatchKeyboardMessage of KeyboardMsg
    | DispatchHostCommand of string
    | ReportAdapterDiagnostic of AdapterDiagnostic

type AdapterCommand<'msg> = AdapterEffect<'msg> list

type AdapterSubscription<'msg> =
    { Id: string; Subscribe: unit -> AdapterCommand<'msg> }

type AdapterProgram<'model, 'msg> =
    { Init: unit -> 'model * AdapterCommand<'msg>
      Update: 'msg -> 'model -> 'model * AdapterCommand<'msg>
      View: 'model -> Control<'msg>            // unchanged — still Control<'msg>
      Subscriptions: 'model -> AdapterSubscription<'msg> list }

// ---- NEW (068): AdapterCommand <-> Elmish Cmd<'msg> bridge -------------------------------
/// Pure, total bridge between the adapter's effect-list command model and Elmish `Cmd<'msg>`.
module AdapterCmd =
    /// The Elmish no-op command (= `Cmd.none`). `toCmd route [] = none`.
    val none: Cmd<'msg>
    /// Lift a single product message into an `AdapterCommand` (`[ DispatchProductMessage msg ]`).
    val ofMessage: msg: 'msg -> AdapterCommand<'msg>
    /// The ordered `DispatchProductMessage` payloads carried by the command (round-trip oracle).
    val productMessages: command: AdapterCommand<'msg> -> 'msg list
    /// Total conversion to an Elmish `Cmd<'msg>`: `route` maps EVERY `AdapterEffect` case
    /// (product and non-product) to a `'msg`, preserving order; `[]` -> `Cmd.none`. FR-003/FR-008.
    val toCmd: route: (AdapterEffect<'msg> -> 'msg) -> command: AdapterCommand<'msg> -> Cmd<'msg>

// ---- EXISTING module (unchanged) + NEW (068) Widget-view entry points --------------------
module ControlsElmish =
    // EXISTING (unchanged):
    val interpretKeyboardEffect: mapCommand: (CommandId -> 'msg) -> effect: KeyboardEffect -> AdapterCommand<'msg>
    val interpretControlEffect: mapRuntime: (ControlRuntimeMsg -> 'msg) -> effect: ControlRuntimeEffect -> AdapterCommand<'msg>
    val subscriptions: keyboard: AdapterSubscription<'msg> list -> controls: AdapterSubscription<'msg> list -> AdapterSubscription<'msg> list
    val program:
        init: (unit -> 'model * AdapterCommand<'msg>) ->
        update: ('msg -> 'model -> 'model * AdapterCommand<'msg>) ->
        view: ('model -> Control<'msg>) ->
        subscriptions: ('model -> AdapterSubscription<'msg> list) ->
            AdapterProgram<'model, 'msg>
    val diagnostic: source: string -> code: string -> message: string -> AdapterDiagnostic

    // NEW (068):
    /// Adapt a typed (`Widget<'msg>`-returning) view to the `Control<'msg>` view the program
    /// record expects (= `view >> Widget.toControl`). FR-001.
    val widgetView: view: ('model -> Widget<'msg>) -> ('model -> Control<'msg>)
    /// Build a program whose view is authored with the typed front door (returns `Widget<'msg>`);
    /// the adapter lowers internally via `Widget.toControl`, so product code needs no shim.
    /// Equivalent to `program init update (widgetView view) subscriptions`. FR-001/FR-004.
    val programOfWidget:
        init: (unit -> 'model * AdapterCommand<'msg>) ->
        update: ('msg -> 'model -> 'model * AdapterCommand<'msg>) ->
        view: ('model -> Widget<'msg>) ->
        subscriptions: ('model -> AdapterSubscription<'msg> list) ->
            AdapterProgram<'model, 'msg>
