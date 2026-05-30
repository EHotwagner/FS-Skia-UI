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
            [ "docs/reports/build.md"; "docs/reports/testing.md"; "docs/reports/evidence.md" ]
            |> List.iter (fun doc ->
                Expect.isTrue (File.Exists(fullPath doc)) $"{doc} exists"
                let content = read doc
                canonicalTargets
                |> List.iter (fun target -> Expect.stringContains content target $"{doc} names {target}"))

            expectFileContains
                "docs/reports/evidence.md"
                [ "readiness/surface-baselines/*.txt"
                  "Active feature `readiness/logs/*.txt`"
                  "Active feature `readiness/fsi/*.txt`"
                  "Active feature `readiness/sample-smoke/*.txt`"
                  "Active feature `readiness/template/**`" ]
        }

        test "docs name deferred roadmap categories outside v1 verification" {
            let docs = read "docs/reports/build.md" + read "docs/reports/testing.md" + read "docs/reports/evidence.md"

            deferredItems
            |> List.iter (fun item -> Expect.stringContains (docs.ToLowerInvariant()) item $"{item} is deferred in docs")
        }

        test "README and workflow delegate to canonical command surface" {
            expectFileContains "README.md" [ "./fake.sh build -t Dev"; "./fake.sh build -t Verify"; "docs/reports/build.md" ]
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

        test "Controls compatibility guidance documents replacement path and non-promises" {
            let compatibility = read "specs/011-controls-boundary-refactor/readiness/compatibility-impact.md"
            let controls = read "docs/reports/controls.md"
            let generatedProduct = read "template/base/docs/product.md"
            let combined = compatibility + Environment.NewLine + controls + Environment.NewLine + generatedProduct

            [ "LineChart"
              "BarChart"
              "PieChart"
              "ScatterPlot"
              "GraphView"
              "DataGrid"
              "FS.Skia.UI.Controls"
              "Scene"
              "Layout"
              "KeyboardInput"
              "SkiaViewer"
              "Elmish" ]
            |> List.iter (fun required ->
                Expect.stringContains combined required $"compatibility guidance preserves {required}")

            [ "compatibility shim"
              "automated external-app migration"
              "release publishing automation" ]
            |> List.iter (fun nonPromise ->
                Expect.stringContains (compatibility.ToLowerInvariant()) nonPromise $"{nonPromise} is explicitly not promised")
        }

        test "Controls boundary readiness evidence covers governance package smoke dependency compatibility and lower-level paths" {
            [ "specs/011-controls-boundary-refactor/readiness/public-surface.md",
              [ "PackageSurfaceCheck"
                "FS.Skia.UI.Controls.Elmish"
                "t067-package-surface.txt" ]
              "specs/011-controls-boundary-refactor/readiness/package-boundary.md",
              [ "active package boundary evidence"
                "FS.Skia.UI.Controls.Elmish"
                "FS.Skia.UI.Charts.txt" ]
              "specs/011-controls-boundary-refactor/readiness/generated-guidance.md",
              [ "GeneratedGuidanceCheck"
                "Controls.Elmish"
                "removed Charts" ]
              "specs/011-controls-boundary-refactor/readiness/dependency-report.md",
              [ "Controls.Elmish owns Fable.Elmish"
                "KeyboardInput owns YamlDotNet"
                "removed Charts package" ]
              "specs/011-controls-boundary-refactor/readiness/compatibility-impact.md",
              [ "Controls-versus-lower-level-path"
                "Scene"
                "Layout"
                "KeyboardInput"
                "SkiaViewer"
                "Elmish"
                "no compatibility shim" ] ]
            |> List.iter (fun (relativePath, required) ->
                expectFileContains relativePath required)
        }
    ]
