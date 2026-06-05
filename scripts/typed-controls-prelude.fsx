#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

type Msg =
    | Save
    | NameChanged of string
    | Toggled of bool

// Author a representative widget tree through the typed front door and finish
// with Widget.toControl (the lowering seam consumed by render + the adapter).
let view: Widget<Msg> =
    Stack.view
        { Stack.defaults with
            Orientation = Vertical
            Spacing = 8.0
            Children =
                [ TextBlock.view { TextBlock.defaults with Text = "Sign in" }
                  Button.view
                      { Button.defaults with
                          Id = Some "submit"
                          Text = "Submit"
                          Intent = Primary
                          OnClick = Some Save }
                  CheckBox.view
                      { CheckBox.defaults with
                          Text = "Remember me"
                          Checked = true
                          OnChanged = Some Toggled } ] }

let control = Widget.toControl view
let rendered = Widget.render Theme.light view

// Stateful typed control: init/update delegate to the existing pure TextInput model.
let textBoxProps: TextBoxProps<Msg> = { TextBox.defaults "email" with Value = "" }
let textBoxModel, textBoxEffects = TextBox.init textBoxProps
let textBoxModel', _ = TextBox.update (InsertText "a@b") textBoxModel
let textBox = TextBox.view textBoxProps textBoxModel'

printfn "typed-root-kind=%s" control.Kind
printfn "typed-children-kinds=%A" (control.Children |> List.map (fun c -> c.Kind))
printfn "typed-node-count=%d" rendered.NodeCount
printfn "typed-diagnostics=%A" rendered.Diagnostics
printfn "typed-button-dispatch=%A" (Control.dispatch { Kind = "click"; ControlId = Some "submit"; Origin = ControlEventOrigin.Pointer; Payload = None } control)
printfn "typed-textbox-init-effects=%A" textBoxEffects
printfn "typed-textbox-draft=%s" textBoxModel'.DraftText
printfn "typed-textbox-kind=%s" (Widget.toControl textBox).Kind
printfn "typed-ofcontrol-roundtrip-kind=%s" (Widget.toControl (Widget.ofControl control)).Kind
