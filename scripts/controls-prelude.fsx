#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open FS.Skia.UI
open FS.Skia.UI.Controls

type Msg =
    | Save
    | NameChanged of string

let view name canSave =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Controls FSI" ]
            TextBox.create [
                TextBox.value name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled canSave
                Button.onClick Save
            ]
        ]
    ]

let root = view "Ada" true
let rendered = Control.render Theme.light root
let changed =
    { Kind = "changed"
      ControlId = Some "text-box"
      Origin = Text
      Payload = Some "Grace" }

printfn "controls-node-count=%d" rendered.NodeCount
printfn "controls-diagnostics=%A" rendered.Diagnostics
printfn "controls-scene=%A" (Scene.describe rendered.Scene)
printfn "controls-catalog-count=%d" (Catalog.supportedCount ())
printfn "controls-text-dispatch=%A" (Control.dispatch changed root)
