module SyntheticErrorEvidenceTests

(* SYNTHETIC FIXTURE: these tests create malformed-input task graphs to verify
   the governed [SEH] audit contract; no product behavior is faked.

   Feature 043 (T025): re-pointed off `python3 compute-task-graph.py` /
   `bash run-audit.sh` onto the typed `Evidence.Engine`. The committed fixture
   inputs are unchanged; each test now asserts the typed `GraphResult` /
   `AuditResult` and the diagnostic vocabulary in the rendered artifacts. *)

open System
open System.IO
open Expecto
open GovernanceTestSupport
open FS.Skia.UI.Build.Evidence

/// The union of every rendered audit artifact — the in-process equivalent of the
/// legacy audit's stdout+stderr stream that the older tests scraped.
let private auditText (a: AuditArtifacts) =
    String.concat
        "\n"
        [ a.ReadinessContractHits
          a.PersistentLaunchHits
          a.PersistentGuiRuntimeHits
          a.WindowVisibilityHits
          a.AuditStatusHits
          a.DiffScanHits
          a.SehAuditSummary
          a.TaskGraphMd ]

let private writeStandardReadiness root =
    let readiness = Path.Combine(root, "readiness")
    Directory.CreateDirectory(readiness) |> ignore
    writeFixtureFile root "readiness/governance-risk-levels.md" "small medium broad required evidence broad validation" |> ignore
    writeFixtureFile root "readiness/aggregate-hang-diagnostics.md" "verdict stage elapsed duration last observed command focused rerun non-authoritative aggregate" |> ignore
    writeFixtureFile root "readiness/runtime-limitations.md" ".NET 10 desktop OpenGL SkiaSharp preview unsupported macOS/mobile/browser no software-renderer fallback" |> ignore

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

let private writeWindowVisibilityReadiness root overridesByFile =
    writeFeature
        root
        "- [X] T001 [skillist: []] Record window visibility validation fixture\n- [X] T002 [skillist: []] Document window visibility validation fixture"
        ""

    writeFixtureFile root "spec.md" "Fix Window Visibility interactive-visible-window.md close-reason-separation.md real-image-evidence.md generated-validation.md" |> ignore
    writeFixtureFile root "plan.md" "Fix Window Visibility generated-validation.md real-image-evidence.md" |> ignore

    let defaults =
        [ "interactive-visible-window.md", "status=ok\nmode=interactive-window\nwindow-visible=observed:true\naccessible-window=true\nfirst-frame-presented=true\nself-closed-for-evidence=false\n"
          "close-reason-separation.md", "close-reason=user-close\nuser-close-observed=true\nevidence-close-observed=false\n"
          "window-state-diagnostics.md", "failure-class=none\nvisible=observed:true\nfocusable=observed:true\nsurface=observed:true\n"
          "window-options.md", "resize-policy=requested observed honored\nstartup-state=normal observed honored\n"
          "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=screenshot\nartifact-kind=image\nartifact-decodable=true\nimage-artifact=readiness/artifacts/window.png\n"
          "generated-validation.md", "exact-package-match=true\ngenerated-tests-exist=true\ngenerated-tests-ran=true\nauthoritative=true\nfailure-class=none\n"
          "evidence-audit.md", "verdict=fixture\n" ]

    let overrideMap = overridesByFile |> Map.ofList

    defaults
    |> List.iter (fun (fileName, content) ->
        writeFixtureFile root $"readiness/{fileName}" (overrideMap |> Map.tryFind fileName |> Option.defaultValue content) |> ignore)

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

            let gr, graphArts = runEvidenceGraphAt fixture.Root
            let grErrors = String.concat "; " gr.Errors
            Expect.equal gr.Verdict GraphVerdict.Ok $"graph succeeds: {grErrors}"

            Expect.stringContains graphArts.TaskGraphMd "accepted [SEH] synthetic" "graph reports accepted synthetic error-handling count"
            Expect.stringContains graphArts.TaskGraphMd "malformed parser input" "graph reports synthetic input class"
            Expect.isFalse (graphArts.TaskGraphMd.Contains("T002 [S*]")) "accepted SEH dependency does not taint real downstream task"
        }

        test "EvidenceAudit Synthetic passes when every synthetic task is valid design-approved SEH" {
            use fixture = new TempFixtureDirectory("seh-pass")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate corrupt file rejection\n- [X] T002 [skillist: []] Document accepted audit report"
                "| T001 | Corrupt file content is the error condition, not a real successful input | infeasible, see spec FR-004 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T019 | corrupt file content | fail with actionable diagnostic | accepted-seh |"

            let res, _ = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Pass "audit emits pass verdict"
            Expect.equal (List.length res.SehSummary.AcceptedSehTasks) 1 "audit counts accepted SEH tasks"
            Expect.isEmpty res.SehSummary.UnacceptedSyntheticTasks "audit reports no unaccepted synthetic tasks"
        }

        test "EvidenceAudit Synthetic rejects ordinary synthetic tasks" {
            use fixture = new TempFixtureDirectory("seh-ordinary-fail")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [skillist: []] Validate with convenience mock\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | Convenience mock avoids real integration | real integration smoke | n/a |  | specs/017-synthetic-error-evidence/tasks.md:T030 | convenience mock | return canned success | blocking |"

            let res, _ = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit emits fail verdict"
            Expect.equal (List.length res.SehSummary.UnacceptedSyntheticTasks) 1 "ordinary synthetic task remains blocking"
        }

        test "EvidenceAudit Synthetic rejects late or non-eligible SEH classification" {
            use fixture = new TempFixtureDirectory("seh-late-fail")

            writeFeature
                fixture.Root
                "- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate placeholder output shortcut\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | placeholder output shortcut added after audit failure | real product output required | n/a | synthetic-error-handling-approved | implementation readiness cleanup after audit failure | placeholder output | return canned placeholder | accepted-seh |"

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on late/non-eligible SEH"
            Expect.equal (List.length res.SehSummary.LateSehTasks) 1 "late SEH task is counted"
            Expect.stringContains arts.SehAuditSummary "non-eligible synthetic evidence class" "non-eligible classification is diagnostic"
            Expect.stringContains arts.SehAuditSummary "Return to design/task generation" "diagnostic directs contributor back to planning"
        }

        test "EvidenceAudit Synthetic rejects malformed SEH inventory rows before implementation" {
            use fixture = new TempFixtureDirectory("seh-malformed-row")

            writeFeature
                fixture.Root
                "- [S] T001 [SEH] synthetic-error-handling-approved [skillist: []] Validate malformed readiness row\n- [X] T002 [skillist: []] Document rejection"
                "| T001 | Malformed readiness row lacks governed metadata | infeasible, see spec FR-025 | n/a | synthetic-error-handling-approved |  | malformed readiness rows |  | accepted-seh-pending |"

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on malformed SEH inventory row"
            Expect.isTrue (res.SehSummary.Diagnostics |> List.exists (fun (t, _, _, _) -> t = "T001")) "audit reports the malformed SEH row task"
            Expect.stringContains arts.SehAuditSummary "missing design-phase source" "audit identifies missing design source"
            Expect.stringContains arts.SehAuditSummary "missing expected error behavior" "audit identifies missing expected error behavior"
            Expect.stringContains arts.SehAuditSummary "missing accepted-seh acceptance status" "audit rejects pending acceptance"
        }

        test "EvidenceAudit Synthetic rejects missing package resolution fields" {
            use fixture = new TempFixtureDirectory("seh-missing-package-fields")

            writePersistentRuntimeReadiness
                fixture.Root
                "requested-version=1.2.3 resolved-version=1.2.3 package-source=local-feed"
                "generated-tests-exist=true generated-tests-ran=true authoritative=true"

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on missing package resolution fields"
            Expect.isTrue (res.PersistentGuiRuntime > 0) "audit classifies the readiness contract blocker"
            Expect.stringContains (auditText arts) "unresolved package mismatch" "missing exact-match package field is rejected"
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

            let gr, _ = runEvidenceGraphAt fixture.Root
            Expect.equal gr.Verdict GraphVerdict.Error "graph rejects corrupt evidence record"
            Expect.stringContains (String.concat "\n" gr.Errors) "T001 depends on T999, which does not exist" "corrupt dependency record is diagnostic"
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

        test "EvidenceAudit Synthetic rejects corrupt image metadata records" {
            use fixture = new TempFixtureDirectory("seh-corrupt-image-metadata")

            writeWindowVisibilityReadiness
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=screenshot\nartifact-kind=image\nartifact-decodable={not-json}\nimage-artifact=readiness/artifacts/window.png\n" ]

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on corrupt image metadata"
            Expect.stringContains (auditText arts) "corrupt image metadata record" "audit reports corrupt image metadata"
        }

        test "EvidenceAudit Synthetic rejects missing generated-validation fields" {
            use fixture = new TempFixtureDirectory("seh-missing-generated-validation")

            writeWindowVisibilityReadiness
                fixture.Root
                [ "generated-validation.md", "exact-package-match=true\nauthoritative=true\n" ]

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on missing generated-validation fields"
            Expect.stringContains (auditText arts) "missing generated validation fields" "audit reports missing generated-validation fields"
        }

        test "EvidenceAudit Synthetic rejects hostile artifact paths" {
            use fixture = new TempFixtureDirectory("seh-hostile-artifact-path")

            writeWindowVisibilityReadiness
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=screenshot\nartifact-kind=image\nartifact-decodable=true\nimage-artifact=../../outside-readiness/window.png\n" ]

            let res, arts = runEvidenceAuditAt fixture.Root
            Expect.equal res.Verdict AuditVerdict.Fail "audit fails on hostile artifact path"
            Expect.stringContains (auditText arts) "hostile artifact path" "audit reports hostile artifact paths"
        }
    ]
