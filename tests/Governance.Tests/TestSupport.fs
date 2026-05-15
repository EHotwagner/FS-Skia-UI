module GovernanceTestSupport

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open System.Xml.Linq
open Expecto

let rec findRepositoryRoot (directory: string) =
    if Directory.GetFiles(directory, "*.sln").Length > 0 || File.Exists(Path.Combine(directory, "build.fsx")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

let fullPath (relativePath: string) =
    Path.Combine(repositoryRoot, relativePath.Replace("/", string Path.DirectorySeparatorChar))

let fileExists relativePath =
    File.Exists(fullPath relativePath)

let directoryExists relativePath =
    Directory.Exists(fullPath relativePath)

let read (relativePath: string) =
    File.ReadAllText(fullPath relativePath)

let readJson (relativePath: string) =
    JsonDocument.Parse(read relativePath)

let readXml (relativePath: string) =
    XDocument.Load(fullPath relativePath)

let expectContains (content: string) (needle: string) (context: string) =
    Expect.stringContains content needle context

let expectFileContains (relativePath: string) (needles: string list) =
    let content = read relativePath

    needles
    |> List.iter (fun needle -> expectContains content needle $"{relativePath} contains {needle}")

let runProcess (fileName: string) (arguments: string) =
    let executable, processArguments =
        if fileName = "./fake.sh" || fileName = "fake.sh" then
            let scriptPath = fullPath "fake.sh"
            "bash", $"\"{scriptPath}\" {arguments}"
        elif fileName.StartsWith("./", StringComparison.Ordinal) then
            fullPath (fileName.Substring 2), arguments
        else
            fileName, arguments

    let startInfo: ProcessStartInfo = ProcessStartInfo(executable, processArguments)
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
        proc.Kill()
        -1, stdout, stderr

let runFakeTarget target =
    runProcess "./fake.sh" $"build -t {target}"

let projectFiles () =
    Directory.EnumerateFiles(repositoryRoot, "*.fsproj", SearchOption.AllDirectories)
    |> Seq.filter (fun file ->
        let relative =
            file.Substring(repositoryRoot.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace('\\', '/')

        (relative.StartsWith("src/", StringComparison.Ordinal)
         || relative.StartsWith("tests/", StringComparison.Ordinal)
         || relative.StartsWith("samples/", StringComparison.Ordinal))
        && not (relative.Contains("/bin/"))
        && not (relative.Contains("/obj/")))
    |> Seq.toList
