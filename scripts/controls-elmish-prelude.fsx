#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"
#r "../src/Controls.Elmish/bin/Debug/net10.0/FS.Skia.UI.Controls.Elmish.dll"
#r "nuget: Fable.Elmish, 4.2.0"

open Elmish
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput

type Msg =
    | Save
    | Runtime of ControlRuntimeMsg

let view _ =
    Button.create [ Button.text "Save"; Button.onClick Save ]

let program =
    ControlsElmish.program
        (fun () -> 0, [])
        (fun _ model -> model, [])
        view
        (fun _ -> [])

let command =
    ControlsElmish.interpretKeyboardEffect (fun _ -> Save) (CommandResolved "save")

let controlCommand =
    ControlsElmish.interpretControlEffect Runtime (FocusChanged(Some "save-button"))

printfn "controls-elmish-prelude view=%s commandCount=%d controlCommandCount=%d" (program.View 0).Kind command.Length controlCommand.Length

// 075 — pointer outcome lowering: a primary Click routes to a product message, the
// accompanying ControlRuntime messages lower to DispatchControlRuntimeMessage, and a
// diagnostic lowers to ReportAdapterDiagnostic — no new AdapterEffect case required.
let routePointer: PointerInteraction -> Msg option =
    function
    | Click(_, PointerButton.Primary, _, _) -> Some Save
    | _ -> None

let pointerInteractions =
    [ PressedDown("save-button", PointerButton.Primary, 5.0, 5.0)
      Click("save-button", PointerButton.Primary, 5.0, 5.0) ]

let pointerRuntimeMsgs: ControlRuntimeMsg list = [ PressControl "save-button"; ReleaseControl "save-button" ]
let pointerCommand = ControlsElmish.interpretPointerOutcome routePointer pointerInteractions pointerRuntimeMsgs
let pointerDiagnosticCommand =
    ControlsElmish.interpretPointerEffect routePointer (Diagnostic { Code = HitTestMiss; Message = "miss"; Control = None; X = 0.0; Y = 0.0 })

printfn
    "controls-elmish-075 pointerCommandCount=%d productMsgs=%A diagnosticCount=%d"
    pointerCommand.Length
    (AdapterCmd.productMessages pointerCommand)
    pointerDiagnosticCommand.Length

// 068 — Widget-returning view path: typed authoring composes through the adapter with no
// Widget.toControl shim in this script; the adapter lowers internally via programOfWidget.
let widgetProgram =
    ControlsElmish.programOfWidget
        (fun () -> 0, [])
        (fun _ model -> model, [])
        (fun _ ->
            FS.Skia.UI.Controls.Typed.Button.view
                { FS.Skia.UI.Controls.Typed.Button.defaults with
                    Text = "Save"
                    OnClick = Some Save })
        (fun _ -> [])

// 068 — AdapterCommand <-> Elmish Cmd<'msg> bridge.
let exampleCommand: AdapterCommand<Msg> = [ DispatchProductMessage Save ]
let bridged: Cmd<Msg> = AdapterCmd.toCmd (function DispatchProductMessage m -> m | _ -> Save) exampleCommand
let productMsgs = AdapterCmd.productMessages exampleCommand

printfn
    "controls-elmish-068 widgetView=%s cmdCount=%d productMsgs=%d empty=%b"
    (widgetProgram.View 0).Kind
    bridged.Length
    productMsgs.Length
    (List.isEmpty (AdapterCmd.toCmd (fun _ -> Save) []))
