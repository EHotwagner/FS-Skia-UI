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

        test "generated graphical template source defaults to persistent viewer host" {
            expectFileContains
                "template/base/src/Product/EvidenceCommands.fs"
                [ "let viewerOptions"
                  "let generatedHost"
                  "MapKey = mapKey"
                  "Tick = tick"
                  "mode=persistent-evidence" ]

            expectFileContains
                "template/base/src/Product/Program.fs"
                [ "Viewer.runApp viewerOptions generatedHost"
                  "--launch-evidence"
                  "mode=interactive-window"
                  "--bounded-smoke"
                  "--bounded-smoke-frame-diagnostics"
                  "--scene-evidence" ]
        }

        test "GeneratedProductCheck enforces persistent host wiring and rejects bounded-only default markers" {
            expectFileContains
                "build.fsx"
                [ "requiredPersistentHostTerms"
                  "Viewer.runApp viewerOptions generatedHost"
                  "window-visible=observed:true"
                  "accessible-window=true"
                  "generated app is missing persistent viewer host wiring"
                  "forbiddenDefaultSubstitutions"
                  "print metadata"
                  "count controls"
                  "run bounded smoke"
                  "emit scene evidence"
                  "first-frame-only=true"
                  "exit after first frame"
                  "without a persistent launch attempt"
                  "bounded-only or print-only default path markers"
                  "/readiness/logs/" ]
        }

        test "generated app default diagnostics report command capability blocked stage category and message" {
            expectFileContains
                "template/base/src/Product/Program.fs"
                [ "let defaultCommand"
                  "Viewer.runtimeCapability()"
                  "missing-package-capability"
                  "unsupported-host-reasons"
                  "command=%s"
                  "blocked-stage=%A"
                  "classification=%A"
                  "category=%A"
                  "message=%s" ]
        }

        test "GeneratedProductCheck records persistent launch diagnostics separately from bounded evidence" {
            expectFileContains
                "build.fsx"
                [ "persistent-launch-diagnostics.log"
                  "generated consumer persistent launch diagnostics"
                  "persistent launch diagnostics captured separately from bounded evidence"
                  "Persistent launch diagnostics log"
                  "PersistentLaunchDiagnosticFailure" ]
        }

        test "GeneratedProductCheck records full window visibility validation contract output" {
            expectFileContains
                "build.fsx"
                [ "generated consumer window diagnostics"
                  "generated consumer window options"
                  "generated consumer image evidence"
                  "WindowDiagnosticsFailure"
                  "WindowOptionsFailure"
                  "VisualEvidenceFailure"
                  "default-interactive-launch"
                  "bounded-evidence-validation"
                  "close-reason-validation"
                  "window-diagnostics-validation"
                  "window-options-validation"
                  "image-evidence-validation"
                  "authoritative"
                  "failure-class" ]
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
                    [ "src/Product/Product.fsproj"
                      "tests/Product.Tests/Product.Tests.fsproj"
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

        test "generated evidence targets reject placeholder-only completion logs and delegate to authoritative validation" {
            let build = read "template/base/build.fsx"
            let evidenceCommands = read "template/base/src/Product/EvidenceCommands.fs"

            Expect.isFalse (build.Contains("| \"EvidenceGraph\"\n    | \"EvidenceAudit\" -> writeLog target")) "generated evidence targets must not share the completion-only placeholder logger"

            [ "runGeneratedEvidenceGraph"
              "runGeneratedEvidenceAudit"
              "delegated-authoritative"
              "validation-area"
              "exit-code"
              "let graphExitCode, graphStdout, graphStderr = runAuthoritativeEvidence \"EvidenceGraph\" featureDir true"
              "if graphExitCode <> 0 then"
              "runAuthoritativeEvidence \"EvidenceAudit\" featureDir false"
              "readiness-contract"
              "synthetic-evidence"
              "unsupported-host-classification"
              "EvidenceAudit\", [ \"EvidenceGraph\" ]" ]
            |> List.iter (fun required ->
                Expect.stringContains build required $"generated evidence target contract includes {required}")

            [ "type GeneratedEvidenceCommandReport"
              "Command: string"
              "Target: string"
              "GeneratedAppIdentity: string"
              "Authority: string"
              "Status: string"
              "ExitCode: int"
              "ValidationArea: string"
              "ReportPath: string"
              "Diagnostics: string list" ]
            |> List.iter (fun required ->
                Expect.stringContains evidenceCommands required $"generated evidence command report includes {required}")
        }

        test "generated normal launch stays separate from evidence graph and audit commands" {
            let program = read "template/base/src/Product/Program.fs"
            let defaultBranch = program.Substring(program.IndexOf("| _ ->", System.StringComparison.Ordinal))

            Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost" "normal launch remains the persistent interactive host"

            [ "EvidenceGraph"
              "EvidenceAudit"
              "runGeneratedEvidenceGraph"
              "runGeneratedEvidenceAudit"
              "readiness/evidence-graph"
              "readiness/evidence-audit" ]
            |> List.iter (fun forbidden ->
                Expect.isFalse (defaultBranch.Contains(forbidden, System.StringComparison.OrdinalIgnoreCase)) $"normal launch must not run evidence command surface {forbidden}")
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
