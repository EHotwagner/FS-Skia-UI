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

        test "FS.Skia.UI.Charts baseline exports expected contract names" {
            assertBaseline "FS.Skia.UI.Charts" typeof<FS.Skia.UI.Charts.ChartConfig>.Assembly
        }

        test "FS.Skia.UI.Layout baseline exports expected contract names" {
            assertBaseline "FS.Skia.UI.Layout" typeof<FS.Skia.UI.Layout.GraphDefinition>.Assembly
        }
    ]
