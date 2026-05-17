module ControlsBoundaryGeneratedGuidanceTests

open System
open Expecto
open GovernanceTestSupport

let controlsReadme = "template/fragments/controls/README.md"
let controlsSkill = "template/fragments/controls/skill/SKILL.md"
let elmishReadme = "template/fragments/elmish/README.md"
let capabilityCatalog = "template/capabilities.yml"
let productReadme = "template/base/README.md"

let generatedGuidanceText paths =
    paths
    |> List.map read
    |> String.concat Environment.NewLine

let expectNoStaleTerm context (content: string) (forbidden: string list) =
    forbidden
    |> List.iter (fun stale ->
        Expect.isFalse (content.Contains(stale, StringComparison.OrdinalIgnoreCase)) $"{context} omits stale term {stale}")

[<Tests>]
let controlsBoundaryGeneratedGuidanceTests =
    testList "Controls boundary generated guidance" [
        test "controls fragment describes one Skia Elmish Controls path" {
            let readme = read controlsReadme
            let skill = read controlsSkill
            let combined = readme + Environment.NewLine + skill

            [ "Skia-rendered"
              "Elmish-style"
              "rich text"
              "DataGrid"
              "Control<'msg>"
              "product-owned" ]
            |> List.iter (fun required ->
                Expect.stringContains combined required $"controls generated guidance contains {required}")

            expectNoStaleTerm
                "controls guidance"
                combined
                [ "renderer-neutral"
                  "renderer neutral"
                  "FS.Skia.UI.Charts"
                  "fs-skia-charts"
                  "chart-only"
                  "DataGrid as chart" ]
        }

        test "generated guidance includes generic message flow and adapter path when Elmish is selected" {
            expectFileContains controlsSkill [ "Control<'msg>"; "message"; "adapter" ]
            expectFileContains elmishReadme [ "FS.Skia.UI.Controls.Elmish"; "commands"; "subscriptions" ]
        }

        test "controls generated skill shows generic product message examples" {
            expectFileContains
                controlsSkill
                [ "type Msg"
                  "Control<'msg>"
                  "TextBox.onChanged"
                  "Button.onClick"
                  "LineChart.create"
                  "DataGrid.create"
                  "DataGrid.rows"
                  "product-owned" ]
        }

        test "Elmish fragment shows Controls adapter program wiring when selected" {
            expectFileContains
                elmishReadme
                [ "FS.Skia.UI.Controls.Elmish"
                  "ControlsElmish.program"
                  "ControlsElmish.interpretKeyboardEffect"
                  "ControlsElmish.interpretControlEffect"
                  "AdapterCommand<'msg>"
                  "AdapterSubscription<'msg>"
                  "subscriptions"
                  "commands" ]
        }

        test "generated guidance rejects stale chart capability wording and host-loop ownership" {
            let combined =
                generatedGuidanceText
                    [ controlsReadme
                      controlsSkill
                      elmishReadme
                      capabilityCatalog
                      productReadme ]

            expectNoStaleTerm
                "generated guidance"
                combined
                [ "FS.Skia.UI.Charts"
                  "fs-skia-charts"
                  "charts capability"
                  "chart-only"
                  "DataGrid as chart"
                  "DataGrid-as-chart"
                  "renderer-neutral"
                  "renderer neutral"
                  "own the host loop"
                  "host-loop ownership"
                  "host loop ownership" ]
        }

        test "controls capability is the active generated home for chart graph and DataGrid guidance" {
            let capabilities = readCatalogCapabilities ()
            let ids = capabilities |> List.map _.Id |> Set.ofList

            Expect.isFalse (ids.Contains "charts") "generated capability catalog has no active charts capability"

            let controls = capabilities |> List.find (fun capability -> capability.Id = "controls")
            let ownerNotes = defaultArg controls.OwnerNotes ""

            Expect.equal controls.PackageId (Some "FS.Skia.UI.Controls") "Controls is the generated controls package"
            Expect.stringContains ownerNotes "chart controls" "Controls capability owns chart guidance"
            Expect.stringContains ownerNotes "graph controls" "Controls capability owns graph guidance"
            Expect.stringContains ownerNotes "DataGrid" "Controls capability owns DataGrid guidance"
        }

        test "controls guidance documents the removed Charts replacement path" {
            let combined = generatedGuidanceText [ controlsReadme; controlsSkill ]

            [ "Charts migration"
              "legacy Charts package"
              "FS.Skia.UI.Controls"
              "LineChart"
              "BarChart"
              "PieChart"
              "ScatterPlot"
              "DataGrid"
              "no compatibility shim" ]
            |> List.iter (fun required ->
                Expect.stringContains combined required $"controls generated guidance documents {required}")
        }
    ]
