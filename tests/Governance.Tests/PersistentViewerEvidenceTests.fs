module PersistentViewerEvidenceTests

open Expecto
open GovernanceTestSupport
open System.IO

let private minimalPersistentFeature (root: string) =
    writeFixtureFile
        root
        "tasks.md"
        """# Tasks: Persistent Viewer Fixture

## Phase 1: Setup

- [X] T001 [skillist: []] Capture supported-host persistent launch evidence
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
"""
    |> ignore

let private runEvidenceAudit featureDir =
    let script = fullPath ".specify/extensions/evidence/scripts/bash/run-audit.sh"
    runProcess "bash" $"{script} {featureDir}"

let private writeReadiness root relativePath content =
    writeFixtureFile root $"readiness/{relativePath}" content |> ignore

let private writePersistentGuiRuntimeFixture root overridesByFile =
    writeFixtureFile root "spec.md" "# Persistent GUI Runtime\n\nRequires interactive-lifecycle.md package-resolution.md generated-verify.md game-visual-evidence.md\n" |> ignore
    writeFixtureFile root "plan.md" "# Persistent GUI Runtime Plan\n" |> ignore
    writeFixtureFile root "tasks.md" "# Tasks\n\n- [X] T001 [skillist: []] Persistent GUI Runtime fixture\n" |> ignore
    writeFixtureFile root "tasks.deps.yml" "schema_version: \"1.0\"\n\ntasks:\n  T001:\n    deps: []\n    skillist: []\n" |> ignore

    let defaults =
        [ "interactive-lifecycle.md", "supported-host=true\nmode=interactive-window\nself-closed-for-evidence=false\nwindow-opened=true\nuser-close-observed=true\n"
          "evidence-launch-mode.md", "status=ok\nmode=persistent-evidence\nself-closed-for-evidence=true\nfirst-frame-presented=true\n"
          "container-session-diagnostics.md", "diagnostic-class=environment-session-ready\nruntime-directory=/tmp/runtime\ndisplay-variable=DISPLAY=:99\ndisplay-socket-exists=true\nsession-bus=present\n"
          "package-resolution.md", "exact-match=true\nrequested-version=0.1.16-persistent.1\nresolved-version=0.1.16-persistent.1\npackage-source=/tmp/feed\n"
          "generated-verify.md", "generated-tests-exist=true\ngenerated-tests-ran=true\nauthoritative=true\nfailure-class=none\n"
          "game-visual-evidence.md", "supported-host=true\nevidence-kind=screenshot\nboard-readable=true\ninput-or-progress-observed=true\n"
          "task-workflow-guidance.md", "batch=fixture graph-before=readiness/task-graph.md graph-after=readiness/task-graph.md skill-loading=recorded red-green-log=present\n"
          "evidence-audit.md", "status=pass readiness acceptance keywords present\n" ]

    let overrideMap = overridesByFile |> Map.ofList

    defaults
    |> List.iter (fun (fileName, content) ->
        writeReadiness root fileName (overrideMap |> Map.tryFind fileName |> Option.defaultValue content))

[<Tests>]
let persistentViewerEvidenceTests =
    testList "persistent viewer evidence contracts" [
        test "evidence command expectations separate persistent launch bounded helpers and synthetic propagation" {
            expectFileContains
                "specs/016-persistent-viewer-contract/readiness/evidence-graph.md"
                [ "persistent graphical launch artifacts"
                  "bounded smoke helpers"
                  "first-frame helpers"
                  "frame-count helpers"
                  "[S*]"
                  "Supported-host persistent launch evidence cannot be synthetic" ]

            expectFileContains
                "specs/016-persistent-viewer-contract/readiness/evidence-audit.md"
                [ "bounded helpers"
                  "scene metadata"
                  "unsupported-host"
                  "ambiguous persistent launch fields"
                  "mode=persistent-window"
                  "input-dispatch" ]
        }

        test "real audit rejection packages cover bounded unsupported and ambiguous persistent launch rejection" {
            expectFileContains
                "specs/016-persistent-viewer-contract/readiness/audit-rejections/bounded-only/audit.log"
                [ "readiness-contract: 0 blocking"
                  "missing supported-host persistent launch evidence"
                  "bounded-only substitution" ]

            expectFileContains
                "specs/016-persistent-viewer-contract/readiness/audit-rejections/unsupported-host-only/audit.log"
                [ "readiness-contract: 0 blocking"
                  "status: 1[X]"
                  "unsupported-host-only persistent launch evidence" ]

            expectFileContains
                "specs/016-persistent-viewer-contract/readiness/audit-rejections/missing-persistent-fields/audit.log"
                [ "readiness-contract: 0 blocking"
                  "missing persistent launch fields"
                  "missing=blocked-stage,classification,category,message" ]
        }

        test "EvidenceAudit rejects real bounded helper and unsupported host packages without supported persistent launch evidence" {
            let boundedOnly = fullPath "specs/016-persistent-viewer-contract/readiness/audit-rejections/bounded-only"
            let code, stdout, stderr = runEvidenceAudit boundedOnly
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects helper-only graphical readiness"
            Expect.stringContains output "persistent-launch: " "audit prints persistent launch scan summary"
            Expect.stringContains output "missing supported-host persistent launch evidence" "audit reports missing supported launch evidence"
            Expect.stringContains output "bounded-only substitution" "audit rejects bounded helper substitution"

            let unsupportedOnly = fullPath "specs/016-persistent-viewer-contract/readiness/audit-rejections/unsupported-host-only"
            let code, stdout, stderr = runEvidenceAudit unsupportedOnly
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unsupported-host-only graphical readiness"
            Expect.stringContains output "unsupported-host-only persistent launch evidence" "audit rejects unsupported-only launch evidence"

            let hitsPath = Path.Combine(boundedOnly, "readiness", "persistent-launch-hits.json")
            Expect.isTrue (File.Exists hitsPath) "audit writes persistent launch hit details"
        }

        test "EvidenceAudit rejects real generated launch output with missing required fields" {
            let missingFields = fullPath "specs/016-persistent-viewer-contract/readiness/audit-rejections/missing-persistent-fields"
            let code, stdout, stderr = runEvidenceAudit missingFields
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects ambiguous persistent launch evidence"
            Expect.stringContains output "missing persistent launch fields" "audit names missing required fields"
            Expect.stringContains output "blocked-stage,classification,category,message" "audit lists missing fields"
        }

        test "EvidenceAudit rejects persistent GUI runtime fixtures with missing readiness files" {
            let fixture = fullPath "specs/018-persistent-gui-runtime/readiness/audit-rejections/missing-readiness-files"
            let code, stdout, stderr = runEvidenceAudit fixture
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects missing persistent GUI runtime readiness files"
            Expect.stringContains output "persistent-gui-runtime:" "audit prints persistent GUI runtime scan summary"
            Expect.stringContains output "missing required readiness files" "audit reports missing required readiness files"
        }

        test "EvidenceAudit rejects bounded-only substitution for persistent GUI interactive evidence" {
            let fixture = fullPath "specs/018-persistent-gui-runtime/readiness/audit-rejections/bounded-only-interactive"
            let code, stdout, stderr = runEvidenceAudit fixture
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects bounded-only interactive evidence"
            Expect.stringContains output "bounded-only substitution for interactive evidence" "audit reports bounded-only substitution"
        }

        test "EvidenceAudit rejects text-only visual metadata on supported generated game hosts" {
            let fixture = fullPath "specs/018-persistent-gui-runtime/readiness/audit-rejections/text-only-visual-supported-host"
            let code, stdout, stderr = runEvidenceAudit fixture
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects text-only visual metadata on supported hosts"
            Expect.stringContains output "text-only visual metadata on supported host" "audit reports text-only visual metadata"
        }

        test "EvidenceAudit rejects unresolved package mismatch evidence" {
            let fixture = fullPath "specs/018-persistent-gui-runtime/readiness/audit-rejections/package-mismatch"
            let code, stdout, stderr = runEvidenceAudit fixture
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unresolved package mismatch"
            Expect.stringContains output "unresolved package mismatch" "audit reports package mismatch"
        }

        test "EvidenceAudit rejects generated tests that exist but did not run" {
            let fixture = fullPath "specs/018-persistent-gui-runtime/readiness/audit-rejections/missing-generated-test-execution"
            let code, stdout, stderr = runEvidenceAudit fixture
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects missing generated test execution"
            Expect.stringContains output "generated tests exist but did not run" "audit reports missing generated test execution"
        }

        test "EvidenceAudit rejects visual game evidence missing board and input proof" {
            use fixture = new TempFixtureDirectory("persistent-gui-runtime-visual-proof")
            writePersistentGuiRuntimeFixture
                fixture.Root
                [ "game-visual-evidence.md", "supported-host=true\nevidence-kind=screenshot\nboard-readable=false\ninput-or-progress-observed=false\n" ]

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects screenshot evidence without readable board or input/progress proof"
            Expect.stringContains output "visual game evidence missing board/input proof" "audit reports missing visual proof"
        }

        test "EvidenceAudit rejects non-authoritative generated verify evidence" {
            use fixture = new TempFixtureDirectory("persistent-gui-runtime-non-authoritative")
            writePersistentGuiRuntimeFixture
                fixture.Root
                [ "generated-verify.md", "generated-tests-exist=true\ngenerated-tests-ran=true\nauthoritative=false\nfailure-class=verification-depth\n" ]

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects non-authoritative generated verify evidence"
            Expect.stringContains output "generated verification is non-authoritative" "audit reports non-authoritative generated verify"
        }

        test "EvidenceAudit rejects missing persistent runtime readiness acceptance keywords" {
            use fixture = new TempFixtureDirectory("persistent-gui-runtime-missing-keywords")
            writePersistentGuiRuntimeFixture
                fixture.Root
                [ "package-resolution.md", "exact-match=true\n"
                  "generated-verify.md", "authoritative=true\n"
                  "game-visual-evidence.md", "supported-host=true\nevidence-kind=screenshot\nboard-readable=true\ninput-or-progress-observed=true\n" ]

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects readiness records missing acceptance keywords"
            Expect.stringContains output "missing readiness acceptance keywords" "audit reports missing package/generated/visual acceptance keywords"
        }

        test "bounded viewer docs label helper commands as non-readiness substitutes" {
            expectFileContains
                "docs/evidence.md"
                [ "CI and diagnostic helpers"
                  "do not replace supported-host persistent graphical launch evidence"
                  "contains only bounded or unsupported-host artifacts as incomplete" ]

            expectFileContains
                "docs/generated-apps.md"
                [ "not interactive readiness substitutes"
                  "Viewer.runApp viewerOptions generatedHost"
                  "only print metadata"
                  "diagnostic helpers only" ]

            expectFileContains
                "template/fragments/skiaviewer/README.md"
                [ "Viewer.runApp viewerOptions generatedHost"
                  "first-frame"
                  "frame-count"
                  "do not substitute"
                  "supported-host persistent graphical launch readiness" ]
        }
    ]
