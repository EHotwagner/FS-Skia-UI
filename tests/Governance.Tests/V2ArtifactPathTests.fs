module V2ArtifactPathTests

open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let v2ArtifactPathTests =
    testList "V2 artifact paths" [
        test "build graph names all required V2 readiness artifact classes" {
            expectFileContains
                "build.fsx"
                [ "template-pack.log"
                  "template-package-contents.md"
                  "source-install.log"
                  "package-install.log"
                  "instantiation.log"
                  "generated-project-scans.md"
                  "dev.log"
                  "verdict.md"
                  "dependencies.md"
                  "generated-guidance.md"
                  "template-drift.md"
                  "artifacts"
                  "templates" ]
        }

        test "feature readiness inventories document setup obligations" {
            let featureReadiness = "specs/007-v2-template-packaging/readiness"

            if directoryExists featureReadiness then
                [ "template-source-inventory.md"
                  "dependency-inventory.md"
                  "evidence-obligations.md"
                  "traceability-matrix.md" ]
                |> List.iter (fun file ->
                    let path = $"{featureReadiness}/{file}"
                    Expect.isTrue (File.Exists(fullPath path)) $"{path} exists")
            else
                Expect.isFalse (directoryExists "specs/007-v2-template-packaging") "generated projects exclude source-only V2 readiness"
        }
    ]
