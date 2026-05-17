module ControlsAccessibilityTests

open Expecto
open FS.Skia.UI.Controls

[<Tests>]
let accessibilityTests =
    testList "Controls accessibility metadata" [
        test "interactive controls expose role name state focus keyboard and contrast metadata" {
            let metadata = Accessibility.defaultFor "button" "Save"
            Expect.equal metadata.Role AccessibilityRole.Button "button role is declared"
            Expect.equal metadata.NameSource "Save" "name source is available"
            Expect.isNonEmpty metadata.State "state metadata is present"
            Expect.isTrue metadata.Keyboard.Focusable "button is focusable"
            Expect.contains metadata.Keyboard.ActivationKeys "Enter" "keyboard operation is documented"
            Expect.isSome metadata.Contrast "contrast evidence is present"
        }

        test "missing accessibility and low contrast fail diagnostics" {
            let lowContrast =
                Accessibility.metadata AccessibilityRole.Button "Low" [ "normal" ] (Some 1) (Accessibility.keyboard true [ "Enter" ] [ "Tab" ]) (Some(Accessibility.contrast FS.Skia.UI.Scene.Colors.black FS.Skia.UI.Scene.Colors.black 1.0 4.5))

            let control = Button.create [ Button.text "Low"; Attr.accessibility lowContrast ]
            let diagnostics = Accessibility.validate control
            Expect.exists diagnostics (fun item -> item.Code = ContrastFailure) "contrast failure is reported"
        }
    ]
