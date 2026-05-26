module SkiaViewerCapabilityTests

open System
open System.Collections.Generic
open Expecto
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type HostModel =
    { Count: int
      Closed: bool }

type HostMsg =
    | Increment
    | Close

[<Tests>]
let tests =
    testList "SkiaViewer MVU contract" [
        test "init emits window-open effect" {
            let model, effects = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            Expect.isFalse model.IsRunning "viewer starts stopped"
            Expect.exists effects (function OpenWindow("Product", { Width = 640; Height = 480 }) -> true | _ -> false) "init emits open effect"
        }

        test "render updates model and emits render effect" {
            let model, _ = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            let scene = Group []
            let next, effects = Viewer.update (Render scene) model
            Expect.equal next.LastScene (Some scene) "last scene is stored"
            Expect.exists effects (function RenderScene rendered when rendered = scene -> true | _ -> false) "render effect is emitted"
        }

        test "bounded run init and update expose pure lifecycle effects" {
            let request =
                { Target = FirstFrame
                  Timeout = TimeSpan.FromSeconds 2.0
                  Diagnostics = Viewer.defaultDiagnostics
                  RendererMode = "vulkan"
                  EvidencePath = Some "readiness/logs/viewer-smoke.json" }

            let model, effects = Viewer.initRun request
            Expect.equal model.FramesRendered 0 "bounded run starts with no frames"
            Expect.exists effects (function OpenBoundedWindow opened when opened.RendererMode = request.RendererMode -> true | _ -> false) "bounded run requests window opening at interpreter edge"

            let size = { Width = 320; Height = 200 }
            let afterFrame, frameEffects = Viewer.updateRun (RecordFrame size) model
            Expect.equal afterFrame.FramesRendered 1 "recorded frame increments count"
            Expect.exists frameEffects (function StopBoundedRun -> true | _ -> false) "first-frame target stops when evidence is collected"
        }

        test "bounded run records first-frame evidence with positive dimensions elapsed time and summary" {
            let request =
                { Target = FirstFrame
                  Timeout = TimeSpan.FromSeconds 2.0
                  Diagnostics = Viewer.defaultDiagnostics
                  RendererMode = "vulkan"
                  EvidencePath = Some "readiness/logs/viewer-smoke.txt" }

            let diagnostic =
                { Level = Info
                  Category = Frame
                  Message = "frame 1 presented"
                  FrameIndex = Some 1
                  Stage = None
                  Elapsed = Some(TimeSpan.FromMilliseconds 16.0) }

            let model, _ = Viewer.initRun request
            let started, startEffects = Viewer.updateRun (RunStarted(DateTimeOffset.UnixEpoch)) model
            Expect.equal started.StartedAt (Some DateTimeOffset.UnixEpoch) "start time is supplied by interpreter message"
            Expect.exists startEffects (function RequestFrame -> true | _ -> false) "started run requests the first frame"

            let withDiagnostic, _ = Viewer.updateRun (RecordDiagnostic diagnostic) started
            let completed, effects = Viewer.updateRun (RecordFrame { Width = 320; Height = 200 }) withDiagnostic

            match completed.Completed with
            | Some(Ok evidence) ->
                Expect.equal evidence.FramesRendered 1 "first-frame target captures one frame"
                Expect.isGreaterThan evidence.Elapsed TimeSpan.Zero "elapsed time is positive"
                Expect.equal evidence.InitialOutputSize { Width = 320; Height = 200 } "output size is captured"
                Expect.equal evidence.RendererMode "vulkan" "renderer mode is preserved"
                Expect.equal evidence.LastDiagnosticSummary (Some "frame 1 presented") "last diagnostic summary is preserved"
                Expect.equal evidence.EvidencePath request.EvidencePath "evidence path is preserved"
            | other -> failtestf "expected first-frame evidence, got %A" other

            Expect.exists effects (function StopBoundedRun -> true | _ -> false) "bounded run stops after the target"
        }

        test "bounded run validates positive frame counts timeouts and durations" {
            let options = { Title = "Product"; InitialSize = { Width = 320; Height = 200 } }
            let scene = Group []

            let invalidFrameRequest =
                { Target = FrameCount 0
                  Timeout = TimeSpan.FromSeconds 2.0
                  Diagnostics = Viewer.defaultDiagnostics
                  RendererMode = "vulkan"
                  EvidencePath = None }

            let invalidTimeoutRequest =
                { invalidFrameRequest with
                    Target = FirstFrame
                    Timeout = TimeSpan.Zero }

            let invalidDurationRequest =
                { invalidFrameRequest with
                    Target = Duration TimeSpan.Zero
                    Timeout = TimeSpan.FromSeconds 2.0 }

            [ invalidFrameRequest, "frame count"
              invalidTimeoutRequest, "timeout"
              invalidDurationRequest, "duration" ]
            |> List.iter (fun (request, expected) ->
                match Viewer.runBounded request options scene with
                | Result.Error failure ->
                    Expect.equal failure.Classification ProductDefect $"{expected} validation is a product defect"
                    Expect.equal failure.BlockedStage App $"{expected} validation is blocked by app/request configuration"
                    Expect.stringContains failure.Message "positive" $"{expected} failure is actionable"
                | Result.Ok evidence -> failtestf "expected %s validation failure, got %A" expected evidence)
        }

        test "bounded run frame-count target stops after the exact positive frame count" {
            let request =
                { Target = FrameCount 3
                  Timeout = TimeSpan.FromSeconds 2.0
                  Diagnostics = Viewer.defaultDiagnostics
                  RendererMode = "vulkan"
                  EvidencePath = None }

            let model, _ = Viewer.initRun request

            let finalModel =
                [ 1..3 ]
                |> List.fold
                    (fun current frame ->
                        let diagnostic =
                            { Level = Info
                              Category = Frame
                              Message = $"frame {frame} presented"
                              FrameIndex = Some frame
                              Stage = None
                              Elapsed = Some(TimeSpan.FromMilliseconds(float frame * 16.0)) }

                        let withDiagnostic, _ = Viewer.updateRun (RecordDiagnostic diagnostic) current
                        Viewer.updateRun (RecordFrame { Width = 320; Height = 200 }) withDiagnostic |> fst)
                    model

            match finalModel.Completed with
            | Some(Ok evidence) ->
                Expect.equal evidence.FramesRendered 3 "exact frame target is captured"
                Expect.equal evidence.LastDiagnosticSummary (Some "frame 3 presented") "last frame diagnostic is summarized"
            | other -> failtestf "expected frame-count evidence, got %A" other
        }

        test "forced pre-frame failures classify blocked stages and unsupported host capabilities" {
            let cases: (ViewerRunBlockedStage * ViewerRunFailureClassification * ViewerDiagnosticCategory) list =
                [ Window, UnsupportedEnvironment, Startup
                  Surface, UnsupportedEnvironment, Startup
                  ViewerRunBlockedStage.Renderer, UnsupportedEnvironment, ViewerDiagnosticCategory.Renderer
                  ViewerRunBlockedStage.Swapchain, UnsupportedEnvironment, ViewerDiagnosticCategory.Swapchain
                  Readback, UnsupportedEnvironment, Screenshot
                  ViewerRunBlockedStage.Scene, ProductDefect, ViewerDiagnosticCategory.Scene
                  App, ProductDefect, Startup
                  Timeout, ProductDefect, Startup
                  Unknown, ProductDefect, Startup ]

            cases
            |> List.iter (fun (stage, classification, category) ->
                let diagnostic =
                    { Level = ViewerDiagnosticLevel.Error
                      Category = category
                      Message = $"blocked at {stage}"
                      FrameIndex = None
                      Stage = Some stage
                      Elapsed = Some TimeSpan.Zero }

                let failure = Viewer.failureFromDiagnostic diagnostic
                Expect.equal failure.BlockedStage stage $"{stage} blocked stage is preserved"
                Expect.equal failure.Classification classification $"{stage} classification is preserved"
                Expect.equal failure.DiagnosticCategory category $"{stage} diagnostic category is preserved"
                Expect.equal failure.LastDiagnosticSummary (Some diagnostic.Message) $"{stage} keeps summary")
        }

        test "bounded run timeout uses last diagnostic summary and stops without shell timeout" {
            let request =
                { Target = Duration(TimeSpan.FromSeconds 10.0)
                  Timeout = TimeSpan.FromMilliseconds 1.0
                  Diagnostics = Viewer.defaultDiagnostics
                  RendererMode = "vulkan"
                  EvidencePath = None }

            let diagnostic =
                { Level = Warning
                  Category = Startup
                  Message = "waiting for first frame"
                  FrameIndex = None
                  Stage = Some Timeout
                  Elapsed = Some(TimeSpan.FromMilliseconds 1.0) }

            let model, _ = Viewer.initRun request
            let withDiagnostic, _ = Viewer.updateRun (RecordDiagnostic diagnostic) model
            let timedOut, effects = Viewer.updateRun TimeoutRun withDiagnostic

            match timedOut.Completed with
            | Some(Result.Error failure) ->
                Expect.equal failure.BlockedStage Timeout "timeout stage is explicit"
                Expect.equal failure.Classification ProductDefect "timeout is product-defect classification"
                Expect.equal failure.LastDiagnosticSummary (Some "waiting for first frame") "last diagnostic summary is retained"
            | other -> failtestf "expected timeout failure, got %A" other

            Expect.exists effects (function StopBoundedRun -> true | _ -> false) "timeout stops the bounded run internally"
        }

        test "diagnostics and viewer key events flow through public update effects" {
            let model, _ = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            let diagnostic =
                { Level = Info
                  Category = Startup
                  Message = "window-created"
                  FrameIndex = None
                  Stage = Some Window
                  Elapsed = Some TimeSpan.Zero }

            let _, diagnosticEffects = Viewer.update (DiagnosticCaptured diagnostic) model
            Expect.exists diagnosticEffects (function EmitDiagnostic emitted when emitted.Message = diagnostic.Message -> true | _ -> false) "diagnostic capture is emitted to the edge"

            let _, keyEffects =
                Viewer.update
                    (KeyEvent { RawKey = "Enter"; Direction = ViewerKeyDirection.KeyDown })
                    model

            Expect.exists keyEffects (function DispatchInput(Enter, true) -> true | _ -> false) "viewer key events dispatch normalized input"
        }

        test "persistent run exposes launch outcome fields or unsupported-host diagnostics" {
            let options = { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            let scene = Group []

            match Viewer.run options scene with
            | Result.Ok outcome ->
                Expect.equal outcome.Status "ok" "persistent launch reports ok status"
                Expect.equal outcome.Mode "persistent-window" "persistent launch mode is explicit"
                Expect.equal outcome.WindowOpened true "window-opened evidence is explicit"
                Expect.equal outcome.ExitPath true "intentional exit path is explicit"
                Expect.equal outcome.InputDispatch "not-applicable" "scene-only launch marks input dispatch not applicable"
                Expect.isNone outcome.BlockedStage "successful launch has no blocked stage"
                Expect.isNone outcome.Classification "successful launch has no failure classification"
            | Result.Error failure ->
                Expect.equal failure.Classification UnsupportedEnvironment "headless or unsupported hosts are classified separately from product defects"
                Expect.equal failure.BlockedStage Window "persistent launch is blocked before window creation"
                Expect.stringContains failure.Message "DISPLAY" "unsupported Linux diagnostics name the missing display host"
        }

        test "runtime capability distinguishes persistent window bounded smoke keyboard and unsupported reasons" {
            let capability = Viewer.runtimeCapability()

            Expect.isTrue capability.BoundedSmoke "bounded smoke remains available as explicit evidence helper"
            Expect.isTrue capability.KeyboardInput "keyboard input capability is reported"
            Expect.equal capability.RendererMode "skia" "renderer mode is reported independently from host support"
            Expect.isEmpty capability.MissingPackageCapabilities "current package exposes the persistent contract and has no package-capability gap"

            if capability.PersistentWindow then
                Expect.isEmpty capability.UnsupportedHostReasons "supported hosts do not report unsupported reasons"
            else
                Expect.isNonEmpty capability.UnsupportedHostReasons "unsupported hosts report actionable reasons"

            capability.UnsupportedHostReasons
            |> List.iter (fun reason ->
                Expect.isFalse (capability.MissingPackageCapabilities |> List.contains reason) "unsupported host reasons are not reported as missing package capabilities")
        }

        test "persistent run preserves bounded APIs as explicit separate helpers" {
            let options = { Title = "Product"; InitialSize = { Width = 320; Height = 200 } }
            let scene = Group []

            let invalidPersistent = Viewer.run { options with Title = "" } scene
            let invalidBounded =
                Viewer.runBounded
                    { Target = FrameCount 0
                      Timeout = TimeSpan.FromSeconds 1.0
                      Diagnostics = Viewer.defaultDiagnostics
                      RendererMode = "skia"
                      EvidencePath = None }
                    options
                    scene

            match invalidPersistent, invalidBounded with
            | Result.Error persistentFailure, Result.Error boundedFailure ->
                Expect.equal persistentFailure.Classification ProductDefect "persistent option validation remains product-defect classification"
                Expect.equal boundedFailure.Classification ProductDefect "bounded request validation remains product-defect classification"
                Expect.stringContains boundedFailure.Message "frame count" "bounded helper keeps its own request validation"
            | other -> failtestf "expected separate persistent and bounded validation failures, got %A" other
        }

        test "bounded helper APIs remain explicitly callable regression" {
            let scene = Group []
            let invalidOptions = { Title = ""; InitialSize = { Width = 320; Height = 200 } }
            let validOptions = { Title = "Product"; InitialSize = { Width = 320; Height = 200 } }

            match Viewer.runUntilFirstFrame invalidOptions scene with
            | Result.Error failure ->
                Expect.equal failure.Classification ProductDefect "first-frame helper keeps option validation"
                Expect.stringContains failure.Message "title" "first-frame helper reports title validation"
            | Result.Ok evidence -> failtestf "expected first-frame validation failure, got %A" evidence

            match Viewer.runForFrames 0 validOptions scene with
            | Result.Error failure ->
                Expect.equal failure.Classification ProductDefect "frame-count helper keeps request validation"
                Expect.stringContains failure.Message "frame count" "frame-count helper reports frame validation"
            | Result.Ok evidence -> failtestf "expected frame-count validation failure, got %A" evidence
        }

        test "generated app host public boundary maps keyboard tick update and close effects" {
            let host =
                { Init = fun () -> { Count = 0; Closed = false }, []
                  Update =
                    fun msg model ->
                        match msg with
                        | Increment -> { model with Count = model.Count + 1 }, [ RenderScene(Group []) ]
                        | Close -> { model with Closed = true }, [ CloseWindow ]
                  View = fun model -> Text((0.0, 0.0), $"count {model.Count}", { Red = 255uy; Green = 255uy; Blue = 255uy; Alpha = 255uy })
                  MapKey = fun key isDown -> if isDown && key = Space then Some Increment else None
                  Tick = fun elapsed -> if elapsed > TimeSpan.Zero then Some Increment else None
                  Diagnostics = Viewer.defaultDiagnostics }

            let model, effects =
                GeneratedAppHost.dispatchKey
                    host
                    { RawKey = "Space"
                      Direction = ViewerKeyDirection.KeyDown }
                    { Count = 0; Closed = false }

            Expect.equal model.Count 1 "keyboard dispatch routes through host update"
            Expect.exists effects (function RenderScene _ -> true | _ -> false) "host update effects are emitted at the viewer boundary"
            Expect.equal (host.Tick(TimeSpan.FromMilliseconds 16.0)) (Some Increment) "tick mapping is public and pure"

            match Viewer.runApp { Title = "Product"; InitialSize = { Width = 640; Height = 480 } } host with
            | Result.Ok outcome ->
                Expect.equal outcome.Mode "persistent-window" "runApp reports persistent-window mode"
                Expect.equal outcome.Status "ok" "runApp reports ok status on supported hosts"
            | Result.Error failure ->
                Expect.equal failure.Classification UnsupportedEnvironment "runApp reports unsupported host separately from product defects"
        }

        test "diagnostic filtering honors categories and level thresholds across startup input renderer and readback categories" {
            let options =
                { Viewer.defaultDiagnostics with
                    MinimumLevel = Warning
                    Categories = Set.ofList [ ViewerDiagnosticCategory.Startup; Input; ViewerDiagnosticCategory.Renderer; Screenshot ] }

            let diagnostic level category message =
                { Level = level
                  Category = category
                  Message = message
                  FrameIndex = None
                  Stage = None
                  Elapsed = Some TimeSpan.Zero }

            let captured =
                [ diagnostic Error ViewerDiagnosticCategory.Startup "startup failed"
                  diagnostic Warning Input "input fallback"
                  diagnostic Warning ViewerDiagnosticCategory.Renderer "renderer fallback"
                  diagnostic Warning Screenshot "readback unavailable" ]

            captured
            |> List.iter (fun item ->
                Expect.isTrue (Viewer.shouldCaptureDiagnostic options item) $"captures {item.Category} {item.Level}")

            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Info ViewerDiagnosticCategory.Startup "startup info")) "info below warning threshold is filtered"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Warning Vulkan "vulkan detail")) "unselected category is filtered"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Error Skia "skia detail")) "unselected Skia category is filtered"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Error ViewerDiagnosticCategory.Swapchain "swapchain detail")) "unselected swapchain category is filtered"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Error ViewerDiagnosticCategory.Scene "scene detail")) "unselected scene category is filtered"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic options (diagnostic Error Frame "frame detail")) "unselected frame category is filtered"
        }

        test "frame sampling excludes repeated per-frame diagnostics unless enabled and bounded by the frame limit" {
            let frame index =
                { Level = Info
                  Category = ViewerDiagnosticCategory.Frame
                  Message = $"frame {index} presented"
                  FrameIndex = Some index
                  Stage = None
                  Elapsed = Some(TimeSpan.FromMilliseconds(float index * 16.0)) }

            let startup =
                { Level = Info
                  Category = ViewerDiagnosticCategory.Startup
                  Message = "window-created"
                  FrameIndex = None
                  Stage = Some Window
                  Elapsed = Some TimeSpan.Zero }

            let startupOnly =
                { Viewer.defaultDiagnostics with
                    Categories = Set.ofList [ ViewerDiagnosticCategory.Startup ]
                    FrameLogLimit = Some 0 }

            Expect.isTrue (Viewer.shouldCaptureDiagnostic startupOnly startup) "startup diagnostics are still captured"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic startupOnly (frame 1)) "startup-only diagnostics exclude frame messages"

            let sampledFrames =
                { startupOnly with
                    Categories = Set.ofList [ ViewerDiagnosticCategory.Startup; ViewerDiagnosticCategory.Frame ]
                    FrameLogLimit = Some 2 }

            Expect.isTrue (Viewer.shouldCaptureDiagnostic sampledFrames (frame 1)) "first sampled frame is captured"
            Expect.isTrue (Viewer.shouldCaptureDiagnostic sampledFrames (frame 2)) "second sampled frame is captured"
            Expect.isFalse (Viewer.shouldCaptureDiagnostic sampledFrames (frame 3)) "frame diagnostics stop after the configured limit"

            let unlimitedFrames = { sampledFrames with FrameLogLimit = None }
            Expect.isTrue (Viewer.shouldCaptureDiagnostic unlimitedFrames (frame 25)) "unbounded frame diagnostics are explicit"
        }

        test "diagnostic sink captures startup input renderer and frame categories in-process" {
            let captured = ResizeArray<ViewerDiagnosticEvent>()

            let options =
                { Viewer.defaultDiagnostics with
                    Categories = Set.ofList [ ViewerDiagnosticCategory.Startup; Input; ViewerDiagnosticCategory.Renderer; ViewerDiagnosticCategory.Frame ]
                    FrameLogLimit = Some 1
                    Sink = Some captured.Add }

            let diagnostic category message frame =
                { Level = Info
                  Category = category
                  Message = message
                  FrameIndex = frame
                  Stage = None
                  Elapsed = Some TimeSpan.Zero }

            [ diagnostic ViewerDiagnosticCategory.Startup "window-created" None
              diagnostic Input "enter dispatched" None
              diagnostic ViewerDiagnosticCategory.Renderer "renderer-ready" None
              diagnostic ViewerDiagnosticCategory.Frame "frame 1 presented" (Some 1)
              diagnostic ViewerDiagnosticCategory.Frame "frame 2 presented" (Some 2) ]
            |> List.iter (Viewer.captureDiagnostic options >> ignore)

            let categories = captured |> Seq.map _.Category |> Set.ofSeq
            Expect.equal categories (Set.ofList [ ViewerDiagnosticCategory.Startup; Input; ViewerDiagnosticCategory.Renderer; ViewerDiagnosticCategory.Frame ]) "sink captures selected categories without stderr scraping"
            Expect.equal captured.Count 4 "frame sampling limits repeated per-frame sink messages"
            Expect.exists captured (fun item -> item.Message = "enter dispatched") "input diagnostic is capturable"
            Expect.exists captured (fun item -> item.Message = "renderer-ready") "renderer diagnostic is capturable"
        }

        test "viewer update emits categorized diagnostics for startup input scene frame and failure milestones" {
            let model, initEffects = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            Expect.exists initEffects (function EmitDiagnostic diagnostic when diagnostic.Category = ViewerDiagnosticCategory.Startup && diagnostic.Message.Contains "Product" -> true | _ -> false) "startup emits categorized diagnostic"

            let _, renderEffects = Viewer.update (Render(Group [])) model
            Expect.exists renderEffects (function EmitDiagnostic diagnostic when diagnostic.Category = ViewerDiagnosticCategory.Scene -> true | _ -> false) "scene render emits categorized diagnostic"

            let _, inputEffects =
                Viewer.update
                    (KeyEvent { RawKey = "Space"; Direction = ViewerKeyDirection.KeyDown })
                    model

            Expect.exists inputEffects (function EmitDiagnostic diagnostic when diagnostic.Category = Input && diagnostic.Message.Contains "Space" -> true | _ -> false) "input emits raw and normalized key diagnostic"

            let _, frameEffects = Viewer.update (FramePresented { Width = 640; Height = 480 }) model
            Expect.exists frameEffects (function EmitDiagnostic diagnostic when diagnostic.Category = ViewerDiagnosticCategory.Frame && diagnostic.Message.Contains "640x480" -> true | _ -> false) "frame milestone emits categorized diagnostic"

            let failure =
                { BlockedStage = ViewerRunBlockedStage.Swapchain
                  Classification = UnsupportedEnvironment
                  DiagnosticCategory = ViewerDiagnosticCategory.Swapchain
                  Message = "swapchain unavailable"
                  LastDiagnosticSummary = Some "swapchain unavailable" }

            let _, failureEffects = Viewer.update (RunFailed failure) model
            Expect.exists failureEffects (function EmitDiagnostic diagnostic when diagnostic.Category = ViewerDiagnosticCategory.Swapchain && diagnostic.Stage = Some ViewerRunBlockedStage.Swapchain -> true | _ -> false) "swapchain failure preserves category and stage"
        }
    ]
