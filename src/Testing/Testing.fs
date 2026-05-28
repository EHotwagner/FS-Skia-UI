namespace FS.Skia.UI.Testing

open System
open FS.Skia.UI.Scene

type PackageReferenceExpectation =
    { PackageId: string
      Required: bool }

type GeneratedProductExpectation =
    { Profile: string
      RequiredFiles: string list
      ForbiddenPrefixes: string list
      PackageReferences: PackageReferenceExpectation list }

type LocalConsumerPackage =
    { PackageId: string
      Version: string
      FeedPath: string }

type LocalConsumerPackageDrift =
    { PackageId: string
      ExpectedVersion: string
      ActualVersion: string option
      FeedPath: string
      RemediationCommand: string }

type LocalConsumerPackageReport =
    { FeedPath: string
      Packages: LocalConsumerPackage list
      ConsumerConfigSnippet: string
      NuGetConfigSnippet: string option
      RestoreCommand: string
      DriftDiagnostics: LocalConsumerPackageDrift list }

type GeneratedValidationCategory =
    | PackageDrift
    | RestoreFailure
    | SemanticTestFailure
    | ViewerStartupFailure
    | UnsupportedHost
    | SceneEvidenceFailure
    | Completed

type GeneratedValidationResult =
    { Category: GeneratedValidationCategory
      Elapsed: TimeSpan
      CommandContext: string
      EvidencePath: string option
      Diagnostics: string list }

type GeneratedProductLaunchValidationResult =
    { InteractiveLaunchRequired: bool
      Diagnostics: string list }

type GeneratedWindowDiagnosticCheck =
    { Output: string
      RequiredFailureClasses: string list
      RequiredNativeFacts: string list }

type GeneratedWindowDiagnosticValidationResult =
    { DiagnosticsComplete: bool
      Diagnostics: string list }

type PackageResolutionCheck =
    { RequestedPackages: LocalConsumerPackage list
      ResolvedPackages: LocalConsumerPackage list
      PackageSources: string list
      RestoreWarnings: string list }

type PackageResolutionCheckResult =
    { ExactMatch: bool
      FailureReason: string option
      Diagnostics: string list }

type GeneratedTestExecutionCheck =
    { TestsExist: bool
      TestsRan: bool
      VerifyRan: bool }

type GeneratedTestExecutionResult =
    { Authoritative: bool
      NonAuthoritativeReason: string option
      Diagnostics: string list }

type VisualEvidenceKind =
    | Screenshot
    | PixelReadback
    | UnsupportedHost

type VisualEvidenceRequest =
    { ScreenshotAvailable: bool
      PixelReadbackAvailable: bool
      BoardReadable: bool option
      InputOrProgressObserved: bool option
      UnsupportedReason: string option }

type VisualEvidenceResult =
    { EvidenceKind: VisualEvidenceKind
      BoardReadable: bool option
      InputOrProgressObserved: bool option
      FallbackReason: string option
      UnsupportedReason: string option
      Diagnostics: string list }

type GeneratedVisualEvidenceCommandCheck =
    { Output: string
      RequestedImageEvidence: bool }

type GeneratedVisualEvidenceCommandResult =
    { Accepted: bool
      EvidenceKind: string option
      FailureReason: string option
      Diagnostics: string list }

type GeneratedValidationContractCheck =
    { PackageResolution: PackageResolutionCheckResult
      GeneratedTests: GeneratedTestExecutionResult
      DefaultInteractiveLaunch: GeneratedProductLaunchValidationResult
      BoundedEvidenceValidated: bool
      CloseReasonValidated: bool
      WindowDiagnostics: GeneratedWindowDiagnosticValidationResult
      WindowOptionsValidated: bool
      ImageEvidence: GeneratedVisualEvidenceCommandResult }

type GeneratedValidationContractResult =
    { Output: string
      Authoritative: bool
      FailureClass: string
      Diagnostics: string list }

type GeneratedLayoutValidationFailureClass =
    | MissingLayoutFacts
    | UnsupportedLayoutFacts
    | OverlappingLayoutBounds
    | DeterministicRenderOnlyClaim

type GeneratedLayoutValidationCheck =
    { Report: LayoutEvidenceReport
      RequireReadableLayout: bool }

type GeneratedLayoutValidationResult =
    { Accepted: bool
      FailureClass: GeneratedLayoutValidationFailureClass option
      Diagnostics: string list }

type HostWarningClass =
    | BenignEnvironmentWarning
    | LaunchFailure
    | RenderingFailure
    | LayoutFailure
    | PackageFailure
    | UnknownWarning

type HostWarningClassificationCheck =
    { RawMessage: string
      KnownBenignMarkers: string list
      LaunchSucceeded: bool
      RenderingSucceeded: bool
      LayoutReadable: bool option
      ExplicitlyUnsupportedWithoutReadabilityClaim: bool
      PackageSucceeded: bool
      EvidencePath: string option }

type HostWarningClassificationResult =
    { WarningClass: HostWarningClass
      RawMessage: string
      Fatal: bool
      EvidencePath: string option
      SupportingFacts: string list
      Diagnostics: string list }

type PersistentLaunchArtifactCheck =
    { ArtifactPath: string
      Lines: string list
      SyntheticFixture: bool
      SupportedHostPassClaimed: bool }

type PersistentLaunchArtifactValidationResult =
    { Accepted: bool
      MissingFields: string list
      Contradictions: string list
      Diagnostics: string list }

type ReadinessFileDiscoveryCheck =
    { ReadinessDirectory: string
      RequiredFiles: string list
      ExistingFiles: string list }

type ReadinessFileDiscoveryResult =
    { Complete: bool
      MissingFiles: string list
      Diagnostics: string list }

type EvidenceReportStatus =
    | EvidenceOk
    | EvidenceUnsupported
    | EvidenceFailed

type EvidenceReportField =
    { Name: string
      Value: string }

type EvidenceReport =
    { Status: EvidenceReportStatus
      Command: string
      OutputPath: string option
      Fields: EvidenceReportField list
      Lines: string list
      ExitCode: int }

type EvidenceReportRequest =
    { Status: EvidenceReportStatus
      Command: string
      OutputPath: string option
      Fields: EvidenceReportField list }

type EvidenceReportValidationResult =
    { Accepted: bool
      MissingFields: string list
      Diagnostics: string list }

type ScreenshotEvidenceReportCheck =
    { Status: string
      EvidenceKind: string option
      ScreenshotPath: string option
      Width: int option
      Height: int option
      ViewerOpenStatus: string option
      FirstFrameStatus: string option
      CaptureAvailability: string option
      CaptureSource: string option
      UnsupportedHostReason: string option
      Fallback: string option
      Diagnostics: string list }

type ScreenshotEvidenceReportValidationResult =
    { Accepted: bool
      MissingFields: string list
      FailureClass: string option
      Diagnostics: string list }

module GeneratedProductAssertions =
    let summarize expectation =
        let packages =
            expectation.PackageReferences
            |> List.map (fun package -> if package.Required then package.PackageId else $"!{package.PackageId}")
            |> String.concat ", "

        $"{expectation.Profile}: files={expectation.RequiredFiles.Length}; forbidden={expectation.ForbiddenPrefixes.Length}; packages={packages}"

    let validateDefaultInteractiveLaunch (source: string) =
        let defaultBranch =
            let marker = "| _ ->"
            let index = source.LastIndexOf(marker, StringComparison.Ordinal)

            if index >= 0 then
                source.Substring(index)
            else
                source

        let contains (value: string) =
            defaultBranch.Contains(value, StringComparison.Ordinal)

        let diagnostics =
            [ if not (contains "Viewer.runApp viewerOptions generatedHost") then
                  "default executable must call Viewer.runApp viewerOptions generatedHost"
              if not (contains "mode=interactive-window") then
                  "default executable must report mode=interactive-window"
              if not (contains "accessible-window=true" || contains "window-visible=observed:true") then
                  "default executable must claim an accessible desktop window"
              if contains "Viewer.runBounded" then
                  "default executable must not use Viewer.runBounded bounded evidence"
              if contains "first-frame-only=true" || contains "exit after first frame" then
                  "default executable must not exit after first frame"
              if contains "SceneEvidence.render" then
                  "default executable must not substitute scene-only metadata"
              if contains "self-closed-for-evidence=true" then
                  "default executable must not report evidence self-close"
              if contains "control-count" || contains "count controls" || contains "print metadata" then
                  "default executable must not be metadata-only"
              if contains "mode=persistent-evidence" then
                  "default executable must keep persistent-evidence behind explicit flags" ]

        { InteractiveLaunchRequired = List.isEmpty diagnostics
          Diagnostics = diagnostics }

    let validateWindowDiagnostics (check: GeneratedWindowDiagnosticCheck) =
        let contains (value: string) =
            check.Output.Contains(value, StringComparison.OrdinalIgnoreCase)

        let statusIsFailureClass =
            contains "status=degraded" || contains "status=unsupported" || contains "status=failed"

        let diagnostics =
            [ if not statusIsFailureClass then
                  "window diagnostics must report degraded unsupported or failed status"
              for failureClass in check.RequiredFailureClasses do
                  if not (contains $"diagnostic-class={failureClass}" || contains $"failure-class={failureClass}") then
                      $"missing generated diagnostic failure class: {failureClass}"
              for fact in check.RequiredNativeFacts do
                  if not (contains $"{fact}=observed:true"
                          || contains $"{fact}=observed:false"
                          || contains $"{fact}=unsupported"
                          || contains $"{fact}=unavailable") then
                      $"missing observable-vs-unsupported native fact: {fact}"
              if contains "private runtime fallback" && not (contains "fallback-full-desktop-session=false") then
                  "private runtime fallback must be disclosed as not a full desktop session"
              if contains "taskbar-only" && contains "status=ok" then
                  "taskbar-only launch must not be reported as status=ok" ]

        { DiagnosticsComplete = List.isEmpty diagnostics
          Diagnostics = diagnostics }

module LocalConsumerPackages =
    let report feedPath (packages: LocalConsumerPackage list) =
        let packageLines =
            packages
            |> List.map (fun package -> $"""<PackageReference Include="{package.PackageId}" Version="{package.Version}" />""")
            |> String.concat Environment.NewLine

        { FeedPath = feedPath
          Packages = packages
          ConsumerConfigSnippet = packageLines
          NuGetConfigSnippet = Some $"<add key=\"local\" value=\"{feedPath}\" />"
          RestoreCommand = "dotnet restore --source " + feedPath
          DriftDiagnostics = [] }

    let classifyDrift (expected: LocalConsumerPackage list) (actual: LocalConsumerPackage list) =
        expected
        |> List.choose (fun package ->
            let actualPackage =
                actual |> List.tryFind (fun candidate -> candidate.PackageId = package.PackageId)

            match actualPackage with
            | Some current when current.Version = package.Version -> None
            | Some current ->
                Some
                    { PackageId = package.PackageId
                      ExpectedVersion = package.Version
                      ActualVersion = Some current.Version
                      FeedPath = package.FeedPath
                      RemediationCommand = "dotnet fake run build.fsx --target PackLocal" }
            | None ->
                Some
                    { PackageId = package.PackageId
                      ExpectedVersion = package.Version
                      ActualVersion = None
                      FeedPath = package.FeedPath
                      RemediationCommand = "dotnet fake run build.fsx --target PackLocal" })

module GeneratedConsumerValidation =
    let summarize (result: GeneratedValidationResult) =
        let evidence = result.EvidencePath |> Option.defaultValue "none"
        let diagnostics = result.Diagnostics |> String.concat "; "
        $"{result.Category}: elapsed={result.Elapsed}; command={result.CommandContext}; evidence={evidence}; diagnostics={diagnostics}"

    let verifyPackageResolution check =
        let drift = LocalConsumerPackages.classifyDrift check.RequestedPackages check.ResolvedPackages

        let nu1603 =
            check.RestoreWarnings
            |> List.filter (fun warning -> warning.Contains("NU1603", StringComparison.OrdinalIgnoreCase))

        let missingSources =
            check.PackageSources |> List.isEmpty

        let diagnostics =
            [ if missingSources then
                  "missing package sources"
              for warning in nu1603 do
                  $"restore warning: {warning}"
              for item in drift do
                  let actual = item.ActualVersion |> Option.defaultValue "missing"
                  $"package mismatch: {item.PackageId} requested={item.ExpectedVersion} resolved={actual}" ]

        let failureReason =
            if not (List.isEmpty nu1603) then
                Some "NU1603"
            elif not (List.isEmpty drift) then
                Some "version-mismatch"
            elif missingSources then
                Some "missing-package-sources"
            else
                None

        { ExactMatch = failureReason.IsNone
          FailureReason = failureReason
          Diagnostics = diagnostics }

    let verifyGeneratedTests check =
        let diagnostics =
            [ if check.TestsExist && not check.TestsRan then
                  "generated tests exist but did not run"
              if check.TestsRan && not check.VerifyRan then
                  "generated tests ran outside generated Verify" ]

        let reason =
            if check.TestsExist && not check.TestsRan then
                Some "missing-generated-test-execution"
            elif check.TestsRan && not check.VerifyRan then
                Some "verify-target-not-authoritative"
            else
                None

        { Authoritative = reason.IsNone
          NonAuthoritativeReason = reason
          Diagnostics = diagnostics }

    let selectVisualEvidence request =
        if request.ScreenshotAvailable then
            { EvidenceKind = Screenshot
              BoardReadable = request.BoardReadable
              InputOrProgressObserved = request.InputOrProgressObserved
              FallbackReason = None
              UnsupportedReason = None
              Diagnostics = [ "screenshot preferred for supported generated game evidence" ] }
        elif request.PixelReadbackAvailable then
            { EvidenceKind = PixelReadback
              BoardReadable = request.BoardReadable
              InputOrProgressObserved = request.InputOrProgressObserved
              FallbackReason = Some "screenshot unavailable; pixel-readback selected"
              UnsupportedReason = None
              Diagnostics = [ "pixel-readback fallback selected"; "screenshot unavailable" ] }
        else
            let reason = request.UnsupportedReason |> Option.defaultValue "no screenshot or pixel-readback path available"

            { EvidenceKind = UnsupportedHost
              BoardReadable = None
              InputOrProgressObserved = None
              FallbackReason = None
              UnsupportedReason = Some reason
              Diagnostics = [ $"unsupported-host visual evidence: {reason}" ] }

    let private outputField name (output: string) =
        output.Split([| '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.tryPick (fun line ->
            let prefix = name + "="

            if line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) then
                Some(line.Substring(prefix.Length).Trim())
            else
                None)

    let private outputContains (value: string) (output: string) =
        output.Contains(value, StringComparison.OrdinalIgnoreCase)

    let validateVisualEvidenceCommandOutput (check: GeneratedVisualEvidenceCommandCheck) =
        let kind = outputField "evidence-kind" check.Output
        let imageDecodable = outputField "image-decodable" check.Output
        let provesScene = outputField "proves-scene-rendering" check.Output
        let provesDesktop = outputField "proves-desktop-visibility" check.Output
        let unsupportedReason = outputField "unsupported-reason" check.Output

        let diagnostics =
            [ match kind with
              | None -> "visual evidence command output must include evidence-kind"
              | Some "image" ->
                  if imageDecodable <> Some "true" then
                      "requested image evidence must be a decodable image, not metadata/hash text"
                  if outputContains "hash=" check.Output && imageDecodable <> Some "true" then
                      "metadata/hash output must be labeled metadata-hash instead of image"
                  if provesScene.IsNone then
                      "image evidence must state whether it proves scene rendering"
                  if provesDesktop.IsNone then
                      "image evidence must state whether it proves desktop visibility"
              | Some "pixel-readback" ->
                  if not (outputContains "fallback-reason=screenshot-unavailable" check.Output) then
                      "pixel-readback evidence must name the screenshot-unavailable fallback reason"
                  if provesScene <> Some "true" then
                      "pixel-readback evidence must prove scene rendering"
                  if provesDesktop <> Some "false" then
                      "pixel-readback evidence must not claim desktop visibility"
              | Some "metadata-hash" ->
                  if provesDesktop <> Some "false" then
                      "metadata/hash evidence must not claim desktop visibility"
              | Some "unsupported-host" ->
                  if unsupportedReason.IsNone then
                      "unsupported-host evidence must include unsupported-reason"
              | Some other -> $"unsupported visual evidence kind: {other}"

              if check.RequestedImageEvidence && kind = Some "metadata-hash" then
                  "requested image evidence cannot be satisfied by metadata/hash output" ]

        let failureReason =
            if diagnostics.IsEmpty then
                None
            elif kind = Some "image" && imageDecodable <> Some "true" then
                Some "metadata-only-image-evidence"
            elif check.RequestedImageEvidence && kind = Some "metadata-hash" then
                Some "metadata-only-image-evidence"
            elif kind = Some "unsupported-host" then
                Some "unsupported-host"
            else
                Some "visual-evidence-incomplete"

        { Accepted = diagnostics.IsEmpty
          EvidenceKind = kind
          FailureReason = failureReason
          Diagnostics = diagnostics }

    let buildValidationContractOutput check =
        let diagnostics =
            [ if not check.PackageResolution.ExactMatch then
                  yield! check.PackageResolution.Diagnostics
              if not check.GeneratedTests.Authoritative then
                  yield! check.GeneratedTests.Diagnostics
              if not check.DefaultInteractiveLaunch.InteractiveLaunchRequired then
                  yield! check.DefaultInteractiveLaunch.Diagnostics
              if not check.BoundedEvidenceValidated then
                  "bounded evidence validation did not run"
              if not check.CloseReasonValidated then
                  "close reason validation did not run"
              if not check.WindowDiagnostics.DiagnosticsComplete then
                  yield! check.WindowDiagnostics.Diagnostics
              if not check.WindowOptionsValidated then
                  "window options validation did not run"
              if not check.ImageEvidence.Accepted then
                  yield! check.ImageEvidence.Diagnostics ]

        let failureClass =
            if not check.PackageResolution.ExactMatch then
                check.PackageResolution.FailureReason |> Option.defaultValue "package-verification"
            elif not check.GeneratedTests.Authoritative then
                check.GeneratedTests.NonAuthoritativeReason |> Option.defaultValue "generated-test-execution"
            elif not check.DefaultInteractiveLaunch.InteractiveLaunchRequired then
                "interactive-launch-validation"
            elif not check.BoundedEvidenceValidated then
                "bounded-evidence-validation"
            elif not check.CloseReasonValidated then
                "close-reason-validation"
            elif not check.WindowDiagnostics.DiagnosticsComplete then
                "window-diagnostics-validation"
            elif not check.WindowOptionsValidated then
                "window-options-validation"
            elif not check.ImageEvidence.Accepted then
                check.ImageEvidence.FailureReason |> Option.defaultValue "visual-evidence-validation"
            else
                "none"

        let authoritative = List.isEmpty diagnostics

        let diagnosticText = String.concat "; " diagnostics

        let output =
            [ $"exact-package-match={check.PackageResolution.ExactMatch.ToString().ToLowerInvariant()}"
              "package-resolution=validated"
              $"generated-tests-ran={(check.GeneratedTests.Authoritative && check.GeneratedTests.NonAuthoritativeReason.IsNone).ToString().ToLowerInvariant()}"
              "generated-test-execution=validated"
              $"default-interactive-launch={check.DefaultInteractiveLaunch.InteractiveLaunchRequired.ToString().ToLowerInvariant()}"
              $"bounded-evidence-validation={check.BoundedEvidenceValidated.ToString().ToLowerInvariant()}"
              $"close-reason-validation={check.CloseReasonValidated.ToString().ToLowerInvariant()}"
              $"window-diagnostics-validation={check.WindowDiagnostics.DiagnosticsComplete.ToString().ToLowerInvariant()}"
              $"window-options-validation={check.WindowOptionsValidated.ToString().ToLowerInvariant()}"
              $"image-evidence-validation={check.ImageEvidence.Accepted.ToString().ToLowerInvariant()}"
              $"authoritative={authoritative.ToString().ToLowerInvariant()}"
              $"failure-class={failureClass}"
              if not diagnostics.IsEmpty then
                  $"diagnostics={diagnosticText}" ]
            |> String.concat Environment.NewLine

        { Output = output
          Authoritative = authoritative
          FailureClass = failureClass
          Diagnostics = diagnostics }

module GeneratedLayoutValidation =
    let validate check =
        let classified = LayoutEvidence.classify check.Report

        let diagnostics =
            [ if check.RequireReadableLayout && classified.ProofLevel <> ReadableLayout then
                  $"layout proof level is {classified.ProofLevel}, expected ReadableLayout"
              if classified.HudRegion.IsNone then
                  "missing HUD region"
              if classified.GameplayRegion.IsNone then
                  "missing gameplay region"
              if classified.TextBounds.IsEmpty then
                  "missing HUD text bounds"
              if classified.GameplayBounds.IsEmpty then
                  "missing gameplay bounds"
              if classified.ProofLevel = UnsupportedLayoutInspection && classified.UnsupportedReasons.IsEmpty then
                  "unsupported layout inspection requires an unsupported reason"
              match classified.OverlapStatus with
              | LayoutOverlaps overlaps -> yield! overlaps |> List.map _.Message
              | NoLayoutOverlap -> ()
              yield! classified.Diagnostics ]
            |> List.distinct

        let failureClass =
            if diagnostics.IsEmpty then
                None
            elif classified.ProofLevel = DeterministicRenderOnly && classified.RenderEvidence.IsSome then
                Some DeterministicRenderOnlyClaim
            elif classified.ProofLevel = UnsupportedLayoutInspection then
                Some UnsupportedLayoutFacts
            else
                match classified.OverlapStatus with
                | LayoutOverlaps _ -> Some OverlappingLayoutBounds
                | NoLayoutOverlap -> Some MissingLayoutFacts

        { Accepted = failureClass.IsNone
          FailureClass = failureClass
          Diagnostics = diagnostics }

module HostWarningClassification =
    let classify (check: HostWarningClassificationCheck) =
        let warningLines =
            check.RawMessage.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map _.Trim()
            |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace line))
            |> Array.toList

        let lineHasKnownMarker (line: string) =
            check.KnownBenignMarkers
            |> List.exists (fun (marker: string) -> line.Contains(marker, StringComparison.OrdinalIgnoreCase))

        let known =
            not warningLines.IsEmpty
            && warningLines |> List.forall lineHasKnownMarker

        let layoutAccepted =
            check.LayoutReadable = Some true || check.ExplicitlyUnsupportedWithoutReadabilityClaim

        let warningClass =
            if not check.PackageSucceeded then PackageFailure
            elif not check.LaunchSucceeded then LaunchFailure
            elif not check.RenderingSucceeded then RenderingFailure
            elif not layoutAccepted then LayoutFailure
            elif known then BenignEnvironmentWarning
            else UnknownWarning

        let fatal =
            match warningClass with
            | BenignEnvironmentWarning -> false
            | _ -> true

        let layoutReadable =
            check.LayoutReadable
            |> Option.map (fun value -> value.ToString().ToLowerInvariant())
            |> Option.defaultValue "none"

        let supportingFacts =
            [ $"launch-succeeded={check.LaunchSucceeded.ToString().ToLowerInvariant()}"
              $"rendering-succeeded={check.RenderingSucceeded.ToString().ToLowerInvariant()}"
              $"layout-readable={layoutReadable}"
              $"unsupported-without-readability-claim={check.ExplicitlyUnsupportedWithoutReadabilityClaim.ToString().ToLowerInvariant()}"
              $"package-succeeded={check.PackageSucceeded.ToString().ToLowerInvariant()}" ]

        let diagnostics =
            [ $"warning-class={warningClass}"
              $"fatal={fatal}"
              yield! supportingFacts
              if String.IsNullOrWhiteSpace check.RawMessage then
                  "raw-message=missing"
              if not known && warningClass = UnknownWarning then
                  "unknown warning marker"
              if warningLines |> List.exists (lineHasKnownMarker >> not) then
                  "unrelated warning or error text present"
              if not check.LaunchSucceeded then
                  "launch evidence failed"
              if not check.RenderingSucceeded then
                  "rendering evidence failed"
              if not layoutAccepted then
                  "layout readability failed or missing"
              if not check.PackageSucceeded then
                  "package evidence failed" ]

        { WarningClass = warningClass
          RawMessage = check.RawMessage
          Fatal = fatal
          EvidencePath = check.EvidencePath
          SupportingFacts = supportingFacts
          Diagnostics = diagnostics }

module PersistentLaunchArtifactValidation =
    let private requiredFields =
        [ "status"
          "mode"
          "command"
          "window-opened"
          "input-dispatch"
          "exit-path"
          "blocked-stage"
          "classification"
          "category"
          "message" ]

    let private parseFields (lines: string list) =
        lines
        |> List.collect (fun (line: string) ->
            line.Split([| ' '; '\t'; '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.choose (fun (token: string) ->
                let equals = token.IndexOf('=')

                if equals <= 0 then
                    None
                else
                    Some(token.Substring(0, equals).Trim().ToLowerInvariant(), token.Substring(equals + 1).Trim()))
            |> Array.toList)
        |> Map.ofList

    let private validValues =
        Map.ofList
            [ "status", Set.ofList [ "ok"; "failed"; "unsupported" ]
              "mode", Set.ofList [ "interactive-window"; "persistent-evidence" ]
              "input-dispatch", Set.ofList [ "verified"; "not-verified"; "not-required"; "failed"; "true"; "false" ]
              "exit-path", Set.ofList [ "true"; "false" ]
              "window-opened", Set.ofList [ "true"; "false" ]
              "first-frame-presented", Set.ofList [ "true"; "false" ]
              "blocked-stage",
              Set.ofList
                  [ "none"
                    "desktopprerequisite"
                    "processlaunch"
                    "windowcreation"
                    "firstframerender"
                    "observation"
                    "capture"
                    "inputverification"
                    "controlledexit"
                    "artifactwrite"
                    "window"
                    "surface"
                    "renderer"
                    "swapchain"
                    "scene"
                    "readback"
                    "app"
                    "timeout"
                    "unknown" ]
              "classification",
              Set.ofList
                  [ "none"
                    "ok"
                    "unsupportedenvironment"
                    "packageresolution"
                    "verificationdepth"
                    "applifecycle"
                    "productdefect" ]
              "category",
              Set.ofList
                  [ "none"
                    "startup"
                    "environmentsession"
                    "input"
                    "frame"
                    "renderer"
                    "vulkan"
                    "skia"
                    "swapchain"
                    "scene"
                    "screenshot" ] ]

    let validate (check: PersistentLaunchArtifactCheck) =
        let fields = parseFields check.Lines

        let missing =
            requiredFields
            |> List.filter (fun field -> not (fields.ContainsKey field))

        let field name = fields |> Map.tryFind name

        let invalidFields =
            validValues
            |> Map.toList
            |> List.choose (fun (name, allowed) ->
                field name
                |> Option.bind (fun value ->
                    let normalized = value.Trim().ToLowerInvariant()

                    if allowed.Contains normalized then
                        None
                    else
                        Some $"{name}={value}"))

        let passClaim =
            check.SupportedHostPassClaimed
            || field "status" = Some "ok"
            || field "classification" = Some "ok"

        let contradictions =
            [ if check.SyntheticFixture && passClaim then
                  "synthetic fixture cannot satisfy supported-host persistent launch"
              if passClaim && field "window-opened" <> Some "true" then
                  "status=ok requires window-opened=true"
              if passClaim && field "first-frame-presented" <> Some "true" then
                  "status=ok requires first-frame-presented=true"
              if passClaim && field "exit-path" <> Some "true" then
                  "status=ok requires exit-path=true"
              if passClaim && field "blocked-stage" <> Some "none" then
                  "status=ok requires blocked-stage=none" ]

        let diagnostics =
            [ $"artifact-path={check.ArtifactPath}"
              if check.SyntheticFixture then
                  "synthetic-fixture=true"
              for item in missing do
                  $"missing-field={item}"
              for item in invalidFields do
                  $"invalid-field={item}"
              yield! contradictions ]

        { Accepted = missing.IsEmpty && invalidFields.IsEmpty && contradictions.IsEmpty
          MissingFields = missing
          Contradictions = invalidFields @ contradictions
          Diagnostics = diagnostics }

module ReadinessFileDiscovery =
    let validate (check: ReadinessFileDiscoveryCheck) =
        let existing =
            check.ExistingFiles
            |> List.map (fun path -> path.Trim().Replace('\\', '/'))
            |> Set.ofList

        let missing =
            check.RequiredFiles
            |> List.filter (fun file ->
                let normalized = file.Trim().Replace('\\', '/')
                not (existing.Contains normalized))

        let diagnostics =
            [ $"readiness-directory={check.ReadinessDirectory}"
              for item in missing do
                  $"missing-readiness-file={item}" ]

        { Complete = missing.IsEmpty
          MissingFiles = missing
          Diagnostics = diagnostics }

module EvidenceReports =
    let statusText status =
        match status with
        | EvidenceOk -> "ok"
        | EvidenceUnsupported -> "unsupported"
        | EvidenceFailed -> "failed"

    let field name value =
        { Name = name
          Value = value }

    let private statusExitCode status =
        match status with
        | EvidenceOk
        | EvidenceUnsupported -> 0
        | EvidenceFailed -> 1

    let private normalizeFields fields =
        fields
        |> List.filter (fun item -> not (String.IsNullOrWhiteSpace item.Name))
        |> List.map (fun item -> { item with Name = item.Name.Trim(); Value = item.Value.Trim() })

    let build (request: EvidenceReportRequest) =
        let standardFields =
            [ field "status" (statusText request.Status)
              field "command" request.Command ]
            @ (request.OutputPath |> Option.map (field "output") |> Option.toList)

        let merged =
            standardFields @ normalizeFields request.Fields
            |> List.distinctBy (fun item -> item.Name.ToLowerInvariant())

        let lines = merged |> List.map (fun item -> $"{item.Name}={item.Value}")

        { Status = request.Status
          Command = request.Command
          OutputPath = request.OutputPath
          Fields = merged
          Lines = lines
          ExitCode = statusExitCode request.Status }

    let write (request: EvidenceReportRequest) =
        let report = build request

        match report.OutputPath with
        | Some path ->
            match IO.Path.GetDirectoryName path with
            | null
            | "" -> ()
            | directory -> IO.Directory.CreateDirectory(directory) |> ignore

            IO.File.WriteAllLines(path, report.Lines)
        | None -> ()

        report.Lines |> List.iter Console.WriteLine
        report

    let validate (report: EvidenceReport) =
        let names =
            report.Fields
            |> List.map (fun field -> field.Name.ToLowerInvariant())
            |> Set.ofList

        let required =
            [ "status"; "command" ]
            @ (if report.OutputPath.IsSome then [ "output" ] else [])
            @ (if report.Status = EvidenceUnsupported then [ "unsupported-host-reason"; "fallback" ] else [])

        let missing = required |> List.filter (fun name -> not (names.Contains name))

        let diagnostics =
            [ if report.Lines <> (report.Fields |> List.map (fun item -> $"{item.Name}={item.Value}")) then
                  "stdout/file lines must match report field ordering"
              if report.Status = EvidenceFailed && report.ExitCode = 0 then
                  "failed reports must use non-zero exit code"
              for item in missing do
                  $"missing-field={item}" ]

        { Accepted = missing.IsEmpty && diagnostics.IsEmpty
          MissingFields = missing
          Diagnostics = diagnostics }

    let validateScreenshotEvidence (check: ScreenshotEvidenceReportCheck) =
        let normalizedStatus = check.Status.Trim().ToLowerInvariant()
        let normalizedKind = check.EvidenceKind |> Option.map (fun value -> value.Trim().ToLowerInvariant())
        let normalizedSource = check.CaptureSource |> Option.map (fun value -> value.Trim().ToLowerInvariant())
        let hostilePath =
            check.ScreenshotPath
            |> Option.exists (fun path ->
                let normalized = path.Replace('\\', '/')
                IO.Path.IsPathRooted path
                || normalized.StartsWith("../", StringComparison.Ordinal)
                || normalized.Contains("/../", StringComparison.Ordinal))

        let hiddenWarning =
            check.Diagnostics
            |> List.exists (fun diagnostic ->
                diagnostic.Contains("warning", StringComparison.OrdinalIgnoreCase)
                || diagnostic.Contains("Gtk-Message", StringComparison.OrdinalIgnoreCase))

        let positiveDimensions =
            match check.Width, check.Height with
            | Some width, Some height -> width > 0 && height > 0
            | _ -> false

        let missing =
            [ if normalizedKind.IsNone then
                  "evidence-kind"
              if check.ViewerOpenStatus.IsNone then
                  "viewer-open-status"
              if check.FirstFrameStatus.IsNone then
                  "first-frame-status"
              if check.CaptureAvailability.IsNone then
                  "capture-availability"
              if normalizedSource.IsNone then
                  "capture-source"
              if normalizedStatus = "ok" then
                  if check.ScreenshotPath.IsNone then
                      "screenshot-path"
                  if check.Width.IsNone then
                      "width"
                  if check.Height.IsNone then
                      "height"
              if normalizedStatus = "unsupported" then
                  if check.UnsupportedHostReason.IsNone then
                      "unsupported-host-reason"
                  if check.Fallback.IsNone then
                      "fallback" ]

        let diagnostics =
            [ if normalizedKind <> Some "screenshot" then
                  "screenshot evidence report must use evidence-kind=screenshot"
              match normalizedStatus with
              | "ok" ->
                  if not positiveDimensions then
                      "successful screenshot evidence requires positive dimensions"
                  if normalizedSource <> Some "live-viewer-window" then
                      "successful screenshot evidence requires capture-source=live-viewer-window"
                  if check.Fallback.IsSome then
                      "successful screenshot evidence must not require deterministic fallback"
                  if hostilePath then
                      "screenshot artifact path must stay within the requested readiness artifact tree"
                  if hiddenWarning then
                      "successful screenshot evidence must not hide warning diagnostics"
              | "unsupported" ->
                  if check.ScreenshotPath.IsSome then
                      "unsupported screenshot evidence must not claim screenshot-path"
                  if normalizedSource = Some "live-viewer-window" then
                      "unsupported screenshot evidence must not claim live viewer capture"
              | "failed" -> ()
              | other -> $"unsupported screenshot status: {other}"
              yield! check.Diagnostics ]

        let failureClass =
            if not missing.IsEmpty then
                Some "missing-screenshot-evidence-fields"
            elif not diagnostics.IsEmpty then
                Some "invalid-screenshot-evidence-fields"
            else
                None

        { Accepted = failureClass.IsNone
          MissingFields = missing
          FailureClass = failureClass
          Diagnostics = diagnostics }
