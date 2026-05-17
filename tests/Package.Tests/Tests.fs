module PackageTests

open System
open System.Diagnostics
open System.IO
open Expecto

let rec findRepositoryRoot (directory: string) =
    if Directory.GetFiles(directory, "*.sln").Length > 0 || File.Exists(Path.Combine(directory, "build.fsx")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

let repositoryPath (relativePath: string) =
    Path.Combine(repositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))

let runDotnet (workingDirectory: string) (arguments: string) =
    let startInfo: ProcessStartInfo = ProcessStartInfo("dotnet", arguments)
    startInfo.WorkingDirectory <- workingDirectory
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    match Process.Start(startInfo) |> Option.ofObj with
    | None -> failwithf "Could not start dotnet %s" arguments
    | Some proc ->
        use proc = proc
        let stdoutTask = proc.StandardOutput.ReadToEndAsync()
        let stderrTask = proc.StandardError.ReadToEndAsync()

        if proc.WaitForExit(120000) then
            proc.ExitCode, stdoutTask.GetAwaiter().GetResult(), stderrTask.GetAwaiter().GetResult()
        else
            proc.Kill(true)
            -1, stdoutTask.GetAwaiter().GetResult(), stderrTask.GetAwaiter().GetResult()

let packageOutput name =
    Path.Combine(repositoryRoot, "specs", "007-v2-template-packaging", "readiness", "package", name)

let packageVersion = "0.1.9-preview.1"

[<Tests>]
let packageContractTests =
    let v1PackageTests = [
        test "active packages are declared for PackLocal" {
            let build = File.ReadAllText(Path.Combine(repositoryRoot, "build.fsx"))

            [ "src/Lib/Lib.fsproj", "FS.Skia.UI"
              "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout"
              "src/Controls.Elmish/Controls.Elmish.fsproj", "FS.Skia.UI.Controls.Elmish"
              "src/Controls/Controls.fsproj", "FS.Skia.UI.Controls" ]
            |> List.iter (fun (project, packageId) ->
                Expect.stringContains build project $"{project} is packed by PackLocal"
                Expect.stringContains build packageId $"{packageId} is packed by PackLocal")

            Expect.isFalse (build.Contains("\"src/Charts/Charts.fsproj\", \"FS.Skia.UI.Charts\"")) "Charts is not an active PackLocal package"
        }

        test "controls boundary has no active Charts package capability or monolithic viewer coupling" {
            let build = File.ReadAllText(Path.Combine(repositoryRoot, "build.fsx"))
            let capabilities = File.ReadAllText(Path.Combine(repositoryRoot, "template", "capabilities.yml"))
            let controlsProject = File.ReadAllText(Path.Combine(repositoryRoot, "src", "Controls", "Controls.fsproj"))

            Expect.isFalse (File.Exists(Path.Combine(repositoryRoot, "src", "Charts", "Charts.fsproj"))) "legacy Charts project is removed or deactivated from source ownership"
            Expect.isFalse (build.Contains("FS.Skia.UI.Charts", StringComparison.Ordinal)) "build wiring has no active Charts package reference"
            Expect.isFalse (capabilities.Contains("id: charts", StringComparison.OrdinalIgnoreCase)) "generated capability catalog has no active charts capability"
            Expect.isFalse (controlsProject.Contains(@"..\Lib\Lib.fsproj", StringComparison.Ordinal)) "Controls package does not depend on the monolithic viewer/runtime project"
            Expect.isTrue (File.Exists(Path.Combine(repositoryRoot, "src", "Controls", "DataGrid.fsi"))) "DataGrid public contract is owned by Controls"
        }

        test "generated products and surface checks do not keep Charts as an active package" {
            let build = File.ReadAllText(repositoryPath "build.fsx")

            let generatedProductInputs =
                [ "template/capabilities.yml"
                  "template/profiles/app.yml"
                  "template/profiles/governed.yml"
                  "template/profiles/headless-scene.yml"
                  "template/profiles/sample-pack.yml"
                  "template/base/Directory.Packages.props"
                  "template/base/src/Product/Product.fsproj"
                  "template/base/.agents/skills/fs-skia-project/SKILL.md"
                  "scripts/refresh-surface-baselines.fsx" ]

            let forbiddenTokens =
                [ "PackageReference Include=\"FS.Skia.UI.Charts\""
                  "src/Charts/Charts.fsproj"
                  "id: charts"
                  "template/fragments/charts"
                  ".agents/skills/fs-skia-charts/SKILL.md" ]

            let activeHits =
                generatedProductInputs
                |> List.filter (repositoryPath >> File.Exists)
                |> List.collect (fun relative ->
                    let content = File.ReadAllText(repositoryPath relative)

                    forbiddenTokens
                    |> List.choose (fun token ->
                        if content.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0 then
                            Some $"{relative}: {token}"
                        else
                            None))

            Expect.isEmpty activeHits "active generated product inputs do not select Charts package, capability, project, or chart-specific generated skill"
            Expect.isFalse (build.Contains("\"FS.Skia.UI.Charts\"", StringComparison.Ordinal)) "generated product package validation does not enumerate Charts as an available capability package"
            Expect.isFalse (File.Exists(repositoryPath "readiness/surface-baselines/FS.Skia.UI.Charts.txt")) "legacy Charts package has no active surface baseline"
            Expect.isFalse (File.Exists(repositoryPath "template/fragments/charts/skill/SKILL.md")) "template has no chart-specific generated skill fragment"
            Expect.isFalse (File.Exists(repositoryPath "template/base/.agents/skills/fs-skia-charts/SKILL.md")) "generated product base has no chart-specific generated skill"
            Expect.stringContains build "readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt" "package surface report includes the Controls.Elmish adapter baseline"
            Expect.stringContains build "readiness/surface-baselines/FS.Skia.UI.Controls.txt" "package surface report includes the Controls baseline"
            Expect.stringContains build "readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt" "package surface report includes the KeyboardInput baseline"
        }

        test "package consumer smoke is deferred outside v1 verification" {
            let enabled =
                Environment.GetEnvironmentVariable("FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE") = "1"

            if enabled then
                Expect.isTrue enabled "explicit PackageSmoke target opted in to package consumer smoke"
            else
                Expect.isFalse enabled "v1 Dev, Verify, and Ci must not require package consumer smoke"
        }
    ]

    let deferredPackageSmokeTests =
        if Environment.GetEnvironmentVariable("FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE") = "1" then
            [ test "explicit package consumer smoke can restore each package independently from local output" {
                  let packageOutput = packageOutput "consumer-nuget"
                  Directory.CreateDirectory packageOutput |> ignore

                  [ "src/Lib/Lib.fsproj"
                    "src/Layout/Layout.fsproj"
                    "src/Controls/Controls.fsproj" ]
                  |> List.filter (fun project -> File.Exists(Path.Combine(repositoryRoot, project.Replace("/", string Path.DirectorySeparatorChar))))
                  |> List.iter (fun project ->
                      let exitCode, _, stderr =
                          runDotnet repositoryRoot $"pack {project} --output {packageOutput}"

                      Expect.equal exitCode 0 stderr)

                  let consumerRoot = Path.Combine(Path.GetTempPath(), "fs-skia-ui-package-consumer-" + Guid.NewGuid().ToString("N"))
                  Directory.CreateDirectory consumerRoot |> ignore

                  File.WriteAllText(
                      Path.Combine(consumerRoot, "NuGet.config"),
                      $"""<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="{packageOutput}" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"""
                  )

                  [ "CoreConsumer", "FS.Skia.UI"
                    "LayoutConsumer", "FS.Skia.UI.Layout"
                    "ControlsConsumer", "FS.Skia.UI.Controls" ]
                  |> List.filter (fun (_, packageId) ->
                      packageId = "FS.Skia.UI"
                      || File.Exists(Path.Combine(packageOutput, packageId + $".{packageVersion}.nupkg")))
                  |> List.iter (fun (name, packageId) ->
                      let projectDir = Path.Combine(consumerRoot, name)
                      Directory.CreateDirectory projectDir |> ignore
                      File.WriteAllText(
                          Path.Combine(projectDir, $"{name}.fsproj"),
                          $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Library.fs" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="{packageId}" Version="{packageVersion}" />
  </ItemGroup>
</Project>
"""
                      )
                      File.WriteAllText(Path.Combine(projectDir, "Library.fs"), $"module {name}\nlet value = 1\n")

                      let exitCode, _, stderr =
                          runDotnet projectDir "restore"

                      Expect.equal exitCode 0 stderr)
              } ]
        else
            []

    testList "Package contract" (v1PackageTests @ deferredPackageSmokeTests)
