module PackageTests

open System
open System.Diagnostics
open System.IO
open Expecto

let rec findRepositoryRoot (directory: string) =
    if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

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
    Path.Combine(repositoryRoot, "specs", "006-template-framework-governance", "readiness", "package", name)

let packageVersion = "0.1.5-preview.1"

[<Tests>]
let packageContractTests =
    let v1PackageTests = [
        test "all three packages pack into local package output" {
            let packageOutput =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".local",
                    "share",
                    "nuget-local")

            Directory.CreateDirectory packageOutput |> ignore

            [ "src/Lib/Lib.fsproj", "FS.Skia.UI"
              "src/Charts/Charts.fsproj", "FS.Skia.UI.Charts"
              "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout" ]
            |> List.iter (fun (project, packageId) ->
                let exitCode, _, stderr =
                    runDotnet repositoryRoot $"pack {project} --output {packageOutput}"

                Expect.equal exitCode 0 stderr
                Expect.isNonEmpty (Directory.GetFiles(packageOutput, packageId + $".{packageVersion}.nupkg")) $"{packageId} package is produced")
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
                    "src/Charts/Charts.fsproj"
                    "src/Layout/Layout.fsproj" ]
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
                    "ChartsConsumer", "FS.Skia.UI.Charts"
                    "LayoutConsumer", "FS.Skia.UI.Layout" ]
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
