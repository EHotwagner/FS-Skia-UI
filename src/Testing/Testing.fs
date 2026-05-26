namespace FS.Skia.UI.Testing

open System

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
              if contains "Viewer.runBounded" then
                  "default executable must not use Viewer.runBounded bounded evidence"
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
    let summarize result =
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
