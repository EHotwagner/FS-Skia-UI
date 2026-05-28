module BuildProcess

open System
open System.Diagnostics
open System.IO
open BuildPaths

let private quote (value: string) =
    "\"" + value.Replace("\"", "\\\"") + "\""

let runProcessWithAllowedExitCodes (label: string) (fileName: string) (arguments: string) (workingDirectory: string) (outputPath: string) (environment: Map<string, string>) (allowedExitCodes: Set<int>) =
    ensureParent outputPath
    File.AppendAllText(outputPath, $"\n## {label}\n$ {fileName} {arguments}\n")

    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.WorkingDirectory <- workingDirectory
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    environment
    |> Map.iter (fun key value -> startInfo.Environment.[key] <- value)

    use proc =
        match Process.Start startInfo |> Option.ofObj with
        | Some proc -> proc
        | None -> failwithf "Could not start %s %s" fileName arguments

    let stdoutTask = proc.StandardOutput.ReadToEndAsync()
    let stderrTask = proc.StandardError.ReadToEndAsync()

    if proc.WaitForExit(30 * 60 * 1000) then
        let stdout = stdoutTask.Result
        let stderr = stderrTask.Result
        File.AppendAllText(outputPath, stdout)
        File.AppendAllText(outputPath, stderr)
        File.AppendAllText(outputPath, $"\nexit-code={proc.ExitCode}\n")

        if allowedExitCodes |> Set.contains proc.ExitCode |> not then
            failwithf "%s failed with exit code %d. See %s" label proc.ExitCode outputPath
    else
        proc.Kill()
        failwithf "%s timed out. See %s" label outputPath

let runProcess label fileName arguments workingDirectory outputPath environment =
    runProcessWithAllowedExitCodes label fileName arguments workingDirectory outputPath environment (Set.singleton 0)

let existingProjects root projects =
    projects
    |> List.filter (fun project -> File.Exists(path [ root; project ]))

let solutionFor root preferredSolution =
    let preferred = path [ root; preferredSolution ]

    if File.Exists preferred then
        Some preferredSolution
    else
        Directory.GetFiles(root, "*.sln")
        |> Array.tryHead
        |> Option.map Path.GetFileName

let runDotnetAction label action solutionFile projects extraArguments outputPath root =
    let existing = existingProjects root projects

    if List.isEmpty existing then
        match solutionFor root solutionFile with
        | Some solution ->
            let arguments =
                [ action; quote solution; extraArguments ]
                |> List.filter (fun part -> part <> "")
                |> String.concat " "

            runProcess label "dotnet" arguments root outputPath Map.empty
        | None ->
            failwithf "No projects were found for %s. Checked: %s" label (String.Join(", ", projects))
    else
        existing
        |> List.iter (fun project ->
            if action = "test" && project.Replace('\\', '/').EndsWith("tests/Smoke.Tests/Smoke.Tests.fsproj", StringComparison.Ordinal) then
                runProcess $"{label} {project}" "dotnet" $"run --project {quote project} --no-restore" root outputPath Map.empty
            else
                let arguments =
                    [ action; quote project; extraArguments ]
                    |> List.filter (fun part -> part <> "")
                    |> String.concat " "

                runProcess $"{label} {project}" "dotnet" arguments root outputPath Map.empty)
