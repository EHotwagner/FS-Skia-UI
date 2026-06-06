module ControlsAccessibilityTests

open Expecto
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

[<Tests>]
let typedGalleryAccessibilityTests =
    testList "Feature 071 typed gallery accessibility (US2)" [
        // SC-005 / contract G6: the typed-authored gallery panel renders through the
        // existing path at >=2 viewports and exposes the expected accessibility roles
        // for its mechanic-group representatives.
        test "typed gallery panel exposes expected accessibility roles at two viewports (SC-005, G6)" {
            let panel = ControlsTypedGalleryPanel.panel

            for width, height in [ 320, 240; 1024, 768 ] do
                let rendered = Control.render Theme.light panel
                let evidence = Scene.renderReadbackEvidence { Width = width; Height = height } rendered.Scene
                Expect.isEmpty rendered.Diagnostics $"typed gallery panel has no diagnostics at {width}x{height}"
                Expect.isNonEmpty evidence.DeterministicHash $"typed gallery panel render evidence has a deterministic hash at {width}x{height}"

            let kinds = ControlsTypedGalleryPanel.kindsPresent panel
            let roleOf kind = (Accessibility.defaultFor kind "typed").Role

            [ "button", AccessibilityRole.Button
              "text-area", AccessibilityRole.TextBox
              "check-box", AccessibilityRole.CheckBox
              "list-box", AccessibilityRole.List
              "tabs", AccessibilityRole.Tab
              "line-chart", AccessibilityRole.Chart
              "graph-view", AccessibilityRole.Graph ]
            |> List.iter (fun (kind, role) ->
                Expect.isTrue (Set.contains kind kinds) $"typed gallery panel includes a typed {kind}"
                Expect.equal (roleOf kind) role $"{kind} exposes the {role} accessibility role")
        }
    ]

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
