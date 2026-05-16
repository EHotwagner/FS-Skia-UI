module GeneratedProjectValidationTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let generatedProjectValidationTests =
    testList "Generated project validation contract" [
        test "TemplateSmoke scans placeholders excluded history minimal scope and generated Dev" {
            expectFileContains
                "build.fsx"
                [ "Placeholder scan: PASS"
                  "Excluded-history scan: PASS"
                  "Minimal optional exclusion scan: PASS"
                  "Generated AGENTS scan: PASS"
                  "Executable script scan: PASS"
                  "generated Dev"
                  "--allow-scripts yes"
                  "--skipGitInit true"
                  "non-visual V2 validation only"
                  "full visual evidence is deferred" ]
        }

        test "minimal required contents are enforced" {
            expectFileContains
                "build.fsx"
                [ "src/Lib/Lib.fsproj"
                  "tests/Lib.Tests/Lib.Tests.fsproj"
                  "tests/Package.Tests/Package.Tests.fsproj"
                  "tests/Governance.Tests/Governance.Tests.fsproj"
                  "samples/BasicViewer/BasicViewer.fsproj"
                  "AGENTS.md"
                  "Directory.Packages.props" ]
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
                  "Charts" ]

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

        test "V3 generated file-list report captures selected capabilities and exclusions" {
            if directoryExists "specs/009-v3-modular-framework" then
                expectGeneratedProductFileList
                    "app-source"
                    [ "src/Product/Product.fsproj"
                      "tests/Product.Tests/Product.Tests.fsproj"
                      ".agents/skills/fs-skia-project/SKILL.md"
                      ".agents/skills/fs-skia-scene/SKILL.md"
                      ".agents/skills/fs-skia-skiaviewer/SKILL.md"
                      ".agents/skills/fs-skia-elmish/SKILL.md"
                      ".agents/skills/fs-skia-keyboard-input/SKILL.md"
                      ".agents/skills/fs-skia-layout/SKILL.md"
                      ".agents/skills/fs-skia-charts/SKILL.md"
                      "PackageReference Include=\"FS.Skia.UI.Scene\""
                      "PackageReference Include=\"FS.Skia.UI.SkiaViewer\""
                      "PackageReference Include=\"FS.Skia.UI.Elmish\""
                      "PackageReference Include=\"FS.Skia.UI.KeyboardInput\""
                      "PackageReference Include=\"FS.Skia.UI.Layout\""
                      "PackageReference Include=\"FS.Skia.UI.Charts\"" ]
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

                let verifyLog = "specs/009-v3-modular-framework/readiness/generated-product-verify/app-source/verify.log"
                Expect.isTrue (fileExists verifyLog) "app-source Verify log exists"
                expectFileContains verifyLog [ "Verify completed for generated product" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not run source-only V3 generated-product checks"
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
                      "PackageReference Include=\"FS.Skia.UI.Charts\""
                      ".agents/skills/fs-skia-charts/SKILL.md"
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
                      "PackageReference Include=\"FS.Skia.UI.Charts\"" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not run source-only V3 matrix checks"
        }
    ]
