module SyntheticErrorEvidenceTests

(* SYNTHETIC FIXTURE: these tests create malformed-input task graphs to verify
   the governed [SEH] audit contract; no product behavior is faked. *)

open System
open System.IO
open Expecto
open GovernanceTestSupport

let private writeStandardReadiness root =
    let readiness = Path.Combine(root, "readiness")
    Directory.CreateDirectory(readiness) |> ignore
    writeFixtureFile root "readiness/governance-risk-levels.md" "small medium broad required evidence broad validation" |> ignore
    writeFixtureFile root "readiness/aggregate-hang-diagnostics.md" "verdict stage elapsed duration last observed command focused rerun non-authoritative aggregate" |> ignore
    writeFixtureFile root "readiness/runtime-limitations.md" ".NET 10 desktop Vulkan SkiaSharp preview unsupported macOS/mobile/browser no software-renderer fallback" |> ignore

let private writeFeature root tasks inventoryRows =
    writeStandardReadiness root

    writeFixtureFile
        root
        "tasks.md"
        $"""# Tasks: Synthetic Fixture

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only
- `[F]` — failed
- `[-]` — skipped

## Phase 1: Setup

{tasks}

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
{inventoryRows}
"""
    |> ignore

    writeFixtureFile
        root
        "tasks.deps.yml"
        """schema_version: "1.0"
tasks:
  T001:
    deps: []
    skillist: []
  T002:
    deps: []
    skillist: []
"""
    |> ignore

let private runAudit root =
    runProcess
        "bash"
        $".specify/extensions/evidence/scripts/bash/run-audit.sh \"{root}\" --base HEAD"

let private runGraph root =
    runProcess
        "python3"
        $".specify/extensions/evidence/scripts/python/compute-task-graph.py \"{root}\""

[<Tests>]
let syntheticErrorEvidenceTests =
    testList "Synthetic error evidence governance" [
        test "task graph Synthetic reports accepted SEH metadata and counts" {
            use fixture = new TempFixtureDirectory("seh-graph")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate malformed parser rejection\n- [X] T002 [skillist: []] Document downstream real review"
                "| T001 | Malformed parser input cannot come from a successful real flow | infeasible, see spec FR-004 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T012 | malformed parser input | reject with parser diagnostic | accepted-seh |"

            writeFixtureFile
                fixture.Root
                "tasks.deps.yml"
                """schema_version: "1.0"
tasks:
  T001:
    deps: []
    skillist: []
  T002:
    deps: [T001]
    skillist: []
"""
            |> ignore

            let code, stdout, stderr = runGraph fixture.Root
            Expect.equal code 0 $"graph succeeds: {stdout} {stderr}"

            let graph = File.ReadAllText(Path.Combine(fixture.Root, "readiness", "task-graph.md"))
            Expect.stringContains graph "accepted [SEH] synthetic" "graph reports accepted synthetic error-handling count"
            Expect.stringContains graph "malformed parser input" "graph reports synthetic input class"
            Expect.isFalse (graph.Contains("T002 [S*]")) "accepted SEH dependency does not taint real downstream task"
        }

        test "EvidenceAudit Synthetic passes when every synthetic task is valid design-approved SEH" {
            use fixture = new TempFixtureDirectory("seh-pass")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate corrupt file rejection\n- [X] T002 [skillist: []] Document accepted audit report"
                "| T001 | Corrupt file content is the error condition, not a real successful input | infeasible, see spec FR-004 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T019 | corrupt file content | fail with actionable diagnostic | accepted-seh |"

            let code, stdout, stderr = runAudit fixture.Root
            Expect.equal code 0 $"audit passes: {stdout} {stderr}"
            Expect.stringContains stdout "verdict=PASS" "audit emits pass verdict"
            Expect.stringContains stdout "accepted-seh-tasks=1" "audit counts accepted SEH tasks"
            Expect.stringContains stdout "unaccepted-synthetic-tasks=0" "audit reports no unaccepted synthetic tasks"
        }

        test "EvidenceAudit Synthetic rejects ordinary synthetic tasks" {
            use fixture = new TempFixtureDirectory("seh-ordinary-fail")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [skillist: []] Validate with convenience mock\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | Convenience mock avoids real integration | real integration smoke | n/a |  | specs/017-synthetic-error-evidence/tasks.md:T030 | convenience mock | return canned success | blocking |"

            let code, stdout, stderr = runAudit fixture.Root
            Expect.equal code 2 $"audit fails: {stdout} {stderr}"
            Expect.stringContains stdout "verdict=FAIL" "audit emits fail verdict"
            Expect.stringContains stdout "unaccepted-synthetic-tasks=1" "ordinary synthetic task remains blocking"
        }

        test "EvidenceAudit Synthetic rejects late or non-eligible SEH classification" {
            use fixture = new TempFixtureDirectory("seh-late-fail")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate placeholder output shortcut\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | placeholder output shortcut added after audit failure | real product output required | n/a | synthetic-error-handling-approved | implementation readiness cleanup after audit failure | placeholder output | return canned placeholder | accepted-seh |"

            let code, stdout, stderr = runAudit fixture.Root
            Expect.equal code 2 $"audit fails: {stdout} {stderr}"
            Expect.stringContains stdout "late-seh-tasks=1" "late SEH task is counted"
            Expect.stringContains stdout "non-eligible synthetic evidence class" "non-eligible classification is diagnostic"
            Expect.stringContains stdout "Return to design/task generation" "diagnostic directs contributor back to planning"
        }

        test "guidance Synthetic documents eligible and non-eligible SEH examples" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.implement.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "[SEH]"
                      "synthetic-error-handling-approved"
                      "malformed parser input"
                      "convenience mocks"
                      "implementation-time relabeling" ])
        }
    ]
