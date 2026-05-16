module ControlsSemanticTests

open Expecto
open FS.Skia.UI
open FS.Skia.UI.Controls

type FormModel =
    { Count: int
      Name: string
      CanSave: bool
      Validation: ValidationState }

type FormMsg =
    | Increment
    | NameChanged of string
    | SaveRequested

let view model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text $"Count: {model.Count}" ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.validation model.Validation
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            LineChart.create [
                LineChart.series [
                    { Name = "trend"
                      Points = [ { X = 0.0; Y = float model.Count; Label = Some "count" } ] }
                ]
            ]
        ]
    ]

[<Tests>]
let semanticTests =
    testList "Controls semantic behavior" [
        test "representative view function renders through public Control surface" {
            let model = { Count = 2; Name = "Ada"; CanSave = true; Validation = Valid }
            let control = view model
            let rendered = Control.render Theme.light control

            Expect.equal rendered.NodeCount 5 "root and children are counted"
            Expect.isEmpty rendered.Diagnostics "representative screen has no diagnostics"
            Expect.contains (Scene.describe rendered.Scene) TextRunElement "render produces fitted text scene output"
            Expect.equal rendered.Layout.Id "stack" "layout node is produced at the public boundary"
        }

        test "model-owned state changes are reflected by re-evaluating the view" {
            let first = view { Count = 1; Name = "Ada"; CanSave = true; Validation = Valid }
            let second = view { Count = 3; Name = "Grace"; CanSave = false; Validation = Invalid "required" }

            Expect.notEqual first.Children[0].Content second.Children[0].Content "control values are model-owned descriptions"
            Expect.isTrue (second.Children[1].Content = Some "Grace") "text box value reflects the current model"
            Expect.isTrue (second.Children[2].Attributes |> List.exists (fun attr -> match attr.Name, attr.Value with "enabled", BoolValue false -> true | _ -> false)) "enabled state reflects current model"
        }
    ]
