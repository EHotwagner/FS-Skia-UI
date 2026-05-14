module GovernanceTestSupport

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

let fullPath (relativePath: string) =
    Path.Combine(repositoryRoot, relativePath.Replace("/", string Path.DirectorySeparatorChar))

let read (relativePath: string) =
    File.ReadAllText(fullPath relativePath)

let expectContains (content: string) (needle: string) (context: string) =
    Expect.stringContains content needle context

let expectFileContains (relativePath: string) (needles: string list) =
    let content = read relativePath

    needles
    |> List.iter (fun needle -> expectContains content needle $"{relativePath} contains {needle}")

let runProcess (fileName: string) (arguments: string) =
    let executable =
        if fileName.StartsWith("./", StringComparison.Ordinal) then
            fullPath (fileName.Substring 2)
        else
            fileName

    let startInfo: ProcessStartInfo = ProcessStartInfo(executable, arguments)
    startInfo.WorkingDirectory <- repositoryRoot
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    use proc =
        match Process.Start(startInfo) |> Option.ofObj with
        | Some proc -> proc
        | None -> failwithf "Could not start %s %s" fileName arguments

    let stdout = proc.StandardOutput.ReadToEnd()
    let stderr = proc.StandardError.ReadToEnd()

    if proc.WaitForExit(240000) then
        proc.ExitCode, stdout, stderr
    else
        proc.Kill(true)
        -1, stdout, stderr
