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
