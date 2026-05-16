module SurfaceAreaTests

open System
open System.IO
open System.Reflection
open Expecto

let rec findRepositoryRoot (directory: string) =
    if Directory.GetFiles(directory, "*.sln").Length > 0 || File.Exists(Path.Combine(directory, "build.fsx")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

let baseline packageName =
    Path.Combine(repositoryRoot, "readiness", "surface-baselines", packageName + ".txt")
    |> File.ReadAllLines
    |> Array.filter (fun line -> line.Trim() <> "")
    |> Set.ofArray

let exportedNames (assembly: Assembly) =
    assembly.GetExportedTypes()
    |> Array.map (fun ty ->
        let fullName =
            match ty.FullName with
            | null -> ty.Name
            | value -> value

        if ty.Name.EndsWith("Module", StringComparison.Ordinal) then
            fullName.Replace("Module", "")
        else
            fullName)
    |> Set.ofArray

let assertBaseline packageName (assembly: Assembly) =
    let expected = baseline packageName
    let actual = exportedNames assembly
    let missing = Set.difference expected actual
    let unexpected = Set.difference actual expected
    Expect.isEmpty missing $"expected public surface for {packageName} is exported"
    Expect.isEmpty unexpected $"no unapproved public exports were added to {packageName}"

[<Tests>]
let surfaceAreaTests =
    testList "Surface baselines" [
        test "surface baselines use stable root readiness path" {
            let baselinePath =
                Path.Combine(repositoryRoot, "readiness", "surface-baselines", "FS.Skia.UI.txt")

            Expect.isTrue (File.Exists baselinePath) "stable FS.Skia.UI package surface baseline exists"
            Expect.isFalse (baselinePath.Contains("specs/002-skia-feature-parity", StringComparison.Ordinal)) "baseline path is not historical feature readiness"
        }

        test "FS.Skia.UI baseline exports expected contract names" {
            assertBaseline "FS.Skia.UI" typeof<FS.Skia.UI.ViewerProgram<int, int>>.Assembly
        }

        test "internal runtime helper modules are not package-visible exports" {
            let actual = exportedNames typeof<FS.Skia.UI.ViewerProgram<int, int>>.Assembly

            [ "FS.Skia.UI.VulkanResources"
              "FS.Skia.UI.VulkanStartup" ]
            |> List.iter (fun helper -> Expect.isFalse (actual.Contains helper) $"{helper} is not package-visible")
        }

        test "FS.Skia.UI.Layout baseline exports expected contract names" {
            assertBaseline "FS.Skia.UI.Layout" typeof<FS.Skia.UI.Layout.GraphDefinition>.Assembly
        }

        test "FS.Skia.UI.Controls baseline exports expected contract names" {
            assertBaseline "FS.Skia.UI.Controls" typeof<FS.Skia.UI.Controls.Control<int>>.Assembly
        }

        test "V3 capability packages declare package-specific contracts and baselines" {
            [ "Scene", "src/Scene/Scene.fsproj", "src/Scene/Scene.fsi", "readiness/surface-baselines/FS.Skia.UI.Scene.txt"
              "SkiaViewer", "src/SkiaViewer/SkiaViewer.fsproj", "src/SkiaViewer/SkiaViewer.fsi", "readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt"
              "Elmish", "src/Elmish/Elmish.fsproj", "src/Elmish/Elmish.fsi", "readiness/surface-baselines/FS.Skia.UI.Elmish.txt"
              "KeyboardInput", "src/KeyboardInput/KeyboardInput.fsproj", "src/KeyboardInput/KeyboardInput.fsi", "readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt"
              "Layout", "src/Layout/Layout.fsproj", "src/Layout/Layout.fsi", "readiness/surface-baselines/FS.Skia.UI.Layout.txt"
              "Controls", "src/Controls/Controls.fsproj", "src/Controls/Types.fsi", "readiness/surface-baselines/FS.Skia.UI.Controls.txt"
              "Testing", "src/Testing/Testing.fsproj", "src/Testing/Testing.fsi", "readiness/surface-baselines/FS.Skia.UI.Testing.txt" ]
            |> List.iter (fun (name, project, contract, baseline) ->
                Expect.isTrue (File.Exists(Path.Combine(repositoryRoot, project))) $"{name} project exists"
                Expect.isTrue (File.Exists(Path.Combine(repositoryRoot, contract))) $"{name} public .fsi contract exists"
                Expect.isTrue (File.Exists(Path.Combine(repositoryRoot, baseline))) $"{name} package surface baseline exists")
        }

        test "Scene package stays dependency-light" {
            let sceneProject = File.ReadAllText(Path.Combine(repositoryRoot, "src", "Scene", "Scene.fsproj"))

            [ "Fable.Elmish"
              "Silk.NET"
              "SkiaSharp"
              "Yoga.Net"
              "YamlDotNet" ]
            |> List.iter (fun forbidden -> Expect.isFalse (sceneProject.Contains forbidden) $"Scene does not reference {forbidden}")
        }

        test "top-level F# visibility modifiers do not replace signature ownership" {
            let sourceFiles =
                Directory.EnumerateFiles(Path.Combine(repositoryRoot, "src"), "*.fs", SearchOption.AllDirectories)
                |> Seq.filter (fun path -> not (path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")) && not (path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")))

            let offending =
                sourceFiles
                |> Seq.collect (fun file ->
                    File.ReadAllLines(file)
                    |> Seq.mapi (fun index line -> file, index + 1, line.TrimStart())
                    |> Seq.choose (fun (file, lineNumber, line) ->
                        if line.StartsWith("private ", StringComparison.Ordinal)
                           || line.StartsWith("internal ", StringComparison.Ordinal)
                           || line.StartsWith("public ", StringComparison.Ordinal) then
                            Some($"{file}:{lineNumber}: {line}")
                        else
                            None))
                |> Seq.toList

            Expect.isEmpty offending "top-level visibility stays in .fsi files"
        }
    ]
