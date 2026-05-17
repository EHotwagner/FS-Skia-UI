module GeneratedProjectValidationTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let generatedProjectValidationTests =
    testList "Generated project validation contract" [
        test "TemplateSmoke scans placeholders excluded history V3 profile references and generated Dev" {
            expectFileContains
                "build.fsx"
                [ "Placeholder scan: PASS"
                  "Excluded-history scan: PASS"
                  "V3 framework-source exclusion scan: PASS"
                  "V3 selected package reference scan: PASS"
                  "Spec Kit install scan: PASS"
                  "Generated AGENTS scan: PASS"
                  "Executable script scan: PASS"
                  "generated Dev"
                  "--allow-scripts yes"
                  "--skipGitInit true"
                  "non-visual V3 validation only"
                  "full visual evidence is deferred" ]
        }

        test "V3 dotnet template required contents are enforced" {
            expectFileContains
                "build.fsx"
                [ "src/{row.ProjectName}/{row.ProjectName}.fsproj"
                  "tests/{row.ProjectName}.Tests/{row.ProjectName}.Tests.fsproj"
                  "docs/product.md"
                  "AGENTS.md"
                  ".specify/memory/constitution.md"
                  ".specify/templates/spec-template.md"
                  ".agents/skills/speckit-specify/SKILL.md"
                  "Directory.Packages.props"
                  "expectedPackages"
                  "forbiddenFrameworkPaths" ]
        }

        test "V3 default generated product content is enforced" {
            expectFileContains
                "build.fsx"
                [ "exactly one product app"
                  "exactly one product test suite"
                  "framework implementation projects"
                  "framework README content"
                  "selected capability skills"
                  "consumer-mode package references"
                  "Scene"
                  "SkiaViewer"
                  "Elmish"
                  "KeyboardInput"
                  "Layout"
                  "Controls" ]

            expectPathsExist
                "V3 template base"
                [ "template/base/src/Product/Product.fsproj"
                  "template/base/tests/Product.Tests/Product.Tests.fsproj"
                  "template/base/README.md"
                  "template/base/docs/product.md"
                  "template/base/build.fsx"
                  "template/base/fake.sh"
                  "template/base/fake.cmd" ]
        }

        test "V3 generated product checks enforce Controls ownership and stale reference diagnostics" {
            expectFileContains
                "build.fsx"
                [ "FS.Skia.UI.Controls.Elmish"
                  "Controls-owned form/chart/graph/DataGrid authoring"
                  "Controls.Elmish adapter references"
                  "stale Charts exclusions"
                  "removed Charts package reference"
                  "framework sample content"
                  "historical specs"
                  "framework readiness evidence"
                  "controls-boundary-guidance" ]

            expectFileContains
                "scripts/template-drift.fsx"
                [ "Controls Boundary Guidance"
                  "controls-boundary-guidance"
                  "ControlsElmish.program"
                  "no compatibility shim" ]
        }

        test "V3 generated file-list report captures selected capabilities and exclusions" {
            if directoryExists "specs/009-v3-modular-framework" then
                expectGeneratedProductFileList
                    "app-source"
                    [ "src/V3AppSource/V3AppSource.fsproj"
                      "tests/V3AppSource.Tests/V3AppSource.Tests.fsproj"
                      ".agents/skills/fs-skia-project/SKILL.md"
                      ".agents/skills/fs-skia-scene/SKILL.md"
                      ".agents/skills/fs-skia-skiaviewer/SKILL.md"
                      ".agents/skills/fs-skia-elmish/SKILL.md"
                      ".agents/skills/fs-skia-keyboard-input/SKILL.md"
                      ".agents/skills/fs-skia-ui-widgets/SKILL.md"
                      ".agents/skills/speckit-specify/SKILL.md"
                      ".agents/skills/speckit-plan/SKILL.md"
                      ".agents/skills/speckit-tasks/SKILL.md"
                      ".agents/skills/speckit-implement/SKILL.md"
                      ".specify/memory/constitution.md"
                      ".specify/templates/spec-template.md"
                      ".specify/scripts/bash/setup-plan.sh"
                      ".specify/workflows/speckit/workflow.yml"
                      "PackageReference Include=\"FS.Skia.UI.Scene\""
                      "PackageReference Include=\"FS.Skia.UI.SkiaViewer\""
                      "PackageReference Include=\"FS.Skia.UI.Elmish\""
                      "PackageReference Include=\"FS.Skia.UI.KeyboardInput\""
                      "PackageReference Include=\"FS.Skia.UI.Layout\""
                      "PackageReference Include=\"FS.Skia.UI.Controls\""
                      "PackageReference Include=\"FS.Skia.UI.Controls.Elmish\""
                      "LineChart.create"
                      "DataGrid.create"
                      "ControlsElmish.program" ]
                    [ "samples/"
                      "tests/Parity.Tests"
                      "specs/00"
                      "readiness/"
                      "src/Lib/Lib.fsproj"
                      ".template.package" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not carry source-only V3 generated product reports"
        }

        test "V3 generated product governance runs product checks and excludes framework maintenance" {
            if directoryExists "specs/009-v3-modular-framework" then
                expectFileContains
                    "template/base/build.fsx"
                    [ "Dev"
                      "Test"
                      "Verify"
                      "GeneratedGuidanceCheck"
                      "TemplateDrift"
                      "EvidenceGraph"
                      "EvidenceAudit"
                      "generated product" ]

                let build = read "template/base/build.fsx"

                [ "TemplatePack"
                  "PackageSurfaceCheck"
                  "SampleContractSmoke"
                  "ParityGallery"
                  "framework-source maintenance" ]
                |> List.iter (fun forbidden -> Expect.isFalse (build.Contains forbidden) $"generated product build excludes {forbidden}")

                let verifyLog =
                    if fileExists "specs/010-skia-controls-library/readiness/generated-product-verify/app-source/verify.log" then
                        "specs/010-skia-controls-library/readiness/generated-product-verify/app-source/verify.log"
                    else
                        "specs/009-v3-modular-framework/readiness/generated-product-verify/app-source/verify.log"

                Expect.isTrue (fileExists verifyLog) "app-source Verify log exists"
                expectFileContains verifyLog [ "Verify completed for generated product" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not run source-only V3 generated-product checks"
        }

        test "V3 source and package generated validation roots prove Controls usage and framework-source exclusions" {
            if fileExists "specs/011-controls-boundary-refactor/readiness/generated-file-lists/app-source.txt" then
                [ "app-source"; "app-package" ]
                |> List.iter (fun profile ->
                    expectGeneratedProductFileList
                        profile
                        [ "PackageReference Include=\"FS.Skia.UI.Controls\""
                          "PackageReference Include=\"FS.Skia.UI.Controls.Elmish\""
                          "PackageReference Include=\"FS.Skia.UI.KeyboardInput\""
                          "RichText.create"
                          "LineChart.create"
                          "GraphView.create"
                          "DataGrid.create"
                          "ControlsElmish.program" ]
                        [ "PackageReference Include=\"FS.Skia.UI.Charts\""
                          "src/Charts"
                          "tests/Charts.Tests"
                          "samples/"
                          "specs/00"
                          "readiness/"
                          "src/Lib/Lib.fsproj"
                          ".template.package"
                          "docs/architecture.md" ])
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not carry active feature validation roots"
        }

        test "V3 generated product matrix reflects selected capability sets" {
            if directoryExists "specs/009-v3-modular-framework" then
                expectGeneratedProductFileList
                    "headless-scene-source"
                    [ "PackageReference Include=\"FS.Skia.UI.Scene\"" ]
                    [ "PackageReference Include=\"FS.Skia.UI.SkiaViewer\""
                      "PackageReference Include=\"FS.Skia.UI.Elmish\""
                      "PackageReference Include=\"FS.Skia.UI.KeyboardInput\""
                      "PackageReference Include=\"FS.Skia.UI.Layout\""
                      "PackageReference Include=\"FS.Skia.UI.Controls\""
                      ".agents/skills/fs-skia-ui-widgets/SKILL.md"
                      "samples/" ]

                expectGeneratedProductFileList
                    "sample-pack-source"
                    [ "PackageReference Include=\"FS.Skia.UI.Scene\""
                      "PackageReference Include=\"FS.Skia.UI.SkiaViewer\""
                      "PackageReference Include=\"FS.Skia.UI.Elmish\""
                      ".agents/skills/fs-skia-samples/SKILL.md"
                      "samples/README.md" ]
                    [ "PackageReference Include=\"FS.Skia.UI.KeyboardInput\""
                      "PackageReference Include=\"FS.Skia.UI.Layout\""
                      "PackageReference Include=\"FS.Skia.UI.Controls\"" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not run source-only V3 matrix checks"
        }
    ]
