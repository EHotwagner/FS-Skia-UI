module ProductBehaviorTests

open System
open Expecto
open Product.Program
open Product.Model
open FS.Skia.UI.Scene

// Feature 060 (FR-005): replaceable scaffold-BEHAVIOR tests. These call the scaffold
// product's `view`/`update`/host/scene-text directly, so when you replace the scaffold
// model with your own you rewrite THIS file. `GovernanceTests.fs` (compiled first) keeps
// its model-agnostic source/structure/evidence scans green across that swap.

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

//#if (profile == "governed" || profile == "headless-scene")
[<Tests>]
let behaviorTests =
    testList "product-behavior" [
        test "generated headless product exposes scene contract" {
            let scene: FS.Skia.UI.Scene.Scene = { Nodes = [ Product.Program.view initialModel ] }
            let text = scene.Nodes |> List.map sceneText |> String.concat " "
            let updated, effects = Product.Program.update Rendered initialModel

            Expect.isNonEmpty scene.Nodes "Product.Program.view returns a scene"
            Expect.stringContains text "Governed headless scene" "headless view renders scene text"
            Expect.equal updated.RenderCount 1 "headless update is callable"
            Expect.isEmpty effects "headless update has no host effects"
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
let behaviorTests =
    testList "product-behavior" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
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

        test "generated default game view renders grid playfield and side information" {
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

            Expect.isGreaterThanOrEqual rectangleCount 20 "grid-style playfield renders multiple cells"
            Expect.isGreaterThanOrEqual lineCount 10 "grid-style playfield renders visible grid lines"
            Expect.stringContains text "tally" "side panel includes tally"
            Expect.stringContains text "stage" "side panel includes stage"
            Expect.stringContains text "upcoming" "side panel includes upcoming token"
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
            let defaultBranch = source.Substring(source.LastIndexOf("| None ->", StringComparison.Ordinal))
            Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost" "normal launch uses interactive host"
            Expect.isFalse (defaultBranch.Contains("--launch-evidence")) "launch evidence flag stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("--bounded-smoke")) "bounded smoke flag stays out of normal launch branch"
            Expect.isFalse (defaultBranch.Contains("self-closed-for-evidence=true")) "normal launch does not report evidence self-close"
        }
    ]
//#endif
