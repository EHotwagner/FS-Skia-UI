#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"
#r "../src/Controls.Elmish/bin/Debug/net10.0/FS.Skia.UI.Controls.Elmish.dll"

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
