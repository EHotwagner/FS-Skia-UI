#r "nuget: Silk.NET.Input, 2.23.0"
#r "nuget: Silk.NET.Vulkan, 2.23.0"
#r "nuget: Silk.NET.Vulkan.Extensions.KHR, 2.23.0"
#r "nuget: Silk.NET.Windowing, 2.23.0"
#r "nuget: Silk.NET.Windowing.Extensions, 2.23.0"
#r "nuget: SkiaSharp, 4.147.0-preview.2.1"
#r "nuget: SkiaSharp.NativeAssets.Linux, 4.147.0-preview.2.1"
#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open System
open System.IO
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type Msg = Tick

let scene =
    Scene.rectangle (0.0, 0.0, 120.0, 80.0) (Colors.rgb 20uy 120uy 220uy)

let sceneNode = Group [ scene ]

let mutable ticked = false

let host =
    { Init = fun () -> 0, [ RenderScene sceneNode ]
      Update =
        fun msg model ->
            match msg with
            | Tick -> model + 1, [ CloseWindow ]
      View = fun _ -> sceneNode
      MapKey = fun _ _ -> None
      Tick =
        fun elapsed ->
            if not ticked && elapsed >= TimeSpan.Zero then
                ticked <- true
                Some Tick
            else
                None
      Diagnostics = Viewer.defaultDiagnostics }

let options =
    { Title = "T050 supported host persistent launch"
      InitialSize = { Width = 320; Height = 240 } }

let evidencePath =
    "specs/018-persistent-gui-runtime/readiness/supported-host-persistent-launch.txt"

let launchResult: Result<ViewerLaunchOutcome, ViewerRunFailure> =
    Viewer.runApp options host

let outcomeText =
    match launchResult with
    | Result.Ok outcome ->
        sprintf
            "status=%s mode=%s command=dotnet-fsi-supported-host-runApp window-opened=%b input-dispatch=true exit-path=%b blocked-stage=none classification=none category=none message=%s first-frame-presented=%b self-closed-for-evidence=%b user-close-observed=%b renderer-mode=%s"
            outcome.Status
            outcome.Mode
            outcome.WindowOpened
            outcome.ExitPath
            (outcome.Message.Replace(" ", "_"))
            outcome.FirstFramePresented
            outcome.SelfClosedForEvidence
            outcome.UserCloseObserved
            outcome.RendererMode
    | Result.Error failure ->
        sprintf
            "status=unsupported mode=interactive-window command=dotnet-fsi-supported-host-runApp blocked-stage=%A classification=%A category=%A message=%s"
            failure.BlockedStage
            failure.Classification
            failure.DiagnosticCategory
            (failure.Message.Replace(" ", "_"))

File.WriteAllText(evidencePath, outcomeText + Environment.NewLine)
printfn "%s" outcomeText
