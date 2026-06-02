module BuildProcessHealth

open System
open System.Diagnostics
open System.IO
open BuildPaths

let tryParseInt64 (value: string) =
    let mutable parsed = 0L

    if Int64.TryParse(value, &parsed) then
        Some parsed
    else
        None

let tryReadLinuxMemAvailableMb () =
    let meminfo = "/proc/meminfo"

    if File.Exists meminfo then
        File.ReadAllLines meminfo
        |> Array.tryPick (fun line ->
            if line.StartsWith("MemAvailable:", StringComparison.Ordinal) then
                line.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.tryItem 1
                |> Option.bind tryParseInt64
                |> Option.map (fun kb -> kb / 1024L)
            else
                None)
    else
        None

let tryReadProcLimit (name: string) =
    let limits = "/proc/self/limits"

    if File.Exists limits then
        File.ReadAllLines limits
        |> Array.tryPick (fun line ->
            if line.StartsWith(name, StringComparison.Ordinal) then
                let remainder = line.Substring(name.Length).Trim()
                let soft =
                    remainder.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries)
                    |> Array.tryHead

                match soft with
                | Some "unlimited" -> None
                | Some value -> tryParseInt64 value
                | None -> None
            else
                None)
    else
        None

let tryCountOpenFileDescriptors () =
    let fd = "/proc/self/fd"

    if Directory.Exists fd then
        try
            Directory.GetFiles(fd).Length |> int64 |> Some
        with _ ->
            None
    else
        None

let tryZombieProcessCount () =
    let proc = "/proc"

    if Directory.Exists proc then
        try
            Directory.GetDirectories proc
            |> Array.choose (fun directory ->
                let name = Path.GetFileName directory
                let isPid = name |> Seq.forall Char.IsDigit

                if isPid then
                    let statPath = path [ directory; "stat" ]

                    try
                        if File.Exists statPath then
                            let stat = File.ReadAllText statPath
                            let closeParen = stat.LastIndexOf(')')

                            if closeParen >= 0 && stat.Length > closeParen + 2 && stat.[closeParen + 2] = 'Z' then
                                Some 1
                            else
                                Some 0
                        else
                            Some 0
                    with _ ->
                        Some 0
                else
                    None)
            |> Array.sum
            |> Some
        with _ ->
            None
    else
        None

let runShortCommand workingDirectory (fileName: string) (arguments: string) (timeoutMs: int) =
    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.WorkingDirectory <- workingDirectory
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    // Feature 049: force the deterministic graphics backend on the short-command child too
    // (no-op unless DualDisplay), matching the BuildProcess spawn-edge guarantee (C2/FR-003).
    let merged =
        [ for entry in startInfo.Environment do
              match entry.Value with
              | null -> ()
              | value -> yield entry.Key, value ]
        |> Map.ofList

    let normalized = BuildEnvironment.normalizeGraphicsEnv merged
    startInfo.Environment.Clear()
    normalized |> Map.iter (fun key value -> startInfo.Environment.[key] <- value)

    try
        use proc =
            match Process.Start startInfo |> Option.ofObj with
            | Some proc -> proc
            | None -> failwithf "Could not start %s %s" fileName arguments

        let stdoutTask = proc.StandardOutput.ReadToEndAsync()
        let stderrTask = proc.StandardError.ReadToEndAsync()

        if proc.WaitForExit timeoutMs then
            proc.ExitCode, stdoutTask.Result + stderrTask.Result
        else
            proc.Kill()
            -1, $"Timed out after {timeoutMs}ms"
    with ex ->
        -1, ex.Message

let thresholdDecision ruleId signal defaultValue comparison actualValue envVar platform =
    let overrideText = Environment.GetEnvironmentVariable envVar
    let reasonText = Environment.GetEnvironmentVariable(envVar + "_REASON")

    let overrideValue, overrideSource, overrideReason, overrideDiagnostic =
        if String.IsNullOrWhiteSpace overrideText then
            None, None, None, None
        else
            match tryParseInt64 overrideText with
            | None ->
                None,
                Some envVar,
                (if String.IsNullOrWhiteSpace reasonText then None else Some reasonText),
                Some $"malformed threshold override {envVar}={overrideText}"
            | Some value when String.IsNullOrWhiteSpace reasonText ->
                Some value, Some envVar, None, Some $"threshold override {envVar} requires {envVar}_REASON"
            | Some value -> Some value, Some envVar, Some reasonText, None

    let threshold = overrideValue |> Option.defaultValue defaultValue

    let passed =
        match overrideDiagnostic, actualValue with
        | Some _, _ -> Some false
        | None, None -> None
        | None, Some actual when comparison = ">=" -> Some(actual >= threshold)
        | None, Some actual when comparison = "<=" -> Some(actual <= threshold)
        | None, Some _ -> Some false

    let diagnostic =
        match overrideDiagnostic, actualValue, passed with
        | Some diagnostic, _, _ -> Some diagnostic
        | None, None, _ -> Some $"unsupported signal {signal} on {platform}"
        | None, Some actual, Some false -> Some $"{ruleId} failed: {signal} actual {actual} must be {comparison} {threshold}"
        | _ -> None

    ruleId,
    signal,
    defaultValue,
    comparison,
    actualValue,
    overrideValue,
    overrideSource,
    overrideReason,
    platform,
    passed,
    diagnostic

let markdownOption value unit =
    match value with
    | Some number -> $"{number}{unit}"
    | None -> "unsupported"
