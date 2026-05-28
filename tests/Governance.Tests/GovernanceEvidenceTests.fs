module GovernanceEvidenceTests

open System
open System.Text.Json
open Expecto
open GovernanceTestSupport

let featureDir = "specs/015-improve-governance-weaknesses"

let containsAll relativePath terms =
    expectFileContains relativePath terms

[<Tests>]
let governanceEvidenceTests =
    testList "governance evidence contracts" [
        test "skill loading evidence records pre-work loads and rejection diagnostics" {
            containsAll
                $"{featureDir}/readiness/skill-loading-evidence.md"
                [ "T001"
                  "speckit-evidence-graph"
                  "loaded"
                  "work_started_at"
                  "declared skill speckit-implement has no pre-work load evidence"
                  "loaded after work started"
                  "resolves to multiple SKILL.md files" ]
        }

        test "task graph reports confidence-based skill assessments" {
            use document = readJson $"{featureDir}/readiness/task-graph.json"

            let task =
                document.RootElement.GetProperty("tasks").EnumerateArray()
                |> Seq.tryFind (fun task -> task.GetProperty("id").GetString() = "T009")
                |> Option.defaultWith (fun () -> failtest "task graph contains T009")

            let rows = task.GetProperty("skill_match_assessments").EnumerateArray() |> Seq.toList

            Expect.isTrue (rows |> List.exists (fun row -> row.GetProperty("confidence").GetString() = "high")) "T009 has high confidence evidence graph assessment"
            Expect.isTrue (rows |> List.exists (fun row -> row.GetProperty("matched_signals").GetArrayLength() > 0)) "T009 reports matched signals"

            containsAll
                $"{featureDir}/readiness/task-graph.md"
                [ "Skill Match Assessments"
                  "Confidence"
                  "Reviewer disposition" ]
        }

        test "skill calibration readiness covers ambiguous indirect false-positive and valid-empty cases" {
            containsAll
                $"{featureDir}/readiness/skill-detection-calibration.md"
                [ "Obvious match accepted"
                  "Ambiguous match"
                  "Indirect capability ownership"
                  "False positive"
                  "Valid empty skill list"
                  "Phase 6 currently keeps `[skillist: []]`" ]
        }

        test "risk-level evidence names proportionate checks and broad boundaries" {
            containsAll
                $"{featureDir}/readiness/governance-risk-levels.md"
                [ "small"
                  "medium"
                  "broad"
                  "./fake.sh build -t EvidenceGraph"
                  "./fake.sh build -t EvidenceAudit"
                  "./fake.sh build -t GeneratedGuidanceCheck"
                  "Broad validation is required only if" ]
        }

        test "aggregate timeout verdict records non-authoritative focused rerun separation" {
            containsAll
                $"{featureDir}/readiness/aggregate-hang-diagnostics.md"
                [ "target"
                  "verdict category"
                  "elapsed duration"
                  "last observed command"
                  "recommended focused rerun"
                  "focused rerun result"
                  "non-authoritative aggregate" ]
        }

        test "runtime limitations document current boundaries without claiming new support" {
            containsAll
                $"{featureDir}/readiness/runtime-limitations.md"
                [ ".NET 10 desktop"
                  "Vulkan"
                  "SkiaSharp preview"
                  "unsupported macOS/mobile/browser"
                  "no software-renderer fallback"
                  "no package/API/runtime support expansion" ]
        }

        test "implementation guidance requires auditable pre-task skill records" {
            containsAll
                ".agents/skills/speckit-implement/SKILL.md"
                [ "readiness/skill-loading-evidence.md"
                  "task id"
                  "skill id"
                  "resolved path"
                  "loaded_at"
                  "work_started_at"
                  "reviewer exception" ]
        }

        test "generated task guidance explains confidence review and risk-level evidence" {
            containsAll
                ".specify/templates/tasks-template.md"
                [ "confidence"
                  "matched signals"
                  "reviewer disposition"
                  "small, medium, and broad"
                  "non-authoritative aggregate"
                  "audit-enforced readiness files"
                  "readiness/governance-risk-levels.md"
                  "readiness/aggregate-hang-diagnostics.md"
                  "readiness/runtime-limitations.md"
                  "readiness/generated-validation-authority.md"
                  "readiness/skill-loading-evidence-workflow.md"
                  "readiness/audit-diagnostics.md"
                  "readiness/readiness-contract-discovery.md"
                  "readiness/framework-guidance.md"
                  "readiness/evidence-vocabulary.md"
                  "readiness/evidence-graph.md"
                  "readiness/evidence-audit.md" ]
        }
    ]
