module SmokeTests

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

let readinessPath segments =
    let historicalFeature = Path.Combine(repositoryRoot, "specs", "004-keyboard-state-display")
    let readinessRoot =
        if File.Exists(Path.Combine(historicalFeature, "spec.md")) then
            Path.Combine(historicalFeature, "readiness")
        else
            Path.Combine(repositoryRoot, "readiness")

    Path.Combine(Array.ofList (readinessRoot :: segments))

let runProcess (fileName: string) (arguments: string) =
    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.WorkingDirectory <- repositoryRoot
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    use proc =
        match Process.Start startInfo |> Option.ofObj with
        | Some proc -> proc
        | None -> failwithf "Could not start %s %s" fileName arguments

    let stdout: string = proc.StandardOutput.ReadToEnd()
    let stderr: string = proc.StandardError.ReadToEnd()
    proc.WaitForExit()
    proc.ExitCode, stdout, stderr

[<Tests>]
let smokeContractTests =
    testList "Sample smoke contract" [
        test "all parity samples expose contract smoke entry points" {
            [ "BasicViewer", "samples/BasicViewer/BasicViewer.fsproj"
              "InteractiveViewer", "samples/InteractiveViewer/InteractiveViewer.fsproj"
              "ParityGallery", "samples/ParityGallery/ParityGallery.fsproj"
              "EffectsGallery", "samples/EffectsGallery/EffectsGallery.fsproj"
              "ChartsGallery", "samples/ChartsGallery/ChartsGallery.fsproj"
              "DataGridGallery", "samples/DataGridGallery/DataGridGallery.fsproj"
              "LayoutGraphGallery", "samples/LayoutGraphGallery/LayoutGraphGallery.fsproj"
              "ScreenshotGallery", "samples/ScreenshotGallery/ScreenshotGallery.fsproj"
              "DemoReel", "samples/DemoReel/DemoReel.fsproj"
              "KeyboardInputGallery", "samples/KeyboardInputGallery/KeyboardInputGallery.fsproj" ]
            |> List.iter (fun (sample, project) ->
                let projectPath = Path.Combine(repositoryRoot, project)
                let programPath = Path.Combine(Path.GetDirectoryName projectPath |> Option.ofObj |> Option.defaultValue repositoryRoot, "Program.fs")
                let source = File.ReadAllText programPath

                Expect.isTrue (File.Exists projectPath) $"{sample} project exists"
                Expect.stringContains source "--contract-smoke" $"{sample} has a contract smoke argument"
                Expect.stringContains source "status=ok" $"{sample} reports smoke success"
                Expect.stringContains source $"sample={sample}" $"{sample} identifies itself")
        }

        test "KeyboardInputGallery contract smoke captures keyboard state display evidence" {
            let exitCode, stdout, stderr =
                runProcess "dotnet" "run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke"

            let evidencePath =
                readinessPath [ "sample-smoke"; "keyboard-input-gallery-state-display.txt" ]

            Path.GetDirectoryName evidencePath
            |> Option.ofObj
            |> Option.iter (Directory.CreateDirectory >> ignore)

            let evidence = stdout + stderr
            File.WriteAllText(evidencePath, evidence)

            Expect.equal exitCode 0 "KeyboardInputGallery contract smoke exits successfully"
            Expect.stringContains stdout "status=ok" "sample reports success"
            Expect.stringContains stdout "sample=KeyboardInputGallery" "sample identifies itself"
            Expect.stringContains stdout "compact-labels=" "smoke includes compact display model evidence"
            Expect.stringContains stdout "expanded-stack=" "smoke includes expanded display model evidence"
            Expect.stringContains stdout "hidden=KeyboardStateDisplayHidden" "smoke includes hidden display evidence"
            Expect.stringContains stdout "TextRunElement" "smoke includes rendered scene text primitive"
        }
    ]
