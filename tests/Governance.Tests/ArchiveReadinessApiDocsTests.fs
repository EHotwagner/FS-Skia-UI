module ArchiveReadinessApiDocsTests

open System
open Expecto
open GovernanceTestSupport

let featureDir = "specs/036-archive-readiness-api-docs"

let requiredFields =
    [ "feature-id"
      "path"
      "classification"
      "archival-marker"
      "rationale"
      "preservation-status"
      "owner"
      "replacement-path" ]

[<Tests>]
let archiveReadinessApiDocsTests =
    testList "Archive readiness and API docs governance" [
        test "archive inventory records required sections fields markers and current-gate exclusion" {
            let path = $"{featureDir}/readiness/archive-inventory.md"
            let content = read path

            [ "# Archive Inventory"
              "## Classification Policy"
              "## Current Evidence"
              "## Archived In Place"
              "## Replaced Or Retained"
              "## Removable Candidates"
              "## Informational Historical Findings"
              "historical-readiness-audit-context"
              "must not be presented as satisfying current"
              "retained in place" ]
            |> List.iter (fun term -> Expect.stringContains content term $"{path} contains {term}")

            requiredFields
            |> List.iter (fun field -> Expect.stringContains content field $"{path} contains row field {field}")
        }

        test "current evidence map names authoritative readiness files and archived path policy" {
            let path = $"{featureDir}/readiness/current-evidence-map.md"
            let content = read path

            [ "Historical readiness cannot satisfy current package, template, generated-product, or audit gates"
              "archive-inventory.md"
              "current-evidence-map.md"
              "stale-reference-scan.md"
              "api-reference-generator-evaluation.md"
              "fsharp-formatting-spike.md"
              "generated-guidance-check.md"
              "evidence-graph.md"
              "evidence-audit.md"
              "verification-command" ]
            |> List.iter (fun term -> Expect.stringContains content term $"{path} contains {term}")
        }

        test "Stale reference scanner Synthetic rejects malformed scanner metadata fixtures" {
            let fixtures =
                [ "missing-scan-area", "missing scan-area"
                  "unresolved-path", "unresolved referenced path"
                  "actionless-diagnostic", "diagnostic lacks next action" ]

            fixtures
            |> List.iter (fun (fixture, expected) ->
                let exitCode, stdout, stderr =
                    runProcessUnlocked "dotnet" $"fsi scripts/stale-readiness-reference-scan.fsx --fixture {fixture}"

                Expect.notEqual exitCode 0 $"synthetic fixture {fixture} is rejected"
                Expect.isTrue ((stdout + stderr).Contains(expected, StringComparison.OrdinalIgnoreCase)) $"synthetic fixture {fixture} reports {expected}")
        }

        test "stale reference scan distinguishes active blocking fields from historical informational findings" {
            let path = $"{featureDir}/readiness/stale-reference-scan.md"
            let content = read path

            [ "source-path"
              "referenced-path"
              "scan-area"
              "severity"
              "reason"
              "replacement-path"
              "line"
              "next-action"
              "historical informational"
              "blocking-findings: 0" ]
            |> List.iter (fun term -> Expect.stringContains content term $"{path} contains {term}")
        }

        test "generated guidance explains current evidence archives source shaped fsi and fsdocs authority limits" {
            [ "docs/reports/generated-apps.md"
              "docs/reports/template-profile.md"
              "template/base/README.md"
              "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "current feature readiness paths are authoritative"
                      "historical feature readiness is audit context only"
                      "Archived material must not be cited as current"
                      "source-shaped `.fsi` package API reference remains authoritative"
                      "FSharp.Formatting/fsdocs output is secondary or hybrid"
                      "Package consumers must not use assembly reflection or repository source inspection" ])
        }

        test "generated guidance check report records validation results and replacement instructions" {
            expectFileContains
                $"{featureDir}/readiness/generated-guidance-check.md"
                [ "Generated Guidance Check"
                  "docs/generated-apps.md"
                  "docs/template-profile.md"
                  "template/base/README.md"
                  "template/base/docs/product.md"
                  "replacement instruction"
                  "source-shaped `.fsi` package API reference remains authoritative" ]
        }
    ]
