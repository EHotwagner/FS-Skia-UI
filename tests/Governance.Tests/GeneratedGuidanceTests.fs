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
                  "Charts"
                  "./fake.sh build -t Dev"
                  "./fake.sh build -t Test"
                  "./fake.sh build -t Verify" ]

            let readme = read "template/base/README.md"
            Expect.isFalse (readme.Contains("V2Analysis")) "product README omits framework V2 analysis"
            Expect.isFalse (readme.Contains("subsystem design", System.StringComparison.OrdinalIgnoreCase)) "product README omits framework subsystem design"
        }
    ]
