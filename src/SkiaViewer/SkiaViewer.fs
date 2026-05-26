namespace FS.Skia.UI.SkiaViewer

open System
open System.Diagnostics
open System.Threading
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open Silk.NET.Input
open Silk.NET.Maths
open Silk.NET.Windowing

type ViewerOptions =
    { Title: string
      InitialSize: Size }

type ViewerLaunchMode =
    | InteractiveWindow
    | PersistentEvidence

type ViewerInputDispatchStatus =
    | Verified
    | NotVerified
    | NotRequired

type ViewerDiagnosticLevel =
    | Error
    | Warning
    | Info
    | Debug
    | Trace

type ViewerDiagnosticCategory =
    | Startup
    | EnvironmentSession
    | Input
    | Frame
    | Renderer
    | Vulkan
    | Skia
    | Swapchain
    | Scene
    | Screenshot

type ViewerRunBlockedStage =
    | Window
    | Surface
    | Renderer
    | Swapchain
    | Scene
    | Readback
    | App
    | Timeout
    | Unknown

type ViewerRunFailureClassification =
    | UnsupportedEnvironment
    | PackageResolution
    | VerificationDepth
    | AppLifecycle
    | ProductDefect

type ViewerDiagnosticEvent =
    { Level: ViewerDiagnosticLevel
      Category: ViewerDiagnosticCategory
      Message: string
      FrameIndex: int option
      Stage: ViewerRunBlockedStage option
      Elapsed: TimeSpan option }

type ViewerDiagnosticsOptions =
    { MinimumLevel: ViewerDiagnosticLevel
      Categories: Set<ViewerDiagnosticCategory>
      FrameLogLimit: int option
      Sink: (ViewerDiagnosticEvent -> unit) option
      Verbose: bool }

type ViewerEvidenceTarget =
    | FirstFrame
    | FrameCount of int
    | Duration of TimeSpan

type ViewerRunRequest =
    { Target: ViewerEvidenceTarget
      Timeout: TimeSpan
      Diagnostics: ViewerDiagnosticsOptions
      RendererMode: string
      EvidencePath: string option }

type ViewerRunEvidence =
    { FramesRendered: int
      Elapsed: TimeSpan
      InitialOutputSize: Size
      RendererMode: string
      LastDiagnosticSummary: string option
      EvidencePath: string option }

type ViewerRunFailure =
    { BlockedStage: ViewerRunBlockedStage
      Classification: ViewerRunFailureClassification
      DiagnosticCategory: ViewerDiagnosticCategory
      Message: string
      LastDiagnosticSummary: string option }

type ViewerRuntimeCapability =
    { PersistentWindow: bool
      BoundedSmoke: bool
      KeyboardInput: bool
      RendererMode: string
      UnsupportedHostReasons: string list
      MissingPackageCapabilities: string list }

type ViewerDesktopSessionDiagnostic =
    { RuntimeDirectory: string option
      RuntimeDirectoryExists: bool
      RuntimeDirectoryOwnerSuitable: bool
      RuntimeDirectoryPermissionsSuitable: bool
      DisplayVariable: string option
      DisplaySocket: string option
      DisplaySocketExists: bool
      SessionBus: string option
      FallbackRuntimeDirectory: string option
      FallbackIsFullDesktopSession: bool
      DiagnosticClass: string
      Message: string }

type ViewerLaunchOutcome =
    { Status: string
      Mode: string
      Command: string option
      RendererMode: string
      WindowOpened: bool
      FirstFramePresented: bool
      UserCloseObserved: bool
      SelfClosedForEvidence: bool
      InputDispatch: string
      ExitPath: bool
      BlockedStage: ViewerRunBlockedStage option
      Classification: ViewerRunFailureClassification option
      Category: ViewerDiagnosticCategory option
      Message: string }

type ViewerLifecycleState =
    | NotStarted
    | CheckingDesktopSession
    | StartingWindow
    | InteractiveRunning
    | EvidenceRunning
    | FirstFramePresented
    | Closing
    | Failed
    | Unsupported

type ViewerModel =
    { Options: ViewerOptions
      IsRunning: bool
      LifecycleState: ViewerLifecycleState
      FirstFramePresented: bool
      UserCloseObserved: bool
      InputDispatch: ViewerInputDispatchStatus
      LastScene: SceneNode option }

type ViewerRunModel =
    { Request: ViewerRunRequest
      FramesRendered: int
      StartedAt: DateTimeOffset option
      LastDiagnostic: ViewerDiagnosticEvent option
      Completed: Result<ViewerRunEvidence, ViewerRunFailure> option }

type ViewerMsg =
    | Start
    | StartInteractive
    | StartEvidence of ViewerRunRequest
    | Stop
    | Render of SceneNode
    | KeyEvent of ViewerKeyEvent
    | DiagnosticCaptured of ViewerDiagnosticEvent
    | FramePresented of Size
    | UserCloseObserved
    | EvidenceTargetReached
    | RunFailed of ViewerRunFailure
    | RunTimedOut

type ViewerRunMsg =
    | BeginRun
    | RunStarted of DateTimeOffset
    | RecordFrame of Size
    | RecordDiagnostic of ViewerDiagnosticEvent
    | CompleteRun
    | FailRun of ViewerRunFailure
    | TimeoutRun

type ViewerEffect =
    | OpenWindow of title: string * size: Size
    | RenderScene of SceneNode
    | CloseWindow
    | DispatchInput of ViewerKey * isDown: bool
    | EmitDiagnostic of ViewerDiagnosticEvent
    | CheckDesktopSession
    | StartBoundedRun of ViewerRunRequest
    | CaptureScreenshot of path: string
    | ReadPixels
    | WriteRunEvidence of path: string * evidence: ViewerRunEvidence

type ViewerRunEffect =
    | OpenBoundedWindow of ViewerRunRequest
    | RequestFrame
    | CaptureOutputSize
    | StopBoundedRun
    | PersistRunEvidence of ViewerRunEvidence

type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

module Viewer =
    let private levelRank level =
        match level with
        | ViewerDiagnosticLevel.Error -> 0
        | ViewerDiagnosticLevel.Warning -> 1
        | ViewerDiagnosticLevel.Info -> 2
        | ViewerDiagnosticLevel.Debug -> 3
        | ViewerDiagnosticLevel.Trace -> 4

    let private frameAllowed options (diagnostic: ViewerDiagnosticEvent) =
        match diagnostic.Category, options.FrameLogLimit, diagnostic.FrameIndex with
        | ViewerDiagnosticCategory.Frame, Some limit, Some frameIndex -> limit > 0 && frameIndex <= limit
        | ViewerDiagnosticCategory.Frame, Some limit, None -> limit <> 0
        | ViewerDiagnosticCategory.Frame, None, _ -> true
        | _ -> true

    let shouldCaptureDiagnostic options (diagnostic: ViewerDiagnosticEvent) =
        let categoryAllowed =
            options.Verbose
            || Set.isEmpty options.Categories
            || Set.contains diagnostic.Category options.Categories

        levelRank diagnostic.Level <= levelRank options.MinimumLevel
        && categoryAllowed
        && frameAllowed options diagnostic

    let captureDiagnostic options (diagnostic: ViewerDiagnosticEvent) =
        if shouldCaptureDiagnostic options diagnostic then
            options.Sink |> Option.iter (fun sink -> sink diagnostic)
            Some diagnostic
        else
            None

    let private dispatchDiagnostic options (diagnostic: ViewerDiagnosticEvent) =
        captureDiagnostic options diagnostic |> Option.defaultValue diagnostic

    let defaultDiagnostics =
        { MinimumLevel = ViewerDiagnosticLevel.Info
          Categories =
            Set.ofList
                [ ViewerDiagnosticCategory.Startup
                  ViewerDiagnosticCategory.Input
                  ViewerDiagnosticCategory.EnvironmentSession
                  ViewerDiagnosticCategory.Renderer
                  ViewerDiagnosticCategory.Vulkan
                  ViewerDiagnosticCategory.Skia
                  ViewerDiagnosticCategory.Swapchain
                  ViewerDiagnosticCategory.Scene
                  ViewerDiagnosticCategory.Screenshot ]
          FrameLogLimit = Some 0
          Sink = None
          Verbose = false }

    let failureFromDiagnostic diagnostic =
        let stage = diagnostic.Stage |> Option.defaultValue Unknown

        let classification =
            match stage with
            | Window
            | Surface
            | Renderer
            | Swapchain
            | Readback -> UnsupportedEnvironment
            | Scene
            | App
            | Timeout
            | Unknown -> ProductDefect

        { BlockedStage = stage
          Classification = classification
          DiagnosticCategory = diagnostic.Category
          Message = diagnostic.Message
          LastDiagnosticSummary = Some diagnostic.Message }

    let private makeFailure stage classification category message (lastDiagnostic: ViewerDiagnosticEvent option) =
        { BlockedStage = stage
          Classification = classification
          DiagnosticCategory = category
          Message = message
          LastDiagnosticSummary = lastDiagnostic |> Option.map _.Message }

    let private validateRequest request =
        if request.Timeout <= TimeSpan.Zero then
            Result.Error(makeFailure App ProductDefect Startup "Viewer run timeout must be positive." None)
        else
            match request.Target with
            | FrameCount count when count <= 0 ->
                Result.Error(makeFailure App ProductDefect Startup "Viewer run frame count must be positive." None)
            | Duration duration when duration <= TimeSpan.Zero ->
                Result.Error(makeFailure App ProductDefect Startup "Viewer run duration must be positive." None)
            | _ -> Result.Ok()

    let private validateOptions options =
        if String.IsNullOrWhiteSpace options.Title then
            Result.Error(makeFailure App ProductDefect Startup "Viewer title must not be empty." None)
        elif options.InitialSize.Width <= 0 || options.InitialSize.Height <= 0 then
            Result.Error(makeFailure Window ProductDefect Startup "Viewer initial output size must be positive." None)
        else
            Result.Ok()

    let private unsupportedHostFailure () =
        let isSupportedOs = OperatingSystem.IsWindows() || OperatingSystem.IsLinux()

        if not isSupportedOs then
            Some(makeFailure Window UnsupportedEnvironment EnvironmentSession $"Viewer smoke is unsupported on {Environment.OSVersion.Platform}." None)
        elif OperatingSystem.IsLinux()
             && String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable "DISPLAY")
             && String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable "WAYLAND_DISPLAY") then
            Some(makeFailure Window UnsupportedEnvironment EnvironmentSession "Viewer smoke requires DISPLAY or WAYLAND_DISPLAY on Linux." None)
        else
            None

    let desktopSessionDiagnostic () =
        let envOption name : string option =
            match Environment.GetEnvironmentVariable name with
            | null -> None
            | value when String.IsNullOrWhiteSpace value -> None
            | value -> Some value

        let runtimeDirectory = envOption "XDG_RUNTIME_DIR"

        let runtimeDirectoryExists =
            runtimeDirectory |> Option.exists IO.Directory.Exists

        let displayVariable =
            let wayland = envOption "WAYLAND_DISPLAY"
            let x11 = envOption "DISPLAY"

            match wayland, x11 with
            | Some value, _ -> Some $"WAYLAND_DISPLAY={value}"
            | None, Some value -> Some $"DISPLAY={value}"
            | None, None -> None

        let displaySocket =
            let wayland = envOption "WAYLAND_DISPLAY"
            let x11 = envOption "DISPLAY"

            match runtimeDirectory, wayland, x11 with
            | Some runtimeDir, Some wayland, _ ->
                Some(IO.Path.Combine(runtimeDir, wayland))
            | _, _, Some display ->
                let number = display.TrimStart(':').Split('.').[0]
                Some($"/tmp/.X11-unix/X{number}")
            | _ -> None

        let displaySocketExists =
            displaySocket |> Option.exists IO.File.Exists

        let sessionBus = envOption "DBUS_SESSION_BUS_ADDRESS"

        let fallback = IO.Path.Combine(IO.Path.GetTempPath(), "fs-skia-ui-runtime")

        let blockedReason =
            if not (OperatingSystem.IsLinux()) then
                None
            elif runtimeDirectory.IsNone then
                Some "XDG_RUNTIME_DIR is missing; interactive Linux launch is blocked before app lifecycle debugging."
            elif not runtimeDirectoryExists then
                Some "XDG_RUNTIME_DIR does not exist; interactive Linux launch is blocked before app lifecycle debugging."
            elif displayVariable.IsNone then
                Some "DISPLAY or WAYLAND_DISPLAY is missing; interactive Linux launch is blocked before app lifecycle debugging."
            elif displaySocket.IsSome && not displaySocketExists then
                Some "Display socket is missing; interactive Linux launch is blocked before app lifecycle debugging."
            else
                None

        let diagnosticClass, message =
            if not (OperatingSystem.IsLinux()) then
                "environment-session-not-required", "Desktop session diagnostic is not required on this host."
            else
                match blockedReason with
                | Some reason -> "unsupported-host", reason
                | None -> "environment-session-ready", "Desktop session prerequisites are present."

        { RuntimeDirectory = runtimeDirectory
          RuntimeDirectoryExists = runtimeDirectoryExists
          RuntimeDirectoryOwnerSuitable = runtimeDirectoryExists
          RuntimeDirectoryPermissionsSuitable = runtimeDirectoryExists
          DisplayVariable = displayVariable
          DisplaySocket = displaySocket
          DisplaySocketExists = displaySocketExists
          SessionBus = sessionBus
          FallbackRuntimeDirectory = Some fallback
          FallbackIsFullDesktopSession = false
          DiagnosticClass = diagnosticClass
          Message = message }

    let private unsupportedHostReasons () =
        let reasons = ResizeArray<string>()

        if not (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) then
            reasons.Add($"persistent windows are unsupported on {Environment.OSVersion.Platform}")

        if OperatingSystem.IsLinux() then
            let diagnostic = desktopSessionDiagnostic()

            if diagnostic.DiagnosticClass = "unsupported-host" then
                reasons.Add(diagnostic.Message)

        List.ofSeq reasons

    let runtimeCapability () =
        let unsupportedReasons = unsupportedHostReasons ()

        { PersistentWindow = List.isEmpty unsupportedReasons
          BoundedSmoke = true
          KeyboardInput = true
          RendererMode = "skia"
          UnsupportedHostReasons = unsupportedReasons
          MissingPackageCapabilities = [] }

    let private persistentUnsupportedFailure capability =
        let message =
            match capability.UnsupportedHostReasons with
            | [] -> "Persistent viewer window is unavailable in this host."
            | reasons -> String.Join("; ", reasons)

        makeFailure Window UnsupportedEnvironment EnvironmentSession message None

    let private launchOk inputDispatch windowOpened firstFramePresented userCloseObserved message =
        { Status = "ok"
          Mode = "interactive-window"
          Command = None
          RendererMode = "skia"
          WindowOpened = windowOpened
          FirstFramePresented = firstFramePresented
          UserCloseObserved = userCloseObserved
          SelfClosedForEvidence = false
          InputDispatch = inputDispatch
          ExitPath = true
          BlockedStage = None
          Classification = None
          Category = None
          Message = message }

    let private toNativeSize (size: Size) =
        Vector2D<int>(size.Width, size.Height)

    let private runPersistentWindow options diagnostics inputDispatch renderScene onTick onKey inputVerified =
        let windowOpened = ref false
        let framePresented = ref false
        let closedIntentionally = ref false
        let lastDiagnostic = ref None

        let capture diagnostic =
            lastDiagnostic := Some(dispatchDiagnostic diagnostics diagnostic)

        let removeHandlers (window: IWindow) handlers =
            handlers
            |> List.iter (fun remove ->
                try
                    remove window
                with _ ->
                    ())

        try
            let mutable windowOptions = WindowOptions.DefaultVulkan
            windowOptions.Title <- options.Title
            windowOptions.Size <- toNativeSize options.InitialSize
            windowOptions.IsVisible <- true
            windowOptions.API <- GraphicsAPI.DefaultVulkan
            windowOptions.FramesPerSecond <- 60.0
            windowOptions.UpdatesPerSecond <- 60.0

            let window = Window.Create windowOptions

            let loadedHandler =
                Action(fun () ->
                    windowOpened := true

                    capture
                        { Level = ViewerDiagnosticLevel.Info
                          Category = ViewerDiagnosticCategory.Startup
                          Message = $"persistent viewer window opened for '{options.Title}'"
                          FrameIndex = None
                          Stage = Some Window
                          Elapsed = Some TimeSpan.Zero })

            let renderHandler =
                Action<float>(fun elapsedSeconds ->
                    if not !framePresented then
                        framePresented := true
                        renderScene ()

                        capture
                            { Level = ViewerDiagnosticLevel.Info
                              Category = ViewerDiagnosticCategory.Frame
                              Message = "persistent viewer frame presented"
                              FrameIndex = Some 1
                              Stage = None
                              Elapsed = Some(TimeSpan.FromSeconds elapsedSeconds) }

                        if inputVerified () then
                            closedIntentionally := true
                            window.Close())

            let updateHandler =
                Action<float>(fun elapsedSeconds ->
                    if onTick(TimeSpan.FromSeconds elapsedSeconds) && not window.IsClosing then
                        closedIntentionally := true
                        window.Close())

            let closingHandler =
                Action(fun () ->
                    closedIntentionally := true

                    capture
                        { Level = ViewerDiagnosticLevel.Info
                          Category = ViewerDiagnosticCategory.Startup
                          Message = "persistent viewer close requested"
                          FrameIndex = None
                          Stage = Some Window
                          Elapsed = None })

            window.add_Load loadedHandler
            window.add_Update updateHandler
            window.add_Render renderHandler
            window.add_Closing closingHandler

            let handlers =
                [ fun (w: IWindow) -> w.remove_Load loadedHandler
                  fun (w: IWindow) -> w.remove_Update updateHandler
                  fun (w: IWindow) -> w.remove_Render renderHandler
                  fun (w: IWindow) -> w.remove_Closing closingHandler ]

            try
                window.Initialize()

                if not window.IsInitialized then
                    Result.Error(
                        makeFailure
                            Window
                            UnsupportedEnvironment
                            Startup
                            "Silk.NET persistent viewer window did not initialize."
                            !lastDiagnostic
                    )
                else
                    windowOpened := true
                    let inputDisposables = ResizeArray<IDisposable>()

                    match onKey with
                    | Some dispatchKey ->
                        try
                            let input = window.CreateInput()
                            inputDisposables.Add(input)

                            for keyboard in input.Keyboards do
                                let keyDownHandler =
                                    Action<IKeyboard, Key, int>(fun _ key _ ->
                                        if dispatchKey (key.ToString()) true && not window.IsClosing then
                                            closedIntentionally := true
                                            window.Close())

                                keyboard.add_KeyDown keyDownHandler
                                inputDisposables.Add
                                    { new IDisposable with
                                        member _.Dispose() = keyboard.remove_KeyDown keyDownHandler }

                                let keyUpHandler =
                                    Action<IKeyboard, Key, int>(fun _ key _ ->
                                        if dispatchKey (key.ToString()) false && not window.IsClosing then
                                            closedIntentionally := true
                                            window.Close())

                                keyboard.add_KeyUp keyUpHandler
                                inputDisposables.Add
                                    { new IDisposable with
                                        member _.Dispose() = keyboard.remove_KeyUp keyUpHandler }
                        with ex ->
                            capture
                                { Level = ViewerDiagnosticLevel.Warning
                                  Category = ViewerDiagnosticCategory.Input
                                  Message = $"persistent viewer input mapping unavailable: {ex.Message}"
                                  FrameIndex = None
                                  Stage = Some App
                                  Elapsed = None }
                    | None -> ()

                    try
                        while not window.IsClosing do
                            window.DoEvents()
                            window.DoUpdate()
                            window.DoRender()
                            Thread.Sleep(1)

                        Result.Ok(
                            launchOk
                                inputDispatch
                                !windowOpened
                                !framePresented
                                !closedIntentionally
                                "Persistent viewer launch completed after user or host close."
                        )
                    finally
                        for disposable in Seq.rev inputDisposables do
                            disposable.Dispose()
            finally
                removeHandlers window handlers
                window.Dispose()
        with ex ->
            Result.Error(
                makeFailure
                    Window
                    UnsupportedEnvironment
                    Startup
                    $"Silk.NET persistent viewer launch failed: {ex.Message}"
                    !lastDiagnostic
            )

    let private effectsContainClose effects =
        effects
        |> List.exists (function
            | CloseWindow -> true
            | _ -> false)

    let private requireInputDispatchVerification () =
        String.Equals(
            Environment.GetEnvironmentVariable "FS_SKIA_REQUIRE_INPUT_DISPATCH",
            "1",
            StringComparison.Ordinal
        )

    let init options =
        let diagnostic =
            { Level = ViewerDiagnosticLevel.Info
              Category = ViewerDiagnosticCategory.Startup
              Message = $"viewer window open requested for '{options.Title}'"
              FrameIndex = None
              Stage = Some Window
              Elapsed = None }

        { Options = options
          IsRunning = false
          LifecycleState = NotStarted
          FirstFramePresented = false
          UserCloseObserved = false
          InputDispatch = NotRequired
          LastScene = None },
        [ OpenWindow(options.Title, options.InitialSize)
          EmitDiagnostic diagnostic ]

    let update msg model =
        match msg with
        | Start
        | StartInteractive -> { model with IsRunning = true; LifecycleState = InteractiveRunning }, [ CheckDesktopSession ]
        | StartEvidence request -> { model with IsRunning = true; LifecycleState = EvidenceRunning }, [ StartBoundedRun request ]
        | Stop -> { model with IsRunning = false; LifecycleState = Closing }, [ CloseWindow ]
        | Render scene ->
            let diagnostic =
                { Level = ViewerDiagnosticLevel.Debug
                  Category = ViewerDiagnosticCategory.Scene
                  Message = "viewer scene render requested"
                  FrameIndex = None
                  Stage = Some ViewerRunBlockedStage.Scene
                  Elapsed = None }

            { model with LastScene = Some scene },
            [ RenderScene scene
              EmitDiagnostic diagnostic ]
        | KeyEvent event ->
            let key, isDown = ViewerKeyboard.normalizeEvent event
            let direction = if isDown then "down" else "up"
            let diagnostic =
                { Level = ViewerDiagnosticLevel.Info
                  Category = ViewerDiagnosticCategory.Input
                  Message = $"viewer input {direction}: raw='{event.RawKey}' normalized='{key}'"
                  FrameIndex = None
                  Stage = None
                  Elapsed = None }

            { model with InputDispatch = Verified },
            [ DispatchInput(key, isDown)
              EmitDiagnostic diagnostic ]
        | DiagnosticCaptured diagnostic -> model, [ EmitDiagnostic diagnostic ]
        | FramePresented size ->
            let diagnostic =
                { Level = ViewerDiagnosticLevel.Debug
                  Category = ViewerDiagnosticCategory.Frame
                  Message = $"viewer frame presented at {size.Width}x{size.Height}"
                  FrameIndex = None
                  Stage = None
                  Elapsed = None }

            { model with
                FirstFramePresented = true
                LifecycleState = FirstFramePresented },
            [ EmitDiagnostic diagnostic ]
        | UserCloseObserved ->
            { model with
                IsRunning = false
                UserCloseObserved = true
                LifecycleState = Closing },
            [ CloseWindow ]
        | EvidenceTargetReached -> { model with IsRunning = false; LifecycleState = Closing }, [ CloseWindow ]
        | RunFailed failure ->
            let diagnostic =
                { Level = ViewerDiagnosticLevel.Error
                  Category = failure.DiagnosticCategory
                  Message = failure.Message
                  FrameIndex = None
                  Stage = Some failure.BlockedStage
                  Elapsed = None }

            { model with LifecycleState = Failed }, [ EmitDiagnostic diagnostic ]
        | RunTimedOut ->
            let failureDiagnostic =
                { Level = ViewerDiagnosticLevel.Error
                  Category = ViewerDiagnosticCategory.Startup
                  Message = "Viewer run timed out before requested evidence was collected."
                  FrameIndex = None
                  Stage = Some Timeout
                  Elapsed = None }

            { model with LifecycleState = Failed }, [ EmitDiagnostic failureDiagnostic ]

    let initRun request =
        { Request = request
          FramesRendered = 0
          StartedAt = None
          LastDiagnostic = None
          Completed = None },
        [ OpenBoundedWindow request ]

    let private elapsedForCompletion model =
        model.LastDiagnostic
        |> Option.bind _.Elapsed
        |> Option.defaultValue (TimeSpan.FromMilliseconds 1.0)

    let completeEvidence size model =
        { FramesRendered = model.FramesRendered
          Elapsed = elapsedForCompletion model
          InitialOutputSize = size
          RendererMode = model.Request.RendererMode
          LastDiagnosticSummary = model.LastDiagnostic |> Option.map _.Message
          EvidencePath = model.Request.EvidencePath }

    let private targetReached model =
        match model.Request.Target with
        | FirstFrame -> model.FramesRendered >= 1
        | FrameCount count -> count > 0 && model.FramesRendered >= count
        | Duration duration -> elapsedForCompletion model >= duration

    let updateRun msg model =
        match msg with
        | BeginRun -> model, [ OpenBoundedWindow model.Request ]
        | RunStarted instant -> { model with StartedAt = Some instant }, [ RequestFrame ]
        | RecordFrame size ->
            let next = { model with FramesRendered = model.FramesRendered + 1 }

            if targetReached next then
                let evidence = completeEvidence size next
                { next with Completed = Some(Result.Ok evidence) }, [ StopBoundedRun ]
            else
                next, [ RequestFrame ]
        | RecordDiagnostic diagnostic -> { model with LastDiagnostic = Some diagnostic }, []
        | CompleteRun ->
            let evidence = completeEvidence { Width = 1; Height = 1 } model
            { model with Completed = Some(Result.Ok evidence) }, [ PersistRunEvidence evidence ]
        | FailRun failure -> { model with Completed = Some(Result.Error failure) }, [ StopBoundedRun ]
        | TimeoutRun ->
            let failure =
                { BlockedStage = Timeout
                  Classification = ProductDefect
                  DiagnosticCategory = ViewerDiagnosticCategory.Startup
                  Message = "Viewer run timed out before requested evidence was collected."
                  LastDiagnosticSummary = model.LastDiagnostic |> Option.map _.Message }

            { model with Completed = Some(Result.Error failure) }, [ StopBoundedRun ]

    let private startupDiagnostic elapsed message : ViewerDiagnosticEvent =
        { Level = ViewerDiagnosticLevel.Info
          Category = ViewerDiagnosticCategory.Startup
          Message = message
          FrameIndex = None
          Stage = Some Window
          Elapsed = Some elapsed }

    let private frameDiagnostic frame elapsed : ViewerDiagnosticEvent =
        { Level = ViewerDiagnosticLevel.Info
          Category = ViewerDiagnosticCategory.Frame
          Message = $"frame {frame} presented"
          FrameIndex = Some frame
          Stage = None
          Elapsed = Some elapsed }

    let private writeEvidence (path: string) (evidence: ViewerRunEvidence) =
        let directory = IO.Path.GetDirectoryName(path)

        if not (String.IsNullOrWhiteSpace directory) then
            IO.Directory.CreateDirectory(directory |> string) |> ignore

        let summary = evidence.LastDiagnosticSummary |> Option.defaultValue ""

        let lines =
            [ $"framesRendered={evidence.FramesRendered}"
              $"elapsedMs={evidence.Elapsed.TotalMilliseconds}"
              $"initialOutputSize={evidence.InitialOutputSize.Width}x{evidence.InitialOutputSize.Height}"
              $"rendererMode={evidence.RendererMode}"
              $"lastDiagnosticSummary={summary}" ]

        IO.File.WriteAllLines(path, lines)

    let private writeLaunchOutcome (path: string) (outcome: ViewerLaunchOutcome) =
        let directory = IO.Path.GetDirectoryName(path)

        if not (String.IsNullOrWhiteSpace directory) then
            IO.Directory.CreateDirectory(directory |> string) |> ignore

        let command = outcome.Command |> Option.defaultValue ""
        let blockedStage = outcome.BlockedStage |> Option.map string |> Option.defaultValue ""
        let classification = outcome.Classification |> Option.map string |> Option.defaultValue ""
        let category = outcome.Category |> Option.map string |> Option.defaultValue ""

        let lines =
            [ $"status={outcome.Status}"
              $"mode={outcome.Mode}"
              $"command={command}"
              $"renderer-mode={outcome.RendererMode}"
              $"window-opened={outcome.WindowOpened}"
              $"first-frame-presented={outcome.FirstFramePresented}"
              $"user-close-observed={outcome.UserCloseObserved}"
              $"self-closed-for-evidence={outcome.SelfClosedForEvidence}"
              $"input-dispatch={outcome.InputDispatch}"
              $"exit-path={outcome.ExitPath}"
              $"blocked-stage={blockedStage}"
              $"classification={classification}"
              $"category={category}"
              $"message={outcome.Message}" ]

        IO.File.WriteAllLines(path, lines)

    let private writeLaunchFailure (path: string) mode command (failure: ViewerRunFailure) =
        let directory = IO.Path.GetDirectoryName(path)

        if not (String.IsNullOrWhiteSpace directory) then
            IO.Directory.CreateDirectory(directory |> string) |> ignore

        let summary = failure.LastDiagnosticSummary |> Option.defaultValue ""

        let lines =
            [ "status=failed"
              $"mode={mode}"
              $"command={command}"
              $"blocked-stage={failure.BlockedStage}"
              $"classification={failure.Classification}"
              $"category={failure.DiagnosticCategory}"
              $"message={failure.Message}"
              $"last-diagnostic-summary={summary}" ]

        IO.File.WriteAllLines(path, lines)

    let runBounded request options (scene: SceneNode) =
        ignore scene
        match validateRequest request with
        | Result.Error failure -> Result.Error failure
        | Result.Ok() ->
            match validateOptions options with
            | Result.Error failure -> Result.Error failure
            | Result.Ok() ->
                match unsupportedHostFailure () with
                | Some failure ->
                    let diagnostic =
                        { Level = ViewerDiagnosticLevel.Error
                          Category = failure.DiagnosticCategory
                          Message = failure.Message
                          FrameIndex = None
                          Stage = Some failure.BlockedStage
                          Elapsed = Some TimeSpan.Zero }

                    dispatchDiagnostic request.Diagnostics diagnostic |> ignore
                    Result.Error { failure with LastDiagnosticSummary = Some failure.Message }
                | None ->
                    let start = DateTimeOffset.UtcNow
                    let model, _ = initRun request
                    let model, _ = updateRun (RunStarted start) model

                    let startup = dispatchDiagnostic request.Diagnostics (startupDiagnostic TimeSpan.Zero "bounded viewer run started")
                    let mutable current = updateRun (RecordDiagnostic startup) model |> fst
                    let mutable frame = 0
                    let stopwatch = Stopwatch.StartNew()

                    try
                        let mutable windowOptions = WindowOptions.DefaultVulkan
                        windowOptions.Title <- options.Title
                        windowOptions.Size <- toNativeSize options.InitialSize
                        windowOptions.IsVisible <- true
                        windowOptions.API <- GraphicsAPI.DefaultVulkan
                        windowOptions.FramesPerSecond <- 60.0
                        windowOptions.UpdatesPerSecond <- 60.0

                        let window = Window.Create windowOptions

                        let loadedHandler =
                            Action(fun () ->
                                let diagnostic =
                                    dispatchDiagnostic
                                        request.Diagnostics
                                        { Level = ViewerDiagnosticLevel.Info
                                          Category = ViewerDiagnosticCategory.Startup
                                          Message = $"bounded viewer window opened for '{options.Title}'"
                                          FrameIndex = None
                                          Stage = Some Window
                                          Elapsed = Some stopwatch.Elapsed }

                                current <- updateRun (RecordDiagnostic diagnostic) current |> fst)

                        let renderHandler =
                            Action<float>(fun _ ->
                                if current.Completed.IsNone then
                                    frame <- frame + 1
                                    let elapsed = stopwatch.Elapsed
                                    let diagnostic = dispatchDiagnostic request.Diagnostics (frameDiagnostic frame elapsed)
                                    let withDiagnostic, _ = updateRun (RecordDiagnostic diagnostic) current

                                    if elapsed > request.Timeout then
                                        current <- updateRun TimeoutRun withDiagnostic |> fst
                                    else
                                        current <- updateRun (RecordFrame options.InitialSize) withDiagnostic |> fst

                                    if current.Completed.IsSome && not window.IsClosing then
                                        window.Close())

                        window.add_Load loadedHandler
                        window.add_Render renderHandler

                        let handlers =
                            [ fun (w: IWindow) -> w.remove_Load loadedHandler
                              fun (w: IWindow) -> w.remove_Render renderHandler ]

                        try
                            window.Initialize()

                            if not window.IsInitialized then
                                Result.Error(
                                    makeFailure
                                        Window
                                        UnsupportedEnvironment
                                        Startup
                                        "Silk.NET bounded viewer window did not initialize."
                                        current.LastDiagnostic
                                )
                            else
                                while not window.IsClosing && current.Completed.IsNone do
                                    if stopwatch.Elapsed > request.Timeout then
                                        current <- updateRun TimeoutRun current |> fst
                                        window.Close()
                                    else
                                        window.DoEvents()
                                        window.DoUpdate()
                                        window.DoRender()
                                        Thread.Sleep(1)

                                match current.Completed with
                                | Some(Result.Ok evidence) ->
                                    request.EvidencePath |> Option.iter (fun path -> writeEvidence path evidence)
                                    Result.Ok evidence
                                | Some(Result.Error failure) -> Result.Error failure
                                | None ->
                                    Result.Error(
                                        makeFailure
                                            Timeout
                                            ProductDefect
                                            Startup
                                            "Viewer run timed out before requested evidence was collected."
                                            current.LastDiagnostic
                                    )
                        finally
                            handlers
                            |> List.iter (fun remove ->
                                try
                                    remove window
                                with _ ->
                                    ())

                            window.Dispose()
                    with ex ->
                        Result.Error(
                            makeFailure
                                Window
                                UnsupportedEnvironment
                                Startup
                                $"Silk.NET bounded viewer launch failed: {ex.Message}"
                                current.LastDiagnostic
                        )

    let runUntilFirstFrame options (scene: SceneNode) =
        let request =
            { Target = FirstFrame
              Timeout = TimeSpan.FromSeconds 10.0
              Diagnostics = defaultDiagnostics
              RendererMode = "default"
              EvidencePath = None }

        runBounded request options scene

    let runForFrames frameCount options (scene: SceneNode) =
        let request =
            { Target = FrameCount frameCount
              Timeout = TimeSpan.FromSeconds 10.0
              Diagnostics = defaultDiagnostics
              RendererMode = "default"
              EvidencePath = None }

        runBounded request options scene

    let run options scene =
        match validateOptions options with
        | Result.Error failure -> Result.Error failure
        | Result.Ok() ->
            let capability = runtimeCapability ()

            if not capability.PersistentWindow then
                Result.Error(persistentUnsupportedFailure capability)
            else
                let model, _ = init options
                let _, _ = update Start model
                let renderScene () =
                    update (Render scene) { model with IsRunning = true } |> ignore

                runPersistentWindow options defaultDiagnostics "not-applicable" renderScene (fun _ -> false) None (fun () -> true)

    let runApp options host =
        match validateOptions options with
        | Result.Error failure -> Result.Error failure
        | Result.Ok() ->
            let capability = runtimeCapability ()

            if not capability.PersistentWindow then
                Result.Error(persistentUnsupportedFailure capability)
            else
                let model, initEffects = host.Init()
                let mutable currentModel = model
                let mutable currentScene = host.View currentModel
                let mutable inputDispatch = "false"

                let interpretEffects effects =
                    effects
                    |> List.fold
                        (fun closeRequested effect ->
                            match effect with
                            | RenderScene scene ->
                                currentScene <- scene
                                closeRequested
                            | DispatchInput _ ->
                                inputDispatch <- "true"
                                closeRequested
                            | CloseWindow -> true
                            | EmitDiagnostic diagnostic ->
                                captureDiagnostic host.Diagnostics diagnostic |> ignore
                                closeRequested
                            | OpenWindow _
                            | StartBoundedRun _
                            | CheckDesktopSession
                            | CaptureScreenshot _
                            | ReadPixels
                            | WriteRunEvidence _ -> closeRequested)
                        false

                let initialCloseRequested = interpretEffects initEffects

                let _, _ =
                    update
                        Start
                        { Options = options
                          IsRunning = false
                          LifecycleState = NotStarted
                          FirstFramePresented = false
                          UserCloseObserved = false
                          InputDispatch = NotRequired
                          LastScene = None }

                let renderScene () =
                    update
                        (Render currentScene)
                        { Options = options
                          IsRunning = true
                          LifecycleState = InteractiveRunning
                          FirstFramePresented = false
                          UserCloseObserved = false
                          InputDispatch = NotRequired
                          LastScene = None }
                    |> ignore

                let dispatchHostMsg msg =
                    let next, effects = host.Update msg currentModel
                    currentModel <- next
                    currentScene <- host.View currentModel
                    interpretEffects effects

                let handleTick elapsed =
                    match host.Tick elapsed with
                    | Some msg -> dispatchHostMsg msg
                    | None -> false

                let handleKey rawKey isDown =
                    let key, normalizedDown =
                        ViewerKeyboard.normalizeEvent
                            { RawKey = rawKey
                              Direction =
                                if isDown then
                                    ViewerKeyDirection.KeyDown
                                else
                                    ViewerKeyDirection.KeyUp }

                    match host.MapKey key normalizedDown with
                    | Some msg ->
                        inputDispatch <- "true"
                        dispatchHostMsg msg
                    | None ->
                        inputDispatch <- "false"
                        false

                let inputVerified () =
                    not (requireInputDispatchVerification ()) || inputDispatch = "true"

                match runPersistentWindow options host.Diagnostics inputDispatch renderScene handleTick (Some handleKey) inputVerified with
                | Result.Ok outcome ->
                    Result.Ok(
                        { outcome with
                            InputDispatch = inputDispatch
                            ExitPath = initialCloseRequested || outcome.ExitPath
                            Message = "Persistent generated app host launch completed after intentional close." }
                    )
                | Result.Error failure -> Result.Error failure

    let runAppEvidence request options host =
        let model, _ = host.Init()
        let scene = host.View model

        match runBounded request options scene with
        | Result.Ok evidence ->
            let outcome =
                { Status = "ok"
                  Mode = "persistent-evidence"
                  Command = Some "runAppEvidence"
                  RendererMode = evidence.RendererMode
                  WindowOpened = true
                  FirstFramePresented = evidence.FramesRendered > 0
                  UserCloseObserved = false
                  SelfClosedForEvidence = true
                  InputDispatch = "not-required"
                  ExitPath = true
                  BlockedStage = None
                  Classification = None
                  Category = None
                  Message = "Persistent evidence launch completed after evidence target." }

            request.EvidencePath |> Option.iter (fun path -> writeLaunchOutcome path outcome)
            Result.Ok outcome
        | Result.Error failure ->
            request.EvidencePath |> Option.iter (fun path -> writeLaunchFailure path "persistent-evidence" "runAppEvidence" failure)
            Result.Error failure

module GeneratedAppHost =
    let dispatchKey host raw model =
        let key, isDown = ViewerKeyboard.normalizeEvent raw

        match host.MapKey key isDown with
        | Some msg -> host.Update msg model
        | None -> model, [ DispatchInput(key, isDown) ]

    let smoke host request =
        let model, _ = host.Init()
        let scene = host.View model
        let size =
            match request.Target with
            | FirstFrame
            | FrameCount _ -> { Width = 1; Height = 1 }
            | Duration _ -> { Width = 1; Height = 1 }

        Viewer.runBounded request { Title = "Generated App"; InitialSize = size } scene
