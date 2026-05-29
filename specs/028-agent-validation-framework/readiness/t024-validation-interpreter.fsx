#r "../../../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open System
open System.IO
open FS.Skia.UI.AgentValidation

let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", ".."))

let inputs =
    { RepositoryRoot = repoRoot
      FeatureMetadataPath = Path.Combine(repoRoot, ".specify", "feature.json")
      ValidationContractPath = Path.Combine(repoRoot, "validation.contract.yml")
      SelectionReportPath = Path.Combine(__SOURCE_DIRECTORY__, "validation-selection-report.md")
      BaseRef = Some "master" }

let rec drain model effects =
    match effects with
    | [] -> model
    | effect :: rest ->
        printfn "effect: %A" effect

        match ValidationSelectionInterpreter.interpret inputs effect model with
        | None ->
            printfn "effect-result: report-written"
            drain model rest
        | Some msg ->
            printfn "message: %A" msg
            let next, nextEffects = ValidationSelection.update msg model
            drain next (rest @ nextEffects)

let initial, effects = ValidationSelection.init (Some "028-agent-validation-framework")
let final = drain initial effects

printfn "feature: %A" final.Feature
printfn "changed-path-source: %A" (final.ChangedPathSource |> Option.map _.Kind)
printfn "selected-rule-ids: %A" final.SelectedRuleIds
printfn "required-gates: %A" final.RequiredGates
printfn "authority: %A" final.Authority
printfn "degraded: %b" final.Degraded
printfn "report-exists: %b" (File.Exists inputs.SelectionReportPath)

if final.Degraded then
    failwithf "selection degraded unexpectedly: %A" final.Diagnostics

if final.SelectedRuleIds.IsEmpty then
    failwith "expected at least one selected rule"

if not (File.Exists inputs.SelectionReportPath) then
    failwith "expected validation selection report"
