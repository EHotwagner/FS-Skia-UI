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

let private writeWindowVisibilityFixture root overridesByFile omittedFiles =
    writeFixtureFile root "spec.md" "# Fix Window Visibility\n\nRequires interactive-visible-window.md close-reason-separation.md real-image-evidence.md generated-validation.md\n" |> ignore
    writeFixtureFile root "plan.md" "# Fix Window Visibility Plan\n" |> ignore
    writeFixtureFile root "tasks.md" "# Tasks\n\n- [X] T001 [skillist: []] Window visibility audit fixture\n" |> ignore
    writeFixtureFile root "tasks.deps.yml" "schema_version: \"1.0\"\n\ntasks:\n  T001:\n    deps: []\n    skillist: []\n" |> ignore

    let defaults =
        [ "governance-risk-levels.md", "small medium broad required evidence broad validation\n"
          "aggregate-hang-diagnostics.md", "verdict stage elapsed duration last observed command focused rerun non-authoritative aggregate\n"
          "runtime-limitations.md", ".NET 10 desktop Vulkan SkiaSharp preview unsupported macOS/mobile/browser no software-renderer fallback\n"
          "interactive-visible-window.md", "status=ok\nmode=interactive-window\nwindow-visible=observed:true\naccessible-window=true\nfirst-frame-presented=true\nself-closed-for-evidence=false\nprocess-running=true\ntaskbar-entry=true\n"
          "close-reason-separation.md", "close-reason=user-close\nuser-close-observed=true\nevidence-close-observed=false\n"
          "window-state-diagnostics.md", "status=degraded\ndiagnostic-class=environment-session\nnative-handle=unsupported\nvisible=unsupported\nfocusable=unsupported\nfocused=unsupported\nminimized=unsupported\nmaximized=unsupported\nclient-size=unavailable\nrenderable-surface=unsupported\ninput-devices=unsupported\nstatus=failed\ndiagnostic-class=window-visibility\nnative-handle=observed:true\nvisible=observed:false\nfocusable=observed:false\nfocused=unsupported\nminimized=observed:false\nmaximized=observed:false\nclient-size=640x480\nrenderable-surface=observed:true\ninput-devices=observed:false\nstatus=failed\ndiagnostic-class=app-lifecycle\nnative-handle=observed:true\nvisible=observed:true\nfocusable=observed:true\nfocused=observed:true\nminimized=observed:false\nmaximized=observed:false\nclient-size=640x480\nrenderable-surface=observed:true\ninput-devices=observed:true\nstatus=failed\ndiagnostic-class=product-defect\nnative-handle=observed:true\nvisible=observed:true\nfocusable=observed:true\nfocused=unsupported\nminimized=observed:false\nmaximized=observed:false\nclient-size=0x0\nrenderable-surface=observed:false\ninput-devices=unavailable\n"
          "window-options.md", "status=honored diagnostic-class=window-options option=resize requested=resizable observed=resizable\nstatus=honored diagnostic-class=window-options option=maximize requested=maximizable observed=maximizable\nstatus=honored diagnostic-class=window-options option=startup-state requested=normal observed=normal\nstatus=honored diagnostic-class=window-options option=startup-position requested=centered observed=centered\nstatus=honored diagnostic-class=window-options option=backend requested=default observed=default\n"
          "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=screenshot\nartifact-kind=image\nartifact-decodable=true\nimage-artifact=readiness/artifacts/window.png\n"
          "generated-validation.md", "exact-package-match=true\ngenerated-tests-exist=true\ngenerated-tests-ran=true\nauthoritative=true\nfailure-class=none\n"
          "evidence-audit.md", "verdict=fixture\n" ]

    let overrideMap = overridesByFile |> Map.ofList
    let omitted = omittedFiles |> Set.ofList

    defaults
    |> List.iter (fun (fileName, content) ->
        if not (omitted.Contains fileName) then
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

        test "EvidenceAudit rejects missing window visibility readiness files" {
            use fixture = new TempFixtureDirectory("window-visibility-missing-readiness")
            writeWindowVisibilityFixture fixture.Root [] [ "interactive-visible-window.md"; "real-image-evidence.md" ]

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects missing 019 readiness files"
            Expect.stringContains output "window-visibility:" "audit prints window visibility scan summary"
            Expect.stringContains output "missing required window visibility readiness file" "audit reports missing required files"
        }

        test "EvidenceAudit rejects process taskbar-only visible-window substitution" {
            use fixture = new TempFixtureDirectory("window-visibility-taskbar-only")
            writeWindowVisibilityFixture
                fixture.Root
                [ "interactive-visible-window.md", "status=ok\nmode=interactive-window\nwindow-visible=observed:false\naccessible-window=false\nfirst-frame-presented=true\nself-closed-for-evidence=false\nprocess-running=true\ntaskbar-entry=true\nclassification=process/taskbar-only\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects taskbar-only substitution"
            Expect.stringContains output "process/taskbar-only visible-window substitution" "audit reports taskbar-only visible-window substitution"
        }

        test "EvidenceAudit rejects missing window diagnostic classes" {
            use fixture = new TempFixtureDirectory("window-visibility-missing-diagnostic-classes")
            writeWindowVisibilityFixture
                fixture.Root
                [ "window-state-diagnostics.md", "status=failed\ndiagnostic-class=window-visibility\nnative-handle=observed:true\nvisible=observed:false\nfocusable=observed:false\nrenderable-surface=observed:true\ninput-devices=observed:false\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects missing diagnostic classes"
            Expect.stringContains output "missing diagnostic classes" "audit reports missing diagnostic classes"
            Expect.stringContains output "environment-session" "audit names missing environment/session class"
        }

        test "EvidenceAudit rejects unsupported-host-only window diagnostics as visible readiness" {
            use fixture = new TempFixtureDirectory("window-visibility-unsupported-only")
            writeWindowVisibilityFixture
                fixture.Root
                [ "interactive-visible-window.md", "status=ok\nmode=interactive-window\nwindow-visible=unsupported\naccessible-window=false\nfirst-frame-presented=true\nself-closed-for-evidence=false\nunsupported-host-only=true\n"
                  "window-state-diagnostics.md", "status=unsupported\ndiagnostic-class=environment-session\nnative-handle=unsupported\nvisible=unsupported\nfocusable=unsupported\nrenderable-surface=unsupported\ninput-devices=unsupported\nunsupported-host-only=true\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unsupported-host-only visible readiness"
            Expect.stringContains output "unsupported-host-only visible-window claim" "audit reports unsupported-host-only claim"
        }

        test "EvidenceAudit rejects taskbar-only success in window diagnostics" {
            use fixture = new TempFixtureDirectory("window-visibility-diagnostic-taskbar-success")
            writeWindowVisibilityFixture
                fixture.Root
                [ "window-state-diagnostics.md", "status=ok\ndiagnostic-class=window-visibility\ntaskbar-entry=true\nnative-handle=observed:true\nvisible=observed:false\nfocusable=observed:false\nrenderable-surface=observed:true\ninput-devices=observed:false\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects taskbar-only success diagnostics"
            Expect.stringContains output "process/taskbar-only success claim" "audit reports taskbar-only diagnostic success"
        }

        test "EvidenceAudit rejects evidence close reported as user close" {
            use fixture = new TempFixtureDirectory("window-visibility-evidence-close")
            writeWindowVisibilityFixture
                fixture.Root
                [ "close-reason-separation.md", "close-reason=evidence-close\nuser-close-observed=true\nevidence-close-observed=true\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects evidence close as user close"
            Expect.stringContains output "evidence close reported as user close" "audit reports close reason conflation"
        }

        test "EvidenceAudit rejects metadata-only screenshot claims" {
            use fixture = new TempFixtureDirectory("window-visibility-metadata-screenshot")
            writeWindowVisibilityFixture
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=screenshot\nartifact-kind=metadata\nartifact-decodable=false\nmetadata-only screenshot claim\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects metadata-only screenshot evidence"
            Expect.stringContains output "metadata-only screenshot claim" "audit reports metadata-only screenshot claim"
        }

        test "EvidenceAudit rejects unsupported screenshot hosts that claim proof" {
            use fixture = new TempFixtureDirectory("window-visibility-unsupported-screenshot-proof")
            writeWindowVisibilityFixture
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nstatus=unsupported\nevidence-kind=screenshot\nunsupported-host-reason=DISPLAY is missing\nfallback=deterministic-scene-evidence\nscreenshot-path=none\nartifact-kind=image\nartifact-decodable=true\nproves-scene-rendering=false\nproves-desktop-visibility=true\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unsupported screenshot proof claims"
            Expect.stringContains output "unsupported screenshot cannot prove desktop visibility" "audit reports unsupported screenshot proof claim"
        }

        test "EvidenceAudit rejects unsupported screenshot hosts without explicit reason" {
            use fixture = new TempFixtureDirectory("window-visibility-unsupported-screenshot-reason")
            writeWindowVisibilityFixture
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nstatus=unsupported\nevidence-kind=screenshot\nfallback=deterministic-scene-evidence\nscreenshot-path=none\nproves-scene-rendering=false\nproves-desktop-visibility=false\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unsupported screenshot records without unsupported-host reason"
            Expect.stringContains output "unsupported screenshot missing unsupported-host reason" "audit reports missing unsupported-host reason"
        }

        test "EvidenceAudit rejects visual evidence missing proof fields" {
            use fixture = new TempFixtureDirectory("window-visibility-image-missing-proof-fields")
            writeWindowVisibilityFixture
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=true\nevidence-kind=image\npath=readiness/artifacts/window.png\nimage-decodable=true\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects image evidence without proof fields"
            Expect.stringContains output "missing visual evidence proof fields" "audit reports missing proof fields"
        }

        test "EvidenceAudit rejects pixel readback desktop visibility claims" {
            use fixture = new TempFixtureDirectory("window-visibility-pixel-desktop-claim")
            writeWindowVisibilityFixture
                fixture.Root
                [ "real-image-evidence.md", "requested-image-evidence=false\nevidence-kind=pixel-readback\npath=readiness/artifacts/readback.txt\nfallback-reason=screenshot-unavailable\nproves-scene-rendering=true\nproves-desktop-visibility=true\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects scene-only pixel readback as desktop visibility proof"
            Expect.stringContains output "pixel-readback cannot prove desktop visibility" "audit reports pixel-readback desktop visibility claim"
        }

        test "EvidenceAudit rejects unresolved generated validation package mismatch" {
            use fixture = new TempFixtureDirectory("window-visibility-package-mismatch")
            writeWindowVisibilityFixture
                fixture.Root
                [ "generated-validation.md", "exact-package-match=false\ngenerated-tests-exist=true\ngenerated-tests-ran=true\nauthoritative=true\nfailure-class=package/verification\nwarning=NU1603\npackage mismatch\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unresolved generated validation package mismatch"
            Expect.stringContains output "unresolved package mismatch" "audit reports package mismatch"
        }

        test "EvidenceAudit rejects missing generated validation test execution" {
            use fixture = new TempFixtureDirectory("window-visibility-missing-generated-tests")
            writeWindowVisibilityFixture
                fixture.Root
                [ "generated-validation.md", "exact-package-match=true\ngenerated-tests-exist=true\ngenerated-tests-ran=false\nauthoritative=true\nfailure-class=generated-validation\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects missing generated test execution"
            Expect.stringContains output "missing generated test execution" "audit reports missing generated tests"
        }

        test "EvidenceAudit rejects missing window option rows" {
            use fixture = new TempFixtureDirectory("window-visibility-missing-window-options")
            writeWindowVisibilityFixture
                fixture.Root
                [ "window-options.md", "status=honored diagnostic-class=window-options option=resize requested=resizable observed=resizable\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects incomplete window option readiness"
            Expect.stringContains output "missing option rows" "audit reports missing option rows"
            Expect.stringContains output "backend" "audit names missing backend option"
        }

        test "EvidenceAudit rejects silently ignored unsupported window options" {
            use fixture = new TempFixtureDirectory("window-visibility-ignored-unsupported-options")
            writeWindowVisibilityFixture
                fixture.Root
                [ "window-options.md", "option=resize requested=resizable observed=resizable\noption=maximize requested=maximizable observed=maximizable\noption=startup-state requested=normal observed=normal\noption=startup-position requested=centered observed=centered\noption=backend requested=opengl observed=default\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects unsupported options without explicit diagnostics"
            Expect.stringContains output "silently ignored unsupported window option" "audit reports ignored unsupported option"
        }

        test "EvidenceAudit rejects window option failures hidden as app lifecycle" {
            use fixture = new TempFixtureDirectory("window-visibility-option-failure-hidden")
            writeWindowVisibilityFixture
                fixture.Root
                [ "window-options.md", "status=failed diagnostic-class=app-lifecycle option=resize requested=fixed-size observed=none\noption=maximize requested=maximizable observed=maximizable\noption=startup-state requested=normal observed=normal\noption=startup-position requested=centered observed=centered\noption=backend requested=default observed=default\n" ]
                []

            let code, stdout, stderr = runEvidenceAudit fixture.Root
            let output = stdout + stderr

            Expect.equal code 2 "audit rejects option failures hidden under app lifecycle"
            Expect.stringContains output "window-options failure hidden under app-lifecycle" "audit reports hidden window-options failure"
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
                  "Viewer.runApp viewerOptions Product.Program.generatedHost"
                  "only print metadata"
                  "diagnostic helpers only" ]

            expectFileContains
                "template/fragments/skiaviewer/README.md"
                [ "Viewer.runApp viewerOptions Product.Program.generatedHost"
                  "first-frame"
                  "frame-count"
                  "do not substitute"
                  "supported-host persistent graphical launch readiness" ]
        }
    ]
