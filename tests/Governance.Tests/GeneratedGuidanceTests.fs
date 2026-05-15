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

[<Tests>]
let generatedGuidanceTests =
    testList "Generated guidance hardening" [
        test "active and preset spec templates include V2 prompts" {
            [ ".specify/templates/spec-template.md"
              ".specify/presets/fsharp-opinionated/templates/spec-template.md" ]
            |> List.iter (fun template ->
                let content = read template |> fun text -> text.ToLowerInvariant()
                requiredSpecPrompts
                |> List.iter (fun prompt -> Expect.stringContains content prompt $"{template} contains {prompt}"))
        }

        test "active and preset plan templates include V2 planning decisions" {
            [ ".specify/templates/plan-template.md"
              ".specify/presets/fsharp-opinionated/templates/plan-template.md" ]
            |> List.iter (fun template ->
                let content = read template |> fun text -> text.ToLowerInvariant()
                requiredPlanPrompts
                |> List.iter (fun prompt -> Expect.stringContains content (prompt.ToLowerInvariant()) $"{template} contains {prompt}"))
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
    ]
