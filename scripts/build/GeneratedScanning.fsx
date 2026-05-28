module BuildGeneratedScanning

open System
open System.Diagnostics
open System.IO

let quote (value: string) =
    "\"" + value.Replace("\"", "\\\"") + "\""

let fileShouldBeScanned (filePath: string) =
    let normalized = filePath.Replace('\\', '/')
    [ "/bin/"; "/obj/"; "/.fake/"; "/.git/"; "/.template.config/"; "/readiness/logs/" ]
    |> List.exists (fun segment -> normalized.IndexOf(segment, StringComparison.Ordinal) >= 0)
    |> not

let isWindows =
    Path.DirectorySeparatorChar = '\\'

let hasUserExecutePermission filePath =
    if isWindows then
        true
    else
        let startInfo = ProcessStartInfo("test", $"-x {quote filePath}")
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false

        use proc =
            match Process.Start startInfo |> Option.ofObj with
            | Some proc -> proc
            | None -> failwith "Could not start test -x"

        proc.WaitForExit(30 * 1000) && proc.ExitCode = 0
