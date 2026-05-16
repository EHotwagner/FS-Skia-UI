module ControlsPublicSurfaceTests

open System.IO
open Expecto

let repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "build.fsx")) then
            dir
        else
            Directory.GetParent(dir) |> Option.ofObj |> Option.map _.FullName |> Option.defaultValue dir |> find
    find __SOURCE_DIRECTORY__

[<Tests>]
let publicSurfaceTests =
    testList "Controls public surface files" [
        test "all required signature files exist" {
            [ "Types.fsi"; "Control.fsi"; "Attributes.fsi"; "Theme.fsi"; "Accessibility.fsi"; "Diagnostics.fsi"; "Catalog.fsi"; "TextInput.fsi"; "Collections.fsi"; "Charts.fsi"; "CustomControl.fsi" ]
            |> List.iter (fun file ->
                let path = Path.Combine(repositoryRoot, "src", "Controls", file)
                Expect.isTrue (File.Exists path) $"{file} exists")
        }
    ]
