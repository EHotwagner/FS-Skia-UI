module GeneratedGuidanceTests

open Expecto
open GovernanceTestSupport

let requiredSpecPrompts =
    [ "package impact"
      "public contract impact"
      "state workflow impact"
      "layout/rendering impact"
      "evidence obligations"
      "unsupported scope"
      "build-target impact" ]

let requiredPlanPrompts =
    [ "template ownership"
      "dependency impact"
      "command-surface impact"
      "generated project impact"
      "evidence paths"
      ".fsi"
      "MVU/effect boundary"
      "synthetic evidence"
      "test evidence"
      "observability"
      "deferred scope" ]

let specTemplates =
    [ ".specify/templates/spec-template.md"
      ".specify/presets/fsharp-opinionated/templates/spec-template.md" ]

let planTemplates =
    [ ".specify/templates/plan-template.md"
      ".specify/presets/fsharp-opinionated/templates/plan-template.md" ]

let selectedPersistentLaunchContract =
    "Viewer.runApp viewerOptions Product.Program.generatedHost"

let unqualifiedSelectedPersistentLaunchContract =
    "Viewer.runApp viewerOptions generatedHost"

let alternatePersistentLaunchContracts =
    [ "Viewer.runAppWithWindowBehavior viewerOptions windowBehavior generatedHost"
      "Viewer.runAppWithWindowBehavior viewerOptions windowBehaviorRequest generatedHost" ]

[<Tests>]
let generatedGuidanceTests =
    testList "Generated guidance hardening" [
        test "active and preset spec templates include prompts in the governance section" {
            specTemplates
            |> List.iter (fun template ->
                requiredSpecPrompts
                |> List.iter (expectPromptInSection template "Framework Governance Prompts"))
        }

        test "active and preset plan templates include decisions in the repository governance section" {
            planTemplates
            |> List.iter (fun template ->
                requiredPlanPrompts
                |> List.iter (expectPromptInSection template "Repository Governance Decisions"))
        }

        test "section parser rejects prompts that appear only in deferred scope" {
            let fixture =
                "# Spec\n\n## Requirements\n\n### Framework Governance Prompts\n\n- **Package impact**: present\n\n## Deferred Roadmap\n\npublic contract impact\n"

            let governance = requireSection "Framework Governance Prompts" fixture "fixture"
            Expect.isFalse (governance.Content.ToLowerInvariant().Contains("public contract impact")) "wrong-section prompt does not satisfy governance section"
        }

        test "Generated guidance scanner Synthetic rejects reflection-first package discovery fixtures" {
            use fixture = new TempFixtureDirectory("generated-guidance-synthetic-reflection")

            // SYNTHETIC: malformed generated guidance intentionally recommends reflection and repository source copying; real evidence path is GeneratedGuidanceCheck on generated product guidance.
            let malformed =
                writeFixtureFile
                    fixture.Root
                    "template/base/docs/product.md"
                    "# Product guidance\n\nUse assembly reflection first to discover FS.Skia.UI APIs, then copy files from repository src/ when names are unclear.\n"

            let content = System.IO.File.ReadAllText malformed
            Expect.stringContains content "assembly reflection first" "synthetic fixture contains forbidden reflection-first advice"
            Expect.stringContains content "copy files from repository src/" "synthetic fixture contains forbidden repository-source-copy advice"

            let scanner = readBuildGovernanceSources ()

            [ "validateForbiddenGeneratedGuidanceAdvice"
              "reflection-first"
              "repository-source-copy"
              "package-reference alternative" ]
            |> List.iter (fun required ->
                Expect.stringContains scanner required $"GeneratedGuidanceCheck scanner rejects malformed advice with {required}")
        }

        test "API discovery feedback classification records required categories and next actions" {
            expectFileContains
                "specs/035-api-discovery-names/readiness/feedback-classification.md"
                [ "PackageDocumentationDiscoverability"
                  "PublicContractErgonomics"
                  "GeneratedTemplateWorkflow"
                  "ConsumerAuthoringGuidance"
                  "contract-change:"
                  "generated-guidance:"
                  "runtime-scope:"
                  "evidence-path:"
                  "next-action:"
                  "elapsed: 00:04:12" ]
        }

        test "generated guidance points to package references and Scene Controls qualification rules" {
            [ "template/base/docs/product.md"
              "template/base/README.md"
              "docs/reports/generated-apps.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "source-shaped package API reference"
                      "Do not use assembly reflection"
                      "repository source inspection"
                      "FS.Skia.UI.Scene.Rect"
                      "FS.Skia.UI.Scene.Paint"
                      "FS.Skia.UI.Scene.TextRun"
                      "FS.Skia.UI.Controls.TextBlock.create"
                      "FS.Skia.UI.Controls.TextBox.onChanged"
                      "FS.Skia.UI.Controls.Stack.children"
                      "namespace open order" ])
        }

        test "active and preset prompt classes remain semantically aligned" {
            let promptSet (template: string) (section: string) (prompts: string list) =
                let content = (requireSection section (read template) template).Content.ToLowerInvariant()

                prompts
                |> List.filter (fun prompt -> content.Contains(prompt.ToLowerInvariant()))
                |> Set.ofList

            Expect.equal
                (promptSet specTemplates[0] "Framework Governance Prompts" requiredSpecPrompts)
                (promptSet specTemplates[1] "Framework Governance Prompts" requiredSpecPrompts)
                "active and preset spec prompt classes match"

            Expect.equal
                (promptSet planTemplates[0] "Repository Governance Decisions" requiredPlanPrompts)
                (promptSet planTemplates[1] "Repository Governance Decisions" requiredPlanPrompts)
                "active and preset plan prompt classes match"
        }

        test "generated task guidance requires persistent viewer launch evidence separately from bounded helpers" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md" ]
            |> List.iter (fun template ->
                expectFileContains
                    template
                    [ "persistent graphical launch task"
                      "default executable"
                      "Bounded smoke, first-frame, frame-count, scene metadata"
                      "MUST NOT be described as completing interactive graphical readiness" ])
        }

        test "generated task guidance rejects print-only and bounded-only default graphical viewer paths" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md" ]
            |> List.iter (fun template ->
                expectFileContains
                    template
                    [ "MUST reject viewer-backed default executable paths"
                      "print metadata"
                      "count controls"
                      "run bounded smoke"
                      "emit scene evidence"
                      "exit without a persistent launch attempt" ])
        }

        test "generated task guidance requires implementation workflow evidence records" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md" ]
            |> List.iter (fun template ->
                expectFileContains
                    template
                    [ "implementation batch records"
                      "red-green evidence log"
                      "graph before/after"
                      "before and after every status change"
                      "skill-loading notes"
                      "persistent launch rules"
                      "non-authoritative aggregate reporting"
                      "non-authoritative aggregate" ])
        }

        test "implementation guidance requires pre-work skill loading and graph refresh evidence" {
            [ ".agents/skills/speckit-implement/SKILL.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.implement.md" ]
            |> List.iter (fun guidance ->
                expectFileContains
                    guidance
                    [ "readiness/skill-loading-evidence.md"
                      "loaded_at"
                      "work_started_at"
                      "reviewer exception"
                      "graph before/after"
                      "before and after every status change"
                      "red-green evidence log"
                      "implementation batch records" ])
        }

        test "Spec Kit docs distinguish V2 obligations from deferred roadmap work" {
            expectFileContains
                "docs/reports/speckit.md"
                [ "package impact"
                  "template ownership"
                  "Deferred Roadmap"
                  "visual evidence"
                  "release validation"
                  "external repository split"
                  "distribution automation" ]
        }

        test "V3 generated product docs describe selected capabilities without framework architecture copy" {
            expectFileContains
                "template/base/README.md"
                [ "generated product"
                  "Scene"
                  "SkiaViewer"
                  "Elmish"
                  "KeyboardInput"
                  "Layout"
                  "Controls"
                  "./fake.sh build -t Dev"
                  "./fake.sh build -t Test"
                  "./fake.sh build -t Verify" ]

            let readme = read "template/base/README.md"
            Expect.isFalse (readme.Contains("V2Analysis")) "product README omits framework V2 analysis"
            Expect.isFalse (readme.Contains("subsystem design", System.StringComparison.OrdinalIgnoreCase)) "product README omits framework subsystem design"
        }

        test "generated app guidance names qualified app-owned scene host and update values" {
            [ "docs/reports/generated-apps.md"
              "docs/reports/testing.md"
              "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "Product.Program.view"
                      "Product.Program.generatedHost"
                      "Product.Program.update"
                      "FS.Skia.UI.Scene.Scene" ])
        }

        test "generated app source and tests keep public contract names app-qualified" {
            expectFileContains
                "template/base/src/Product/Program.fs"
                [ "module Product.Program"
                  "let view"
                  "let generatedHost"
                  "let update" ]

            expectFileContains
                "template/base/tests/Product.Tests/Tests.fs"
                [ "Product.Program.view"
                  "Product.Program.generatedHost"
                  "Product.Program.update" ]
        }

        test "generated guidance keeps app commands distinct from viewer effects" {
            [ "docs/reports/generated-apps.md"
              "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "DispatchHostCommand"
                      "RenderScene"
                      "viewer effects"
                      "app command" ])

            expectFileContains
                "template/base/src/Product/Program.fs"
                [ "let interpretAtHostBoundary"
                  "let viewerEffectsForModel"
                  "let appCommandName" ]

            expectFileContains
                "template/base/src/Product/EvidenceCommands.fs"
                [ "let interpretAtHostBoundary"
                  "let viewerEffectsForModel"
                  "let appCommandName"
                  "RenderScene(view model)" ]

            let source = read "template/base/src/Product/EvidenceCommands.fs"
            Expect.isFalse (source.Contains("[ DispatchHostCommand \"save:Product\"; RenderScene", System.StringComparison.Ordinal)) "generated viewer effects are not appended to app command lists"
        }

        test "generated guidance reuses shared Scene geometry instead of local duplicates" {
            [ "template/fragments/scene/README.md"
              "docs/reports/generated-apps.md"
              "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "shared Scene geometry"
                      "layout"
                      "collision" ])

            let source = read "template/base/src/Product/Program.fs"
            [ "type LocalGeometry"
              "type LocalBounds"
              "type CollisionRect"
              "type RenderBounds" ]
            |> List.iter (fun duplicateType ->
                Expect.isFalse (source.Contains(duplicateType, System.StringComparison.Ordinal)) $"generated source does not introduce duplicate geometry record {duplicateType}")
        }

        test "generated guidance recommends domain geometry names and not primitive collisions" {
            let guidancePaths =
                [ "docs/reports/generated-apps.md"
                  "template/base/docs/product.md"
                  "template/fragments/scene/README.md" ]

            let combined =
                guidancePaths
                |> List.map read
                |> String.concat System.Environment.NewLine

            [ "WorldRect"; "WorldPoint"; "TrackBounds" ]
            |> List.iter (fun required ->
                Expect.stringContains combined required $"generated guidance includes domain geometry example {required}")

            [ "domain Rect"
              "domain Point"
              "domain Size"
              "app Rect"
              "app Point"
              "app Size" ]
            |> List.iter (fun stalePattern ->
                Expect.isFalse (combined.Contains(stalePattern, System.StringComparison.OrdinalIgnoreCase)) $"generated guidance rejects stale primitive collision pattern {stalePattern}")
        }

        test "generated screenshot wording requires live-window proof and keeps deterministic fallback separate" {
            [ "docs/reports/generated-apps.md"
              "docs/reports/evidence.md"
              "template/base/docs/product.md"
              "template/fragments/skiaviewer/README.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "live viewer-window capture"
                      "first-frame"
                      "evidence-kind=screenshot"
                      "deterministic-scene-evidence"
                      "must not claim screenshot proof" ])
        }

        test "generated game guidance qualifies CloseRequested and converts app vectors to scene points" {
            [ "template/base/docs/product.md"
              "template/fragments/scene/README.md"
              "docs/reports/generated-apps.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "Product.Program.Msg.CloseRequested"
                      "CloseRequested"
                      "app-owned message"
                      "toScenePoint"
                      "domain vector"
                      "Scene.Point"
                      "explicit conversion" ])
        }

        test "generated evidence guidance separates semantic scene facts from screenshot and pixel-readback claims" {
            [ "template/base/docs/product.md"
              "template/fragments/testing/README.md"
              "docs/reports/evidence.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "semantic scene facts"
                      "deterministic-scene-evidence"
                      "does not prove semantic object presence"
                      "lander"
                      "terrain"
                      "landing pad"
                      "HUD metrics"
                      "pixel-readback fallback"
                      "fallback-reason"
                      "proves-screenshot=false" ])
        }

        test "generated screenshot evidence command writes capability detail fields" {
            let source = read "template/base/src/Product/EvidenceCommands.fs"

            [ "Viewer.captureScreenshotEvidence"
              "evidenceField \"viewer-open-status\""
              "evidenceField \"first-frame-status\""
              "evidenceField \"capture-availability\""
              "evidenceField \"capture-source\""
              "evidenceField \"deterministic-fallback-kind\""
              "evidenceField \"proves-screenshot\"" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"generated screenshot evidence command includes {required}")

            Expect.isFalse (source.Contains("evidenceField \"capture-source\" \"pixel-readback\"", System.StringComparison.Ordinal)) "pixel readback is not relabeled as screenshot proof"
            Expect.isFalse (source.Contains("evidenceField \"capture-source\" \"deterministic-scene-render\"\n              evidenceField \"proves-screenshot\" \"true\"", System.StringComparison.Ordinal)) "deterministic scene fallback is not relabeled as screenshot proof"
        }

        test "generated Linux detached launch guidance preserves logs stderr and stdin detachment" {
            [ "docs/reports/generated-apps.md"
              "template/base/docs/product.md"
              "template/fragments/skiaviewer/README.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "setsid"
                      "> readiness/logs/"
                      "2>&1"
                      "< /dev/null"
                      "&" ])

            let combined =
                [ "docs/reports/generated-apps.md"
                  "template/base/docs/product.md"
                  "template/fragments/skiaviewer/README.md" ]
                |> List.map read
                |> String.concat System.Environment.NewLine

            [ "nohup dotnet run"
              "dotnet run &"
              "simple backgrounding" ]
            |> List.iter (fun stalePattern ->
                Expect.isFalse (combined.Contains(stalePattern, System.StringComparison.OrdinalIgnoreCase)) $"generated guidance rejects stale detached launch pattern {stalePattern}")
        }

        test "generated viewer guidance scans one selected persistent launch contract across owned artifacts" {
            [ "docs/reports/generated-apps.md"
              "template/fragments/skiaviewer/README.md"
              "specs/022-breakout-demo-feedback/quickstart.md" ]
            |> List.iter (fun path ->
                let content = read path
                Expect.stringContains content selectedPersistentLaunchContract $"{path} names the selected persistent launch contract"

                alternatePersistentLaunchContracts
                |> List.iter (fun alternate ->
                    Expect.isFalse (content.Contains(alternate)) $"{path} does not drift to alternate persistent launch contract {alternate}"))

            let source = read "template/base/src/Product/Program.fs"
            let tests = read "template/base/tests/Product.Tests/Tests.fs"

            Expect.stringContains source unqualifiedSelectedPersistentLaunchContract "generated source calls the selected persistent launch contract"
            Expect.stringContains tests unqualifiedSelectedPersistentLaunchContract "generated tests assert the selected persistent launch contract"

            alternatePersistentLaunchContracts
            |> List.iter (fun alternate ->
                Expect.isFalse (source.Contains(alternate)) $"generated source does not retain alternate launch contract {alternate}"
                Expect.isFalse (tests.Contains(alternate)) $"generated tests do not retain alternate launch contract {alternate}")
        }

        test "generated viewer guidance readiness report records selected contract and distinct evidence kinds" {
            let report = read "specs/022-breakout-demo-feedback/readiness/generated-viewer-guidance.md"

            [ "package-version="
              "selected-entry-point=Viewer.runApp"
              "selected-contract=Viewer.runApp viewerOptions Product.Program.generatedHost"
              "files-scanned="
              "deterministic-render-evidence="
              "persistent-launch-evidence="
              "screenshot-evidence=" ]
            |> List.iter (fun required ->
                Expect.stringContains report required $"generated viewer guidance report includes {required}")
        }

        test "generated guidance contains compact consumer API map before coding" {
            [ "docs/reports/generated-apps.md"
              "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "Compact Consumer API Map"
                      "FS.Skia.UI.KeyboardInput.ViewerKey"
                      "ArrowLeft"
                      "ArrowRight"
                      "Escape"
                      "ViewerKeyboard.normalize"
                      "Viewer.GeneratedAppHost"
                      "ShouldClose"
                      "OpenWindow"
                      "RenderScene"
                      "CloseWindow"
                      "DispatchViewer"
                      "Scene.rectangle"
                      "Scene.textRun"
                      "explicit fonts"
                      "brand or typography" ])
        }

        test "readiness guidance names feature scoped contracts and mandatory terms" {
            [ "docs/reports/generated-apps.md"
              "template/base/docs/product.md"
              "docs/reports/evidence.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "specs/032-sokoban-feedback-followups/readiness/"
                      "default-text-glyph-capture.md"
                      "interactive-window-close-evidence.md"
                      "consumer-guidance-scan.md"
                      "readiness-contract-scan.md"
                      "task-guidance-scan.md"
                      "governance risk levels"
                      "aggregate hang diagnostics"
                      "runtime limitations"
                      "supported-host persistent launch evidence" ])
        }

        test "task generation guidance names free-form titles, owns ownership, and dependency shape" {
            // Feature 059 (FR-009/FR-010): the title-trigger pitfall prose is replaced by
            // free-form-title + structured `owns:` ownership guidance.
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
              ".agents/skills/speckit-tasks/SKILL.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "free-form"
                      "owns"
                      "one key per task id"
                      "wrapper"
                      "tasks.deps.yml"
                      "object shape"
                      "indented"
                      "deps"
                      "skillist"
                      "[skillist: ...]" ])
        }
    ]
