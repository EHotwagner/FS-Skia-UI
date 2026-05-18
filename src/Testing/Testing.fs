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

module GeneratedProductAssertions =
    let summarize expectation =
        let packages =
            expectation.PackageReferences
            |> List.map (fun package -> if package.Required then package.PackageId else $"!{package.PackageId}")
            |> String.concat ", "

        $"{expectation.Profile}: files={expectation.RequiredFiles.Length}; forbidden={expectation.ForbiddenPrefixes.Length}; packages={packages}"

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
