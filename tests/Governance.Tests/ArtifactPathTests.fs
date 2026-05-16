module ArtifactPathTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let artifactPathTests =
    testList "Governance artifact paths" [
        test "surface baselines live at stable root readiness path" {
            [ "FS.Skia.UI.txt"; "FS.Skia.UI.Controls.txt"; "FS.Skia.UI.Layout.txt" ]
            |> List.iter (fun fileName ->
                let baselinePath = fullPath $"readiness/surface-baselines/{fileName}"
                Expect.isTrue (File.Exists baselinePath) $"{fileName} exists in stable baseline directory")

            expectFileContains "scripts/refresh-surface-baselines.fsx" [ "\"readiness\""; "\"surface-baselines\"" ]
            let refreshScript = read "scripts/refresh-surface-baselines.fsx"
            Expect.isFalse (refreshScript.Contains("specs\", \"002-skia-feature-parity", StringComparison.Ordinal)) "refresh script does not write historical feature baselines"
            if fileExists "tests/Package.Tests/SurfaceAreaTests.fs" then
                expectFileContains "tests/Package.Tests/SurfaceAreaTests.fs" [ "\"readiness\", \"surface-baselines\""; "surface baselines use stable root readiness path" ]
            else
                Expect.isTrue (fileExists "tests/Package.Tests/Package.Tests.fsproj") "minimal profile keeps package tests without optional surface baselines"
        }

        test "consumer package smoke is explicitly deferred from v1 targets" {
            expectFileContains "tests/Package.Tests/Tests.fs" [ "FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE"; "package consumer smoke is deferred outside v1 verification" ]
            expectFileContains "build.fsx" [ "PackageSmoke"; "FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE"; "Dev"; "Verify"; "Ci" ]
        }

        test "build graph documents and requires every v1 plus v2 artifact class" {
            expectFileContains
                "build.fsx"
                [ "logs"
                  "fsi"
                  "sample-smoke"
                  "task-graph.json"
                  "diff-scan-hits.json"
                  "pack-local.txt"
                  "package-surface-check.txt"
                  "v1 plus v2 verification artifact set" ]
        }
    ]
