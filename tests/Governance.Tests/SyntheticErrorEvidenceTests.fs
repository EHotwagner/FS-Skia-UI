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

let private writePersistentRuntimeReadiness root packageResolution generatedVerify =
    writeFeature
        root
        "- [X] T001 [skillist: []] Record persistent GUI runtime evidence\n- [X] T002 [skillist: []] Document persistent GUI runtime evidence"
        ""

    writeFixtureFile root "spec.md" "Persistent GUI Runtime interactive-lifecycle.md package-resolution.md generated-verify.md game-visual-evidence.md" |> ignore
    writeFixtureFile root "plan.md" "persistent-gui-runtime package-resolution.md generated-verify.md game-visual-evidence.md" |> ignore
    writeFixtureFile root "readiness/interactive-lifecycle.md" "mode=interactive-window self-closed-for-evidence=false" |> ignore
    writeFixtureFile root "readiness/evidence-launch-mode.md" "mode=persistent-evidence self-closed-for-evidence=true input-dispatch=observed" |> ignore
    writeFixtureFile root "readiness/container-session-diagnostics.md" "runtime directory display socket session bus unsupported-host reason" |> ignore
    writeFixtureFile root "readiness/package-resolution.md" packageResolution |> ignore
    writeFixtureFile root "readiness/generated-verify.md" generatedVerify |> ignore
    writeFixtureFile root "readiness/game-visual-evidence.md" "supported-host=true evidence-kind=screenshot board-readable=true input-or-progress-observed=true" |> ignore
    writeFixtureFile root "readiness/task-workflow-guidance.md" "implementation batch records red-green evidence log graph before/after skill-loading notes non-authoritative aggregate" |> ignore
    writeFixtureFile root "readiness/evidence-audit.md" "EvidenceGraph EvidenceAudit PASS required readiness acceptance keywords" |> ignore

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

        test "EvidenceAudit Synthetic rejects malformed SEH inventory rows before implementation" {
            use fixture = new TempFixtureDirectory("seh-malformed-row")

            writeFeature
                fixture.Root
                "- [S] T001 [SEH] synthetic-error-handling-approved [skillist: []] Validate malformed readiness row\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | Malformed readiness row lacks governed metadata | infeasible, see spec FR-025 | n/a | synthetic-error-handling-approved |  | malformed readiness rows |  | accepted-seh-pending |"

            let code, stdout, stderr = runAudit fixture.Root
            Expect.equal code 2 $"audit fails: {stdout} {stderr}"
            Expect.stringContains stdout "diagnostic=T001" "audit reports the malformed SEH row task"
            Expect.stringContains stdout "missing design-phase source" "audit identifies missing design source"
            Expect.stringContains stdout "missing expected error behavior" "audit identifies missing expected error behavior"
            Expect.stringContains stdout "missing accepted-seh acceptance status" "audit rejects pending acceptance"
        }

        test "EvidenceAudit Synthetic rejects invalid command arguments with usage diagnostics" {
            let code, stdout, stderr =
                runProcess
                    "bash"
                    ".specify/extensions/evidence/scripts/bash/run-audit.sh --not-a-real-audit-flag"

            Expect.equal code 4 $"invalid flag exits as usage error: {stdout} {stderr}"
            Expect.stringContains stderr "unknown flag: --not-a-real-audit-flag" "invalid command argument is diagnostic"
        }

        test "EvidenceAudit Synthetic rejects missing package resolution fields" {
            use fixture = new TempFixtureDirectory("seh-missing-package-fields")

            writePersistentRuntimeReadiness
                fixture.Root
                "requested-version=1.2.3 resolved-version=1.2.3 package-source=local-feed"
                "generated-tests-exist=true generated-tests-ran=true authoritative=true"

            let code, stdout, stderr = runAudit fixture.Root
            Expect.equal code 2 $"audit fails: {stdout} {stderr}"
            Expect.stringContains stdout "persistent GUI runtime hits" "audit classifies the readiness contract blocker"
            Expect.stringContains stderr "unresolved package mismatch" "missing exact-match package field is rejected"
        }

        test "task graph Synthetic rejects corrupt evidence records" {
            use fixture = new TempFixtureDirectory("seh-corrupt-record")

            writeFeature
                fixture.Root
                "- [S] T001 [SEH] synthetic-error-handling-approved [skillist: []] Validate corrupt evidence record"
                "| T001 | Corrupt evidence record | infeasible, see spec FR-025 | n/a | synthetic-error-handling-approved | specs/018-persistent-gui-runtime/plan.md FR-025 | corrupt evidence records | fail with parse diagnostic | accepted-seh |"

            writeFixtureFile
                fixture.Root
                "tasks.deps.yml"
                "schema_version: \"1.0\"\ntasks:\n  T001:\n    deps: [T999]\n    skillist: []\n"
            |> ignore

            let code, stdout, stderr = runGraph fixture.Root
            Expect.equal code 2 $"graph rejects corrupt evidence record: {stdout} {stderr}"
            Expect.stringContains (stdout + stderr) "T001 depends on T999, which does not exist" "corrupt dependency record is diagnostic"
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
