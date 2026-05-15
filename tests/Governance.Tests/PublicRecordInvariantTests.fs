module PublicRecordInvariantTests

open System
open System.Text.RegularExpressions
open Expecto
open GovernanceTestSupport

let signatureFiles =
    [ "src/Lib/Library.fsi"
      "src/Lib/KeyboardInput.fsi"
      "src/Layout/Types.fsi"
      "src/Charts/Types.fsi"
      "src/Charts/Candlestick.fsi" ]

let typeRegex = Regex(@"^type\s+([A-Za-z0-9_']+)", RegexOptions.Compiled)

let recordNames relativePath =
    let lines = (read relativePath).Replace("\r\n", "\n").Split('\n')

    (([], None), lines)
    ||> Array.fold (fun (records, pending) line ->
        let trimmed = line.TrimStart()
        let typeMatch = typeRegex.Match(trimmed)

        if typeMatch.Success then
            records, Some typeMatch.Groups[1].Value
        elif trimmed.StartsWith("{", StringComparison.Ordinal) then
            match pending with
            | Some name -> name :: records, None
            | None -> records, None
        elif trimmed.StartsWith("|", StringComparison.Ordinal) || trimmed.StartsWith("module ", StringComparison.Ordinal) then
            records, None
        else
            records, pending)
    |> fst
    |> List.rev

let inventoryRows () =
    let inventoryPath = "specs/008-targeted-refactor-governance/readiness/record-invariants.md"

    if fileExists inventoryPath then
        markdownTableRows inventoryPath
        |> List.filter (fun row -> row.Length >= 8 && row[0] <> "Package")
    else
        []

[<Tests>]
let publicRecordInvariantTests =
    testList "Public record invariant inventory" [
        test "every public record in signature files has an inventory row" {
            if not (fileExists "specs/008-targeted-refactor-governance/readiness/record-invariants.md") then
                Expect.isFalse (directoryExists "specs/008-targeted-refactor-governance") "source feature record inventory is absent from generated projects"
            else
                let inventoryRecords =
                    inventoryRows ()
                    |> List.map (fun row -> row[1])
                    |> Set.ofList

                signatureFiles
                |> List.collect recordNames
                |> List.distinct
                |> List.iter (fun recordName -> Expect.contains inventoryRecords recordName $"inventory contains {recordName}")
        }

        test "helper or validation-first recommendations carry a stable follow-up id" {
            if not (fileExists "specs/008-targeted-refactor-governance/readiness/record-invariants.md") then
                Expect.isFalse (directoryExists "specs/008-targeted-refactor-governance") "source feature record inventory is absent from generated projects"
            else
                inventoryRows ()
                |> List.iter (fun row ->
                    let decision = row[5]
                    let followUp = row[7]

                    if decision.Contains("Recommend", StringComparison.OrdinalIgnoreCase) then
                        Expect.isTrue (followUp.StartsWith("API-", StringComparison.Ordinal)) $"{row[1]} recommendation has follow-up id")
        }
    ]
