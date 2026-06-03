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

    // Feature 049: overlay the caller's map onto the inherited ambient environment, then force
    // the deterministic graphics backend (no-op unless the host is DualDisplay). Re-applying at
    // the spawn edge guarantees the child is normalized independent of inheritance (C2/FR-003).
    // The child's exit code is never rewritten, so genuine failures still surface (C5/FR-008).
    environment
    |> Map.iter (fun key value -> startInfo.Environment.[key] <- value)

    let merged =
        [ for entry in startInfo.Environment do
              match Option.ofObj entry.Value with
              | None -> ()
              | Some value -> yield entry.Key, value ]
        |> Map.ofList

    let normalized = BuildEnvironment.normalizeGraphicsEnv merged
    startInfo.Environment.Clear()
    normalized |> Map.iter (fun key value -> startInfo.Environment.[key] <- value)

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
        // Feature 049: the existing 30-minute bound is unchanged; on a kill, enrich the diagnostic
        // so a no-usable-backend failure fails fast and legibly instead of looking like a product
        // regression (C4/FR-005). The child's exit code is not rewritten (C5/FR-008).
        proc.Kill()
        let diagnostic = BuildEnvironment.graphicsTimeoutDiagnostic label outputPath
        File.AppendAllText(outputPath, "\n" + diagnostic + "\n")
        failwith diagnostic

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
        |> Option.map (fun file -> Path.GetFileName file |> Option.ofObj |> Option.defaultValue "")

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
            let normalizedProject = project.Replace('\\', '/')
            // Native-GUI Expecto suites bypass the VSTest/YoloDev adapter (libdecor-gtk testhost crash
            // under a dual Wayland/X11 display, 049) and run as direct Expecto executables.
            if action = "test" && normalizedProject.EndsWith("tests/Smoke.Tests/Smoke.Tests.fsproj", StringComparison.Ordinal) then
                runProcess $"{label} {project}" "dotnet" $"run --project {quote project} --no-restore" root outputPath Map.empty
            elif action = "test" && normalizedProject.EndsWith("tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj", StringComparison.Ordinal) then
                runProcess $"{label} {project}" "dotnet" $"run --project {quote project} --no-restore -- --sequenced" root outputPath Map.empty
            else
                let arguments =
                    [ action; quote project; extraArguments ]
                    |> List.filter (fun part -> part <> "")
                    |> String.concat " "

                runProcess $"{label} {project}" "dotnet" arguments root outputPath Map.empty)
