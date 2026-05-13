module SmokeTests

open System
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
    ]
