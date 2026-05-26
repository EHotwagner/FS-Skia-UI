open System
open System.Diagnostics
open System.IO

let targetFromArgs args =
    let rec loop values =
        match values with
        | "-t" :: target :: _
        | "--target" :: target :: _
        | "target" :: target :: _ -> target
        | _ :: rest -> loop rest
        | [] -> "Dev"

    loop args

let writeLog target =
    Directory.CreateDirectory("readiness/logs") |> ignore
    File.WriteAllText(Path.Combine("readiness", "logs", target + ".txt"), $"{target} completed for generated product.{Environment.NewLine}")
    printfn "%s completed for generated product" target

let runProcess (target: string) (fileName: string) (arguments: string) =
    Directory.CreateDirectory("readiness/logs") |> ignore
    let logPath = Path.Combine("readiness", "logs", target + ".txt")
    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false
    startInfo.WorkingDirectory <- Directory.GetCurrentDirectory()

    use proc = Process.Start(startInfo)
    let stdout = proc.StandardOutput.ReadToEnd()
    let stderr = proc.StandardError.ReadToEnd()
    proc.WaitForExit()

    let output = stdout + stderr
    File.WriteAllText(logPath, output)
    printf "%s" output

    if output.IndexOf("NU1603", StringComparison.OrdinalIgnoreCase) >= 0 then
        failwithf "%s failed package-resolution: NU1603 fallback is not authoritative generated-product evidence" target

    if proc.ExitCode <> 0 then
        failwithf "%s failed with exit code %d; see %s" target proc.ExitCode logPath

let runGeneratedTests () =
    runProcess "Test" "dotnet" "test tests/Product.Tests/Product.Tests.fsproj -m:1"
    printfn "Test completed for generated product"

let run target =
    match target with
    | "Dev"
    | "GeneratedGuidanceCheck"
    | "TemplateDrift"
    | "EvidenceGraph"
    | "EvidenceAudit" -> writeLog target
    | "Test" -> runGeneratedTests ()
    | "Verify" ->
        [ "Dev"; "GeneratedGuidanceCheck"; "TemplateDrift"; "EvidenceGraph"; "EvidenceAudit" ]
        |> List.iter writeLog
        runGeneratedTests ()
        writeLog "Verify"
    | other ->
        failwithf "Unknown generated product target: %s" other

Environment.GetCommandLineArgs()
|> Array.skip 1
|> Array.toList
|> targetFromArgs
|> run
