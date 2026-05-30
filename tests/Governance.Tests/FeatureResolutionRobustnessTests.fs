module FeatureResolutionRobustnessTests

(* US1 (spec 037): the governance tooling must resolve the real active feature
   and report its real task count, hard-fail when no real feature resolves, and
   never silently fall back to a placeholder/stub. These tests exercise the
   real scripts (compute-task-graph.py, common.sh) and assert the build.fsx
   resolution source has the hardcoded fallback removed. *)

open System
open System.IO
open Expecto
open GovernanceTestSupport

let private featureJson root featureDirectory =
    writeFixtureFile
        root
        ".specify/feature.json"
        $"""{{
  "feature_directory": "{featureDirectory}"
}}
"""
    |> ignore

let private writeFeature root relativeDir taskLines depLines =
    writeFixtureFile
        root
        $"{relativeDir}/tasks.md"
        $"""# Tasks: Resolution Fixture

## Status Legend

- `[ ]` - pending

## Phase 1: Setup

{taskLines}

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
"""
    |> ignore

    writeFixtureFile
        root
        $"{relativeDir}/tasks.deps.yml"
        $"""schema_version: "1.0"
tasks:
{depLines}
"""
    |> ignore

let private runGraph featureDir =
    runProcess
        "python3"
        $".specify/extensions/evidence/scripts/python/compute-task-graph.py \"{featureDir}\""

[<Tests>]
let featureResolutionRobustnessTests =
    testList "US1 fail-loud feature resolution" [

        test "compute-task-graph echoes resolved feature id and real task count" {
            use fixture = new TempFixtureDirectory("resolve-real-count")
            featureJson fixture.Root "specs/050-real-feature"
            writeFeature
                fixture.Root
                "specs/050-real-feature"
                ("- [ ] T001 [skillist: []] Set up the workspace\n"
                 + "- [ ] T002 [skillist: []] Wire the second piece\n"
                 + "- [ ] T003 [skillist: []] Wire the third piece")
                ("  T001:\n    deps: []\n    skillist: []\n"
                 + "  T002:\n    deps: []\n    skillist: []\n"
                 + "  T003:\n    deps: []\n    skillist: []\n")

            let featureDir = Path.Combine(fixture.Root, "specs", "050-real-feature")
            let code, stdout, stderr = runGraph featureDir
            let output = stdout + stderr
            Expect.equal code 0 $"valid feature resolves: {output}"
            Expect.stringContains output "050-real-feature" "echoes the resolved feature id"
            Expect.stringContains output "real-task-count: 3" "echoes the real task count"
        }

        test "compute-task-graph hard-fails on an empty or unparseable task file" {
            use fixture = new TempFixtureDirectory("resolve-empty-tasks")
            featureJson fixture.Root "specs/051-empty-feature"
            // tasks.md exists but declares no tasks — must not be a passable stub.
            writeFeature fixture.Root "specs/051-empty-feature" "" ""

            let featureDir = Path.Combine(fixture.Root, "specs", "051-empty-feature")
            let code, stdout, stderr = runGraph featureDir
            let output = stdout + stderr
            Expect.notEqual code 0 $"empty task file is blocking, not a stub pass: {output}"
            Expect.stringContains output "empty or unparseable" "names the empty/unparseable task file failure"
        }

        test "compute-task-graph surfaces a recorded-vs-scanned feature mismatch" {
            use fixture = new TempFixtureDirectory("resolve-mismatch")
            // Recorded active feature differs from the directory actually scanned.
            featureJson fixture.Root "specs/060-recorded-feature"
            writeFeature
                fixture.Root
                "specs/070-scanned-feature"
                "- [ ] T001 [skillist: []] Set up the workspace"
                "  T001:\n    deps: []\n    skillist: []\n"

            let featureDir = Path.Combine(fixture.Root, "specs", "070-scanned-feature")
            let _code, stdout, stderr = runGraph featureDir
            let output = stdout + stderr
            Expect.stringContains output "mismatch" "surfaces the recorded-vs-scanned mismatch"
            Expect.stringContains output "060-recorded-feature" "names the recorded feature"
            Expect.stringContains output "070-scanned-feature" "names the scanned feature"
        }

        test "build.fsx resolution drops the hardcoded placeholder and hard-fails" {
            let build = read "build.fsx"
            Expect.isFalse
                (build.Contains("007-v2-template-packaging", StringComparison.Ordinal))
                "the hardcoded 007 placeholder fallback is removed"
            Expect.stringContains build "Cannot resolve the active feature" "names the resolution failure"
            Expect.stringContains build "feature_directory" "names the authoritative feature_directory source"
        }

        test "common.sh get_feature_paths terminal-fails when no real feature resolves" {
            use fixture = new TempFixtureDirectory("resolve-terminal-fail")
            Directory.CreateDirectory(Path.Combine(fixture.Root, ".specify")) |> ignore
            Directory.CreateDirectory(Path.Combine(fixture.Root, "specs")) |> ignore
            let commonSh = fullPath ".specify/scripts/bash/common.sh"

            let scriptPath =
                writeFixtureFile
                    fixture.Root
                    "harness.sh"
                    $"""#!/usr/bin/env bash
set -euo pipefail
cd "{fixture.Root}"
export SPECIFY_FEATURE=999-missing-feature
unset SPECIFY_FEATURE_DIRECTORY
source "{commonSh}"
get_feature_paths
"""

            let code, stdout, stderr = runProcess "bash" $"\"{scriptPath}\""
            let output = stdout + stderr
            Expect.notEqual code 0 $"unresolved feature is terminal-fail: {output}"
            Expect.stringContains output "No real active feature resolved" "emits the fail-loud resolution message"
        }

        test "common.sh get_feature_paths resolves a real recorded feature" {
            use fixture = new TempFixtureDirectory("resolve-terminal-pass")
            Directory.CreateDirectory(Path.Combine(fixture.Root, ".specify")) |> ignore
            Directory.CreateDirectory(Path.Combine(fixture.Root, "specs", "050-real-feature")) |> ignore
            featureJson fixture.Root "specs/050-real-feature"
            let commonSh = fullPath ".specify/scripts/bash/common.sh"

            let scriptPath =
                writeFixtureFile
                    fixture.Root
                    "harness.sh"
                    $"""#!/usr/bin/env bash
set -euo pipefail
cd "{fixture.Root}"
export SPECIFY_FEATURE=050-real-feature
unset SPECIFY_FEATURE_DIRECTORY
source "{commonSh}"
get_feature_paths
"""

            let code, stdout, stderr = runProcess "bash" $"\"{scriptPath}\""
            let output = stdout + stderr
            Expect.equal code 0 $"real recorded feature resolves: {output}"
            Expect.stringContains output "050-real-feature" "prints the resolved feature dir"
        }
    ]
