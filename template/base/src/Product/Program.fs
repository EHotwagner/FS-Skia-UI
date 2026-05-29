module Product.Program

open System
open Product.Model
open Product.View
open Product.LayoutEvidence
//#if (profile == "governed" || profile == "headless-scene")

type Model = Product.Model.Model
type Msg = Product.Model.Msg
let initialModel = Product.Model.initialModel
let update = Product.Model.update
let view = Product.View.view
let layoutEvidenceForSize = Product.LayoutEvidence.layoutEvidenceForSize

[<EntryPoint>]
let main args =
    match Product.EvidenceCommands.tryRunEvidenceCommand (List.ofArray args) with
    | Some exitCode -> exitCode
    | None ->
        printfn "status=ok mode=headless-scene command=dotnet-run scene-nodes=1"
        0

//#else
open FS.Skia.UI.SkiaViewer
open System.IO
open Product.WindowOptions

type Model = Product.Model.Model
type Screen = Product.Model.Screen
type InputFlowDiagnostic = Product.Model.InputFlowDiagnostic
type Msg = Product.Model.Msg
type GeneratedLayoutValidationFailureClass = Product.Model.GeneratedLayoutValidationFailureClass
type GeneratedLayoutValidationResult = Product.Model.GeneratedLayoutValidationResult
type WindowBehaviorSettings = Product.WindowOptions.WindowBehaviorSettings

let initialModel = Product.Model.initialModel
let screenName = Product.Model.screenName
let keyName = Product.Model.keyName
let diagnostic = Product.Model.diagnostic
let transitionViewerInput = Product.Model.transitionViewerInput
let dispatchViewerKey = Product.Model.dispatchViewerKey
let visibleRows = Product.View.visibleRows
let init = Product.Model.init
let update = Product.Model.update
let subscriptions = Product.Model.subscriptions
let controlsExampleView = Product.View.controlsExampleView
let adapterProgram = Product.View.adapterProgram
let hudRegionForSize = Product.LayoutEvidence.hudRegionForSize
let gameplayRegionForSize = Product.LayoutEvidence.gameplayRegionForSize
let boundsInside = Product.LayoutEvidence.boundsInside
let activeGameplayBoundsForSize = Product.LayoutEvidence.activeGameplayBoundsForSize
let movementUsesGameplayRegion = Product.LayoutEvidence.movementUsesGameplayRegion
let spawnUsesGameplayRegion = Product.LayoutEvidence.spawnUsesGameplayRegion
let collisionUsesGameplayRegion = Product.LayoutEvidence.collisionUsesGameplayRegion
let layoutEvidenceForSize = Product.LayoutEvidence.layoutEvidenceForSize
let validateGeneratedLayout = Product.LayoutEvidence.validateGeneratedLayout
let view = Product.View.view
let mapKey = Product.EvidenceCommands.mapKey
let tick = Product.EvidenceCommands.tick
let viewerOptions = Product.EvidenceCommands.viewerOptions
let appCommandName = Product.EvidenceCommands.appCommandName
let viewerEffectsForModel = Product.EvidenceCommands.viewerEffectsForModel
let interpretAtHostBoundary = Product.EvidenceCommands.interpretAtHostBoundary
let generatedHost = Product.EvidenceCommands.generatedHost
let defaultCommand = Product.EvidenceCommands.defaultCommand
let windowBehaviorArgsFromFile = Product.WindowOptions.windowBehaviorArgsFromFile
let parseWindowBehavior = Product.WindowOptions.parseWindowBehavior
let toViewerWindowBehavior = Product.WindowOptions.toViewerWindowBehavior
let windowOptionStatusText = Product.WindowOptions.windowOptionStatusText
let manualWindowOptionResults = Product.WindowOptions.manualWindowOptionResults
let windowOptionsReport = Product.WindowOptions.windowOptionsReport

[<EntryPoint>]
let main args =
    match Product.EvidenceCommands.tryRunEvidenceCommand (List.ofArray args) with
    | Some exitCode -> exitCode
    | None ->
        let args = List.ofArray args
        let windowBehavior = parseWindowBehavior args
        let windowBehaviorRequest = toViewerWindowBehavior windowBehavior
        let capability = Viewer.runtimeCapability()
        let desktopSessionDiagnosticApi = "Viewer.desktopSessionDiagnostic()"

        let optional value =
            value |> Option.defaultValue "none"

        let envOption name =
            match Environment.GetEnvironmentVariable name with
            | null -> None
            | value when String.IsNullOrWhiteSpace value -> None
            | value -> Some value

        let runtimeDirectory = envOption "XDG_RUNTIME_DIR"
        let runtimeDirectoryExists = runtimeDirectory |> Option.exists Directory.Exists
        let waylandDisplay = envOption "WAYLAND_DISPLAY"
        let x11Display = envOption "DISPLAY"

        let displayVariable =
            match waylandDisplay, x11Display with
            | Some value, _ -> Some $"WAYLAND_DISPLAY={value}"
            | None, Some value -> Some $"DISPLAY={value}"
            | None, None -> None

        let displaySocket =
            if runtimeDirectory.IsSome && waylandDisplay.IsSome then
                Some(Path.Combine(runtimeDirectory.Value, waylandDisplay.Value))
            elif x11Display.IsSome then
                let display = x11Display.Value
                let number = display.TrimStart(':').Split('.').[0]
                Some($"/tmp/.X11-unix/X{number}")
            else
                None

        let displaySocketExists = displaySocket |> Option.exists File.Exists
        let sessionBus = envOption "DBUS_SESSION_BUS_ADDRESS"

        let diagnosticClass, desktopMessage =
            if runtimeDirectory.IsNone || displayVariable.IsNone || (displaySocket.IsSome && not displaySocketExists) then
                "unsupported-host", "Desktop session prerequisites are missing before app lifecycle debugging."
            else
                "environment-session-ready", "Desktop session prerequisites are present."

        let missingPackageCapability =
            if List.isEmpty capability.MissingPackageCapabilities then
                "none"
            else
                String.concat "," capability.MissingPackageCapabilities

        let unsupportedHostReasons =
            if List.isEmpty capability.UnsupportedHostReasons then
                "none"
            else
                String.concat "|" capability.UnsupportedHostReasons

        let fallbackFullDesktopSession = "fallback-is-full-desktop-session=false"

        let windowOptionResults =
            manualWindowOptionResults windowBehaviorRequest

        let windowOptionSummary =
            windowOptionResults
            |> List.map (fun (option, _, _, status, _) -> $"{option}:{windowOptionStatusText status}")
            |> String.concat ","

        match Viewer.runApp viewerOptions generatedHost with
        | Result.Ok outcome ->
            let inputDispatchStatus =
                match $"%A{outcome.InputDispatch}" with
                | "Verified"
                | "true" -> "verified"
                | "NotVerified"
                | "false" -> "not-verified"
                | value -> value.ToLowerInvariant()

            printfn "status=%s mode=%s command=%s window-opened=%b window-visible=observed:true accessible-window=true first-frame-presented=%b user-close-observed=%b self-closed-for-evidence=%b input-dispatch=%s exit-path=%b renderer-mode=%s blocked-stage=none classification=none category=none window-options=%s missing-package-capability=%s unsupported-host-reasons=%s diagnostic-api=%s diagnostic-class=%s runtime-directory=%s runtime-directory-exists=%b display-variable=%s display-socket-exists=%b session-bus=%s %s message=%s desktop-message=%s" outcome.Status outcome.Mode defaultCommand outcome.WindowOpened outcome.FirstFramePresented outcome.UserCloseObserved outcome.SelfClosedForEvidence inputDispatchStatus outcome.ExitPath outcome.RendererMode windowOptionSummary missingPackageCapability unsupportedHostReasons desktopSessionDiagnosticApi diagnosticClass (optional runtimeDirectory) runtimeDirectoryExists (optional displayVariable) displaySocketExists (optional sessionBus) fallbackFullDesktopSession outcome.Message desktopMessage
            0
        | Result.Error (failure: ViewerRunFailure) ->
            printfn "status=%s mode=interactive-window command=%s window-visible=unsupported accessible-window=false blocked-stage=%A classification=%A category=%A window-options=%s missing-package-capability=%s unsupported-host-reasons=%s diagnostic-api=%s diagnostic-class=%s runtime-directory=%s runtime-directory-exists=%b display-variable=%s display-socket-exists=%b session-bus=%s %s message=%s desktop-message=%s" (if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed") defaultCommand failure.BlockedStage failure.Classification failure.DiagnosticCategory windowOptionSummary missingPackageCapability unsupportedHostReasons desktopSessionDiagnosticApi diagnosticClass (optional runtimeDirectory) runtimeDirectoryExists (optional displayVariable) displaySocketExists (optional sessionBus) fallbackFullDesktopSession failure.Message desktopMessage
            if failure.Classification = UnsupportedEnvironment then 0 else 1
//#endif
