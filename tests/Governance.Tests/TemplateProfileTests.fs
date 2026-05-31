module TemplateProfileTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let templateProfileTests =
    testList "Template profile metadata" [
        test "template identity and profile choices are declared" {
            if fileExists ".template.config/template.json" then
                let content = read ".template.config/template.json"

                [ "\"shortName\": \"fs-skia-ui\""
                  "\"identity\": \"FS.Skia.UI.Template\""
                  "\"profile\""
                  "\"defaultValue\": \"app\""
                  "\"choice\": \"app\""
                  "\"choice\": \"headless-scene\""
                  "\"choice\": \"governed\""
                  "\"choice\": \"sample-pack\""
                  "\"rootNamespace\""
                  "\"packagePrefix\""
                  "\"authors\""
                  "\"repositoryUrl\""
                  "\"targetFramework\""
                  "\"skipGitInit\""
                  "\"postActions\""
                  ".template.config/generated/"
                  "chmod +x"
                  "git init"
                  "git add ."
                  "commit --allow-empty"
                  "[Spec Kit] Initial commit"
                  "3A7C4B45-1F5D-4A30-959A-51B88E82B5D2" ]
                |> List.iter (fun needle -> Expect.stringContains content needle $"template.json contains {needle}")
            else
                Expect.isFalse (directoryExists ".template.config") "generated projects do not carry template metadata"
        }

        test "dotnet template sources V3 product base and selected capability skills" {
            if fileExists ".template.config/template.json" then
                expectFileContains
                    ".template.config/template.json"
                    [ "template/base/"
                      ".specify/"
                      ".agents/skills/"
                      "memory/constitution.md"
                      "template/product-skills/fs-skia-scene/"
                      "template/product-skills/fs-skia-skiaviewer/"
                      "template/product-skills/fs-skia-elmish/"
                      "template/product-skills/fs-skia-keyboard-input/"
                      "template/product-skills/fs-skia-ui-widgets/"
                      "template/product-skills/fs-skia-testing/"
                      "template/fragments/samples/"
                      "fs-skia-samples" ]
            else
                Expect.isFalse (directoryExists "specs/007-v2-template-packaging") "generated projects exclude source-only history"
        }

        test "generated AGENTS guidance is not tied to source active feature" {
            if fileExists ".template.config/generated/AGENTS.md" then
                let content = read ".template.config/generated/AGENTS.md"
                Expect.stringContains content "specs/<feature>/plan.md" "generated AGENTS points at generated feature plans"
                Expect.isFalse (content.Contains "specs/008-targeted-refactor-governance") "generated AGENTS omits source-only active feature"
            else
                Expect.isFalse (directoryExists ".template.config") "generated projects do not carry template metadata"
        }

        test "V3 capability catalog has required default app capabilities" {
            Expect.isTrue (fileExists "template/capabilities.yml") "template/capabilities.yml exists"
            let capabilities = readCatalogCapabilities ()
            let defaultApp = capabilities |> List.filter _.DefaultApp |> List.map _.DisplayName |> Set.ofList

            Expect.equal
                defaultApp
                (Set.ofList [ "Scene"; "SkiaViewer"; "Elmish"; "KeyboardInput"; "Layout"; "Controls" ])
                "default app resolves to the required V3 capability set"

            Expect.isFalse (defaultApp.Contains "Samples") "samples are excluded from the default app"
        }

        test "V3 capability catalog rows have ownership metadata and closed dependencies" {
            let capabilities = readCatalogCapabilities ()
            let ids = capabilities |> List.map _.Id |> Set.ofList

            [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "controls"; "testing"; "samples" ]
            |> List.iter (fun id -> Expect.isTrue (ids.Contains id) $"catalog contains {id}")

            capabilities
            |> List.iter (fun capability ->
                Expect.isNonEmpty capability.DisplayName $"{capability.Id} has displayName"
                Expect.isTrue (capability.Project.IsSome || capability.NonRuntime) $"{capability.Id} declares runtime project or non-runtime marker"
                Expect.isNonEmpty capability.Contracts $"{capability.Id} declares contracts or no-public-surface record"
                Expect.isNonEmpty capability.Tests $"{capability.Id} declares tests"
                Expect.isSome capability.Skill $"{capability.Id} declares skill"
                Expect.isSome capability.TemplateFragment $"{capability.Id} declares template fragment"
                Expect.isNonEmpty capability.Profiles $"{capability.Id} declares profiles"
                Expect.isNonEmpty capability.Evidence $"{capability.Id} declares evidence classes"
                capability.Dependencies
                |> List.iter (fun dep -> Expect.isTrue (ids.Contains dep) $"{capability.Id} dependency {dep} exists"))

            let catalog = read "template/capabilities.yml"

            [ "ownerNotes"
              "docs:"
              "surfaceBaseline:"
              "validation paths"
              "FS.Skia.UI.Scene"
              "FS.Skia.UI.SkiaViewer"
              "FS.Skia.UI.Elmish"
              "FS.Skia.UI.KeyboardInput"
              "FS.Skia.UI.Layout"
              "FS.Skia.UI.Controls"
              "FS.Skia.UI.Testing" ]
            |> List.iter (fun needle -> Expect.stringContains catalog needle $"catalog includes {needle}")
        }

        test "Controls capability is active home for controls rich text charts graph and DataGrid" {
            let capabilities = readCatalogCapabilities ()
            let ids = capabilities |> List.map _.Id |> Set.ofList

            Expect.isFalse (ids.Contains "charts") "no generated charts capability remains active"

            let controls = capabilities |> List.find (fun capability -> capability.Id = "controls")
            let ownerNotes = defaultArg controls.OwnerNotes ""

            [ "src/Controls/Types.fsi"
              "src/Controls/Control.fsi"
              "src/Controls/RichText.fsi"
              "src/Controls/Charts.fsi"
              "src/Controls/DataGrid.fsi"
              "src/Controls/CustomControl.fsi" ]
            |> List.iter (fun contract ->
                Expect.contains controls.Contracts contract $"Controls capability declares {contract}")

            [ "ordinary controls"
              "rich text"
              "chart controls"
              "graph controls"
              "DataGrid" ]
            |> List.iter (fun phrase ->
                Expect.stringContains ownerNotes phrase $"Controls capability owner notes mention {phrase}")

            [ "app"; "governed"; "sample-pack" ]
            |> List.iter (fun profile ->
                Expect.contains controls.Profiles profile $"Controls capability participates in {profile} profile")

            expectFileContains "template/profiles/app.yml" [ "controls" ]
            Expect.isFalse ((read "template/profiles/app.yml").Contains("charts", System.StringComparison.OrdinalIgnoreCase)) "app profile has no charts capability"
        }

        test "V3 profile rows are present for source and package validation" {
            expectPathsExist
                "V3 profiles"
                [ "template/profiles/app.yml"
                  "template/profiles/headless-scene.yml"
                  "template/profiles/governed.yml"
                  "template/profiles/sample-pack.yml" ]

            [ "template/profiles/app.yml", [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "controls"; "full-governance" ]
              "template/profiles/headless-scene.yml", [ "scene" ]
              "template/profiles/governed.yml", [ "full-governance" ]
              "template/profiles/sample-pack.yml", [ "samples" ] ]
            |> List.iter (fun (path, required) -> expectFileContains path required)
        }

        test "V3 capability dependencies resolve prerequisites without cycles" {
            let capabilities = readCatalogCapabilities ()
            let byId = capabilities |> List.map (fun capability -> capability.Id, capability) |> Map.ofList

            let rec resolve seen capabilityId =
                if Set.contains capabilityId seen then
                    seen
                else
                    let capability = byId[capabilityId]
                    capability.Dependencies |> List.fold resolve (Set.add capabilityId seen)

            Expect.equal (resolve Set.empty "elmish") (Set.ofList [ "scene"; "skiaviewer"; "elmish" ]) "elmish pulls viewer and scene"
            Expect.equal (resolve Set.empty "samples") (Set.ofList [ "scene"; "skiaviewer"; "elmish"; "samples" ]) "samples pull prerequisites"

            capabilities
            |> List.iter (fun capability ->
                let closure = resolve Set.empty capability.Id
                Expect.equal closure.Count (Set.count closure) $"{capability.Id} closure is deterministic")
        }

        test "V3 profiles keep samples explicit" {
            let app = read "template/profiles/app.yml"
            let samplePack = read "template/profiles/sample-pack.yml"

            Expect.isFalse (app.Contains("samples: true")) "app profile does not include samples"
            Expect.isFalse (app.Contains("[samples]")) "app profile does not select samples"
            Expect.stringContains samplePack "samples: true" "sample-pack includes samples"
            Expect.stringContains samplePack "samples" "sample-pack selects samples"
        }
    ]
