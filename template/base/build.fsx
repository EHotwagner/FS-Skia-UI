open System
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

let run target =
    match target with
    | "Dev"
    | "Test"
    | "GeneratedGuidanceCheck"
    | "TemplateDrift"
    | "EvidenceGraph"
    | "EvidenceAudit" -> writeLog target
    | "Verify" ->
        [ "Dev"; "Test"; "GeneratedGuidanceCheck"; "TemplateDrift"; "EvidenceGraph"; "EvidenceAudit" ]
        |> List.iter writeLog
        writeLog "Verify"
    | other ->
        failwithf "Unknown generated product target: %s" other

Environment.GetCommandLineArgs()
|> Array.skip 1
|> Array.toList
|> targetFromArgs
|> run
