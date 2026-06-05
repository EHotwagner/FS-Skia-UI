module ControlsInteractionTests

open Expecto
open FS.Skia.UI.Controls

type Msg =
    | Save of int
    | Changed of string

let click id =
    { Kind = "click"; ControlId = Some id; Origin = ControlEventOrigin.Pointer; Payload = None }

[<Tests>]
let interactionTests =
    testList "Controls interaction dispatch" [
        test "pointer activation dispatches exactly one current-view message" {
            let view value =
                Button.create [
                    Button.text "Save"
                    Button.onClick (Save value)
                ]
                |> Control.withKey "save-button"

            Expect.equal (Control.dispatch (click "save-button") (view 1)) [ Save 1 ] "first view dispatches first model value"
            Expect.equal (Control.dispatch (click "save-button") (view 2)) [ Save 2 ] "re-rendered view dispatches current model value"
        }

        test "disabled and read-only controls suppress disallowed dispatch" {
            let disabled =
                Button.create [
                    Button.text "Save"
                    Button.enabled false
                    Button.onClick (Save 1)
                ]
                |> Control.withKey "save-button"

            let readOnly =
                TextBox.create [
                    TextBox.value "Ada"
                    TextBox.readOnly true
                    TextBox.onChanged Changed
                ]
                |> Control.withKey "name"

            Expect.equal (Control.dispatch (click "save-button") disabled) [] "disabled button suppresses click"

            let changed =
                { Kind = "changed"; ControlId = Some "name"; Origin = ControlEventOrigin.Text; Payload = Some "Grace" }

            Expect.equal (Control.dispatch changed readOnly) [] "read-only text box suppresses change"
        }

        test "keyboard activation uses the same message-oriented event path" {
            let button =
                Button.create [ Button.text "Save"; Button.onClick (Save 7) ]
                |> Control.withKey "save-button"

            let key =
                { Kind = "click"; ControlId = Some "save-button"; Origin = ControlEventOrigin.Keyboard; Payload = Some "Enter" }

            Expect.equal (Control.dispatch key button) [ Save 7 ] "keyboard activation dispatches through current event binding"
        }
    ]

[<Tests>]
let typedInteractionTests =
    testList "Typed controls interaction dispatch" [
        test "typed Button OnClick dispatches the same message as legacy onClick" {
            let typed =
                FS.Skia.UI.Controls.Typed.Button.view
                    { FS.Skia.UI.Controls.Typed.Button.defaults with
                        Id = Some "save-button"
                        Text = "Save"
                        OnClick = Some(Save 1) }
                |> Widget.toControl

            Expect.equal (Control.dispatch (click "save-button") typed) [ Save 1 ] "typed Button binds identically to Button.onClick"
        }

        test "typed Button with disabled state suppresses dispatch" {
            let disabled =
                FS.Skia.UI.Controls.Typed.Button.view
                    { FS.Skia.UI.Controls.Typed.Button.defaults with
                        Id = Some "save-button"
                        Enabled = false
                        OnClick = Some(Save 1) }
                |> Widget.toControl

            Expect.equal (Control.dispatch (click "save-button") disabled) [] "typed disabled button suppresses click"
        }

        test "typed Button without OnClick dispatches nothing" {
            let noHandler =
                FS.Skia.UI.Controls.Typed.Button.view
                    { FS.Skia.UI.Controls.Typed.Button.defaults with
                        Id = Some "save-button"
                        Text = "Save" }
                |> Widget.toControl

            Expect.equal (Control.dispatch (click "save-button") noHandler) [] "unset OnClick yields no dispatch"
        }

        test "typed CheckBox OnChanged maps payload identically to legacy onChanged" {
            let typed =
                FS.Skia.UI.Controls.Typed.CheckBox.view
                    { FS.Skia.UI.Controls.Typed.CheckBox.defaults with
                        Id = Some "agree"
                        OnChanged = Some(fun isChecked -> Changed(if isChecked then "on" else "off")) }
                |> Widget.toControl

            let changed =
                { Kind = "changed"; ControlId = Some "agree"; Origin = ControlEventOrigin.Text; Payload = Some "true" }

            Expect.equal (Control.dispatch changed typed) [ Changed "on" ] "typed CheckBox maps the boolean payload identically"
        }
    ]
