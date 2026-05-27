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
                "docs/speckit.md"
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
            [ "docs/generated-apps.md"
              "docs/testing.md"
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
    ]
