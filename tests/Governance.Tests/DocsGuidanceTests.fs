module DocsGuidanceTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

let canonicalTargets =
    [ "Dev"
      "Verify"
      "Ci"
      "PackLocal"
      "RefreshSurfaceBaselines"
      "PackageSurfaceCheck"
      "FsiTranscripts"
      "SampleContractSmoke"
      "TemplateCheck"
      "DependencyReport"
      "GeneratedGuidanceCheck"
      "TemplateDrift"
      "EvidenceGraph"
      "EvidenceAudit" ]

let deferredItems =
    [ "visual evidence"
      "package consumer smoke"
      "release validation"
      "external template repository"
      "distribution automation" ]

[<Tests>]
let docsGuidanceTests =
    testList "Docs and generated guidance" [
        test "build testing and evidence docs cover canonical targets and artifact paths" {
            [ "docs/build.md"; "docs/testing.md"; "docs/evidence.md" ]
            |> List.iter (fun doc ->
                Expect.isTrue (File.Exists(fullPath doc)) $"{doc} exists"
                let content = read doc
                canonicalTargets
                |> List.iter (fun target -> Expect.stringContains content target $"{doc} names {target}"))

            expectFileContains
                "docs/evidence.md"
                [ "readiness/surface-baselines/*.txt"
                  "specs/007-v2-template-packaging/readiness/logs/*.txt"
                  "specs/007-v2-template-packaging/readiness/fsi/*.txt"
                  "specs/007-v2-template-packaging/readiness/sample-smoke/*.txt"
                  "specs/007-v2-template-packaging/readiness/template/**" ]
        }

        test "docs name deferred roadmap categories outside v1 verification" {
            let docs = read "docs/build.md" + read "docs/testing.md" + read "docs/evidence.md"

            deferredItems
            |> List.iter (fun item -> Expect.stringContains (docs.ToLowerInvariant()) item $"{item} is deferred in docs")
        }

        test "README and workflow delegate to canonical command surface" {
            expectFileContains "README.md" [ "./fake.sh build -t Dev"; "./fake.sh build -t Verify"; "docs/build.md" ]
            expectFileContains ".specify/workflows/speckit/workflow.yml" [ "./fake.sh build -t Ci"; "canonical" ]
        }

        test "generated task guidance names canonical targets and preserves evidence graph requirements" {
            expectFileContains
                ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
                [ "./fake.sh build -t Dev"
                  "./fake.sh build -t Verify"
                  "PackLocal"
                  "RefreshSurfaceBaselines"
                  "PackageSurfaceCheck"
                  "TemplateCheck"
                  "DependencyReport"
                  "GeneratedGuidanceCheck"
                  "TemplateDrift"
                  "EvidenceGraph"
                  "EvidenceAudit"
                  "tasks.deps.yml"
                  "speckit.evidence.graph" ]
        }

        test "task-skill review rationale is captured" {
            expectFileContains
                ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
                [ "tasks-template.md"
                  "speckit.evidence.graph" ]
        }
    ]
