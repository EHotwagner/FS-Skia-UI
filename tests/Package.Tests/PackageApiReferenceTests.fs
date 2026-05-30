module PackageApiReferenceTests

open System
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

let referenceRoot =
    repositoryPath "specs/035-api-discovery-names/readiness/package/api-reference"

let referencePath packageId =
    Path.Combine(referenceRoot, packageId + ".md")

let readReference packageId =
    let path = referencePath packageId
    Expect.isTrue (File.Exists path) $"{packageId} source-shaped package API reference exists at {path}"
    File.ReadAllText path

let requiredPackages =
    [ "FS.Skia.UI.Scene", [ "src/Scene/Scene.fsi" ]
      "FS.Skia.UI.SkiaViewer", [ "src/SkiaViewer/SkiaViewer.fsi" ]
      "FS.Skia.UI.Elmish", [ "src/Elmish/Elmish.fsi" ]
      "FS.Skia.UI.KeyboardInput", [ "src/KeyboardInput/KeyboardInput.fsi" ]
      "FS.Skia.UI.Layout", [ "src/Layout/Layout.fsi"; "src/Layout/Types.fsi"; "src/Layout/Graph.fsi"; "src/Layout/GraphValidation.fsi" ]
      "FS.Skia.UI.Controls", [ "src/Controls/Types.fsi"; "src/Controls/Control.fsi"; "src/Controls/Attributes.fsi"; "src/Controls/DataGrid.fsi"; "src/Controls/Charts.fsi"; "src/Controls/TextInput.fsi"; "src/Controls/RichText.fsi" ]
      "FS.Skia.UI.Controls.Elmish", [ "src/Controls.Elmish/ControlsElmish.fsi" ]
      "FS.Skia.UI.Testing", [ "src/Testing/Testing.fsi" ] ]

[<Tests>]
let packageApiReferenceTests =
    testList "Package API reference generation" [
        test "curated fsi package reference emits one source-shaped index per package" {
            let indexPath = Path.Combine(referenceRoot, "index.md")
            Expect.isTrue (File.Exists indexPath) $"package API reference index exists at {indexPath}"

            let index = File.ReadAllText indexPath

            requiredPackages
            |> List.iter (fun (packageId, sourcePaths) ->
                Expect.stringContains index packageId $"index lists {packageId}"
                Expect.isTrue (File.Exists(referencePath packageId)) $"{packageId} has a package-specific reference file"

                let reference = readReference packageId
                Expect.stringContains reference $"package-id: {packageId}" $"{packageId} reference records package id"
                Expect.stringContains reference "package-version:" $"{packageId} reference records package version"
                Expect.stringContains reference "symbol-count:" $"{packageId} reference records sampled symbol count"
                Expect.stringContains reference "omitted-symbol-reasons:" $"{packageId} reference records omitted symbol reasons"

                sourcePaths
                |> List.iter (fun sourcePath ->
                    Expect.stringContains reference sourcePath $"{packageId} reference cites curated input {sourcePath}"))
        }

        test "source-shaped reference preserves F# authoring names and parameter labels" {
            let scene = readReference "FS.Skia.UI.Scene"
            let controls = readReference "FS.Skia.UI.Controls"
            let viewer = readReference "FS.Skia.UI.SkiaViewer"
            let keyboard = readReference "FS.Skia.UI.KeyboardInput"

            [ "type Rect ="
              "Width: float"
              "Height: float"
              "LinearGradient of startPoint: Point * endPoint: Point * colors: Color list"
              "DropShadow of dx: float * dy: float * blur: float * color: Color"
              "TextRun"
              "SceneElementKind" ]
            |> List.iter (fun sample -> Expect.stringContains scene sample $"Scene reference preserves {sample}")

            [ "type Control<'msg>"
              "KnownControl.TextBlock"
              "StandardAttributeName.VisibleRange"
              "DataGrid.create"
              "LineChart.series"
              "TextBox.onChanged" ]
            |> List.iter (fun sample -> Expect.stringContains controls sample $"Controls reference preserves {sample}")

            [ "type ViewerOptions ="
              "InitialSize: Size"
              "ViewerWindowPosition"
              "Coordinates of x: int * y: int" ]
            |> List.iter (fun sample -> Expect.stringContains viewer sample $"SkiaViewer reference preserves {sample}")

            [ "KeyboardModel"
              "KeyboardEvent"
              "KeyDown"
              "KeyUp" ]
            |> List.iter (fun sample -> Expect.stringContains keyboard sample $"KeyboardInput reference preserves {sample}")
        }

        test "reference output carries XML summaries and unsupported symbol diagnostics" {
            let scene = readReference "FS.Skia.UI.Scene"
            let controls = readReference "FS.Skia.UI.Controls"
            let testing = readReference "FS.Skia.UI.Testing"

            [ scene; controls; testing ]
            |> List.iter (fun reference ->
                Expect.stringContains reference "Public contract type exposed by this FS.Skia.UI package." "XML summary text is preserved"
                Expect.stringContains reference "omitted-symbol-reasons:" "omitted symbols are reported even when the list is empty"
                Expect.stringContains reference "unsupported-symbols:" "unsupported signatures are diagnostic instead of silently dropped")
        }

        test "reference generation declares no reflection or repository-source authoring fallback" {
            let index = File.ReadAllText(Path.Combine(referenceRoot, "index.md"))

            [ "generated-from: curated-fsi"
              "assembly-reflection: false"
              "repository-source-authoring-fallback: false" ]
            |> List.iter (fun required ->
                Expect.stringContains index required $"reference index records {required}")
        }
    ]
