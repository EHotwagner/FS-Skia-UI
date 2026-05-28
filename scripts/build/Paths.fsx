module BuildPaths

open System
open System.IO

let path segments =
    segments |> Array.ofList |> Path.Combine

let ensureParent (filePath: string) =
    match Path.GetDirectoryName filePath |> Option.ofObj with
    | Some directory when directory <> "" -> Directory.CreateDirectory directory |> ignore
    | _ -> ()

let cleanDirectoryContents directory =
    if Directory.Exists directory then
        Directory.GetFiles(directory)
        |> Array.iter File.Delete

        Directory.GetDirectories(directory)
        |> Array.iter (fun child -> Directory.Delete(child, true))
    else
        Directory.CreateDirectory directory |> ignore

let appendLine outputPath line =
    ensureParent outputPath
    File.AppendAllText(outputPath, line + Environment.NewLine)
