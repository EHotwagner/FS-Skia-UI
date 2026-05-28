module ProductTests

open System
open Expecto
open Product.Program
open Product.Model
open FS.Skia.UI.Scene

let rec collectSceneNodes node =
    seq {
        yield node
        match node with
        | Group scenes ->
            for scene in scenes do
                for child in scene.Nodes do
                    yield! collectSceneNodes child
        | ClipNode(_, scene)
        | ColorSpaceNode(_, scene)
        | PerspectiveNode(_, scene) ->
            for child in scene.Nodes do
                yield! collectSceneNodes child
        | PictureNode picture ->
            for child in picture.Scene.Nodes do
                yield! collectSceneNodes child
        | _ -> ()
    }

let sceneText node =
    collectSceneNodes node
    |> Seq.choose (function Text(_, value, _) -> Some value | TextRun run -> Some run.Text | _ -> None)
    |> String.concat " "

let productSource file =
    System.IO.File.ReadAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Product", file))

let productSources files =
    files |> List.map productSource |> String.concat "\n"

//#if (profile == "governed" || profile == "headless-scene")
[<Tests>]
let tests =
    testList "product" [
        test "generated headless product exposes scene contract" {
            let scene: FS.Skia.UI.Scene.Scene = { Nodes = [ Product.Program.view initialModel ] }
            let text = scene.Nodes |> List.map sceneText |> String.concat " "
            let updated, effects = Product.Program.update Rendered initialModel

            Expect.isNonEmpty scene.Nodes "Product.Program.view returns a scene"
            Expect.stringContains text "Governed headless scene" "headless view renders scene text"
            Expect.equal updated.RenderCount 1 "headless update is callable"
            Expect.isEmpty effects "headless update has no host effects"
        }

        test "generated headless product exposes deterministic scene evidence command" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "--scene-evidence" "headless profile exposes scene evidence"
            Expect.stringContains source "SceneEvidence.render" "scene evidence uses public Scene evidence helper"
            Expect.stringContains source "RendererMode = \"deterministic-scene\"" "scene evidence is deterministic"
            Expect.isFalse (source.Contains("Viewer.runApp")) "headless profile does not require the viewer runtime"
            Expect.isFalse (source.Contains("ControlsElmish")) "headless profile does not require Controls Elmish adapters"
        }

        test "generated headless layout evidence is readable" {
            let report = Product.Program.layoutEvidenceForSize { Width = 640; Height = 480 } initialModel

            Expect.equal report.ProofLevel ReadableLayout "headless layout report proves readable layout"
            Expect.isSome report.HudRegion "headless layout report has a named summary region"
            Expect.isSome report.GameplayRegion "headless layout report has a named content region"
            Expect.isNonEmpty report.TextBounds "headless layout report has text bounds"
            Expect.isNonEmpty report.GameplayBounds "headless layout report has scene content bounds"
            Expect.equal report.OverlapStatus NoLayoutOverlap "headless layout report has no overlaps"
        }

        //#if (profile == "governed")
        test "generated governed profile validates layout through Testing helpers" {
            let report = Product.Program.layoutEvidenceForSize { Width = 640; Height = 480 } initialModel
            let result =
                FS.Skia.UI.Testing.GeneratedLayoutValidation.validate
                    { Report = report
                      RequireReadableLayout = true }

            Expect.isTrue result.Accepted "governed profile can validate generated layout evidence"
            Expect.equal result.FailureClass None "accepted governed layout has no failure class"
        }
        //#endif
    ]
//#else
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.SkiaViewer

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }

        test "generated product source is split by responsibility in compile order" {
            let productDir = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Product")
            let project = System.IO.File.ReadAllText(System.IO.Path.Combine(productDir, "Product.fsproj"))

            [ "Model.fs"; "View.fs"; "LayoutEvidence.fs"; "EvidenceCommands.fs"; "Program.fs" ]
            |> List.iter (fun file ->
                Expect.isTrue (System.IO.File.Exists(System.IO.Path.Combine(productDir, file))) $"{file} exists in generated product source"
                Expect.stringContains project $"Compile Include=\"{file}\"" $"{file} is included in compile order")

            let modelIndex = project.IndexOf("Model.fs", StringComparison.Ordinal)
            let viewIndex = project.IndexOf("View.fs", StringComparison.Ordinal)
            let layoutIndex = project.IndexOf("LayoutEvidence.fs", StringComparison.Ordinal)
            let evidenceIndex = project.IndexOf("EvidenceCommands.fs", StringComparison.Ordinal)
            let programIndex = project.IndexOf("Program.fs", StringComparison.Ordinal)

            Expect.isLessThan modelIndex viewIndex "model compiles before view"
            Expect.isLessThan viewIndex layoutIndex "view compiles before layout evidence"
            Expect.isLessThan layoutIndex evidenceIndex "layout evidence compiles before evidence commands"
            Expect.isLessThan evidenceIndex programIndex "evidence commands compile before entrypoint"

            let program = System.IO.File.ReadAllText(System.IO.Path.Combine(productDir, "Program.fs"))
            Expect.stringContains program "[<EntryPoint>]" "Program.fs keeps the entrypoint"
            Expect.stringContains program "match List.ofArray args" "Program.fs owns command dispatch"
            Expect.isFalse (program.Contains("let writeGeneratedEvidenceLines", StringComparison.Ordinal)) "Program.fs does not own report writing"
            Expect.isFalse (program.Contains("let layoutEvidenceForSize size model : LayoutEvidenceReport", StringComparison.Ordinal)) "Program.fs does not own layout evidence implementation"
        }

        test "generated public contract exposes qualified app-owned names" {
            let scene: FS.Skia.UI.Scene.Scene = { Nodes = [ Product.Program.view initialModel ] }
            let host = Product.Program.generatedHost
            let updated, _ = Product.Program.update NoOp initialModel

            Expect.isNonEmpty scene.Nodes "Product.Program.view returns a scene"
            Expect.equal updated initialModel "Product.Program.update is callable as the app reducer"
            Expect.isSome (host.MapKey Enter true) "Product.Program.generatedHost exposes viewer input mapping"
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

            let ended, _ = Product.Program.update EndReached resumed

            let restarted, _ =
                dispatchViewerKey { RawKey = "Enter"; Direction = ViewerKeyDirection.KeyDown } ended

            Expect.equal options.Screen Options "options screen opens through viewer key"
            Expect.equal main.Screen Main "options selection enters main screen"
            Expect.equal paused.Screen Paused "space pauses main interaction"
            Expect.equal resumed.Screen Main "escape resumes from pause"
            Expect.equal restarted.Screen Initial "end screen restarts through viewer Enter"
        }

        test "pure generated app transitions expose model message and effect behavior" {
            let started, startEffects = Product.Program.update (ViewerInput(Enter, true)) initialModel
            let interacted, interactionEffects = Product.Program.update (ViewerInput(ArrowLeft, true)) started

            Expect.equal started.Screen Main "pure update starts app"
            Expect.isEmpty startEffects "input transition has no host command"
            Expect.equal interacted.PrimaryInteractions 1 "primary interaction is counted"
            Expect.isEmpty interactionEffects "primary interaction has no host command"
        }

        test "generated host boundary keeps app commands separate from viewer effects" {
            let unchanged, appCommands = Product.Program.update SaveRequested initialModel
            let hosted, observedAppCommands, viewerEffects = Product.Program.interpretAtHostBoundary SaveRequested initialModel
            let hostUpdated, hostViewerEffects = Product.Program.generatedHost.Update SaveRequested initialModel

            Expect.equal unchanged initialModel "save command does not mutate the app model"
            Expect.equal hosted initialModel "host boundary preserves pure update result"
            Expect.equal hostUpdated initialModel "generated host uses the same pure update result"
            Expect.exists appCommands (function DispatchHostCommand "save:Product" -> true | _ -> false) "pure update emits an app command"
            Expect.equal observedAppCommands appCommands "host boundary exposes app commands before interpretation"
            Expect.exists (observedAppCommands |> List.map Product.Program.appCommandName) ((=) "app-command:dispatch-host-command:save:Product") "app command category is named separately"
            Expect.exists viewerEffects (function RenderScene _ -> true | _ -> false) "host boundary emits viewer render effect separately"
            Expect.equal hostViewerEffects.Length viewerEffects.Length "generated host returns the same number of viewer effects to SkiaViewer"
            Expect.exists hostViewerEffects (function RenderScene _ -> true | _ -> false) "generated host returns render effects to SkiaViewer"
        }

        test "generated graphical app exposes bounded smoke command" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "--launch-evidence" "generated product exposes explicit launch evidence CLI"
            Expect.stringContains source "Viewer.runBounded" "launch evidence uses a bounded evidence entry point"
            Expect.stringContains source "mode=persistent-evidence" "launch evidence reports evidence mode"
            Expect.stringContains source "--bounded-smoke" "generated product exposes bounded smoke CLI"
            Expect.stringContains source "--bounded-smoke-frame-diagnostics" "generated product exposes explicit frame diagnostic smoke CLI"
            Expect.stringContains source "Viewer.runBounded" "bounded smoke uses the public SkiaViewer bounded run entry point"
            Expect.stringContains source "status=unsupported" "bounded smoke reports unsupported host conditions explicitly"
            Expect.stringContains source "diagnostic-mode={diagnosticMode}" "generated smoke writes readable diagnostics mode"
            Expect.stringContains source "startup-focused" "startup-focused generated smoke is the default"
            Expect.stringContains source "frame-focused" "frame-focused generated smoke is opt-in"
            Expect.stringContains source "FrameLogLimit = if includeFrameDiagnostics then Some 1 else Some 0" "generated smoke limits repeated frame diagnostics"
        }

        test "generated evidence commands are opt-in and not reported as ongoing interactive play" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]
            let program = productSource "Program.fs"
            let defaultBranch = program.Substring(program.LastIndexOf("| args ->", StringComparison.Ordinal))

            Expect.stringContains source "--launch-evidence" "first-frame launch evidence is exposed only by explicit CLI flag"
            Expect.stringContains source "--bounded-smoke" "bounded evidence smoke is exposed only by explicit CLI flag"
            Expect.stringContains source "--bounded-smoke-frame-diagnostics" "frame diagnostics are exposed only by explicit CLI flag"
            Expect.stringContains source "--image-evidence" "image evidence is exposed only by explicit CLI flag"
            Expect.stringContains source "--screenshot-evidence" "screenshot evidence is exposed only by explicit CLI flag"
            Expect.stringContains source "--pixel-readback-evidence" "pixel-readback evidence is exposed only by explicit CLI flag"
            Expect.stringContains source "input-dispatch=not-required" "bounded evidence reports that input dispatch is not an interactive-play claim"
            Expect.stringContains source "self-closed-for-evidence=true" "bounded evidence reports self-close semantics"
            Expect.stringContains source "mode=persistent-evidence" "bounded evidence uses persistent evidence mode"
            Expect.stringContains source "command=--launch-evidence" "first-frame evidence records the evidence command"
            Expect.stringContains source "\"--image-evidence\"" "image evidence records the evidence command"
            Expect.stringContains source "\"--screenshot-evidence\"" "screenshot evidence records the evidence command"
            Expect.stringContains source "\"--pixel-readback-evidence\"" "pixel-readback evidence records the evidence command"
            Expect.stringContains source "Viewer.runBounded" "generated evidence commands use bounded viewer evidence entry points"
            Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost" "normal launch remains the persistent interactive path"
            Expect.isFalse (defaultBranch.Contains("mode=persistent-evidence")) "normal launch does not report bounded evidence mode"
            Expect.isFalse (defaultBranch.Contains("self-closed-for-evidence=true")) "normal launch does not claim evidence self-close"
            Expect.isFalse (defaultBranch.Contains("input-dispatch=not-required")) "normal launch does not reuse bounded evidence input-dispatch wording"
            Expect.isFalse (defaultBranch.Contains("--image-evidence")) "image evidence stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("--screenshot-evidence")) "screenshot evidence stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("--pixel-readback-evidence")) "pixel-readback evidence stays out of normal launch branch"
        }

        test "generated visual evidence commands require screenshot proof pixel fallback and unsupported diagnostics" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "--image-evidence" "generated product exposes image evidence command"
            Expect.stringContains source "--screenshot-evidence" "generated product exposes screenshot evidence command"
            Expect.stringContains source "--pixel-readback-evidence" "generated product exposes pixel-readback evidence command"
            Expect.stringContains source "evidenceField \"evidence-kind\" \"image\"" "image command records image evidence kind"
            Expect.stringContains source "evidenceField \"image-decodable\"" "image command records decodability"
            Expect.stringContains source "evidenceField \"proves-scene-rendering\" \"true\"" "image command records scene-rendering proof claim"
            Expect.stringContains source "evidenceField \"proves-desktop-visibility\" \"false\"" "image command records desktop-visibility proof claim"
            Expect.stringContains source "evidenceField \"evidence-kind\" \"screenshot\"" "screenshot command records screenshot evidence kind"
            Expect.stringContains source "Viewer.captureScreenshotEvidence" "screenshot command uses the viewer screenshot evidence contract"
            Expect.stringContains source "deterministic-scene-evidence" "unsupported screenshot command records deterministic fallback"
            Expect.stringContains source "evidenceField \"viewer-open-status\"" "screenshot command reports viewer-open status"
            Expect.stringContains source "evidenceField \"first-frame-status\"" "screenshot command reports first-frame status"
            Expect.stringContains source "evidenceField \"capture-availability\"" "screenshot command reports capture availability"
            Expect.stringContains source "evidenceField \"capture-source\"" "screenshot command reports capture source"
            Expect.stringContains source "evidenceField \"deterministic-fallback-kind\"" "screenshot command reports deterministic fallback kind"
            Expect.stringContains source "evidenceField \"proves-screenshot\"" "screenshot command reports screenshot proof boolean"
            Expect.isFalse (source.Contains("evidenceField \"capture-source\" \"pixel-readback\"", StringComparison.Ordinal)) "pixel readback is not relabeled as screenshot capture source"
            Expect.isFalse (source.Contains("evidenceField \"capture-source\" \"deterministic-scene-render\"\n              evidenceField \"proves-screenshot\" \"true\"", StringComparison.Ordinal)) "deterministic render is not relabeled as screenshot proof"
            Expect.stringContains source "evidenceField \"evidence-kind\" evidenceKind" "pixel-readback command records fallback evidence kind"
            Expect.stringContains source "evidenceField \"fallback-reason\" fallbackReason" "pixel-readback command records why screenshot proof was unavailable"
            Expect.stringContains source "screenshot-unavailable" "pixel-readback command names screenshot unavailability"
            Expect.stringContains source "evidenceField \"board-readable\" \"true\"" "visual evidence proves the board/grid is readable"
            Expect.stringContains source "evidenceField \"input-or-progress-observed\" \"true\"" "visual evidence proves input dispatch or time progression was observed"
            Expect.stringContains source "evidenceField \"unsupported-host-reason\"" "unsupported visual evidence reports why neither visual path is available"
            Expect.stringContains source "evidenceField \"supported-host\" \"false\"" "unsupported visual evidence is explicit instead of substituting text-only metadata"
        }

        test "generated evidence commands share Testing report conventions" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "let writeEvidenceReport" "generated product defines one local report wrapper"
            Expect.stringContains source "generatedEvidenceStatusText" "generated product shares normalized report status vocabulary"
            Expect.stringContains source "| GeneratedEvidenceOk -> \"ok\"" "generated product preserves ok status vocabulary"
            Expect.stringContains source "| GeneratedEvidenceUnsupported -> \"unsupported\"" "generated product preserves unsupported status vocabulary"
            Expect.stringContains source "| GeneratedEvidenceFailed -> \"failed\"" "generated product preserves failed status vocabulary"
            Expect.stringContains source "generatedEvidenceExitCode" "generated product keeps report status to exit-code semantics local"
            Expect.stringContains source "| GeneratedEvidenceUnsupported -> 0" "unsupported generated evidence remains a non-failing host fact"
            Expect.stringContains source "| GeneratedEvidenceFailed -> 1" "failed generated evidence remains a failing command result"
            Expect.stringContains source "writeEvidenceReport" "shared report wrapper is called by generated evidence commands"
            Expect.stringContains source "evidenceField \"command\" command" "report wrapper preserves command field"
            Expect.stringContains source "evidenceField \"output\" evidencePath" "report wrapper preserves output field"
            Expect.stringContains source "writeGeneratedEvidenceLines evidencePath true (generatedEvidenceExitCode status) lines" "report wrapper creates parent directories, writes the requested output path, and preserves exit-code semantics"
            Expect.stringContains source "lines |> List.iter (printfn \"%s\")" "report wrapper echoes report fields to stdout"
            Expect.stringContains source "\"--layout-evidence\"" "layout command reports through the shared convention"
            Expect.stringContains source "\"--launch-evidence\"" "launch command preserves its public command name"
            Expect.stringContains source "\"--image-evidence\"" "image command reports through the shared convention"
            Expect.stringContains source "\"--screenshot-evidence\"" "screenshot command reports through the shared convention"
            Expect.stringContains source "\"--pixel-readback-evidence\"" "pixel-readback command reports through the shared convention"
        }

        test "generated graphical app default executable path uses persistent host" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "let viewerOptions" "generated product declares viewer options"
            Expect.stringContains source "let generatedHost" "generated product declares generated host"
            Expect.stringContains source "MapKey = mapKey" "generated host wires keyboard mapping"
            Expect.stringContains source "Tick = tick" "generated host wires tick mapping"
            Expect.stringContains source "Viewer.runApp viewerOptions generatedHost" "default path runs persistent generated app host"
            Expect.stringContains source "mode=interactive-window" "default path reports interactive mode"
            Expect.stringContains source "accessible-window=true" "successful default path reports accessible desktop window claim"
            Expect.stringContains source "window-visible=observed:true" "successful default path reports observed visible window"
            Expect.stringContains source "accessible-window=false" "unsupported default path does not claim visible accessibility"
            Expect.stringContains source "mode=interactive-window" "unsupported default diagnostics still identify interactive mode"
            Expect.stringContains source "--bounded-smoke" "bounded smoke remains behind an explicit flag"
            Expect.stringContains source "--launch-evidence" "launch evidence remains behind an explicit flag"
        }

        test "generated normal launch reports desktop session diagnostics without evidence fallback" {
            let source = productSource "Program.fs"
            let defaultBranch = source.Substring(source.LastIndexOf("| args ->", StringComparison.Ordinal))

            Expect.stringContains defaultBranch "Viewer.desktopSessionDiagnostic()" "normal launch captures desktop/session diagnostics before app lifecycle debugging"
            Expect.stringContains defaultBranch "diagnostic-class=" "normal launch reports diagnostic classification"
            Expect.stringContains defaultBranch "runtime-directory=" "normal launch reports runtime directory state"
            Expect.stringContains defaultBranch "display-variable=" "normal launch reports display variable state"
            Expect.stringContains defaultBranch "display-socket-exists=" "normal launch reports display socket state"
            Expect.stringContains defaultBranch "session-bus=" "normal launch reports session bus state"
            Expect.stringContains defaultBranch "fallback-is-full-desktop-session=false" "private runtime fallback is labeled as not a full desktop session"
            Expect.isFalse (defaultBranch.Contains("Viewer.runBounded")) "normal launch does not silently switch to bounded evidence"
            Expect.isFalse (defaultBranch.Contains("SceneEvidence.render")) "normal launch does not silently switch to scene-only metadata"
            Expect.isFalse (defaultBranch.Contains("--launch-evidence")) "explicit evidence flag stays out of normal launch diagnostics"
            Expect.isFalse (defaultBranch.Contains("--scene-evidence")) "scene evidence flag stays out of normal launch diagnostics"
        }

        test "generated window diagnostics command reports failure classes and native facts before app debugging" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]
            let program = productSource "Program.fs"
            let defaultBranch = program.Substring(program.LastIndexOf("| args ->", StringComparison.Ordinal))

            Expect.stringContains source "--window-diagnostics" "generated product exposes an explicit window diagnostics command"
            Expect.stringContains source "diagnostic-class=environment-session" "diagnostics include environment/session class"
            Expect.stringContains source "diagnostic-class=window-visibility" "diagnostics include window visibility class"
            Expect.stringContains source "diagnostic-class=app-lifecycle" "diagnostics include app lifecycle class"
            Expect.stringContains source "diagnostic-class=product-defect" "diagnostics include product defect class"
            Expect.stringContains source "native-handle=observed:true" "diagnostics include native handle facts"
            Expect.stringContains source "visible=observed:false" "diagnostics include visible observed-false facts"
            Expect.stringContains source "focusable=observed:false" "diagnostics include focusable facts"
            Expect.stringContains source "minimized=observed:false" "diagnostics include minimized facts"
            Expect.stringContains source "maximized=observed:false" "diagnostics include maximized facts"
            Expect.stringContains source "client-size=0x0" "diagnostics include zero-sized client facts"
            Expect.stringContains source "renderable-surface=observed:false" "diagnostics include renderable-surface facts"
            Expect.stringContains source "input-devices=unavailable" "diagnostics include input-device availability facts"
            Expect.stringContains source "fallback-is-full-desktop-session=" "diagnostics disclose fallback session status"
            Expect.isFalse (defaultBranch.Contains("--window-diagnostics")) "normal launch does not silently switch to diagnostics mode"
        }

        test "generated app Synthetic exposes window behavior flags and option diagnostics without leaving interactive launch" {
            let source = productSources [ "Program.fs"; "WindowOptions.fs" ]
            let program = productSource "Program.fs"
            let defaultBranch = program.Substring(program.LastIndexOf("| args ->", StringComparison.Ordinal))

            Expect.stringContains source "--window-resize" "resize policy is configurable"
            Expect.stringContains source "--window-maximize" "maximize policy is configurable"
            Expect.stringContains source "--window-startup" "startup state is configurable"
            Expect.stringContains source "--window-position" "startup position is configurable"
            Expect.stringContains source "--window-backend" "backend preference is configurable"
            Expect.stringContains source "--window-options-file" "option files are supported"
            Expect.stringContains source "--window-options" "generated product exposes option diagnostics"
            Expect.stringContains source "windowBehaviorArgsFromFile" "option files are parsed into launch flags"
            Expect.stringContains source "toViewerWindowBehavior windowBehavior" "parsed flags become the public viewer request"
            Expect.stringContains source "Viewer.validateWindowLaunchBehavior viewerOptions.InitialSize" "generated diagnostics use public launch behavior validation"
            Expect.stringContains source "Viewer.runApp viewerOptions generatedHost" "default launch applies the selected persistent viewer contract"
            Expect.stringContains source "manualWindowOptionResults windowBehaviorRequest" "normal launch validates parsed behavior request before calling SkiaViewer"
            Expect.stringContains source "window-options=%s" "normal launch reports option validation output"
            Expect.stringContains source "option=resize" "option report includes resize rows"
            Expect.stringContains source "option=maximize" "option report includes maximize rows"
            Expect.stringContains source "option=startup-state" "option report includes startup-state rows"
            Expect.stringContains source "option=startup-position" "option report includes startup-position rows"
            Expect.stringContains source "option=backend" "option report includes backend rows"
            Expect.stringContains source "status=unsupported" "unsupported host/backend option diagnostics are explicit"
            Expect.isFalse (defaultBranch.Contains("Viewer.runBounded")) "window options do not switch normal launch to bounded evidence"
        }

        test "generated default game view renders board grid and side information" {
            let rendered = view initialModel
            let nodes = collectSceneNodes rendered |> Seq.toList
            let text = sceneText rendered

            let rectangleCount =
                nodes
                |> List.filter (function Rectangle _ | PaintedRectangle _ -> true | _ -> false)
                |> List.length

            let lineCount =
                nodes
                |> List.filter (function Line _ -> true | _ -> false)
                |> List.length

            Expect.isGreaterThanOrEqual rectangleCount 20 "Tetris-style board renders multiple cells"
            Expect.isGreaterThanOrEqual lineCount 10 "Tetris-style board renders visible grid lines"
            Expect.stringContains text "score" "side panel includes score"
            Expect.stringContains text "level" "side panel includes level"
            Expect.stringContains text "next" "side panel includes next piece"
        }

        test "generated default game view uses circular and elliptical entities without rectangle substitution" {
            let rendered = view initialModel
            let nodes = collectSceneNodes rendered |> Seq.toList

            let roundEntityCount =
                nodes
                |> List.filter (function Circle _ | FilledEllipse _ | Ellipse _ -> true | _ -> false)
                |> List.length

            Expect.isGreaterThanOrEqual roundEntityCount 3 "generated scene renders at least three circular or elliptical entities"
            Expect.contains (Scene.describe { Nodes = nodes }) CircleElement "generated scene contains public circle element"
            Expect.contains (Scene.describe { Nodes = nodes }) EllipseElement "generated scene contains public ellipse element"
        }

        test "generated game layout evidence separates HUD and gameplay at default and constrained sizes" {
            let defaultReport = Product.Program.layoutEvidenceForSize { Width = 1280; Height = 720 } initialModel
            let constrainedReport = Product.Program.layoutEvidenceForSize { Width = 640; Height = 480 } initialModel

            [ defaultReport; constrainedReport ]
            |> List.iter (fun report ->
                Expect.equal report.ProofLevel ReadableLayout "generated report proves readable layout"
                Expect.isSome report.HudRegion "HUD region is named"
                Expect.isSome report.GameplayRegion "gameplay region is named"
                Expect.isNonEmpty report.TextBounds "HUD text bounds are present"
                Expect.isNonEmpty report.GameplayBounds "active gameplay bounds are present"
                Expect.equal report.OverlapStatus NoLayoutOverlap "HUD and gameplay bounds do not overlap"
                Expect.equal report.MeasurementMode ApproximateTextBounds "generated layout evidence reports the measurement mode"
                Expect.isEmpty report.UnsupportedReasons "readable generated layout does not use unsupported-host classification")
        }

        test "generated game layout validation fails broken HUD and gameplay layouts" {
            let hudOverlap = Product.Program.layoutEvidenceForSize { Width = 480; Height = 480 } initialModel
            let gameplayOverlap =
                Product.Program.layoutEvidenceForSize
                    { Width = 640; Height = 480 }
                    { initialModel with ActiveRow = -6 }

            let hudResult = Product.Program.validateGeneratedLayout hudOverlap
            let gameplayResult = Product.Program.validateGeneratedLayout gameplayOverlap

            Expect.isFalse hudResult.Accepted "HUD/HUD overlap fails validation"
            Expect.equal hudResult.FailureClass (Some OverlappingLayoutBounds) "HUD/HUD overlap is classified"
            Expect.isFalse gameplayResult.Accepted "HUD/gameplay overlap fails validation"
            Expect.equal gameplayResult.FailureClass (Some OverlappingLayoutBounds) "HUD/gameplay overlap is classified"
        }

        test "generated gameplay policies use gameplay region for active entity movement and bounds" {
            let started, _ = Product.Program.update (ViewerInput(Enter, true)) initialModel
            let moved, _ = Product.Program.update (ViewerInput(ArrowLeft, true)) started
            let ticked, _ = Product.Program.update GameTick moved

            let region = Product.Program.gameplayRegionForSize { Width = 640; Height = 480 }
            let bounds = Product.Program.activeGameplayBoundsForSize { Width = 640; Height = 480 } ticked

            Expect.isTrue (Product.Program.boundsInside region.Bounds bounds.Bounds) "active entity remains inside gameplay region"
            Expect.isTrue (Product.Program.movementUsesGameplayRegion { Width = 640; Height = 480 } ticked) "movement policy is region based"
            Expect.isTrue (Product.Program.spawnUsesGameplayRegion { Width = 640; Height = 480 } initialModel) "spawn policy is region based"
            Expect.isTrue (Product.Program.collisionUsesGameplayRegion { Width = 640; Height = 480 } ticked) "collision policy is region based"
        }

        test "generated default game dispatches input advances over time and keeps evidence flags opt-in" {
            let started, _ = dispatchViewerKey { RawKey = "Enter"; Direction = ViewerKeyDirection.KeyDown } initialModel
            let moved, _ = dispatchViewerKey { RawKey = "ArrowLeft"; Direction = ViewerKeyDirection.KeyDown } started

            Expect.notEqual moved initialModel "keyboard input changes playable game state"
            Expect.isGreaterThan moved.PrimaryInteractions started.PrimaryInteractions "left input is reflected in gameplay state"

            match tick (TimeSpan.FromMilliseconds 500.0) with
            | Some tickMsg ->
                let afterTick, _ = Product.Program.update tickMsg moved
                Expect.notEqual afterTick moved "time-based tick advances gameplay state"
            | None -> failtest "generated game tick must advance gameplay over time"

            let source = System.IO.File.ReadAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Product", "Program.fs"))
            let defaultBranch = source.Substring(source.LastIndexOf("| args ->", StringComparison.Ordinal))
            Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost" "normal launch uses interactive host"
            Expect.isFalse (defaultBranch.Contains("--launch-evidence")) "launch evidence flag stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("--bounded-smoke")) "bounded smoke flag stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("self-closed-for-evidence=true")) "normal launch does not report evidence self-close"
        }

        test "generated graphical app exposes deterministic scene evidence command" {
            let source = productSources [ "Program.fs"; "EvidenceCommands.fs" ]

            Expect.stringContains source "--scene-evidence" "generated product exposes non-window scene evidence CLI"
            Expect.stringContains source "SceneEvidence.render" "scene evidence uses public Scene evidence helper"
            Expect.stringContains source "RendererMode = \"deterministic-scene\"" "scene evidence remains separate from live viewer startup"
            Expect.stringContains source "readiness/headless-scene-evidence.txt" "scene evidence writes a stable readiness path"
        }

        test "generated evidence graph command delegates to authoritative validation" {
            let build = System.IO.File.ReadAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "build.fsx"))

            Expect.stringContains build "let runGeneratedEvidenceGraph" "generated build exposes graph command runner"
            Expect.stringContains build ".specify/extensions/evidence/scripts/bash/run-audit.sh" "graph command delegates to copied Spec Kit audit script"
            Expect.stringContains build "--graph-only" "graph command selects graph-only authoritative validation"
            Expect.stringContains build "authority=delegated-authoritative" "graph report records delegated authority"
            Expect.stringContains build "status=failed" "graph report has an explicit failed status path"
            Expect.stringContains build "authoritative validation failed" "graph failure is reported before any pass claim"
            Expect.stringContains build "let runGeneratedEvidenceAudit" "generated build exposes audit command runner"
            Expect.stringContains build "let graphExitCode, graphStdout, graphStderr = runAuthoritativeEvidence \"EvidenceGraph\" featureDir true" "audit command requires graph validation first"
            Expect.stringContains build "runAuthoritativeEvidence \"EvidenceAudit\" featureDir false" "audit command delegates full audit validation"
            Expect.stringContains build "readiness-contract" "audit report distinguishes readiness contract failures"
            Expect.stringContains build "synthetic-evidence" "audit report distinguishes synthetic evidence failures"
            Expect.isFalse (build.Contains("| \"EvidenceGraph\"\n    | \"EvidenceAudit\" -> writeLog target")) "evidence commands are not completion-only logs"
        }
    ]
//#endif
