#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Charts/bin/Debug/net10.0/FS.Skia.UI.Charts.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"

open System
open System.IO
open System.Reflection

let names (assembly: Assembly) =
    assembly.GetExportedTypes()
    |> Array.map (fun ty ->
        match ty.FullName with
        | null -> ty.Name
        | fullName when ty.Name.EndsWith("Module", StringComparison.Ordinal) -> fullName.Replace("Module", "")
        | fullName -> fullName)
    |> Array.distinct
    |> Array.sort

let write packageName values =
    let path =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "readiness", "surface-baselines", packageName + ".txt")

    Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
    File.WriteAllLines(path, values)
    printfn "wrote %s" path

write "FS.Skia.UI" (names typeof<FS.Skia.UI.ViewerProgram<int, int>>.Assembly)
write "FS.Skia.UI.Charts" (names typeof<FS.Skia.UI.Charts.ChartConfig>.Assembly)
write "FS.Skia.UI.Layout" (names typeof<FS.Skia.UI.Layout.GraphDefinition>.Assembly)
