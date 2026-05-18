module ProductTests

open Expecto
open Product.Program
open FS.Skia.UI.Controls
open FS.Skia.UI.KeyboardInput

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }

        test "product-owned controls example is wired" {
            let view = controlsExampleView initialModel
            Expect.isGreaterThan (Control.count view) 7 "product example owns form, rich text, chart, graph, and DataGrid controls"
        }

        test "product-owned form chart and DataGrid controls are constructible" {
            let textBox =
                TextBox.create [
                    TextBox.value initialModel.Name
                    TextBox.onChanged NameChanged
                ]

            let lineChart = LineChart.create [ LineChart.series initialModel.Revenue ]
            let dataGrid = DataGrid.create initialModel.GridColumns [ DataGrid.rows initialModel.GridRows ]

            Expect.isGreaterThan (Control.count textBox) 0 "TextBox product example is constructible"
            Expect.isGreaterThan (Control.count lineChart) 0 "LineChart product example is constructible"
            Expect.isGreaterThan (Control.count dataGrid) 0 "DataGrid product example is constructible"
        }

        test "generated product adapter program is product-owned" {
            let model, initCommands = adapterProgram.Init()
            let updated, saveCommands = adapterProgram.Update SaveRequested model
            let view = adapterProgram.View updated
            let subscriptions = adapterProgram.Subscriptions updated

            Expect.isEmpty initCommands "adapter init starts without host commands"
            Expect.isNonEmpty saveCommands "save emits product-owned adapter command"
            Expect.isEmpty subscriptions "default generated product has no subscriptions"
            Expect.isGreaterThan (Control.count view) 7 "adapter view returns Controls"
        }

        test "generated graphical app starts through viewer key event" {
            let started, _ =
                dispatchViewerKey { RawKey = "Enter"; Direction = ViewerKeyDirection.KeyDown } initialModel

            Expect.equal started.Screen Main "initial screen starts from viewer Enter"
            Expect.equal started.LastInput (Some Enter) "normalized input is stored"
            Expect.exists started.InputDiagnostics (fun item -> item.Flow = "initial-start" && item.RawKey = Some "Enter") "diagnostic names the viewer input flow"
        }

        test "generated graphical app options pause back and restart flows use viewer keys" {
            let options, _ =
                dispatchViewerKey { RawKey = "O"; Direction = ViewerKeyDirection.KeyDown } initialModel

            let main, _ =
                dispatchViewerKey { RawKey = "Return"; Direction = ViewerKeyDirection.KeyDown } options

            let paused, _ =
                dispatchViewerKey { RawKey = "Space"; Direction = ViewerKeyDirection.KeyDown } main

            let resumed, _ =
                dispatchViewerKey { RawKey = "Esc"; Direction = ViewerKeyDirection.KeyDown } paused

            let ended, _ = update EndReached resumed

            let restarted, _ =
                dispatchViewerKey { RawKey = "Enter"; Direction = ViewerKeyDirection.KeyDown } ended

            Expect.equal options.Screen Options "options screen opens through viewer key"
            Expect.equal main.Screen Main "options selection enters main screen"
            Expect.equal paused.Screen Paused "space pauses main interaction"
            Expect.equal resumed.Screen Main "escape resumes from pause"
            Expect.equal restarted.Screen Initial "end screen restarts through viewer Enter"
        }

        test "pure generated app transitions expose model message and effect behavior" {
            let started, startEffects = update (ViewerInput(Enter, true)) initialModel
            let interacted, interactionEffects = update (ViewerInput(ArrowLeft, true)) started

            Expect.equal started.Screen Main "pure update starts app"
            Expect.isEmpty startEffects "input transition has no host command"
            Expect.equal interacted.PrimaryInteractions 1 "primary interaction is counted"
            Expect.isEmpty interactionEffects "primary interaction has no host command"
        }

        test "generated graphical app exposes bounded smoke command" {
            let source = System.IO.File.ReadAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Product", "Program.fs"))

            Expect.stringContains source "--bounded-smoke" "generated product exposes bounded smoke CLI"
            Expect.stringContains source "--bounded-smoke-frame-diagnostics" "generated product exposes explicit frame diagnostic smoke CLI"
            Expect.stringContains source "Viewer.runBounded" "bounded smoke uses the public SkiaViewer bounded run entry point"
            Expect.stringContains source "status=unsupported" "bounded smoke reports unsupported host conditions explicitly"
            Expect.stringContains source "diagnostic-mode={diagnosticMode}" "generated smoke writes readable diagnostics mode"
            Expect.stringContains source "startup-focused" "startup-focused generated smoke is the default"
            Expect.stringContains source "frame-focused" "frame-focused generated smoke is opt-in"
            Expect.stringContains source "FrameLogLimit = if includeFrameDiagnostics then Some 1 else Some 0" "generated smoke limits repeated frame diagnostics"
        }

        test "generated graphical app exposes deterministic scene evidence command" {
            let source = System.IO.File.ReadAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Product", "Program.fs"))

            Expect.stringContains source "--scene-evidence" "generated product exposes non-window scene evidence CLI"
            Expect.stringContains source "SceneEvidence.render" "scene evidence uses public Scene evidence helper"
            Expect.stringContains source "RendererMode = \"deterministic-scene\"" "scene evidence remains separate from live viewer startup"
            Expect.stringContains source "readiness/headless-scene-evidence.txt" "scene evidence writes a stable readiness path"
        }
    ]
